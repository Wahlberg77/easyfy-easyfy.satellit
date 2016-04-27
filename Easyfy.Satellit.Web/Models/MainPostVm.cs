using System;
using System.Collections.Generic;
using System.Linq;
using Easyfy.Satellit.Model.Posts;

namespace Easyfy.Satellit.Web.Models
{
  public class MainPostVm
  {
    public string BlogRef { get; set; }

    public List<Post> Posts { get; set; }

    public Blog Blogs { get; set; }

    public string GetParagraphs(string html, int numberOfParagraphs)
    {
      const string paragraphSeparator = "</p>";
      var paragraphs = html.Split(new[] { paragraphSeparator }, StringSplitOptions.RemoveEmptyEntries);
      return string.Join("", paragraphs.Take(numberOfParagraphs).Select(paragraph => paragraph + paragraphSeparator));
    }

  }
}
