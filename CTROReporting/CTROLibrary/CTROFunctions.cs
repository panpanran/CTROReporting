using System;
using System.Data;
using System.Data.OleDb;
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

        public static DataSet ReadExcelToDataSet(string pathname)
        {
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes'", pathname);


            DataSet data = new DataSet();

            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    var dataTable = new System.Data.DataTable();
                    dataTable.TableName = sheetName;
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                }
            }

            return data;
        }

        private static string[] GetExcelSheetNames(string connectionString)
        {
            System.Data.DataTable dt = null;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheetNames = new String[dt.Rows.Count];
                int i = 0;

                foreach (DataRow row in dt.Rows)
                {
                    excelSheetNames[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
                return excelSheetNames;
            }
        }
    }

}
