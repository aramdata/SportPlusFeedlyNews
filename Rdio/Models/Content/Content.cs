using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models.Content
{
    public class Content
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int contenttype { get; set; }
        public string rssid { get; set; }
        public string userid { get; set; }
        public string basecontentid { get; set; }
        public long createdateticks { get; set; }
        public string url { get; set; }

    }
}