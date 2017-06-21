using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  [Collection("SocialMedia")]

  public class StatusTest : IDisposable
  {
    public StatusTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=social_media_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Status_GetAll_DatabaseEmptyOnload()
    {
      List<Status> testList = Status.GetAll();

      List<Status> controlList = new List<Status>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Status_Save_SavesStatusToDatabase()
    {
      Status controlStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      controlStatus.Save();

      Status testStatus = Status.GetAll()[0];

      Assert.Equal(controlStatus, testStatus);
    }

    [Fact]
    public void Status_Equals_ObjectsEqualsIdenticalObject()
    {
      Status post1 = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      Status post2 = new Status("Hello world", 1, new DateTime(2017, 06, 19));

      Assert.Equal(post1, post2);
    }

    [Fact]
    public void Status_Find_FindsStatusById()
    {
      Status controlStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      controlStatus.Save();

      Status testStatus = Status.Find(controlStatus.Id);

      Assert.Equal(controlStatus, testStatus);
    }

    [Fact]
    public void Status_Delete_DeletesIndividualStatus()
    {
      Status post1 = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      post1.Save();
      Status post2 = new Status("Goodbye world", 1, new DateTime(2017, 06, 20));
      post2.Save();

      post2.Delete();

      List<Status> testList = Status.GetAll();
      List<Status> controlList = new List<Status>{post1};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Status_GetComments_ReturnsStatussComments()
    {
      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();
      Comment comment1 = new Comment("Hallo Wereld", newStatus.Id, 1, new DateTime(2017, 06, 19));
      comment1.Save();
      Comment comment2 = new Comment("Hola Mundo", newStatus.Id, 1, new DateTime(2017, 06, 19));
      comment2.Save();

      List<Comment> testList = newStatus.GetComments();
      List<Comment> controlList = new List<Comment>{comment1, comment2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Status_Update_UpdatesStatusContent()
    {
      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      newStatus.Update("Goodbye world");

      Assert.Equal("Goodbye world", newStatus.Content);
    }

    [Fact]
    public void Status_GetUsersWhoLike_ReturnsListOfUsers()
    {
      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user1.LikeStatus(newStatus);
      user2.LikeStatus(newStatus);

      List<User> testList = newStatus.GetUsersWhoLike();
      List<User> controlList = new List<User>{user1, user2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Status_GetUsersWhoDislike_ReturnsListOfUsers()
    {
      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user1.DislikeStatus(newStatus);
      user2.DislikeStatus(newStatus);

      List<User> testList = newStatus.GetUsersWhoDislike();
      List<User> controlList = new List<User>{user1, user2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Status_GetPosterName()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();

      Status newStatus = new Status("Hello world", user1.Id, new DateTime(2017, 06, 19));
      newStatus.Save();

      Assert.Equal("Joshua Fairchild", newStatus.GetPosterName());
    }

    public void Dispose()
    {
      Status.DeleteAll();
      User.DeleteAll();
    }
  }
}
