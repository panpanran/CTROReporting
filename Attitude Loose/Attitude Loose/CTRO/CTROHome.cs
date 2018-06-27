﻿using Attitude_Loose.Models;
using Attitude_Loose.Test;
using Microsoft.AspNet.Identity;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Attitude_Loose.CTRO
{
    public class CTROHome
    {
        //excel
        public int CreateReport(string startDate, string endDate, ApplicationUser user, ReportSetting[] reportsettings, Report report, out string savepath)
        {
            Type type = Type.GetType("Attitude_Loose.Test." + report.ReportName.Replace(" - ", "") + "Report");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            object bookObject = null;
            savepath = report.Savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + user.UserName + ".xlsx";

            if (!string.IsNullOrEmpty(startDate))
            {
                if (!string.IsNullOrEmpty(endDate))
                {
                    savepath = report.Savepath + "_" + startDate.Replace("-", "") + "-" + endDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + user.UserName + ".xlsx";
                }
                else
                {
                    savepath = report.Savepath + " from " + startDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_" + user.UserName + ".xlsx";
                }
            }
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    DataSet conclusionDS = new DataSet();
                    object[] parametersArray = new object[] { conn, startDate, endDate, reportsettings, conclusionDS };
                    bookObject = methodInfo.Invoke(classInstance, parametersArray);
                    conclusionDS = (DataSet)parametersArray[4];
                    DataSet bookDS = (DataSet)bookObject;
                    for (int n = 0; n < bookDS.Tables.Count; n++)
                    {
                        ReportSetting reportsetting = reportsettings.Where(x => x.Category == bookDS.Tables[n].TableName).FirstOrDefault();
                        CTRPFunctions.WriteExcelByDataTable(bookDS.Tables[n], savepath, report.Template, reportsetting.Startrow, reportsetting.Startcolumn);
                        if(reportsetting.AdditionStartrow >0 && reportsetting.AdditionStartcolumn > 0)
                        {
                            CTRPFunctions.WriteExcelByDataTable(conclusionDS.Tables[n], savepath, null, reportsetting.AdditionStartrow, reportsetting.AdditionStartcolumn);
                        }
                    }

                    CTRPFunctions.SendEmail(report.ReportName + " Report", "Hi Sir/Madam, <br /><br /> Attached please find. Your " + report.ReportName.ToLower() + " report has been done. Or you can find it at shared drive. <br /><br /> Thank you", user.Email, savepath);
                    return 1;
                }
                catch (Exception ex)
                {
                    return 0;
                    throw;
                }
            }
        }

        //chart
        public void CreateAnalysisChart(string startDate, string endDate, ReportSetting[] reportsettings, string reportname, out string[] XLabel, out string[] YLabel, out string[] Xaxis, out string[] ChartName, out string[] ChartType, out List<Dictionary<string, string>> Yaxis, out string[] Loginname)
        {
            Type type = Type.GetType("Attitude_Loose.Test." + reportname + "Report");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            DataSet bookDS = null;
            string[] variables = { "worknumber", "worktime" };
            Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
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
                    ChartName = new string[] { "Daily Number Chart", "Work Number Rank Chart", "Daily Efficiency Chart", "Work Efficiency Rank Chart" };
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
    }

}