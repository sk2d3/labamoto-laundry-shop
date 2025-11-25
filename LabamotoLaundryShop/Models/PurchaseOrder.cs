using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinLevel { get; set; }
        public string Unit { get; set; }
        public int OrderQuantity { get; set; }
        public string Supplier { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending by default
    }
}