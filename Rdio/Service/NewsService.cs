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

            CacheService CacheService = new CacheService();
            CacheService.AddToCache(Service.CacheService.PortalCategories, Categories);

            return Categories;

            //CacheService CacheService = new CacheService();
            //if (CacheService.GetCache(Service.CacheService.PortalCategories) == null)
            //{
            //    var Params = new List<Tuple<string, string>>
            //{
            //    new Tuple<string, string>("userid", Util.Configuration.UserId)
            //};
            //    var result = await Util.ApiUtility.HttpRequest("UserBlog/GetCategories", Params);
            //    var Categories = Util.ApiUtility.GetServiceResult<Models.ContentManager.Category>(result);
            //    //return Categories;
            //    CacheService.AddToCache(Service.CacheService.PortalCategories, Categories);
            //}
            //return CacheService.GetCache(Service.CacheService.PortalCategories) as List<Models.ContentManager.Category>;
        }

        public static List<Models.ContentManager.Category> PortalCategories()
        {
            //Depend On Call once PortalCategoriesAsync  Befor Call This Method ....

            CacheService CacheService = new CacheService();
            return CacheService.GetCache(Service.CacheService.PortalCategories) as List<Models.ContentManager.Category>;
        }

        public async Task<IEnumerable<Models.Content.NewsContent>>  GetBlockNews(string CategoryId,string BlockCode,int Count)
        {
            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("userid", Util.Configuration.UserId),
                new Tuple<string, string>("CategoryId", CategoryId),
                new Tuple<string, string>("BlockCode", BlockCode),
                new Tuple<string, string>("Count", Count.ToString())

            };

            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetBlockNews", Params);
            var News = Util.ApiUtility.GetServiceResult<Models.Content.NewsContent>(result);
            return News;
        }
    }
}