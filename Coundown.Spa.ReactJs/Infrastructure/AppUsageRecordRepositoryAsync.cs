using Coundown.Spa.ReactJs.Settings;
using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public class AppUsageRecordRepositoryAsync : IAppUsageRecordRepositoryAsync
    {
        private readonly IMongoDatabase _mongoDb;
        private readonly AppSettings _appSettings;

        public AppUsageRecordRepositoryAsync(IMongoDatabase mongoDatabase, IOptions<AppSettings> appSettings)
        {
            this._mongoDb = mongoDatabase;
            this._appSettings = appSettings.Value;
        }

        public async Task AddAsync(AppUsageRecord entity)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.DailyRecordCollectionName);
            await collection.InsertOneAsync(entity, null);
        }

        public async Task<IEnumerable<AppUsageRecord>> FindAsync(Specification<AppUsageRecord> specification)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.DailyRecordCollectionName);
            var findAsync = await collection.FindAsync(specification.ToExpression());
            return await findAsync.ToListAsync();
        }

        public async Task<AppUsageRecord> GetAsync(object id)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.DailyRecordCollectionName);
            var result = await collection.FindAsync(Builders<AppUsageRecord>.Filter.Eq(e => e.Id, (string)id));
            return await result.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(AppUsageRecord entity, object id)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.DailyRecordCollectionName);
            await collection.ReplaceOneAsync(Builders<AppUsageRecord>.Filter.Eq(f => f.Id, (string)id), entity);
        }
    }
}
