using CTROReporting.Infrastructure;
using CTROReporting.Models;
using CTROReporting.CTRO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace CTROReporting.EW
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
            try
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
                    string temp = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=panr2&$password=Prss_1234&$table=participant&$lang=en&where=email=%27" + originalincomingemail + "%27";
                    string participanttext = ewparticipant.GetIDList("email=%27" + originalincomingemail + "%27 or alternate_email=%27" + originalincomingemail + "%27").ToArray()[1];
                    string id = GetValueByFieldName("EWREST_id_0", participanttext.Replace(" ", ""));
                    if (!string.IsNullOrEmpty(id))
                    {
                        Participant participant = ewparticipant.GetById(id);
                        url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId
                            + "&original_incoming_email=" + originalincomingemail
                            + "&work_phone=" + participant.Phone
                            + "&email=" + originalincomingemail
                            + "&organization_name=" + participant.Organization.Replace("&", "%26")
                            + "&full_name=" + participant.FullName;
                    }
                }

                if (!string.IsNullOrEmpty(url))
                {
                    string html = CTROFunctions.GetHTMLByUrl(url);
                }
                else
                {
                    url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&full_name=Please create a new user or add this email to the 'alternate email' of the existing user";
                    string html = CTROFunctions.GetHTMLByUrl(url);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        public void BulkUpdate(string[] ticketlist)
        {
            //Update(GetById("80100"));

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Update(GetById(id));
            }
        }
    }

    public class EWTriageAccrual : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            string description = "TriageAccrual";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&description=" + description;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }


        public void BulkUpdate(string[] ticketlist)
        {
            //Update(GetById("80100"));

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(id);
                if (ticket.State == "New Request")
                {
                    if (ticket.Internal_analysis.ToLower().Replace("closed to accrual", "").Replace("target accrual", "").Contains("accrual"))
                    {
                        if (ticket.Summary.ToLower().Contains("participating site") && !ticket.Summary.ToLower().Contains("accrual"))
                        { }
                        else
                        {
                            UpdateByID(id);
                        }
                    }
                }
            }
        }

        public void UpdateByID(string id)
        {
            Update(GetById(id));
        }
    }

    public class EWTriageClinicalTrialsDotGov : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            string description = "TriageClinicalTrialsDotGov";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&description=" + description;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void BulkUpdate(string[] ticketlist)
        {
            //Update(GetById("80100"));

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(id);
                if (ticket.State == "New Request")
                {
                    if (ticket.OrganizationName == "NIH, National Library of Medicine (NLM)" || ticket.Summary.Contains("CTEP Study Results Review"))
                    {
                        UpdateByID(id);
                    }
                }
            }
        }

        public void UpdateByID(string id)
        {
            Update(GetById(id));
        }
    }

    public class EWTriageScientific : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            string description = "TriageScientific";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&description=" + description;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void BulkUpdate(string[] ticketlist)
        {
            //Update(GetById("80100"));

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(id);
                if (ticket.State == "New Request")
                {
                    if (ticket.Summary.Contains("Trial Comparison Document Review"))
                    {
                        UpdateByID(id);
                    }
                }
            }
        }

        public void UpdateByID(string id)
        {
            Update(GetById(id));
        }
    }

    public class EWTriageTSRFeedback : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            string description = "TriageTSRFeedback";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&description=" + description;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void BulkUpdate(string[] ticketlist)
        {
            //Update(GetById("80100"));

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(id);
                if (ticket.State == "New Request")
                {
                    if (ticket.Summary.ToLower().Contains("trial amendment tsr for review") ||
                        ticket.Summary.ToLower().Contains("trial files attached for review") ||
                        ticket.Summary.ToLower().Contains("feedback for") ||
                        ticket.Summary.ToLower().Contains("tsr update"))
                    {
                        UpdateByID(id);
                    }
                }
            }
        }

        public void UpdateByID(string id)
        {
            Update(GetById(id));
        }
    }

    public class EWTriageOnHoldTrials : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            string description = "TriageOnHoldTrials";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&description=" + description;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void BulkUpdate(string[] ticketlist)
        {
            //Update(GetById("80100"));

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(id);
                if (ticket.State == "New Request")
                {
                    if (ticket.Summary.Contains("Trial PROCESSING ON HOLD"))
                    {
                        UpdateByID(id);
                    }
                }
            }
        }

        public void UpdateByID(string id)
        {
            Update(GetById(id));
        }
    }

}