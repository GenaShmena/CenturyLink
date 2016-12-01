using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net;

using log4net.Config;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WebClientConsole
{
    class Program
    {
        static System.Timers.Timer WCC_Timer;
        static int eventFired;
        static string baseURI            = ConfigurationManager.AppSettings["ServerTrackUri"].ToString();
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
        static string computerName       = Environment.GetEnvironmentVariable("COMPUTERNAME");
        static HttpClient client         = new HttpClient();
        static string quit               = null;
        static string command            = "P";
        static string threadType         = "S";
        static int numOfThreads          = 1;
        static int timePeriod            = 0;
        static double timeInterval       = Convert.ToDouble(ConfigurationManager.AppSettings["Frequency"].ToString());

        static void Main(string[] args)
        {
            ParseCmdLineArgs(args);

            switch (command)
            {
                case "G":
                    GetPerfCounters().Wait();
                break;
                case "P":
                    SetPerfCounters();
                break;
            }

            Console.ReadLine();
        }

        private static void ParseCmdLineArgs(string[] args)
        {
            if (args == null || args.Count() == 0)
            {
                Console.WriteLine("No arguments provided");
                Environment.Exit(1);

                threadType = "S";
                numOfThreads = 1;
            }
            else
            {
                string temp = null;
                int pos     = 0;
                foreach (string s in args)
                {
                    temp = s.ToUpper();
                    temp = (char.IsLetter(temp[0])) ? temp : temp.Substring(1);
                    pos = temp.IndexOf(':');

                    char c = temp[0];

                    switch (c)
                    {
                        case 'F':
                            timeInterval = Convert.ToDouble(temp.Substring(pos + 1));
                        break;
                        case 'V':
                            if (temp[pos + 1] == 'P' || temp[pos + 1] == 'G')
                            {
                                command = temp[pos + 1].ToString();
                            }
                            else
                            {
                                Console.WriteLine("Incorrect verb for command. Only supported verbs are 'p[ost]' or 'g[et]");
                                Environment.Exit(0);
                            }
                        break;
                        case 'C':
                            computerName = temp.Substring(pos + 1);
                        break;
                        case 'S':
                            threadType = "S";
                        break;
                        case 'M':
                            threadType = "M";
                            numOfThreads = int.Parse(temp.Substring(pos + 1));
                        break;
                        case 'T':
                            timePeriod = int.Parse(temp.Substring(pos + 1));
                        break;
                    }
                }
            }
        }

        static async Task GetPerfCounters()
        {
            ServerTrack serverTrack = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage respMsg = await client.GetAsync(string.Format("{0}?SrvName={1}&TimeInterval={2}", baseURI, computerName, timePeriod));
                    if (respMsg.IsSuccessStatusCode)
                    {
                        serverTrack = await respMsg.Content.ReadAsAsync<ServerTrack>();
                    }
                }

                if (serverTrack != null)
                {
                    Console.WriteLine("Server: {0}", serverTrack.Server);
                    if (serverTrack.Counters != null && serverTrack.Counters.Count > 0)
                    {
                        Console.WriteLine("  Counters:");
                        foreach (PerfCounters pf in serverTrack.Counters)
                        {
                            Console.WriteLine("    TimeStamp: {0}, Cpu_Usage: {1}, Ram_Usage: {2}", pf.TimeStamp, Math.Round(pf.Cpu_Usage, 1), Math.Round(pf.Ram_Usage, 1));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
        private static void SetPerfCounters()
        {
            WCC_Timer = new System.Timers.Timer(timeInterval);
            WCC_Timer.Elapsed += OnTimer;
            WCC_Timer.AutoReset = true;
            WCC_Timer.Enabled = true;

            quit = Console.ReadLine().ToUpper();
        }
        private static void OnTimer(Object source, ElapsedEventArgs e)
        {
            if (threadType == "S")
            {
                Post((object)computerName);
            }

            if (threadType == "M")
            {
                string compName = null;
                for (int i = 0; i <= numOfThreads; i++)
                {
                    compName = i == 0 ? computerName : computerName + i.ToString();
                    Thread t = new Thread(Post);
                    t.Name = compName;
                    t.IsBackground = true;

                    t.Start((object)(compName));
                }
            }
        }

        static void Post(object computerName)
        {
            if (eventFired == 20 || quit == "Q")
            {
                WCC_Timer.Enabled = false;
            }
            else
            {
                double cpu = 0.0, ram = 0.0;
                WMIClass wmi = new WMIClass();
                bool b = wmi.GetCounters(ref cpu, ref ram);
                if (b == true)
                {
                    string dateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    using (HttpClient client = new HttpClient())
                    {
                        HttpMethod m                 = new HttpMethod("POST");
                        string s                     = string.Format("{0}{1}?TimeStamp={2}&Cpu_Usage={3}&Ram_Usage={4}", baseURI, computerName, dateTime, cpu.ToString(), ram.ToString());
                        HttpRequestMessage msg       = new HttpRequestMessage(new HttpMethod("POST"), s);
                        HttpResponseMessage response = client.SendAsync(msg).Result;
                        Console.WriteLine("CoputerName: {0}, StatusCode: {1}", computerName, response.StatusCode.ToString());
                    }
                }
            }

            if (eventFired == 20 || quit == "Q")
            {
                WCC_Timer.Enabled = false;
            }
        }
    }
}
