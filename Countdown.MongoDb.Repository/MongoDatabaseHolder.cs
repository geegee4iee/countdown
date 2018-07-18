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

            var client = new MongoClient(ConfigurationManager.AppSettings["MongoDbConnectionString"]);
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
