using Countdown.Core.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;

namespace Countdown.MigrateData
{
    class Program
    {
        static void Main(string[] args)
        {
            BsonClassMap.RegisterClassMap<AppUsageRecord>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Date).SetSerializer(new DateTimeSerializer(dateOnly: true, representation: MongoDB.Bson.BsonType.String));
            });

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("active_app_record");



            var collection = database.GetCollection<AppUsageRecord>("daily_records");
            var results = collection.Find(Builders<AppUsageRecord>.Filter.Empty, null).ToList();



            foreach (var res in results)
            {
                var dateStr = res.Id.Split('_')[1];
                var date = DateTime.ParseExact(dateStr, "yyyyMMdd", null);
                if (res.Date != date)
                {
                    res.Date = date;
                    collection.ReplaceOne(Builders<AppUsageRecord>.Filter.Eq(c => c.Id, res.Id), res);
                    Console.WriteLine($"Updating item {res.Id} successfully!");
                }
            }
        }
    }
}
