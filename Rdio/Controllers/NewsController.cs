using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rdio.Models.ContentManager;
using Rdio.Models.Legue;
using Rdio.Service;
using Rdio.Util;
using Rdio.ViewModel.News;

namespace Rdio.Controllers
{
    public class NewsController : Controller
    {
        NewsService NewsService =new NewsService();
        LegueService LegueService = new LegueService();

        public async Task<ActionResult> Service(string CategoryId)
        {
            try
            {

                if (CategoryId == null)
                {
                    //var Category = NewsService.PortalCategories().FirstOrDefault();
                    //var Blocks = Category.blocks.Where(q => q.blockrssbind != null && q.blockrssbind.Any()).ToList();
                    var BlocksNews = new List<BlockNewsVM>();
                    BlocksNews.AddRange(await NewsService.GetBlockNewsForAllCategories("TOP",4));
                    BlocksNews.AddRange(await NewsService.GetBlockNewsForAllCategories("LATESTNEWS", 4));

                    //foreach (var block in Blocks)
                    //    BlocksNews.Add(new BlockNewsVM
                    //    {
                    //        Block = block,
                    //        News = (await this.NewsService.GetBlockNews(Category._id, block.code, 20)).ToList()
                    //    });

                    var Legues = new List<Varzesh3Legue>();
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.BartarIran));
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.BartarEnglish));
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.Azadegan));
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.Laliga));
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.Bondes));
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.SerieA));
                    Legues.Add(await LegueService.GetFootbalLegue(Configuration.FootbalLegue.Leshampione));

                    var LeguesFixture = new List<Varzesh3LegueFixture>();
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.BartarIran));
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.BartarEnglish));
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.Azadegan));
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.Laliga));
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.Bondes));
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.SerieA));
                    LeguesFixture.Add(await LegueService.GetFootbalLegueFixture(Configuration.FootbalLegue.Leshampione));

                    var model = new ViewModel.News.ServiceVM
                    {
                        Category = new Category(),
                        BlockNews = BlocksNews,
                        FootbalLegues = Legues,
                        FootbalLeguesFixture = LeguesFixture
                    };
                    return View("Home", model);

                }
                else
                {
                    var Category = NewsService.PortalCategories().FirstOrDefault(q => q._id == CategoryId); 
                    var Blocks = Category.blocks.Where(q => q.blockrssbind != null && q.blockrssbind.Any()).ToList();
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
                        BlockNews = BlocksNews,
                        //FootbalLegues = Legues,
                        //FootbalLeguesFixture = LeguesFixture
                    };
                    return View("Service", model);

                }
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