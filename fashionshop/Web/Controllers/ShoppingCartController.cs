using AutoMapper;
using Common;
using Microsoft.AspNet.Identity;
using Model.Models;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Web.Infrastructure.Extensions;
using Web.Models;

namespace Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        IProductService _productService;
        ApplicationUserManager _userManager;
        IOrderService _orderService;
        public ShoppingCartController(IOrderService orderService, IProductService productService, ApplicationUserManager userManager)
        {
            this._productService = productService;
            this._userManager = userManager;
            this._orderService = orderService;
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {
            if (Session[CommonConstants.SessionCart] == null)
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            return View();
        }

        public JsonResult GetAll()
        {
            if (Session[CommonConstants.SessionCart] == null)
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            return Json(new
            {
                data = cart,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Add(int productId)
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            if (cart == null)
            {
                cart = new List<ShoppingCartViewModel>();
            }
            var product = _productService.GetById(productId);
            if (product.Quantity == 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Sản phẩm hiện tại đang hết hàng."
                });
            }
            if (cart.Any(x => x.ProductId == productId))
            {
                foreach (var item in cart)
                {
                    if (item.ProductId == productId)
                    {
                        item.Quantity += 1;
                    }
                }
            }
            else
            {
                ShoppingCartViewModel newItem = new ShoppingCartViewModel();
                newItem.ProductId = productId;
                newItem.Product = Mapper.Map<Product, ProductViewModel>(product);
                newItem.Quantity = 1;
                cart.Add(newItem);
            }
            Session[CommonConstants.SessionCart] = cart;
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult Update(string cartData)
        {
            var cartViewModel = new JavaScriptSerializer().Deserialize<List<ShoppingCartViewModel>>(cartData);
            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            foreach (var item in cartSession)
            {
                foreach (var jitem in cartViewModel)
                {
                    if (item.ProductId == jitem.ProductId)
                    {
                        item.Quantity = jitem.Quantity;
                    }
                }
            }
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult Delete(int productId)
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            if (cart != null)
            {
                cart.RemoveAll(x => x.ProductId == productId);
                return Json(new
                {
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }

        [HttpPost]
        public JsonResult DeleteAll()
        {
            Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public JsonResult GetUserLogin()
        {
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = _userManager.FindById(userId);
                return Json(new
                {
                    data = user,
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }

        [HttpPost]
        public JsonResult CreateOrder(string orderViewModel)
        {
            var order = new JavaScriptSerializer().Deserialize<OrderViewModel>(orderViewModel);
            var orderNew = new Order();
            orderNew.UpdateOrder(order);
            if (Request.IsAuthenticated)
            {
                orderNew.CustomerId = User.Identity.GetUserId();
                orderNew.CustomerName = User.Identity.GetUserName();
            }
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            bool isEnough = true;
            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            var productName = "";
            var quantity = 0;
            var productId = 0;
            foreach (var item in cartSession)
            {
                var orderDetail = new OrderDetail();
                orderDetail.ProductID = item.ProductId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.Price = item.Product.Price;
                orderDetails.Add(orderDetail);
                isEnough = _productService.SellProduct(item.ProductId, item.Quantity);
                if (isEnough == false)
                {
                    productId = item.ProductId;
                    quantity = item.Product.Quantity;
                    productName = item.Product.Name;
                    break;
                }
                
            }
            if (isEnough)
            {
                _orderService.Create(orderNew, orderDetails);
                _productService.Save();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false,
                    message = "" + productName + ". Mã SP("+productId+")"+" hiện chỉ còn: ("+quantity+") sản phẩm."
                });
            }


        }
    }
}