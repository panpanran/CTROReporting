using CTROLibrary.EW;
using CTROLibrary.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace CTROLibrary.CTRO
{
    #region Abstract CTROReport
    public abstract class CTROReport
    {
        public abstract DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS);
        public virtual DataTable CreateSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            conclusionDT = new DataTable();
            return new DataTable();
        }

        public virtual Email GetEmail(Report report, ApplicationUser user, DataSet bookDS)
        {
            Email email = new Email();
            email.Subject = report.ReportName + " Report " + string.Format("{0:MM/dd/yyyy}", DateTime.Now);
            email.To = user.Email;
            email.Body = report.Email.Body.Replace("reportname", report.ReportName);
            email.From = report.Email.From;
            return email;
        }

    }
    #endregion

    #region Turnaround
    public class TurnaroundReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();
            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.Load(datareader);
                outputDS.Tables.Add(CreateSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }

        public override DataTable CreateSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime tsrdate = new DateTime();
            DateTime accepteddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;

            outputDT.Columns.Add("overalldurations", typeof(Int32));
            outputDT.Columns.Add("onholdtime", typeof(Int32));
            outputDT.Columns.Add("processingtime", typeof(Int32));
            int overalldurations = 0;
            int onholdtime = -100;
            int processingtime = 0;

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    tsrdate = (DateTime)row["tsrdate"];
                    accepteddate = (DateTime)row["accepteddate"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (tsrdate >= accepteddate)
                        overalldurations = CTROFunctions.CountBusinessDays(accepteddate, tsrdate, CTROConst.Holidays);

                    //if (reactivateddate >= accepteddate && reactivateddate <= tsrdate)
                    //{
                    //    overalldurations = CTROFunctions.CountBusinessDays(reactivateddate, tsrdate, CTRPConst.Holidays);
                    //}


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= tsrdate && onholddate >= accepteddate)
                        {
                            onholdtime = tsrdate > offholddate
                                ? CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays)
                                : CTROFunctions.CountBusinessDays(onholddate, tsrdate, CTROConst.Holidays);
                        }
                        else
                        {
                            onholdtime = 0;
                        }
                    }
                    else
                    {
                        onholdtime = 0;
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("overalldurations"))
                    {
                        row.Table.Columns.Add("overalldurations", typeof(Int32));
                        row.Table.Columns.Add("onholdtime", typeof(Int32));
                        row.Table.Columns.Add("processingtime", typeof(Int32));
                    }
                    row["overalldurations"] = overalldurations;
                    row["onholdtime"] = onholdtime;
                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["onholdtime"] = Convert.ToInt32(outputDT.Rows[index]["onholdtime"]) + onholdtime;
                        outputDT.Rows[index]["processingtime"] = Convert.ToInt32(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                    //}
                    //CTROFunctions.SendEmail("dd", "dd", "ran.pan@nih.gov");
                }
                tempDT = outputDT.AsEnumerable()
                    //.Where(x => x.Field<string>("additionalcomments") == "")  //if compute the multi on-hold records
                    .GroupBy(x => x.Field<DateTime>("tsrdate").Date)
                    .Select(x => new { TSRDate = String.Format("{0:MM/dd/yyyy}", x.Key.Date), TSRNumber = x.Count(), AvgProcessingTime = String.Format("{0:.##}", x.Select(y => y.Field<int>("processingtime")).Average()) }).ToDataTable();
                tempDT.Rows.Add("Grand Total"
                    , outputDT.Rows.Count
                    , String.Format("{0:.##}", outputDT.AsEnumerable()
                    //.Where(x => x.Field<string>("additionalcomments") == "") //if compute the multi on-hold records
                    .Select(x => x.Field<int>("processingtime")).Average()));
                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override Email GetEmail(Report report, ApplicationUser user, DataSet bookDS)
        {
            IEnumerable<DataRow> originalRows = bookDS.Tables["Original"].AsEnumerable();
            IEnumerable<DataRow> amendmentRows = bookDS.Tables["Amendment"].AsEnumerable();
            IEnumerable<DataRow> abbreviatedRows = bookDS.Tables["Abbreviated"].AsEnumerable();
            string monthparam = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(originalRows.Max(x => x.Field<DateTime>("tsrdate").Date).Month);
            string oaanumberparam = (originalRows.Count() + amendmentRows.Count() + abbreviatedRows.Count()).ToString();
            string oaatimeparam = String.Format("{0:.##}", (originalRows.Sum(x => x.Field<int>("processingtime")) + amendmentRows.Sum(x => x.Field<int>("processingtime")) + abbreviatedRows.Sum(x => x.Field<int>("processingtime"))) / (double)(originalRows.Count() + amendmentRows.Count() + abbreviatedRows.Count()));
            string oanumberparam = (originalRows.Count() + amendmentRows.Count()).ToString();
            string oatimeparam = String.Format("{0:.##}", (originalRows.Sum(x => x.Field<int>("processingtime")) + amendmentRows.Sum(x => x.Field<int>("processingtime"))) / (double)(originalRows.Count() + amendmentRows.Count()));
            string body = report.Email.Body;
            Email email = new Email();
            email.Subject = report.ReportName + " Report " + string.Format("{0:MM/dd/yyyy}", DateTime.Now);
            email.To = user.Email;
            body = body.Replace("monthparam", monthparam);
            body = body.Replace("oaanumberparam", oaanumberparam);
            body = body.Replace("oaatimeparam", oaatimeparam);
            body = body.Replace("oanumberparam", oanumberparam);
            body = body.Replace("oatimeparam", oatimeparam);
            email.Body = body;
            email.From = report.Email.From;
            return email;
        }
    }
    #endregion

    #region Sponsor Not Match
    public class SponsorReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = reportSettings[0].Code;
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            nciDT.Load(datareader);
            nciDT.TableName = "NCI";
            outputDS.Tables.Add(nciDT);
            return outputDS;
        }
    }
    #endregion

    #region Phase NA
    public class PhaseNAReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = reportSettings[0].Code.Replace("startDate", startDate).Replace("endDate", endDate);
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            nciDT.Load(datareader);
            nciDT.TableName = "NCI";
            outputDS.Tables.Add(nciDT);
            return outputDS;
        }
    }
    #endregion

    #region Trial Processing

    public class TrialProcessingReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet rankDS)
        {
            //All
            DataSet outputDS = new DataSet();
            rankDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //SDA Abstraction
            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.Load(datareader);
                switch (reportSetting.Category)
                {
                    case "PDA Validation ":
                        tempDT.TableName = reportSetting.Category;
                        outputDS.Tables.Add(CreateValidationSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "PDA Abstraction ":
                        outputDS.Tables.Add(CreatePDAAbstractionSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "PDA QC ":
                        outputDS.Tables.Add(CreatePDAQCSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "PDA Summary":
                        tempDT.TableName = reportSetting.Category;
                        outputDS.Tables.Add(tempDT);
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "SDA Abstraction":
                        outputDS.Tables.Add(CreateSDAAbstractionSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "SDA QC":
                        outputDS.Tables.Add(CreateSDAQCSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "SDA Summary":
                        tempDT.TableName = reportSetting.Category;
                        outputDS.Tables.Add(tempDT);
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "Ticket":
                        tempDT.TableName = reportSetting.Category;
                        EWUserSupport eWUserSupport = new EWUserSupport();
                        List<Ticket> tickets = eWUserSupport.GetTickets("modified_date>%27" + startDate + "%27%20and%20modified_date<%27" + endDate + "%27");
                        outputDS.Tables.Add(CreateTicketSheet(tickets, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;

                }
            }

            //rankDS.Tables["SDA Abstraction"].Columns.Add("totalexpectedtime", typeof(double));
            //foreach (DataRow row in rankDS.Tables["SDA Abstraction"].Rows)
            //{
            //    row["totalexpectedtime"] = rankDS.Tables["SDA QC"].AsEnumerable().Where(x => x.Field<string>("username") == row["username"].ToString()).Select(x => x.Field<double>("expectedtime")).FirstOrDefault() + (double)row["expectedtime"];
            //}

            //rankDS.Tables["SDA QC"].Columns.Add("totalexpectedtime", typeof(double));
            //foreach (DataRow row in rankDS.Tables["SDA QC"].Rows)
            //{
            //    row["totalexpectedtime"] = rankDS.Tables["SDA Abstraction"].AsEnumerable().Where(x => x.Field<string>("username") == row["username"].ToString()).Select(x => x.Field<double>("expectedtime")).FirstOrDefault() + (double)row["expectedtime"];
            //}

            return outputDS;
        }

        public DataTable CreatePDAAbstractionSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }
                tempDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { username = x.Field<string>("loginname") })
                    .Select(x => new
                    {
                        x.Key.username,
                        original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                        originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                        amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                        abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33
                    }).OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33);
                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreatePDAQCSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }

                tempDT = outputDT.AsEnumerable()
                .GroupBy(x => new { username = x.Field<string>("loginname") })
                .Select(x => new
                {
                    x.Key.username,
                    original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                    originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                    amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                    abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.17
                }).
                OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.17);


                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateSDAAbstractionSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }
                tempDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { username = x.Field<string>("loginname") })
                    .Select(x => new
                    {
                        x.Key.username,
                        original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                        originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                        amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                        abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 2 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.35
                    }).OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 2 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.35);
                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateSDAQCSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }

                tempDT = outputDT.AsEnumerable()
                .GroupBy(x => new { username = x.Field<string>("loginname") })
                .Select(x => new
                {
                    x.Key.username,
                    original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                    originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                    amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                    abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 1.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25
                }).
                OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 1.5 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25);


                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateValidationSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DataTable outputDT = inputDT;
            DataTable tempDT = new DataTable();
            tempDT = outputDT.AsEnumerable()
            .GroupBy(x => new { username = x.Field<string>("loginname") })
            .Select(x => new
            {
                x.Key.username,
                original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25
            }).
            OrderBy(x => x.expectedtime).ToDataTable();
            tempDT.Rows.Add("Grand Total and Avg",
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.5 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25);


            conclusionDT = tempDT;
            conclusionDT.TableName = tablename;

            return outputDT;
        }

        public DataTable CreateTicketSheet(List<Ticket> tickets, string tablename, out DataTable conclusionDT)
        {
            DataTable outputTable = new DataTable();
            outputTable.Columns.Add("ticketid", typeof(string));
            outputTable.Columns.Add("fullname", typeof(string));
            outputTable.Columns.Add("email", typeof(string));
            outputTable.Columns.Add("summary", typeof(string));
            outputTable.Columns.Add("assignedto", typeof(string));
            outputTable.Columns.Add("state", typeof(string));
            outputTable.Columns.Add("category", typeof(string));
            outputTable.Columns.Add("createddate", typeof(string));
            outputTable.Columns.Add("modifiedby", typeof(string));
            outputTable.Columns.Add("modifieddate", typeof(string));
            outputTable.Columns.Add("organizationname", typeof(string));

            foreach (Ticket ticket in tickets)
            {
                outputTable.Rows.Add(
                    ticket.TicketId,
                    ticket.FullName,
                    ticket.Email,
                    ticket.Summary,
                    ticket.AssignedTo,
                    ticket.State,
                    ticket.Category,
                    ticket.Created_date,
                    ticket.Modified_by,
                    ticket.Modified_date,
                    ticket.OrganizationName);
            }

            conclusionDT = new DataTable();
            conclusionDT.Columns.Add("user", typeof(string));
            conclusionDT.Columns.Add("closedticket", typeof(string));
            conclusionDT.Columns.Add("unclosedticket", typeof(string));

            conclusionDT.TableName = tablename;

            foreach (string name in tickets.Select(m => m.AssignedTo).Distinct())
            {
                conclusionDT.Rows.Add(name
                    , tickets.Where(m => m.AssignedTo == name && m.State == "Closed").Count()
                    , tickets.Where(m => m.AssignedTo == name && m.State != "Closed").Count());
            }

            conclusionDT.Rows.Add("Grand Total"
                , tickets.Where(m => m.State == "Closed").Count()
                , tickets.Where(m => m.State != "Closed").Count());
            outputTable.TableName = tablename;
            return outputTable;
        }

    }

    #endregion

    #region Onhold
    public class OnholdReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();
            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = reportSettings[0].Code.Replace("startDate", startDate);
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            nciDT.Load(datareader);
            nciDT.TableName = reportSettings[0].Category;
            outputDS.Tables.Add(nciDT);
            return outputDS;
        }
    }
    #endregion

    #region Offhold
    public class OffholdReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = string.Empty;
            if (DateTime.Now.Date.DayOfWeek == DayOfWeek.Monday)
            {
                codetext = reportSettings[0].Code.Replace("offholddate", "now()::date - 3");
            }
            else
            {
                codetext = reportSettings[0].Code.Replace("offholddate", "now()::date - 1");
            }
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable trialdata = new DataTable();
            trialdata.Load(datareader);

            outputDS.Tables.Add();
            outputDS.Tables[0].TableName = reportSettings[0].Category;

            outputDS.Tables[0].Columns.Add("nciid", typeof(string));
            outputDS.Tables[0].Columns.Add("submissionnumber", typeof(string));
            outputDS.Tables[0].Columns.Add("accepted", typeof(DateTime));
            outputDS.Tables[0].Columns.Add("onholddate", typeof(string));
            outputDS.Tables[0].Columns.Add("offholddate", typeof(string));
            outputDS.Tables[0].Columns.Add("onholdreasontype", typeof(string));
            outputDS.Tables[0].Columns.Add("onholdreason", typeof(string));
            outputDS.Tables[0].Columns.Add("realexpected", typeof(DateTime));

            DateTime startdate = new DateTime();
            DateTime enddate;
            DateTime reactivateddate;
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
                nciid = workloadrow[0].ToString();

                string code = @"select 
dw_study.nci_id,
accepted.submission_number, 
accepted.date,
dw_study_on_hold_status.on_hold_date,
dw_study_on_hold_status.off_hold_date,
reactivated.date, 
dw_study_on_hold_status.reason,
dw_study_on_hold_status.reason_description
from(select * from dw_study where nci_id = 'nciidpara') dw_study
left join(select * from dw_study_milestone where name = 'Submission Acceptance Date') accepted
on accepted.nci_id = dw_study.nci_id
and accepted.submission_number = dw_study.submission_number
left join(select* from dw_study_milestone where name = 'Submission Reactivated Date') reactivated
on reactivated.nci_id = dw_study.nci_id
and reactivated.submission_number = dw_study.submission_number
left join(select* from dw_study_on_hold_status) dw_study_on_hold_status
on dw_study_on_hold_status.nci_id = dw_study.nci_id
order by dw_study.nci_id, dw_study_on_hold_status.off_hold_date;";
                cmd = new NpgsqlCommand(code.Replace("nciidpara", nciid), conn);
                datareader = cmd.ExecuteReader();
                DataTable nciDT = new DataTable();
                nciDT.Load(datareader);
                int totaldays = 0;


                for (int i = 0; i < nciDT.Rows.Count; i++)
                {
                    onholdflag = 0;
                    if (i == 0)
                    {
                        accepteddate = string.IsNullOrEmpty(nciDT.Rows[i].ItemArray[2].ToString()) ? new DateTime() : (DateTime)nciDT.Rows[i].ItemArray[2];
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
                            if (offholddate > startdate)
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
                            try
                            {
                                submissionnumber = nciDT.Rows[0].ItemArray[1].ToString();
                                onholdreasontype = nciDT.Rows[0].ItemArray[6].ToString();
                                onholdreason = nciDT.Rows[0].ItemArray[7].ToString();
                                outputDS.Tables[0].Rows.Add(nciid, submissionnumber, accepteddate,
                                    onholddate.Date == Convert.ToDateTime("1/1/0001") ? null : onholddate.ToShortDateString(),
                                    offholddate.Date == Convert.ToDateTime("1/1/0001") ? null : offholddate.ToShortDateString(),
                                    onholdreasontype, onholdreason, enddate);

                            }
                            catch (Exception ex)
                            {
                                //Logging.WriteLog("EWDashboardCheck", "GenerateReport", "nciiid:" + nciid + ", error message: " + ex.Message);
                            }
                        }
                        break;
                }
            }

            return outputDS;
        }
    }
    #endregion

    #region Reactivated
    public class ReactivatedReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = string.Empty;
            if (DateTime.Now.Date.DayOfWeek == DayOfWeek.Monday)
            {
                codetext = reportSettings[0].Code.Replace("reactivateddate", "now()::date - 3");
            }
            else
            {
                codetext = reportSettings[0].Code.Replace("reactivateddate", "now()::date - 1");
            }
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            nciDT.Load(datareader);
            nciDT.TableName = reportSettings[0].Category;
            outputDS.Tables.Add(nciDT);
            return outputDS;
        }
    }
    #endregion

    #region TSRSent
    public class TSRSentReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = string.Empty;
            if (DateTime.Now.Date.DayOfWeek == DayOfWeek.Monday)
            {
                codetext = reportSettings[0].Code.Replace("tsrDate", "now()::date - 3");
            }
            else
            {
                codetext = reportSettings[0].Code.Replace("tsrDate", "now()::date - 1");
            }
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            nciDT.Load(datareader);
            nciDT.TableName = reportSettings[0].Category;
            outputDS.Tables.Add(nciDT);
            return outputDS;
        }
    }
    #endregion

    #region PDA Abstraction and QC
    public class PDAAbstractorReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }
    }

    #endregion

    #region Biomarker
    public class SDABiomarkerReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }
    }

    #endregion

    #region Sponsor NCI with Actual PCD
    public class SponsorNCIwithActualPCDReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string codetext = reportSettings[0].Code;
            cmd = new NpgsqlCommand(codetext, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            nciDT.Load(datareader);
            nciDT.TableName = "NCI";
            outputDS.Tables.Add(nciDT);
            return outputDS;
        }
    }

    #endregion

    #region zeroaccrual
    public class Zeroaccrualreport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }

    }

    #endregion

    #region National Trials Missing CTEP ID
    public class NationalTrialsMissingCTEPIDReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }

    }

    #endregion

    #region Trials with incorrect NCT ID Report
    public class TrialswithincorrectNCTIDReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }

    }

    #endregion

    #region Dashboard Expectation Date Check Report
    public class DashboardExpectationDateCheckReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code;

                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();

                EWDashboardCheck ew = new EWDashboardCheck();
                tempDT = ew.DashboardCheck(codetext);
                tempDT.TableName = reportSetting.Category;
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }

    }

    #endregion

    #region User Support Trial Processing Report

    public class UserSupportTrialProcessingReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet rankDS)
        {
            //All
            DataSet outputDS = new DataSet();
            rankDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //SDA Abstraction
            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.Load(datareader);
                tempDT.TableName = reportSetting.Category;
                tempconclusionDT.TableName = reportSetting.Category;

                if (tempDT.Rows.Count > 0)
                {
                    switch (reportSetting.Category)
                    {
                        case "Validation":
                            outputDS.Tables.Add(CreateValidationSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                            rankDS.Tables.Add(tempconclusionDT);
                            break;
                        case "Abstraction":
                            outputDS.Tables.Add(CreateAbstractionSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                            rankDS.Tables.Add(tempconclusionDT);
                            break;
                        case "Ticket":
                            EWUserSupport eWUserSupport = new EWUserSupport();
                            List<Ticket> tickets = eWUserSupport.GetTickets("modified_date>%27" + startDate + "%27%20and%20modified_date<%27" + endDate + "%27%20and%20(%20assigned_to_=%27Renae%20Brunetto%27%20or%20assigned_to_=%27Chessie%20Jones%27%20or%20assigned_to_=%27Bobbie Sanders%27)");
                            outputDS.Tables.Add(CreateTicketSheet(tickets, reportSetting.Category, out tempconclusionDT));
                            rankDS.Tables.Add(tempconclusionDT);
                            break;
                        case "Summary":
                            outputDS.Tables.Add(tempDT);
                            break;
                    }
                }
                else
                {
                    outputDS.Tables.Add(tempDT);
                    rankDS.Tables.Add(tempconclusionDT);
                }
            }

            return outputDS;
        }

        public DataTable CreateAbstractionSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }
                tempDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { username = x.Field<string>("loginname") })
                    .Select(x => new
                    {
                        x.Key.username,
                        original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                        originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                        amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                        abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33
                    }).OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33);
                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateTicketSheet(List<Ticket> tickets, string tablename, out DataTable conclusionDT)
        {
            DataTable outputTable = new DataTable();
            outputTable.Columns.Add("ticketid", typeof(string));
            outputTable.Columns.Add("fullname", typeof(string));
            outputTable.Columns.Add("email", typeof(string));
            outputTable.Columns.Add("summary", typeof(string));
            outputTable.Columns.Add("assignedto", typeof(string));
            outputTable.Columns.Add("state", typeof(string));
            outputTable.Columns.Add("category", typeof(string));
            outputTable.Columns.Add("createddate", typeof(string));
            outputTable.Columns.Add("modifiedby", typeof(string));
            outputTable.Columns.Add("modifieddate", typeof(string));
            outputTable.Columns.Add("organizationname", typeof(string));

            foreach (Ticket ticket in tickets)
            {
                outputTable.Rows.Add(
                    ticket.TicketId,
                    ticket.FullName,
                    ticket.Email,
                    ticket.Summary,
                    ticket.AssignedTo,
                    ticket.State,
                    ticket.Category,
                    ticket.Created_date,
                    ticket.Modified_by,
                    ticket.Modified_date,
                    ticket.OrganizationName);
            }

            conclusionDT = new DataTable();
            conclusionDT.Columns.Add("user", typeof(string));
            conclusionDT.Columns.Add("closedticket", typeof(string));
            conclusionDT.Columns.Add("unclosedticket", typeof(string));
            conclusionDT.Columns.Add("total", typeof(string));

            conclusionDT.TableName = tablename;

            foreach (string name in tickets.Select(m => m.AssignedTo).Distinct())
            {
                conclusionDT.Rows.Add(name
                    , tickets.Where(m => m.AssignedTo == name && m.State == "Closed").Count()
                    , tickets.Where(m => m.AssignedTo == name && m.State != "Closed").Count()
                    , tickets.Where(m => m.AssignedTo == name).Count());
            }

            conclusionDT.Rows.Add("Grand Total"
                , tickets.Where(m => m.State == "Closed").Count()
                , tickets.Where(m => m.State != "Closed").Count()
                , tickets.Count());
            outputTable.TableName = tablename;
            return outputTable;
        }

        public DataTable CreateValidationSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DataTable outputDT = inputDT;
            DataTable tempDT = new DataTable();
            tempDT = outputDT.AsEnumerable()
            .GroupBy(x => new { username = x.Field<string>("loginname") })
            .Select(x => new
            {
                x.Key.username,
                original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25
            }).
            OrderBy(x => x.expectedtime).ToDataTable();
            tempDT.Rows.Add("Grand Total and Avg",
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.5 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25);


            conclusionDT = tempDT;
            conclusionDT.TableName = tablename;

            return outputDT;
        }
    }

    #endregion

    #region PDA Trial Processing Report

    public class PDATrialProcessingReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet rankDS)
        {
            //All
            DataSet outputDS = new DataSet();
            rankDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //SDA Abstraction
            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.Load(datareader);
                switch (reportSetting.Category)
                {
                    case "Validation":
                        tempDT.TableName = reportSetting.Category;
                        outputDS.Tables.Add(CreateValidationSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "Abstraction":
                        outputDS.Tables.Add(CreateAbstractionSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "QC":
                        outputDS.Tables.Add(CreateQCSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "Summary":
                        tempDT.TableName = reportSetting.Category;
                        outputDS.Tables.Add(tempDT);
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "Ticket":
                        tempDT.TableName = reportSetting.Category;
                        EWUserSupport eWUserSupport = new EWUserSupport();
                        List<Ticket> tickets = eWUserSupport.GetTickets("modified_date>%27" + startDate + "%27%20and%20modified_date<%27" + endDate + "%27%20and%20(%20assigned_to_=%27Jaliza Cabral%27%20or%20assigned_to_=%27Hannah Gill%27" +
                            "%20or%20assigned_to_=%27Temisan Otubu%27%20or%20assigned_to_=%27Kirsten Larco%27%20or%20assigned_to_=%27Elena Gebeniene%27%20or%20assigned_to_=%27Iryna Asipenka%27)");
                        outputDS.Tables.Add(CreateTicketSheet(tickets, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;

                }
            }

            return outputDS;
        }

        public DataTable CreateAbstractionSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }
                tempDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { username = x.Field<string>("loginname") })
                    .Select(x => new
                    {
                        x.Key.username,
                        original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                        originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                        amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                        abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33
                    }).OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33);
                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateQCSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }

                tempDT = outputDT.AsEnumerable()
                .GroupBy(x => new { username = x.Field<string>("loginname") })
                .Select(x => new
                {
                    x.Key.username,
                    original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                    originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                    amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                    abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.17
                }).
                OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.17);


                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateValidationSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DataTable outputDT = inputDT;
            DataTable tempDT = new DataTable();
            tempDT = outputDT.AsEnumerable()
            .GroupBy(x => new { username = x.Field<string>("loginname") })
            .Select(x => new
            {
                x.Key.username,
                original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25
            }).
            OrderBy(x => x.expectedtime).ToDataTable();
            tempDT.Rows.Add("Grand Total and Avg",
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 0.5 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25);


            conclusionDT = tempDT;
            conclusionDT.TableName = tablename;

            return outputDT;
        }

        public DataTable CreateTicketSheet(List<Ticket> tickets, string tablename, out DataTable conclusionDT)
        {
            DataTable outputTable = new DataTable();
            outputTable.Columns.Add("ticketid", typeof(string));
            outputTable.Columns.Add("fullname", typeof(string));
            outputTable.Columns.Add("email", typeof(string));
            outputTable.Columns.Add("summary", typeof(string));
            outputTable.Columns.Add("assignedto", typeof(string));
            outputTable.Columns.Add("state", typeof(string));
            outputTable.Columns.Add("category", typeof(string));
            outputTable.Columns.Add("createddate", typeof(string));
            outputTable.Columns.Add("modifiedby", typeof(string));
            outputTable.Columns.Add("modifieddate", typeof(string));
            outputTable.Columns.Add("organizationname", typeof(string));

            foreach (Ticket ticket in tickets)
            {
                outputTable.Rows.Add(
                    ticket.TicketId,
                    ticket.FullName,
                    ticket.Email,
                    ticket.Summary,
                    ticket.AssignedTo,
                    ticket.State,
                    ticket.Category,
                    ticket.Created_date,
                    ticket.Modified_by,
                    ticket.Modified_date,
                    ticket.OrganizationName);
            }

            conclusionDT = new DataTable();
            conclusionDT.Columns.Add("user", typeof(string));
            conclusionDT.Columns.Add("closedticket", typeof(string));
            conclusionDT.Columns.Add("unclosedticket", typeof(string));

            conclusionDT.TableName = tablename;

            foreach (string name in tickets.Select(m => m.AssignedTo).Distinct())
            {
                conclusionDT.Rows.Add(name
                    , tickets.Where(m => m.AssignedTo == name && m.State == "Closed").Count()
                    , tickets.Where(m => m.AssignedTo == name && m.State != "Closed").Count());
            }

            conclusionDT.Rows.Add("Grand Total"
                , tickets.Where(m => m.State == "Closed").Count()
                , tickets.Where(m => m.State != "Closed").Count());
            outputTable.TableName = tablename;
            return outputTable;
        }

    }

    #endregion

    #region SDA Trial Processing Report

    public class SDATrialProcessingReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet rankDS)
        {
            //All
            DataSet outputDS = new DataSet();
            rankDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //SDA Abstraction
            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.Load(datareader);
                switch (reportSetting.Category)
                {
                    case "Abstraction":
                        outputDS.Tables.Add(CreateAbstractionSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "QC":
                        outputDS.Tables.Add(CreateQCSheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "QA":
                        outputDS.Tables.Add(CreateQASheet(tempDT, reportSetting.Category, out tempconclusionDT));
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                    case "Summary":
                        tempDT.TableName = reportSetting.Category;
                        outputDS.Tables.Add(tempDT);
                        rankDS.Tables.Add(tempconclusionDT);
                        break;
                }
            }

            return outputDS;
        }

        public DataTable CreateAbstractionSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }
                tempDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { username = x.Field<string>("loginname") })
                    .Select(x => new
                    {
                        x.Key.username,
                        original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                        originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                        amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                        abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                        expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 2 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.35
                    }).OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 2 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.35);
                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateQCSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DateTime starttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;
            outputDT.Columns.Add("processingtime", typeof(TimeSpan));
            TimeSpan overalldurations = new TimeSpan();
            TimeSpan onholdtime = new TimeSpan();
            TimeSpan processingtime = new TimeSpan();

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    onholdtime = new TimeSpan();
                    starttime = (DateTime)row["startedtime"];
                    endtime = (DateTime)row["completedtime"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (endtime >= starttime)
                    {
                        overalldurations = endtime - starttime;
                        if (overalldurations.Days > 1)
                        {
                            //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                            overalldurations = overalldurations - TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTROConst.Holidays));
                        }
                    }


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= endtime && onholddate >= starttime)
                        {
                            onholdtime = offholddate - onholddate;
                            if (onholdtime.Days > 1)
                            {
                                //TimeSpan ss = TimeSpan.FromDays(overalldurations.Days + 2 - CTROFunctions.CountBusinessDays(starttime, endtime, CTRPConst.Holidays));
                                onholdtime = onholdtime - TimeSpan.FromDays(onholdtime.Days + 2 - CTROFunctions.CountBusinessDays(onholddate, offholddate, CTROConst.Holidays));
                            }

                        }
                    }

                    processingtime = overalldurations - onholdtime;

                    if (!row.Table.Columns.Contains("processingtime"))
                    {
                        row.Table.Columns.Add("processingtime", typeof(TimeSpan));
                    }

                    row["processingtime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]));

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["additionalcomments"] += "Additional On-Hold " + row["onholddate"].ToString() + " - " + row["offholddate"].ToString() + ": " + row["onholddescription"];
                        outputDT.Rows[index]["processingtime"] = (TimeSpan)(outputDT.Rows[index]["processingtime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }

                tempDT = outputDT.AsEnumerable()
                .GroupBy(x => new { username = x.Field<string>("loginname") })
                .Select(x => new
                {
                    x.Key.username,
                    original = x.Where(y => y.Field<string>("trialtype") == "Original").Count(),
                    originalavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Original").Select(z => z.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    amendment = x.Where(y => y.Field<string>("trialtype") == "Amendment").Count(),
                    amendmentavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    abbreviated = x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count(),
                    abbreviatedavgtime = String.Format("{0:.##}", x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                    expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 1.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25
                }).
                OrderBy(x => x.expectedtime).ToDataTable();
                tempDT.Rows.Add("Grand Total and Avg",
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
                String.Format("{0:.##}", outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Select(y => y.Field<TimeSpan>("processingtime").TotalHours).DefaultIfEmpty().Average()),
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 1.5 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.5 +
                outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.25);


                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateQASheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            DataTable outputDT = inputDT.Copy();
            DataTable tempDT = new DataTable();
            outputDT.TableName = tablename;

            try
            {
                tempDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { tsrer = x[7].ToString() })
                    .Select(x => new
                    {
                        x.Key.tsrer,
                        monday = x.Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Monday).Count(),
                        tueday = x.Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Tuesday).Count(),
                        wednesday = x.Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Wednesday).Count(),
                        thursday = x.Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Thursday).Count(),
                        friday = x.Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Friday).Count(),
                        total = x.Count()
                    }).ToDataTable();

                tempDT.Rows.Add("Grand Total",
                    outputDT.AsEnumerable().Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Monday).Count(),
                    outputDT.AsEnumerable().Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Tuesday).Count(),
                    outputDT.AsEnumerable().Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Wednesday).Count(),
                    outputDT.AsEnumerable().Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Thursday).Count(),
                    outputDT.AsEnumerable().Where(y => Convert.ToDateTime(y[6]).DayOfWeek == DayOfWeek.Friday).Count(),
                    outputDT.AsEnumerable().Count());

                conclusionDT = tempDT;
                conclusionDT.TableName = tablename;
                return outputDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

    #endregion

    #region Lead Disease Flag Report
    public class LeadDiseaseFlagReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                //abbreviated
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }
    }

    #endregion

    #region No DT4 Anatomical Site Report
    public class NoDT4AnatomicalSiteReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }
    }

    #endregion

    #region CCR Trials Report
    public class CCRTrialsReport : CTROReport
    {
        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();

            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;

            foreach (ReportSetting reportSetting in reportSettings)
            {
                string codetext = reportSetting.Code;
                cmd = new NpgsqlCommand(codetext, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempDT = new DataTable();
                DataTable tempconclusionDT = new DataTable();
                tempDT.TableName = reportSetting.Category;
                tempDT.Load(datareader);
                outputDS.Tables.Add(tempDT);
                conclusionDS.Tables.Add(tempconclusionDT);
            }

            return outputDS;
        }
    }

    #endregion

}