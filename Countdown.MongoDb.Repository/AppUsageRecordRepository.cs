using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.MongoDb.Repository
{
    public class AppUsageRecordRepository : IAppUsageRecordRepository
    {
        const string CollectionName = "daily_records";
        public void Add(AppUsageRecord record)
        {
            var recordCollection = MongoDatabaseHolder.Database.GetCollection<AppUsageRecord>(CollectionName);
            recordCollection.InsertOne(record);
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public AppUsageRecord Get(object id)
        {
            var recordCollection = MongoDatabaseHolder.Database.GetCollection<AppUsageRecord>(CollectionName);

            var record = recordCollection.Find(Builders<AppUsageRecord>.Filter.Eq(f => f.Id, (string)id)).FirstOrDefault();

            return record;
        }

        public void Update(AppUsageRecord entity, object id)
        {
            var collection = MongoDatabaseHolder.Database.GetCollection<AppUsageRecord>(CollectionName);

            var result = collection.ReplaceOne(Builders<AppUsageRecord>.Filter.Eq(f => f.Id, (string)id), entity);
        }
    }
}
