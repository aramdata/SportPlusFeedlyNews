using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.BaseContent
{
    public class BaseContentSearchVM
    {
        public int page { get; set; }
        public string rssid { get; set; }
        public List<string> tags { get; set; }
        public List<string> categories { get; set; }
        public string siteid { get; set; }

    }
}