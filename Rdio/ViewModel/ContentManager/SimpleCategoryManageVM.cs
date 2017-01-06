using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rdio.Models.ContentManager;

namespace Rdio.ViewModel.ContentManager
{
    public class SimpleCategoryManageVM
    {
        public string _id { get; set; }
        public string userId { get; set; }
        public string title { get; set; }
        public string parentId { get; set; }
        public string parenttitle { get; set; }
        public List<Block> blocks { get; set; }
        public List<Block> allblocks { get; set; }
        public List<Category> allcategories { get; set; }
    }
}