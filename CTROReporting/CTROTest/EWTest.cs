using CTROLibrary;
using CTROLibrary.EW;
using CTROLibrary.Model;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace CTROTest
{
    [TestFixture()]
    public class EWTest
    {
        [Test]
        public void ProcessingTest()
        {
            EWFormatOriginalIncomingEmail ewFormat = new EWFormatOriginalIncomingEmail();
            string[] tickets = ewFormat.GetIDList("full_name is null and assigned_to_ is null").ToArray();
            ewFormat.BulkUpdate(tickets);
            EWTriageAccrual ewTriageAccrual = new EWTriageAccrual();
            ewTriageAccrual.BulkUpdate(tickets);
            EWTriageClinicalTrialsDotGov ewTriageClinicalTrialsDotGov = new EWTriageClinicalTrialsDotGov();
            ewTriageClinicalTrialsDotGov.BulkUpdate(tickets);
            EWTriageScientific ewEWTriageScientific = new EWTriageScientific();
            ewEWTriageScientific.BulkUpdate(tickets);
            EWTriageTSRFeedback ewTriageTSRFeedback = new EWTriageTSRFeedback();
            ewTriageTSRFeedback.BulkUpdate(tickets);
            EWTriageOnHoldTrials ewTriageOnHoldTrials = new EWTriageOnHoldTrials();
            ewTriageOnHoldTrials.BulkUpdate(tickets);
        }


        [Test]
        public void LoggingTest()
        {
            Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "Test Start");
        }


        [Test]
        public void TSRFeedback()
        {
            //string nciid = Regex.Match("RE: NCI CTRP: Trial AMENDMENT TSR for REVIEW for NCI-2017-00101, 201701084", "NCI-.*?,").Value.Replace(",","");
            //TSRFeedbackTest("86330", "NCI-2018-02345");
            EWSolutionTSRFeedback eWHome = new EWSolutionTSRFeedback();
            Ticket ticket = eWHome.GetById("87771");
            eWHome.Update(ticket);
            //ApplicationUser user = new ApplicationUser();
            //eWHome.BulkUpdate("assigned_to_=%27Ran%20Pan%27%20and category like '%2519%25' and%20modified_by%20not%20like%20%27%25panr2%25%27", user);
        }

        [Test]
        public void Zeroaccrual()
        {
            EWZeroaccrual eWHome = new EWZeroaccrual();
            eWHome.BulkAssigneTo("summary like '%Trials with Zero Accruals 2017%' and assigned_to_='Julia Shpigenur'");
        }

        [Test]
        public void OriginalIncomingEmailFormat()
        {
            EWFormatOriginalIncomingEmail eWHome = new EWFormatOriginalIncomingEmail();
            string[] tickets = eWHome.GetIDList("full_name is null and assigned_to_ is null").ToArray();
            eWHome.BulkUpdate(tickets);
        }

        [Test]
        public void TriageAccrual()
        {
            EWTriageAccrual eWHome = new EWTriageAccrual();
            string[] tickets = eWHome.GetIDList("category%20is%20null%20and%20assigned_to_%20is%20null%20and%20email%20is%20not%20null%20and%20modified_by=%27panr2%27").ToArray();

            eWHome.BulkUpdate(tickets);
        }

        [Test]
        public void TriageClinicalTrialsDotGov()
        {
            EWTriageClinicalTrialsDotGov eWHome = new EWTriageClinicalTrialsDotGov();
            string[] tickets = eWHome.GetIDList("category%20is%20null%20and%20assigned_to_%20is%20null%20and%20email%20is%20not%20null%20and%20modified_by=%27panr2%27").ToArray();
            eWHome.BulkUpdate(tickets);
        }

        [Test]
        public void TriageScientific()
        {
            EWTriageScientific eWHome = new EWTriageScientific();
            string[] tickets = { "1", "EWREST_id='82839'", "3" };
            eWHome.BulkUpdate(tickets);
        }

    }
}
