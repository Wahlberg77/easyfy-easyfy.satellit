using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Easyfy.Satellit.Model.Posts
{
  public class Blog
  {
    public const string IdPrefix = "blogs/";
    public string Id { get; set; }
    //[Required]
    //[DisplayName("Ladda upp en banner")]
    //public HttpPostedFileBase Banner { get; set; }

    [Required]
    [DisplayName("Namn på bloggen*")]
    public string BlogName { get; set; }

    [Required]
    [DisplayName("META Description*")]
    public string Description { get; set; }

    [Required]
    public string Url { get; set; }

    public DateTime CreatedAt { get; set; }
  }
}