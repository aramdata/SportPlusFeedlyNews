using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models.ContentManager
{
    public class Block
    {
        public string title { get; set; }
        public string code { get; set; }
        public List<string> blockrssbind { get; set; }
    }
}