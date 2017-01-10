using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.News
{
    public class NewsVM
    {
        public string Titr { get; set; }
        public string Rotitr { get; set; }
        public string Lead { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public long DateTicks { get; set; }
        public List<string> Tags { get; set; }
        public Models.ContentManager.Category Category { get; set; }
        public List<BlockNewsVM> BlockNews { get; set; }

    }
}