using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  public class Comment
  {
    public int Id {get;set;}
    public string Content {get;set;}
    public int PostId {get;set;}
    public int UserId {get;set;}
    public int Likes {get;set;}
    public int Dislikes {get;set;}
    public DateTime Timestamp {get;set;}

    public Comment()
    {
      Id = 0;
      Content = null;
      PostId = 0;
      UserId = 0;
      Likes = 0;
      Dislikes = 0;
      Timestamp = default(DateTime);
    }

    public Comment(string content, int postId, int userId, DateTime timestamp, int likes = 0, int dislikes = 0, int id = 0)
    {
      Id = id;
      Content = content;
      PostId = postId;
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
        bool postIdEquality = this.PostId == newComment.PostId;
        bool userIdEquality = this.UserId == newComment.UserId;
        bool likesEquality = this.Likes == newComment.Likes;
        bool dislikesEquality = this.Dislikes == newComment.Dislikes;
        bool timestampEquality = this.Timestamp == newComment.Timestamp;
        return (idEquality && contentEquality && postIdEquality && userIdEquality && likesEquality && dislikesEquality && timestampEquality);
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
        int postId = rdr.GetInt32(2);
        int userId = rdr.GetInt32(3);
        int likes = rdr.GetInt32(4);
        int dislikes = rdr.GetInt32(5);
        DateTime timestamp = rdr.GetDateTime(6);
        Comment newComment = new Comment(content, postId, userId, timestamp, likes, dislikes, id);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO comments (content, post_id, user_id, likes, dislikes, timestamp) OUTPUT INSERTED.id VALUES (@Content, @PostId, @UserId, @Likes, @Dislikes, @Timestamp);", conn);
      cmd.Parameters.Add(new SqlParameter("@Content", this.Content));
      cmd.Parameters.Add(new SqlParameter("@PostId", this.PostId));
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
        foundComment.PostId = rdr.GetInt32(2);
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

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM comments", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
