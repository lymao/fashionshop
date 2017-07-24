using Data.Infrastructure;
using Data.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IOrderService
    {
        bool Create(Order oder, List<OrderDetail> orderDetails);
        Order CreateAll(Order order);
        List<Order> GetList(string startDate, string endDate, string filter, string pamentStatus, int page, int pageSize, out int totalRow);
        IEnumerable<Product> GetAllProduct();
        Order GetOrder(int id);
        List<OrderDetail> GetOrderDetails(int orderId);
        Product GetProductById(int id);
        void Delete(int id);
        void Save();

    }
    public class OrderService : IOrderService
    {
        IOrderRepository _orderRepository;
        IOrderDetailRepository _orderDetailRepository;
        IProductRepository _productRepository;
        IUnitOfWork _unitOfWork;
        public OrderService(IProductRepository productRepository, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IUnitOfWork unitOfWork)
        {
            this._orderRepository = orderRepository;
            this._orderDetailRepository = orderDetailRepository;
            this._unitOfWork = unitOfWork;
            this._productRepository = productRepository;
        }

        public bool Create(Order oder, List<OrderDetail> orderDetails)
        {
            try
            {
                var order = _orderRepository.Add(oder);
                _unitOfWork.Commit();
                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderID = order.ID;
                    _orderDetailRepository.Add(orderDetail);
                }
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Order CreateAll(Order order)
        {
            try
            {
                _orderRepository.Add(order);
                _unitOfWork.Commit();
                return order;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            _orderRepository.Delete(id);
        }

        public IEnumerable<Product> GetAllProduct()
        {
            return _productRepository.GetAll(new string[] { "ProductSizes.Size"});
        }

        public List<Order> GetList(string startDate, string endDate, string filter, string pamentStatus, int page, int pageSize, out int totalRow)
        {
            var query = _orderRepository.GetAll();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.CustomerName.Contains(filter));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime sDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= sDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime eDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate <= eDate);
            }
            if (!string.IsNullOrEmpty(pamentStatus))
            {
                query = query.Where(x => x.PaymentStatus == pamentStatus);
            }
            totalRow = query.Count();
            return query.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize).ToList();
        }

        public Order GetOrder(int id)
        {
            return _orderRepository.GetSingleByCondition(x => x.ID == id, new string[] { "OrderDetails", "OrderDetails.Product", "OrderDetails.Size" });
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            return _orderDetailRepository.GetMulti(x => x.OrderID == orderId, new string[] { "Size", "Product" }).ToList();
        }

        public Product GetProductById(int id)
        {
            return _productRepository.GetSingleByCondition(x=>x.ID==id,new string[] { "ProductSizes.Size"});
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
