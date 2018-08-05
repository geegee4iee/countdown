using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Configurations
{
    public class Settings
    {
        // seconds
        public const int MonitorInterval = 1;
        // seconds
        public const int PersistentRecordInterval = 30;
        // seconds
        public const int UserIdleWindow = 60 * 5;

        private Settings()
        {

        }

        private static readonly Lazy<Settings> _settings = new Lazy<Settings>(() => new Settings());

        public static Settings RuntimeConfigs
        {
            get
            {
                return _settings.Value;
            }
        }

        private JObject _cachedConfig;

        private JObject ParseConfigFile()
        {
            if (_cachedConfig != null) return _cachedConfig;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (File.Exists(path))
            {
                var raw = File.ReadAllText(path);
                _cachedConfig = JObject.Parse(raw);
            }

            return _cachedConfig;
        }

        private void UpdateConfigFile(string raw)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (File.Exists(path))
            {
                File.WriteAllText(path, raw);
            }
        }

        public string[] GetBlockedUrls()
        {
            string[] results = null;

            var parsedJObj = ParseConfigFile();
            var blockedUrls = parsedJObj.SelectToken("blocked_urls").Children();
            results = new string[blockedUrls.Count()];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = (string)blockedUrls.ElementAt(i);
            }

            return results;
        }

        public void UpdateBlockedUrls(string[] newUrls)
        {
            var parsedJObj = ParseConfigFile();
            var blockedUrls = parsedJObj.SelectToken("blocked_urls");
            blockedUrls.Replace(JArray.FromObject(newUrls));

            var raw = parsedJObj.ToString();
            UpdateConfigFile(raw);
        }

        public int GetListeningPort()
        {
            var parsedJObj = ParseConfigFile();
            var port = parsedJObj.SelectToken("listening_port").Value<int>();

            return port;
        }
    }
}
