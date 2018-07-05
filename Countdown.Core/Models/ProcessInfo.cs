using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Countdown.Core.Models
{
    public class ProcessInfo : EntityModel
    {
        private static Regex _regex = new Regex(@"[^0-9a-zA-Z]+");

        public ProcessInfo(in Process activeProcess, in int monitorIntervalSeconds)
        {
            if (activeProcess != null)
            {
                this.TotalAmountOfTime += TimeSpan.FromSeconds(monitorIntervalSeconds);
                this.MainWindowTitle = activeProcess.MainWindowTitle;
                this.ProcessName = activeProcess.ProcessName;
                this.Id = $"{this.ProcessName}_{_regex.Replace(this.MainWindowTitle, "")}";
            }
        }

        public TimeSpan TotalAmountOfTime { get; set; }
        public string MainWindowTitle { get; set; }
        public string ProcessName { get; set; }
    }
}