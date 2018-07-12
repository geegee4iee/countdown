using Coundown.Spa.ReactJs.Core;
using Countdown.Core.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Hubs
{
    public class ActiveAppUsageHub : Hub
    {
        private readonly IAppSettings _appSettings;
        private readonly IMongoDatabase _mongoDatabase;

        public ActiveAppUsageHub(IMongoDatabase _mongoDatabase, IAppSettings _appSettings)
        {
            this._appSettings = _appSettings;
            this._mongoDatabase = _mongoDatabase;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
