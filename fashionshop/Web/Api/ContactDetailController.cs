using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Web.Infratructure.Core;
using AutoMapper;
using Model.Models;
using Web.Models;
using Web.Infrastructure.Core;
using Web.Infrastructure.Extensions;
using System.Web.Script.Serialization;

namespace Web.Api
{
    [RoutePrefix("api/contact")]
    public class ContactDetailController : ApiControllerBase
    {
        IContactDetailService _contactDetailService;
        public ContactDetailController(IContactDetailService contactDetailService, IErrorService errorService) : base(errorService)
        {
            this._contactDetailService = contactDetailService;
        }

        [Route("getbyid/{id:int}")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;
                var model = _contactDetailService.GetContactDetailById(id);
                var responseData = Mapper.Map<ContactDetail, ContactDetailViewModel>(model);
                return response = request.CreateResponse(HttpStatusCode.OK, responseData);
            });
        }

        [Route("getall")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                var totalCount = 0;
                var model = _contactDetailService.GetAll(keyword);
                totalCount = model.Count();
                var query = model.OrderBy(x => x.ID).Skip(page * pageSize).Take(pageSize);
                var responseData = Mapper.Map<IEnumerable<ContactDetail>, IEnumerable<ContactDetailViewModel>>(query);
                var paginationSet = new PaginationSet<ContactDetailViewModel>
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
        public HttpResponseMessage Create(HttpRequestMessage request, ContactDetailViewModel contactDetailVM)
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
                    var newContactDetail = new ContactDetail();
                    newContactDetail.UpdateContactDetail(contactDetailVM);
                    _contactDetailService.Add(newContactDetail);
                    _contactDetailService.Save();
                    var responseData = Mapper.Map<ContactDetail, ContactDetailViewModel>(newContactDetail);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }
                return response;
            });

        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, ContactDetailViewModel contactDetailVM)
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
                    var dbContactDetail = _contactDetailService.GetContactDetailById(contactDetailVM.ID);
                    dbContactDetail.UpdateContactDetail(contactDetailVM);
                    _contactDetailService.Update(dbContactDetail);
                    _contactDetailService.Save();
                    var responseData = Mapper.Map<ContactDetail, ContactDetailViewModel>(dbContactDetail);
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
                    var oldContactDetail = _contactDetailService.Delete(id);
                    _contactDetailService.Save();

                    var responseData = Mapper.Map<ContactDetail, ContactDetailViewModel>(oldContactDetail);
                    response = request.CreateResponse(HttpStatusCode.OK, responseData);

                }
                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedContacts)
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
                    var listContactDetails = new JavaScriptSerializer().Deserialize<List<int>>(checkedContacts);
                    foreach (var item in listContactDetails)
                    {
                        _contactDetailService.Delete(item);
                    }
                    _contactDetailService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, listContactDetails.Count);

                }
                return response;
            });
        }
    }
}
