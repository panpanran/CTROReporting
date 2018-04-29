using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Attitude_Loose.Test
{
    public class CTRPReports
    {
        public DataSet TurnroundBook(NpgsqlConnection conn, string startDate, string endDate, out DataSet conclusionDS)
        {
            //All
            DataSet outputDS = new DataSet();
            conclusionDS = new DataSet();
            NpgsqlCommand cmd = null;
            NpgsqlDataReader datareader = null;
            //abbreviated
            string abbreviatedtext = System.IO.File.ReadAllText(CTRPConst.turnround_abbreviated_file).Replace("startDate", startDate).Replace("endDate", endDate);
            cmd = new NpgsqlCommand(abbreviatedtext, conn);
            datareader = cmd.ExecuteReader();
            DataTable abbreviatedDT = new DataTable();
            DataTable abbreviatedconclusionDT = new DataTable();
            abbreviatedDT.Load(datareader);
            outputDS.Tables.Add(TurnroundSheet(abbreviatedDT, startDate, endDate, "Abbreviated", out abbreviatedconclusionDT));
            conclusionDS.Tables.Add(abbreviatedconclusionDT);
            //Amendment
            string amendtext = System.IO.File.ReadAllText(CTRPConst.turnround_amendment_file).Replace("startDate", startDate).Replace("endDate", endDate);
            cmd = new NpgsqlCommand(amendtext, conn);
            datareader = cmd.ExecuteReader();
            DataTable amendDT = new DataTable();
            DataTable amendconclusionDT = new DataTable();
            amendDT.Load(datareader);
            outputDS.Tables.Add(TurnroundSheet(amendDT, startDate, endDate, "Amendment", out amendconclusionDT));
            conclusionDS.Tables.Add(amendconclusionDT);
            //Original
            string originaltext = System.IO.File.ReadAllText(CTRPConst.turnround_original_file).Replace("startDate", startDate).Replace("endDate", endDate);
            cmd = new NpgsqlCommand(originaltext, conn);
            datareader = cmd.ExecuteReader();
            DataTable originalDT = new DataTable();
            DataTable originalconclusionDT = new DataTable();
            originalDT.Load(datareader);
            outputDS.Tables.Add(TurnroundSheet(originalDT, startDate, endDate, "Original", out originalconclusionDT));
            conclusionDS.Tables.Add(originalconclusionDT);
            return outputDS;
        }

        private DataTable TurnroundSheet(DataTable inputDT, string startDate, string endDate, string tablename, out DataTable conclusionDT)
        {
            DateTime tsrdate = new DateTime();
            DateTime accepteddate = new DateTime();
            DateTime onholddate = new DateTime();
            DateTime offholddate = new DateTime();
            DateTime reactivateddate = new DateTime();
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
                        overalldurations = CTRPFunctions.CountBusinessDays(accepteddate, tsrdate, CTRPConst.Holidays);

                    //if (reactivateddate >= accepteddate && reactivateddate <= tsrdate)
                    //{
                    //    overalldurations = CTRPFunctions.CountBusinessDays(reactivateddate, tsrdate, CTRPConst.Holidays);
                    //}


                    if (offholddate >= onholddate)
                    {
                        //exclude the onholddate not within processing
                        if (onholddate <= tsrdate && onholddate >= accepteddate)
                        {
                            onholdtime = tsrdate > offholddate
                                ? CTRPFunctions.CountBusinessDays(onholddate, offholddate, CTRPConst.Holidays)
                                : CTRPFunctions.CountBusinessDays(onholddate, tsrdate, CTRPConst.Holidays);
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
                    //CTRPFunctions.SendEmail("dd", "dd", "ran.pan@nih.gov");
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
}