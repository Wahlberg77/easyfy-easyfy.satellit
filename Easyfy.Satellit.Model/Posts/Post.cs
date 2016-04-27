using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Raven.Imports.Newtonsoft.Json;

namespace Easyfy.Satellit.Model.Posts
{
  public class Post
  {
    public Post()
    {
      Comments = new List<Comment.Comment>();
      InternalTags = new List<string>();
    }

    public string Id { get; set; }

    public const string Idprefix = "posts/";

    public List<string> InternalTags { get; set; }

    [Required]
    [DisplayName("Titel på inlägget")]
    public string Title { get; set; }

    public string FriendlyUrl { get; set; }

    [AllowHtml]
    [DisplayName("Innehåll")]
    public string Content { get; set; }

    [JsonIgnore]
    [DisplayName("Taggar")]
    public string Tags { get; set; }

    [Required]
    public PostStatus Status { get; set; }

    [Required(ErrorMessage = "Datum måste anges")]
    [DataType(DataType.Date)]
    [DisplayName("Skapad datum")]
    public DateTime Created { get; set; }

    [Required]
    [DisplayName("Blogg")]
    public string BlogReference { get; set; }

    [Required]
    [DisplayName("Författare")]
    public string Author { get; set; }

    public List<Comment.Comment> Comments { get; set; }

    public List<string> InternalSites { get; set; }
  }

  public enum PostStatus
  {
    Draft,
    Active,
    Deleted
  }
}