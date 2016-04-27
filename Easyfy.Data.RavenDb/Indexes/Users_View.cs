using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easyfy.Satellit.Model.Membership;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Easyfy.Data.RavenDb.Indexes
{
  public class Users_View : AbstractIndexCreationTask<User>
  {
    public Users_View()
    {
      Map = users => from u in users
                     select new
                     {
                       u.UserName,
                     };

      Index(o => o.UserName, FieldIndexing.NotAnalyzed);
    }
  }
}
