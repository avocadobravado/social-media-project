using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  public class User
  {
    public int Id {get;set;}
    public string FirstName {get;set;}
    public string LastName {get;set;}
    public string Username {get;set;}
    public string Password {get;set;}
    public string Email {get;set;}
    public DateTime Timestamp {get;set;}

    public User()
    {
      Id = 0;
      FirstName = null;
      LastName = null;
      Username = null;
      Password = null;
      Email = null;
      Timestamp = default(DateTime);
    }

    public User(string firstName, string lastName, string username, string password, string email, DateTime timestamp, int id = 0)
    {
      Id = id;
      FirstName = firstName;
      LastName = lastName;
      Username = username;
      Password = password;
      Email = email;
      Timestamp = timestamp;
    }

    public override bool Equals(System.Object otherUser)
    {
      if(!(otherUser is User))
      {
        return false;
      }
      else
      {
        User newUser = (User) otherUser;
        bool idEquality = this.Id == newUser.Id;
        bool firstNameEquality = this.FirstName == newUser.FirstName;
        bool lastNameEquality = this.LastName == newUser.LastName;
        bool usernameEquality = this.Username == newUser.Username;
        bool passwordEquality = this.Password == newUser.Password;
        bool emailEquality = this.Email == newUser.Email;
        bool timestampEquality = this.Timestamp == newUser.Timestamp;
        return (idEquality && firstNameEquality && lastNameEquality && usernameEquality && passwordEquality && emailEquality && timestampEquality);
      }
    }

    public static List<User> GetAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM users", conn);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<User> allUsers = new List<User>{};

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string firstName = rdr.GetString(1);
        string lastName = rdr.GetString(2);
        string username = rdr.GetString(3);
        string password = rdr.GetString(4);
        string email = rdr.GetString(5);
        DateTime timestamp = rdr.GetDateTime(6);
        User newUser = new User(firstName, lastName, username, password, email, timestamp, id);
        allUsers.Add(newUser);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return allUsers;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO users (first_name, last_name, username, password, email, timestamp) OUTPUT INSERTED.id VALUES (@FirstName, @LastName, @Username, @Password, @Email, @Timestamp);", conn);
      cmd.Parameters.Add(new SqlParameter("@FirstName", this.FirstName));
      cmd.Parameters.Add(new SqlParameter("@LastName", this.LastName));
      cmd.Parameters.Add(new SqlParameter("@Username", this.Username));
      cmd.Parameters.Add(new SqlParameter("@Password", this.Password));
      cmd.Parameters.Add(new SqlParameter("@Email", this.Email));
      cmd.Parameters.Add(new SqlParameter("@Timestamp", this.Timestamp));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static User Find(int idToFInd)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE id = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@UserId", idToFInd));

      SqlDataReader rdr = cmd.ExecuteReader();

      User foundUser = new User();
      while(rdr.Read())
      {
        foundUser.Id = rdr.GetInt32(0);
        foundUser.FirstName = rdr.GetString(1);
        foundUser.LastName = rdr.GetString(2);
        foundUser.Username = rdr.GetString(3);
        foundUser.Password = rdr.GetString(4);
        foundUser.Email = rdr.GetString(5);
        foundUser.Timestamp = rdr.GetDateTime(6);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return foundUser;
    }

    public bool UsernameTaken()
    {
      List<User> allUsers = User.GetAll();
      bool isTaken = false;

      foreach(User user in allUsers)
      {
        if(this.Username == user.Username)
        {
          isTaken = true;
        }
      }

      return isTaken;
    }

    public List<Post> GetPosts()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM posts WHERE user_id = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Post> posts = new List<Post>{};

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string content = rdr.GetString(1);
        int userId = rdr.GetInt32(2);
        int likes = rdr.GetInt32(3);
        int dislikes = rdr.GetInt32(4);
        DateTime timestamp = rdr.GetDateTime(5);
        Post newPost = new Post(content, userId, timestamp, likes, dislikes, id);
        posts.Add(newPost);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return posts;
    }

    public void AddFriend(User userToAdd)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO user_friendships (user1_id, user2_id, timestamp) VALUES (@User1Id, @User2Id, @Timestamp);", conn);
      cmd.Parameters.Add(new SqlParameter("@User1Id", this.Id));
      cmd.Parameters.Add(new SqlParameter("@User2Id", userToAdd.Id));
      cmd.Parameters.Add(new SqlParameter("@Timestamp", DateTime.Now));

      cmd.ExecuteNonQuery();
      if(conn != null)
      {
        conn.Close();
      }
    }

    public List<User> GetFriends()
    {
      List<User> friends = new List<User>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT users.* FROM users JOIN user_friendships ON (users.id = user_friendships.user2_id) WHERE user1_id = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string firstName = rdr.GetString(1);
        string lastName = rdr.GetString(2);
        string username = rdr.GetString(3);
        string password = rdr.GetString(4);
        string email = rdr.GetString(5);
        DateTime timestamp = rdr.GetDateTime(6);
        User newUser = new User(firstName, lastName, username, password, email, timestamp, id);
        friends.Add(newUser);
      }

      if(rdr != null)
      {
        rdr.Close();
      }

      SqlCommand cmd2 = new SqlCommand("SELECT users.* FROM users JOIN user_friendships ON (users.id = user_friendships.user1_id) WHERE user2_id = @UserId;", conn);
      cmd2.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr2 = cmd2.ExecuteReader();

      while(rdr2.Read())
      {
        int id = rdr2.GetInt32(0);
        string firstName = rdr2.GetString(1);
        string lastName = rdr2.GetString(2);
        string username = rdr2.GetString(3);
        string password = rdr2.GetString(4);
        string email = rdr2.GetString(5);
        DateTime timestamp = rdr2.GetDateTime(6);
        User newUser = new User(firstName, lastName, username, password, email, timestamp, id);
        friends.Add(newUser);
      }

      if(rdr2 != null)
      {
        rdr2.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return friends;
    }

    public bool IsFriendsWith(User userToCheck)
    {
      bool result = false;
      List<User> userFriends = this.GetFriends();
      foreach(User friend in userFriends)
      {
        if(friend.Username == userToCheck.Username)
        {
          result = true;
        }
      }
      return result;
    }

    public static List<User> Search(string searchQuery)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE first_name LIKE @SearchQuery OR last_name LIKE @SearchQuery OR username LIKE @SearchQuery OR email LIKE @SearchQuery", conn);
      cmd.Parameters.Add(new SqlParameter("@SearchQuery", $"%{searchQuery}%"));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<User> matches = new List<User>{};

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string firstName = rdr.GetString(1);
        string lastName = rdr.GetString(2);
        string username = rdr.GetString(3);
        string password = rdr.GetString(4);
        string email = rdr.GetString(5);
        DateTime timestamp = rdr.GetDateTime(6);
        User newUser = new User(firstName, lastName, username, password, email, timestamp, id);
        matches.Add(newUser);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return matches;
    }

    public void Update(string firstName, string lastName, string username, string password, string email)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE users SET first_name = @FirstName, last_name = @LastName, username = @Username, password = @Password, email = @Email OUTPUT INSERTED.first_name, INSERTED.last_name, INSERTED.username, INSERTED.password, INSERTED.email WHERE id = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@FirstName", firstName));
      cmd.Parameters.Add(new SqlParameter("@LastName", lastName));
      cmd.Parameters.Add(new SqlParameter("@Username", username));
      cmd.Parameters.Add(new SqlParameter("@Password", password));
      cmd.Parameters.Add(new SqlParameter("@Email", email));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.FirstName = rdr.GetString(0);
        this.LastName = rdr.GetString(1);
        this.Username = rdr.GetString(2);
        this.Password = rdr.GetString(3);
        this.Email = rdr.GetString(4);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public void LikePost(Post postToLike)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO post_likes (post_id, user_id) VALUES (@PostId, @UserId);",conn);
      cmd.Parameters.Add(new SqlParameter("@PostId", postToLike.Id));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }

      postToLike.Like();
    }

    public void DislikePost(Post postToDislike)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO post_dislikes (post_id, user_id) VALUES (@PostId, @UserId);",conn);
      cmd.Parameters.Add(new SqlParameter("@PostId", postToDislike.Id));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }

      postToDislike.Dislike();
    }

    public void RemoveFriend(User userToRemove)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM user_friendships WHERE user1_id = @ThisUserId AND user2_id = @UserToRemoveId; DELETE FROM user_friendships WHERE user2_id = @ThisUserId AND user1_id = @UserToRemoveId;", conn);
      cmd.Parameters.Add(new SqlParameter("@ThisUserId", this.Id));
      cmd.Parameters.Add(new SqlParameter("@UserToRemoveId", userToRemove.Id));
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM users WHERE id = @UserId; DELETE FROM comments WHERE user_id = @UserId; DELETE FROM posts WHERE user_id = @UserId; DELETE FROM user_friendships WHERE user1_id = @UserId; DELETE FROM user_friendships WHERE user2_id = @UserId", conn);
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM users; DELETE FROM comments; DELETE FROM posts; DELETE FROM user_friendships;", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
