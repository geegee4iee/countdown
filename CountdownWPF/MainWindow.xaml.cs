﻿using Countdown.Core.Infrastructure;
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

        IRepository<AppUsageRecord> _repository = ServiceLocator.GetInstance<IRepository<AppUsageRecord>>();
        IRepository<AppUsageRecord> _backupRepository = ServiceLocator.GetInstance<IRepository<AppUsageRecord>>("LocalRepository");

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
            var backupRecord = _backupRepository.Get(AppUsageRecord.GetGeneratedId(DateTime.Now));

            try
            {
                _bufferedTodayAppRecord = _repository.Get(AppUsageRecord.GetGeneratedId(DateTime.Now));
                if (_bufferedTodayAppRecord == null)
                {
                    if (backupRecord != null)
                    {
                        _bufferedTodayAppRecord = backupRecord;
                    } else
                    {
                        _bufferedTodayAppRecord = new AppUsageRecord(DateTime.Now);
                    }
                }
            } catch (Exception)
            {

                _mainRepositoryUnavailable = true;
                _bufferedTodayAppRecord = backupRecord;
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
                BackupLocally();

                return;
            }

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

            Debug.WriteLine("Persisting data sucessfully");
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
                var processInfo = new ProcessInfo(activeProcess, Settings.MonitorInterval);

                var activeApps = _bufferedTodayAppRecord.ActiveApps;
                if (activeApps.ContainsKey(processInfo.Id))
                {
                    activeApps[processInfo.Id].TotalAmountOfTime += TimeSpan.FromSeconds(Settings.MonitorInterval);
                }
                else
                {
                    activeApps.Add(processInfo.Id, processInfo);
                }
            }
        }

        bool IsUserIdle()
        {
            var idleTime = UserInputInfoUtils.GetLastInputTime();
            if (idleTime > Settings.UserIdleWindow)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("User idle detected");
                }

                return true;
            }

            return false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _monitorActiveProcessTimer.Dispose();
            _persistentRecordTimer.Dispose();
            _caculateTimeTimer.Stop();

            if (Debugger.IsAttached) Debug.WriteLine("Closing the app");
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
