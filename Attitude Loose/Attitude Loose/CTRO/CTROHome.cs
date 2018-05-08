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
        public void CreatAnalysisChart(string startDate, string endDate, string toemail, out string Xaxis, out Dictionary<string, string> nYaxis, out Dictionary<string, string> tYaxis, out string[] Loginname)
        {
            CTRPReports reports = new CTRPReports();
            nYaxis = new Dictionary<string, string>();
            tYaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    DataSet tempDS = reports.PDAWorkloadBook(conn, startDate, endDate);

                    Loginname = tempDS.Tables["NCI"].AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
                    List<string> tempdates = tempDS.Tables["NCI"].AsEnumerable().OrderBy(x => x.Field<int>("completeddate")).Select(x => x.Field<int>("completeddate").ToString()).Distinct().ToList();
                    Xaxis = string.Join(",", tempdates);
                    foreach (string ln in Loginname)
                    {
                        List<int> nYvalue = new List<int>();
                        List<string> tYvalue = new List<string>();
                        foreach (string tdate in tempdates)
                        {
                            int tempnumber = tempDS.Tables["NCI"].AsEnumerable()
                                .Where(x => x.Field<string>("loginname") == ln && x.Field<int>("completeddate") == Convert.ToInt32(tdate))
                                .Select(x => x.Field<int>("worknumber")).FirstOrDefault();
                            string temptime = tempDS.Tables["NCI"].AsEnumerable()
                                .Where(x => x.Field<string>("loginname") == ln && x.Field<int>("completeddate") == Convert.ToInt32(tdate))
                                .Select(x => x.Field<string>("worktime")).FirstOrDefault();
                            if (tempnumber != 0)
                            {
                                nYvalue.Add(tempnumber);
                                tYvalue.Add(temptime);
                            }
                            else
                            {
                                nYvalue.Add(0);
                                tYvalue.Add("0");
                            }
                        }
                        nYaxis.Add(ln, string.Join(",", nYvalue));
                        tYaxis.Add(ln, string.Join(",", tYvalue));
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