using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class CategoryRssBindEditVM
    {
        public string _id { get; set; }
        public List<SimpleBlockRssBindModel> BlockRssBind { get; set; }
    }


    public class SimpleBlockRssBindModel
    {
        public string CategoryId { get; set; }
        public string BlockCode { get; set; }
        public string RssId { get; set; }

    }
}