using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rdio.Service;

namespace Rdio.ViewModel.News
{
    public class ServiceVM
    {
        public Models.ContentManager.Category Categories { get; set; }
        public List<Models.ContentManager.Block> Blocks { get; set; }
        public List<Tuple<Models.ContentManager.Block,List<Models.Content.NewsContent>>> BlockNews { get; set; }


    }
}