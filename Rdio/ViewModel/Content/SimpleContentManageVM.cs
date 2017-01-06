using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.Content
{
    public class SimpleContentManageVM
    {
        public string _id { get; set; }
        public int contenttype { get; set; }
        public string rssid { get; set; }
        public string userid { get; set; }
        public string basecontentid { get; set; }
        public string url { get; set; }
        public long createdateticks { get; set; }
        public string titr { get; set; }
        public string rotitr { get; set; }
        public string lead { get; set; }
        public string content { get; set; }
        public string image { get; set; }
        public List<string> tags { get; set; }
        public string pinsertdate { get; set; }
        public string pdate { get; set; }


    }
}