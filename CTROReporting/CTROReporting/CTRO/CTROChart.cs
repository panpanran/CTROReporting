using CTRPReporting.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CTRPReporting.CTRO
{
    public abstract class CTROChart
    {
        public abstract string[] CreateBook(NpgsqlConnection conn, string startDate, string endDate, CTRPReporting.Models.Chart chart, out string[] Xaxis, out List<Dictionary<string, string>> Yaxis);
        public virtual DataTable CreateSheet(DataTable inputDT, string tablename)
        {
            return new DataTable();
        }
    }


    public class SDAWorkloadChart : CTROChart
    {
        public DataTable CreateDailySheet(DataTable inputDT, string tablename)
        {
            DateTime completeddate = new DateTime();
            DateTime starteddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable dailyDT = new DataTable();

            outputDT.Columns.Add("workhour", typeof(Int32));
            outputDT.Columns.Add("worktime", typeof(Int32));
            double overalldurations = 0;
            double onholdtime = -100;
            double processingtime = 0;

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    completeddate = (DateTime)row["completeddate"];
                    starteddate = (DateTime)row["starteddate"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (completeddate >= starteddate)
                    {
                        overalldurations = (completeddate - starteddate).TotalMinutes;
                    }

                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= completeddate && onholddate >= starteddate)
                        {
                            onholdtime = completeddate > offholddate
                                ? (offholddate - onholddate).TotalMinutes
                                : (completeddate - onholddate).TotalMinutes;
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

                    if (!row.Table.Columns.Contains("worktime"))
                    {
                        row.Table.Columns.Add("worktime", typeof(Int32));
                    }
                    row["worktime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]) && x.Field<string>("loginname") == row["loginname"].ToString());

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["worktime"] = Convert.ToInt32(outputDT.Rows[index]["worktime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }

                dailyDT = outputDT.AsEnumerable()
                .GroupBy(x => new { loginname = x.Field<string>("loginname"), completeddate = x.Field<DateTime>("completeddate").Date })
                .Select(x => new
                {
                    x.Key.loginname,
                    completeddate = Convert.ToInt32(String.Format("{0:yyyyMMdd}", x.Key.completeddate)),
                    workhour = (x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "Abstraction").Count() * 2 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "Abstraction").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "Abstraction").Count() * 0.35
                    + x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "QC").Count() * 1.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "QC").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "QC").Count() * 0.25).ToString(),
                    worktime = String.Format("{0:.##}", x.Select(y => y.Field<int>("worktime")).Average())
                }).ToDataTable();
                dailyDT.TableName = tablename;

                return dailyDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable CreateRankSheet(DataTable inputDT, string tablename)
        {
            DateTime completeddate = new DateTime();
            DateTime starteddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            //DateTime reactivateddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            DataTable conclusionDT = new DataTable();


            double overalldurations = 0;
            double onholdtime = -100;
            double processingtime = 0;

            try
            {
                foreach (DataRow row in inputDT.Rows)
                {
                    completeddate = (DateTime)row["completeddate"];
                    starteddate = (DateTime)row["starteddate"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    //reactivateddate = string.IsNullOrEmpty(row["reactivateddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["reactivateddate"]);
                    if (completeddate >= starteddate)
                    {
                        overalldurations = (completeddate - starteddate).TotalMinutes;
                    }

                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= completeddate && onholddate >= starteddate)
                        {
                            onholdtime = completeddate > offholddate
                                ? (offholddate - onholddate).TotalMinutes
                                : (completeddate - onholddate).TotalMinutes;
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

                    if (!row.Table.Columns.Contains("worktime"))
                    {
                        row.Table.Columns.Add("worktime", typeof(Int32));
                    }
                    row["worktime"] = processingtime;

                    var Duplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
x.Field<int>("submissionnumber") == Convert.ToInt32(row["submissionnumber"]) && x.Field<string>("loginname") == row["loginname"].ToString());

                    if (Duplicate.Count() == 1)
                    {
                        int index = outputDT.Rows.IndexOf(Duplicate.First());
                        outputDT.Rows[index]["worktime"] = Convert.ToInt32(outputDT.Rows[index]["worktime"]) - onholdtime;
                    }
                    else
                    {
                        outputDT.ImportRow(row);
                    }
                }

                conclusionDT = outputDT.AsEnumerable()
                    .GroupBy(x => new { loginname = x.Field<string>("loginname") }).Select(x => new
                    {
                        x.Key.loginname,
                        workhour = (x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "Abstraction").Count() * 2 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "Abstraction").Count() * 0.75 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "Abstraction").Count() * 0.35
                    + x.Where(y => y.Field<string>("trialtype") == "Original" && y.Field<string>("category") == "QC").Count() * 1.5 + x.Where(y => y.Field<string>("trialtype") == "Amendment" && y.Field<string>("category") == "QC").Count() * 0.5 + x.Where(y => y.Field<string>("trialtype") == "Abbreviated" && y.Field<string>("category") == "QC").Count() * 0.25).ToString(),
                        worktime = String.Format("{0:.##}", x.Select(y => y.Field<int>("worktime")).Average())
                    }).ToDataTable();
                conclusionDT.TableName = tablename;

                return conclusionDT;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public override string[] CreateBook(NpgsqlConnection conn, string startDate, string endDate, CTRPReporting.Models.Chart chart, out string[] Xaxis, out List<Dictionary<string, string>> Yaxis)
        {
            //All
            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //NCI
            string chart_text = chart.ChartSettings.FirstOrDefault().Code.Replace("startDate", startDate).Replace("endDate", endDate);
            cmd = new NpgsqlCommand(chart_text, conn);
            datareader = cmd.ExecuteReader();
            DataTable nciDT = new DataTable();
            DataTable rankDT = new DataTable();
            string[] loginname = { };
            Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();

            nciDT.Load(datareader);

            DataTable dailyDT = CreateDailySheet(nciDT, "Daily Hours");
            DataTable conclusionDT = CreateRankSheet(nciDT, "Total Hours");

            loginname = dailyDT.AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
            List<string> tempdates = dailyDT.AsEnumerable().OrderBy(x => x.Field<int>("completeddate")).Select(x => x.Field<int>("completeddate").ToString()).Distinct().ToList();
            Xaxis = new string[] { string.Join(",", tempdates), "" };

            foreach (string ln in loginname)
            {
                string worktotal = conclusionDT.AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<string>("workhour")).FirstOrDefault();
                List<string> Yvalue = new List<string>();
                foreach (string tdate in tempdates)
                {
                    string tempvalue = dailyDT.AsEnumerable()
                        .Where(x => x.Field<string>("loginname") == ln && x.Field<int>("completeddate") == Convert.ToInt32(tdate))
                        .Select(x => x.Field<string>("workhour")).FirstOrDefault();

                    if (string.IsNullOrEmpty(tempvalue))
                    {
                        Yvalue.Add("0");
                    }
                    else
                    {
                        Yvalue.Add(tempvalue);
                    }
                }
                tempYaxis.Add(ln, string.Join(",", Yvalue));


                if (string.IsNullOrEmpty(worktotal))
                {
                    worktotal = "0";
                }

                temprankYaxis.Add(ln, worktotal);
            }
            Yaxis.Add(tempYaxis);
            temprankYaxis = temprankYaxis.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            Yaxis.Add(temprankYaxis);
            tempYaxis = new Dictionary<string, string>();
            temprankYaxis = new Dictionary<string, string>();

            return loginname;
        }
    }

    public class PDAWorkloadChart : CTROChart
    {
        public DataTable CreateDailySheet(DataTable inputDT)
        {
            DataTable outputDT = inputDT.AsEnumerable().Select(x => new { loginname = x.Field<string>("loginname"), completeddate= String.Format("{0:yyyyMMdd}", x.Field<DateTime>("completeddate")),workhour = x.Field<Decimal>("workhour") }).ToDataTable();
            return outputDT;
        }

        public override string[] CreateBook(NpgsqlConnection conn, string startDate, string endDate, CTRPReporting.Models.Chart chart, out string[] Xaxis, out List<Dictionary<string, string>> Yaxis)
        {
            //All
            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            List<DataTable> nciDT = new List<DataTable>();

            //NCI
            for (int i = 0; i < chart.ChartSettings.Count; i++)
            {
                string chart_text = chart.ChartSettings.ToArray()[i].Code.Replace("startDate", startDate).Replace("endDate", endDate);
                cmd = new NpgsqlCommand(chart_text, conn);
                datareader = cmd.ExecuteReader();
                DataTable tempdatatable = new DataTable();
                tempdatatable.Load(datareader);
                tempdatatable.TableName = chart.ChartSettings.ToArray()[i].Category;
                nciDT.Add(tempdatatable);
            }
            string[] loginname = { };
            Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();


            DataTable dailyDT = CreateDailySheet(nciDT.Where(x=>x.TableName == "Daily Hours").FirstOrDefault());
            DataTable conclusionDT = nciDT.Where(x => x.TableName == "Total Hours").FirstOrDefault();

            loginname = dailyDT.AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
            List<string> tempdates = dailyDT.AsEnumerable().OrderBy(x => Convert.ToInt32(x.Field<string>("completeddate"))).Select(x => x.Field<string>("completeddate")).Distinct().ToList();
            Xaxis = new string[] { string.Join(",", tempdates), "" };

            foreach (string ln in loginname)
            {
                string worktotal = conclusionDT.AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<Decimal>("workhour").ToString()).FirstOrDefault();
                List<string> Yvalue = new List<string>();
                foreach (string tdate in tempdates)
                {
                    string tempvalue = dailyDT.AsEnumerable()
                        .Where(x => x.Field<string>("loginname") == ln && x.Field<string>("completeddate") == tdate)
                        .Select(x => x.Field<Decimal>("workhour").ToString()).FirstOrDefault();

                    if (string.IsNullOrEmpty(tempvalue))
                    {
                        Yvalue.Add("0");
                    }
                    else
                    {
                        Yvalue.Add(tempvalue);
                    }
                }
                tempYaxis.Add(ln, string.Join(",", Yvalue));


                if (string.IsNullOrEmpty(worktotal))
                {
                    worktotal = "0";
                }

                temprankYaxis.Add(ln, worktotal);
            }
            Yaxis.Add(tempYaxis);
            temprankYaxis = temprankYaxis.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            Yaxis.Add(temprankYaxis);
            tempYaxis = new Dictionary<string, string>();
            temprankYaxis = new Dictionary<string, string>();

            return loginname;
        }
    }

}