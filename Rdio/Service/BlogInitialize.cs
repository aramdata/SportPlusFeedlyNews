using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Rdio.Service
{
    public class BlogInitialize
    {
        Service.NewsService NewsService = new NewsService();
        private readonly Task<bool> _documentStore;

        public BlogInitialize()
        {
            _documentStore = Initialize();
        }
        private async Task<bool> Initialize()
        {
            await NewsService.PortalCategoriesAsync();
            return true;
        }
    }
}