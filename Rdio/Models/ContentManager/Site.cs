using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Models.ContentManager
{
    public class Site
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string userid { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public long createdateticks { get; set; }
        public List<Models.Crawl.CrawlTemplate> template { get; set; }


    }
}