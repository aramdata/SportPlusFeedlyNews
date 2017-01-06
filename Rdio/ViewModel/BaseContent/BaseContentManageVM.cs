using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.BaseContent
{
    public class BaseContentManageVM
    {
        public string rssid { get; set; }
        public string rsstitle { get; set; }
        public List<Models.ContentManager.Site> sites { get; set; }
        public List<Models.ContentManager.Rss> rsses { get; set; }
        public List<string> categories { get; set; }
        public List<string> tags { get; set; }


    }
}