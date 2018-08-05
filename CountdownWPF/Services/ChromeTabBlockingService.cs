using CountdownWPF.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Services
{
    public class ChromeTabBlockingService
    {
        private ChromeTabBlockingService() { }
        private static Lazy<ChromeTabBlockingService> lazy = new Lazy<ChromeTabBlockingService>(() => new ChromeTabBlockingService());

        public static ChromeTabBlockingService Instance => lazy.Value;

        private string[] _blockedUrls;
        private HttpListener _listener;
        public void StartListening()
        {
            _blockedUrls = Settings.RuntimeConfigs.GetBlockedUrls();

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{Settings.RuntimeConfigs.GetListeningPort()}/");
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(ProcessRequest), null);
        }

        public void StopListening()
        {
            _listener.Abort();
        }

        static private object _obj = new object();
        public void RefreshBlockingUrls()
        {
            lock (_obj)
            {
                _blockedUrls = Settings.RuntimeConfigs.GetBlockedUrls();
            }
        }

        private void ProcessRequest(IAsyncResult result)
        {
            HttpListenerContext context = null;
            HttpListenerRequest request = null;

            try
            {
                context = _listener.EndGetContext(result);
                request = context.Request;


                string postData;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    postData = WebUtility.UrlDecode(reader.ReadToEnd());

                    var rawProps = postData.Split('&');
                    Dictionary<string, string> props = new Dictionary<string, string>(rawProps.Length);

                    foreach (var prop in rawProps)
                    {
                        var rawEntry = prop.Split('=');

                        if (rawEntry.Length == 2)
                            props.Add(rawEntry[0].ToLower(), rawEntry[1]);
                    }

                    props.TryGetValue("url", out string url);

                    if (!string.IsNullOrWhiteSpace(url) && _blockedUrls.Any(str => url.Contains(str)))
                    {
                        var response = context.Response;
                        response.AddHeader("Access-Control-Allow-Origin", "*");
                        response.ContentType = "text/html";

                        var buffer = Encoding.UTF8.GetBytes("b");
                        response.ContentLength64 = buffer.Length;
                        var output = response.OutputStream;

                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                        response.Close();

                        Debug.WriteLine($"Blocked url: {url}");
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        context.Response.Close();
                    }
                }

                _listener.BeginGetContext(new AsyncCallback(ProcessRequest), null);
            }
            catch (ObjectDisposedException ex)
            {

            }
        }

        
    }
}
