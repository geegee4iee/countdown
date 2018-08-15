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
        IList<Session> Timeline { get; set; }

        public void StartNewSession(TimeSpan from)
        {
            if (Timeline == null) Timeline = new List<Session>();

            if (Timeline.Count > 1)
            {
                if (Timeline[Timeline.Count - 1].End == TimeSpan.Zero)
                {
                    throw new InvalidOperationException("Cannot start new session if old session is not ended");
                }
            }

            Timeline.Add(new Session(from));
            Debug.WriteLine($"Start new session of window: {MainWindowTitle}");
        }

        public void EndCurrentSession(TimeSpan end)
        {
            Debug.WriteLine($"End session of window: {MainWindowTitle}");

            if (Timeline == null)
            {
                Timeline = new List<Session>();

                // If the start of the session hasn't been defined, assigned the end time to it.
                Timeline.Add(new Session(end));
            }

            Timeline[Timeline.Count - 1].End = end;
        }
    }
}