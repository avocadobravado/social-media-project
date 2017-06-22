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
    public string ImgURL {get;set;}

    public User()
    {
      Id = 0;
      FirstName = null;
      LastName = null;
      Username = null;
      Password = null;
      Email = null;
      Timestamp = default(DateTime);
      ImgURL = "https://github.com/avocadobravado/social-media-project-inspiration/blob/master/default-avatar.png?raw=true";
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
      ImgURL = "https://github.com/avocadobravado/social-media-project-inspiration/blob/master/default-avatar.png?raw=true";
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
        bool imgEquality = this.ImgURL == newUser.ImgURL;
        return (idEquality && firstNameEquality && lastNameEquality && usernameEquality && passwordEquality && emailEquality && timestampEquality && imgEquality);
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
        newUser.ImgURL = rdr.GetString(7);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO users (first_name, last_name, username, password, email, timestamp, img_url) OUTPUT INSERTED.id VALUES (@FirstName, @LastName, @Username, @Password, @Email, @Timestamp, @ImgURL);", conn);
      cmd.Parameters.Add(new SqlParameter("@FirstName", this.FirstName));
      cmd.Parameters.Add(new SqlParameter("@LastName", this.LastName));
      cmd.Parameters.Add(new SqlParameter("@Username", this.Username));
      cmd.Parameters.Add(new SqlParameter("@Password", this.Password));
      cmd.Parameters.Add(new SqlParameter("@Email", this.Email));
      cmd.Parameters.Add(new SqlParameter("@Timestamp", this.Timestamp));
      cmd.Parameters.Add(new SqlParameter("@ImgURL", this.ImgURL));

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
        foundUser.ImgURL = rdr.GetString(7);
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

    public List<Status> GetStatuses()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM statuses WHERE user_id = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Status> statuses = new List<Status>{};

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string content = rdr.GetString(1);
        int userId = rdr.GetInt32(2);
        int likes = rdr.GetInt32(3);
        int dislikes = rdr.GetInt32(4);
        DateTime timestamp = rdr.GetDateTime(5);
        Status newStatus = new Status(content, userId, timestamp, likes, dislikes, id);
        statuses.Add(newStatus);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      statuses.Sort((status1, status2) => DateTime.Compare(status1.Timestamp, status2.Timestamp));
      statuses.Reverse();

      return statuses;
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
        newUser.ImgURL = rdr.GetString(7);
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
        newUser.ImgURL = rdr2.GetString(7);
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
        newUser.ImgURL = rdr.GetString(7);
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

    public void LikeStatus(Status statusToLike)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO status_likes (status_id, user_id) VALUES (@StatusId, @UserId);",conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", statusToLike.Id));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }

      statusToLike.Like();
    }

    public void DislikeStatus(Status statusToDislike)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO status_dislikes (status_id, user_id) VALUES (@StatusId, @UserId);",conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", statusToDislike.Id));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }

      statusToDislike.Dislike();
    }

    public void LikeComment(Comment commentToLike)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO comment_likes (comment_id, user_id) VALUES (@CommentId, @UserId);",conn);
      cmd.Parameters.Add(new SqlParameter("@CommentId", commentToLike.Id));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }

      commentToLike.Like();
    }

    public void DislikeComment(Comment commentToDislike)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO comment_dislikes (comment_id, user_id) VALUES (@CommentId, @UserId);",conn);
      cmd.Parameters.Add(new SqlParameter("@CommentId", commentToDislike.Id));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }

      commentToDislike.Dislike();
    }

    public bool HasLikedStatus(Status statusToCheck)
    {
      bool result = false;
      List<User> usersWhoLiked = statusToCheck.GetUsersWhoLike();
      foreach(User user in usersWhoLiked)
      {
        if(user.Username == this.Username)
        {
          result = true;
        }
      }
      return result;
    }

    public bool HasDislikedStatus(Status statusToCheck)
    {
      bool result = false;
      List<User> usersWhoLiked = statusToCheck.GetUsersWhoDislike();
      foreach(User user in usersWhoLiked)
      {
        if(user.Username == this.Username)
        {
          result = true;
        }
      }
      return result;
    }

    public bool HasLikedComment(Comment commentToCheck)
    {
      bool result = false;
      List<User> usersWhoLiked = commentToCheck.GetUsersWhoLike();
      foreach(User user in usersWhoLiked)
      {
        if(user.Username == this.Username)
        {
          result = true;
        }
      }
      return result;
    }

    public bool HasDislikedComment(Comment commentToCheck)
    {
      bool result = false;
      List<User> usersWhoLiked = commentToCheck.GetUsersWhoDislike();
      foreach(User user in usersWhoLiked)
      {
        if(user.Username == this.Username)
        {
          result = true;
        }
      }
      return result;
    }

    public List<Status> GetTimeline()
    {
      List<User> friends = this.GetFriends();
      List<Status> myStatuses = this.GetStatuses();
      List<Status> timeline = new List<Status>{};
      foreach(var friend in friends)
      {
        List<Status> statuses = friend.GetStatuses();
        foreach(var status in statuses)
        {
          timeline.Add(status);
        }
      }
      foreach(var status in myStatuses)
      {
        timeline.Add(status);
      }

      timeline.Sort((status1, status2) => DateTime.Compare(status1.Timestamp, status2.Timestamp));
      timeline.Reverse();

      return timeline;
    }

    public static bool AccountExists(string usernameToCheck)
    {
      bool result = false;
      List<User> allUsers = User.GetAll();
      foreach(User user in allUsers)
      {
        if(user.Username == usernameToCheck)
        {
          result = true;
        }
      }
      return result;
    }

    public static User LookupByUsername(string searchQuery)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM users WHERE username = @SearchQuery", conn);
      cmd.Parameters.Add(new SqlParameter("@SearchQuery", searchQuery));

      SqlDataReader rdr = cmd.ExecuteReader();
      User newUser = new User();
      while(rdr.Read())
      {
        newUser.Id = rdr.GetInt32(0);
        newUser.FirstName = rdr.GetString(1);
        newUser.LastName = rdr.GetString(2);
        newUser.Username = rdr.GetString(3);
        newUser.Password = rdr.GetString(4);
        newUser.Email = rdr.GetString(5);
        newUser.Timestamp = rdr.GetDateTime(6);
        newUser.ImgURL = rdr.GetString(7);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return newUser;
    }

    public void SaveImg(string imgURL)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE users SET img_url = @URL OUTPUT INSERTED.img_url WHERE id = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@URL", imgURL));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.ImgURL = rdr.GetString(0);
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

    public string GetImg()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT img_url FROM users WHERE @UserId = @UserId;", conn);
      cmd.Parameters.Add(new SqlParameter("@UserId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      string url = null;
      while(rdr.Read())
      {
        url = rdr.GetString(0);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return url;
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

      SqlCommand cmd = new SqlCommand("DELETE FROM users WHERE id = @UserId; DELETE FROM comments WHERE user_id = @UserId; DELETE FROM statuses WHERE user_id = @UserId; DELETE FROM user_friendships WHERE user1_id = @UserId; DELETE FROM user_friendships WHERE user2_id = @UserId", conn);
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

      SqlCommand cmd = new SqlCommand("DELETE FROM users; DELETE FROM comments; DELETE FROM statuses; DELETE FROM user_friendships;", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
