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
    public void User_GetPosts_ReturnsUsersPosts()
    {
      User newUser = new User("Joshua", "Fairchild", "jfairchild", "password", "mail@mail.com", new DateTime(2017, 06, 19));
      newUser.Save();

      Post post1 = new Post("Hello world", newUser.Id, new DateTime(2017, 06, 19));
      post1.Save();
      Post post2 = new Post("Goodbye world", newUser.Id, new DateTime(2017, 06, 19));
      post2.Save();

      List<Post> testList = newUser.GetPosts();
      List<Post> controlList = new List<Post>{post1, post2};

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

    public void Dispose()
    {
      User.DeleteAll();
    }
  }
}
