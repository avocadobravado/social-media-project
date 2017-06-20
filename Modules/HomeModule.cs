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
      Post["/login"] = _ => {
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
            return View["news.cshtml"];
          }
        }
      };
      Post["/account_created"] = _ => {
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
    }
  }
}
