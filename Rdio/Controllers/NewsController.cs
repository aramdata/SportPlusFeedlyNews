using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rdio.Service;

namespace Rdio.Controllers
{
    public class NewsController : Controller
    {
        NewsService NewsService =new NewsService();
        public async Task<ActionResult> Service(int? CategoryId)
        {
            var m = await this.NewsService.PortalCategoriesAsync();
            var model=new ViewModel.News.ServiceVM
            {
                //Categories = (await NewsService.PortalCategories()).ToList()
            };

            if (CategoryId==null)
                return View("Home", model);
            return View();
        }

        public async Task<ActionResult> B(int? CategoryId)
        {
            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("userid", Util.Configuration.UserId),
                new Tuple<string, string>("CategoryId", "587089a4f61ebb1fb8395de8"),
                new Tuple<string, string>("BlockCode", "SPECIAL"),
                new Tuple<string, string>("Count", "20")

            };

            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetBlockNews", Params);
            var categories = Util.ApiUtility.GetServiceResult<Models.Content.NewsContent>(result);
            return View("Service");
        }
    }
}