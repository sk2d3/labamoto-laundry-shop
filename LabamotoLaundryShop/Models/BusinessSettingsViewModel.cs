using System;
using System.Collections.Generic;

namespace LabamotoLaundryShop.Models
{
    public class GeneralInfo
    {
        public int Id { get; set; }
        public string SettingKey { get; set; }  // renamed from Key for clarity
        public string SettingValue { get; set; } // renamed from Value
    }

    public class BusinessHour
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public string OpenTime { get; set; } = "08:00";  // default value
        public string CloseTime { get; set; } = "17:00"; // default value
        public string Status { get; set; } = "Open";     // Open/Closed
    }


    public class Holiday
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public string Description { get; set; }
    }

    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = false;
        public string Details { get; set; }
    }

    public class SystemConfig
    {
        public int Id { get; set; }
        public string SettingKey { get; set; }  // renamed for clarity
        public string SettingValue { get; set; } // renamed for clarity
    }

    public class BusinessSettingsViewModel
    {
        public List<GeneralInfo> GeneralInfo { get; set; } = new List<GeneralInfo>();
        public List<BusinessHour> BusinessHours { get; set; } = new List<BusinessHour>();
        public List<Holiday> Holidays { get; set; } = new List<Holiday>();
        public List<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        public List<SystemConfig> SystemConfig { get; set; } = new List<SystemConfig>();
    }
}
