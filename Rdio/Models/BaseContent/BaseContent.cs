using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models.BaseContent
{
    public class BaseContent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string rssid { get; set; }
        public string userid { get; set; }
        public long dateticks { get; set; }
        public long insertdateticks { get; set; }
        public bool iscrawled { get; set; }

    }
}