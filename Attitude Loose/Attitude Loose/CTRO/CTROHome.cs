using Attitude_Loose.Test;
using Microsoft.AspNet.Identity;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Attitude_Loose.CTRO
{
    public class CTROHome
    {
        //turnround
        public async Task<int> CreateTurnroundReportAsync(string startDate, string endDate, string toemail)
        {
            Task<int> t = Task.Run(() =>
            {
                CTRPReports reports = new CTRPReports();
                using (var conn = new NpgsqlConnection(CTRPConst.connString))
                {
                    conn.Open();
                    try
                    {
                        string savepath = CTRPConst.turnround_savepath + "_" + startDate.Replace("-", "") + "-" + endDate.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";
                        string templatepath = CTRPConst.turnround_template_file;
                        DataSet conclusionturnroundDS = new DataSet();
                        DataSet turnroundDS = reports.TurnroundBook(conn, startDate, endDate, out conclusionturnroundDS);

                        CTRPFunctions.WriteExcelByDataSet(turnroundDS, savepath, templatepath, 2, 1);
                        CTRPFunctions.WriteExcelByDataSet(conclusionturnroundDS, savepath, null, 2, 18);
                        CTRPFunctions.SendEmail("Turnround Report", "Attached please find. \r\n This is turnround report from " + startDate + " to " + endDate, toemail, savepath);
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                        throw;
                    }
                }
            });
            await t;
            return t.Result;
        }

        //sponsornotmatch
        public async Task<int> CreateSponsorNotMatcReportAsync(string toemail)
        {
            Task<int> t = Task.Run(() =>
            {
                CTRPReports reports = new CTRPReports();
                using (var conn = new NpgsqlConnection(CTRPConst.connString))
                {
                    conn.Open();
                    try
                    {
                        string savepath = CTRPConst.sponsornotmatch_savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";
                        string templatepath = CTRPConst.sponsornotmatch_template_file;
                        DataSet sponsorDS = reports.SponsorNotMatchBook(conn);

                        CTRPFunctions.WriteExcelByDataSet(sponsorDS, savepath, templatepath, 2, 1);
                        CTRPFunctions.SendEmail("Sponsor Report", "Attached please find. \r\n This is sponsor report generated at " + DateTime.Now.ToString(), toemail, savepath);
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return 0;
                        throw;
                    }
                }
            });
            await t;
            return t.Result;
        }

        //analysis
        public void CreatPDAWorkloadAnalysisChart(string startDate, string endDate, string toemail, out string[] Xaxis, out string[] ChartName, out string[] ChartType, out List<Dictionary<string, string>> Yaxis, out string[] Loginname)
        {
            CTRPReports reports = new CTRPReports();
            string[] variables = { "worknumber", "worktime" };
            Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();
            DataSet rankDS = new DataSet();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    DataSet tempDS = reports.PDAWorkloadBook(conn, startDate, endDate, out rankDS);

                    Loginname = tempDS.Tables["NCI"].AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
                    List<string> tempdates = tempDS.Tables["NCI"].AsEnumerable().OrderBy(x => x.Field<int>("completeddate")).Select(x => x.Field<int>("completeddate").ToString()).Distinct().ToList();
                    Xaxis = new string[] { string.Join(",", tempdates), "", string.Join(",", tempdates), "" };
                    ChartName = new string[] {"Daily Number Chart", "Work Number Rank Chart", "Daily Efficiency Chart", "Work Efficiency Rank Chart" };
                    ChartType = new string[] { "line", "bar", "line", "bar" };

                    foreach (string s in variables)
                    {
                        foreach (string ln in Loginname)
                        {
                            string worktotal = rankDS.Tables["NCI"].AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<string>(s)).FirstOrDefault();
                            List<string> Yvalue = new List<string>();
                            foreach (string tdate in tempdates)
                            {
                                string tempvalue = tempDS.Tables["NCI"].AsEnumerable()
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

                    string test = "";
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}