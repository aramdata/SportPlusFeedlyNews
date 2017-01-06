using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.Service
{
    public class CacheService
    {
        public static string InProccessKeyName = "InProccessFileName";
        public static string VideoConvertProgressKeyName = "VideoConvertProgress";
        public static string VideoConvertLogReceivKeyName = "VideoConvertLogReceiv";

        public void AddToCache(string key,string value)
        {
            try
            {
                HttpRuntime.Cache[key] = value;
            }
            catch { }
        }

        public string GetCache(string key)
        {
            try
            {
                var model = HttpRuntime.Cache[key];
                if (model == null)
                    return string.Empty;
                else
                    return model.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public void RemoveCache(string key) {
            try
            {
                if (HttpRuntime.Cache[key] != null)
                    HttpRuntime.Cache[key]=string.Empty;
            }
            catch { }
        }

    }
}