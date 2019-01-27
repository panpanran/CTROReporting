using CTROLibrary.CTRO;
using CTROLibrary.EW;
using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using Npgsql;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CTROTest
{
    [TestFixture()]
    public class PATest
    {
        [Test]
        public void PilotUpdateTest()
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options, TimeSpan.FromSeconds(120));
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


            //Read Data
            string nciid = "NCI-2017-00086";
            string pilot = "No";
            string organization = "Wake Forest NCORP Research Base";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\Pilot field Update\Pilot worksheet Doing.xlsx");
            string comment = "test";

            //Do Loop
            foreach (DataTable table in trialdata.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    try
                    {
                        nciid = row.ItemArray[0].ToString();
                        pilot = row.ItemArray[1].ToString();

                        //organization = row.ItemArray[16].ToString();
                        //comment = "Per EW 85484 Anticipated Primary Completion Date 01/01/2100 was removed and N/A was selected";
                        comment = "Added Pilot status per UCSF request in EW# 87796.";


                        //Find Trial
                        IWebElement trialSearchMenuOption = driver.FindElement(By.Id("trialSearchMenuOption"));
                        trialSearchMenuOption.Click();
                        IWebElement identifier = driver.FindElement(By.Id("identifier"));
                        identifier.SendKeys(nciid);
                        identifier.SendKeys(Keys.Enter);
                        IWebElement triallink = driver.FindElements(By.TagName("a")).First(element => element.Text == nciid);
                        triallink.Click();
                        //Checkout
                        IWebElement checkoutspan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Scientific Check Out");
                        checkoutspan.Click();
                        //Design Details
                        IWebElement designstatussitelink = driver.FindElements(By.TagName("a")).First(element => element.Text == "Design Details");
                        designstatussitelink.Click();
                        IWebElement ddlrecStatus = driver.FindElement(By.Id("comment"));
                        ddlrecStatus.SendKeys(pilot);
                        IWebElement savespan = driver.FindElements(By.TagName("span")).First(element => element.Text == "Save");
                        savespan.Click();
                        //Checkin
                        IWebElement trialidentification = driver.FindElements(By.TagName("a")).First(element => element.Text == "Trial Identification");
                        trialidentification.Click();
                        IWebElement btnadmincheckin = driver.FindElements(By.TagName("span")).First(element => element.Text == "Scientific Check In");
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
        public void AddCTEPANDDCPIDTest()
        {
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\Add DCP and CTEP ID 20181217\dob-zip-cleanup-list(sanitized) 20181217.xlsx");
            //string status = "";
            string codetext = @"select
dw_study.nci_id,
dw_study.ctep_id,
dw_study.dcp_id
from dw_study; ";
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                NpgsqlCommand cmd = null;
                NpgsqlDataReader datareader = null;

                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\Add DCP and CTEP ID 20181217\CTEP ADDED.txt"))
                {
                    foreach (DataRow row in trialdata.Tables[0].Rows)
                    {
                        try
                        {
                            DataTable ccrDT = new DataTable();
                            //if (row[5].ToString() == "Open - No Longer Recruiting - Follow-up Only")
                            //{
                            //    status = "CLOSED_TO_ACCRUAL";
                            //}
                            //else
                            //{
                            //    status = "CLOSED_TO_ACCRUAL_AND_INTERVENTION";
                            //}

                            codetext = @"select
dw_study.nci_id,
dw_study.ctep_id,
dw_study.dcp_id,
dw_study.lead_org
from dw_study where nct_id = '" + row[0].ToString() + @"'";
                            cmd = new NpgsqlCommand(codetext, conn);
                            datareader = cmd.ExecuteReader();
                            ccrDT.Load(datareader);
                            sw.WriteLine(ccrDT.Rows[0].ItemArray[3].ToString());
                        }
                        catch (Exception ex)
                        {
                            sw.WriteLine("No records.");
                        }
                    }
                }
            }
        }


        [Test]
        public void CCRActiveTest()
        {
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\CCR Active Report\List of NCI studies close to accrual 20181210.xlsx");
            //string status = "";
            string codetext = @"select
dw_study.nci_id,
dw_study.ctep_id,
dw_study.dcp_id,
dw_close_to_accrual.status_date,
dw_active.status_date
from(select * from dw_study where REPLACE(ccr_id, '-', '') = '11C0061') dw_study
join(select * from dw_study_overall_status where status = 'CLOSED_TO_ACCRUAL') dw_close_to_accrual
on dw_close_to_accrual.nci_id = dw_study.nci_id
join(select * from dw_study_overall_status where status = 'ACTIVE') dw_active
on dw_active.nci_id = dw_study.nci_id; ";
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                NpgsqlCommand cmd = null;
                NpgsqlDataReader datareader = null;

                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\CCR Active Report\CCR Active Close to Accrual.txt"))
                {
                    foreach (DataRow row in trialdata.Tables[0].Rows)
                    {
                        try
                        {
                            DataTable ccrDT = new DataTable();
                            //if (row[5].ToString() == "Open - No Longer Recruiting - Follow-up Only")
                            //{
                            //    status = "CLOSED_TO_ACCRUAL";
                            //}
                            //else
                            //{
                            //    status = "CLOSED_TO_ACCRUAL_AND_INTERVENTION";
                            //}

                            codetext = @"select
dw_study.nci_id,
dw_study.ctep_id,
dw_study.dcp_id,
dw_close_to_accrual.status_date,
dw_active.status_date
from(select * from dw_study where REPLACE(ccr_id, '-', '') = '" + row[0].ToString() + @"') dw_study
join(select * from dw_study_overall_status where status in ('CLOSED_TO_ACCRUAL','CLOSED_TO_ACCRUAL_AND_INTERVENTION')) dw_close_to_accrual
on dw_close_to_accrual.nci_id = dw_study.nci_id
join(select * from dw_study_overall_status where status = 'ACTIVE') dw_active
on dw_active.nci_id = dw_study.nci_id order by dw_active.status_date; ";
                            cmd = new NpgsqlCommand(codetext, conn);
                            datareader = cmd.ExecuteReader();
                            ccrDT.Load(datareader);
                            sw.WriteLine(ccrDT.Rows[0].ItemArray[2].ToString());
                        }
                        catch (Exception ex)
                        {
                            sw.WriteLine("No records.");
                        }
                    }
                }
            }
        }


        [Test]
        public void OffholdDashboardTest()
        {
            EWDashboardCheck ew = new EWDashboardCheck();
            ApplicationUser user = new ApplicationUser();

            //ew.DashboardCheck(user);
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
            password.SendKeys("Prss_7890");
            password.Submit();
            IWebElement acceptclaim = driver.FindElement(By.Id("acceptDisclaimer"));
            acceptclaim.Click();


            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\PCD2100 Report\PCD2100 Report 20181214.xlsx");
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
                        //comment = "Per EW 85484 Anticipated Primary Completion Date 01/01/2100 was removed and N/A was selected";
                        comment = "Per EW#85484, Primary Completion Date 1/1/2100 is removed";


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
                        txtPCD.ExecuteScript("document.getElementById('startDate').setAttribute('value', '" + "12/1/2019" + "')");
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
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Single Participating Sites.xlsx");
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
            //Find participating site number
            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Network Trials 20181022.xlsx");
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
            //Delete sites
            IWebDriver driver = new ChromeDriver();
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

        [Test]
        public void NCTNReportTest4()
        {
            //Find ps status
            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Single Participating Sites 20181115.xlsx");
            //Do Loop
            using (var conn = new NpgsqlConnection(CTROConst.paconnString))
            {
                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN PS Status.txt"))
                {
                    foreach (DataTable table in trialdata.Tables)
                    {
                        sw.WriteLine(table.TableName);

                        foreach (DataRow row in table.Rows)
                        {
                            nciid = row.ItemArray[0].ToString();
                            organization = row.ItemArray[16].ToString();
                            sql = @"select
study_protocol.nci_id,
study_site_accrual_status.status_code,
study_site_accrual_status.status_date
from (select * from study_site_accrual_status where status_code in ('CLOSED_TO_ACCRUAL','CLOSED_TO_ACCRUAL_AND_INTERVENTION') and deleted = 'false') study_site_accrual_status
join
(select * from study_site where functional_code = 'TREATING_SITE') study_site
on study_site_accrual_status.study_site_identifier = study_site.identifier
join (select * from study_protocol where nci_id = '" + nciid + @"') study_protocol
on study_protocol.identifier = study_site.study_protocol_identifier
join healthcare_facility
on healthcare_facility.identifier = study_site.healthcare_facility_identifier
join (select * from organization where name = '" + organization + @"') organization
on organization.identifier = healthcare_facility.organization_identifier
order by study_protocol.nci_id";

                            NpgsqlCommand cmd = null;
                            NpgsqlDataReader datareader = null;
                            //NCI
                            cmd = new NpgsqlCommand(sql, conn);
                            datareader = cmd.ExecuteReader();
                            DataTable nciDT = new DataTable();
                            nciDT.Load(datareader);
                            if (nciDT.Rows.Count > 0)
                            {
                                sw.WriteLine(nciDT.Rows[0].ItemArray[1].ToString());
                            }
                            else
                            {
                                sw.WriteLine("");
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void NCTNReportTest5()
        {
            //Find cta history
            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Single Participating Sites.xlsx");
            //Do Loop
            using (var conn = new NpgsqlConnection(CTROConst.paconnString))
            {
                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN CTA History.txt"))
                {
                    foreach (DataTable table in trialdata.Tables)
                    {
                        sw.WriteLine(table.TableName);

                        foreach (DataRow row in table.Rows)
                        {
                            nciid = row.ItemArray[0].ToString();
                            organization = row.ItemArray[16].ToString();
                            sql = @"select
study_protocol.nci_id,
study_site_accrual_status.status_code,
study_site_accrual_status.status_date
from (select * from study_site_accrual_status where status_code in ('CLOSED_TO_ACCRUAL','CLOSED_TO_ACCRUAL_AND_INTERVENTION') 
	  and deleted = 'false' and status_date::date < '2009-01-01') study_site_accrual_status
join
(select * from study_site where functional_code = 'TREATING_SITE') study_site
on study_site_accrual_status.study_site_identifier = study_site.identifier
join (select * from study_protocol where nci_id = '" + nciid + @"') study_protocol
on study_protocol.identifier = study_site.study_protocol_identifier
join healthcare_facility
on healthcare_facility.identifier = study_site.healthcare_facility_identifier
join (select * from organization where name = '" + organization + @"') organization
on organization.identifier = healthcare_facility.organization_identifier
order by study_protocol.nci_id;";

                            NpgsqlCommand cmd = null;
                            NpgsqlDataReader datareader = null;
                            //NCI
                            cmd = new NpgsqlCommand(sql, conn);
                            datareader = cmd.ExecuteReader();
                            DataTable nciDT = new DataTable();
                            nciDT.Load(datareader);
                            if (nciDT.Rows.Count > 0)
                            {
                                sw.WriteLine(nciid + "----" + nciDT.Rows[0].ItemArray[2].ToString());
                            }
                            else
                            {
                                sw.WriteLine(nciid + "----");
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void NCTNReportTest6()
        {
            //Find cta history
            //Read Data
            string nciid = "NCI-2017-00086";
            string organization = "Wake Forest NCORP Research Base";
            string sql = "";
            DataSet trialdata = CTROFunctions.ReadExcelToDataSet(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN Single Participating Sites 20181115.xlsx");
            //Do Loop
            using (var conn = new NpgsqlConnection(CTROConst.paconnString))
            {
                conn.Open();
                using (StreamWriter sw = File.CreateText(@"C:\Users\panr2\Downloads\DataWarehouse\NCTN Report\NCTN CTA 20181115.txt"))
                {
                    foreach (DataTable table in trialdata.Tables)
                    {
                        sw.WriteLine(table.TableName);

                        foreach (DataRow row in table.Rows)
                        {
                            nciid = row.ItemArray[0].ToString();
                            organization = row.ItemArray[16].ToString();
                            sql = @"select 
study_protocol.nci_id,
study_overall_status.status_code,
study_overall_status.status_date
from (select * from study_protocol where nci_id = '" + nciid + @"') study_protocol
join (select * from study_overall_status where status_code in ('CLOSED_TO_ACCRUAL','CLOSED_TO_ACCRUAL_AND_INTERVENTION') and deleted = 'false') study_overall_status
on study_overall_status.study_protocol_identifier = study_protocol.identifier;";

                            NpgsqlCommand cmd = null;
                            NpgsqlDataReader datareader = null;
                            //NCI
                            cmd = new NpgsqlCommand(sql, conn);
                            datareader = cmd.ExecuteReader();
                            DataTable nciDT = new DataTable();
                            nciDT.Load(datareader);
                            if (nciDT.Rows.Count > 0)
                            {
                                sw.WriteLine(nciDT.Rows[0].ItemArray[1].ToString());
                            }
                            else
                            {
                                sw.WriteLine("");
                            }
                        }
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
    }
}
