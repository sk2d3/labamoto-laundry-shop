using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
	public class Notification
	{
        public int NotificationID { get; set; }
        public int? OrderID { get; set; }
        public int? CustomerID { get; set; }
        public string NotificationType { get; set; } // e.g., "Order Confirmation"
        public string MessageType { get; set; }      // e.g., "SMS"
        public string Recipient { get; set; }        // e.g., customer number/email
        public string Status { get; set; }
    }
}