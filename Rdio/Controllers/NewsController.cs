using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Rdio.Controllers
{
    public class NewsController : Controller
    {
        public async Task<ActionResult> Service(int? CategoryId)
        {
            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("userid", Util.Configuration.UserId)
            };

            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetCategories", Params);
            if (CategoryId == null)
                CategoryId = 5;
            var categories = Util.ApiUtility.GetServiceResult<Models.ContentManager.Category>(result);
            return View();
        }

        public async Task<ActionResult> B(int? CategoryId)
        {
            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("userid", Util.Configuration.UserId),
                new Tuple<string, string>("CategoryId", "586d50862810091d046d7faa"),
                new Tuple<string, string>("BlockCode", "SELECTED"),
                new Tuple<string, string>("Count", "20")

            };

            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetBlockNews", Params);
            var categories = Util.ApiUtility.GetServiceResult<Models.Content.NewsContent>(result);
            return View("Service");
        }
    }
}