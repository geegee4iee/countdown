using Coundown.Spa.ReactJs.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration config;

        public AppSettings(IConfiguration config)
        {
            this.config = config;
        }

        public string MongoDbCollectionName => config[nameof(MongoDbCollectionName)];
    }
}
