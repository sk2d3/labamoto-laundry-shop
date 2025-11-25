using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class CustomerRepository
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }

        // ✅ Get all customers
        public IEnumerable<Customer> GetAllCustomers()
        {
            using (var connection = _context.CreateConnection()) // Use CreateConnection()
            {
                string sql = "SELECT * FROM customers ORDER BY CustomerID ASC";
                return connection.Query<Customer>(sql).ToList(); // Query<T>() is available on IDbConnection
            }
        }

        public void CreateCustomer(Customer customer)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            INSERT INTO CUSTOMERS (FullName, ContactNumber, Email, CustomerType, TotalOrders, TotalSpent)
            VALUES (@FullName, @ContactNumber, @Email, @CustomerType, @TotalOrders, @TotalSpent)";

                connection.Execute(sql, customer);
            }
        }


        public void UpdateCustomer(Customer customer)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            UPDATE CUSTOMERS
            SET FullName = @FullName,
                ContactNumber = @ContactNumber,
                Email = @Email,
                CustomerType = @CustomerType,
                TotalOrders = @TotalOrders,
                TotalSpent = @TotalSpent
            WHERE CustomerID = @CustomerID";

                connection.Execute(sql, customer);
            }
        }

    }
}
