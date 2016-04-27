using System.Data.Entity;

namespace Easyfy.Satellit.Admin.Models
{
    public class EasyfySatellitAdminContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public EasyfySatellitAdminContext() : base("name=EasyfySatellitAdminContext")
        {
        }

        public System.Data.Entity.DbSet<Easyfy.Satellit.Model.Posts.Post> Posts { get; set; }

        public System.Data.Entity.DbSet<Easyfy.Satellit.Model.Posts.Blog> Blogs { get; set; }

        public System.Data.Entity.DbSet<Easyfy.Satellit.Model.Comment.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<Easyfy.Satellit.Model.Tags.Tag> Tags { get; set; }
    
    }
}
