using AutoMapper;
using Common;
using Model.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Infrastructure.Core;
using Web.Models;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        IProductCategoryService _productCategoryService;
        IProductService _productService;
        public ProductController(IProductCategoryService productCategoryService,IProductService productService)
        {
            this._productCategoryService = productCategoryService;
            this._productService = productService;
        }
        // GET: Product
        public ActionResult Index(int id,int page = 1,string sort ="")
        {
            var category = _productCategoryService.GetById(id);
            ViewBag.category = Mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
            int totalRow = 0;
            int pageSize = int.Parse(ConfigHelper.GetByKey("pageSize"));
            var productModel = _productService.GetListProductByCategoryIdPaging(id, page,pageSize, sort, out totalRow);

            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)(totalRow / pageSize));
            var paginationSet = new PaginationSet<ProductViewModel>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPage,
                TotalCount = totalRow,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Items = productViewModel
            };

            return View(paginationSet);
        }

        [ChildActionOnly]
        public ActionResult CategoryPartial()
        {
            var model = _productCategoryService.GetAll();
            var listProductCategory = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);
            return PartialView(listProductCategory);
        }
    }
}