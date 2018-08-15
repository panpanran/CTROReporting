using Attitude_Loose.EW;
using Attitude_Loose.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Test
{
    [TestFixture()]
    public class EWTest
    {
        [Test]
        public void TSRFeedback()
        {
            //EWContinueReview ewcr = new EWContinueReview();
            //ewcr.BulkUpdate("");
            EWTSRFeedback eWHome = new EWTSRFeedback();
            //eWHome.UpdateByID("81080");
            eWHome.BulkUpdate("assigned_to_=%27Ran%20Pan%27%20and category like '%2519%25' and%20assigned>%27" + DateTime.Now.ToString("yyyy-MM-dd") + "%27");
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
            string[] tickets = {"1", "EWREST_id='82839'", "3"};
            eWHome.BulkUpdate(tickets);
        }

    }
}