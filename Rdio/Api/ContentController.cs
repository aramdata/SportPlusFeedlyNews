﻿using MongoDB.Bson;
using Rdio.Models;
using Rdio.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rdio.Api
{
    public class ContentController : ApiController
    {
        Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();
        Repository.ContentRepository contentRepository = new Repository.ContentRepository();

        [Authorize]
        [Route("api/Content/Manage")]
        public async Task<ViewModel.Content.ContentManageServiceResult> PostManage([FromBody]ViewModel.BaseContent.BaseContentSearchVM simpleSearch)
        {
            try
            {
                var page = simpleSearch.page < 1 ? 1 : simpleSearch.page;
                var limit = 20;
                var skip = limit * (page - 1);

                //var rssids = new List<string>();
                //var contentManagerRepository = new Repository.ContentManagerRepository();
                //var _userRssList = await contentManagerRepository.GetUserAllRss(Util.Common.My.id);
                //if (_userRssList.Any())
                //{
                //    if (simpleSearch.categories!=null && simpleSearch.categories.Any() && !simpleSearch.categories.Any(s=>s=="-1"))
                //        _userRssList = _userRssList.Where(s => s.categories.Any(x => simpleSearch.categories.Contains(x))).ToList();

                //    if (simpleSearch.tags != null &&  simpleSearch.tags.Any() && !simpleSearch.tags.Any(s => s == "-1"))
                //        _userRssList = _userRssList.Where(s => s.tags.Any(x => simpleSearch.tags.Contains(x))).ToList();

                //    if(!string.IsNullOrWhiteSpace(simpleSearch.siteid) && simpleSearch.siteid!="-1")
                //        _userRssList = _userRssList.Where(s => s.siteid== simpleSearch.siteid).ToList();

                //    if (!string.IsNullOrWhiteSpace(simpleSearch.rssid) && simpleSearch.rssid!="-1")
                //        _userRssList = _userRssList.Where(s => s._id == simpleSearch.rssid).ToList();

                //    rssids = _userRssList.Select(s => s._id).ToList();
                //}

                //string q = "{aggregate:'basecontent',pipeline:[{$match:{ 'rssid':{$in:"+ rssids.toJSON() + "}}},{$sort : { 'dateticks' : -1  }},{$skip:" + skip.ToString() + "},{$limit:" + limit.ToString() + "}]}";
                string q = "{aggregate:'content',pipeline:[{$match:{}},{$sort : { 'createdateticks' : -1  }},{$skip:" + skip.ToString() + "},{$limit:" + limit.ToString() + "}]}";


                var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>(q);
                var model0 = new List<Models.Content.NewsContent>();
                foreach (var item in _model.GetValue("result").AsBsonArray)
                    model0.Add(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Models.Content.NewsContent>(item.AsBsonDocument));

                var model = new List<ViewModel.Content.SimpleContentManageVM>();
                foreach (var item in model0)
                {
                    var rssModel = await ContentManagerRepository.RssInfo(item.rssid);
                    var siteModel = await ContentManagerRepository.SiteInfo(rssModel.siteid);
                    model.Add(new ViewModel.Content.SimpleContentManageVM()
                    {
                        basecontentid=item.basecontentid,
                        content=item.content,
                        contenttype=item.contenttype,
                        createdateticks=item.createdateticks,
                        image=item.image,
                        lead=item.lead,
                        rotitr=item.rotitr,
                        tags=item.tags,
                        titr=item.titr,
                        pinsertdate = Common.ConvertG2JDateText(new DateTime(item.createdateticks),true),
                        rssid = item.rssid,
                        url = item.url,
                        userid = item.userid,
                        _id = item._id,
                    });
                }
                    
                var result = new ViewModel.Content.ContentManageServiceResult();
                result.Data = model;
                result.CurrentPage = page;
                result.PrevPage = (page == 1 ? 2 : page) - 1;
                result.NextPage = page + 1;
                result.ServiceResultStatus = (int)Util.Common.ServiceResultStatus.OK;
                return result;
            }
            catch (Exception ex)
            {
                return new ViewModel.Content.ContentManageServiceResult()
                {
                    ServiceResultStatus = (int)Util.Common.ServiceResultStatus.Error,
                    ServiceResultMassage = ex.GetBaseException().Message
                };
            }
        }
    }
}
