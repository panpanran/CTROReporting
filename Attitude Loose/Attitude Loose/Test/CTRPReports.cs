using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Test
{
    public class CTRPReports
    {
        private string turnround_original_file = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\amendment_sql.txt";

        public DataTable Turnround(NpgsqlConnection conn, string startDate, string endDate)
        {
            return Turnround_Original(conn, startDate, endDate);
        }

        private DataTable Turnround_Original(NpgsqlConnection conn, string startDate, string endDate)
        {
            string querytext = System.IO.File.ReadAllText(turnround_original_file).Replace("startDate", startDate).Replace("endDate", endDate);
            var cmd = new NpgsqlCommand(querytext, conn);
            NpgsqlDataReader datareader = cmd.ExecuteReader();
            DataTable inputDT = new DataTable();
            inputDT.Load(datareader);

            DateTime tsrdate = new DateTime();
            DateTime receiveddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
            //var results = inputDT.AsEnumerable().Where(p => (p.Field<DateTime>("tsrdate") - p.Field<DateTime>("receiveddate")).Days -

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
                    receiveddate = (DateTime)row["receiveddate"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    if (tsrdate >= receiveddate)
                        overalldurations = CTRPFunctions.CountBusinessDays(receiveddate, tsrdate, CTRPFunctions.Holidays);
                    if (!string.IsNullOrEmpty(row["offholddate"].ToString()) && !string.IsNullOrEmpty(row["offholddate"].ToString()))
                    {
                        if (offholddate >= onholddate)
                        {
                            onholdtime = CTRPFunctions.CountBusinessDays(onholddate, offholddate, CTRPFunctions.Holidays);
                        }
                    }
                    else
                    {
                        onholdtime = 0;
                    }
                    processingtime = overalldurations - onholdtime;
                    //if (processingtime > 7)
                    //{
                    if (!row.Table.Columns.Contains("overalldurations"))
                    {
                        row.Table.Columns.Add("overalldurations", typeof(Int32));
                        row.Table.Columns.Add("onholdtime", typeof(Int32));
                        row.Table.Columns.Add("processingtime", typeof(Int32));
                    }
                    row["overalldurations"] = overalldurations;
                    row["onholdtime"] = onholdtime;
                    row["processingtime"] = processingtime;

                    outputDT.ImportRow(row);

                    int countDuplicate = outputDT.AsEnumerable().Where(x => x.Field<string>("trialid") == row["trialid"].ToString() &&
                    x.Field<int>("submission") == Convert.ToInt32(row["trialid"])).Count();

                    //}
                    //CTRPFunctions.SendEmail("dd", "dd", "ran.pan@nih.gov");
                }

                var results = inputDT.AsEnumerable().GroupBy(x => new { ID = x.Field<string>("trialid"), Submission = x.Field<int>("submission") }).Select(y => y.First());
                return results.CopyToDataTable();
                //var results = inputDT.AsEnumerable().Where(p => (p.Field<DateTime>("tsrdate") - p.Field<DateTime>("receiveddate")).Days -

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void Turnround_Amendment(string path, NpgsqlConnection conn, string startDate, string endDate)
        {
            string querytext = System.IO.File.ReadAllText(path).Replace("startDate", "2018-04-01").Replace("endDate", "2018-04-13");
            var cmd = new NpgsqlCommand(querytext, conn);
            NpgsqlDataReader datareader = cmd.ExecuteReader();
            DataTable inputDT = new DataTable();
            inputDT.Load(datareader);

            DateTime tsrdate = new DateTime();
            DateTime receiveddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
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
                    receiveddate = (DateTime)row["receiveddate"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    if (tsrdate >= receiveddate)
                        overalldurations = CTRPFunctions.CountBusinessDays(receiveddate, tsrdate, CTRPFunctions.Holidays);
                    if (!string.IsNullOrEmpty(row["offholddate"].ToString()) && !string.IsNullOrEmpty(row["offholddate"].ToString()))
                    {
                        if (offholddate >= onholddate)
                        {
                            onholdtime = CTRPFunctions.CountBusinessDays(onholddate, offholddate, CTRPFunctions.Holidays);
                        }
                    }
                    else
                    {
                        onholdtime = 0;
                    }
                    processingtime = overalldurations - onholdtime;
                    //if (processingtime > 7)
                    //{
                    //    if (!row.Table.Columns.Contains("overalldurations"))
                    //    {
                    //        row.Table.Columns.Add("overalldurations", typeof(Int32));
                    //        row.Table.Columns.Add("onholdtime", typeof(Int32));
                    //        row.Table.Columns.Add("processingtime", typeof(Int32));
                    //    }
                    //    row["overalldurations"] = overalldurations;
                    //    row["onholdtime"] = onholdtime;
                    //    row["processingtime"] = processingtime;

                    //    outputDT.ImportRow(row);
                    //}
                    //CTRPFunctions.SendEmail("dd", "dd", "ran.pan@nih.gov");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void Turnround_Abbreviate(string path, NpgsqlConnection conn, string startDate, string endDate)
        {
            string querytext = System.IO.File.ReadAllText(path).Replace("startDate", "2018-04-01").Replace("endDate", "2018-04-13");
            var cmd = new NpgsqlCommand(querytext, conn);
            NpgsqlDataReader datareader = cmd.ExecuteReader();
            DataTable inputDT = new DataTable();
            inputDT.Load(datareader);

            DateTime tsrdate = new DateTime();
            DateTime receiveddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            DataTable outputDT = inputDT.Clone();
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
                    receiveddate = (DateTime)row["receiveddate"];
                    onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                    offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                    if (tsrdate >= receiveddate)
                        overalldurations = CTRPFunctions.CountBusinessDays(receiveddate, tsrdate, CTRPFunctions.Holidays);
                    if (!string.IsNullOrEmpty(row["offholddate"].ToString()) && !string.IsNullOrEmpty(row["offholddate"].ToString()))
                    {
                        if (offholddate >= onholddate)
                        {
                            onholdtime = CTRPFunctions.CountBusinessDays(onholddate, offholddate, CTRPFunctions.Holidays);
                        }
                    }
                    else
                    {
                        onholdtime = 0;
                    }
                    processingtime = overalldurations - onholdtime;
                    //if (processingtime > 7)
                    //{
                    //    if (!row.Table.Columns.Contains("overalldurations"))
                    //    {
                    //        row.Table.Columns.Add("overalldurations", typeof(Int32));
                    //        row.Table.Columns.Add("onholdtime", typeof(Int32));
                    //        row.Table.Columns.Add("processingtime", typeof(Int32));
                    //    }
                    //    row["overalldurations"] = overalldurations;
                    //    row["onholdtime"] = onholdtime;
                    //    row["processingtime"] = processingtime;

                    //    outputDT.ImportRow(row);
                    //}
                    //CTRPFunctions.SendEmail("dd", "dd", "ran.pan@nih.gov");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}