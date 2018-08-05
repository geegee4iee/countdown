using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Services
{
    public class LoggingService
    {
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine($"===>{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static void Log(string logMessage)
        {
            using (StreamWriter w = File.AppendText($"log-{DateTime.Now.ToString("yyyyMMdd")}.txt"))
            {
                Log(logMessage, w);
            }
        }

        public static void LogException(Exception ex)
        {
            Log(ex.Message + "\r\n" + ex.StackTrace);
        }

        public static string[] DumpLog(StreamReader r)
        {
            string line;
            List<string> logs = new List<string>();
            while ((line = r.ReadLine()) != null)
            {
                logs.Add(line);
            }

            return logs.ToArray();
        }
    }
}
