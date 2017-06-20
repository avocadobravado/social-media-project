using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  [Collection("SocialMedia")]

  public class CommentTest : IDisposable
  {
    public CommentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=social_media_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Comment_GetAll_DatabaseEmptyOnload()
    {
      List<Comment> testList = Comment.GetAll();

      List<Comment> controlList = new List<Comment>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Comment_Save_SavesCommentToDatabase()
    {
      Comment controlComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      controlComment.Save();

      Comment testComment = Comment.GetAll()[0];

      Assert.Equal(controlComment, testComment);
    }

    [Fact]
    public void Comment_Equals_ObjectsEqualsIdenticalObject()
    {
      Comment comment1 = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      Comment comment2 = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));

      Assert.Equal(comment1, comment2);
    }

    [Fact]
    public void Comment_Find_FindsCommentById()
    {
      Comment controlComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      controlComment.Save();

      Comment testComment = Comment.Find(controlComment.Id);

      Assert.Equal(controlComment, testComment);
    }

    [Fact]
    public void Comment_Delete_DeletesIndividualComment()
    {
      Comment comment1 = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      comment1.Save();
      Comment comment2 = new Comment("Goodbye world", 1, 1, new DateTime(2017, 06, 20));
      comment2.Save();

      comment2.Delete();

      List<Comment> testList = Comment.GetAll();
      List<Comment> controlList = new List<Comment>{comment1};

      Assert.Equal(controlList, testList);
    }

    public void Dispose()
    {
      Comment.DeleteAll();
    }
  }
}
