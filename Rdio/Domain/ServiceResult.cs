using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Domain
{
    public class ServiceResult<T>
    {
        public int ServiceResultStatus { get; set; }
        public string ServiceResultMassage { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}