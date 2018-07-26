using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Countdown.Tests.Models
{
    public class AppUsageRecordTests
    {
        [Fact]
        void Should_generate_id_with_correct_format()
        {
            string id = "date_20170808";
            var generatedId = AppUsageRecord.GetGeneratedId(new DateTime(2017, 8, 8));

            Assert.Equal(id, generatedId);
        }

        [Fact]
        void Should_instantiate_with_correct_properties()
        {
            var date = new DateTime(2018, 1, 1);
            var id = "date_20180101";
            var record = new AppUsageRecord(date);

            Assert.Equal(record.Date, date);
            Assert.Equal(record.Id, id);
            Assert.True(record.ActiveApps.Count == 0);
        }

        [Fact]
        void Should_merge_with_other_record_correctly()
        {
            var record1 = new AppUsageRecord(new DateTime(2018, 1, 1));
            record1.ActiveApps.Add(new KeyValuePair<string, ProcessInfo>("key1", new ProcessInfo("key1")
            {
                TotalAmountOfTime = TimeSpan.FromSeconds(1)
            }));

            var record2 = new AppUsageRecord(new DateTime(2018, 1, 1));
            record2.ActiveApps.Add(new KeyValuePair<string, ProcessInfo>("key2", new ProcessInfo("key2")
            {
                TotalAmountOfTime = TimeSpan.FromSeconds(2)
            }));


            record1.MergeWith(record2);

            Assert.Equal("key2", record1.ActiveApps["key2"].Id);
            Assert.Equal(TimeSpan.FromSeconds(2), record1.ActiveApps["key2"].TotalAmountOfTime);
        }
    }
}
