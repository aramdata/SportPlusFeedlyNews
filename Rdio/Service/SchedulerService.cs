using MongoDB.Bson;
using Rdio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Rdio.Service
{
    public class SchedulerService
    {
        string CacheKey = "SchedulerInProccess";
        public enum SchedulerStat {
            inProccess=0,
            idle=1
        }
        public bool IsInProccess()
        {
            try
            {
                var model = HttpRuntime.Cache[CacheKey];
                if (model == null)
                    return false;
                else
                    return int.Parse(model.ToString()) == (int)SchedulerStat.inProccess ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void SetScheduleInProccess(SchedulerStat stat)
        {
            try
            {
                var model = HttpRuntime.Cache[CacheKey];
                HttpRuntime.Cache[CacheKey] = (int)stat;
            }
            catch { }
        }
    }
}