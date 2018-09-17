﻿using CTROLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CTROEWSystem
{
    public partial class EWTicketTrageService : ServiceBase
    {
        private Timer tickettimer = null;
        public EWTicketTrageService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            tickettimer = new Timer();
            tickettimer.Interval = 10000; // every 3 minutes
            tickettimer.Elapsed += new System.Timers.ElapsedEventHandler(tickettimerTick);
            tickettimer.Enabled = true;
            Logging.WriteLog("XXXX", "ABC", "Test Start");
        }

        private void tickettimerTick(object sender, ElapsedEventArgs e)
        {
            //try
            //{
            //    EWFormatOriginalIncomingEmail ewFormat = new EWFormatOriginalIncomingEmail();
            //    string[] tickets = ewFormat.GetIDList("full_name is null and assigned_to_ is null").ToArray();
            //    ewFormat.BulkUpdate(tickets);
            //    EWTriageAccrual ewTriageAccrual = new EWTriageAccrual();
            //    ewTriageAccrual.BulkUpdate(tickets);
            //    EWTriageClinicalTrialsDotGov ewTriageClinicalTrialsDotGov = new EWTriageClinicalTrialsDotGov();
            //    ewTriageClinicalTrialsDotGov.BulkUpdate(tickets);
            //    EWTriageScientific ewEWTriageScientific = new EWTriageScientific();
            //    ewEWTriageScientific.BulkUpdate(tickets);
            //    EWTriageTSRFeedback ewTriageTSRFeedback = new EWTriageTSRFeedback();
            //    ewTriageTSRFeedback.BulkUpdate(tickets);
            //    EWTriageOnHoldTrials ewTriageOnHoldTrials = new EWTriageOnHoldTrials();
            //    ewTriageOnHoldTrials.BulkUpdate(tickets);
            //}
            //catch (Exception ex)
            //{
                Logging.WriteLog("XXXX", "ABC", "Test by Ran Pan");
            //}
        }

        protected override void OnStop()
        {
            tickettimer.Enabled = false;
            Logging.WriteLog("XXXX", "ABC", "Test Stop");
        }
    }
}
