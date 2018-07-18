using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Countdown.Core.MachineLearning;
using Countdown.Core.Utils;
using Countdown.ML.ExternalUtils;
using DataAccess;
using libsvm;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Countdown.ML
{
    public class Record
    {
        public string Text { get; set; }
        public bool IsSunny { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {

            
            CreateTrainingData();
            // TrainingRecordData();


        }

        private static void CreateTrainingData()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("active_app_record");
            var collection = database.GetCollection<AppUsageRecord>("daily_records");

            var records = collection.Find(Builders<AppUsageRecord>.Filter.Empty).ToList();

            var trainingDataCollection = database.GetCollection<ProcessInfoLabeledItem>("training_data");

            string userInput;

            var hashAlgorithm = new Murmur3Hash();

            foreach (var appUsageRecord in records)
            {
                foreach (var processInfo in appUsageRecord.ActiveApps)
                {
                    var trainingItem = new ProcessInfoLabeledItem
                    {
                        Title = processInfo.Value.MainWindowTitle,
                        Process = processInfo.Value.ProcessName
                    };

                    trainingItem.GenerateId(hashAlgorithm);

                    var persistentItem = trainingDataCollection.Find(Builders<ProcessInfoLabeledItem>.Filter.Eq(f => f.Id, trainingItem.Id)).FirstOrDefault();
                    if (persistentItem != null) continue;

                    Console.WriteLine($"Is Title [{processInfo.Value.MainWindowTitle}] in process [{processInfo.Value.ProcessName}] good or bad? n-(Neural), g-(Good), b-(Bad):");
                    userInput = Console.ReadLine().Trim().ToLower();

                    if (userInput != "b" && userInput != "n")
                    {
                        trainingItem.Category = Karma.Good;

                    }
                    else if (userInput == "n")
                    {
                        trainingItem.Category = Karma.Neutral;

                    }
                    else
                    {
                        trainingItem.Category = Karma.Bad;
                    }

                    if (persistentItem == null)
                    {
                        Console.WriteLine($"Inserted item with category {trainingItem.Category.ToString()}");
                        trainingDataCollection.InsertOne(trainingItem);
                    }
                    else
                    {
                        Console.WriteLine($"Updated item with category {trainingItem.Category.ToString()}");
                        trainingDataCollection.ReplaceOne(Builders<ProcessInfoLabeledItem>.Filter.Eq(f => f.Id, trainingItem.Id), trainingItem);
                    };
                }
            }
        }

        private static void TrainingRecordData()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("active_app_record");

            Func<string, string[]> filter = w => Regex.Replace(w, @"[^a-zA-Z]", " ").Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d.Count() < 20).ToArray();
            var trainingDataCollection = database.GetCollection<ProcessInfoLabeledItem>("training_data");
            var records = trainingDataCollection.Find(Builders<ProcessInfoLabeledItem>.Filter.Empty).ToList();
            var vocabulary = records.Select(c=> c.Title + " " + c.Process).SelectMany(filter).Distinct().OrderBy(str => str).ToList();

            List<string> x = records.Select(item => item.Title + " " + item.Process).ToList();
            double[] y = records.Select(item => (double)item.Category).ToArray();

            var problemBuilder = new TextClassificationProblemBuilder();

            problemBuilder.RefineText = filter;
            var problem = problemBuilder.CreateProblem(x, y, vocabulary.ToList());

            const int C = 1;
            var model = new C_SVC(problem, KernelHelper.LinearKernel(), C);
            var _predictionDictionary = new Dictionary<Karma, string> { { Karma.Bad, "Bad" }, { Karma.Good, "Good" }, { Karma.Neutral, "Neutral" } };

            var newXs = database.GetCollection<AppUsageRecord>("daily_records").Find(Builders<AppUsageRecord>.Filter.Eq(f => f.Id, AppUsageRecord.GetGeneratedId(DateTime.Now))).FirstOrDefault().ActiveApps.Select(c=> c.Value).Select(c => c.MainWindowTitle + " " + c.ProcessName);

            foreach (var _x in newXs)
            {
                var newX = TextClassificationProblemBuilder.CreateNode(_x, vocabulary, problemBuilder.RefineText);
                var predictedY = model.Predict(newX);
                Console.WriteLine($"For title {_x}");
                Console.WriteLine($"The prediction is {_predictionDictionary[(Karma)predictedY]}");
            }

        }

        private void TrainingData()
        {
            string dateFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"sunnyData.csv");
            var dataTable = DataTable.New.ReadCsv(dateFilePath);
            List<string> x = dataTable.Rows.Select(row => row["Text"]).ToList();
            double[] y = dataTable.Rows.Select(row => double.Parse(row["IsSunny"])).ToArray();

            var vocabulary = x.SelectMany(GetWords).Distinct().OrderBy(w => w).ToList();

            var problemBuilder = new TextClassificationProblemBuilder();
            var problem = problemBuilder.CreateProblem(x, y, vocabulary.ToList());

            const int C = 1;
            var model = new C_SVC(problem, KernelHelper.LinearKernel(), C);

            string userInput;
            var _predictionDictionary = new Dictionary<int, string> { { -1, "Rainy" }, { 1, "Sunny" } };
            do
            {
                userInput = Console.ReadLine();
                var newX = TextClassificationProblemBuilder.CreateNode(userInput, vocabulary);

                var predictedY = model.Predict(newX);
                Console.WriteLine("The prediction is {0}", _predictionDictionary[(int)predictedY]);
                Console.WriteLine(new string('=', 50));
            } while (userInput != "quit");

            Console.WriteLine("");
        }

        private static IEnumerable<string> GetWords(string x)
        {
            return x.Split(new[] { ' ', '\t', '-', '_' }, StringSplitOptions.RemoveEmptyEntries).Select(c=> c.Trim().ToLower());
        }
    }
}
