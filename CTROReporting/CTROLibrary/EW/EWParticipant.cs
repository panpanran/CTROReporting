using CTROLibrary.Model;
using CTROLibrary.CTRO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace CTROLibrary.EW
{
    public class EWParticipant : IEWHome<Participant>
    {
        public IEnumerable<string> GetIDList(string where)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=" + CTROConst.EW_ACCOUNT + "&$password=" + CTROConst.EW_PASSWORD + "&$table=participant&$lang=en&where=" + where;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = html.Split(stringSeparators, StringSplitOptions.None);
            return lines;
        }

        public Participant GetById(string id)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWRead?$KB=CBIIT&$table=participant&$login=" + CTROConst.EW_ACCOUNT + "&$password=" + CTROConst.EW_PASSWORD + "&$lang=en&id=" + id;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string fullname = GetValueByFieldName("EWREST_full_name", html);
            string email = GetValueByFieldName("EWREST_email", html);
            string organization = GetValueByFieldName("EWREST_organization_name", html);
            string phone = GetValueByFieldName("EWREST_work_phone", html);
            Participant participant = new Participant
            {
                ParticipantId = id,
                FullName = fullname,
                Email = email,
                Phone = phone,
                Organization = organization
            };
            return participant;
        }

        public string GetValueByFieldName(string fieldname, string content)
        {
            string pattern = fieldname + @"='(.*?)';";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = rgx.Match(content);
            string value = match.Value.Replace(fieldname + "='", "").Replace("';", "");
            return value;
        }

        public virtual void Update(Participant ticket)
        { }
    }
}
