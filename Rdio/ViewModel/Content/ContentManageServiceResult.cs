using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.Content
{
    public class ContentManageServiceResult : ServiceResult
    {
        public List<ViewModel.Content.SimpleContentManageVM> Data { get; set; }
    }
}