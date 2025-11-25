using System;
using System.Collections.Generic;

namespace LabamotoLaundryShop.Models
{
    public class StaffViewModel
    {
        public List<StaffDisplay> StaffList { get; set; }
        public List<AttendanceRecord> AttendanceRecords { get; set; }
        public List<WeeklySchedule> WeeklySchedules { get; set; }



        public int TotalStaff { get; set; }
        public int OnDutyCount { get; set; }
        public int OffDutyCount { get; set; }
        public int AdminCount { get; set; }


        public int StaffID { get; set; }       // database auto-increment
        public string EmployeeID { get; set; } // UI-friendly ID like "E-001"
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Shift { get; set; }
        public string AccessLevel { get; set; }
        public decimal Rate { get; set; }
        public string Address { get; set; }

        public string Status { get; set; }

    }

    public class StaffDisplay
    {
        public int StaffID { get; set; }
        public string EmployeeID { get; set; }  // <-- change from int to string
        public string FullName { get; set; }
        public string Role { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public string AccessLevel { get; set; }
        public decimal Rate { get; set; }
        public string Address { get; set; }
    }

    public class WeeklyScheduleRaw
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string Role { get; set; }
        public string DayOfWeek { get; set; }
        public string Shift { get; set; }
        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }
    }

    public class AttendanceRecord
    {
        public int AttendanceID { get; set; }
        public string Name { get; set; }
        public int StaffID { get; set; }
        public string Role { get; set; }
        public DateTime TimeIn { get; set; }       // <-- DateTime
        public DateTime? TimeOut { get; set; }     // <-- nullable DateTime
        public string Status { get; set; }
    }

    public class PayrollRecord
    {
        public int PayrollID { get; set; }  // <- add this
        public int StaffID { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public decimal Rate { get; set; }      // daily rate
        public int DaysWorked { get; set; }

        public decimal BasePay { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Bonus { get; set; }
        public decimal GrossPay { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }

        public DateTime PayDate { get; set; }
        // ✅ Add these two properties
        public bool IsProcessed { get; set; } = false;
        public DateTime? ProcessedDate { get; set; }
    }

    public class WeeklySchedule
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }  // This comes from staff.FullName
        public string Role { get; set; }

        public string Mon { get; set; } = "OFF";
        public string Tue { get; set; } = "OFF";
        public string Wed { get; set; } = "OFF";
        public string Thu { get; set; } = "OFF";
        public string Fri { get; set; } = "OFF";
        public string Sat { get; set; } = "OFF";
        public string Sun { get; set; } = "OFF";
        public string DayOfWeek { get; set; }
        public string Shift { get; set; }

        public TimeSpan TimeIn { get; set; }   // keep only TimeSpan
        public TimeSpan TimeOut { get; set; }  // keep only TimeSpan
    }



}
