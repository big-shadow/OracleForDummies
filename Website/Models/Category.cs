using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OFD;

namespace Website.Models
{
    public class Category : Model
    {
        public string Title { get; set; }
        
        public Category()
            : base()
        {

        }

        public Category(int id = 0)
            : base(id)
        {

        }
    }
}