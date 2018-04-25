using Attitude_Loose.Test;
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
        public async Task<int> CreateTurnroundReportAsync(string startDate, string endDate)
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
                        //CTRPFunctions.SendEmail("Turnround Report", "This is a test email. ", "ran.pan@nih.gov", @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Submission Status.xlsx");
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
    }
}