using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  public class Status
  {
    public int Id {get;set;}
    public string Content {get;set;}
    public int UserId {get;set;}
    public int Likes {get;set;}
    public int Dislikes {get;set;}
    public DateTime Timestamp {get;set;}

    public void Like()
    {
      this.Likes++;
    }

    public void Dislike()
    {
      this.Dislikes++;
    }

    public Status()
    {
      Id = 0;
      Content = null;
      UserId = 0;
      Likes = 0;
      Dislikes = 0;
      Timestamp = default(DateTime);
    }

    public Status(string content, int userId, DateTime timestamp, int likes = 0, int dislikes = 0, int id = 0)
    {
      Id = id;
      Content = content;
      UserId = userId;
      Likes = likes;
      Dislikes = dislikes;
      Timestamp = timestamp;
    }

    public override bool Equals(System.Object otherStatus)
    {
      if(!(otherStatus is Status))
      {
        return false;
      }
      else
      {
        Status newStatus = (Status) otherStatus;
        bool idEquality = this.Id == newStatus.Id;
        bool contentEquality = this.Content == newStatus.Content;
        bool userIdEquality = this.UserId == newStatus.UserId;
        bool likesEquality = this.Likes == newStatus.Likes;
        bool dislikesEquality = this.Dislikes == newStatus.Dislikes;
        bool timestampEquality = this.Timestamp == newStatus.Timestamp;
        return (idEquality && contentEquality && userIdEquality && likesEquality && dislikesEquality && timestampEquality);
      }
    }

    public static List<Status> GetAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM statuses", conn);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Status> allStatuses = new List<Status>{};

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string content = rdr.GetString(1);
        int userId = rdr.GetInt32(2);
        int likes = rdr.GetInt32(3);
        int dislikes = rdr.GetInt32(4);
        DateTime timestamp = rdr.GetDateTime(5);
        Status newStatus = new Status(content, userId, timestamp, likes, dislikes, id);
        allStatuses.Add(newStatus);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return allStatuses;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO statuses (content, user_id, likes, dislikes, timestamp) OUTPUT INSERTED.id VALUES (@Content, @UserId, @Likes, @Dislikes, @Timestamp);", conn);
      cmd.Parameters.Add(new SqlParameter("@Content", this.Content));
      cmd.Parameters.Add(new SqlParameter("@UserId", this.UserId));
      cmd.Parameters.Add(new SqlParameter("@Likes", this.Likes));
      cmd.Parameters.Add(new SqlParameter("@Dislikes", this.Dislikes));
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

    public static Status Find(int idToFind)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM statuses WHERE id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", idToFind));

      SqlDataReader rdr = cmd.ExecuteReader();

      Status foundStatus = new Status();
      while(rdr.Read())
      {
        foundStatus.Id = rdr.GetInt32(0);
        foundStatus.Content = rdr.GetString(1);
        foundStatus.UserId = rdr.GetInt32(2);
        foundStatus.Likes = rdr.GetInt32(3);
        foundStatus.Dislikes = rdr.GetInt32(4);
        foundStatus.Timestamp = rdr.GetDateTime(5);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return foundStatus;
    }

    public List<Comment> GetComments()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM comments WHERE status_id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Comment> comments = new List<Comment>{};
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string content = rdr.GetString(1);
        int statusId = rdr.GetInt32(2);
        int userId = rdr.GetInt32(3);
        int likes = rdr.GetInt32(4);
        int dislikes = rdr.GetInt32(5);
        DateTime timestamp = rdr.GetDateTime(6);
        Comment newComment = new Comment(content, statusId, userId, timestamp, likes, dislikes, id);
        comments.Add(newComment);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return comments;
    }

    public void Update(string newContent)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE statuses SET content = @Content OUTPUT INSERTED.content WHERE id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@Content", newContent));
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Content = rdr.GetString(0);
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

    public List<User> GetUsersWhoLike()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT users.* FROM statuses JOIN status_likes ON (statuses.id = status_likes.status_id) JOIN users ON (users.id = status_likes.user_id) WHERE status_id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<User> users = new List<User>{};
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
        users.Add(newUser);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return users;
    }

    public List<User> GetUsersWhoDislike()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT users.* FROM statuses JOIN status_dislikes ON (statuses.id = status_dislikes.status_id) JOIN users ON (users.id = status_dislikes.user_id) WHERE status_id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<User> users = new List<User>{};
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
        users.Add(newUser);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return users;
    }

    public string GetPosterName()
    {
      User whoPosted = User.Find(this.UserId);
      string result = $"{whoPosted.FirstName} {whoPosted.LastName}";
      return result;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM statuses WHERE id = @StatusId; DELETE FROM comments WHERE status_id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

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

      SqlCommand cmd = new SqlCommand("DELETE FROM statuses; DELETE FROM comments;", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
