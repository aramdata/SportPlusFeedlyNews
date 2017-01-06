using MongoDB.Bson;
using Rdio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Rdio.Util;

namespace Rdio.Repository
{
    public class BaseContentRepository
    {
        Service.RssService rssService = new Service.RssService();
        public async Task<bool> Add(List<Models.BaseContent.BaseContent> model)
        {
            try
            {
                var BsonDocumentList = new List<BsonDocument>();
                foreach (var item in model)
                {
                    var isReapeated = await rssService.IsRepeatedUrl(item.url);
                    if (!isReapeated)
                        BsonDocumentList.Add(new BsonDocument() { { "title", item.title }, { "url", item.url }, { "description", item.description }, { "rssid", item.rssid }, { "userid", item.userid }, { "insertdateticks", item.insertdateticks }, { "dateticks", item.dateticks } , { "iscrawled",false } });
                }
                    
                var collection = NoSql.Instance.GetCollection<BsonDocument>("basecontent");
                await collection.InsertManyAsync(BsonDocumentList);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddRssURlInRedis(List<Models.BaseContent.BaseContent> model)
        {
            try
            {
                var db = Models.NoSql.Redis.GetDatabase();
                foreach (var item in model)
                {
                    var hashurl = Util.Common.GetMD5(item.url);
                    var res = await db.StringSetAsync(hashurl,item.url);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> ChangeIsCrawled(Models.BaseContent.BaseContent model)
        {
            try
            {
                var res = await NoSql.Instance.RunCommandAsync<BsonDocument>("{update:'basecontent',updates:[{q:{_id:ObjectId('" + model._id + "')},u:{$set:{iscrawled:true}},upsert:false}]}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Models.BaseContent.BaseContent>> Deque(int count)
        {
            var model = new List<Models.BaseContent.BaseContent>();
            try
            {
                var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'basecontent',pipeline:[{$match:{iscrawled:false}},{$sort : {'_id' : -1 }},{$limit:" + count + "}]}");
                //var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>("{aggregate:'rss',pipeline:[{$match:{}},{$sort : {'_id' : -1 }},{$limit:" + count + "}]}");

                if (_model.GetValue("result").AsBsonArray.Any())
                    foreach (var item in _model.GetValue("result").AsBsonArray)
                        model.Add(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Models.BaseContent.BaseContent>(item.AsBsonDocument));
            }
            catch (Exception ex)
            {
            }
            return model;

        }


    }
}