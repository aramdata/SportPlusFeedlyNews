using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class CategoryManageServiceResult : ServiceResult
    {
        public List<Models.ContentManager.Category> Data { get; set; }
    }
}