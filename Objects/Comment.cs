using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  public class Comment
  {
    public int Id {get;set;}
    public string Content {get;set;}
    public int StatusId {get;set;}
    public int UserId {get;set;}
    public int Likes {get;set;}
    public int Dislikes {get;set;}
    public DateTime Timestamp {get;set;}

    public void Like()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE comments SET likes = @Likes OUTPUT INSERTED.likes WHERE id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@Likes", this.Likes + 1));
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Likes = rdr.GetInt32(0);
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

    public void Dislike()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE comments SET dislikes = @Likes OUTPUT INSERTED.dislikes WHERE id = @StatusId;", conn);
      cmd.Parameters.Add(new SqlParameter("@Likes", this.Dislikes + 1));
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.Id));

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this.Dislikes = rdr.GetInt32(0);
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

    public Comment()
    {
      Id = 0;
      Content = null;
      StatusId = 0;
      UserId = 0;
      Likes = 0;
      Dislikes = 0;
      Timestamp = default(DateTime);
    }

    public Comment(string content, int statusId, int userId, DateTime timestamp, int likes = 0, int dislikes = 0, int id = 0)
    {
      Id = id;
      Content = content;
      StatusId = statusId;
      UserId = userId;
      Likes = likes;
      Dislikes = dislikes;
      Timestamp = timestamp;
    }

    public override bool Equals(System.Object otherComment)
    {
      if(!(otherComment is Comment))
      {
        return false;
      }
      else
      {
        Comment newComment = (Comment) otherComment;
        bool idEquality = this.Id == newComment.Id;
        bool contentEquality = this.Content == newComment.Content;
        bool statusIdEquality = this.StatusId == newComment.StatusId;
        bool userIdEquality = this.UserId == newComment.UserId;
        bool likesEquality = this.Likes == newComment.Likes;
        bool dislikesEquality = this.Dislikes == newComment.Dislikes;
        bool timestampEquality = this.Timestamp == newComment.Timestamp;
        return (idEquality && contentEquality && statusIdEquality && userIdEquality && likesEquality && dislikesEquality && timestampEquality);
      }
    }

    public static List<Comment> GetAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM comments", conn);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Comment> allComments = new List<Comment>{};

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
        allComments.Add(newComment);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return allComments;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO comments (content, status_id, user_id, likes, dislikes, timestamp) OUTPUT INSERTED.id VALUES (@Content, @StatusId, @UserId, @Likes, @Dislikes, @Timestamp);", conn);
      cmd.Parameters.Add(new SqlParameter("@Content", this.Content));
      cmd.Parameters.Add(new SqlParameter("@StatusId", this.StatusId));
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

    public static Comment Find(int idToFind)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM comments WHERE id = @CommentId;", conn);
      cmd.Parameters.Add(new SqlParameter("@CommentId", idToFind));

      SqlDataReader rdr = cmd.ExecuteReader();

      Comment foundComment = new Comment();
      while(rdr.Read())
      {
        foundComment.Id = rdr.GetInt32(0);
        foundComment.Content = rdr.GetString(1);
        foundComment.StatusId = rdr.GetInt32(2);
        foundComment.UserId = rdr.GetInt32(3);
        foundComment.Likes = rdr.GetInt32(4);
        foundComment.Dislikes = rdr.GetInt32(5);
        foundComment.Timestamp = rdr.GetDateTime(6);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return foundComment;
    }

    public void Update(string newContent)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE comments SET content = @Content OUTPUT INSERTED.content WHERE id = @CommentId;", conn);
      cmd.Parameters.Add(new SqlParameter("@Content", newContent));
      cmd.Parameters.Add(new SqlParameter("@CommentId", this.Id));

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

      SqlCommand cmd = new SqlCommand("SELECT users.* FROM comments JOIN comment_likes ON (comments.id = comment_likes.comment_id) JOIN users ON (users.id = comment_likes.user_id) WHERE comment_id = @CommentId;", conn);
      cmd.Parameters.Add(new SqlParameter("@CommentId", this.Id));

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

      SqlCommand cmd = new SqlCommand("SELECT users.* FROM comments JOIN comment_dislikes ON (comments.id = comment_dislikes.comment_id) JOIN users ON (users.id = comment_dislikes.user_id) WHERE comment_id = @CommentId;", conn);
      cmd.Parameters.Add(new SqlParameter("@CommentId", this.Id));

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

    public string GetCommenterName()
    {
      User whoPosted = User.Find(this.UserId);
      string result = $"{whoPosted.FirstName} {whoPosted.LastName}";
      return result;
    }

    public string GetCommenterImg()
    {
      User whoPosted = User.Find(this.UserId);
      string result = whoPosted.ImgURL;
      return result;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM comments WHERE id = @CommentId;", conn);
      cmd.Parameters.Add(new SqlParameter("@CommentId", this.Id));

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

      SqlCommand cmd = new SqlCommand("DELETE FROM comments;", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
