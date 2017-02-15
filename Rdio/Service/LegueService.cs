using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Rdio.Models.Legue;

namespace Rdio.Service
{
    public class LegueService
    {
        CacheService CacheService = new CacheService();

        public async Task<Models.Legue.Varzesh3Legue> GetFootbalLegue(Rdio.Util.Configuration.FootbalLegue FootbalLegue)
        {
            var CachKey = $"{Service.CacheService.FootballLegueTable}_{(int) FootbalLegue}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as Models.Legue.Varzesh3Legue;

            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("FootbalLegueId", ((int)FootbalLegue).ToString())
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetFootbalLegue", Params);
            var Legue = Util.ApiUtility.GetServiceResult<Models.Legue.Varzesh3Legue>(result);
            if (Legue != null && Legue.Any())
            {
                CacheService.AddToCache(CachKey, Legue.FirstOrDefault(), DateTime.Now.AddMinutes(30));
                return Legue.FirstOrDefault();
            }

            return new Varzesh3Legue();
        }

        public async Task<Models.Legue.Varzesh3LegueFixture> GetFootbalLegueFixture(Rdio.Util.Configuration.FootbalLegue FootbalLegue)
        {
            var CachKey = $"{Service.CacheService.FootballLegueFixture}_{(int)FootbalLegue}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as Models.Legue.Varzesh3LegueFixture;

            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("FootbalLegueId", ((int)FootbalLegue).ToString())
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetFootbalLegueFixture", Params);
            var Legue = Util.ApiUtility.GetServiceResult<Models.Legue.Varzesh3LegueFixture>(result);
            if (Legue != null && Legue.Any())
            {
                CacheService.AddToCache(CachKey, Legue.FirstOrDefault(), DateTime.Now.AddMinutes(30));
                return Legue.FirstOrDefault();
            }

            return new Varzesh3LegueFixture();
        }


        public async Task<Models.Legue.Varzesh3Legue> GetVollybalLegue(Rdio.Util.Configuration.VollybalLegue VollybalLegue)
        {
            var CachKey = $"{Service.CacheService.VollyballLegueTable}_{(int)VollybalLegue}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as Models.Legue.Varzesh3Legue;

            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("FootbalLegueId", ((int)VollybalLegue).ToString())
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetVollybalLegue", Params);
            var Legue = Util.ApiUtility.GetServiceResult<Models.Legue.Varzesh3Legue>(result);
            if (Legue != null && Legue.Any())
            {
                CacheService.AddToCache(CachKey, Legue.FirstOrDefault(), DateTime.Now.AddMinutes(30));
                return Legue.FirstOrDefault();
            }

            return new Varzesh3Legue();
        }

        public async Task<Models.Legue.Varzesh3LegueFixture> GetVollybalLegueFixture(Rdio.Util.Configuration.VollybalLegue VollybalLegue)
        {
            var CachKey = $"{Service.CacheService.VollyballLegueFixture}_{(int)VollybalLegue}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as Models.Legue.Varzesh3LegueFixture;

            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("FootbalLegueId", ((int)VollybalLegue).ToString())
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetVollybalLegueFixture", Params);
            var Legue = Util.ApiUtility.GetServiceResult<Models.Legue.Varzesh3LegueFixture>(result);
            if (Legue != null && Legue.Any())
            {
                CacheService.AddToCache(CachKey, Legue.FirstOrDefault(), DateTime.Now.AddMinutes(30));
                return Legue.FirstOrDefault();
            }

            return new Varzesh3LegueFixture();
        }


        public async Task<Models.Legue.Varzesh3Legue> GetBasketbalLegue(Rdio.Util.Configuration.BasketbalLegue BasketbalLegue)
        {
            var CachKey = $"{Service.CacheService.BasketballLegueTable}_{(int)BasketbalLegue}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as Models.Legue.Varzesh3Legue;

            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("FootbalLegueId", ((int)BasketbalLegue).ToString())
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetBasketbalLegue", Params);
            var Legue = Util.ApiUtility.GetServiceResult<Models.Legue.Varzesh3Legue>(result);
            if (Legue != null && Legue.Any())
            {
                CacheService.AddToCache(CachKey, Legue.FirstOrDefault(), DateTime.Now.AddMinutes(30));
                return Legue.FirstOrDefault();
            }

            return new Varzesh3Legue();
        }

        public async Task<Models.Legue.Varzesh3LegueFixture> GetBasketbalLegueFixture(Rdio.Util.Configuration.BasketbalLegue BasketbalLegue)
        {
            var CachKey = $"{Service.CacheService.BasketballLegueFixture}_{(int)BasketbalLegue}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as Models.Legue.Varzesh3LegueFixture;

            var Params = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("FootbalLegueId", ((int)BasketbalLegue).ToString())
            };
            var result = await Util.ApiUtility.HttpRequest("UserBlog/GetBasketbalLegueFixture", Params);
            var Legue = Util.ApiUtility.GetServiceResult<Models.Legue.Varzesh3LegueFixture>(result);
            if (Legue != null && Legue.Any())
            {
                CacheService.AddToCache(CachKey, Legue.FirstOrDefault(), DateTime.Now.AddMinutes(30));
                return Legue.FirstOrDefault();
            }

            return new Varzesh3LegueFixture();
        }
    }
}