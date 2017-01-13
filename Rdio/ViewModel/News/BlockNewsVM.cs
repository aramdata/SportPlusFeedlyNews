using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.News
{
    public class BlockNewsVM
    {
        public Models.ContentManager.Block Block { get; set; }
        public List<Models.Content.NewsContent> News { get; set; }
        public Models.ContentManager.Category Category { get; set; }

    }
}