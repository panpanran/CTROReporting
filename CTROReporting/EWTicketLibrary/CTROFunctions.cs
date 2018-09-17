using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CTROLibrary
{
    public class CTROFunctions
    {
        public static string GetHTMLByUrl(string url)
        {
            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                htmlCode = client.DownloadString(url);
            }
            return htmlCode;
        }

    }
}
