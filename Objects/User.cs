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

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM users", conn);
      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }
  }
}
