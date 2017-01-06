using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rdio.Models.ContentManager;

namespace Rdio.ViewModel.ContentManager
{
    public class SimpleCategoryRssBindManageVM
    {
        public string _id { get; set; }
        public List<Rss> Rss { get; set; }
        public List<Category> Categories { get; set; }
        public string UserId { get; set; }
        public List<Tuple<string, string>> RssSitesName { get; set; }
    }
}