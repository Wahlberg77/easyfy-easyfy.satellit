using System.Collections.Generic;
using System.ComponentModel;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Model.Comment;
using Easyfy.Satellit.Model.Posts;

namespace Easyfy.Satellit.Admin.Models
{
  public class CommentVm
  {
    public List<Comments_ByPosts.CommentBlah> MyTables { get; set; }
  }

  public class MyTable
  {
    public string Id { get; set; }

    public string BlogName { get; set; }

    public string PostName { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string Author { get; set; }

    public string Email { get; set; }

    public string Url { get; set; }
    public bool IsPublished { get; set; }
  }
}