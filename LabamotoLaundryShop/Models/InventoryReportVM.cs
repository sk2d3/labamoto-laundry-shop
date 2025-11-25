using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class InventoryReportVM
    {
        public List<InventoryItem> Items { get; set; }
        public int SelectedPeriod { get; set; }
        public string SelectedType { get; set; }
        public string SelectedCategory { get; set; }
    }
}