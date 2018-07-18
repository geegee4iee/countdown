using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public interface IAppUsageRecordRepositoryAsync: IRepository<AppUsageRecord>
    {
        Task AddAsync(AppUsageRecord entity);
        Task<AppUsageRecord> GetAsync(object id);
        Task UpdateAsync(AppUsageRecord entity, object id);
        Task<IEnumerable<AppUsageRecord>> FindAsync(Specification<AppUsageRecord> specification);
    }
}
