using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rdio.Models.ContentManager
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string userid { get; set; }
        public string title { get; set; }
        public string parentId { get; set; }
        public List<Block> blocks { get; set; }
    }
}