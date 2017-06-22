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
      Post["/users/{userId}/statuses/{statusId}/comment"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Status selectedStatus = Status.Find(parameters.statusId);
        Comment newComment = new Comment(Request.Form["content"], selectedStatus.Id, selectedUser.Id, DateTime.Now);
        newComment.Save();
        List<Status> timeline = selectedUser.GetTimeline();
        model.Add("timeline", timeline);
        model.Add("user", selectedUser);
        return View["news.cshtml", model];
      };
      Post["/users/{userId}/status"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.userId);
        Status newStatus = new Status(Request.Form["status"], selectedUser.Id, DateTime.Now);
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
          model.Add("friends", loggedInUser.GetFriends());
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
      Post["/users/{loggedInId}/profile_view/{viewingId}/follow"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        User selectedUser = User.Find(parameters.viewingId);
        loggedInUser.AddFriend(selectedUser);
        List<Status> userStatuses = selectedUser.GetStatuses();
        model.Add("user", loggedInUser);
        model.Add("selected-user", selectedUser);
        model.Add("user-statuses", userStatuses);
        return View["friend.cshtml", model];
      };
      Get["/users/{loggedInId}/profile_view"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.loggedInId);
        List<Status> selectedUserStatuses = selectedUser.GetStatuses();
        List<User> friendsList = selectedUser.GetFriends();
        model.Add("user", selectedUser);
        model.Add("user-statuses", selectedUserStatuses);
        model.Add("friends", friendsList);
        return View["profile.cshtml", model];
      };
      Get["/users/{loggedInId}/news"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User selectedUser = User.Find(parameters.loggedInId);
        List<Status> timeline = selectedUser.GetTimeline();
        model.Add("timeline", timeline);
        model.Add("user", selectedUser);
        return View["news.cshtml", model];
      };
      Get["/users/{loggedInId}/search"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        string searchQuery = Request.Query["search-query"];
        List<User> matches = User.Search(searchQuery);
        model.Add("user", loggedInUser);
        model.Add("search-query", searchQuery);
        model.Add("matches", matches);
        return View["searchresults.cshtml", model];
      };
      Get["/users/{loggedInId}/friends"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        List<User> friendsList = loggedInUser.GetFriends();
        model.Add("friends", friendsList);
        model.Add("user", loggedInUser);
        return View["searchresults.cshtml", model];
      };
      Post["/users/{loggedInId}/statuses/{statusId}/like"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        string directTo = Request.Form["redirect"];
        User loggedInUser = User.Find(parameters.loggedInId);
        Status statusToLike = Status.Find(parameters.statusId);
        // Console.WriteLine(loggedInUser.HasLikedStatus(statusToLike));
        if(!(loggedInUser.HasLikedStatus(statusToLike)))
        {
          // Console.WriteLine("Hello");
          loggedInUser.LikeStatus(statusToLike);
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
        else
        {
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
      };
      Post["/users/{loggedInId}/statuses/{statusId}/dislike"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        string directTo = Request.Form["redirect"];
        User loggedInUser = User.Find(parameters.loggedInId);
        Status statusToDislike = Status.Find(parameters.statusId);
        // Console.WriteLine(loggedInUser.HasDislikedStatus(statusToDislike));
        if(!(loggedInUser.HasDislikedStatus(statusToDislike)))
        {
          // Console.WriteLine("Hello");
          loggedInUser.DislikeStatus(statusToDislike);
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
        else
        {
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
      };
      Post["/users/{loggedInId}/statuses/{statusId}/comments/{commentId}/like"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        string directTo = Request.Form["redirect"];
        User loggedInUser = User.Find(parameters.loggedInId);
        Status selectedStatus = Status.Find(parameters.statusId);
        Comment commentToLike = Comment.Find(parameters.commentId);
        // Console.WriteLine(loggedInUser.HasDislikedStatus(statusToDislike));
        if(!(loggedInUser.HasLikedComment(commentToLike)))
        {
          // Console.WriteLine("Hello");
          loggedInUser.LikeComment(commentToLike);
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
        else
        {
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
      };
      Post["/users/{loggedInId}/statuses/{statusId}/comments/{commentId}/dislike"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        string directTo = Request.Form["redirect"];
        User loggedInUser = User.Find(parameters.loggedInId);
        Status selectedStatus = Status.Find(parameters.statusId);
        Comment commentToDislike = Comment.Find(parameters.commentId);
        // Console.WriteLine(loggedInUser.HasDislikedStatus(statusToDislike));
        if(!(loggedInUser.HasDislikedComment(commentToDislike)))
        {
          // Console.WriteLine("Hello");
          loggedInUser.DislikeComment(commentToDislike);
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
        else
        {
          if(directTo == "friend")
          {
            User selectedUser = User.Find(Request.Form["userId"]);
            List<Status> selectedUserStatuses = selectedUser.GetStatuses();
            model.Add("user", loggedInUser);
            model.Add("selected-user", selectedUser);
            model.Add("user-statuses", selectedUserStatuses);
            return View[directTo + ".cshtml", model];
          }
          else
          {
            model.Add("user", loggedInUser);
            model.Add("timeline", loggedInUser.GetTimeline());
            model.Add("user-statuses", loggedInUser.GetStatuses());
            model.Add("friends", loggedInUser.GetFriends());
            return View[directTo + ".cshtml", model];
          }
        }
      };
      Get["/users/{loggedInId}/profile/edit"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        model.Add("user", loggedInUser);
        return View["editprofile.cshtml", model];
      };
      Patch["users/{loggedInId}/profile/updated"] = parameters => {
        Dictionary <string, object> model = new Dictionary<string, object>{};
        User loggedInUser = User.Find(parameters.loggedInId);
        string password = Request.Form["password"];
        string passwordConfirm = Request.Form["password-confirm"];
        if(password != passwordConfirm)
        {
          model.Add("user", loggedInUser);
          model.Add("password-mismatch", true);
          return View["editprofile.cshtml", model];
        }
        else
        {
          loggedInUser.Update(Request.Form["first-name"], Request.Form["last-name"], Request.Form["username"], password, Request.Form["email"]);
          model.Add("user", loggedInUser);
          model.Add("user-statuses", loggedInUser.GetStatuses());
          model.Add("friends", loggedInUser.GetFriends());
          return View["profile.cshtml", model];
        }
      };
      Delete["/users/{loggedInId}/profile/delete"] = parameters => {
        User loggedInUser = User.Find(parameters.loggedInId);
        loggedInUser.Delete();
        return View["index.cshtml"];
      };
    }
  }
}

//NOTE FIX SEARCH METHOD/RESULTS
