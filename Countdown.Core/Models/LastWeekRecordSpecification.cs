using Countdown.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Countdown.Core.Models
{
    public class LastWeekRecordSpecification : Specification<AppUsageRecord>
    {
        DateTime _beginOfLastWeek;
        DateTime _endOfLastWeek;
        public LastWeekRecordSpecification(DateTime currentDate)
        {
            _beginOfLastWeek = currentDate.Date.AddDays(-7 - (currentDate.DayOfWeek != DayOfWeek.Sunday ? currentDate.DayOfWeek - DayOfWeek.Monday : 7 - (int) DayOfWeek.Monday));
            _endOfLastWeek = currentDate.Date.AddDays(-7 + (currentDate.DayOfWeek != DayOfWeek.Sunday ? 7 - (int)currentDate.DayOfWeek : 0));
        }

        public override Expression<Func<AppUsageRecord, bool>> ToExpression()
        {
            return app => app.Date >= _beginOfLastWeek && app.Date <= _endOfLastWeek;
        }
    }
}
