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
using Web.Infrastructure.Extensions;
using System.Web.Script.Serialization;
using Common;
using System.Web;
using System.IO;
using OfficeOpenXml;
using Shop.Common;

namespace Web.Api
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiControllerBase
    {
        IOrderService _orderService;
        IProductSizeService _productSizeService;
        public OrderController(IProductSizeService productSizeService, IErrorService errorService, IOrderService orderService) : base(errorService)
        {
            this._orderService = orderService;
            this._productSizeService = productSizeService;
        }

        [Route("getlistorder")]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, string startDate, string endDate, string filter, string pamentStatus, int page, int pageSize)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow;
                HttpResponseMessage response = null;
                var query = _orderService.GetList(startDate, endDate, filter, pamentStatus, page, pageSize, out totalRow);
                List<OrderViewModel> model = Mapper.Map<List<Order>, List<OrderViewModel>>(query);
                PaginationSet<OrderViewModel> pagedSet = new PaginationSet<OrderViewModel>()
                {
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                    Items = model
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }

        [Route("getorder/{id:int}")]
        public HttpResponseMessage GetOrder(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                try
                {
                    HttpResponseMessage response = null;
                    var query = _orderService.GetOrder(id);
                    OrderViewModel model = Mapper.Map<Order, OrderViewModel>(query);
                    response = request.CreateResponse(HttpStatusCode.OK, model);
                    return response;
                }
                catch (Exception ex)
                {

                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }

            });
        }

        [Route("getallproduct")]
        [HttpGet]
        public HttpResponseMessage GetAllProduct(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var query = _orderService.GetAllProduct();
                var model = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(query);
                response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }

        [Route("geproductbyid")]
        [HttpGet]
        public HttpResponseMessage GetProductById(HttpRequestMessage request, int productId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var query = _orderService.GetProductById(productId);
                var model = Mapper.Map<Product, ProductViewModel>(query);
                response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }

        [Route("createorder")]
        [HttpPost]
        public HttpResponseMessage CreateOrder(HttpRequestMessage request, OrderViewModel orderVM)
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
                    try
                    {
                        var newOrder = new Order();
                        newOrder.UpdateOrder(orderVM);
                        var listOrderDetails = new List<OrderDetail>();
                        bool isEnough = true;
                        foreach (var item in orderVM.OrderDetails)
                        {
                            listOrderDetails.Add(new OrderDetail()
                            {
                                ProductID = item.ProductID,
                                SizeId = item.SizeID,
                                Quantity = item.Quantity,
                                Price = item.Price
                            });
                            isEnough = _productSizeService.SellProduct(item.ProductID, item.SizeID, item.Quantity);
                            if (isEnough == false)
                            {
                                break;
                            }
                        }
                        if (isEnough)
                        {
                            newOrder.OrderDetails = listOrderDetails;
                            var result = _orderService.CreateAll(newOrder);
                            var model = Mapper.Map<Order, OrderViewModel>(result);
                            return request.CreateResponse(HttpStatusCode.OK, model);
                        }
                        else
                        {
                            response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Số lượng trong kho không đủ");
                        }
                    }
                    catch (Exception ex)
                    {

                        response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                    }

                }
                return response;
            });
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            try
            {
                _orderService.Delete(id);
                _orderService.Save();
                return request.CreateResponse(HttpStatusCode.OK, id);
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }

        [Route("deletemulti")]
        [HttpDelete]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedOrders)
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
                    var listProduct = new JavaScriptSerializer().Deserialize<List<int>>(checkedOrders);
                    foreach (var item in listProduct)
                    {
                        _orderService.Delete(item);
                    }

                    _orderService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, listProduct.Count);
                }

                return response;
            });
        }

        [Route("exportExcel/{id}")]
        [HttpGet]
        public HttpResponseMessage ExportOrder(HttpRequestMessage request, int id)
        {
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string documentName = GenerateOrder(id);
            if (!string.IsNullOrEmpty(documentName))
            {
                return request.CreateResponse(HttpStatusCode.OK, folderReport + "/" + documentName);
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error export");

            }

            // If something fails or somebody calls invalid URI, throw error.
        }
        #region Export to Excel
        private string GenerateOrder(int orderId)
        {
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            // Template File
            string templateDocument = HttpContext.Current.Server.MapPath("~/Templates/OrderTemplate.xlsx");
            string documentName = string.Format("Order-{0}-{1}.xlsx", orderId, DateTime.Now.ToString("yyyyMMddhhmmsss"));
            string fullPath = Path.Combine(filePath, documentName);
            // Results Output
            MemoryStream output = new MemoryStream();
            try
            {
                // Read Template
                using (FileStream templateDocumentStream = File.OpenRead(templateDocument))
                {
                    // Create Excel EPPlus Package based on template stream
                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        // Grab the sheet with the template, sheet name is "BOL".
                        ExcelWorksheet sheet = package.Workbook.Worksheets["Order"];
                        // Data Acces, load order header data.
                        var order = _orderService.GetOrder(orderId);

                        // Insert customer data into template
                        sheet.Cells[4, 1].Value = "Tên khách hàng: " + order.CustomerName;
                        sheet.Cells[5, 1].Value = "Địa chỉ: " + order.CustomerAddress;
                        sheet.Cells[6, 1].Value = "Điện thoại: " + order.CustomerMobile;
                        // Start Row for Detail Rows
                        int rowIndex = 9;

                        // load order details
                        var orderDetails = _orderService.GetOrderDetails(orderId);
                        int count = 1;
                        foreach (var orderDetail in orderDetails)
                        {
                            // Cell 1, Carton Count
                            sheet.Cells[rowIndex, 1].Value = count.ToString();
                            // Cell 2, Order Number (Outline around columns 2-7 make it look like 1 column)
                            sheet.Cells[rowIndex, 2].Value = orderDetail.Product.Name;
                            // Cell 8, Weight in LBS (convert KG to LBS, and rounding to whole number)
                            sheet.Cells[rowIndex, 3].Value = orderDetail.Quantity.ToString();

                            sheet.Cells[rowIndex, 4].Value = orderDetail.Price.ToString("N0");
                            sheet.Cells[rowIndex, 5].Value = (orderDetail.Price * orderDetail.Quantity).ToString("N0");
                            // Increment Row Counter
                            rowIndex++;
                            count++;
                        }
                        double total = (double)(orderDetails.Sum(x => x.Quantity * x.Price));
                        sheet.Cells[24, 5].Value = total.ToString("N0");

                        var numberWord = "Thành tiền (viết bằng chữ): " + NumberHelper.ToString(total);
                        sheet.Cells[26, 1].Value = numberWord;
                        if (order.CreatedDate.HasValue)
                        {
                            var date = order.CreatedDate.Value;
                            sheet.Cells[28, 3].Value = "Ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year;

                        }
                        package.SaveAs(new FileInfo(fullPath));
                    }
                    return documentName;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        #endregion
    }
}
