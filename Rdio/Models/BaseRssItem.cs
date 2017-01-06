using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models
{
    public class BaseRssItem
    {
        public string _is { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public long dateticks { get; set; }
    }
}