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
        Post newPost = new Post("Hello world", 1, DateTime.Now);
        newPost.Save();
        return View["index.cshtml", newPost];
      };
    }
  }
}
