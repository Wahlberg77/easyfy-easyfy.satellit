using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Easyfy.Satellit.Model.Comment
{
  public class Comment
  {
    public string Id { get; set; }

    [Required(ErrorMessage = "Ange en titel på er kommentar")]
    [DisplayName("Titel*")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Skriv en kommentar")]
    [DisplayName("Kommentar*")]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; }

    [Required(ErrorMessage = "Ange ert namn")]
    [DisplayName("Namn*")]
    public string Author { get; set; }

    [Required(ErrorMessage = "Ange er Epost")]
    [DisplayName("Epost*")]
    public string Email { get; set; }

    public string Url { get; set; }

    public bool IsPublished { get; set; }

  }
}