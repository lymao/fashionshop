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
        List<Order> GetList(string startDate, string endDate, string filter, string pamentStatus, int page, int pageSize, out int totalRow);
    }
    public class OrderService : IOrderService
    {
        IOrderRepository _orderRepository;
        IOrderDetailRepository _orderDetailRepository;
        IUnitOfWork _unitOfWork;
        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IUnitOfWork unitOfWork)
        {
            this._orderRepository = orderRepository;
            this._orderDetailRepository = orderDetailRepository;
            this._unitOfWork = unitOfWork;
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
            return query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
