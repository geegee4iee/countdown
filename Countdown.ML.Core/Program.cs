using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Countdown.Core.MachineLearning;
using Countdown.Core.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Countdown.ML.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = Task.Run(Train);
            t.Wait();

            var t2 = Task.Run(Verify);
            t2.Wait();

        }

        private static async Task Verify()
        {
            KarmaPredictor predictor = new KarmaPredictor();

            if (!predictor.IsModelTrained()) return;

            predictor.LoadModel();

            Func<string, string[]> filter = w => Regex.Replace(w, @"[^a-zA-Z]", " ").Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d.Count() < 20).ToArray();

            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("active_app_record");
            var trainingCollection = db.GetCollection<AppUsageRecord>("daily_records");
            var cursor = await trainingCollection.FindAsync(Builders<AppUsageRecord>.Filter.Eq(f => f.Id, AppUsageRecord.GetGeneratedId(DateTime.Now)));
            var trainingData = await cursor.FirstOrDefaultAsync();


            var processInfos = trainingData.ActiveApps.Select(f => f.Value).ToArray();

            var predictions = predictor.Predict(processInfos);

            for (int i = 0; i < processInfos.Length; i++)
            {
                Console.WriteLine($"for window title [{processInfos[i].MainWindowTitle}] result: {predictions[i]}");
            }
        }

        private static async Task Train()
        {
            KarmaPredictor predictor = new KarmaPredictor();
            if (predictor.IsModelTrained()) return;

            Func<string, string[]> filter = w => Regex.Replace(w, @"[^a-zA-Z]", " ").Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d.Count() < 20).ToArray();

            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("active_app_record");
            var trainingCollection = db.GetCollection<ProcessInfoLabeledItem>("training_data");
            var cursor = await trainingCollection.FindAsync(Builders<ProcessInfoLabeledItem>.Filter.Empty);
            var trainingData = await cursor.ToListAsync();

            predictor.TrainModel(trainingData);
            predictor.SaveModel();

            Console.WriteLine("Learning completed, save trained model to disk");

        }
    }
}
