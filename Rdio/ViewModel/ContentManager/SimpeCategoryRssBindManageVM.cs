using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class SimpeCategoryRssBindManageVM
    {
        public List<Models.ContentManager.Category> Categories { get; set; }
        public List<Models.ContentManager.Rss> Rss { get; set; }
        public string UserId { get; set; }
    }
}