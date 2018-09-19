using CTRPReporting.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace CTRPReporting.CTRO
{
    public abstract class CTROReport
    {
        public abstract DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet conclusionDS);
        public virtual DataTable CreateSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
        {
            conclusionDT = new DataTable();
            return new DataTable();
        }
    }

    #region turnaround
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
    }
    #endregion

    #region sponsor not match
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

    #region PDA Workload
//    public class PDAAbstractionReport : CTRPReport
//    {
//        public override DataTable CreateSheet(DataTable inputDT, string tablename, out DataTable conclusionDT)
//        {
//            DateTime completeddate = new DateTime();
//            DateTime starteddate = new DateTime();
//            DateTime onholddate = new DateTime();
//            DateTime offholddate = new DateTime();
//            //DateTime reactivateddate = new DateTime();
//            DataTable outputDT = inputDT.Clone();
//            DataTable dailyDT = new DataTable();
//            conclusionDT = new DataTable();

//            outputDT.Columns.Add("worknumber", typeof(Int32));
//            outputDT.Columns.Add("worktime", typeof(Int32));
//            double overalldurations = 0;
//            double onholdtime = -100;
//            double processingtime = 0;

//            try
//            {
//                foreach (DataRow row in inputDT.Rows)
//                {
//                    completeddate = (DateTime)row["completeddate"];
//                    starteddate = (DateTime)row["starteddate"];
//                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
//                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
//                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
//                    if (completeddate >= starteddate)
//                    {
//                        overalldurations = (completeddate - starteddate).TotalMinutes;
//                    }

//                    if (offholddate >= onholddate)
//                    {
//                        //exclude the onholddate not within processing
//                        if (onholddate <= completeddate && onholddate >= starteddate)
//                        {
//                            onholdtime = completeddate > offholddate
//                                ? (offholddate - onholddate).TotalMinutes
//                                : (completeddate - onholddate).TotalMinutes;
//                        }
//                        else
//                        {
//                            onholdtime = 0;
//                        }
//                    }
//                    else
//                    {
//                        onholdtime = 0;
//                    }

//                    processingtime = overalldurations - onholdtime;

//                    if (!row.Table.Columns.Contains("worktime"))
//                    {
//                        row.Table.Columns.Add("worktime", typeof(Int32));
//                    }
//                    row["worktime"] = processingtime;

//                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
//x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]) && x.Field<string>("loginname") == row["loginname"].ToString());

//                    if (Duplicate.Count() == 1)
//                    {
//                        int index = outputDT.Rows.IndexOf(Duplicate.First());
//                        outputDT.Rows[index]["worktime"] = Convert.ToInt32(outputDT.Rows[index]["worktime"]) - onholdtime;
//                    }
//                    else
//                    {
//                        outputDT.ImportRow(row);
//                    }
//                }

//                dailyDT = outputDT.AsEnumerable()
//                .GroupBy(x => new { loginname = x.Field<string>("loginname"), completeddate = x.Field<DateTime>("completeddate").Date })
//                .Select(x => new
//                {
//                    x.Key.loginname,
//                    completeddate = Convert.ToInt32(String.Format("{0:yyyyMMdd}", x.Key.completeddate)),
//                    workhour = (x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "Abstraction").Count() * 2 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "Abstraction").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "Abstraction").Count() * 0.35
//                    + x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "QC").Count() * 1.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "QC").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "QC").Count() * 0.25).ToString(),
//                    worktime = String.Format("{0:.##}", x.Select(y => y.Field<int>("worktime")).Average())
//                }).ToDataTable();
//                dailyDT.TableName = tablename;

//                conclusionDT = outputDT.AsEnumerable()
//                    .GroupBy(x => new { loginname = x.Field<string>("loginname") }).Select(x => new
//                    {
//                        x.Key.loginname,
//                        workhour = (x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "Abstraction").Count() * 2 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "Abstraction").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "Abstraction").Count() * 0.35
//                    + x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "QC").Count() * 1.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "QC").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "QC").Count() * 0.25).ToString(),
//                        worktime = String.Format("{0:.##}", x.Select(y => y.Field<int>("worktime")).Average())
//                    }).ToDataTable();
//                conclusionDT.TableName = tablename;

//                return dailyDT;
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//        }

//        public override DataSet CreateBook(NpgsqlConnection conn, string startDate, string endDate, ReportSetting[] reportSettings, out DataSet rankDS)
//        {
//            //All
//            DataSet outputDS = new DataSet();
//            rankDS = new DataSet();
//            //All

//            NpgsqlCommand cmd = null;
//            NpgsqlDataReader datareader = null;
//            //NCI
//            string admin_abstraction_text = reportSettings[0].Code.Replace("startDate", startDate).Replace("endDate", endDate);
//            cmd = new NpgsqlCommand(admin_abstraction_text, conn);
//            datareader = cmd.ExecuteReader();
//            DataTable nciDT = new DataTable();
//            DataTable rankDT = new DataTable();
//            nciDT.Load(datareader);
//            outputDS.Tables.Add(CreateSheet(nciDT, reportSettings[0].Category, out rankDT));
//            rankDS.Tables.Add(rankDT);
//            return outputDS;
//        }
//    }
//    public class SDAAbstractionReport : PDAAbstractionReport
//    {
//    }
//    public class PDAQCReport : PDAAbstractionReport
//    {
//    }
//    public class SDAQCReport : PDAAbstractionReport
//    {
//    }

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
                expectedtime = x.Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 + x.Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33
            }).
            OrderBy(x => x.expectedtime).ToDataTable();
            tempDT.Rows.Add("Grand Total and Avg",
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count().ToString(),
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Original").Count() * 1 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Amendment").Count() * 0.75 +
            outputDT.AsEnumerable().Where(y => y.Field<string>("trialtype") == "Abbreviated").Count() * 0.33);


            conclusionDT = tempDT;
            conclusionDT.TableName = tablename;

            return outputDT;
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
            string codetext = reportSettings[0].Code;
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
            string codetext = reportSettings[0].Code;
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

}