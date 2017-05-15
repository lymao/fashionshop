using AutoMapper;
using Model.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        IProductCategoryService _productCategoryService;
        ICommonService _commonService;
        ISlideService _slideService;
        public HomeController(IProductCategoryService productCategoryService,ICommonService commonService,ISlideService slideService)
        {
            this._productCategoryService = productCategoryService;
            this._commonService = commonService;
            this._slideService = slideService;
        }
        // GET: Home
        public ActionResult Index()
        {
            var homeViewModel = new HomeViewModel();


            return View(homeViewModel);
        }
        [ChildActionOnly]
        public ActionResult HeaderPartial()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult MenuPartial()
        {
            var model = _productCategoryService.GetAll();
            var listProductCategory = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);
            return PartialView(listProductCategory);
        }
        [ChildActionOnly]
        public ActionResult FooterPartial()
        {
            var model = _commonService.GetFooter();
            var listFooter = Mapper.Map<Footer, FooterViewModel>(model);

            return PartialView(listFooter);
        }

        [ChildActionOnly]
        public ActionResult SlidePartial()
        {
            var slideModel = _slideService.GetSlides();
            var slideView = Mapper.Map<IEnumerable<Slide>, IEnumerable<SlideViewModel>>(slideModel);
            return PartialView(slideView);
        }
    }
}