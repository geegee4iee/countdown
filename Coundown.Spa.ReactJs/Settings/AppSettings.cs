using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Settings
{
    public class AppSettings
    {
        public string MongoConnectionString { get; set; }
        public string DailyRecordCollectionName { get; set; }
        public string LabeledRecordCollectionName { get; set; }
    }
}
