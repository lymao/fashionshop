using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Web.Infratructure.Core;
using Web.Models;
using AutoMapper;
using Model.Models;
using Web.Infrastructure.Core;

namespace Web.Api
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiControllerBase
    {
        IOrderService _orderService;
        public OrderController(IErrorService errorService,IOrderService orderService) : base(errorService)
        {
            this._orderService = orderService;
        }

        [Route("getlistorder")]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, string startDate, string endDate, string filter, string pamentStatus, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                HttpResponseMessage response = null;
                var query = _orderService.GetList(startDate, endDate, filter, pamentStatus, page, pageSize, out totalRow);
                List<OrderViewModel> model = Mapper.Map<List<Order>, List<OrderViewModel>>(query);
                PaginationSet<OrderViewModel> pagedSet = new PaginationSet<OrderViewModel>()
                {
                    Page = page,
                    TotalCount=totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                    Items = model
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }
    }
}
