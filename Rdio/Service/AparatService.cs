using AngleSharp.Parser.Html;
using Rdio.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Rdio.Service
{
    public class AparatService
    {
        CacheService CacheService = new CacheService();

        public async Task<List<Models.Aparat.Video>> GetChannelVideo(string ChannelName)
        {
            var CachKey = $"{Service.CacheService.GetAparatChannelVideo}_{ChannelName}";

            if (CacheService.GetCache(CachKey) != null)
                return CacheService.GetCache(CachKey) as List<Models.Aparat.Video>;

            var model = new List<Models.Aparat.Video>();
            var result = await HttpRequest(ChannelName, new List<Tuple<string, string>>());

            var ChannelVidoePattern = "#block-grid-video-list li > div > div:first-of-type a";
            var FileFromUrlRegex = @"http[s]?(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\(\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+";

            var parser = new HtmlParser();
            var document = parser.Parse(result);
            var videoItems = document.QuerySelectorAll(ChannelVidoePattern);
            foreach (var videoItem in videoItems)
            {
                var VideoPicture = Regex.Match(videoItem.GetAttribute("style"), FileFromUrlRegex);
                var VideoPath = videoItem.GetAttribute("href");
                result= await HttpRequest(VideoPath, new List<Tuple<string, string>>());
                parser = new HtmlParser();
                document = parser.Parse(result);

                var MainVideoPath = document.QuerySelectorAll("li.download-link a").LastOrDefault().GetAttribute("href");
                //var MainVideoPath = MainVideo.GetAttribute("src");
                var VideoTitle= document.QuerySelector("h1.vone__title").InnerHtml;

                model.Add(new Models.Aparat.Video {
                    VideoPath= MainVideoPath,
                    Title= VideoTitle,
                    VideoImagePath= VideoPicture!=null ? VideoPicture.Value.Replace(");","") : string.Empty
                });
            }
            CacheService.AddToCache(CachKey, model, DateTime.Now.AddMinutes(30));
            return model;
        }

        public static async Task<string> HttpRequest(string url, List<Tuple<string, string>> Parames, Configuration.HttpRequestType RequestType = Configuration.HttpRequestType.Get)
        {
            var result = string.Empty;
            if (RequestType == Configuration.HttpRequestType.Get)
                foreach (var param in Parames)
                {
                    if (Parames.IndexOf(param) == 0)
                        url += $"?{param.Item1}={param.Item2}";
                    else
                        url += $"&{param.Item1}={param.Item2}";
                }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://www.aparat.com");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }

                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "fail");

            }

            return result;
        }
    }
}