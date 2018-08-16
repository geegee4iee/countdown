using Coundown.Spa.ReactJs.Settings;
using Countdown.Core.MachineLearning;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public class ProcessInfoLabeledItemRepositoryAsync : IProcessInfoLabeledItemRepositoryAsync
    {
        private readonly IMongoCollection<ProcessInfoLabeledItem> _collection;

        public ProcessInfoLabeledItemRepositoryAsync(IMongoDatabase database, IOptions<AppSettings> appSettings)
        {
            this._collection = database.GetCollection<ProcessInfoLabeledItem>(appSettings.Value.LabeledRecordCollectionName);
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
