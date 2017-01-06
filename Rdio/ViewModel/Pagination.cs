using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel
{
    public class Pagination
    {
        public int PrevPage { get; set; }
        public int NextPage { get; set; }
        public int CurrentPage { get; set; }

    }
}