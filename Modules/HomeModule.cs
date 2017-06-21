using Nancy;
using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using SocialMedia.Objects;

namespace SocialMedia
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        return View["index.cshtml"];
      };
      Post["/login"] = _ => {                                                   //NOTE IM NOT VERY RESTFUL
        Dictionary <string, object> model = new Dictionary<string, object>{};

        string username = Request.Form["username"];
        string password = Request.Form["password"];
        if(!(User.AccountExists(username)))
        {
          model.Add("bad-login", true);
          return View["index.cshtml", model];
        }
        else
        {
          User loggedInUser = User.LookupByUsername(username);
          if(password != loggedInUser.Password)
          {
            model.Add("bad-password", true);
            return View["index.cshtml", model];
          }
          else
          {
            List<Post> timeline = loggedInUser.GetTimeline();
            model.Add("user", loggedInUser);
            model.Add("timeline", timeline);
            return View["news.cshtml", model];
          }
        }
      };
      Post["/account_created"] = _ => {                                       //NOTE IM NOT VERY RESTFUL
        Dictionary <string, object> model = new Dictionary<string, object>{};
        string firstName = Request.Form["first-name"];
        string lastName = Request.Form["last-name"];
        string username = Request.Form["username"];
        string password = Request.Form["password"];
        string passwordConfirm = Request.Form["password-confirm"];
        string email = Request.Form["email"];
        if(password != passwordConfirm)
        {
          model.Add("password-mismatch", true);
          return View["index.cshtml", model];
        }
        else
        {
          User newUser = new User(firstName, lastName, username, password, email, DateTime.Now);
          if(newUser.UsernameTaken())
          {
            model.Add("username-taken", true);
            return View["index.cshtml", model];
          }
          else
          {
            newUser.Save();
            List<Post> timeline = newUser.GetTimeline();
            model.Add("timeline", timeline);
            model.Add("user", newUser);
            return View["news.cshtml", model];
          }
        }
      };
      // Post["/users/{userId}/posts/{postId}/comment"] = parameters => {
      //   Dictionary <string, object> model = new Dictionary<string, object>{};
      //   User selectedUser = User.Find(parameters.userId);
      //   Post selectedPost = Post.Find(parameters.postId);                       //NOTE UHOH, IM A BIG PROBLEM
      //   Comment newComment = new Comment(Request.Form["comment"], selectedPost.Id, selectedUser.Id, DateTime.Now);
      //   newComment.Save();
      //   List<Post> timeline = selectedUser.GetTimeline();
      //   model.Add("timeline", timeline);
      //   model.Add("user", selectedUser);
      //   return View["news.cshtml", model];
      // };
      Post["/users/{userId}/post"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Post newPost = new Post(Request.Form["post"], selectedUser.Id, DateTime.Now);
        newPost.Save();
        List<Post> timeline = selectedUser.GetTimeline();
        model.Add("timeline", timeline);
        model.Add("user", selectedUser);
        return View["news.cshtml", model];
      };
      Get["/users/{loggedInId}/profile_view/{viewingId}"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        User selectedUser = User.Find(parameters.viewingId);
        List<Post> usersPosts = selectedUser.GetPosts();
        model.Add("selected-user", selectedUser);
        model.Add("user-posts", usersPosts);
        if(loggedInUser.IsFriendsWith(selectedUser))
        {
          //VIEWING FRIENDS PAGE
          return View["friend.cshtml", model];
        }
        else
        {
          //VIEWING A NON FRIEND
          return View["notfriend.cshtml", model];     //NOTE NEED TO HANDLE THE CASE OF LOGGED IN USR VIEWING THEMSELVES
        }
      };
    }
  }
}
