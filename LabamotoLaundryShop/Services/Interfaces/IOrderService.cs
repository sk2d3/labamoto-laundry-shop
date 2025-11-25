using System;
using System.Collections.Generic;
using LabamotoLaundryShop.Models;

namespace LabamotoLaundryShop.Services.Interfaces
{
    public interface IOrderService
    {
        decimal GetTotalIncome(DateTime startDate, DateTime endDate);
        decimal GetTotalIncomeToday();
        decimal GetTotalIncomeWeekly();
        decimal GetTotalIncomeMonthly();
        int GetActiveOrdersCount();
        int GetTodaysOrdersCount();
        int GetOrdersCountByStatus(string status);
        IEnumerable<Order> GetOrdersByStatus(string status);
        int GetOrdersCountByDate(DateTime date);

        IEnumerable<Order> GetOrders(DateTime? startDate = null, DateTime? endDate = null);
        Dictionary<DateTime, decimal> GetIncomeOverTime(DateTime? startDate = null, DateTime? endDate = null);
    }
}
