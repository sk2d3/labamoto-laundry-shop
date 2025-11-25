using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class InventoryItem
    {
        public int InventoryItemID { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int CurrentStock { get; set; }
        public string Unit { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal MinLevel => MinimumStock; // For convenience
        public DateTime TransactionDate { get; set; }
        public string Brand { get; set; }
        public int ReorderPoint { get; set; }
        public decimal UnitCost { get; set; }
        public string Supplier { get; set; }
        public string SupplierContact { get; set; }
        public string StorageLocation { get; set; }
        public string Notes { get; set; }

        // Computed properties
        public string Status => CurrentStock <= 0 ? "OUT" : (CurrentStock <= MinimumStock ? "Low" : "OK");
        public int? EstimatedDaysRemaining => CurrentStock > 0 ? (int?)(CurrentStock / 1) : null; // Optional simple logic

        // ✅ Add this property for repository / Razor usage
        public DateTime? LastOrderDate { get; set; }
    }
}