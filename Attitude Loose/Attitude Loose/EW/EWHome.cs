using Attitude_Loose.Models;
using Attitude_Loose.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace Attitude_Loose.EW
{
    public interface IEWHome<T> where T : class
    {
        void Update(T tablename);
        T GetById(string id);
        IEnumerable<string> GetIDList(string where);
    }

    public class EWFormatOriginalIncomingEmail : EWTicket
    {
        public override void Update(Ticket ticket)
        {
            string originalincomingemail = Regex.Match(ticket.Original_incoming_email, "<.*?>").Value.Replace("<", "").Replace(">", "");
            if (string.IsNullOrEmpty(originalincomingemail))
            {
                originalincomingemail = Regex.Match(ticket.Original_incoming_email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Value;
            }
            //string fullname = "";

            //if (ticket.Original_incoming_email.Contains("\""))
            //{
            //    fullname = Regex.Match(ticket.Original_incoming_email, ", .*?\"").Value.Replace(", ", "").Replace("\\\"", "") + " " + Regex.Match(ticket.Original_incoming_email, "\".*?,").Value.Replace("\"", "").Replace(",", "");
            //}
            //else
            //{
            //    fullname = Regex.Match(ticket.Original_incoming_email, ".*?<").Value.Replace("<", "");
            //}
            string url = null;
            if (!string.IsNullOrEmpty(originalincomingemail))
            {
                EWParticipant ewparticipant = new EWParticipant();
                string temp = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=panr2&$password=Rp0126$$&$table=participant&$lang=en&where=email=%27" + originalincomingemail + "%27";
                string participanttext = ewparticipant.GetIDList("email=%27" + originalincomingemail + "%27").ToArray()[1];
                string id = GetValueByFieldName("EWREST_id_0", participanttext.Replace(" ", ""));
                if (!string.IsNullOrEmpty(id))
                {
                    Participant participant = ewparticipant.GetById(id);
                    url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Rp0126$$&$lang=en&id=" + ticket.TicketId
                        + "&original_incoming_email=" + originalincomingemail
                        + "&work_phone=" + participant.Phone
                        + "&email=" + originalincomingemail
                        + "&organization_name=" + participant.Organization
                        + "&full_name=" + participant.FullName;
                }
            }

            if (!string.IsNullOrEmpty(url))
            {
                string html = CTRPFunctions.GetHTMLByUrl(url);
            }
            else
            {
                url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Rp0126$$&$lang=en&id=" + ticket.TicketId + "&full_name=Please Create New User";
                string html = CTRPFunctions.GetHTMLByUrl(url);
            }
        }


        public void BulkUpdate(string where)
        {
            Update(GetById("80100"));

            //string[] ticketlist = GetIDList(where).ToArray();
            //for (int i = 1; i < ticketlist.Length - 1; i++)
            //{
            //    string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
            //    Update(GetById(id));
            //}
        }
    }

}