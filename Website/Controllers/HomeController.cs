using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Models;
using System.Text.RegularExpressions;
using System.Text;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private static List<string> IgnoredSearchTerms;

        public HomeController()
        {
            IgnoredSearchTerms = new List<string>();
            IgnoredSearchTerms.Add("the");
            IgnoredSearchTerms.Add("and");
            IgnoredSearchTerms.Add("if");
            IgnoredSearchTerms.Add("it");
        }

        public ActionResult Index()
        {
            Random rnd = new Random();

            Category category = new Category();
            //category.Drop();

            category.Title = "Random (" + rnd.Next(1, 100) + ")";
            category.Save();

            Author author = new Author();
            //user.Drop();

            author.Username = "Ray Winkelman";
            author.Password = "password";
            author.Save();

            Post post = new Post();
            //post.Drop();

            post.Title = "Cucumber Socks (" + rnd.Next(1, 100) + ")";
            post.AuthorID = author.ID;
            post.CategoryID = category.ID;
            post.DatePosted = DateTime.Now;
            post.PostText = "I had a really fun time. (" + rnd.Next(1, 100) + "). I wonder when this will get trimmed?";
            post.Save();

            return View();
        }

        // Partials
        [HttpPost]
        [ActionName("PostSearch")]
        public ActionResult GetPosts(string keyword)
        {
            // This is for SQL injection attempts. 
            keyword = Regex.Replace(keyword, @"([^\w|\s])", string.Empty);

            StringBuilder sql = new StringBuilder("RowNum < 9 ");

            foreach (string term in keyword.Split())
            {
                if (term.Length > 0 && !IgnoredSearchTerms.Contains(term.ToLowerInvariant()))
                {
                    sql.Append("AND PostText LIKE '%" + term + "%'");
                }
            }

            ViewBag.Posts = Post.GetWhere<Post>(sql.ToString());

            return PartialView("~/Views/Posts/PostsTable.cshtml");
        }

        [HttpPost]
        [ActionName("RecentPosts")]
        public ActionResult GetPosts(int count)
        {
            ViewBag.Posts = Post.GetWhere<Post>("RowNum <= " + count + " ORDER BY DatePosted DESC");

            return PartialView("~/Views/Posts/PostsTable.cshtml");
        }

        [HttpPost]
        [ActionName("RecentPostsInCategory")]
        public ActionResult GetPostsInCategory(int id)
        {
            ViewBag.Posts = Post.GetWhere<Post>("CategoryID = " + id + " AND RowNum < 9 ORDER BY DatePosted DESC");

            return PartialView("~/Views/Posts/PostsTable.cshtml");
        }

        [HttpPost]
        [ActionName("SinglePost")]
        public ActionResult GetSinglePost(int id)
        {
            ViewBag.Post = Post.ScalarWhereID<Post>(id);

            return PartialView("~/Views/Posts/SinglePost.cshtml");
        }
    }
}
