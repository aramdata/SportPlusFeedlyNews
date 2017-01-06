using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class SimpleRssVM : ServiceResult
    {
        public List<Models.ContentManager.Rss> Data { get; set; }
    }
}