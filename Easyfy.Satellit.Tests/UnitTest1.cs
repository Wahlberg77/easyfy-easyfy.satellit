using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Easyfy.Data.RavenDb;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Model.Posts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Easyfy.Satellit.Tests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void CehckIftagsIsAddedCorrectly()
    {
      var myListOfStrings = new List<string>();
      var tags = "fsdfsd, öl, wqwq";
      var result = SeparateTags(tags);

      Assert.AreEqual(3, result.Count);
      Assert.AreEqual("öl", result[1]);
    }

    private List<string> SeparateTags(string tags)
    {
      var tagArr = tags.Split(',');

      return tagArr.Select(tag => Regex.Replace(tag, @"\s+", "")).ToList();
    }
  }
}
