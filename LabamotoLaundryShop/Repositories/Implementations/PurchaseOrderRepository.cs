using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class PurchaseOrderRepository
    {
        private readonly DapperContext _context;

        public PurchaseOrderRepository(DapperContext context)
        {
            _context = context;
        }

        // Create a new purchase order
        public void CreatePurchaseOrder(PurchaseOrder order)
        {
            using (IDbConnection conn = _context.CreateConnection())
            {
                var sql = @"
                    INSERT INTO purchaseorders
                        (ItemName, CurrentStock, MinLevel, Unit, OrderQuantity, Supplier, UnitCost, TotalCost, DeliveryDate, Notes, CreatedAt, Status)
                    VALUES
                        (@ItemName, @CurrentStock, @MinLevel, @Unit, @OrderQuantity, @Supplier, @UnitCost, @TotalCost, @DeliveryDate, @Notes, @CreatedAt, @Status)";
                conn.Execute(sql, order);
            }
        }

        // Get all purchase orders (latest first)
        public List<PurchaseOrder> GetAllPurchaseOrders()
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT * FROM purchaseorders ORDER BY CreatedAt DESC";
                return connection.Query<PurchaseOrder>(query).ToList();
            }
        }
    }
}
