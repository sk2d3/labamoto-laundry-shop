using LabamotoLaundryShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.ViewModels
{
    public class OwnerDashboardViewModel
    {
        public decimal TotalIncomeToday { get; set; }
        public decimal TotalIncomeWeekly { get; set; }
        public decimal TotalIncomeMonthly { get; set; }
        public int ActiveOrders { get; set; }
        public int TodaysOrders { get; set; }

        // Pie chart counts for order statuses
        public int QueuedCount { get; set; }
        public int WashingCount { get; set; }
        public int DryingCount { get; set; }
        public int ReadyCount { get; set; }

        public List<AlertViewModel> Alerts { get; set; } = new List<AlertViewModel>();
    }

    
}
