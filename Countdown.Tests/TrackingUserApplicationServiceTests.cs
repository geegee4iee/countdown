using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using CountdownWPF.Services;
using Moq;
using System;
using Xunit;

namespace Countdown.Tests
{
    public class TrackingUserApplicationServiceTests
    {
        [Fact]
        void Instantiate_service_successfully()
        {
            var mockMainRepository = new Mock<IAppUsageRecordRepository>();
            var mockBackupRepository = new Mock<IAppUsageRecordRepository>();
            TrackingUserApplicationService _service = new TrackingUserApplicationService(mockMainRepository.Object, mockBackupRepository.Object);
        }

        [Fact]
        void BufferPersistentRecords_should_return_from_main_repository()
        {
            var mockMainRepository = new Mock<IAppUsageRecordRepository>();
            var mockBackupRepository = new Mock<IAppUsageRecordRepository>();
            TrackingUserApplicationService _service = new TrackingUserApplicationService(mockMainRepository.Object, mockBackupRepository.Object);

            var appUsageRecord = new AppUsageRecord(DateTime.Now);
            mockMainRepository.Setup(r => r.Get(AppUsageRecord.GetGeneratedId(DateTime.Now))).Returns(appUsageRecord);
            mockBackupRepository.Setup(r => r.Get(AppUsageRecord.GetGeneratedId(DateTime.Now))).Returns<AppUsageRecord>(null);

            _service.BufferPersistentRecords();

            var bufferedRecord = _service.GetBufferedRecord();

            Assert.Equal(appUsageRecord.Id, bufferedRecord.Id);
            Assert.Equal(appUsageRecord.Date, bufferedRecord.Date);
            Assert.Equal(appUsageRecord.ActiveApps, bufferedRecord.ActiveApps);
        }
    }
}
