using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  public class Post
  {
    public int Id {get;set;}
    public string Content {get;set;}
    public int UserId {get;set;}
    public int Likes {get;set;}
    public int Dislikes {get;set;}
    public DateTime Timestamp {get;set;}

    public Post()
    {
      Id = 0;
      Content = null;
      UserId = 0;
      Likes = 0;
      Dislikes = 0;
      Timestamp = default(DateTime);
    }

    public Post(string content, int userId, DateTime timestamp, int likes = 0, int dislikes = 0, int id = 0)
    {
      Id = id;
      Content = content;
      UserId = userId;
      Likes = likes;
      Dislikes = dislikes;
      Timestamp = timestamp;
    }

    public override bool Equals(System.Object otherPost)
    {
      if(!(otherPost is Post))
      {
        return false;
      }
      else
      {
        Post newPost = (Post) otherPost;
        bool idEquality = this.Id == newPost.Id;
        bool contentEquality = this.Content == newPost.Content;
        bool userIdEquality = this.UserId == newPost.UserId;
        bool likesEquality = this.Likes == newPost.Likes;
        bool dislikesEquality = this.Dislikes == newPost.Dislikes;
        bool timestampEquality = this.Timestamp == newPost.Timestamp;
        return (idEquality && contentEquality && userIdEquality && likesEquality && dislikesEquality && timestampEquality);
      }
    }

    public static List<Post> GetAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM posts", conn);

      SqlDataReader rdr = cmd.ExecuteReader();

      List<Post> allPosts = new List<Post>{};

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string content = rdr.GetString(1);
        int userId = rdr.GetInt32(2);
        int likes = rdr.GetInt32(3);
        int dislikes = rdr.GetInt32(4);
        DateTime timestamp = rdr.GetDateTime(5);
        Post newPost = new Post(content, userId, timestamp, likes, dislikes, id);
        allPosts.Add(newPost);
      }

      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return allPosts;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO posts (content, user_id, likes, dislikes, timestamp) OUTPUT INSERTED.id VALUES (@Content, @UserId, @Likes, @Dislikes, @Timestamp);", conn);
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

    public static Post Find(int idToFind)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM posts WHERE id = @PostId;", conn);
      cmd.Parameters.Add(new SqlParameter("@PostId", idToFind));

      SqlDataReader rdr = cmd.ExecuteReader();

      Post foundPost = new Post();
      while(rdr.Read())
      {
        foundPost.Id = rdr.GetInt32(0);
        foundPost.Content = rdr.GetString(1);
        foundPost.UserId = rdr.GetInt32(2);
        foundPost.Likes = rdr.GetInt32(3);
        foundPost.Dislikes = rdr.GetInt32(4);
        foundPost.Timestamp = rdr.GetDateTime(5);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }

      return foundPost;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM posts", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
