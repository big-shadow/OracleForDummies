using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OFD;

namespace Website.Models
{
    public class Author : Model
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public Author()
            : base()
        {

        }

        public Author(int id = 0)
            : base(id)
        {

        }
    }
}