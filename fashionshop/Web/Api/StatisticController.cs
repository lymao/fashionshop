using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Web.Infratructure.Core;

namespace Web.Api
{
    [RoutePrefix("api/statistic")]
    public class StatisticController : ApiControllerBase
    {
        IStatisticService _statisticService;
        public StatisticController(IStatisticService statisticService, IErrorService errorService) : base(errorService)
        {
            this._statisticService = statisticService;
        }
        [Route("getrevenue")]
        public HttpResponseMessage GetRevenueStatistic(HttpRequestMessage request, string fromDate, string toDate)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _statisticService.GetRevenueStatistic(fromDate, toDate);
                HttpResponseMessage res = request.CreateResponse(HttpStatusCode.OK, model);
                return res;
            });
        }
    }
}
