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
        public ProcessInfo(string id)
        {
            this.Id = id;
        }

        public TimeSpan TotalAmountOfTime { get; set; }
        public string MainWindowTitle { get; set; }
        public string ProcessName { get; set; }
    }
}