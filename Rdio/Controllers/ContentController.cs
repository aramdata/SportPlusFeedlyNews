using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Rdio.Controllers
{
    public class ContentController : Controller
    {
        Repository.ContentRepository contentRepository = new Repository.ContentRepository();
        Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();

        [Authorize]
        public async Task<ActionResult> Manage(string rssid)
        {
            var rssModel = await ContentManagerRepository.RssInfo(rssid);
            var userid = Util.Common.My.id;
            var UserAllRss = await ContentManagerRepository.GetUserAllRss(userid);
            var UserAllSite = await ContentManagerRepository.GetUserAllSite(userid);
            var UserCategories = UserAllRss.SelectMany(s => s.categories).Distinct().ToList();
            var UserTags = UserAllRss.SelectMany(s => s.tags).Distinct().ToList();

            return View(new ViewModel.BaseContent.BaseContentManageVM()
            {
                rssid = rssModel._id,
                rsstitle = rssModel.title,
                categories = UserCategories,
                rsses = UserAllRss,
                sites = UserAllSite,
                tags = UserTags,
            });
        }
        public async Task<ActionResult> item(string id)
        {
            var model = await contentRepository.ContentInfo(id);
            return View(model);
        }
    }
}