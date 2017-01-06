using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models.Content
{
    public class NewsContent:Content
    {
        public string titr { get; set; }
        public string rotitr { get; set; }
        public string lead { get; set; }
        public string content { get; set; }
        public string image { get; set; }
        public List<string> tags { get; set; }

    }
}