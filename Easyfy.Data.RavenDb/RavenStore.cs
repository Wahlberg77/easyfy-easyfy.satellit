using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easyfy.Data.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;

namespace Easyfy.Data.RavenDb
{
  public class RavenStore
  {
    private static readonly object LockObject = new object();

    private static volatile IDocumentStore _documentStore;

    static RavenStore()
    {
    }

    public static IDocumentStore DocumentStore
    {
      get
      {
        if (_documentStore != null)
          return _documentStore;

        lock (LockObject)
        {
          if (_documentStore != null)
            return _documentStore;


          var documentStore = new DocumentStore
          {
            ConnectionStringName = "CommerceDb",
            DefaultDatabase = ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]
          };

          _documentStore = documentStore.Initialize();
          if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]))
            _documentStore.DatabaseCommands.EnsureDatabaseExists(ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]);

          if ((ConfigurationManager.AppSettings.Get("RavenDb.CreateIndexesOnStartup") ?? "false").Equals("true",StringComparison.CurrentCultureIgnoreCase))
          {
            var catalog = new CompositionContainer(new AssemblyCatalog(typeof(Users_View).Assembly));
            IndexCreation.CreateIndexes(catalog, _documentStore.DatabaseCommands.ForDatabase(ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]), _documentStore.Conventions);
          }
        }

        return _documentStore;
      }
    }

    public static string DatabaseName { get { return ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]; } }

    public static IDocumentSession GetSession()
    {
      if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]))
        return DocumentStore.OpenSession(ConfigurationManager.AppSettings["RavenDb.DefaultDataBase"]);

      return DocumentStore.OpenSession();
    }

    public static IAsyncDocumentSession GetAsyncSession()
    {
      if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"]))
        return DocumentStore.OpenAsyncSession(ConfigurationManager.AppSettings["RavenDb.DefaultDataBase"]);

      return DocumentStore.OpenAsyncSession();
    }
  }
}
