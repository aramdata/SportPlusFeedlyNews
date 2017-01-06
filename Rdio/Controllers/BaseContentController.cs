using AngleSharp.Parser.Html;
using MongoDB.Bson;
using Rdio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Rdio.Controllers
{
    public class BaseContentController : Controller
    {
        Service.CrawlerService crawlService = new Service.CrawlerService();
        public async Task<ActionResult> m(string q)
        {
            string qq = "{aggregate:'basecontent',pipeline:[{$match:{ 'rssid':'5846f904281007277c52777c'}},{$sort : { 'dateticks' : -1  }},{$limit:1}]}";
            var _model = await NoSql.Instance.RunCommandAsync<BsonDocument>(qq);
            var model0 = new List<Models.BaseContent.BaseContent>();
            foreach (var item in _model.GetValue("result").AsBsonArray)
                model0.Add(MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Models.BaseContent.BaseContent>(item.AsBsonDocument));
            await crawlService.Crawler(model0.FirstOrDefault());
            if (!string.IsNullOrWhiteSpace(q))
            {
                var url = "http://www.farsnews.com/newstext.php?nn=13950924001718";
                var html = "";
                using (var client = new HttpClient())
                {
                    html = await client.GetStringAsync(url);
                }
                var parser = new HtmlParser();
                var document = parser.Parse(html);
                var elements = document.QuerySelectorAll(q);
                var re = "";
                foreach (var item in elements)
                {
                    re = item.GetAttribute("src");
                }
                ViewBag.re = re;
                ViewBag.q = q;
            }
            return View();
        }
        Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();

        [Authorize]
        public async Task<ActionResult> Manage(string rssid)
        {
            var rssModel = await ContentManagerRepository.RssInfo(rssid);
            var userid = Util.Common.My.id;
            var UserAllRss = await ContentManagerRepository.GetUserAllRss(userid);
            var UserAllSite = await ContentManagerRepository.GetUserAllSite(userid);
            var UserCategories = UserAllRss.SelectMany(s => s.categories).Distinct().ToList();
            var UserTags = UserAllRss.SelectMany(s=>s.tags).Distinct().ToList();

            return View(new ViewModel.BaseContent.BaseContentManageVM() {
                rssid=rssModel._id,
                rsstitle=rssModel.title,
                categories=UserCategories,
                rsses = UserAllRss,
                sites = UserAllSite,
                tags = UserTags,
            });
        }
    }
}