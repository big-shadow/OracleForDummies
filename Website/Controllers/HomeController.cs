using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Random rnd = new Random();

            Post post = new Post();
            //post.Drop();

            post.Title = "Epic Meal Time " + rnd.Next(1, 100);
            post.AuthorID = 1;
            post.CategoryID = 1;
            post.DatePosted = DateTime.Now;
            post.PostText = "A really cool post! " + rnd.Next(1, 100);
            post.Save();

            return View();
        }

        // Partials
        [HttpPost]
        [ActionName("PostSearch")]
        public ActionResult GetPosts(string keyword)
        {
            ViewBag.Posts = Post.GetWhere<Post>("PostText LIKE '%" + keyword + "%'");

            return PartialView("~/Views/Posts/PostsTable.cshtml");
        }

        [HttpPost]
        [ActionName("RecentPosts")]
        public ActionResult GetPosts(int count)
        {
            ViewBag.Posts = Post.GetWhere<Post>("RowNum <= " + count + " ORDER BY DatePosted DESC");

            return PartialView("~/Views/Posts/PostsTable.cshtml");
        }
    }
}
