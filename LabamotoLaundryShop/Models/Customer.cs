using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }          // Primary key
        public string FullName { get; set; }         // Customer name
        public string ContactNumber { get; set; }    // Phone number
        public string Email { get; set; }            // Email
        public string CustomerType { get; set; }     // Regular, VIP, Corporate
        public int TotalOrders { get; set; }         // Total number of orders
        public decimal TotalSpent { get; set; }      // Lifetime value / total spent
        public DateTime? LastVisit { get; set; }     // Nullable last visit date
        public int LoyaltyPoints { get; set; }       // Loyalty points
    }

    // ViewModel for the Index page to include summary/stats
    public class CustomerViewModel
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();

        public int TotalCustomers { get; set; }
        public int VIPCustomers { get; set; }
        public int CorporateAccounts { get; set; }
        public int NewThisMonth { get; set; } // count of customers added in current month
    }
}