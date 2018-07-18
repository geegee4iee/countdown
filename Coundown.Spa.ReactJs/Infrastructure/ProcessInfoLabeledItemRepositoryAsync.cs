using Coundown.Spa.ReactJs.Core;
using Countdown.Core.MachineLearning;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public class ProcessInfoLabeledItemRepositoryAsync : IProcessInfoLabeledItemRepositoryAsync
    {
        private readonly IMongoCollection<ProcessInfoLabeledItem> _collection;

        public ProcessInfoLabeledItemRepositoryAsync(IMongoDatabase database, IAppSettings appSettings)
        {
            this._collection = database.GetCollection<ProcessInfoLabeledItem>(appSettings.LabeledRecordCollectionName);
        }

        public async Task Add(ProcessInfoLabeledItem item)
        {
            if (await _collection.CountDocumentsAsync(Builders<ProcessInfoLabeledItem>.Filter.Eq(f => f.Id, item.Id)) == 0)
            {
                await _collection.InsertOneAsync(item);
            }
        }
    }
}
