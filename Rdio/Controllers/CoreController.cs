using MongoDB.Bson;
using Rdio.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using UC = Rdio.Util.Common;
using Rdio.Util;
//using System.Collections.Generic;
using System;

namespace Rdio.Controllers
{
    public class CoreController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> initialize()
        {
            if ((await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'users',pipeline:[{$match:{uname:'admin'}},{$limit:1}]}")).GetValue("result").AsBsonArray.Count == 0)
            {
                var collection = NoSql.Instance.GetCollection<BsonDocument>("roles");
                var bdoc = new BsonDocument { { "name", "مدیر" } };
                await collection.InsertOneAsync(bdoc);
                bdoc = new BsonDocument { { "name", "سازمانی" } };
                await collection.InsertOneAsync(bdoc);
                bdoc = new BsonDocument { { "name", "مشتری" } };
                await collection.InsertOneAsync(bdoc);
                collection = NoSql.Instance.GetCollection<BsonDocument>("users");
                bdoc = new BsonDocument { { "name", "مدیر سیستم" }, { "uname", "admin" }, { "password", "f865b53623b121fd34ee5426c792e5c33af8c227" /*admin123*/ }, { "email", "a@b.com" }, { "status", "فعال" }, { "createdat", DateTime.Now }, { "roles", new BsonArray(new string[] { "مدیر", "سازمانی" }) } };
                await collection.InsertOneAsync(bdoc);
                string uid = bdoc["_id"].ToString();
                collection = NoSql.Instance.GetCollection<BsonDocument>("perms");
                bdoc = new BsonDocument { { "title", "تنظیمات مدیریتی سیستم" }, { "key", "perm_superuser" }, { "users", new BsonArray { new BsonDocument { { "id", uid }, { "name", "مدیر سیستم" } } } }, { "roles", new BsonArray() } };
                await collection.InsertOneAsync(bdoc);
            }
            return new HttpNotFoundResult();
        }
    }
}