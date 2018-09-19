using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using CTRPReporting.CTRO;
using Microsoft.AspNet.Identity;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace CTRPReporting.CTRO
{
    public class CTROHome
    {
        private object GetCache(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        //excel
        public int CreateReport(string startDate, string endDate, ApplicationUser user, Report report, out string savepath)
        {
            Type type = Type.GetType("CTRPReporting.CTRO." + report.ReportName.Replace(" - ", "").Replace(" ", "") + "Report");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            object bookObject = null;

            StringBuilder pathtext = new StringBuilder();
            pathtext.Append(AppDomain.CurrentDomain.BaseDirectory + "/Excel/" + user.UserName + "/");

            if (report.ReportName == "SDA - Biomarker")
            {
                pathtext.Append(report.Savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx");
            }
            else if (!string.IsNullOrEmpty(startDate))
            {
                if (!string.IsNullOrEmpty(endDate))
                {
                    pathtext.Append(report.Savepath + "_" + startDate.Replace("-", "") + "-" + endDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + user.UserName + ".xlsx");
                }
                else
                {
                    pathtext.Append(report.Savepath + " from " + startDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + user.UserName + ".xlsx");
                }
            }
            else
            {
                pathtext.Append(report.Savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + user.UserName + ".xlsx");
            }

            savepath = pathtext.ToString();
            try
            {
                //Cache
                string key = report.ReportId + startDate + endDate;
                object pathCache = GetCache(key);
                if (pathCache == null)
                {
                    using (var conn = new NpgsqlConnection(CTROConst.connString))
                    {
                        conn.Open();
                        ReportSetting[] reportSettings = report.ReportSettings.ToArray();
                        DataSet conclusionDS = new DataSet();
                        object[] parametersArray = new object[] { conn, startDate, endDate, reportSettings, conclusionDS };
                        bookObject = methodInfo.Invoke(classInstance, parametersArray);
                        conclusionDS = (DataSet)parametersArray[4];
                        DataSet bookDS = (DataSet)bookObject;
                        bool ifchart = false;
                        for (int n = 0; n < bookDS.Tables.Count; n++)
                        {
                            ReportSetting reportsetting = reportSettings.Where(x => x.Category == bookDS.Tables[n].TableName).FirstOrDefault();
                            if (reportsetting.ReportType == "chart")
                            {
                                ifchart = true;
                            }
                            CTROFunctions.WriteExcelByDataTable(bookDS.Tables[n], user, savepath, report.Template, reportsetting.Startrow, reportsetting.Startcolumn, ifchart);
                            if (reportsetting.AdditionStartrow > 0 && reportsetting.AdditionStartcolumn > 0)
                            {
                                CTROFunctions.WriteExcelByDataTable(conclusionDS.Tables[n], user, savepath, null, reportsetting.AdditionStartrow, reportsetting.AdditionStartcolumn, ifchart);
                            }
                        }

                        CTROFunctions.SendEmail(report.ReportName + " Report", "Hi Sir/Madam, <br /><br /> Attached please find. Your " + report.ReportName.ToLower() + " report has been done. Or you can find it at shared drive. <br /><br /> Thank you", user.Email, savepath);
                        HttpRuntime.Cache.Add(key, savepath, null, Cache.NoAbsoluteExpiration, new TimeSpan(4, 0, 0), CacheItemPriority.Default, null);
                        pathCache = savepath;
                    }
                }
                else
                {
                    System.IO.File.Copy(pathCache.ToString(), savepath, true);
                    CTROFunctions.SendEmail(report.ReportName + " Report", "Hi Sir/Madam, <br /><br /> Attached please find. Your " + report.ReportName.ToLower() + " report has been done. Or you can find it at shared drive. <br /><br /> Thank you", user.Email, savepath);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return 0;
            }
        }

        //chart
        public void CreateAnalysisChart(string startDate, string endDate, ReportSetting[] reportsettings, string reportname, out string[] XLabel, out string[] YLabel, out string[] Xaxis, out string[] ChartName, out string[] ChartType, out List<Dictionary<string, string>> Yaxis, out string[] Loginname)
        {
            Type type = Type.GetType("CTRPReporting.CTRO." + reportname + "Report");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            DataSet bookDS = null;
            string[] variables = { "workhour", "worktime" };
            Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                conn.Open();
                try
                {
                    DataSet rankDS = new DataSet();
                    object[] parametersArray = new object[] { conn, startDate, endDate, reportsettings, rankDS };
                    bookDS = (DataSet)methodInfo.Invoke(classInstance, parametersArray);
                    rankDS = (DataSet)parametersArray[4];

                    Loginname = rankDS.Tables[0].AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
                    List<string> tempdates = bookDS.Tables[0].AsEnumerable().OrderBy(x => x.Field<int>("completeddate")).Select(x => x.Field<int>("completeddate").ToString()).Distinct().ToList();
                    Xaxis = new string[] { string.Join(",", tempdates), "", string.Join(",", tempdates), "" };
                    ChartName = new string[] { "Daily Hours Chart", "Total Hours Chart", "Daily Efficiency Chart", "Work Efficiency Rank Chart" };
                    ChartType = new string[] { "line", "bar", "line", "bar" };
                    XLabel = new string[] { "Date", "PDA Team", "Date", "PDA Team" };
                    YLabel = new string[] { "Number", "Avg Time Per Work", "Number", "Avg Time Per Work" };

                    foreach (string s in variables)
                    {
                        foreach (string ln in Loginname)
                        {
                            string worktotal = rankDS.Tables[0].AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<string>(s)).FirstOrDefault();
                            List<string> Yvalue = new List<string>();
                            foreach (string tdate in tempdates)
                            {
                                string tempvalue = bookDS.Tables[0].AsEnumerable()
                                    .Where(x => x.Field<string>("loginname") == ln && x.Field<int>("completeddate") == Convert.ToInt32(tdate))
                                    .Select(x => x.Field<string>(s)).FirstOrDefault();

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
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }


        public void CreateChart(string startDate, string endDate, CTRPReporting.Models.Chart chart, out string[] XLabel, out string[] YLabel, out string[] Xaxis, out string[] ChartName, out string[] ChartType, out List<Dictionary<string, string>> Yaxis, out string[] Loginname)
        {
            Type type = Type.GetType("CTRPReporting.CTRO." + chart.ChartName.Replace(" - ", "") + "Chart");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);

            Xaxis = new string[] { };
            Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTROConst.connString))
            {
                conn.Open();
                try
                {
                    object[] parametersArray = new object[] { conn, startDate, endDate, chart, Xaxis, Yaxis };
                    Loginname = (string[])methodInfo.Invoke(classInstance, parametersArray);

                    ChartName = chart.ChartSettings.Select(x => x.Category).ToArray();
                    ChartType = chart.ChartSettings.Select(x => x.ChartType).ToArray();
                    XLabel = chart.ChartSettings.Select(x => x.XLabel).ToArray();
                    YLabel = chart.ChartSettings.Select(x => x.YLabel).ToArray();
                    Xaxis = (string[])parametersArray[4];
                    Yaxis = (List<Dictionary<string, string>>)parametersArray[5];

                    //    for (int i = 0; i < chartDS.Tables.Count; i++)
                    //    {


                    //        foreach (string ln in Loginname)
                    //        {
                    //            string worktotal = rankDS.Tables[0].AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<string>("workhour")).FirstOrDefault();
                    //            List<string> Yvalue = new List<string>();
                    //            foreach (string tdate in tempdates)
                    //            {
                    //                string tempvalue = chartDS.Tables[i].AsEnumerable()
                    //                    .Where(x => x.Field<string>("loginname") == ln && x.Field<int>("completeddate") == Convert.ToInt32(tdate))
                    //                    .Select(x => x.Field<string>("workhour")).FirstOrDefault();

                    //                if (string.IsNullOrEmpty(tempvalue))
                    //                {
                    //                    Yvalue.Add("0");
                    //                }
                    //                else
                    //                {
                    //                    Yvalue.Add(tempvalue);
                    //                }
                    //            }
                    //            tempYaxis.Add(ln, string.Join(",", Yvalue));


                    //            if (string.IsNullOrEmpty(worktotal))
                    //            {
                    //                worktotal = "0";
                    //            }

                    //            temprankYaxis.Add(ln, worktotal);
                    //        }
                    //        Yaxis.Add(tempYaxis);
                    //        temprankYaxis = temprankYaxis.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    //        Yaxis.Add(temprankYaxis);
                    //        tempYaxis = new Dictionary<string, string>();
                    //        temprankYaxis = new Dictionary<string, string>();
                    //    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }

}