using CTRPReporting.Models;
using CTRPReporting.CTRO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CTRPReporting.EW
{
    public class EWTicket : IEWHome<Ticket>
    {
        public IEnumerable<string> GetIDList(string where)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=panr2&$password=Prss_1234&$table=ctro_tickets&$lang=en&where=" + where;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = html.Split(stringSeparators, StringSplitOptions.None);
            return lines;
        }

        public Ticket GetById(string id)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWRead?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + id;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string fullname = GetValueByFieldName("EWREST_full_name", html);
            string email = GetValueByFieldName("EWREST_email", html);
            string summary = GetValueByFieldName("EWREST_summary", html);
            string assigned_to_ = GetValueByFieldName("EWREST_assigned_to_", html);
            string state = GetValueByFieldName("EWREST_state", html);
            string original_incoming_email = GetValueByFieldName("EWREST_original_incoming_email", html);
            string organization_name = GetValueByFieldName("EWREST_organization_name", html);
            string internal_analysis = GetValueByFieldName("EWREST_internal_analysis", html);
            Ticket ticket = new Ticket
            {
                TicketId = id,
                FullName = fullname,
                Email = email,
                Summary = summary,
                AssignedTo = assigned_to_,
                State = state,
                Original_incoming_email = original_incoming_email,
                OrganizationName = organization_name,
                Internal_analysis = internal_analysis
            };
            return ticket;
        }

        public string GetValueByFieldName(string fieldname, string content)
        {
            string pattern = fieldname + @"='(.*?)';";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = rgx.Match(content);
            string value = match.Value.Replace(fieldname + "='", "").Replace("';", "");
            return value;
        }

        public virtual void Update(Ticket ticket)
        { }

    }

    public class EWTSRFeedback : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            string content = @"From: Pan, Ran (NIH/NCI) [C]
   To: emailRep
   Subject: summaryRep

   Hi nameRep,

   Thank you for verifying the TSR.

   Best,

   Ran Pan";
            content = content.Replace("emailRep", ticket.Email)
                .Replace("summaryRep", ticket.Summary)
                .Replace("nameRep", ticket.FullName);
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }


        public void BulkUpdate(string where)
        {
            //Update(GetById("82066"));

            string[] ticketlist = GetIDList(where).ToArray();
            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Update(GetById(id));
            }
        }

        public void UpdateByID(string id)
        {
            Update(GetById(id));
        }
    }

    public class EWContinueReview : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            //string content = @"Board Approval Number updated to 05/31/2018, uploaded Continuing Review document. ";
            //string content = @"Trial is out of scope for CTRP. No action required.";
            string content = @"No action required, trial present in PA as NCI-2014-01029 is a CCR/CTEP trial and all updates should be submitted via CTEP services. ";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }


        public void BulkUpdate(string where)
        {
            Update(GetById("80338"));

            //string[] ticketlist = GetIDList(where).ToArray();
            //for (int i = 1; i < ticketlist.Length - 1; i++)
            //{
            //    string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ",""));
            //    Update(GetById(id));
            //}
        }
    }

    public class EWZeroaccrual : EWTicket
    {
        public List<Ticket> GetTickets(string where)
        {
            List<Ticket> tickets = new List<Ticket>();
            string[] ticketlist = GetIDList(where).ToArray();
            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                tickets.Add(GetById(id));
            }
            return tickets;
        }

        public override void Update(Ticket ticket)
        {
            string content = @"From: Pan, Ran (NIH/NCI) [C]
   To: emailRep
   Subject: summaryRep

   Hi nameRep,

   Thank you for verifying the TSR.

   Best,

   Ran Pan";
            content = content.Replace("emailRep", ticket.Email)
                .Replace("summaryRep", ticket.Summary)
                .Replace("nameRep", ticket.FullName);
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void AssignedTo(Ticket ticket)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_1234&$lang=en&id=" + ticket.TicketId + "&description=" + ticket.Summary;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void BulkClose(string where)
        {
            string[] ticketlist = GetIDList(where).ToArray();
            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Update(GetById(id));
            }
        }

        public void BulkAssigneTo(string where)
        {
            string[] ticketlist = GetIDList(where).ToArray();
            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(id);
                if (ticket.State == "Open")
                {
                    AssignedTo(ticket);
                }
            }
        }
    }
}