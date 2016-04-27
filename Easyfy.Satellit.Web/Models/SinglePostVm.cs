using System.Collections.Generic;
using Easyfy.Satellit.Model.Comment;
using Easyfy.Satellit.Model.Posts;

namespace Easyfy.Satellit.Web.Models
{
  public class SinglePostVm
  {
    public SinglePostVm()
    {
      Posts = new List<Post>();
      Blog = new Blog();
    }

    public Post Post { get; set; }
    public Blog Blog { get; set; }
    public string BlogRef { get; set; }
    public Comment Comment { get; set; }
    public List<Post> Posts { get; set; }

    public string PostId
    {
      get { return Post.Id.Split('/')[1]; }
    }
  }
}