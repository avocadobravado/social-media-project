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
  }
}
