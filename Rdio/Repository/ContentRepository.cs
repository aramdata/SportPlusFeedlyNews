using MongoDB.Bson;
using Rdio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Rdio.Util;
using MongoDB.Driver;

namespace Rdio.Repository
{
    public class ContentRepository
    {
        public async Task<Models.Content.NewsContent> ContentInfo(string id)
        {
            var model = new Models.Content.NewsContent()
            { content=string.Empty,
            contenttype=0,
            image= string.Empty,
            lead= string.Empty,
            rotitr= string.Empty,
            rssid= string.Empty,
            tags=new List<string>(),
            titr= string.Empty,
            basecontentid = string.Empty,
            userid= string.Empty,
            _id= string.Empty,
            };

            try
            {
                var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'content',pipeline:[{$match:{_id:ObjectId('" + id + "')}},{$limit:1}]}");
                if (_model.GetValue("result").AsBsonArray.Any())
                    model = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Models.Content.NewsContent>(_model.GetValue("result")[0].AsBsonDocument);
            }
            catch (Exception ex)
            {
            }
            return model;
        }


        public async Task<bool> AddContent(Models.Content.NewsContent model)
        {
            try
            {
                var _id = ObjectId.GenerateNewId().ToString();
                if (model.tags == null)
                    model.tags = new List<string>();
                var res = await NoSql.Instance.RunCommandAsync<BsonDocument>("{update:'content',updates:[{q:{_id:ObjectId('" + _id + "')},u:{$set:{_id:ObjectId('" + _id + "'),userid:'" + model.userid + "',contenttype:"+model.contenttype+ ",rssid:'"+model.rssid+ "',basecontentid:'"+model.basecontentid+ "',titr:'" + model.titr + "',rotitr:'"+model.rotitr + "',lead:'"+model.lead+ "',content:'"+model.content + "',image:'"+model.image + "',tags:"+model.tags.toJSON()+",createdateticks:" + DateTime.Now.Ticks + ",url:'" + model.url + "'}},upsert:true}]}");

                //var collection = NoSql.Instance.GetCollection<BsonDocument>("content");
                //await collection.InsertOneAsync(model.ToBsonDocument());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}