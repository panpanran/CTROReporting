using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTROLibrary
{
    public class Logging
    {
        public static void WriteLog(string classname, string methodname, string ex)
        {
            StringBuilder path = new StringBuilder(@"C:\Users\panr2\Downloads\C#\CTROReporting\CTROEWSystem");

            if (!Directory.Exists(path.ToString()))
            {
                Directory.CreateDirectory(path.ToString());
            }

            path.Append(DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            string output = classname + " > " + methodname + " > " + ex;
            if (!File.Exists(path.ToString()))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path.ToString()))
                {
                    Log(output, sw);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path.ToString()))
                {
                    Log(output, sw);
                }
            }
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
