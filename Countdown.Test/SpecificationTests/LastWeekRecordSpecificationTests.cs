using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Countdown.Test.SpecificationTests
{
    public class LastWeekRecordSpecificationTests
    {
        [Fact]
        public void Filter_exact_date()
        {
            LastWeekRecordSpecification spec = new LastWeekRecordSpecification(new DateTime(2018, 7, 9));
            var expression = spec.ToExpression().Compile();
            Assert.True(expression(new AppUsageRecord(new DateTime(2018, 7, 2))));
            Assert.True(expression(new AppUsageRecord(new DateTime(2018, 7, 8))));
            Assert.False(expression(new AppUsageRecord(new DateTime(2018, 7, 9))));
            Assert.False(expression(new AppUsageRecord(new DateTime(2018, 7, 1))));
        }

        [Fact]
        public void Filter_for_sunday()
        {
            LastWeekRecordSpecification spec = new LastWeekRecordSpecification(new DateTime(2018, 7, 15));
            var expression = spec.ToExpression().Compile();
            Assert.True(expression(new AppUsageRecord(new DateTime(2018, 7, 2))));
            Assert.True(expression(new AppUsageRecord(new DateTime(2018, 7, 8))));
            Assert.False(expression(new AppUsageRecord(new DateTime(2018, 7, 9))));
            Assert.False(expression(new AppUsageRecord(new DateTime(2018, 7, 1))));
        }
    }
}
