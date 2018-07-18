using Countdown.Core.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure.BsonMapping
{
    public class BsonConfiguration
    {
        public static void Add()
        {
            BsonClassMap.RegisterClassMap<AppUsageRecord>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Date).SetSerializer(new DateTimeSerializer(dateOnly: true, representation: MongoDB.Bson.BsonType.String));
            });
        }
    }
}
