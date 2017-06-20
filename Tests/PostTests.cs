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

    [Fact]
    public void Post_Delete_DeletesIndividualPost()
    {
      Post post1 = new Post("Hello world", 1, new DateTime(2017, 06, 19));
      post1.Save();
      Post post2 = new Post("Goodbye world", 1, new DateTime(2017, 06, 20));
      post2.Save();

      post2.Delete();

      List<Post> testList = Post.GetAll();
      List<Post> controlList = new List<Post>{post1};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Post_GetComments_ReturnsPostsComments()
    {
      Post newPost = new Post("Hello world", 1, new DateTime(2017, 06, 19));
      newPost.Save();
      Comment comment1 = new Comment("Hallo Wereld", newPost.Id, 1, new DateTime(2017, 06, 19));
      comment1.Save();
      Comment comment2 = new Comment("Hola Mundo", newPost.Id, 1, new DateTime(2017, 06, 19));
      comment2.Save();

      List<Comment> testList = newPost.GetComments();
      List<Comment> controlList = new List<Comment>{comment1, comment2};

      Assert.Equal(controlList, testList);
    }

    public void Dispose()
    {
      Post.DeleteAll();
    }
  }
}
