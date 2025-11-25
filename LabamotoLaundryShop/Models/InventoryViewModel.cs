using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class InventoryViewModel
    {
        public List<InventoryItem> InventoryItems { get; set; }
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public decimal MonthlyUsageCost { get; set; }
    }

}