using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class InventoryChartData
    {
        public string Category { get; set; }
        public int TotalStock { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}