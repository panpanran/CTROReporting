using CTROLibrary.Model;
using CTROLibrary.CTRO;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Reflection;

namespace CTROLibrary.EW
{
    public class EWTicket : IEWHome<Ticket>
    {
        public IEnumerable<string> GetIDList(string where)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=" + ConfigurationManager.AppSettings["V_RANPAN_EW_ACCOUNT"] + "&$password=" + ConfigurationManager.AppSettings["V_RANPAN_EW_PASSWORD"] + "&$table=ctro_tickets&$lang=en&where=" + where;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = html.Split(stringSeparators, StringSplitOptions.None);
            return lines;
        }

        public Ticket GetById(string id)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWRead?$KB=CBIIT&$table=ctro_tickets&$login=" + ConfigurationManager.AppSettings["V_RANPAN_EW_ACCOUNT"] + "&$password=" + ConfigurationManager.AppSettings["V_RANPAN_EW_PASSWORD"] + "&$lang=en&id=" + id;
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

    public class EWSolutionTSRFeedback : EWTicket
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
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=" + ConfigurationManager.AppSettings["V_RANPAN_EW_ACCOUNT"] + "&$password=" + ConfigurationManager.AppSettings["V_RANPAN_EW_PASSWORD"] + "&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public bool BulkUpdate(string where, ApplicationUser user)
        {
            //Update(GetById("82066"));


            //Logging.WriteLog("EWSolutionTSRFeedback", "BulkUpdate", "Start!");

            string[] ticketlist = GetIDList(where).ToArray();

            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["V_CTROChromeDriver"], options, TimeSpan.FromSeconds(120));
            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            driver.Navigate().GoToUrl("https://trials.nci.nih.gov/pa/protected/studyProtocolexecute.action");
            //Login
            IWebElement username = driver.FindElement(By.Id("j_username"));
            username.SendKeys(ConfigurationManager.AppSettings["V_RANPAN_PA_ACCOUNT"]);
            IWebElement password = driver.FindElement(By.Id("j_password"));
            password.SendKeys(ConfigurationManager.AppSettings["V_RANPAN_PA_PASSWORD"]);
            password.Submit();
            IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
            acceptclaim.Click();

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string ticketid = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(ticketid);
                string nciid = Regex.Match(ticket.Summary, "NCI-.*?,").Value.Replace(",", "");


                bool tsrfeedback = TSRFeedback(ticketid, nciid, driver, user);
                if (tsrfeedback)
                {
                    Update(ticket);
                }
            }

            //Logging.WriteLog("EWSolutionTSRFeedback", "BulkUpdate", "Finished!");

            return true;

        }

        public bool TSRFeedback(string ticket, string nciid, IWebDriver driver, ApplicationUser user)
        {
            Logger logger = new Logger
            {
                ClassName = this.GetType().Name,
                MethodName = "TSRFeedback",
                Level = 1,
                Message = "NCIID: " + nciid + ", TICKETID: " + ticket + ", Createdtime: " + DateTime.Now.ToString() + " TSR Feedback Done.",
                UserId = user.Id
            };
            //Read Data
            string comment = "test";
            try
            {
                comment = "EW#" + ticket + " – TSR verified";

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
                //Trial Milestones
                IWebElement participatingsitelink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Trial Milestones");
                participatingsitelink.Click();
                //Set value
                IWebElement ddlmilestone = driver.FindElement(By.Id("milestone"));
                ddlmilestone.SendKeys("Submitter Trial Summary Report Feedback Date");
                IWebElement txtmilestonecomment = driver.FindElement(By.Id("milestoneCommentsTA"));
                txtmilestonecomment.SendKeys(comment);
                IWebElement addspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Add Milestone");
                addspan.Click();

                if (driver.FindElements(By.TagName("input")).Where(element => element.GetAttribute("value") == "Initial Abstraction Verified Date").Count() == 0)
                {
                    ddlmilestone = driver.FindElement(By.Id("milestone"));
                    ddlmilestone.SendKeys("Initial Abastraction Verified Date");
                    addspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Add Milestone");
                    addspan.Click();
                }
                else if (driver.FindElements(By.TagName("input")).Where(element => element.GetAttribute("value") == "On-going Abstraction Verified Date").Count() == 0)
                {
                    ddlmilestone = driver.FindElement(By.Id("milestone"));
                    ddlmilestone.SendKeys("On-going Abstraction Verified Date");
                    addspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Add Milestone");
                    addspan.Click();
                }
                int ongoingspannumber = driver.FindElements(By.TagName("span"))
                    .Where(element => element.Text.Contains("There is a problem with the current abstraction")).Count();
                if (ongoingspannumber > 0)
                {
                    Logging.WriteLog("nciid: " + nciid, "ticket: " + ticket, "validation error");
                    logger.Message = "NCIID: " + nciid + ", TICKETID: " + ticket + ", Createdtime: " + DateTime.Now.ToString() + " Error message is: " + "there has been a on-going comment.";
                    var log = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
                }


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
                var url = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
            }
            catch (Exception ex)
            {
                logger.Message = "NCIID: " + nciid + ", TICKETID: " + ticket + ", Createdtime: " + DateTime.Now.ToString() + " Error message is: " + ex.Message;
                var url = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
                return false;
            }

            return true;
        }
    }

    public class EWSolutionTicketTriagingSchedule : EWTicket
    {
        public bool ScheduleTicketsTriaging()
        {
            try
            {
                CTROHangfire.Start();
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog("EWSolutionTicketTriagingSchedule", "ScheduleTicketsTriaging", ex.Message);
                return false;
            }
        }
    }


    public class EWDashboardCheck : EWTicket
    {
        public DataTable DashboardCheck(string code)
        {
            DataTable table = new DataTable();
            //Logger logger = new Logger
            //{
            //    ClassName = this.GetType().Name,
            //    MethodName = "DashboardCheck",
            //    Level = 1,
            //    Message = "Dashboard check finished.",
            //    UserId = user.Id
            //};

            try
            {
                string input = ConfigurationManager.AppSettings["V_CTROChromeDriver"] + "/workload.csv";
                //string output = @"C:\Users\panr2\Downloads\result.txt";
                if (DownloadCSV())
                {
                    Thread.Sleep(1000);
                    table = GenerateReport(input, code);
                }
                //CTROFunctions.SendEmail("Dashboard Expectation Date Check " + String.Format("{0:yyyyMMdd}", DateTime.Now), "Hi Dena, <br /><br /> Attached please find. Dashboard Expectation Date Check Report has been done. <br /><br /> Thank you", "dena.sumaida@nih.gov", output);
                //CTROFunctions.SendEmail("Dashboard Expectation Date Check " + String.Format("{0:yyyyMMdd}", DateTime.Now), "Hi Vika, <br /><br /> Attached please find. Dashboard Expectation Date Check Report has been done. <br /><br /> Thank you", "grinbergv@mail.nih.gov", output);
                //CTROFunctions.SendEmail("Dashboard Expectation Date Check " + String.Format("{0:yyyyMMdd}", DateTime.Now), "Hi Ran, <br /><br /> Attached please find. Dashboard Expectation Date Check Report has been done. <br /><br /> Thank you", "ran.pan@nih.gov", output);
                //var url = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
                return table;
            }
            catch (Exception ex)
            {
                //logger.Level = 2;
                //logger.Message = "Dashboard check failed.";
                //var url = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
                return table;
            }
        }

        public bool DownloadCSV()
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", ConfigurationManager.AppSettings["V_CTROChromeDriver"]);
                //options.AddUserProfilePreference("intl.accept_languages", "nl");
                //options.AddUserProfilePreference("disable-popup-blocking", "true");
                IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["V_CTROChromeDriver"], options, TimeSpan.FromSeconds(120));

                //Notice navigation is slightly different than the Java version
                //This is because 'get' is a keyword in C#
                driver.Navigate().GoToUrl("https://trials.nci.nih.gov/pa/protected/studyProtocolexecute.action");
                //Login
                IWebElement username = driver.FindElement(By.Id("j_username"));
                username.SendKeys(ConfigurationManager.AppSettings["V_RANPAN_PA_ACCOUNT"]);
                IWebElement password = driver.FindElement(By.Id("j_password"));
                password.SendKeys(ConfigurationManager.AppSettings["V_RANPAN_PA_PASSWORD"]);
                password.Submit();
                IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
                acceptclaim.Click();

                //CSV
                IWebElement csvspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "CSV");
                csvspan.Click();
                return true;
            }
            catch (Exception ex)
            {
                //Logging.WriteLog("EWDashboardCheck", "DownloadCSV", ex.Message);
                return false;
            }
        }

        public DataTable GenerateReport(string input, string code)
        {
            DataTable outputTable = new DataTable();
            outputTable.Columns.Add("nciid", typeof(string));
            outputTable.Columns.Add("submissionnumber", typeof(string));
            outputTable.Columns.Add("accepted", typeof(DateTime));
            //outputTable.Columns.Add("reactivateddate", typeof(string));
            //outputTable.Columns.Add("onholddate", typeof(string));
            //outputTable.Columns.Add("offholddate", typeof(string));
            //outputTable.Columns.Add("onholdreasontype", typeof(string));
            //outputTable.Columns.Add("onholdreason", typeof(string));
            outputTable.Columns.Add("dashboardexpected", typeof(DateTime));
            outputTable.Columns.Add("realexpected", typeof(DateTime));

            DataTable trialdata = CTROFunctions.GetDataTableFromCsv(input, true);

            using (var conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PADWConnectionString"].ConnectionString))
            {
                conn.Open();
                NpgsqlCommand cmd = null;
                NpgsqlDataReader datareader = null;
                DateTime expecteddate;
                DateTime startdate = new DateTime();
                DateTime enddate;
                DateTime reactivateddate = new DateTime();
                DateTime onholddate = new DateTime();
                DateTime offholddate = new DateTime();
                DateTime accepteddate = new DateTime();

                string nciid;
                string submissionnumber;
                string onholdreasontype;
                string onholdreason;

                int onholdflag = 0;

                //using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\result.txt"))
                //{
                foreach (DataRow workloadrow in trialdata.Rows)
                {
                    nciid = "NCI-" + workloadrow[0].ToString();
                    string codetext = code.Replace("nciidpara", nciid);
                    DateTime lastworkdate = DateTime.Now.Date;
                    while (CTROFunctions.CountBusinessDays(lastworkdate, DateTime.Now.Date, CTROConst.Holidays) <= 1)
                    {
                        lastworkdate = lastworkdate.AddDays(-1);
                    }
                    codetext = codetext.Replace("offholddate", "'" + lastworkdate.ToShortDateString() + "'");

                    cmd = new NpgsqlCommand(codetext, conn);
                    datareader = cmd.ExecuteReader();
                    DataTable nciDT = new DataTable();
                    nciDT.Load(datareader);
                    int totaldays = 0;
                    expecteddate = (DateTime)workloadrow.ItemArray[3];

                    for (int i = 0; i < nciDT.Rows.Count; i++)
                    {
                        onholdflag = 0;
                        if (i == 0)
                        {
                            accepteddate = string.IsNullOrEmpty(workloadrow.ItemArray[5].ToString()) ? new DateTime() : (DateTime)workloadrow.ItemArray[5];
                            reactivateddate = string.IsNullOrEmpty(nciDT.Rows[i].ItemArray[5].ToString()) ? new DateTime() : (DateTime)(nciDT.Rows[i].ItemArray[5]);
                            startdate = reactivateddate > accepteddate ? reactivateddate : accepteddate;
                            totaldays = CTROFunctions.CountBusinessDays(startdate, DateTime.Now.Date, CTROConst.Holidays);
                        }
                        onholddate = string.IsNullOrEmpty(nciDT.Rows[i].ItemArray[3].ToString()) ? new DateTime() : (DateTime)(nciDT.Rows[i].ItemArray[3]);
                        offholddate = string.IsNullOrEmpty(nciDT.Rows[i].ItemArray[4].ToString()) ? new DateTime() : (DateTime)(nciDT.Rows[i].ItemArray[4]);
                        if (onholddate > Convert.ToDateTime("1/1/0001"))
                        {
                            if (offholddate > Convert.ToDateTime("1/1/0001"))
                            {
                                if (offholddate.Date > startdate.Date)
                                {
                                    totaldays = totaldays - CTROFunctions.CountBusinessDays(onholddate > startdate ? onholddate : startdate, offholddate, CTROConst.Holidays);
                                }
                            }
                            else
                            {
                                onholdflag = 2;
                            }
                        }
                    }

                    if (nciDT.Rows.Count == 0)
                    {
                        reactivateddate = new DateTime();
                        onholddate = new DateTime();
                        offholddate = new DateTime();
                    }

                    if (accepteddate.Date == Convert.ToDateTime("1/1/0001"))
                    {
                        //No record
                        onholdflag = 1;
                    }


                    switch (onholdflag)
                    {
                        case 1:
                            //sw.WriteLine("nciid: " + nciid + ", this trial has not been accepted.");
                            break;
                        case 2:
                            //sw.WriteLine("nciid: " + nciid + ", this trial is still on-hold.");
                            break;
                        case 0:
                            if (totaldays < 0)
                            {
                                //sw.WriteLine("There are some errors. nciid: " + nciid + ", expecteddate on dashboard: " + expecteddate);
                            }
                            else
                            {
                                enddate = DateTime.Now.Date;
                                while (CTROFunctions.CountBusinessDays(DateTime.Now.Date, enddate, CTROConst.Holidays) <= 10 - totaldays)
                                {
                                    enddate = enddate.AddDays(1);
                                }
                                if (CTROFunctions.CountBusinessDays(DateTime.Now.Date, enddate, CTROConst.Holidays) < 106)
                                {
                                    if (expecteddate.Date != enddate.Date)
                                    {
                                        try
                                        {
                                            submissionnumber = nciDT.Rows[0].ItemArray[1].ToString();
                                            onholdreasontype = nciDT.Rows[0].ItemArray[6].ToString();
                                            onholdreason = nciDT.Rows[0].ItemArray[7].ToString();
                                            outputTable.Rows.Add(nciid, submissionnumber, accepteddate,
                                                //reactivateddate.Date == Convert.ToDateTime("1/1/0001")? null: reactivateddate.ToShortDateString(), 
                                                //onholddate.Date == Convert.ToDateTime("1/1/0001") ? null : onholddate.ToShortDateString(),
                                                //offholddate.Date == Convert.ToDateTime("1/1/0001") ? null : offholddate.ToShortDateString(), 
                                                //onholdreasontype, 
                                                //onholdreason, 
                                                expecteddate, enddate);
                                            //Logging.WriteLog("EWDashboardCheck", "GenerateReport", "nciid: " + nciid + " need to be changed");
                                            //sw.WriteLine("nciid: " + nciid + ", expecteddate on dashboard: " + expecteddate + ", real expected date: " + enddate);
                                        }
                                        catch (Exception ex)
                                        {
                                            //Logging.WriteLog("EWDashboardCheck", "GenerateReport", "nciiid:" + nciid + ", error message: " + ex.Message);
                                        }
                                    }
                                }
                                //else
                                //{
                                //    sw.WriteLine("nciid: " + nciid + ", expecteddate on dashboard is even more than real expected date. So just leave it there.");
                                //}
                            }
                            break;
                    }
                }
                //}

                if (File.Exists(input))
                {
                    File.Delete(input);
                }
                return outputTable;
            }
        }
    }

    public class EWContinueReview : EWTicket
    {

        public override void Update(Ticket ticket)
        {
            //string content = @"Board Approval Number updated to 05/31/2018, uploaded Continuing Review document. ";
            //string content = @"Trial is out of scope for CTRP. No action required.";
            string content = @"No action required, trial present in PA as NCI-2014-01029 is a CCR/CTEP trial and all updates should be submitted via CTEP services. ";

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=" + ConfigurationManager.AppSettings["V_RANPAN_EW_ACCOUNT"] + "&$password=" + ConfigurationManager.AppSettings["V_RANPAN_EW_PASSWORD"] + "&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }


        public bool BulkUpdate(string where, ApplicationUser user)
        {
            Update(GetById("80338"));

            //string[] ticketlist = GetIDList(where).ToArray();
            //for (int i = 1; i < ticketlist.Length - 1; i++)
            //{
            //    string id = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ",""));
            //    Update(GetById(id));
            //}
            return true;
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
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=" + ConfigurationManager.AppSettings["V_RANPAN_EW_ACCOUNT"] + "&$password=" + ConfigurationManager.AppSettings["V_RANPAN_EW_PASSWORD"] + "&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void AssignedTo(Ticket ticket)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=" + ConfigurationManager.AppSettings["V_RANPAN_EW_ACCOUNT"] + "&$password=" + ConfigurationManager.AppSettings["V_RANPAN_EW_PASSWORD"] + "&$lang=en&id=" + ticket.TicketId + "&description=" + ticket.Summary;
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
