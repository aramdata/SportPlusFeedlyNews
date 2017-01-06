using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.BaseContent
{
    public class SimpleBaseContentManageVM
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string rssid { get; set; }
        public string userid { get; set; }
        public long dateticks { get; set; }
        public long insertdateticks { get; set; }
        public string sitetitle { get; set; }
        public string rsstitle { get; set; }
        public string pinsertdate { get; set; }
        public string pdate { get; set; }


    }
}