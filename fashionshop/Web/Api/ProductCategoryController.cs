using AutoMapper;
using Model.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Web.Infrastructure.Core;
using Web.Infrastructure.Extensions;
using Web.Infratructure.Core;
using Web.Models;

namespace Web.Api
{
    [RoutePrefix("api/productcategory")]
    public class ProductCategoryController : ApiControllerBase
    {
        IProductCategoryService _productCategoryService;
        public ProductCategoryController(IErrorService errorService, IProductCategoryService productCategoryService) : base(errorService)
        {
            this._productCategoryService = productCategoryService;
        }
        [Route("getallparents")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productCategoryService.GetAll().Where(x => x.Status == true);

                var responseData = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productCategoryService.GetById(id);
                var responseData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
             {
                 int totalCount = 0;
                 var model = _productCategoryService.GetAll(keyword);
                 totalCount = model.Count();
                 var query = model.OrderBy(x => x.ID).Skip(page * pageSize).Take(pageSize);
                 var responseData = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(query);
                 var paginationSet = new PaginationSet<ProductCategoryViewModel>()
                 {
                     Items = responseData,
                     Page = page,
                     TotalCount = totalCount,
                     TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize)
                 };
                 var response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                 return response;
             });
        }
        [Route("create")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductCategoryViewModel productCategoryVM)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newProductCategory = new ProductCategory();
                    newProductCategory.UpdateProductCategory(productCategoryVM);
                    _productCategoryService.Add(newProductCategory);
                    _productCategoryService.Save();

                    var responseData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(newProductCategory);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;

            });
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request,ProductCategoryViewModel productCategoryVM)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbProductCategory = _productCategoryService.GetById(productCategoryVM.ID);
                    dbProductCategory.UpdateProductCategory(productCategoryVM);
                    dbProductCategory.UpdatedDate = DateTime.Now;
                    _productCategoryService.Update(dbProductCategory);
                    _productCategoryService.Save();

                    var responseData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(dbProductCategory);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }
                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
             {
                 HttpResponseMessage response = null;
                 var oldProductCategory = _productCategoryService.Delete(id);
                 _productCategoryService.Save();

                 var responseData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(oldProductCategory);
                 response = request.CreateResponse(HttpStatusCode.OK, responseData);
                 return response;
             });
        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedProductCategories)// <= truyen vao mot chuoi
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(checkedProductCategories);// Deserializer chuỗi ra thành list<int>
                foreach(var item in listProductCategory)
                {
                    _productCategoryService.Delete(item);
                }
                _productCategoryService.Save();

                response = request.CreateResponse(HttpStatusCode.OK, listProductCategory.Count);
                return response;
            });
        }

    }
}
