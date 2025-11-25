using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
	public class Order
	{
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } // new
        public int CreatedByStaffID { get; set; }
        public int ServiceTypeID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal RushSurcharge { get; set; }
        public decimal AddOnTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
    }
}