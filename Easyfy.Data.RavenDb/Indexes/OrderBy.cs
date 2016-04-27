using System.Linq;
using Easyfy.Satellit.Model.Posts;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Easyfy.Data.RavenDb.Indexes
{
  public class OrderBy : AbstractIndexCreationTask<Post>
  {
    public OrderBy()
    {
      Map = posts => from p in posts
        select new
        {
          p.BlogReference
        };
      Sort(x => x.BlogReference, SortOptions.String);
    }
  }
}