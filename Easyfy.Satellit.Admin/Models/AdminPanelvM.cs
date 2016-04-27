using System.Collections.Generic;
using Easyfy.Satellit.Model.Posts;

namespace Easyfy.Satellit.Admin.Models
{
  public class AdminPanelvM
  {
    public List<Post> Posts { get; set; }

    public int AllNotApprovedCommentsCount { get; set; }

    public string TakeIdNr(string id)
    {
      var idNr =  id.Split('/')[1];
      return idNr;
    }

    public List<Blog> Blogs { get; set; }

    public string TakeIdnr(string id)
    {
      var idNr = id.Split('/')[1];
      return idNr;
    }
  }
}