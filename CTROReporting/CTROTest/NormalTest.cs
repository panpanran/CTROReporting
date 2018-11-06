using CTROReporting.Infrastructure;
using CTROReporting.CTRO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebSocketSharp;
using System.Net.Mail;
using Npgsql;
using System.Text.RegularExpressions;

namespace CTROTest
{
    [TestFixture()]
    public class NormalTest
    {
        [Test]
        public void WebSocketTest()
        {
            using (var ws = new WebSocket("ws://dragonsnest.far/Laputa"))
            {
                ws.OnMessage += (sender, e) =>
                  Console.WriteLine("Laputa says: " + e.Data);
                ws.Connect();
                ws.OnOpen += (sender, e) =>
                {
                    ws.Send("BALUS");
                };

                ws.OnClose += (sender, e) => { };

                Console.ReadKey(true);
            }
        }

        [Test]
        public void PCD2100Test()
        {
            IWebDriver driver = new ChromeDriver();
            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            driver.Navigate().GoToUrl("https://trials.nci.nih.gov/pa/protected/studyProtocolexecute.action");
            //Login
            IWebElement username = driver.FindElement(By.Id("j_username"));
            username.SendKeys("panr");
            IWebElement password = driver.FindElement(By.Id("j_password"));
            password.SendKeys("Prss_6789");
            password.Submit();
            IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
            acceptclaim.Click();


            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\PCD2100 Report\PCD2100 Report 20181106.xlsx");
            string comment = "test";

            //Do Loop
            foreach (DataTable table in trialdata.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    try
                    {
                        nciid = row.ItemArray[0].ToString();
                        //organization = row.ItemArray[16].ToString();
                        comment = "Per EW 85484 Anticipated Primary Completion Date 01/01/2100 was removed and N/A was selected";

                        //Find Trial
                        IWebElement trialSearchMenuOption = driver.FindElement(By.Id("trialSearchMenuOption"));
                        trialSearchMenuOption.Click();
                        IWebElement identifier = driver.FindElement(By.Id("identifier"));
                        identifier.SendKeys(nciid);
                        identifier.SendKeys(Keys.Enter);
                        IWebElement triallink = driver.FindElements(By.TagName("a")).First(element => element.Text == nciid);
                        triallink.Click();
                        //Checkout
                        IWebElement checkoutspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Admin Check Out");
                        checkoutspan.Click();
                        //Trial Status
                        IWebElement participatingsitelink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Trial Status");
                        participatingsitelink.Click();
                        //Set value
                        IJavaScriptExecutor txtPCD = (IJavaScriptExecutor)driver;
                        txtPCD.ExecuteScript("document.getElementById('primaryCompletionDate').setAttribute('value', '" + "" + "')");
                        //IJavaScriptExecutor radioPCD = (IJavaScriptExecutor)driver;
                        //radioPCD.ExecuteScript("document.getElementById('primaryCompletionDateTypeN/A').setAttribute('value', '" + "N/A" + "')");
                        IWebElement radioPCD = driver.FindElement(By.Id("primaryCompletionDateTypeN/A")); 
                        radioPCD.Click();

                        IWebElement savespan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Save");
                        savespan.Click();

                        //Checkin
                        IWebElement trialidentification = driver.FindElements(By.TagName("a")).First(element => element.Text == "Trial Identification");
                        trialidentification.Click();
                        IWebElement btnadmincheckin = driver.FindElements(By.TagName("span")).First(element => element.Text == "Admin Check In");
                        btnadmincheckin.Click();
                        if (driver.FindElements(By.TagName("button")).Where(element => element.Text == "Proceed with Check-in").Count() != 0)
                        {
                            IWebElement btnproceedcheckin = driver.FindElements(By.TagName("button")).First(element => element.Text == "Proceed with Check-in");
                            btnproceedcheckin.Click();
                        }
                        IWebElement txtcomments = driver.FindElement(By.Id("comments"));
                        txtcomments.SendKeys(comment);
                        IWebElement btnOk = driver.FindElements(By.TagName("button")).First(element => element.Text == "Ok");
                        btnOk.Click();
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteLog(nciid, organization, ex.Message);
                        throw;
                    }
                }
            }
        }

        [Test]
        public void GetOrganizationPOID()
        {
            //Find accrual number
            //Read Data
            string ctepid = "TX000";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\Temporary Report\POID Report 20181102.xlsx");
            //Do Loop
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\Temporary Report\Poid List.txt"))
                {
                    foreach (DataTable table in trialdata.Tables)
                    {
                        sw.WriteLine(table.TableName);

                        foreach (DataRow row in table.Rows)
                        {
                            ctepid = row.ItemArray[0].ToString();
                            if (ctepid.Length == 4)
                            {
                                ctepid = "0" + ctepid;
                            }
                            sql = "select po_id from dw_organization where ctep_id = '" + ctepid + "'";

                            NpgsqlCommand cmd = null;
                            NpgsqlDataReader datareader = null;
                            //NCI
                            cmd = new NpgsqlCommand(sql, conn);
                            datareader = cmd.ExecuteReader();
                            DataTable nciDT = new DataTable();
                            nciDT.Load(datareader);
                            if (nciDT.Rows.Count == 0)
                            {
                                sw.WriteLine("NULL");
                            }
                            else
                            {
                                sw.WriteLine(nciDT.Rows[0].ItemArray[0].ToString());
                            }
                        }
                    }
                }
            }
        }


        [Test]
        public void NCTNReportTest1()
        {
            //Find accrual number
            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Non-Intervention 20181031.xlsx");
            //Do Loop
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Accrual.txt"))
                {
                    foreach (DataTable table in trialdata.Tables)
                    {
                        sw.WriteLine(table.TableName);

                        foreach (DataRow row in table.Rows)
                        {
                            nciid = row.ItemArray[0].ToString();
                            organization = row.ItemArray[6].ToString();
                            sql = "select count(*) accrual from dw_study_site_accrual_details where nci_id = '" + nciid + "' and org_name = '" + organization + "'";

                            NpgsqlCommand cmd = null;
                            NpgsqlDataReader datareader = null;
                            //NCI
                            cmd = new NpgsqlCommand(sql, conn);
                            datareader = cmd.ExecuteReader();
                            DataTable nciDT = new DataTable();
                            nciDT.Load(datareader);
                            sw.WriteLine(nciDT.Rows[0].ItemArray[0].ToString());
                        }
                    }
                }
            }
        }

        [Test]
        public void NCTNReportTest2()
        {
            //Find accrual number
            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN COG 20181030.xlsx");
            //Do Loop
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Sites.txt"))
                {
                    foreach (DataTable table in trialdata.Tables)
                    {
                        sw.WriteLine(table.TableName);

                        foreach (DataRow row in table.Rows)
                        {
                            nciid = row.ItemArray[0].ToString();
                            organization = row.ItemArray[6].ToString();
                            sql = @"select
dw_study_participating_site.org_number
from(select * from dw_study
where lead_org = '" + organization + "' and nci_id = '" + nciid + @"') dw_study
join(select nci_id, count(org_name) org_number from dw_study_participating_site group by nci_id) dw_study_participating_site
on dw_study.nci_id = dw_study_participating_site.nci_id
order by dw_study.nci_id";

                            NpgsqlCommand cmd = null;
                            NpgsqlDataReader datareader = null;
                            //NCI
                            cmd = new NpgsqlCommand(sql, conn);
                            datareader = cmd.ExecuteReader();
                            DataTable nciDT = new DataTable();
                            nciDT.Load(datareader);
                            if (nciDT.Rows.Count == 0)
                            {
                                sw.WriteLine("0");
                            }
                            else
                            {
                                sw.WriteLine(nciDT.Rows[0].ItemArray[0].ToString());
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void NCTNReportTest3()
        {
            IWebDriver driver = new ChromeDriver();
            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            driver.Navigate().GoToUrl("https://trials.nci.nih.gov/pa/protected/studyProtocolexecute.action");
            //Login
            IWebElement username = driver.FindElement(By.Id("j_username"));
            username.SendKeys("panr");
            IWebElement password = driver.FindElement(By.Id("j_password"));
            password.SendKeys("Prss_6789");
            password.Submit();
            IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
            acceptclaim.Click();


            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN COG 20181101.xlsx");
            string comment = "test";

            //Do Loop
            foreach (DataTable table in trialdata.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    try
                    {
                        nciid = row.ItemArray[0].ToString();
                        organization = row.ItemArray[16].ToString();
                        comment = "Per EW # 85298 CTRP data clean up, the participating site " + organization + " is removed, as the site is national and not accruing any patients.";

                        //Find Trial
                        IWebElement trialSearchMenuOption = driver.FindElement(By.Id("trialSearchMenuOption"));
                        trialSearchMenuOption.Click();
                        IWebElement identifier = driver.FindElement(By.Id("identifier"));
                        identifier.SendKeys(nciid);
                        identifier.SendKeys(Keys.Enter);
                        IWebElement triallink = driver.FindElements(By.TagName("a")).First(element => element.Text == nciid);
                        triallink.Click();
                        //Checkout
                        IWebElement checkoutspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Admin Check Out");
                        checkoutspan.Click();
                        //Participating Site
                        IWebElement participatingsitelink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Participating Sites");
                        participatingsitelink.Click();
                        //Find participating site
                        //string temp = driver.PageSource;
                        //MatchCollection matches = Regex.Matches(temp, @"<!DOCTYPE html PUBLIC(.*?)IE=edge");
                        //string matchrow = matches[0].Value;

                        IWebElement deletecheckbox = RecursiveSites(nciid, organization, driver);
                        deletecheckbox.Click();
                        IWebElement deletebtn = driver.FindElements(By.TagName("span")).First(element => element.Text == "Delete");
                        deletebtn.Click();
                        string currentwindow = driver.CurrentWindowHandle;
                        driver.SwitchTo().Frame("popupFrame");
                        IWebElement okbtn = driver.FindElements(By.TagName("span")).First(element => element.Text == "OK");
                        okbtn.Click();
                        driver.SwitchTo().Window(currentwindow);
                        //Checkin
                        IWebElement trialidentification = driver.FindElements(By.TagName("a")).First(element => element.Text == "Trial Identification");
                        trialidentification.Click();
                        IWebElement btnadmincheckin = driver.FindElements(By.TagName("span")).First(element => element.Text == "Admin Check In");
                        btnadmincheckin.Click();
                        if (driver.FindElements(By.TagName("button")).Where(element => element.Text == "Proceed with Check-in").Count() != 0)
                        {
                            IWebElement btnproceedcheckin = driver.FindElements(By.TagName("button")).First(element => element.Text == "Proceed with Check-in");
                            btnproceedcheckin.Click();
                        }
                        IWebElement txtcomments = driver.FindElement(By.Id("comments"));
                        txtcomments.SendKeys(comment);
                        IWebElement btnOk = driver.FindElements(By.TagName("button")).First(element => element.Text == "Ok");
                        btnOk.Click();
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteLog(nciid, organization, ex.Message);
                        throw;
                    }
                }
            }
        }

        public IWebElement RecursiveSites(string nciid, string organization, IWebDriver driver)
        {
            var rowmatch = driver.FindElements(By.TagName("td"));
            var matchid = rowmatch.IndexOf(rowmatch.Where(x => x.Text == organization).SingleOrDefault());
            if (matchid < 0)
            {
                IWebElement nextlink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Next");
                if (nextlink != null)
                {
                    nextlink.Click();
                    return RecursiveSites(nciid, organization, driver);
                }
                else
                {
                    Logging.WriteLog(nciid, organization, "cannot find this participating site");
                    throw new Exception();
                }
            }
            return rowmatch[matchid + 6];
        }


        [Test]
        public void ProtocolAbstractionTest()
        {
            try
            {
                IWebDriver driver = new ChromeDriver();
                //Notice navigation is slightly different than the Java version
                //This is because 'get' is a keyword in C#
                driver.Navigate().GoToUrl("https://trials.nci.nih.gov/pa/protected/studyProtocolexecute.action");
                //Login
                IWebElement username = driver.FindElement(By.Id("j_username"));
                username.SendKeys("panr");
                IWebElement password = driver.FindElement(By.Id("j_password"));
                password.SendKeys("Prss_5678");
                password.Submit();
                IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
                acceptclaim.Click();

                //Read Data
                string trial = "NCI-2017-00086";
                string siteLocalTrialIdentifier = "12345";
                string recStatus = "Closed To Accrual and Intervention";
                string recStatusDate = "09/01/2018";
                string poid = "1757021";
                string comment = "test";
                DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\MGH to MGHCC\MGHCC List.xlsx");

                //Do Loop
                foreach (DataTable table in trialdata.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        trial = row.ItemArray[0].ToString();
                        siteLocalTrialIdentifier = row.ItemArray[1].ToString();
                        recStatus = row.ItemArray[2].ToString();
                        recStatusDate = row.ItemArray[3].ToString();
                        //poid = row.ItemArray[4].ToString();
                        comment = "Add organization MGHCC for EW ticket 77588";



                        //Find Trial
                        IWebElement trialSearchMenuOption = driver.FindElement(By.Id("trialSearchMenuOption"));
                        trialSearchMenuOption.Click();
                        IWebElement identifier = driver.FindElement(By.Id("identifier"));
                        identifier.SendKeys(trial);
                        identifier.SendKeys(Keys.Enter);
                        IWebElement triallink = driver.FindElements(By.TagName("a")).First(element => element.Text == trial);
                        triallink.Click();
                        //Checkout
                        IWebElement checkoutspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Admin Check Out");
                        checkoutspan.Click();
                        //Participating Site
                        IWebElement participatingsitelink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Participating Sites");
                        participatingsitelink.Click();
                        IWebElement addsitelink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Add");
                        addsitelink.Click();
                        IWebElement lookuplink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Look Up");
                        lookuplink.Click();
                        IWebElement popupInner = driver.FindElement(By.Id("popupInner"));
                        //Popwindow for organization look up
                        string currentwindow = driver.CurrentWindowHandle;
                        driver.SwitchTo().Frame("popupFrame");
                        IWebElement txtorgNameSearch = driver.FindElement(By.Name("orgName"));
                        txtorgNameSearch.SendKeys("Massachusetts General Hospital Cancer Center");
                        IWebElement searchsitespan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Search");
                        searchsitespan.Click();
                        //Wait 2 seconds
                        Thread.Sleep(2000);
                        IWebElement selectsitespan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Select");
                        selectsitespan.Click();
                        //Back to main window
                        driver.SwitchTo().Window(currentwindow);
                        IWebElement txtsiteLocalTrialIdentifier = driver.FindElement(By.Id("siteLocalTrialIdentifier"));
                        txtsiteLocalTrialIdentifier.SendKeys(siteLocalTrialIdentifier);
                        IWebElement ddlrecStatus = driver.FindElement(By.Id("recStatus"));
                        ddlrecStatus.SendKeys(recStatus);
                        //Set readonly field by JS
                        IWebElement txtrecStatusDate = driver.FindElement(By.Id("recStatusDate"));
                        IJavaScriptExecutor rectatusDateJS = (IJavaScriptExecutor)driver;
                        rectatusDateJS.ExecuteScript("document.getElementById('recStatusDate').setAttribute('value', '" + recStatusDate + "')");
                        if (driver.FindElements(By.Id("participatingOrganizationscreate_dateOpenedForAccrual")).Count != 0)
                        {
                            rectatusDateJS.ExecuteScript("document.getElementById('participatingOrganizationscreate_dateOpenedForAccrual').setAttribute('value', '" + "1/1/2010" + "')");
                        }
                        if (recStatus == "Completed")
                        {
                            rectatusDateJS.ExecuteScript("document.getElementById('participatingOrganizationscreate_dateClosedForAccrual').setAttribute('value', '" + "1/1/2010" + "')");
                        }
                        IWebElement sitesavespan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Save");
                        sitesavespan.Click();
                        ////Investigatorsjavascript:void(0)
                        //IWebElement tabinvestigator = driver.FindElements(By.TagName("a")).First(element => element.Text == "Investigators");
                        //tabinvestigator.Click();
                        //IWebElement spanadd = driver.FindElements(By.TagName("span")).First(element => element.Text == "Add");
                        //spanadd.Click();
                        //currentwindow = driver.CurrentWindowHandle;
                        //driver.SwitchTo().Frame("popupFrame");
                        //IWebElement txtpoID = driver.FindElement(By.Id("poID"));
                        //txtpoID.SendKeys(poid);
                        //IWebElement searchinvestigatorspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Search");
                        //searchinvestigatorspan.Click();
                        ////Wait 2 seconds
                        //Thread.Sleep(2000);
                        //IWebElement selectinvestigatorspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Select");
                        //selectinvestigatorspan.Click();
                        //driver.SwitchTo().Window(currentwindow);
                        //Checkin
                        IWebElement trialidentification = driver.FindElements(By.TagName("a")).First(element => element.Text == "Trial Identification");
                        trialidentification.Click();
                        IWebElement btnadmincheckin = driver.FindElements(By.TagName("span")).First(element => element.Text == "Admin Check In");
                        btnadmincheckin.Click();
                        if (driver.FindElements(By.TagName("button")).Where(element => element.Text == "Proceed with Check-in").Count() != 0)
                        {
                            IWebElement btnproceedcheckin = driver.FindElements(By.TagName("button")).First(element => element.Text == "Proceed with Check-in");
                            btnproceedcheckin.Click();
                        }
                        IWebElement txtcomments = driver.FindElement(By.Id("comments"));
                        txtcomments.SendKeys(comment);
                        IWebElement btnOk = driver.FindElements(By.TagName("button")).First(element => element.Text == "Ok");
                        btnOk.Click();
                    }
                }

                driver.Quit();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [Test]
        public void EventlogTest()
        {
            if (!EventLog.SourceExists("RanSource"))
            {
                EventLog.CreateEventSource("RanSource", "MyNewLog");
                Console.WriteLine("CreatedEventSource");
                Console.WriteLine("Please restart application");
                Console.ReadKey();
                return;
            }
            EventLog myLog = new EventLog();
            myLog.Source = "MySource";
            myLog.WriteEntry("Log event!");
        }


        [Test]
        public void XMLTest()
        {
            XNamespace ew = "ContactList";
            XElement root = new XElement(ew + "Root");
            Console.WriteLine(root);
            XAttribute contacts = new XAttribute("contacts", root);
        }


        [Test]
        public void DEBUGTest(Socket socket)
        {
#if (DEBUG)
            Console.WriteLine("Debug Mode");
#elif (RELEASE)
               Console.WriteLine("Release Mode");
#endif
        }


        [Test]
        public Task<Socket> AcceptAsync(Socket socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");

            var tcs = new TaskCompletionSource<Socket>();

            socket.BeginAccept(asyncResult =>
            {
                try
                {
                    var s = asyncResult.AsyncState as Socket;
                    var client = s.EndAccept(asyncResult);

                    tcs.SetResult(client);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            }, socket);

            return tcs.Task;
        }

        [Test]
        public void ArgumentExceptionTest()
        {
            try
            {
                string a = "";
                if (a == "")
                {
                    throw new ArgumentNullException();
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        [Test]
        public void TractSourceTest()
        {
            Stream outputFile = File.Create(@"C:\Users\panr2\Downloads\DataWarehouse\tracefile.txt");
            TextWriterTraceListener textListener = new TextWriterTraceListener(outputFile);
            TraceSource traceSource = new TraceSource("myTraceSource", SourceLevels.All);
            traceSource.Listeners.Clear();
            traceSource.Listeners.Add(textListener);
            traceSource.TraceInformation("Trace output");
            traceSource.Flush();
            traceSource.Close();
        }

        [Test]
        public void WebRequestTest()
        {
            WebRequest request = WebRequest.Create("https://www.microsoft.com/en-us/");
            WebResponse response = request.GetResponse();
            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            string responseText = responseStream.ReadToEnd();
            Console.WriteLine(responseText); // Displays the HTML of the website
            response.Close();
        }

        [Test]
        public void ConcurrentTest()
        {
            var dict = new ConcurrentDictionary<string, int>();
            if (dict.TryAdd("k1", 42))
            {
                Console.WriteLine("Added");
            }
            if (dict.TryUpdate("k1", 21, 42))
            {
                Console.WriteLine("42 updated to 21");
            }
            dict["k1"] = 42; // Overwrite unconditionally
            int r1 = dict.AddOrUpdate("k1", 3, (s, i) => i * 2);
            int r2 = dict.GetOrAdd("k2", 3);
        }



        [Test]
        public void SendEmailTest()
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("dena.sumaida@nih.gov"));
            msg.From = new MailAddress("ncictro@mail.nih.gov", "CTRO Reporting System");
            msg.Subject = "Test Public Email";
            msg.Body = "Only for testing. Can you receive it ? Is it OK like this ?";
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("ncictro@mail.nih.gov", "20180806!!N_c_i");
            client.Port = 25; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "mailfwd.nih.gov";
            client.EnableSsl = false;

            //if (!string.IsNullOrEmpty(AttachmentFileName))
            //{
            //    Attachment attachment = new Attachment(AttachmentFileName, MediaTypeNames.Application.Octet);
            //    ContentDisposition disposition = attachment.ContentDisposition;
            //    disposition.CreationDate = File.GetCreationTime(AttachmentFileName);
            //    disposition.ModificationDate = File.GetLastWriteTime(AttachmentFileName);
            //    disposition.ReadDate = File.GetLastAccessTime(AttachmentFileName);
            //    disposition.FileName = Path.GetFileName(AttachmentFileName);
            //    disposition.Size = new FileInfo(AttachmentFileName).Length;
            //    disposition.DispositionType = DispositionTypeNames.Attachment;
            //    msg.Attachments.Add(attachment);
            //}
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message;
            }
        }
    }
}
