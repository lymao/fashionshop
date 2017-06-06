using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            routes.MapRoute(
                name: "Register",
                url: "dang-ky.html",
                defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional },
                namespaces:new string[] {"Web.Controllers"} 
            );

            routes.MapRoute(
                name: "Login",
                url: "dang-nhap.html",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
               name: "Search",
               url: "tim-kiem.html",
               defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
               namespaces: new string[] { "Web.Controllers" }
           );

            routes.MapRoute(
                    name: "Cart",
                    url: "gio-hang.html",
                    defaults: new { controller = "ShoppingCart", action = "Index", id = UrlParameter.Optional },
                    namespaces: new string[] { "Web.Controllers" });

            routes.MapRoute(
                name: "Contact",
                url: "lien-he.html",
                defaults: new { controller = "ContactDetail", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
                name: "Account",
                url: "tai-khoan.html",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
                name: "ChangeAccount",
                url: "cap-nhat-tai-khoan.html",
                defaults: new { controller = "Account", action = "ChangeAccount", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
                name: "ChangePassword",
                url: "doi-mat-khau.html",
                defaults: new { controller = "Account", action = "ChangePassword", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
                name: "Product",
                url: "{alias}.pc-{id}.html",
                defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
                name: "ProductDetail",
                url: "{alias}.p-{productId}.html",
                defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
                namespaces: new string[] { "Web.Controllers" }
            );

            routes.MapRoute(
                   name: "ProductByTag",
                   url: "tag/{tagId}.html",
                   defaults: new { controller = "Product", action = "ListProductByTag", tagId = UrlParameter.Optional },
                   namespaces: new string[] { "Web.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
