using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTROLibrary
{
    public class Logging
    {
        public static void WriteLog(string classname, string methodname, Exception ex)
        {
            StringBuilder strLogText = new StringBuilder();

            strLogText.Append("Message ---\n{0}" + ex.Message);

            strLogText.Append(Environment.NewLine + "Source ---\n{0}" + ex.Source);
            strLogText.Append(Environment.NewLine + "StackTrace ---\n{0}" + ex.StackTrace);
            strLogText.Append(Environment.NewLine + "TargetSite ---\n{0}" + ex.TargetSite);
            if (ex.InnerException != null)
            {
                strLogText.Append(Environment.NewLine + "Inner Exception is {0}" + ex.InnerException);
                //error prone
            }
            if (ex.HelpLink != null)
            {
                strLogText.Append(Environment.NewLine + "HelpLink ---\n{0}" + ex.HelpLink);//error prone
            }

            StreamWriter log;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd", new CultureInfo("en-GB"));

            string errorFolder = ConfigurationManager.AppSettings["V_CTROLogging"];

            if (!System.IO.Directory.Exists(errorFolder))
            {
                System.IO.Directory.CreateDirectory(errorFolder);
            }

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!File.Exists($@"{errorFolder}\Log_{timestamp}.txt"))
            {
                log = new StreamWriter($@"{errorFolder}\Log_{timestamp}.txt");
            }
            else
            {
                log = File.AppendText($@"{errorFolder}\Log_{timestamp}.txt");
            }

            // Write to the file:
            log.WriteLine(Environment.NewLine + DateTime.Now);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            log.WriteLine("Class Name :- " + classname);
            log.WriteLine("Method Name :- " + methodname);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            log.WriteLine(strLogText);
            log.WriteLine();
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }

    }
}
