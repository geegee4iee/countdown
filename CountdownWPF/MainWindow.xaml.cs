using Countdown.Core.Infrastructure;
using Countdown.Core.Utils;
using CountdownWPF.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Threading;

namespace CountdownWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _caculateTimeTimer = new DispatcherTimer();

        TrackingUserApplicationService _trackingService;

        string _originalWindowTitle = null;
        string _idleWindowTitle = null;

        System.Windows.Forms.NotifyIcon notifyIcon = null;

        public MainWindow()
        {
            InitializeComponent();

            InitializeServices();

            CreateNotifyIcon();

            RegisterUIUpdateDispatchers();
        }

        private void InitializeServices()
        {
            _trackingService = new TrackingUserApplicationService(ServiceLocator.GetInstance<IAppUsageRecordRepository>(), ServiceLocator.GetInstance<IAppUsageRecordRepository>("LocalRepository"));

            Task.Run(() =>
            {
                _trackingService.StartTracking();
            });

            ChromeTabBlockingService.Instance.StartListening();
        }

        protected override void OnInitialized(EventArgs e)
        {
            _originalWindowTitle = this.Title;
            _idleWindowTitle = _originalWindowTitle + " (Idle)";
            base.OnInitialized(e);
        }

        private void RegisterUIUpdateDispatchers()
        {
            _caculateTimeTimer.Interval = TimeSpan.FromSeconds(1);
            _caculateTimeTimer.Tick += UpdateCountDownTimer;
            _caculateTimeTimer.Start();
        }

        private void CreateNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.DoubleClick += OnNotifyIconDoublyClicked;
            Uri uri = new Uri("/Assets/clock-icon.ico", UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            notifyIcon.Icon = new System.Drawing.Icon(info.Stream);
            notifyIcon.Visible = false;
            notifyIcon.Text = "Minimized Countdown App";
        }

        private void OnNotifyIconDoublyClicked(object sender, EventArgs e)
        {
            if (!this.IsVisible)
            {
                this.Show();

                this.WindowState = WindowState.Normal;
                this.notifyIcon.Visible = false;
                _caculateTimeTimer.Start();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _caculateTimeTimer.Stop();
            _trackingService.StopTracking();

            ChromeTabBlockingService.Instance.StopListening();

            Debug.WriteLine("Closing the app");
        }

        protected override void OnDeactivated(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                notifyIcon.Visible = true;
                this.Hide();

                _caculateTimeTimer.Stop();
            }
            base.OnDeactivated(e);
        }


        public void UpdateCountDownTimer(object o, object e)
        {
            if (_trackingService.IsUserIdle())
            {
                this.Title = _idleWindowTitle;

                return;
            }
            else
            {
                this.Title = _originalWindowTitle;
            }

            var remainingTime = TimeCalculatorUtils.GetRemainingTimeFromNowToTheEndOfDay();
            this.countDownTxt.Text = remainingTime.ToString(@"hh\:mm\:ss");
            this.countDownInHoursTxt.Text = remainingTime.TotalHours.ToString("F2");
            this.countDownInMinutesTxt.Text = ((int)remainingTime.TotalMinutes).ToString();

            var remainingTimeToYearEnd = TimeCalculatorUtils.GetRemainingTimeFromNowToTheEndOfYear();
            this.countDownToYearEndTxt.Text = $"From now to the end of this year, remaining {(int)remainingTimeToYearEnd.TotalDays} days, {(int)remainingTimeToYearEnd.TotalHours} hours";
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            var window = new OptionsWindow();
            window.ShowDialog();
        }
    }
}
