using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Core
{
    public interface IAppSettings
    {
        string MongoDbCollectionName { get; }
    }
}
