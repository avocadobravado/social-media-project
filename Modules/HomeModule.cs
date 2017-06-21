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
            List<Status> timeline = loggedInUser.GetTimeline();
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
            List<Status> timeline = newUser.GetTimeline();
            model.Add("timeline", timeline);
            model.Add("user", newUser);
            return View["news.cshtml", model];
          }
        }
      };
      Post["/users/{userId}/posts/{postId}/comment"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Status selectedStatus = Status.Find(parameters.postId);
        Comment newComment = new Comment(Request.Form["comment"], selectedStatus.Id, selectedUser.Id, DateTime.Now);
        newComment.Save();
        List<Status> timeline = selectedUser.GetTimeline();
        model.Add("timeline", timeline);
        model.Add("user", selectedUser);
        return View["news.cshtml", model];
      };
      Post["/users/{userId}/post"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Status newStatus = new Status(Request.Form["post"], selectedUser.Id, DateTime.Now);
        newStatus.Save();
        List<Status> timeline = selectedUser.GetTimeline();
        model.Add("timeline", timeline);
        model.Add("user", selectedUser);
        return View["news.cshtml", model];
      };
      Get["/users/{loggedInId}/profile_view/{viewingId}"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        User selectedUser = User.Find(parameters.viewingId);
        List<Status> userStatuses = selectedUser.GetStatuses();
        if(loggedInUser.IsFriendsWith(selectedUser))
        {
          //VIEWING FRIENDS PAGE
          model.Add("user", loggedInUser);
          model.Add("selected-user", selectedUser);
          model.Add("user-statuses", userStatuses);
          return View["friend.cshtml", model];
        }
        else if(loggedInUser.Username == selectedUser.Username)
        {
          //VIEWING OWN PAGE
          model.Add("user", loggedInUser);
          model.Add("user-statuses", loggedInUser.GetStatuses());
          return View["profile.cshtml", model];
        }
        else
        {
          //VIEWING A NON FRIEND
          model.Add("user", loggedInUser);
          model.Add("selected-user", selectedUser);
          model.Add("user-statuses", userStatuses);
          return View["notfriend.cshtml", model];
        }
      };
      Post["/users/{userId}/statuses/new"]= parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Status newStatus = new Status(Request.Form["content"], selectedUser.Id, DateTime.Now);
        newStatus.Save();
        List<Status> selectedUserStatuses = selectedUser.GetStatuses();
        List<User> friendsList = selectedUser.GetFriends();
        model.Add("user", selectedUser);
        model.Add("user-statuses", selectedUserStatuses);
        model.Add("friends", friendsList);
        return View["profile.cshtml", model];
      };
      Post["/users/{userId}/statuses/{statusId}/comments/new"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Status selectedStatus = Status.Find(parameters.statusId);
        Comment newComment = new Comment(Request.Form["content"], selectedStatus.Id, selectedUser.Id, DateTime.Now);
        newComment.Save();
        List<Status> selectedUserStatuses = selectedUser.GetStatuses();
        List<User> friendsList = selectedUser.GetFriends();
        model.Add("user", selectedUser);
        model.Add("user-statuses", selectedUserStatuses);
        model.Add("friends", friendsList);
        return View["profile.cshtml", model];
      };
      Post["/users/{loggedInId}/profile_view/{viewingId}/statuses/{statusId}/comment"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        User selectedUser = User.Find(parameters.viewingId);
        Status selectedStatus = Status.Find(parameters.statusId);
        Comment newComment = new Comment(Request.Form["content"], selectedStatus.Id, loggedInUser.Id, DateTime.Now);
        newComment.Save();
        List<Status> userStatuses = selectedUser.GetStatuses();
        model.Add("user", loggedInUser);
        model.Add("selected-user", selectedUser);
        model.Add("user-statuses", userStatuses);
        return View["friend.cshtml", model];
      };
      Delete["/users/{loggedInId}/profile_view/{viewingId}/unfollow"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        User selectedUser = User.Find(parameters.viewingId);
        loggedInUser.RemoveFriend(selectedUser);
        List<Status> userStatuses = selectedUser.GetStatuses();
        model.Add("user", loggedInUser);
        model.Add("selected-user", selectedUser);
        model.Add("user-statuses", userStatuses);
        return View["notfriend.cshtml", model];
      };
    }
  }
}
