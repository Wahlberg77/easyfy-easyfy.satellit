using System.Linq;
using Easyfy.Satellit.Model.Posts;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Easyfy.Data.RavenDb.Indexes
{
  public class Posts_View : AbstractIndexCreationTask<Post>
  {
    public Posts_View()
    {
      Map = posts => from p in posts
        select new
        {
          SearchMe = string.Join(p.BlogReference, p.Title, p.Created, p.InternalTags),
          p.Title,
          p.BlogReference,
          p.Created,
          p.InternalTags,
          p.FriendlyUrl,
          p.Status
        };
      Store("SearchMe", FieldStorage.Yes);
      Index("SearchMe", FieldIndexing.Analyzed);
      Index(o => o.BlogReference, FieldIndexing.NotAnalyzed);
      Index(o => o.Title, FieldIndexing.Analyzed);
      Index(o => o.Created, FieldIndexing.Analyzed);
      Index(o => o.InternalTags, FieldIndexing.Analyzed);
      Index(o => o.Status, FieldIndexing.Analyzed);
      Index(o => o.FriendlyUrl, FieldIndexing.NotAnalyzed);
    }
  }

  public class Comments_ByPosts : AbstractIndexCreationTask<Post, Comments_ByPosts.CommentBlah>
  {
    public class CommentBlah
    {
      public string CommentId { get; set; }
      public string BlogName { get; set; }
      public string PostId { get; set; }
      public string PostTitle { get; set; }
      public string CommentTitle  { get; set; }
      public string CommentAuthor { get; set; }
      public string CommentContent { get; set; }
      public string CommentEmail { get; set; }
      public string CommentUrl { get; set; }
      public bool IsPublished { get; set; }
    }

    public Comments_ByPosts()
    {
      Map = posts => from post in posts
        from comment in post.Comments
        let blog = LoadDocument<Blog>(post.BlogReference)
        select new
        {
          CommentId = comment.Id,
          blog.BlogName,
          PostId = post.Id,
          PostTitle = post.Title,
          CommentTitle = comment.Title,
          CommentAuthor = comment.Author,
          CommentContent = comment.Content,
          CommentEmail = comment.Email,
          CommentUrl = comment.Url,
          comment.IsPublished
        };

      Store("BlogName", FieldStorage.Yes);
      Store("CommentId", FieldStorage.Yes);
      Store("PostId", FieldStorage.Yes);
      Store("PostTitle", FieldStorage.Yes);
      Store("CommentAuthor", FieldStorage.Yes);
      Store("CommentTitle", FieldStorage.Yes);
      Store("CommentContent", FieldStorage.Yes);
      Store("CommentEmail", FieldStorage.Yes);
      Store("CommentUrl", FieldStorage.Yes);
      Store("IsPublished", FieldStorage.Yes);

    }
  }

 
}