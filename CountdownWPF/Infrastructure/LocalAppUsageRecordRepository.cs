using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Infrastructure
{
    class LocalAppUsageRecordRepository : IAppUsageRecordRepository
    {
        readonly string savesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "saves");
        public void Add(AppUsageRecord entity)
        {
            string savesDir = savesFolderPath;
            if (!Directory.Exists(savesDir))
            {
                Directory.CreateDirectory(savesDir);
            }

            string fileName = Path.Combine(savesDir, entity.Id + ".bin");

            using (var saveFileStream = File.OpenWrite(fileName))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFileStream, entity);
                Debug.WriteLine("Backing up locally successfully");
            }
        }

        public AppUsageRecord Get(object id)
        {
            var path = Path.Combine(savesFolderPath, (string)id + ".bin");
            if (File.Exists(path)) {
                Debug.WriteLine("Reading local backup");
                using (var openFileStream = File.OpenRead(path))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    AppUsageRecord backup = (AppUsageRecord) deserializer.Deserialize(openFileStream);

                    return backup;
                }
            }

            return null;
        }

        public void Update(AppUsageRecord entity, object id)
        {
            string savesDir = savesFolderPath;
            if (!Directory.Exists(savesDir))
            {
                Directory.CreateDirectory(savesDir);
            }

            string fileName = Path.Combine(savesDir, (string) id + ".bin");

            using (var saveFileStream = File.OpenWrite(fileName))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFileStream, entity);
                Debug.WriteLine("Backing up locally successfully");
            }
        }

        public void Delete(object id)
        {
            string fileName = Path.Combine(savesFolderPath, (string)id + ".bin");

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
