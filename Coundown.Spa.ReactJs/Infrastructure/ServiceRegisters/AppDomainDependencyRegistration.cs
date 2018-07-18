using Coundown.Spa.ReactJs.Core;
using Coundown.Spa.ReactJs.Services;
using Countdown.Core.MachineLearning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure.ServiceRegisters
{
    public static class AppDomainDependencyRegistration
    {
        public static void AddAppDomainDependencies(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(serviceProvider =>
            {
                var client = new MongoClient(config["MongoDbConnectionString"]);
                var database = client.GetDatabase(config["MongoDbDatabase"]);

                return database;
            });

            services.AddSingleton<IAppSettings>(new AppSettings(config));
            services.AddScoped<IAppUsageRecordRepositoryAsync, AppUsageRecordRepositoryAsync>();
            services.AddScoped<IProcessInfoLabeledItemRepositoryAsync, ProcessInfoLabeledItemRepositoryAsync>();
            services.AddSingleton<IPredictKarmaService, PredictKarmaService>();
        }
    }
}
