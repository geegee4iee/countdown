using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Raven.Json.Linq;

namespace CountdownWPF.Infrastructure
{
    class EmbeddedRecordRepository : IAppUsageRecordRepository
    {
        public void Update(AppUsageRecord record)
        {
            var store = DocumentStoreHolder.RavenStore;

            if (store.WasDisposed) return;

            using (var session = store.OpenSession())
            {
                store.DatabaseCommands.Put(record.Id, null, RavenJObject.FromObject(record), new RavenJObject());

                session.SaveChanges();
            }
        }

        public AppUsageRecord Get(string id)
        {
            if (DocumentStoreHolder.RavenStore.WasDisposed) return null;

            using (var session = DocumentStoreHolder.RavenStore.OpenSession())
            {
                var record = session.Load<AppUsageRecord>(id);

                return record;
            }
        }

        public void Add(AppUsageRecord record)
        {
            var store = DocumentStoreHolder.RavenStore;

            if (store.WasDisposed) return;

            using (var session = store.OpenSession())
            {
                session.Store(record, record.Id);

                session.SaveChanges();
            }
        }

        public AppUsageRecord Get(object id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(AppUsageRecord entity, object id)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(object id)
        {
            throw new System.NotImplementedException();
        }
    }
}
