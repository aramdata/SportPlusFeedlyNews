using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Rdio.Domain;
using Rdio.Models.ContentManager;
using Rdio.Repository;

namespace Rdio.Api
{
    public class UserBlogController : ApiController
    {
        Repository.ContentManagerRepository ContentManagerRepository = new Repository.ContentManagerRepository();

        [Route("api/UserBlog/GetCategories")]
        public async Task<ServiceResult<Models.ContentManager.Category>> GetCategories([FromUri]ViewModel.UserBlog.GetCategoriesVM model)
        {
            try
            {
                var res = await ContentManagerRepository.GetUserCategories(model.userid);
                return new ServiceResult<Category>
                {
                    Data = res,
                    ServiceResultStatus = (int)Rdio.Util.Common.ServiceResultStatus.OK,
                    ServiceResultMassage = Util.Common.ServiceResultMessage.OKMessage.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult <Category>
                {
                    Data =null,
                    ServiceResultStatus = (int)Rdio.Util.Common.ServiceResultStatus.Error,
                    ServiceResultMassage = ex.GetBaseException().Message
                };
            }
        }
    }
}
