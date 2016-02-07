using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OFD;

namespace Website.Models
{
    public class Post : Model
    {
        public string Title { get; set; }
        public int AuthorID { get; set; }
        public DateTime DatePosted { get; set; }
        public string PostText { get; set; }
        public int CategoryID { get; set; }

        public Post()
            : base()
        {

        }

        public Post(int id = 0)
            : base(id)
        {

        }
    }
}