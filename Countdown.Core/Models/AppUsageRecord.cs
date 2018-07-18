using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Models
{
    [Serializable]
    public class AppUsageRecord : EntityModel
    {
        private const string PrefixId = "date_";
        private const string DateFormat = "yyyyMMdd";
        public IDictionary<string, ProcessInfo> ActiveApps { get; set; }
        public DateTime Date { get; set; }

        public AppUsageRecord(in DateTime date)
        {
            Id = PrefixId + date.ToString(DateFormat);
            Date = date.Date;

            ActiveApps = new Dictionary<string, ProcessInfo>();
        }

        public static string GetGeneratedId(in DateTime date)
        {
            return PrefixId + date.ToString(DateFormat);
        }
    }
}
