using Countdown.Core.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.MongoDb.Repository
{
    public class MongoDatabaseHolder
    {
        private static Lazy<IMongoDatabase> _database = new Lazy<IMongoDatabase>(InitializeMongoDatabase);

        public static IMongoDatabase Database
        {
            get
            {
                return _database.Value;
            }
        }

        private static IMongoDatabase InitializeMongoDatabase()
        {
            ConfigBsonMapping();
            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress("localhost", 27017);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);

            var client = new MongoClient(settings);
            var database = client.GetDatabase("active_app_record");

            return database;
        }

        private static void ConfigBsonMapping()
        {
            BsonClassMap.RegisterClassMap<AppUsageRecord>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Date).SetSerializer(new DateTimeSerializer(dateOnly: true, representation: MongoDB.Bson.BsonType.String));
            });
        }
    }
}
