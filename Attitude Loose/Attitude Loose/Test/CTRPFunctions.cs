using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;

namespace Attitude_Loose.Test
{
    public static class CTRPFunctions
    {
        public static void SendEmail(string Subject, string Body, string ToEmail, string AttachmentFileName)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(ToEmail));
            msg.From = new MailAddress("panpanr@gmail.com", "CTRO Reporting System");
            msg.Subject = Subject;
            msg.Body = Body;
            msg.IsBodyHtml = true;
            
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("panpanr@gmail.com", "Prss_1234");
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;

            if (AttachmentFileName != null)
            {
                Attachment attachment = new Attachment(AttachmentFileName, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(AttachmentFileName);
                disposition.ModificationDate = File.GetLastWriteTime(AttachmentFileName);
                disposition.ReadDate = File.GetLastAccessTime(AttachmentFileName);
                disposition.FileName = Path.GetFileName(AttachmentFileName);
                disposition.Size = new FileInfo(AttachmentFileName).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                msg.Attachments.Add(attachment);
            }
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message;
            }
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var table = new DataTable();

            int i = 0;
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                table.Columns.Add(new DataColumn(prop.Name, prop.PropertyType));
                ++i;
            }

            foreach (var item in source)
            {
                var values = new object[i];
                i = 0;
                foreach (var prop in props)
                    values[i++] = prop.GetValue(item);
                table.Rows.Add(values);
            }

            return table;
        }

        public static void WriteExcelByDataSet(DataSet dataset, string savepath, string templatepath, int startrow, int startcolumn)
        {
            string data = null;
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            if (templatepath != null)
            {
                System.IO.File.Copy(templatepath, savepath, true);
            }

            xlWorkBook = xlApp.Workbooks.Open(savepath, misValue);

            for (int n = 0; n < dataset.Tables.Count; n++)
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[dataset.Tables[n].TableName];
                xlWorkSheet.Rows.WrapText = true;
                Excel.Range EntireRow = xlWorkSheet.Cells.EntireRow;
                //EntireRow.RowHeight = 15;
                EntireRow.ColumnWidth = 20;
                try
                {
                    // column headings
                    //for (var i = 0; i < dataset.Tables[n].Columns.Count; i++)
                    //{
                    //    xlWorkSheet.Cells[1, i + 1] = dataset.Tables[n].Columns[i].ColumnName;
                    //}

                    for (int i = 0; i <= dataset.Tables[n].Rows.Count - 1; i++)
                    {
                        for (int j = 0; j <= dataset.Tables[n].Columns.Count - 1; j++)
                        {
                            data = dataset.Tables[n].Rows[i].ItemArray[j].ToString();
                            if (dataset.Tables[n].Columns[j].ColumnName == "additionalcomments" && !string.IsNullOrEmpty(data))
                            {
                                xlWorkSheet.Cells[i + startrow, j + startcolumn].RowHeight = 45;
                            }

                            xlWorkSheet.Cells[i + startrow, j + startcolumn] = data;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                ReleaseObject(xlWorkSheet);
            }
            xlWorkBook.Sheets["Sheet1"].Delete();
            xlWorkBook.Save();
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

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