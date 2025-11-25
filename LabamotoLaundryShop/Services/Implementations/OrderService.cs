using System;
using System.Collections.Generic;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Interfaces;
using LabamotoLaundryShop.Services.Interfaces;

namespace LabamotoLaundryShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public decimal GetTotalIncome(DateTime startDate, DateTime endDate)
        {
            return _orderRepository.GetTotalIncome(startDate, endDate);
        }

        public decimal GetTotalIncomeToday()
        {
            return GetTotalIncome(DateTime.Today, DateTime.Today.AddDays(1));
        }

        public decimal GetTotalIncomeWeekly()
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return GetTotalIncome(startOfWeek, DateTime.Today.AddDays(1));
        }

        public decimal GetTotalIncomeMonthly()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return GetTotalIncome(startOfMonth, DateTime.Today.AddDays(1));
        }

        public int GetActiveOrdersCount()
        {
            return _orderRepository.GetActiveOrdersCount();
        }

        public int GetTodaysOrdersCount()
        {
            return _orderRepository.GetOrdersCountByDate(DateTime.Today);
        }

        public int GetOrdersCountByStatus(string status)
        {
            return _orderRepository.GetOrdersCountByStatus(status);
        }

        public IEnumerable<Order> GetOrdersByStatus(string status)
        {
            return _orderRepository.GetOrdersByStatus(status);
        }

        public int GetOrdersCountByDate(DateTime date)
        {
            return _orderRepository.GetOrdersCountByDate(date);
        }

        public IEnumerable<Order> GetOrders(DateTime? startDate = null, DateTime? endDate = null)
        {
            return _orderRepository.GetOrders(startDate, endDate);
        }

        public Dictionary<DateTime, decimal> GetIncomeOverTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            return _orderRepository.GetIncomeOverTime(startDate, endDate);
        }
    }
}
