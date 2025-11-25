using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabamotoLaundryShop.Models
{
    public class NotificationsViewModel
    {
        public List<NotificationTemplate> Templates { get; set; } = new List<NotificationTemplate>();
        public NotificationSetting Settings { get; set; } = new NotificationSetting();
    }

    // NotificationTemplate.cs
    public class NotificationTemplate
    {
        public int NotificationTemplateID { get; set; }
        public string TemplateName { get; set; }       // e.g., "Order Confirmation SMS"
        public string TriggerEvent { get; set; }       // e.g., "Order Created"
        public string MessageType { get; set; }        // SMS or Email
        public string MessageContent { get; set; }     // The message with placeholders
        public string Status { get; set; } = "Enabled";
    }

    // NotificationSetting.cs
    public class NotificationSetting
    {
        public int SettingID { get; set; }
        public string SmsGateway { get; set; } = "Semaphore"; // Default
        public string SenderName { get; set; } = "CleanFresh";
        public bool EmailReceiptEnabled { get; set; } = true;
        public bool DailySummaryEnabled { get; set; } = true;
        public bool WeeklyReportEnabled { get; set; } = true;
        public string OwnerEmail { get; set; } = "owner@cleanfresh.com";
        public string SupportEmail { get; set; } = "support@cleanfresh.com";
        public int SmsCredits { get; set; } = 450;
        public int SmsUsedThisMonth { get; set; } = 120;
        public decimal SmsCost { get; set; } = 1.00M;
    }

    
}