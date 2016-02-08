using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public static class ViewHelper
    {
        public static string GetUsername(int id)
        {
            return Author.ScalarWhereID<Author>(id).Username;
        }

        public static string GetCategoryName(int id)
        {
            return Category.ScalarWhereID<Category>(id).Title;
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("MMMM d, yyyy - h:mm") + date.ToString("tt").ToLower();
        }

        public static string LimitLength(string text, int length)
        {
            if (text.Length > length)
            {
                return text.Substring(0, length) + " ...";
            }

            return text;
        }
    }
}