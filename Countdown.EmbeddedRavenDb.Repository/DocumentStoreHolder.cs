using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database;
using Raven.Database.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Infrastructure
{
    public class DocumentStoreHolder
    {
        private static Lazy<IDocumentStore> _persistentStore = new Lazy<IDocumentStore>(CreatePersistentDocumentStore);

        private static IDocumentStore CreatePersistentDocumentStore()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080, false);
            var store = new EmbeddableDocumentStore
            {
               DefaultDatabase = "PersistentStore",
               UseEmbeddedHttpServer = true
            };

            store.Configuration.EnableResponseLoggingForEmbeddedDatabases = true;
            store.Initialize();

            return store;
        }

        public static IDocumentStore RavenStore => _persistentStore.Value;

        private static IDocumentStore CreateDocumentStore()
        {

            var store = new EmbeddableDocumentStore
            {
                DataDirectory = "Data",
                RunInMemory = true,
            };

            store.Configuration.Storage.Voron.AllowOn32Bits = true;
            store.Initialize();

            return store;
        }

        public static void FreeDocumentStores()
        {
            if (!_persistentStore.Value.WasDisposed)
            {
                _persistentStore.Value.Dispose();
            }
        }

    }
}
