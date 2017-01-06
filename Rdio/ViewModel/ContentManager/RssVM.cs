using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class RssVM
    {
        public string _id { get; set; }
        public string siteid { get; set; }
        public string sitetitle { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string userid { get; set; }
        public List<string> tags { get; set; }
        public List<string> categories { get; set; }
        public string lang { get; set; }
    }
}