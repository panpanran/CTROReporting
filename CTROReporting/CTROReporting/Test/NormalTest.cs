using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace CTRPReporting.Test
{

    [TestFixture()]
    public class NormalTest
    {
        [Test]
        public void ProtocolAbstractionTest()
        {
            IWebDriver driver = new ChromeDriver();
            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            driver.Navigate().GoToUrl("https://trials-stage.nci.nih.gov/pa/protected/studyProtocolexecute.action");

            //Read Data
            DataSet trialdata = CTRPFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\Temporary Report\2017-2018 Comparison January to September 20180912.xlsx");
            string trial = "NCI-2017-00086";
            string siteLocalTrialIdentifier = "12345";
            string recStatus = "Closed To Accrual and Intervention";
            string recStatusDate = "09/01/2018";

            //Login
            IWebElement username = driver.FindElement(By.Id("j_username"));
            username.SendKeys("panr");
            IWebElement password = driver.FindElement(By.Id("j_password"));
            password.SendKeys("Prss_5678");
            password.Submit();
            IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
            acceptclaim.Click();

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
            IWebElement searchspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Search");
            searchspan.Click();
            //Wait 2 seconds
            Thread.Sleep(2000);
            IWebElement selectspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Select");
            selectspan.Click();
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
            IWebElement sitesavespan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Save");
            sitesavespan.Click();
            //Investigators
            IWebElement tabinvestigator = driver.FindElements(By.TagName("a")).First(element => element.Text == "Investigators");
            tabinvestigator.Click();
            IWebElement spanadd = driver.FindElements(By.TagName("span")).First(element => element.Text == "Add");
            spanadd.Click();

            driver.Quit();
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
    }
}