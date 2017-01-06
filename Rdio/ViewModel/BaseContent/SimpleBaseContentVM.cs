using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.BaseContent
{
    public class SimpleBaseContentVM : ServiceResult
    {
        public List<Models.BaseContent.BaseContent> Data { get; set; }
    }
}