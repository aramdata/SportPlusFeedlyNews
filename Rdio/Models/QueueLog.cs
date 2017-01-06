using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models
{
    public class QueueLog
    {
        public string exceptionmessage { get; set; }
        public long dateticks { get; set; }
        public int status { get; set; }
        public int logmessage { get; set; }


    }
}