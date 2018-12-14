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

namespace CTROLibrary.EW
{
    public class EWTicket : IEWHome<Ticket>
    {
        public IEnumerable<string> GetIDList(string where)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWSelect?$KB=CBIIT&$login=panr2&$password=Prss_3456&$table=ctro_tickets&$lang=en&where=" + where;
            string html = CTROFunctions.GetHTMLByUrl(url);
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = html.Split(stringSeparators, StringSplitOptions.None);
            return lines;
        }

        public Ticket GetById(string id)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWRead?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_3456&$lang=en&id=" + id;
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
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_3456&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }


        public bool BulkUpdate(string where, ApplicationUser user)
        {
            //Update(GetById("82066"));


            //Logging.WriteLog("EWSolutionTSRFeedback", "BulkUpdate", "Start!");

            string[] ticketlist = GetIDList(where).ToArray();

            IWebDriver driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            driver.Navigate().GoToUrl("https://trials.nci.nih.gov/pa/protected/studyProtocolexecute.action");
            //Login
            IWebElement username = driver.FindElement(By.Id("j_username"));
            username.SendKeys("panr");
            IWebElement password = driver.FindElement(By.Id("j_password"));
            password.SendKeys("Prss_7890");
            password.Submit();
            IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
            acceptclaim.Click();

            for (int i = 1; i < ticketlist.Length - 1; i++)
            {
                string ticketid = GetValueByFieldName("EWREST_id_" + (i - 1).ToString(), ticketlist[i].Replace(" ", ""));
                Ticket ticket = GetById(ticketid);
                string nciid = Regex.Match(ticket.Summary, "NCI-.*?,").Value.Replace(",", "");


                TSRFeedback(ticketid, nciid, driver, user);
                Update(ticket);
            }

            //Logging.WriteLog("EWSolutionTSRFeedback", "BulkUpdate", "Finished!");

            return true;

        }

        public void TSRFeedback(string ticket, string nciid, IWebDriver driver, ApplicationUser user)
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

            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_3456&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
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
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_3456&$lang=en&id=" + ticket.TicketId + "&internal_analysis=" + content;
            string html = CTROFunctions.GetHTMLByUrl(url);
        }

        public void AssignedTo(Ticket ticket)
        {
            string url = "https://cbiitsupport.nci.nih.gov/ewws/EWUpdate?$KB=CBIIT&$table=ctro_tickets&$login=panr2&$password=Prss_3456&$lang=en&id=" + ticket.TicketId + "&description=" + ticket.Summary;
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
