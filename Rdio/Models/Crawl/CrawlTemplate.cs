using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rdio.Models.Crawl
{
    public class CrawlTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string domain { get; set; }
        public string sampleurl { get; set; }
        public List<CrawlStructur> structure { get; set; }


    }
}