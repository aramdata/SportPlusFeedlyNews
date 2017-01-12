using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rdio.Service;
using Rdio.ViewModel.News;

namespace Rdio.Controllers
{
    public class NewsController : Controller
    {
        NewsService NewsService =new NewsService();
        public async Task<ActionResult> Service(string CategoryId)
        {
            try
            {
                
                var Category = new Models.ContentManager.Category();
                Category = string.IsNullOrWhiteSpace(CategoryId) ? NewsService.PortalCategories().FirstOrDefault() : NewsService.PortalCategories().FirstOrDefault(q => q._id == CategoryId);
                var Blocks = Category.blocks.Where(q =>q.blockrssbind!=null && q.blockrssbind.Any()).ToList();
                var BlocksNews = new List<BlockNewsVM>();
                foreach (var block in Blocks)
                    BlocksNews.Add(new BlockNewsVM
                    {
                        Block = block,
                        News = (await this.NewsService.GetBlockNews(Category._id, block.code, 20)).ToList()
                    });

                var model = new ViewModel.News.ServiceVM
                {
                    Category = Category,
                    BlockNews = BlocksNews
                };

                if (CategoryId == null)
                    return View("Home", model);
                return View("Service", model);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public async Task<ActionResult> Item(string ContentId)
        {
            var NewsItem =await this.NewsService.GetNewsInfo(ContentId);
            var Category = Rdio.Service.NewsService.NewsDefaultCategory(NewsItem.rssid);
            var BlocksNews = new List<BlockNewsVM>();
            BlocksNews.Add(new BlockNewsVM
            {
                Block = new Models.ContentManager.Block {code = string.Empty},
                News = (await this.NewsService.GetBlockNews(Category._id, string.Empty, 20)).ToList()
            });


            var model = new ViewModel.News.NewsVM
            {
                Titr = NewsItem.titr,
                Rotitr = NewsItem.rotitr,
                Lead = NewsItem.lead,
                Content = NewsItem.content,
                Image = NewsItem.image,
                Tags = NewsItem.tags,
                Category = Category,
                BlockNews = BlocksNews,
                DateTicks = NewsItem.createdateticks
            };
            return View(model);
        }

    }
}