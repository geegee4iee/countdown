using Countdown.Core.Infrastructure;
using Countdown.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Countdown.Core.Models
{
    [Serializable]
    public class ProcessInfo : EntityModel
    {
        Murmur3Hash hashFunction = new Murmur3Hash();
        public ProcessInfo(in Process activeProcess, in int monitorIntervalSeconds)
        {
            if (activeProcess != null)
            {
                this.TotalAmountOfTime += TimeSpan.FromSeconds(monitorIntervalSeconds);
                this.MainWindowTitle = activeProcess.MainWindowTitle;
                this.ProcessName = activeProcess.ProcessName;

                string hashStr = null;

                if (!_cachedPartialKeys.TryGetValue(this.MainWindowTitle, out hashStr))
                {
                    byte[] hashed = hashFunction.ComputeHash(Encoding.Unicode.GetBytes(this.MainWindowTitle));
                    hashStr = hashed.ToHex();
                    _cachedPartialKeys.Add(this.MainWindowTitle, hashStr);
                }
                
                this.Id = $"{this.ProcessName}_{hashStr}";
            }
        }

        private static readonly Dictionary<string, string> _cachedPartialKeys = new Dictionary<string, string>();
        public TimeSpan TotalAmountOfTime { get; set; }
        public string MainWindowTitle { get; set; }
        public string ProcessName { get; set; }
    }
}