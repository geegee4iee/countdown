using Coundown.Spa.ReactJs.Core;
using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public class AppUsageRecordRepository : IRepository<AppUsageRecord>
    {
        private readonly IMongoDatabase _mongoDb;
        private readonly IAppSettings _appSettings;

        public AppUsageRecordRepository(IMongoDatabase mongoDatabase, IAppSettings appSettings)
        {
            this._mongoDb = mongoDatabase;
            this._appSettings = appSettings;
        }

        public void Add(AppUsageRecord entity)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(AppUsageRecord entity)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.MongoDbCollectionName);
            await collection.InsertOneAsync(entity, null);
        }

        public AppUsageRecord Get(object id)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.MongoDbCollectionName);
            return collection.Find(Builders<AppUsageRecord>.Filter.Eq(e => e.Id, (string)id)).FirstOrDefault();
        }

        public async Task<AppUsageRecord> GetAsync(object id)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.MongoDbCollectionName);
            var result = await collection.FindAsync(Builders<AppUsageRecord>.Filter.Eq(e => e.Id, (string)id));
            return await result.FirstOrDefaultAsync();
        }

        public void Update(AppUsageRecord entity, object id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(AppUsageRecord entity, object id)
        {
            var collection = _mongoDb.GetCollection<AppUsageRecord>(_appSettings.MongoDbCollectionName);
            await collection.ReplaceOneAsync(Builders<AppUsageRecord>.Filter.Eq(f => f.Id, (string)id), entity);
        }
    }
}
