using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Interfaces;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _context;

        public OrderRepository(DapperContext context)
        {
            _context = context;
        }

        public decimal GetTotalIncome(DateTime startDate, DateTime endDate)
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = @"SELECT IFNULL(SUM(TotalAmount),0) 
                               FROM ORDERS 
                               WHERE OrderDate BETWEEN @StartDate AND @EndDate;";
                return connection.ExecuteScalar<decimal>(sql, new { StartDate = startDate, EndDate = endDate });
            }
        }

        public int GetActiveOrdersCount()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = @"SELECT COUNT(*) 
                               FROM ORDERS 
                               WHERE Status != 'Completed';";
                return connection.ExecuteScalar<int>(sql);
            }
        }

        public int GetOrdersCountByStatus(string status)
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = @"SELECT COUNT(*) 
                               FROM ORDERS 
                               WHERE Status = @Status;";
                return connection.ExecuteScalar<int>(sql, new { Status = status });
            }
        }

        public int GetOrdersCountByDate(DateTime date)
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = @"SELECT COUNT(*) 
                               FROM ORDERS 
                               WHERE DATE(OrderDate) = @Date;";
                return connection.ExecuteScalar<int>(sql, new { Date = date.Date });
            }
        }

        public IEnumerable<Order> GetOrdersByStatus(string status)
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = @"SELECT * 
                               FROM ORDERS 
                               WHERE Status = @Status;";
                return connection.Query<Order>(sql, new { Status = status }).ToList();
            }
        }

        public IEnumerable<Order> GetOrders(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = "SELECT * FROM ORDERS WHERE 1=1";

                if (startDate.HasValue) sql += " AND OrderDate >= @StartDate";
                if (endDate.HasValue) sql += " AND OrderDate <= @EndDate";

                return connection.Query<Order>(sql, new { StartDate = startDate, EndDate = endDate }).ToList();
            }
        }

        public Dictionary<DateTime, decimal> GetIncomeOverTime(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"SELECT DATE(OrderDate) AS DateOnly, IFNULL(SUM(TotalAmount),0) AS Total
                FROM ORDERS
                WHERE 1=1";

                if (startDate.HasValue) sql += " AND OrderDate >= @StartDate";
                if (endDate.HasValue) sql += " AND OrderDate <= @EndDate";

                sql += " GROUP BY DATE(OrderDate) ORDER BY DATE(OrderDate) ASC";

                var result = connection.Query(sql, new { StartDate = startDate, EndDate = endDate });
                var dict = new Dictionary<DateTime, decimal>();

                foreach (var row in result)
                    dict.Add(row.DateOnly, row.Total);

                return dict;
            }
        }
    }
}
