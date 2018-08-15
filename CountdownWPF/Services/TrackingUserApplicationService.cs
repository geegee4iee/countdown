using Countdown.Core.Factories;
using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using CountdownWPF.Configurations;
using CountdownWPF.Utils;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CountdownWPF.Services
{
    public class TrackingUserApplicationService
    {
        IAppUsageRecordRepository _repository;
        IAppUsageRecordRepository _backupRepository;

        Timer _monitorActiveProcessTimer = null;
        Timer _persistentRecordTimer = null;

        bool _isIdle = false;
        bool _mainRepositoryUnavailable = false;

        public AppUsageRecord _bufferedTodayAppRecord = null;

        string _currentProcessId = string.Empty;

        public TrackingUserApplicationService(IAppUsageRecordRepository repository, IAppUsageRecordRepository backupRepository)
        {
            _repository = repository;
            _backupRepository = backupRepository;
        }

        public void StartTracking(bool bufferPersistentRecords = true)
        {
            if (bufferPersistentRecords) BufferPersistentRecords();

            if (Debugger.IsAttached)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        MonitorUserActiveProcess(null);

                        await Task.Delay(1000);
                    }
                });
            } else
            {
                
                _monitorActiveProcessTimer = new Timer(MonitorUserActiveProcess, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(Settings.MonitorInterval));
                _persistentRecordTimer = new Timer(PersistentRecords, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(Settings.PersistentRecordInterval));
            }
        }

        public void StopTracking()
        {
            _monitorActiveProcessTimer?.Dispose();
            _persistentRecordTimer?.Dispose();
        }

        private void MonitorUserActiveProcess(object state)
        {
            Debug.WriteLine($"{nameof(MonitorUserActiveProcess)} is running on thread id={Thread.CurrentThread.ManagedThreadId}");

            if (DateTime.Now.Date != _bufferedTodayAppRecord.Date)
            {
                _bufferedTodayAppRecord = new AppUsageRecord(DateTime.Now);
            }

            if (this._isIdle = IsUserIdle()) return;

            var activeProcess = ProcessUtils.GetActiveProcess();
            if (activeProcess != null)
            {
                var processInfoId = ProcessInfoFactory.CreateId(activeProcess);
                var activeApps = _bufferedTodayAppRecord.ActiveApps;
                if (!string.IsNullOrWhiteSpace(processInfoId) && activeApps.ContainsKey(processInfoId))
                {
                    var processInfo = activeApps[processInfoId];
                    if (string.IsNullOrEmpty(_currentProcessId)) _currentProcessId = processInfoId;

                    if (_currentProcessId != processInfoId)
                    {
                        var oldProcessInfo = activeApps[_currentProcessId];
                        oldProcessInfo.EndCurrentSession(DateTime.Now.TimeOfDay);

                        processInfo.StartNewSession(DateTime.Now.TimeOfDay);
                    }
                    
                    processInfo.TotalAmountOfTime += TimeSpan.FromSeconds(Settings.MonitorInterval);
                }
                else
                {
                    var processInfo = ProcessInfoFactory.Create(activeProcess, Settings.MonitorInterval);
                    processInfo.StartNewSession(DateTime.Now.TimeOfDay);

                    activeApps.Add(processInfo.Id, processInfo);
                }

                _currentProcessId = processInfoId;
            }
        }

        public bool IsUserIdle()
        {
            var idleTime = UserInputInfoUtils.GetLastInputTime();
            if (idleTime > Settings.UserIdleWindow)
            {
                Debug.WriteLine("User idle detected");

                return true;
            }

            return false;
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
                }
                else
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error when persisting data: {ex.Message}");

                LoggingService.LogException(ex);

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

        public AppUsageRecord GetBufferedRecord()
        {
            return _bufferedTodayAppRecord;
        }

        public void BufferPersistentRecords()
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
    }
}
