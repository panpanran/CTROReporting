using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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


        public static T GetDataFromJson<T>(string servicename, string methodname, string para = "default")
        {
            StringBuilder url = new StringBuilder("http://local.ctroreporting.com/api/" + servicename + "/" + methodname);
            if (para != "default")
            {
                url.Append("?" + para);
            }

            string json = new WebClient().DownloadString(url.ToString());
            var sdata = new JavaScriptSerializer().Deserialize<T>(json);
            return sdata;
        }

        public static async Task<Uri> CreateDataFromJson<T>(string servicename, string methodname, T data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://local.ctroreporting.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/" + servicename + "/" + methodname, data);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
    }
}
