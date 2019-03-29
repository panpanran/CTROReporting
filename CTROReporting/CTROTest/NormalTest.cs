using CTROLibrary.Infrastructure;
using CTROLibrary.CTRO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebSocketSharp;
using System.Net.Mail;
using Npgsql;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

namespace CTROTest
{
    [TestFixture()]
    public class NormalTest
    {
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Test]
        public void EncryptTest()
        {
            using (var stream = GenerateStreamFromString("Prsssss002"))
            {
                // create the hash code of the text to sign
                SHA1 sha = SHA1.Create();
                byte[] hashcode = sha.ComputeHash(stream);
                // use the CreateSignature method to sign the data
                DSA dsa = DSA.Create();
                byte[] signature = dsa.CreateSignature(hashcode);

                // use the VerifySignature method to verify the DSA signature
                bool isSignatureValid = dsa.VerifySignature(hashcode, signature);
            }
        }

        [Test]
        public void SSHConnectTest()
        {
            CTROFunctions.ConnectSSHCTRP();
        }


        [Test]
        public void WebSocketTest()
        {
            using (var ws = new WebSocket("ws://dragonsnest.far/Laputa"))
            {
                ws.OnMessage += (sender, e) =>
                  Console.WriteLine("Laputa says: " + e.Data);
                ws.Connect();
                ws.OnOpen += (sender, e) =>
                {
                    ws.Send("BALUS");
                };

                ws.OnClose += (sender, e) => { };

                Console.ReadKey(true);
            }
        }

        [Test]
        public void EventlogTest()
        {
            if (!EventLog.SourceExists("RanSource"))
            {
                EventLog.CreateEventSource("RanSource", "MyNewLog");
                Console.WriteLine("CreatedEventSource");
                Console.WriteLine("Please restart application");
                Console.ReadKey();
                return;
            }
            EventLog myLog = new EventLog();
            myLog.Source = "MySource";
            myLog.WriteEntry("Log event!");
        }


        [Test]
        public void XMLTest()
        {
            XNamespace ew = "ContactList";
            XElement root = new XElement(ew + "Root");
            Console.WriteLine(root);
            XAttribute contacts = new XAttribute("contacts", root);
        }


        [Test]
        public void DEBUGTest(Socket socket)
        {
#if (DEBUG)
            Console.WriteLine("Debug Mode");
#elif (RELEASE)
               Console.WriteLine("Release Mode");
#endif
        }

        [Test]
        public void GetDataTableFromCsvTest()
        {
            DataTable tb = CTROFunctions.GetDataTableFromCsv(@"C:\Users\panr2\Downloads\CSharp\CTROReporting\SearchTrialResults.csv", true);
            foreach (DataRow row in tb.Rows)
            {
                if (!row[0].ToString().Contains("NCI"))
                {
                    string aa = "";
                }
            }
        }

        [Test]
        public void PORTInUseTest()
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            int num = ipEndPoints.Where(x => x.Port == 5434).Count();
        }

        [Test]
        public Task<Socket> AcceptAsync(Socket socket)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");

            var tcs = new TaskCompletionSource<Socket>();

            socket.BeginAccept(asyncResult =>
            {
                try
                {
                    var s = asyncResult.AsyncState as Socket;
                    var client = s.EndAccept(asyncResult);

                    tcs.SetResult(client);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            }, socket);

            return tcs.Task;
        }

        [Test]
        public void ArgumentExceptionTest()
        {
            try
            {
                string a = "";
                if (a == "")
                {
                    throw new ArgumentNullException();
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        [Test]
        public void TractSourceTest()
        {
            Stream outputFile = File.Create(@"C:\Users\panr2\Downloads\DataWarehouse\tracefile.txt");
            TextWriterTraceListener textListener = new TextWriterTraceListener(outputFile);
            TraceSource traceSource = new TraceSource("myTraceSource", SourceLevels.All);
            traceSource.Listeners.Clear();
            traceSource.Listeners.Add(textListener);
            traceSource.TraceInformation("Trace output");
            traceSource.Flush();
            traceSource.Close();
        }

        [Test]
        public void WebRequestTest()
        {
            WebRequest request = WebRequest.Create("https://www.microsoft.com/en-us/");
            WebResponse response = request.GetResponse();
            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            string responseText = responseStream.ReadToEnd();
            Console.WriteLine(responseText); // Displays the HTML of the website
            response.Close();
        }

        [Test]
        public void ConcurrentTest()
        {
            var dict = new ConcurrentDictionary<string, int>();
            if (dict.TryAdd("k1", 42))
            {
                Console.WriteLine("Added");
            }
            if (dict.TryUpdate("k1", 21, 42))
            {
                Console.WriteLine("42 updated to 21");
            }
            dict["k1"] = 42; // Overwrite unconditionally
            int r1 = dict.AddOrUpdate("k1", 3, (s, i) => i * 2);
            int r2 = dict.GetOrAdd("k2", 3);
        }



        [Test]
        public void SendEmailTest()
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("dena.sumaida@nih.gov"));
            msg.From = new MailAddress("ncictro@mail.nih.gov", "CTRO Reporting System");
            msg.Subject = "Test Public Email";
            msg.Body = "Only for testing. Can you receive it ? Is it OK like this ?";
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("ncictro@mail.nih.gov", "20180806!!N_c_i");
            client.Port = 25; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "mailfwd.nih.gov";
            client.EnableSsl = false;

            //if (!string.IsNullOrEmpty(AttachmentFileName))
            //{
            //    Attachment attachment = new Attachment(AttachmentFileName, MediaTypeNames.Application.Octet);
            //    ContentDisposition disposition = attachment.ContentDisposition;
            //    disposition.CreationDate = File.GetCreationTime(AttachmentFileName);
            //    disposition.ModificationDate = File.GetLastWriteTime(AttachmentFileName);
            //    disposition.ReadDate = File.GetLastAccessTime(AttachmentFileName);
            //    disposition.FileName = Path.GetFileName(AttachmentFileName);
            //    disposition.Size = new FileInfo(AttachmentFileName).Length;
            //    disposition.DispositionType = DispositionTypeNames.Attachment;
            //    msg.Attachments.Add(attachment);
            //}
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                string errormessage = ex.Message;
            }
        }
    }
}
