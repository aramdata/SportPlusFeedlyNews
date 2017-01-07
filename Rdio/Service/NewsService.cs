using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Rdio.Service
{
    public class NewsService
    {

        public async Task<IEnumerable<Models.ContentManager.Category>> PortalCategoriesAsync()
        {
            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("userid", Util.Configuration.UserId)
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetCategories", Params);
            var Categories = Util.ApiUtility.GetServiceResult<Models.ContentManager.Category>(result);
            return Categories;
        }

        public static List<Models.ContentManager.Category> PortalCategories()
        {
            CacheService CacheService = new CacheService();
            if (CacheService.GetCache(Service.CacheService.PortalCategories) == null)
            {
                NewsService NewsService=new NewsService();
                var model = NewsService.PortalCategoriesAsync().Result.ToList();
                CacheService.AddToCache(Service.CacheService.PortalCategories,model);
            }
            return CacheService.GetCache(Service.CacheService.PortalCategories) as List<Models.ContentManager.Category>;
        }
    }
}