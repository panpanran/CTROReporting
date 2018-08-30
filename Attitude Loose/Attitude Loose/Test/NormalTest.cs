using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Attitude_Loose.Test
{

    [TestFixture()]
    public class NormalTest
    {

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
    }
}