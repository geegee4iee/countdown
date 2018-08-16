using System;
using System.Linq;
using Coundown.Spa.ReactJs.Settings;
using Countdown.Core.MachineLearning;
using Countdown.Core.Models;
using Countdown.ML.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Coundown.Spa.ReactJs.Services
{
    public class PredictKarmaService : IPredictKarmaService
    {
        private readonly Lazy<KarmaPredictor> _predictor = new Lazy<KarmaPredictor>();
        private readonly IMongoCollection<ProcessInfoLabeledItem> _collection;

        public PredictKarmaService(IMongoDatabase database, IOptions<AppSettings> appSettings)
        {
            _collection = database.GetCollection<ProcessInfoLabeledItem>(appSettings.Value.LabeledRecordCollectionName);
        }

        public Karma Predict(ProcessInfo item)
        {
            if (!_predictor.Value.IsModelTrained())
            {
                throw new NullReferenceException("Model hasn't been trained!");
            }

            return _predictor.Value.Predict(item);
        }

        public Karma[] Predict(ProcessInfo[] items)
        {
            if (!_predictor.Value.IsModelTrained())
            {
                throw new NullReferenceException("Model hasn't been trained!");
            }

            return _predictor.Value.Predict(items);
        }

        public void TrainModel()
        {
            if (_predictor.Value.IsModelTrained())
            {
                return;
            }

            var trainingData = _collection.Find(Builders<ProcessInfoLabeledItem>.Filter.Empty).ToList();
            _predictor.Value.TrainModel(trainingData);
            _predictor.Value.SaveModel();
        }
    }
}
