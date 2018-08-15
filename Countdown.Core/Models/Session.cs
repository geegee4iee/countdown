using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Models
{
    public class Session
    {
        public Session(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }

        public Session(TimeSpan start)
        {
            Start = start;
        }

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
