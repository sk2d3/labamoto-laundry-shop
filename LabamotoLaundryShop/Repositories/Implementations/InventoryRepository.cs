using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LabamotoLaundryShop.Repositories.Implementations
{
    public class InventoryRepository
    {
        private readonly DapperContext _context;

        public InventoryRepository(DapperContext context)
        {
            _context = context;
        }

        // Get all inventory items
        public List<InventoryItem> GetAllInventory()
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        InventoryItemID,
                        ItemName,
                        Category,
                        Brand,
                        CurrentStock,
                        MinimumStock,
                        Unit,
                        ReorderPoint,
                        UnitCost,
                        Supplier,
                        SupplierContact,
                        StorageLocation,
                        Notes,
                        TransactionDate
                    FROM inventory_items
                    ORDER BY InventoryItemID ASC";

                var items = connection.Query<InventoryItem>(sql).ToList();

                // Optional: populate LastOrderDate for out-of-stock display
                foreach (var item in items)
                {
                    var lastOrderSql = @"
                        SELECT MAX(TransactionDate)
                        FROM inventory_transactions
                        WHERE InventoryItemID = @Id AND TransactionType = 'Restock'";
                    item.LastOrderDate = connection.QueryFirstOrDefault<DateTime?>(lastOrderSql, new { Id = item.InventoryItemID });
                }

                return items;
            }
        }

        // Add new inventory item
        public void AddInventoryItem(InventoryItem item)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    INSERT INTO inventory_items 
                        (ItemName, Category, Brand, CurrentStock, MinimumStock, Unit, ReorderPoint, UnitCost, Supplier, SupplierContact, StorageLocation, Notes)
                    VALUES 
                        (@ItemName, @Category, @Brand, @CurrentStock, @MinimumStock, @Unit, @ReorderPoint, @UnitCost, @Supplier, @SupplierContact, @StorageLocation, @Notes)";
                connection.Execute(sql, item);
            }
        }

        // Update existing inventory item
        public void UpdateInventoryItem(InventoryItem item)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    UPDATE inventory_items
                    SET 
                        ItemName = @ItemName,
                        Category = @Category,
                        Brand = @Brand,
                        CurrentStock = @CurrentStock,
                        MinimumStock = @MinimumStock,
                        Unit = @Unit,
                        ReorderPoint = @ReorderPoint,
                        UnitCost = @UnitCost,
                        Supplier = @Supplier,
                        SupplierContact = @SupplierContact,
                        StorageLocation = @StorageLocation,
                        Notes = @Notes
                    WHERE InventoryItemID = @InventoryItemID";
                connection.Execute(sql, item);
            }
        }

        public List<string> GetCategories()
        {
            return new List<string>
        {
            "Detergents & Cleaning",
            "Packaging & Supplies",
            "Equipment Parts",
            "Utilities"
        };
        }
    }
}
