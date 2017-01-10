using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Rdio.Util
{
    public static class ApiUtility
    {
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
                client.BaseAddress = new Uri(Configuration.APIBaseUrl);
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

        public static IEnumerable<T> GetServiceResult<T>(string response)
        {
            var serviceResult = Util.Common.fromJSON<Domain.ServiceResult<T>>(response);
            return serviceResult.Data;
        }
    }

}