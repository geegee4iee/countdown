using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.ApplicationModel.Background;
using HtmlAgilityPack;
using System.Net.Http;
using Windows.UI.Xaml.Media.Imaging;
using Countdown.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Countdown
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<string, int> appUsage = new Dictionary<string, int>();
        Dictionary<string, int> appUsagePersistence = new Dictionary<string, int>();
        DispatcherTimer _dispatcherTimer;
        public MainPage()
        {
            this.InitializeComponent();
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += UpdateCountDownTimer;
            _dispatcherTimer.Tick += LoadAppInfo;

            _dispatcherTimer.Start();

            GetQuote();
            SendToastNotification();
        }

        private void LoadAppInfo(object sender, object e)
        {
            var processInfoCollection = ProcessDiagnosticInfo.GetForProcesses().ToList();
            foreach(var process in processInfoCollection)
            {
                string name = process.ExecutableFileName;
                if (!appUsage.ContainsKey(name)) {
                    appUsage.Add(name, 1);
                } else
                {
                    appUsage[name]++;
                }
            }
        }

        public async void SendToastNotification()
        {
            var toastNotification = await CountdownToastBuilder.GetToastNotification();
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }

        public async void GetQuote()
        {
            var url = "https://www.brainyquote.com/quote_of_the_day";
            var quotePage = new Uri(url);
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            var quoteImage = doc.DocumentNode.SelectSingleNode(@"//div[contains(@class, 'bqQOTD')]//img");

            if (quoteImage != null)
            {
                imageBox.Source = new BitmapImage(new Uri("https://www.brainyquote.com" + quoteImage.GetAttributeValue("src", "")));
            }
            


;        }


        public void UpdateCountDownTimer(object o, object e)
        {
                var remainingTime = TimeCalculatorUtils.GetRemainingTimeFromNowToTheEndOfDay();
                this.countDownTxt.Text = remainingTime.ToString(@"hh\:mm\:ss");
                this.countDownInHoursTxt.Text = remainingTime.TotalHours.ToString("F2");
                this.countDownInMinutesTxt.Text = ((int)remainingTime.TotalMinutes).ToString();

                var remainingTimeToYearEnd = TimeCalculatorUtils.GetRemainingTimeFromNowToTheEndOfYear();
                this.countDownToYearEndTxt.Text = $"From now to the end of this year, remaining {(int)remainingTimeToYearEnd.TotalDays} days, {(int)remainingTimeToYearEnd.TotalHours} hours";
        }

        
    }
}
