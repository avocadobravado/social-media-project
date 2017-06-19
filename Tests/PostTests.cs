using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SocialMedia.Objects
{
  [Collection("SocialMedia")]

  public class PostTest : IDisposable
  {
    public PostTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=social_media_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Post_GetAll_DatabaseEmptyOnload()
    {
      List<Post> testList = Post.GetAll();

      List<Post> controlList = new List<Post>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Post_Save_SavesPostToDatabase()
    {
      Post controlPost = new Post("Hello world", 1, new DateTime(2017, 06, 19));
      controlPost.Save();

      Post testPost = Post.GetAll()[0];

      Assert.Equal(controlPost, testPost);
    }

    [Fact]
    public void Post_Equals_ObjectsEqualsIdenticalObject()
    {
      Post post1 = new Post("Hello world", 1, new DateTime(2017, 06, 19));
      Post post2 = new Post("Hello world", 1, new DateTime(2017, 06, 19));

      Assert.Equal(post1, post2);
    }

    [Fact]
    public void Post_Find_FindsPostById()
    {
      Post controlPost = new Post("Hello world", 1, new DateTime(2017, 06, 19));
      controlPost.Save();

      Post testPost = Post.Find(controlPost.Id);

      Assert.Equal(controlPost, testPost);
    }

    public void Dispose()
    {
      Post.DeleteAll();
    }
  }
}
