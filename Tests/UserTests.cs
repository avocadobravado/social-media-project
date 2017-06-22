using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  [Collection("SocialMedia")]

  public class UserTest : IDisposable
  {
    public UserTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=social_media_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void User_GetAll_DatabaseEmptyOnload()
    {
      List<User> testList = User.GetAll();

      List<User> controlList = new List<User>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_Save_SavesUserToDatabase()
    {
      User controlUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      controlUser.Save();

      User testUser = User.GetAll()[0];

      Assert.Equal(controlUser, testUser);
    }

    [Fact]
    public void User_Equals_ObjectsEqualsIdenticalObject()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      User user2 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));

      Assert.Equal(user1, user2);
    }

    [Fact]
    public void User_Find_FindsUserById()
    {
      User controlUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      controlUser.Save();

      User testUser = User.Find(controlUser.Id);

      Assert.Equal(controlUser, testUser);
    }

    [Fact]
    public void User_Delete_DeletesIndividualUser()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Jennifer", "Fairchild", "jenfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user2.Delete();

      List<User> testList = User.GetAll();
      List<User> controlList = new List<User>{user1};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_UsernameTaken_ReturnsFalse()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();

      User user2 = new User("Joshua", "Fairchild", "fairchild1", "password", "mail@mail.com", new DateTime(2017, 06, 20));

      user2.UsernameTaken();
      Assert.Equal(false, user2.UsernameTaken());
    }

    [Fact]
    public void User_UsernameTaken_ReturnsTrue()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();

      User user2 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 20));

      user2.UsernameTaken();
      Assert.Equal(true, user2.UsernameTaken());
    }

    [Fact]
    public void User_GetStatuses_ReturnsUsersStatuses()
    {
      User newUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      newUser.Save();

      Status status1 = new Status("Hello world", newUser.Id, new DateTime(2017, 06, 19));
      status1.Save();
      Status status2 = new Status("Goodbye world", newUser.Id, new DateTime(2017, 06, 19));
      status2.Save();

      List<Status> testList = newUser.GetStatuses();
      List<Status> controlList = new List<Status>{status2, status1};

      Assert.Equal(testList, controlList);
    }

    [Fact]
    public void User_AddFriend_User1FriendsUser2()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user1.AddFriend(user2);

      List<User> testList = user1.GetFriends();
      List<User> controlList = new List<User>{user2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_AddFriend_User2FriendsUser1()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user2.AddFriend(user1);

      List<User> testList = user2.GetFriends();
      List<User> controlList = new List<User>{user1};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_AddFriend_MultipleFriendsAdd()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();
      User user3 = new User("Tom", "Hanks", "thanks", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user3.Save();

      user1.AddFriend(user2);
      user3.AddFriend(user1);

      List<User> testList = user1.GetFriends();
      List<User> controlList = new List<User>{user2, user3};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_RemoveFriend_RemovesRelationshipInDB()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();
      User user3 = new User("Tom", "Hanks", "thanks", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user3.Save();

      user1.AddFriend(user2);
      user3.AddFriend(user1);
      user1.RemoveFriend(user3);

      List<User> testList = user1.GetFriends();
      List<User> controlList = new List<User>{user2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_IsFriendsWith_ReturnsTrue()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      user2.AddFriend(user1);

      bool testBool = user1.IsFriendsWith(user2);
      bool controlBool = true;

      Assert.Equal(controlBool, testBool);
    }

    [Fact]
    public void User_IsFriendsWith_ReturnsFalse()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();

      bool testBool = user1.IsFriendsWith(user2);
      bool controlBool = false;

      Assert.Equal(controlBool, testBool);
    }

    [Fact]
    public void User_Search_ReturnsIntendedMatches()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user1.Save();
      User user2 = new User("Butch", "Fairchild", "fairjosh", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user2.Save();
      User user3 = new User("josh", "Fairchild", "jfair", "password", "joshua@mail.com", new DateTime(2017, 06, 19));
      user3.Save();
      User user4 = new User("Tom", "Hanks", "thanks", "password", "tom@mail.com", new DateTime(2017, 06, 19));
      user4.Save();
      User user5 = new User("JOSHUA", "Fairchild", "jfair", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      user5.Save();

      List<User> testList = User.Search("josh");
      List<User> controlList = new List<User>{user1, user2, user3, user5};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void User_Update_UpdatesUserInfo()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      testUser.Update("Tom", "Hanks", "thanks", "password", "hanks@mail.com");

      User controlUser = new User("Tom", "Hanks", "thanks", "password", "hanks@mail.com", new DateTime(2017, 06, 19), testUser.Id);

      Assert.Equal(controlUser, testUser);
    }

    [Fact]
    public void User_LikeStatus_LikesStatus()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      testUser.LikeStatus(newStatus);

      Assert.Equal(1, newStatus.Likes);
    }

    [Fact]
    public void User_DislikeStatus_DislikesStatus()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      testUser.DislikeStatus(newStatus);

      Assert.Equal(1, newStatus.Dislikes);
    }

    [Fact]
    public void User_LikeComment_LikesComment()
    {
      User newUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      newUser.Save();

      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      newUser.LikeComment(newComment);

      Assert.Equal(1, newComment.Likes);
    }

    [Fact]
    public void User_DislikeComment_DislikesComment()
    {
      User newUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      newUser.Save();

      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      newUser.DislikeComment(newComment);

      Assert.Equal(1, newComment.Dislikes);
    }

    [Fact]
    public void User_HasLikedStatus_True()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      testUser.LikeStatus(newStatus);

      Assert.Equal(true, testUser.HasLikedStatus(newStatus));
    }

    [Fact]
    public void User_HasLikedStatus_False()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      Assert.Equal(false, testUser.HasLikedStatus(newStatus));
    }

    [Fact]
    public void User_HasDislikedStatus_True()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      testUser.DislikeStatus(newStatus);

      Assert.Equal(true, testUser.HasDislikedStatus(newStatus));
    }

    [Fact]
    public void User_HasDislikedStatus_False()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Status newStatus = new Status("Hello world", 1, new DateTime(2017, 06, 19));
      newStatus.Save();

      Assert.Equal(false, testUser.HasDislikedStatus(newStatus));
    }

    [Fact]
    public void User_HasLikedComment_True()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      testUser.LikeComment(newComment);

      Assert.Equal(true, testUser.HasLikedComment(newComment));
    }

    [Fact]
    public void User_HasLikedComment_False()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      Assert.Equal(false, testUser.HasLikedComment(newComment));
    }

    [Fact]
    public void User_HasDislikedComment_True()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      testUser.DislikeComment(newComment);

      Assert.Equal(true, testUser.HasDislikedComment(newComment));
    }

    [Fact]
    public void User_HasDislikedComment_False()
    {
      User testUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      testUser.Save();

      Comment newComment = new Comment("Hello world", 1, 1, new DateTime(2017, 06, 19));
      newComment.Save();

      Assert.Equal(false, testUser.HasDislikedComment(newComment));
    }

    public void User_GetTimeline_ChronologicalListOfStatuses()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 01));
      user1.Save();
      User user2 = new User("Guy", "Anderson", "ganderson", "password", "mail@mail.com", new DateTime(2017, 06, 01));
      user2.Save();
      User user3 = new User("Tom", "Hanks", "thanks", "password", "mail@mail.com", new DateTime(2017, 06, 01));
      user3.Save();

      Status status1 = new Status("Hello world", user1.Id, new DateTime(2017, 06, 04));
      status1.Save();
      Status status2 = new Status("Hola mundo", user3.Id, new DateTime(2017, 06, 02));
      status2.Save();
      Status status3 = new Status("Hallo wereld", user3.Id, new DateTime(2017, 06, 03));
      status3.Save();
      Status status4 = new Status("Goodbye world", user2.Id, new DateTime(2017, 06, 05));
      status4.Save();

      user1.AddFriend(user3);

      List<Status> testList = user1.GetTimeline();
      List<Status> controlList = new List<Status>{status2, status3, status1};

      Assert.Equal(controlList, testList);
    }

    public void User_AccountExists_True()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 01));
      user1.Save();

      bool testBool = User.AccountExists("jfairchild");

      Assert.Equal(true, testBool);
    }

    [Fact]
    public void User_LookupByUsername_ReturnsUser()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 01));
      user1.Save();

      User foundUser = User.LookupByUsername("jfairchild");

      Assert.Equal(user1, foundUser);
    }

    [Fact]
    public void User_SaveImg_SavesToDatabase()
    {
      User user1 = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 01));
      user1.Save();

      user1.SaveImg("image.png");

      string imageUrl = user1.GetImg();

      Assert.Equal("image.png", imageUrl);
    }

    public void Dispose()
    {
      User.DeleteAll();
    }
  }
}
