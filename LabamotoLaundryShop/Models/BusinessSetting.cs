using System;
using System.ComponentModel.DataAnnotations;

namespace LabamotoLaundryShop.Models
{
    public class BusinessSetting
    {

        [Key]
        public int SettingID { get; set; }           // Primary key
        public string SettingKey { get; set; }       // e.g., "BusinessName"
        public string SettingValue { get; set; }     // e.g., "Clean & Fresh Laundry Services"
        public string Category { get; set; }         // e.g., "GeneralInfo", "BusinessHours"
    }
}