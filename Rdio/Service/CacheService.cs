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
        public static string PortalCategories = "PortalCategories";

        public void AddToCache(string key,object value)
        {
            try
            {
                HttpRuntime.Cache[key] = value;
            }
            catch { }
        }

        public object GetCache(string key)
        {
            try
            {
                var model = HttpRuntime.Cache[key];
                if (model == null)
                    return null;
                else
                    return model;
            }
            catch (Exception ex)
            {
                return null;
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