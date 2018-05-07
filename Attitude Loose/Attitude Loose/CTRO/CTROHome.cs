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
        public void CreatAnalysisChart(string startDate, string endDate, string toemail, out string Xaxis, out Dictionary<string,string> Yaxis, out string[] Loginname)
        {
            CTRPReports reports = new CTRPReports();
            Yaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    Loginname = reports.PDAWorkloadBook(conn, startDate, endDate).Tables["NCI"].AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
                    Xaxis = string.Join(",", reports.PDAWorkloadBook(conn, startDate, endDate).Tables["NCI"].AsEnumerable().Select(x => x.Field<string>("completeddate")).Distinct().ToList());
                    foreach (string ln in Loginname)
                    {
                        Yaxis.Add(ln, string.Join(",", reports.PDAWorkloadBook(conn, startDate, endDate).Tables["NCI"].AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<int>("completeddate").ToString()).ToList()));
                    }
                    //Yaxis = string.Join(",", reports.PDAWorkloadBook(conn, startDate, endDate).Tables["NCI"].AsEnumerable().Where(x => x.Field<string>("loginname") == "adanoa").Select(x => x.Field<int>("worknumber").ToString()).ToList());
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}