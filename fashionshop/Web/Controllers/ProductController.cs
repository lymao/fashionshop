using AutoMapper;
using Common;
using Data.Repositories;
using Model.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Web.Infrastructure.Core;
using Web.Models;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        IProductCategoryService _productCategoryService;
        IProductService _productService;
        ICommonService _commonService;
        public ProductController(ICommonService commonService, IProductCategoryService productCategoryService, IProductService productService)
        {
            this._productCategoryService = productCategoryService;
            this._productService = productService;
            this._commonService = commonService;
        }
        // GET: Product
        public ActionResult Index(int id, int page = 1, string sort = "")
        {
            var category = _productCategoryService.GetById(id);
            ViewBag.category = Mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
            int totalRow = 0;
            int pageSize = int.Parse(ConfigHelper.GetByKey("pageSize"));
            var productModel = _productService.GetListProductByCategoryIdPaging(id, page, pageSize, sort, out totalRow);

            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)(totalRow / pageSize));
            var paginationSet = new PaginationSet<ProductViewModel>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPage + 1,// totalPage + 1; để hiển thị các sản phẩm lẻ không đủ một trang ở trang cuối cùng
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

        public JsonResult GetListProductByName(string keyword)
        {
            var model = _productService.GetListProductByName(keyword);
            return Json(new
            {
                data = model
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string keyword, int page = 1, string sort = "")

        {
            ViewBag.keyword = keyword;
            int pageSize = int.Parse(ConfigHelper.GetByKey("pageSize"));
            int totalRow = 0;
            var productModel = _productService.Search(keyword, page, pageSize, sort, out totalRow);
            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)(totalRow / pageSize));

            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPage + 1,// totalPage + 1; để hiển thị các sản phẩm lẻ không đủ một trang ở trang cuối cùng
                TotalCount = totalRow,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Items = productViewModel
            };
            return View(paginationSet);
        }

        public ActionResult Detail(int productId)
        {
            _productService.IncreaseView(productId);
            _productService.Save();

            // cookies
            var views = Request.Cookies[CommonConstants.CookiesView];
            if (views == null)
            {
                views = new HttpCookie(CommonConstants.CookiesView);
            }
            // bổ sung mặt hàng đã xem vào Cookies
            views.Values[productId.ToString()] = productId.ToString();
            // đặt thời gian tồn tại của Cookies
            views.Expires.AddHours(1);
            // gửi Cookies về Client để lưu lại
            Response.Cookies.Add(views);
            // lấy List<int> chứa mã hàng đã xem từ Cookies
            var keys = views.Values.AllKeys.Select(x => int.Parse(x)).ToList();
            var query=_productService.GetAll().Where(x => keys.Contains(x.ID));
            ViewBag.Views = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(query);

            var model = _productService.GetDetail(productId);
            var productViewModel = Mapper.Map<Product, ProductViewModel>(model);
            List<string> moreImages = new JavaScriptSerializer().Deserialize<List<string>>(productViewModel.MoreImages);
            ViewBag.MoreImages = moreImages;

            var topViewProductModel = _productService.GetViewProduct(4);
            var topViewProductViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(topViewProductModel);
            ViewBag.TopViewProducts = topViewProductViewModel;

            var relatedProduct = _productService.GetRelatedProduct(productId, 4);
            ViewBag.RelatedProducts = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(relatedProduct);

            var listTag = _productService.GetListTagByProductId(productId);
            ViewBag.Tag = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(listTag);

            return View(productViewModel);
        }

        public ActionResult ListProductByTag(string tagId, int page = 1, string sort = "")
        {
            var tag = _commonService.GetTagById(tagId);
            ViewBag.Tag = Mapper.Map<Tag, TagViewModel>(tag);

            int pageSize = int.Parse(ConfigHelper.GetByKey("pageSize"));
            int totalRow = 0;
            var model = _productService.GetListProductByTag(tagId, page, pageSize, sort, out totalRow);
            var productViewModel = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(model);

            int totalPage = (int)Math.Ceiling((double)(totalRow / pageSize));
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPage + 1,// totalPage + 1; để hiển thị các sản phẩm lẻ không đủ một trang ở trang cuối cùng
                TotalCount = totalRow,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Items = productViewModel
            };
            return View(paginationSet);
        }
    }
}