﻿using CTROLibrary.Model;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace CTROLibrary.CTRO
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        [JsonProperty(".issued")]
        public string issued { get; set; }
        [JsonProperty(".expires")]
        public string expires { get; set; }
    }

    public static class CTROFunctions
    {
        public static string ReadTxtFile(string fieldname, string path)
        {
            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(path))
            {
                // Read the stream to a string, and write the string to the console.
                string output = sr.ReadToEnd();
                string pattern = fieldname + @": (.*?)\r\n";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = rgx.Match(output);
                string value = match.Value.Replace(fieldname + ": ", "").Replace("\r\n", "");
                return value;
            }
        }

        public static bool PortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            int num = ipEndPoints.Where(x => x.Port == port).Count();
            return num > 0 ? true : false;
        }

        public static bool ConnectSSHCTRP()
        {
            if (PortInUse(5434))
            {
                return false;
            }

            Chilkat.SshTunnel tunnel = new Chilkat.SshTunnel();
            tunnel.DestHostname = "ctrp-prod-aurora-cluster.cluster-ro-cbefrrmwjslt.us-east-1.rds.amazonaws.com";
            tunnel.DestPort = 5432;
            tunnel.AcceptLogPath = @"C:\Users\panr2\Downloads\CSharp\CTROReporting\CTROReporting\Logging\ChilkatLog.txt";
            bool success = tunnel.UnlockComponent("Anything for 30-day trial.");

            if (success != true)
            {
                Console.WriteLine(tunnel.LastErrorText);
                return false;
            }


            //  Load a .ppk PuTTY private key.
            Chilkat.SshKey puttyKey = new Chilkat.SshKey();
            string ppkText = puttyKey.LoadText(@"C:\Users\panr2\Downloads\DataWarehouse\PrivateKey.ppk");

            //  "secret" is the actual password for the above PPK.
            if (!puttyKey.IsPrivateKey)
            {
                success = puttyKey.FromPuttyPrivateKey(ppkText);

                if (success != true)
                {
                    Console.WriteLine(puttyKey.LastErrorText);
                    return false;
                }
            }

            string sshHostname = "18.208.10.224";
            int sshPort = 22;

            //  Connect to an SSH server
            if (!tunnel.IsSshConnected())
            {
                success = tunnel.Connect(sshHostname, sshPort);

                if (success != true)
                {
                    Console.WriteLine(tunnel.LastErrorText);
                    return false;
                }

                //  Authenticate with the SSH server using a username + private key.
                //  (The private key serves as the password.  The username identifies
                //  the SSH user account on the server.)
                success = tunnel.AuthenticatePk("panran", puttyKey);

                if (success != true)
                {
                    Console.WriteLine(tunnel.LastErrorText);
                    return false;
                }
            }

            int listenPort = 5434;
            if (!tunnel.IsAccepting)
            {
                success = tunnel.BeginAccepting(listenPort);

                if (!success)
                {
                    Console.WriteLine(tunnel.LastErrorText);
                    return false;
                }
            }

            return true;
        }

        public static string GetHTMLByUrl(string url)
        {
            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                htmlCode = client.DownloadString(url);
            }
            return htmlCode;
        }

        public static string GetToken(string url, string username, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", username ),
                        new KeyValuePair<string, string> ( "Password", password )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url + "/token", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public static T GetDataFromJson<T>(string servicename, string methodname, string para = "default") where T : new()
        {
            StringBuilder url = new StringBuilder("http://local.ctroreporting.com:82/api/" + servicename + "/" + methodname);
            if (para != "default")
            {
                url.Append("?" + para);
            }

            string token = GetToken("http://local.ctroreporting.com:82", "panpanr", "panpanran");

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var t = JsonConvert.DeserializeObject<Token>(token);
                if (!string.IsNullOrWhiteSpace(t.access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);
                }
                var response = client.GetAsync(url.ToString()).Result;
                var json = response.Content.ReadAsStringAsync().Result;
                try
                {
                    var sdata = new JavaScriptSerializer().Deserialize<T>(json);
                    return sdata;
                }
                catch (Exception ex)
                {
                    return new T();
                }
            }
            //WebClient webClient = new WebClient();
            //string json = webClient.DownloadString(url.ToString());
            //var sdata = new JavaScriptSerializer().Deserialize<T>(json);
            //return sdata;
        }

        public static System.Data.DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";
            System.Data.DataTable dataTable = new System.Data.DataTable();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(
                          @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                          ";Extended Properties=\"Text;HDR=" + header + "\""))
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                {
                    dataTable.Locale = CultureInfo.CurrentCulture;
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog("CTROFunctions", "GetDataTableFromCsv", ex);
                return dataTable;
            }
        }

        public static async Task<Uri> CreateDataFromJson<T>(string servicename, string methodname, T data)
        {
            string token = GetToken("http://local.ctroreporting.com:82", "panpanr", "panpanran");

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://local.ctroreporting.com:82/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));


                var t = JsonConvert.DeserializeObject<Token>(token);
                if (!string.IsNullOrWhiteSpace(t.access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);
                }

                HttpResponseMessage response = await client.PostAsJsonAsync("api/" + servicename + "/" + methodname, data);

                response.EnsureSuccessStatusCode();
                // return URI of the created resource.
                return response.Headers.Location;

            }
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

        public static void SendEmail(Email email)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(email.To));
            msg.From = new MailAddress("ran.pan@nih.gov", "CTRO Reporting System");
            msg.Subject = email.Subject;
            msg.Body = email.Body;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("ran.pan@nih.gov", CTROConst.EW_PASSWORD);
            client.Port = 25; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "mailfwd.nih.gov";
            client.EnableSsl = false;

            if (!string.IsNullOrEmpty(email.AttachmentFileName))
            {
                Attachment attachment = new Attachment(email.AttachmentFileName, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(email.AttachmentFileName);
                disposition.ModificationDate = File.GetLastWriteTime(email.AttachmentFileName);
                disposition.ReadDate = File.GetLastAccessTime(email.AttachmentFileName);
                disposition.FileName = Path.GetFileName(email.AttachmentFileName);
                disposition.Size = new FileInfo(email.AttachmentFileName).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                msg.Attachments.Add(attachment);
            }
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message;
            }
        }

        public static System.Data.DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var table = new System.Data.DataTable();

            int i = 0;
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                table.Columns.Add(new DataColumn(prop.Name, prop.PropertyType));
                ++i;
            }

            foreach (var item in source)
            {
                var values = new object[i];
                i = 0;
                foreach (var prop in props)
                    values[i++] = prop.GetValue(item);
                table.Rows.Add(values);
            }

            return table;
        }

        public static Dictionary<string, double> processpercentage = new Dictionary<string, double>();

        public static async Task WriteExcelByDataTable(System.Data.DataTable datatable, ApplicationUser user, string savepath, string templatepath, int startrow, int startcolumn, bool ifchart, int totalrows = 0)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            try
            {
                if (!File.Exists(savepath))
                {
                    System.IO.File.Copy(templatepath, savepath, true);
                }

                xlWorkBook = xlApp.Workbooks.Open(savepath, misValue);

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[datatable.TableName];
                xlWorkSheet.Rows.WrapText = true;
                Excel.Range EntireRow = xlWorkSheet.Cells.EntireRow;

                var startCell = (Range)xlWorkSheet.Cells[startrow, startcolumn];
                var endCell = (Range)xlWorkSheet.Cells[datatable.Rows.Count + startrow - 1, datatable.Columns.Count + startcolumn - 1];
                var writeRange = xlWorkSheet.Range[startCell, endCell];
                var datas = new object[datatable.Rows.Count, datatable.Columns.Count];
                for (var row = 0; row <= datatable.Rows.Count - 1; row++)
                {
                    for (var column = 0; column <= datatable.Columns.Count - 1; column++)
                    {
                        if (datatable.Columns[column].ColumnName == "additionalcomments" && !string.IsNullOrEmpty(datatable.Rows[row].ItemArray[column].ToString()))
                        {
                            xlWorkSheet.Cells[row + startrow, column + startcolumn].RowHeight = 45;
                        }
                        datas[row, column] = datatable.Rows[row].ItemArray[column].ToString();
                    }

                    if (totalrows != 0)
                    {
                        processpercentage[user.UserName] += (double)1 / totalrows;
                    }
                }

                //Chart
                //if (ifchart == true)
                //{
                //    CreateExcelChart(xlWorkSheet, writeRange);
                //}

                writeRange.Value2 = datas;
                //writeRange.AutoFilter(1, Type.Missing, Excel.XlAutoFilterOperator.xlAnd, Type.Missing, true);
                ReleaseObject(xlWorkSheet);

                //xlWorkBook.Sheets["Sheet1"].Delete();
                xlWorkBook.Save();
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);
            }
            catch (Exception ex)
            {
                xlApp.Quit();
                ReleaseObject(xlApp);

                File.Delete(savepath);
                Logging.WriteLog("CTROFunctions", MethodBase.GetCurrentMethod().Name, ex);
                throw;
            }
        }

        public static void CreateExcelChart(Excel.Worksheet xlWorkSheet, Excel.Range chartRange)
        {
            object misValue = System.Reflection.Missing.Value;

            Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
            Excel.Chart chartPage = myChart.Chart;

            chartPage.SetSourceData(chartRange, misValue);
            chartPage.ChartType = Excel.XlChartType.xlColumnClustered;
        }


        public static void WriteExcelByDataSet(DataSet dataset, string savepath, string templatepath, int startrow, int startcolumn)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            try
            {

                if (templatepath != null)
                {
                    System.IO.File.Copy(templatepath, savepath, true);
                }

                xlWorkBook = xlApp.Workbooks.Open(savepath, misValue);

                for (int n = 0; n < dataset.Tables.Count; n++)
                {
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[dataset.Tables[n].TableName];
                    xlWorkSheet.Rows.WrapText = true;
                    Excel.Range EntireRow = xlWorkSheet.Cells.EntireRow;
                    //xlWorkSheet.AutoFilterMode = false;
                    //EntireRow.RowHeight = 15;
                    //EntireRow.ColumnWidth = 20;
                    // column headings
                    //for (var i = 0; i < dataset.Tables[n].Columns.Count; i++)
                    //{
                    //    xlWorkSheet.Cells[1, i + 1] = dataset.Tables[n].Columns[i].ColumnName;
                    //}

                    //for (int i = 0; i <= dataset.Tables[n].Rows.Count - 1; i++)
                    //{
                    //    for (int j = 0; j <= dataset.Tables[n].Columns.Count - 1; j++)
                    //    {
                    //        data = dataset.Tables[n].Rows[i].ItemArray[j].ToString();
                    //        if (dataset.Tables[n].Columns[j].ColumnName == "additionalcomments" && !string.IsNullOrEmpty(data))
                    //        {
                    //            xlWorkSheet.Cells[i + startrow, j + startcolumn].RowHeight = 45;
                    //        }

                    //        xlWorkSheet.Cells[i + startrow, j + startcolumn] = data;
                    //    }
                    //}
                    var startCell = (Range)xlWorkSheet.Cells[startrow, startcolumn];
                    var endCell = (Range)xlWorkSheet.Cells[dataset.Tables[n].Rows.Count + startrow - 1, dataset.Tables[n].Columns.Count + startcolumn - 1];
                    var writeRange = xlWorkSheet.Range[startCell, endCell];
                    var datas = new object[dataset.Tables[n].Rows.Count, dataset.Tables[n].Columns.Count];
                    for (var row = 0; row <= dataset.Tables[n].Rows.Count - 1; row++)
                    {
                        for (var column = 0; column <= dataset.Tables[n].Columns.Count - 1; column++)
                        {
                            if (dataset.Tables[n].Columns[column].ColumnName == "additionalcomments" && !string.IsNullOrEmpty(dataset.Tables[n].Rows[row].ItemArray[column].ToString()))
                            {
                                xlWorkSheet.Cells[row + startrow, column + startcolumn].RowHeight = 45;
                            }
                            datas[row, column] = dataset.Tables[n].Rows[row].ItemArray[column].ToString();
                        }
                    }
                    writeRange.Value2 = datas;
                    //writeRange.AutoFilter(1, Type.Missing, Excel.XlAutoFilterOperator.xlAnd, Type.Missing, true);
                    ReleaseObject(xlWorkSheet);
                }

                //xlWorkBook.Sheets["Sheet1"].Delete();
                xlWorkBook.Save();
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);

            }
            catch (Exception ex)
            {
                xlApp.Quit();
                ReleaseObject(xlApp);

                File.Delete(savepath);
                Logging.WriteLog("CTROFunctions", MethodBase.GetCurrentMethod().Name, ex);

                throw;
            }
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }

        }

        public static int CountBusinessDays(this DateTime firstDay, DateTime lastDay, params DateTime[] Holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday
    ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday
    ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime Holiday in Holidays)
            {
                DateTime bh = Holiday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

        public static List<DateTime> GetBusinessDays(this DateTime firstDay, DateTime lastDay, params DateTime[] Holidays)
        {
            List<DateTime> reportdatelist = new List<DateTime>();
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            for (DateTime reportDate = firstDay; reportDate < lastDay; reportDate = reportDate.AddDays(1))
            {
                if (reportDate.DayOfWeek == DayOfWeek.Sunday || reportDate.DayOfWeek == DayOfWeek.Saturday || Holidays.Contains(reportDate))
                {
                }
                else
                {
                    reportdatelist.Add(reportDate);
                }
            }
            return reportdatelist;
        }
    }

}
