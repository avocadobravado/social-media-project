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

    [Fact]
    public void Comment_Update_UpdatesCommentContent()
    {
      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      newComment.Update("Goodbye world");

      Assert.Equal("Goodbye world", newComment.Content);
    }

    [Fact]
    public void Comment_GetUsersWhoLike_ReturnsListOfUsers()
    {
      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user1.LikeComment(newComment);
      user2.LikeComment(newComment);

      List<User> testList = newComment.GetUsersWhoLike();
      List<User> controlList = new List<User>{user1, user2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Comment_GetUsersWhoDislike_ReturnsListOfUsers()
    {
      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user1.DislikeComment(newComment);
      user2.DislikeComment(newComment);

      List<User> testList = newComment.GetUsersWhoDislike();
      List<User> controlList = new List<User>{user1, user2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Comment_GetCommenterName_ReturnsUserName()
    {
      User newUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      newUser.Save();

      Status newStatus = new Status("Hello world", newUser.Id, new DateTime(2017, 06, 19));
      newStatus.Save();

      Comment newComment = new Comment("Hello world", newStatus.Id, newUser.Id, new DateTime(2017, 06, 19));
      newComment.Save();

      Assert.Equal("Joshua Fairchild", newComment.GetCommenterName());
    }

    public void Dispose()
    {
      Comment.DeleteAll();
      User.DeleteAll();
    }
  }
}
