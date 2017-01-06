using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rdio.Models.ContentManager;

namespace Rdio.Controllers
{
    public class ContentManagerController : Controller
    {
        Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();

        [Authorize]
        public async Task<ActionResult> EditSite(string id)
        {
            var SiteInfo = await ContentManagerRepository.SiteInfo(id);
            var model = new ViewModel.ContentManager.SiteVM();
            if (!string.IsNullOrWhiteSpace(id))
                model = new ViewModel.ContentManager.SiteVM
                {
                    title=SiteInfo.title,
                    url=SiteInfo.url,
                    _id=SiteInfo._id,
                    userid=SiteInfo.userid,
                    createdateticks=SiteInfo.createdateticks
                };
            return View(model);
        }
        [Authorize]
        public ActionResult SiteManage()
        {
            return View();
        }
        [Authorize]
        public async Task<ActionResult> RssManage(string id,string siteid)
        {
            var siteModel = await ContentManagerRepository.SiteInfo(siteid);
            var rssModel = await ContentManagerRepository.RssInfo(id);

            var model = new ViewModel.ContentManager.RssVM() {
                sitetitle= siteModel.title,
                siteid= siteModel._id,
                categories= rssModel.categories,
                tags = rssModel.tags,
                lang= rssModel.lang,
                title= rssModel.title,
                url= rssModel.url,
                userid= rssModel.userid,
                _id= rssModel._id
            };
            return View(model);
        }
        [Authorize]
        public async Task<ActionResult> EditTemplate(string siteid,string templateid)
        {
            var SiteModel = await ContentManagerRepository.SiteInfo(siteid);
            Models.Crawl.CrawlTemplate Template=null;
            if (SiteModel.template != null)
                Template = SiteModel.template.FirstOrDefault();

            var model = new ViewModel.ContentManager.TemplateVM()
            {
                siteid = SiteModel._id,
                name = "",
                sampleurl = "",
                type = "",
                structure = new List<Models.Crawl.CrawlStructur>()
            };

            if(Template!=null)
                model= new ViewModel.ContentManager.TemplateVM()
                {
                    _id=Template._id,
                    siteid = SiteModel._id,
                    name = Template.name,
                    sampleurl = Template.sampleurl,
                    type = Template.type,
                    structure = Template.structure
                };
            return View(model);
        }
        [Authorize]
        public async Task<ActionResult> CategoryManage(string id)
        {
            var categoryModel = await ContentManagerRepository.CategoryInfo(id);
            var UserId = Util.Common.My.id;
            var UserCategories = await ContentManagerRepository.GetUserCategories(UserId);
            var ParentTitle = string.Empty;

            if (string.IsNullOrWhiteSpace(categoryModel.userid))
                categoryModel.userid = Util.Common.My.id;

            if (!string.IsNullOrEmpty(categoryModel.parentId) && categoryModel.parentId != "-1")
                ParentTitle = (await ContentManagerRepository.CategoryInfo(categoryModel.parentId)).title;
            var model = new ViewModel.ContentManager.SimpleCategoryManageVM()
            {
                blocks = categoryModel.blocks,
                parentId = categoryModel.parentId,
                parenttitle = ParentTitle,
                allblocks = Util.Configuration.AllBlocks(),
                _id = categoryModel._id,
                title = categoryModel.title,
                allcategories = UserCategories,
                userId = categoryModel.userid
            };
            return View(model);
        }
        [Authorize]
        public async Task<ActionResult> BlockRssBindManage(string id)
        {
            //var categoryModel = await ContentManagerRepository.CategoryInfo(id);
            //var UserId = Util.Common.My.id;
            //var UserCategories = await ContentManagerRepository.GetUserCategories(UserId);
            //var ParentTitle = string.Empty;

            //if (string.IsNullOrWhiteSpace(categoryModel.userid))
            //    categoryModel.userid = Util.Common.My.id;

            //if (!string.IsNullOrEmpty(categoryModel.parentId) && categoryModel.parentId != "-1")
            //    ParentTitle = (await ContentManagerRepository.CategoryInfo(categoryModel.parentId)).title;
            var UserId = Util.Common.My.id;
            var UserRss = await ContentManagerRepository.GetUserAllRss(UserId);
            var RssSitesInfo = await ContentManagerRepository.SitesInfo(UserRss);
            var UserCategories = await ContentManagerRepository.GetUserCategories(UserId);
            var RssSitesName = new List<Tuple<string, string>>();
            foreach (var rss in UserRss)
            {
                var SiteInfo = RssSitesInfo.FirstOrDefault(q => q._id == rss.siteid);
                RssSitesName.Add(new Tuple<string, string>(rss._id,SiteInfo.title));
            }
            var model = new ViewModel.ContentManager.SimpleCategoryRssBindManageVM()
            {
                Rss = UserRss,
                Categories = UserCategories,
                UserId= UserId,
                RssSitesName = RssSitesName
            };
            return View(model);
        }

        public async Task<ActionResult> mohsen() {
            var res = await ContentManagerRepository.DeleteUserCategoriesBlocks();

            var html =new List<string>()
            {
                @"<font color='#333333' size='1' face='tahoma'>کد خبر: 1383520/7&nbsp; زمان: 20:15 &nbsp;1395/10/09<!--/font><font color=white-->&nbsp;&nbsp;بازدید: <span id='visit'>8,992</span></font>",
                @"<div class='news_nav col-xs-12 col-md-4 col-lg-5'>     	<span class='news_nav_title'>تاریخ انتشار: </span>۰۹ دی ۱۳۹۵ - ۱۸:۳۳     	</div>",
                @"<span class='margin-l-md'>پنجشنبه 9 دی 1395 - 12:45:29</span>",
                @"<span class='oliveDate'>1395/10/08</span>",
                @"<span class='oliveDate' style='float:left'>۱۳۹۵/۱۰/۰۶</span>",
                @"<time><i class='fa fa-clock - o'></i> ۰۹ دی ۱۳۹۵ - ۱۳:۱۴ </time>",
                @"<time datetime='2016 - 12 - 29T16: 31:55 + 00:00' itemprop='datePublished'>Dec 29, 2016 16:31:55</time>",
                @"<h5>پنجشنبه، ۹ دی ۱۳۹۵، ساعت ۱۸:۵۰  — تسنیم</h5>",
                @"<time itemprop='dateModified' datetime='2016 - 12 - 29T14: 55:13Z'>29 Dec 2016</time>",
                @"<div class='created'>پنجشنبه 9 دى 1395 ساعت 19:48 < span > 2016 - 12 - 29 19:48:23 </ span ></ div >" ,
                @"<div id='docDiv3Date'>تاریخ انتشار : <span>پنجشنبه ۹ دی ۱۳۹۵ ساعت ۱۹:۵۸</span></div>",
                @"<div class='news_nav news_pdate_c'> 					<span class='news_title'>تاریخ انتشار: </span> 					<sapn class='fa_date'>۰۹ دی ۱۳۹۵ - ۰۰:۴۲</sapn> 					<span class='en_date visible-lg visible - md'> 						29 December 2016 					</span> 				</div>",
                @"<span class='timestamp'>Thu Dec 29, 2016 | 12:30pm EST</span>",
                @"<div class='news_nav news_pdate_c'>۰۹ دی ۱۳۹۵ - ۱۵:۱۶</div>",
            };
            foreach (var item in html)
            {
                Util.Common.ParsDateFromHtml(item);
            }
            //var rs = new Service.RssService();
            //var res=await rs.RssFetcherManager();
            return Content("Salam - ");
        }
    }
}