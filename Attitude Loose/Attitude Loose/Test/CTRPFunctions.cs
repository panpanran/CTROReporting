using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;

namespace Attitude_Loose.Test
{
    public static class CTRPFunctions
    {
        public static DateTime[] Holidays = new DateTime[]{
                    new DateTime(2018,1,1),
                    new DateTime(2018,1,15),
                    new DateTime(2018,2,19),
                    new DateTime(2018,5,28),
                    new DateTime(2018,7,4),
                    new DateTime(2018,9,3),
                    new DateTime(2018,9,3),
                    new DateTime(2018,10,8),
                    new DateTime(2018,11,12),
                    new DateTime(2018,11,22),
                    new DateTime(2018,12,25)
                };

        public static void SendEmail(string Subject, string Body, string ToEmail)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(ToEmail));
            msg.From = new MailAddress("panpanr@gmail", "Ran Pan");
            msg.Subject = Subject;
            msg.Body = Body;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("panpanr@gmail", "Prss_1234");
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.gmail.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message;
            }
        }

        public static void readTxt(string path)
        {
            string text = System.IO.File.ReadAllText(path);
        }

        public static void CreateExcelByDataTable(DataTable dataTable)
        {
            string data = null;
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            // column headings
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                xlWorkSheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
            }

            for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dataTable.Columns.Count - 1; j++)
                {
                    data = dataTable.Rows[i].ItemArray[j].ToString();
                    xlWorkSheet.Cells[i + 2, j + 1] = data;
                }
            }


            xlWorkBook.SaveAs(@"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Turnround Reports.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            ReleaseObject(xlWorkSheet);
            ReleaseObject(xlWorkBook);
            ReleaseObject(xlApp);
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }

        }

        public static int CountBusinessDays(this DateTime firstDay, DateTime lastDay, params DateTime[] Holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday
    ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday
    ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime Holiday in Holidays)
            {
                DateTime bh = Holiday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }
    }
}