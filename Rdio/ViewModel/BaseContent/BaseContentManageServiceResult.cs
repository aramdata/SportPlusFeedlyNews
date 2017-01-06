using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.BaseContent
{
    public class BaseContentManageServiceResult : ServiceResult
    {
        public List<ViewModel.BaseContent.SimpleBaseContentManageVM> Data { get; set; }
    }
}