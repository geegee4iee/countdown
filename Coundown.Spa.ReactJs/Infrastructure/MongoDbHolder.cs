using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public class MongoDbHolder
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
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("active_app_record");

            return database;
        }
    }
}
