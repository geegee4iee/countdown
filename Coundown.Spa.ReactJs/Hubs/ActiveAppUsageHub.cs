using Coundown.Spa.ReactJs.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Hubs
{
    public class ActiveAppUsageHub : Hub
    {
        private readonly AppSettings _appSettings;
        private readonly IMongoDatabase _mongoDatabase;

        public ActiveAppUsageHub(IMongoDatabase _mongoDatabase, IOptions<AppSettings> _appSettings)
        {
            this._appSettings = _appSettings.Value;
            this._mongoDatabase = _mongoDatabase;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
