using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Models
{
    public class AppUsageRecord : EntityModel
    {
        public const string PrefixId = "date_";
        public const string DateFormat = "yyyyMMdd";
        public IDictionary<string, ProcessInfo> ActiveApps { get; set; }
        DateTime Date { get; }

        public AppUsageRecord(in DateTime date)
        {
            Id = PrefixId + date.ToString(DateFormat);
            Date = date;

            ActiveApps = new Dictionary<string, ProcessInfo>();
        }

        public static string GetGeneratedId(in DateTime date)
        {
            return PrefixId + date.ToString(DateFormat);
        }
    }
}
