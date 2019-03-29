using CTROLibrary.Model;
using CTROLibrary.CTRO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Configuration;

namespace CTROLibrary.EW
{
    public class EWTicket : IEWHome<Ticket>
    {
        public IEnumerable<string> GetIDList(string where)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=" + CTROConst.EW_ACCOUNT + "&$password=" + CTROConst.EW_PASSWORD + "&$table=ctro_tickets&$lang=en&where=" + where;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = html.Split(stringSeparators, StringSplitOptions.None);
            return lines;
        }

        public Ticket GetById(string id)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWRead?$KB=CBIIT&$table=ctro_tickets&$login=" + CTROConst.EW_ACCOUNT + "&$password=" + CTROConst.EW_PASSWORD + "&$lang=en&id=" + id;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string fullname = GetValueByFieldName("EWREST_full_name", html);
            string email = GetValueByFieldName("EWREST_email", html);
            string summary = GetValueByFieldName("EWREST_summary", html);
            string assigned_to_ = GetValueByFieldName("EWREST_assigned_to_", html);
            string state = GetValueByFieldName("EWREST_state", html);
            string category = GetValueByFieldName("EWREST_category_0", html);
            string createddate = GetValueByFieldName("EWREST_created_date", html);
            string modifiedby = GetValueByFieldName("EWREST_modified_by", html);
            string modifieddate = GetValueByFieldName("EWREST_modified_date", html);
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
                Category = category,
                Created_date = createddate,
                Modified_by = modifiedby,
                Modified_date = modifieddate,
                Original_incoming_email = original_incoming_email,
                OrganizationName = organization_name,
                Internal_analysis = internal_analysis
            };
            return ticket;
        }

        public Ticket GetByUrl(string url)
        {
            string html = CTROFunctions.GetHTMLByUrl(url);
            string id = GetValueByFieldName("EWREST_id", html);
            string fullname = GetValueByFieldName("EWREST_full_name", html);
            string email = GetValueByFieldName("EWREST_email", html);
            string summary = GetValueByFieldName("EWREST_summary", html);
            string assigned_to_ = GetValueByFieldName("EWREST_assigned_to_", html);
            string state = GetValueByFieldName("EWREST_state", html);
            string category = GetValueByFieldName("EWREST_category_0", html);
            string createddate = GetValueByFieldName("EWREST_created_date", html);
            string modifiedby = GetValueByFieldName("EWREST_modified_by", html);
            string modifieddate = GetValueByFieldName("EWREST_modified_date", html);
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
                Category = category,
                Created_date = createddate,
                Modified_by = modifiedby,
                Modified_date = modifieddate,
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

}
