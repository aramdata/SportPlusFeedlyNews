using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models.ContentManager
{
    public class Rss
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string siteid { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string userid { get; set; }
        public List<string> tags { get; set; }
        public List<string> categories { get; set; }
        public string lang { get; set; }
        public long createdateticks { get; set; }
        public long lastcrawldate { get; set; }
    }
}