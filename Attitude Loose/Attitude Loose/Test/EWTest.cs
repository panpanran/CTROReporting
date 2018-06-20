using Attitude_Loose.CTRO;
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
            eWHome.BulkUpdate("assigned_to_=%27Ran%20Pan%27%20and category=19 and%20assigned>%27" + DateTime.Now.ToString("yyyy-MM-dd") + "%27");
        }

        [Test]
        public void Zeroaccrual()
        {
            EWZeroaccrual eWHome = new EWZeroaccrual();
            eWHome.BulkAssigneTo("summary like '%Trials with Zero Accruals 2017%' and assigned_to_='Julia Shpigenur'");
        }
    }
}