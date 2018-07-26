using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Utils
{
    public class CpuCounterUtils
    {
        private static readonly Lazy<PerformanceCounter> _cpuCounter = new Lazy<PerformanceCounter>(CreateCpuCounter);

        public static float GetCpuUsage()
        {
            var nextValue = _cpuCounter.Value.NextValue();
            Debug.WriteLine($"Current cpu usage is: {nextValue}%");
            return nextValue;
        }

        private static PerformanceCounter CreateCpuCounter()
        {
            var cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            return cpuCounter;
        }
    }
}
