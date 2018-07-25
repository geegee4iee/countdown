using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Countdown.Core.Infrastructure
{
    public interface IAppUsageRecordRepository: IRepository<AppUsageRecord>
    {
        AppUsageRecord Get(object id);
        void Add(AppUsageRecord appUsageRecord);
        void Update(AppUsageRecord appUsage, object id);
        void Delete(object id);
    }
}
