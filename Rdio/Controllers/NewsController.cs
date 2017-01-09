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
            var Category = NewsService.PortalCategories().FirstOrDefault();
            var Blocks = Category.blocks.Where(q => q.blockrssbind.Any()).ToList();

            var BlocksNews = new List<Tuple<Models.ContentManager.Block, List<Models.Content.NewsContent>>>();
            foreach (var block in Blocks)
            {
                BlocksNews.Add(new Tuple<Models.ContentManager.Block, List<Models.Content.NewsContent>>
                    (block, 
                    (await this.NewsService.GetBlockNews(Category._id, block.code, 20)).ToList()
                    ));
            }

            var model =new ViewModel.News.ServiceVM
            {
                Categories = Category,
                Blocks = Blocks,
                BlockNews= BlocksNews
            };

            if (CategoryId==null)
                return View("Home", model);
            return View();
        }
    }
}