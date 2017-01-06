using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Rdio.Service
{
    public class CrawlerService
    {
        Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();
        Repository.ContentRepository ContentRepository = new Repository.ContentRepository();
        Repository.BaseContentRepository baseContentRepository = new Repository.BaseContentRepository();
        Service.CacheService cacheService = new CacheService();

        public async Task<bool> CrawlManager()
        {
            if (!IsInProccess())
            {
                try
                {
                    SetScheduleInProccess(SchedulerStat.inProccess);
                    Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();
                    Repository.BaseContentRepository BaseContentRepository = new Repository.BaseContentRepository();
                    var SuccessList = new List<Models.ContentManager.Rss>();
                    var BaseContentList = new List<Models.BaseContent.BaseContent>();
                    var deque = await BaseContentRepository.Deque(10);
                    //TODO : loop in basecontetn and then just pass them to crawl function and those change status and add to content repository possible not true !!!
                    foreach (var item in deque)
                    {
                        var res = await Crawler(item);
                    }
                    
                    SetScheduleInProccess(SchedulerStat.idle);
                    return true;
                }
                catch (Exception ex)
                {
                    SetScheduleInProccess(SchedulerStat.idle);
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> Crawler(Models.BaseContent.BaseContent model)
        {
            try
            {
                string htmlContent = "";
                var uri = new Uri(model.url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0");
                    client.DefaultRequestHeaders.Add("Host", uri.Authority);
                    using (var r = await client.GetAsync(uri))
                    {
                        htmlContent = await r.Content.ReadAsStringAsync();
                    }
                }
                var parser = new HtmlParser();
                var document = parser.Parse(htmlContent);

                var rssModel = await ContentManagerRepository.RssInfo(model.rssid);
                var SiteModel = await ContentManagerRepository.SiteInfo(rssModel.siteid);

                //var template = Util.Common.fromJSON<Models.Crawl.CrawlTemplate>(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Models/simplecrawltemplate.json")));
                //var template = Util.Common.fromJSON<Models.Crawl.CrawlTemplate>(System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/Models/simplecrawltemplate.json")));
                var template = SiteModel.template.FirstOrDefault();

                if(template!=null)
                {
                    var content = new Models.Content.NewsContent();
                    content.rssid = model.rssid;
                    content.userid = model.userid;
                    content.contenttype = (int)Util.Configuration.ContentType.News;
                    content.basecontentid = model._id;
                    content.url = model.url;

                    foreach (var item in template.structure)
                    {
                        var element = document.QuerySelector(item.query);
                        var elementcontent = "";
                        if (element != null && !string.IsNullOrWhiteSpace(item.query))
                        {
                            switch (item.type)
                            {
                                case "innerhtml":
                                    elementcontent = Util.Common.CleanHtmlContent(element.InnerHtml);
                                    break;
                                case "src":
                                    elementcontent = element.GetAttribute(item.type);
                                    break;
                                default:
                                    break;
                            }
                            try
                            {
                                content.GetType().GetProperty(item.field).SetValue(content, elementcontent, null);
                            }
                            catch
                            {
                            }
                        }
                    }

                    var result = await ContentRepository.AddContent(content);

                    if (result)
                        await baseContentRepository.ChangeIsCrawled(model);
                    return result;
                }

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }

        string IsInProccessCacheKey = "CrawlInProccess";
        public enum SchedulerStat
        {
            inProccess = 0,
            idle = 1
        }
        public bool IsInProccess()
        {
            try
            {
                var model = cacheService.GetCache(IsInProccessCacheKey);
                if (model == null)
                    return false;
                else
                    return int.Parse(model.ToString()) == (int)SchedulerStat.inProccess ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void SetScheduleInProccess(SchedulerStat stat)
        {
            cacheService.AddToCache(IsInProccessCacheKey, ((int)stat).ToString());
        }
    }
}