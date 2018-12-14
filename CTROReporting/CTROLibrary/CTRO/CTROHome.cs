using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.CTRO;
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
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CTROLibrary.CTRO
{
    public class CTROHome
    {
        private object GetCache(string key)
        {
            return MemoryCache.Default.Get(key);
            //return HttpRuntime.Cache.Get(key);
        }

        //excel
        public async Task<string> CreateReport(string startDate, string endDate, ApplicationUser user, Report report)
        {
            Type type = Type.GetType("CTROLibrary.CTRO." + report.ReportName.Replace(" - ", "").Replace(" ", "") + "Report");
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            object bookObject = null;

            StringBuilder pathtext = new StringBuilder();
            pathtext.Append(AppDomain.CurrentDomain.BaseDirectory + "/Excel/" + String.Format("{0:yyyyMMdd}", DateTime.Now) + "/");
            string savepath = string.Empty;
            if (!Directory.Exists(pathtext.ToString()))
            {
                Directory.CreateDirectory(pathtext.ToString());
            }

            if (report.ReportName == "SDA - Biomarker")
            {
                pathtext.Append(report.Savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx");
            }
            else if (report.ReportName == "Turnaround" && Convert.ToDateTime(startDate).Day == 1)
            {
                pathtext.Append(report.Savepath + "_" + Convert.ToDateTime(startDate).ToString("MMMM") + " " + Convert.ToDateTime(startDate).Year.ToString() + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx");
            }

            else if (!string.IsNullOrEmpty(startDate))
            {
                if (!string.IsNullOrEmpty(endDate))
                {
                    pathtext.Append(report.Savepath + "_" + startDate.Replace("-", "") + "-" + endDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx");
                }
                else
                {
                    pathtext.Append(report.Savepath + " from " + startDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx");
                }
            }
            else
            {
                pathtext.Append(report.Savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx");
            }

            savepath = pathtext.ToString();
            try
            {
                //Cache
                string key = report.ReportId + startDate + endDate;
                object pathCache = GetCache(key);
                Logger logger = new Logger
                {
                    ClassName = this.GetType().Name,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    Level = 1,
                    Message = "Creaet report first time for " + user.UserName + " - " + report.ReportName + " at " + DateTime.Now.ToString(),
                    UserId = user.Id
                };

                if (pathCache == null)
                {
                    CTROFunctions.processpercentage[user.UserName] = 0;
                    using (var conn = new NpgsqlConnection(report.ReportName == "Sponsor" ? CTROConst.paconnString : CTROConst.connString))
                    {
                        conn.Open();
                        ReportSetting[] reportSettings = report.ReportSettings.ToArray();
                        DataSet conclusionDS = new DataSet();
                        object[] parametersArray = new object[] { conn, startDate, endDate, reportSettings, conclusionDS };
                        bookObject = methodInfo.Invoke(classInstance, parametersArray);
                        conclusionDS = (DataSet)parametersArray[4];
                        DataSet bookDS = (DataSet)bookObject;
                        bool ifchart = false;
                        int totalrows = 0;
                        for (int n = 0; n < bookDS.Tables.Count; n++)
                        {
                            totalrows += bookDS.Tables[n].Rows.Count;
                        }

                        for (int n = 0; n < bookDS.Tables.Count; n++)
                        {
                            ReportSetting reportsetting = reportSettings.Where(x => x.Category == bookDS.Tables[n].TableName).FirstOrDefault();
                            if (reportsetting.ReportType == "chart")
                            {
                                ifchart = true;
                            }
                            await CTROFunctions.WriteExcelByDataTable(bookDS.Tables[n], user, savepath, report.Template, reportsetting.Startrow, reportsetting.Startcolumn, ifchart, totalrows);
                            if (reportsetting.AdditionStartrow > 0 && reportsetting.AdditionStartcolumn > 0)
                            {
                                await CTROFunctions.WriteExcelByDataTable(conclusionDS.Tables[n], user, savepath, null, reportsetting.AdditionStartrow, reportsetting.AdditionStartcolumn, ifchart);
                            }
                        }

                        CTROFunctions.SendEmail(report.ReportName + " Report", "Hi Sir/Madam, <br /><br /> Attached please find. Your " + report.ReportName.ToLower() + " report has been done. Or you can find it at shared drive. <br /><br /> Thank you", user.Email, savepath);
                        CacheItemPolicy cip = new CacheItemPolicy()
                        {
                            AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(4))
                        };


                        MemoryCache.Default.Set(key, savepath, cip);
                        //HttpRuntime.Cache.Add(key, savepath, null, Cache.NoAbsoluteExpiration, new TimeSpan(4, 0, 0), CacheItemPriority.Default, null);
                        pathCache = savepath;
                        //Trace.Listeners.Add(new TextWriterTraceListener(@"C:\Users\panr2\Downloads\C#\CTROReporting\TextWriterOutput.log", "myListener"));
                        //Trace.TraceInformation("Test message.");
                        //// You must close or flush the trace to empty the output buffer.  
                        //Trace.Flush();
                        var urltext = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
                    }
                }
                else
                {
                    //System.IO.File.Copy(pathCache.ToString(), savepath, true);
                    savepath = pathCache.ToString();
                    CTROFunctions.SendEmail(report.ReportName + " Report", "Hi Sir/Madam, <br /><br /> Attached please find. Your " + report.ReportName.ToLower() + " report has been done. Or you can find it at shared drive. <br /><br /> Thank you", user.Email, pathCache.ToString());
                    logger.Message = "Creaet report with cache for " + user.UserName + " - " + report.ReportName + " at " + DateTime.Now.ToString();
                    var urltext = CTROFunctions.CreateDataFromJson("LoggerService", "CreateLogger", logger);
                }
                Record record = new Record
                {
                    ReportId = report.ReportId,
                    UserId = user.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                };

                record.FilePath = "../Excel/" + String.Format("{0:yyyyMMdd}", DateTime.Now) + "/" + Path.GetFileName(savepath);
                //Add Record
                var url = CTROFunctions.CreateDataFromJson("RecordService", "CreateRecord", record);

                return savepath;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, "There is an error when creating report for " + user.UserName + " - " + report.ReportName + " at " + DateTime.Now.ToString() + " and the error message is: " + ex.Message);
                return string.Empty;
            }
        }

        //chart
        public void CreateAnalysisChart(string startDate, string endDate, ReportSetting[] reportsettings, string reportname, out string[] XLabel, out string[] YLabel, out string[] Xaxis, out string[] ChartName, out string[] ChartType, out List<Dictionary<string, string>> Yaxis, out string[] Loginname)
        {
            Type type = Type.GetType("CTROLibrary.CTRO." + reportname + "Report");
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
                    Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                    throw;
                }
            }
        }


        public void CreateChart(string startDate, string endDate, CTROLibrary.Model.Chart chart, out string[] XLabel, out string[] YLabel, out string[] Xaxis, out string[] ChartName, out string[] ChartType, out List<Dictionary<string, string>> Yaxis, out string[] Loginname)
        {
            Type type = Type.GetType("CTROLibrary.CTRO." + chart.ChartName.Replace(" - ", "") + "Chart");
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