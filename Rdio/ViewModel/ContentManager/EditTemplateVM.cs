using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class EditTemplateVM
    {
        public string _id { get; set; }
        public string siteid { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string sampleurl { get; set; }
        public List<Models.Crawl.CrawlStructur> structure { get; set; }

    }
}