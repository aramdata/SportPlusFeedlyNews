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
        public static string LegueTable = "LegueTable";
        public static string LegueFixture = "LegueFixture";
        public static string GetBlockNewsForAllCategories = "GetBlockNewsForAllCategories";
        public static string GetAparatChannelVideo = "GetAparatChannelVideo";


        public void AddToCache(string key, object value, DateTime? ExpireDate=null)
        {
            try
            {
                if (ExpireDate == null)
                    HttpRuntime.Cache.Insert(key, value);
                else
                    HttpRuntime.Cache.Insert(key, value, null, ExpireDate.Value, TimeSpan.Zero);
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

        public void RemoveCache(string key)
        {
            try
            {
                if (HttpRuntime.Cache[key] != null)
                    HttpRuntime.Cache[key] = string.Empty;
            }
            catch { }
        }

    }
}