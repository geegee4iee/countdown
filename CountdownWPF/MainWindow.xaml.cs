using Countdown.Core.Factories;
using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Countdown.Core.Utils;
using CountdownWPF.Core;
using CountdownWPF.Infrastructure;
using CountdownWPF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Security.Principal;
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
        Timer _monitorActiveProcessTimer = null;
        Timer _persistentRecordTimer = null;

        string _originalWindowTitle = null;
        string _idleWindowTitle = null;

        bool _isIdle = false;
        bool _mainRepositoryUnavailable = false;

        IAppUsageRecordRepository _repository = ServiceLocator.GetInstance<IAppUsageRecordRepository>();
        IAppUsageRecordRepository _backupRepository = ServiceLocator.GetInstance<IAppUsageRecordRepository>("LocalRepository");

        public AppUsageRecord _bufferedTodayAppRecord = null;

        System.Windows.Forms.NotifyIcon notifyIcon = null;

        public MainWindow()
        {
            InitializeComponent();

            CreateNotifyIcon();

            BufferPersistentRecords();

            RegisterUIUpdateDispatchers();

            InitializeTimerJobs();
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

        private void BufferPersistentRecords()
        {
            var todayRecordId = AppUsageRecord.GetGeneratedId(DateTime.Now);

            try
            {
                _bufferedTodayAppRecord = _repository.Get(todayRecordId);
                var backupRecord = _backupRepository.Get(todayRecordId);

                if (_bufferedTodayAppRecord == null)
                {
                    _bufferedTodayAppRecord = new AppUsageRecord(DateTime.Now);
                }

                if (backupRecord != null)
                {
                    _bufferedTodayAppRecord.MergeWith(backupRecord);
                    _backupRepository.Delete(todayRecordId);
                }
            }
            catch (TimeoutException ex)
            {
                Debug.WriteLine(ex.Message);
                _mainRepositoryUnavailable = true;

                var backupRecord = _backupRepository.Get(todayRecordId);
                if (backupRecord == null)
                {
                    _bufferedTodayAppRecord = new AppUsageRecord(DateTime.Now);
                }
                else
                {
                    _bufferedTodayAppRecord = backupRecord;
                }
            }
        }

        private void InitializeTimerJobs()
        {

            _monitorActiveProcessTimer = new Timer(MonitorUserActiveProcess, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(Settings.MonitorInterval));
            _persistentRecordTimer = new Timer(PersistentRecords, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(Settings.PersistentRecordInterval));
        }

        private void PersistentRecords(object state)
        {
            Debug.WriteLine($"{nameof(PersistentRecords)} is running on thread id={Thread.CurrentThread.ManagedThreadId}");

            if (_isIdle) return;

            if (_mainRepositoryUnavailable)
            {
                var result = TryGetFromMainRepository();

                if (result.Item2 == false)
                {
                    BackupLocally();
                    return;
                } else
                {
                    _mainRepositoryUnavailable = false;

                    if (result.Item1 != null)
                    {
                        _bufferedTodayAppRecord.MergeWith(result.Item1);
                        _backupRepository.Delete(_bufferedTodayAppRecord.Id);
                    }
                }
            }

            try
            {
                var todayPersistentRecord = _repository.Get(AppUsageRecord.GetGeneratedId(DateTime.Now));
                if (todayPersistentRecord != null)
                {
                    todayPersistentRecord.ActiveApps = _bufferedTodayAppRecord.ActiveApps;

                    _repository.Update(todayPersistentRecord, todayPersistentRecord.Id);
                }
                else
                {
                    _repository.Add(_bufferedTodayAppRecord);
                }
            } catch (Exception ex)
            {
                Debug.WriteLine($"Error when persisting data: {ex.Message}");
                _mainRepositoryUnavailable = true;

                this.PersistentRecords(state);
            }
            

            Debug.WriteLine("Persisting data sucessfully");
        }

        private Tuple<AppUsageRecord, bool> TryGetFromMainRepository()
        {
            AppUsageRecord appRecord = null;

            try
            {
                appRecord = _repository.Get(AppUsageRecord.GetGeneratedId(DateTime.Now));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                return new Tuple<AppUsageRecord, bool>(null, false);
            }


            return new Tuple<AppUsageRecord, bool>(appRecord, true);
        }

        private void BackupLocally()
        {
            _backupRepository.Update(_bufferedTodayAppRecord, _bufferedTodayAppRecord.Id);
        }

        public void MonitorUserActiveProcess(object state)
        {
            Debug.WriteLine($"{nameof(MonitorUserActiveProcess)} is running on thread id={Thread.CurrentThread.ManagedThreadId}");

            if (this._isIdle = IsUserIdle()) return;

            var activeProcess = ProcessUtils.GetActiveProcess();
            if (activeProcess != null)
            {
                var processInfoId = ProcessInfoFactory.CreateId(activeProcess);
                var activeApps = _bufferedTodayAppRecord.ActiveApps;
                if (!string.IsNullOrWhiteSpace(processInfoId) && activeApps.ContainsKey(processInfoId))
                {
                    activeApps[processInfoId].TotalAmountOfTime += TimeSpan.FromSeconds(Settings.MonitorInterval);
                }
                else
                {
                    var processInfo = ProcessInfoFactory.Create(activeProcess, Settings.MonitorInterval);
                    activeApps.Add(processInfo.Id, processInfo);
                }
            }
        }

        bool IsUserIdle()
        {
            var idleTime = UserInputInfoUtils.GetLastInputTime();
            if (idleTime > Settings.UserIdleWindow)
            {
                Debug.WriteLine("User idle detected");

                return true;
            }

            return false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _monitorActiveProcessTimer.Dispose();
            _persistentRecordTimer.Dispose();
            _caculateTimeTimer.Stop();

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
            Debug.WriteLine($"{nameof(UpdateCountDownTimer)} is running on thread id={Thread.CurrentThread.ManagedThreadId}");

            if (_isIdle)
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


    }
}
