using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Rdio.Service
{
    public class RssService
    {
        Service.CacheService cacheService = new CacheService();
        public async Task<bool> RssFetcherManager()
        {

            //TODO Repeated URL Problem gel  all url add then inset to redis this incorrect maybe first add one by one url in redis then add to list to insert mongo LN 36
            if (!IsInProccess())
            {
                try
                {
                    SetScheduleInProccess(SchedulerStat.inProccess);

                    Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();
                    Repository.BaseContentRepository BaseContentRepository = new Repository.BaseContentRepository();
                    var SuccessList = new List<Models.ContentManager.Rss>();
                    var BaseContentList = new List<Models.BaseContent.BaseContent>();
                    var deque = await ContentManagerRepository.DequeRss(10);
                    foreach (var item in deque)
                    {
                        var res = await RssFetcher(item.url);
                        if (res != null)
                        {
                            SuccessList.Add(item);
                            foreach (var rss in res)
                                BaseContentList.Add(new Models.BaseContent.BaseContent()
                                {
                                    dateticks = rss.dateticks,
                                    description = rss.description,
                                    insertdateticks = DateTime.Now.Ticks,
                                    rssid = item._id,
                                    title = rss.title,
                                    url = rss.url,
                                    userid = item.userid
                                });
                        }
                    }
                    //TODO: Resolve concarrency problem insert repeated if waite for preve task 
                    var addRes = await BaseContentRepository.Add(BaseContentList);
                    var changeRes = await ContentManagerRepository.ChangeLastCarawlDateRss(SuccessList);
                    var AddToRedisRes = await BaseContentRepository.AddRssURlInRedis(BaseContentList);

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
        public async Task<List<Models.BaseRssItem>> RssFetcher(string url)
        {
            try
            {
                var MainRssPattern = "rss/channel/item";
                var MainFeedPattern = "//atom:entry";

                if (string.IsNullOrEmpty(url))
                    return new List<Models.BaseRssItem>();
                string htmlContent = "";
                var uri = new Uri(url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0");
                    client.DefaultRequestHeaders.Add("Host", uri.Authority);
                    using (var r = await client.GetAsync(uri))
                    {
                        htmlContent = await r.Content.ReadAsStringAsync();
                    }
                }
                XmlDocument rssXmlDoc = new XmlDocument();
                rssXmlDoc.LoadXml(htmlContent);
                XmlNodeList rssNodes = rssXmlDoc.SelectNodes(MainRssPattern);
                var res = new List<Models.BaseRssItem>();

                if(rssNodes.Count>0)
                {
                    foreach (XmlNode rssNode in rssNodes)
                    {
                        XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                        string title = rssSubNode != null ? rssSubNode.InnerText : "";

                        rssSubNode = rssNode.SelectSingleNode("link");
                        string link = rssSubNode != null ? rssSubNode.InnerText : "";

                        rssSubNode = rssNode.SelectSingleNode("description");
                        string description = rssSubNode != null ? rssSubNode.InnerText : "";

                        rssSubNode = rssNode.SelectSingleNode("pubDate");
                        string date = rssSubNode != null ? rssSubNode.InnerText : "";
                        var datetime = DateTime.Now;
                        if (!string.IsNullOrWhiteSpace(date))
                            DateTime.TryParse(date, out datetime);

                        res.Add(new Models.BaseRssItem() { title = title, url = link, description = description, dateticks = datetime.Ticks });
                    }
                }
                else
                {
                    XmlNamespaceManager nsMngr = new XmlNamespaceManager(new NameTable());
                    nsMngr.AddNamespace(string.Empty, "http://www.w3.org/2005/Atom");
                    nsMngr.AddNamespace("app", "http://purl.org/atom/app#");
                    nsMngr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    rssNodes = rssXmlDoc.DocumentElement.SelectNodes(MainFeedPattern, nsMngr);
                    if (rssNodes.Count > 0)
                    {
                        foreach (XmlNode rssNode in rssNodes)
                        {
                            XmlNode rssSubNode = rssNode.SelectSingleNode("./atom:title",nsMngr);
                            string title = rssSubNode != null ? rssSubNode.InnerText : "";

                            //rssSubNode = rssNode.SelectSingleNode("./atom:id", nsMngr);
                            rssSubNode = rssNode.SelectSingleNode("./atom:link[1]", nsMngr);
                            string link = (rssSubNode != null && rssSubNode.Attributes!=null) ? rssSubNode.Attributes["href"].Value : "";

                            rssSubNode = rssNode.SelectSingleNode("./atom:summary", nsMngr);
                            string description = rssSubNode != null ? rssSubNode.InnerText : "";

                            rssSubNode = rssNode.SelectSingleNode("./atom:published", nsMngr);
                            string date = rssSubNode != null ? rssSubNode.InnerText : "";
                            var datetime = DateTime.Now;
                            if (!string.IsNullOrWhiteSpace(date))
                                DateTime.TryParse(date, out datetime);

                            res.Add(new Models.BaseRssItem() { title = title, url = link, description = description, dateticks = datetime.Ticks });
                        }
                    }
                }
                
                return res;
            }
            catch (Exception ex)
            {
                //Retune Null To Underestand Exception Is Fire !
                return null;
            }
        }

        public async Task<bool> IsRepeatedUrl(string url)
        {
            try
            {
                var db = Models.NoSql.Redis.GetDatabase();
                var value = await db.StringGetAsync(Util.Common.GetMD5(url));
                if (!string.IsNullOrWhiteSpace(value))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        string IsInProccessCacheKey = "SchedulerInProccess";
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