using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel
{
    public class ServiceResult: Pagination
    {
        public int ServiceResultStatus { get; set; }
        public string ServiceResultMassage { get; set; }

    }
}