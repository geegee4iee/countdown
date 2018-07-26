using System;
using System.Collections.Generic;
using System.Linq;
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

            ActiveApps = new Dictionary<string, ProcessInfo>(300);
        }

        public static string GetGeneratedId(in DateTime date)
        {
            return PrefixId + date.ToString(DateFormat);
        }

        public void MergeWith(AppUsageRecord appRecord2)
        {
            if (appRecord2.Date != this.Date)
            {
                throw new ArgumentException("Cannot merge with different-date record!");
            }

            var secondActiveApps = appRecord2.ActiveApps.Select(a => a.Value);
            foreach(var activeApp in secondActiveApps)
            {
                if (this.ActiveApps.TryGetValue(activeApp.Id, out ProcessInfo processInfo))
                {
                    processInfo.TotalAmountOfTime += activeApp.TotalAmountOfTime;
                }
                else
                {
                    this.ActiveApps.Add(activeApp.Id, activeApp);
                }
            }

        }
    }
}
