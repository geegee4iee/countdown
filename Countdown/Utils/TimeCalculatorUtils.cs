using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.Utils
{
    public static class TimeCalculatorUtils
    {
        public static TimeSpan GetRemainingTimeFromNowToTheEndOfDay()
        {
            return GetRemainingTimeFromNowToNextDays(1);
        }

        public static TimeSpan GetRemainingTimeFromNowToTheEndOfYear()
        {
            return DateTime.Now.GetRemainingTimeToNextYears(1);
        }

        public static TimeSpan GetRemainingTimeFromNowToNextDays(int numOfDays)
        {
            return DateTime.Now.GetRemainingTimeToNextDays(numOfDays);
        }

        public static TimeSpan GetRemainingTimeToNextYears(this DateTime date, int numOfYears)
        {
            if (numOfYears <= 0)
            {
                throw new ArgumentException($"{nameof(numOfYears)} must larger than 0");
            }

            var nextYears = new DateTime(date.Year + numOfYears, 1, 1);

            return nextYears - date;
        }

        public static TimeSpan GetRemainingTimeToNextDays(this DateTime date, int numOfDays)
        {
            if (numOfDays <= 0)
            {
                throw new ArgumentException($"{nameof(numOfDays)} must larger than 0");
            }

            return date.Date.AddDays(numOfDays) - date;
        }

        public static string ToShortTime(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}
