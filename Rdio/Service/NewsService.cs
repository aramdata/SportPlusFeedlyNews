using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Rdio.Models.Content;
using Rdio.Models.ContentManager;
using Rdio.ViewModel.News;

namespace Rdio.Service
{
    public class NewsService
    {
        CacheService CacheService = new CacheService();

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

        public async Task<List<Models.Content.NewsContent>>  GetBlockNews(string CategoryId,string BlockCode,int Count)
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
            if(News!=null)
                return News.ToList();
            return new List<NewsContent>();
        }
        public async Task<List<BlockNewsVM>> GetBlockNewsForAllCategories(string BlockCode, int Count)
        {
            var CachKey = $"{Service.CacheService.GetBlockNewsForAllCategories}_{BlockCode}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as List<BlockNewsVM>;

            var result=new List<BlockNewsVM>();
            foreach (var Category in PortalCategories())
            {
                result.Add(new BlockNewsVM
                {
                    News = await GetBlockNews(Category._id, BlockCode, Count),
                    Category = Category,
                    Block = BlockInfo(Category._id,BlockCode)
                });
            }

            CacheService.AddToCache(CachKey,result,DateTime.Now.AddMinutes(10));
            return result;
        }


        public async Task<Models.Content.NewsContent> GetNewsInfo(string ContentId)
        {
            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("ContentId", ContentId)
            };

            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetNewsInfo", Params);
            var News = Util.ApiUtility.GetServiceResult<Models.Content.NewsContent>(result);
            return News.Any() ? News.FirstOrDefault():new Models.Content.NewsContent();
        }
        public Models.ContentManager.Block BlockInfo(string CategoryId,string BlockCode)
        {
            return
                PortalCategories()
                    .FirstOrDefault(q => q._id == CategoryId)
                    .blocks.FirstOrDefault(q => q.code == BlockCode);
        }

        public static Models.ContentManager.Category NewsDefaultCategory(string ContentId)
        {
            return PortalCategories().FirstOrDefault(q => q.blocks.Any(x => x.blockrssbind.Any(r => r == ContentId)));
        }


    }
}