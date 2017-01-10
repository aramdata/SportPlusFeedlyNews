using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Rdio.Models.ContentManager;

namespace Rdio.Util
{
    public class Configuration
    {
        public static string APIBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];
        public static string UserId => System.Configuration.ConfigurationManager.AppSettings["UserId"];

        public enum HttpRequestType
        {
            Get = 0,
            Post = 1
        }
        public static string GetPhysicalPath(string path) {
            return System.Web.Hosting.HostingEnvironment.MapPath(path);
        }

        public static List<Models.ContentManager.Block> AllBlocks()
        {
            return new List<Block>()
            {
                new Block() {title = "خبر برتر",code = "TOP"},
                new Block() {title = "ویژه",code = "SPECIAL"},
                new Block() {title = "آخرین اخبار",code = "LATESTNEWS"},
                new Block() {title = "برگزیده",code = "SELECTED"},
            };
        }
        public enum ContentType
        {
            [Description("خبر")]
            News = 0,
            [Description("فروشگاه")]
            Shopping = 1
        }
    }
}