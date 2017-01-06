using System.Web.Mvc;
using System.Web.Routing;

namespace Rdio
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.RouteExistingFiles = true;

            routes.MapRoute(
               name: "admin",
               url: "admin",
               defaults: new { controller = "Admin", action = "Index"}
           );
            routes.MapRoute(
                name: "activeregister",
                url: "user/activeregister/{id}/{activationid}/{tmpActivationid}",
                defaults: new { controller = "User", action = "activeregister", id = UrlParameter.Optional, activationid = UrlParameter.Optional, tmpActivationid = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "resetenewpassword",
                url: "user/resetenewpassword/{id}/{activationid}/{tmpActivationid}",
                defaults: new { controller = "User", action = "resetenewpassword", id = UrlParameter.Optional, activationid = UrlParameter.Optional, tmpActivationid = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DownloadProductImage",
                url: "Shopping/DownloadProductImage/{id}/{pid}/{guid}",
                defaults: new { controller = "Shopping", action = "DownloadProductImage", id = UrlParameter.Optional, pid = UrlParameter.Optional, guid = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "photo",
               url: "content/product/{image}",
               defaults: new { controller = "Product", action = "AImage", image = UrlParameter.Optional }
           );
            routes.MapRoute(
         name: "productitem",
         url: "Product/Item/{aid}",
         defaults: new { controller = "Product", action = "Item", aid = UrlParameter.Optional }
     );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "News", action = "Service", id = UrlParameter.Optional }
            );
        }
    }
}
