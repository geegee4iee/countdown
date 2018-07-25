using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Countdown.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Countdown.Core.Factories
{
    public class ProcessInfoFactory
    {
        private static Lazy<ProcessInfoFactory> _instance = new Lazy<ProcessInfoFactory>(Initialize);

        private ProcessInfoFactory()
        {
            _cachedKeys = new Dictionary<string, string>(300);
            _hashAlgorithm = new Murmur3Hash();
        }

        private readonly IDictionary<string, string> _cachedKeys = new Dictionary<string, string>();
        private readonly IHashAlgorithm _hashAlgorithm;

        public static ProcessInfo Create(Process activeProcess, int monitorIntervalInSeconds)
        {
            if (activeProcess != null)
            {
                var hashStr = GenerateIdIfNotFound(activeProcess);

                var processInfo = new ProcessInfo(hashStr);

                processInfo.TotalAmountOfTime += TimeSpan.FromSeconds(monitorIntervalInSeconds);
                processInfo.MainWindowTitle = activeProcess.MainWindowTitle;
                processInfo.ProcessName = activeProcess.ProcessName;

                return processInfo;
            }

            return null;
        }

        public static string CreateId(Process activeProcess)
        {
            return GenerateIdIfNotFound(activeProcess);
        }

        private static string GenerateIdIfNotFound(Process activeProcess)
        {
            var key = activeProcess.MainWindowTitle + " " + activeProcess.ProcessName;
            if (!_instance.Value._cachedKeys.TryGetValue(key, out string hashStr))
            {
                byte[] hashBytes = _instance.Value._hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(key));
                hashStr = hashBytes.ToHex();
                _instance.Value._cachedKeys.Add(key, hashStr);
            }

            return hashStr;
        }

        private static ProcessInfoFactory Initialize()
        {
            return new ProcessInfoFactory();
        }
    }
}
