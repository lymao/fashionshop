using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Web.Infratructure.Core;
using AutoMapper;
using System.Collections;
using Model.Models;
using Web.Models;
using Web.Infrastructure.Core;
using Web.Infrastructure.Extensions;
using System.Web.Script.Serialization;

namespace Web.Api
{
    [RoutePrefix("api/slide")]
    public class SlideController : ApiControllerBase
    {
        ISlideService _slideService;
        public SlideController(IErrorService errorService, ISlideService slideService) : base(errorService)
        {
            this._slideService = slideService;
        }

        [Route("getbyid/{id:int}")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;
                var model = _slideService.GetSlideById(id);
                var responseData = Mapper.Map<Slide, SlideViewModel>(model);
                return response = request.CreateResponse(HttpStatusCode.OK, responseData);
            });
        }

        [Route("getall")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                var totalCount = 0;
                var model = _slideService.GetAll(keyword);
                totalCount = model.Count();
                var query = model.OrderBy(x => x.ID).Skip(page * pageSize).Take(pageSize);
                var responseData = Mapper.Map<IEnumerable<Slide>, IEnumerable<SlideViewModel>>(query);
                var paginationSet = new PaginationSet<SlideViewModel>
                {
                    Page = page,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize),
                    Items = responseData
                };
                var response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }

        [Route("create")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, SlideViewModel slideVM)
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
                    var newSlide = new Slide();
                    newSlide.UpdateSlide(slideVM);
                    _slideService.Add(newSlide);
                    _slideService.Save();
                    var responseData = Mapper.Map<Slide, SlideViewModel>(newSlide);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }
                return response;
            });

        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, SlideViewModel slideVM)
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
                    var dbSlide = _slideService.GetSlideById(slideVM.ID);
                    dbSlide.UpdateSlide(slideVM);
                    _slideService.Update(dbSlide);
                    _slideService.Save();
                    var responseData = Mapper.Map<Slide, SlideViewModel>(dbSlide);
                    response = request.CreateResponse(HttpStatusCode.OK, responseData);
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
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var oldSlide = _slideService.Delete(id);
                    _slideService.Save();

                    var responseData = Mapper.Map<Slide, SlideViewModel>(oldSlide);
                    response = request.CreateResponse(HttpStatusCode.OK, responseData);

                }
                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedSlides)
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
                    var listSlides = new JavaScriptSerializer().Deserialize<List<int>>(checkedSlides);
                    foreach (var item in listSlides)
                    {
                        _slideService.Delete(item);
                    }
                    _slideService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, listSlides.Count);

                }
                return response;
            });
        }


    }
}
