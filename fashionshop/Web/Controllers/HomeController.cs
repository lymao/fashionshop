using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult HeaderPartial()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
    }
}