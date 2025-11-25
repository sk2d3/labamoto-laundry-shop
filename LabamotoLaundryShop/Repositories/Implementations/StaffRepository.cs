using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Data;

namespace LabamotoLaundryShop.Repositories
{
    public class StaffRepository
    {
        private readonly DapperContext _context;

        public StaffRepository()
        {
            _context = new DapperContext();
        }

        // Get all staff data with dashboard stats
        public StaffViewModel GetStaffDashboardData()
        {
            using (var connection = _context.CreateConnection())
            {
                // Fetch all staff from database
                var staffList = connection.Query<StaffViewModel>("SELECT * FROM staff").ToList();

                // Map to display model
                var displayList = staffList.Select(s => new StaffDisplay
                {
                    StaffID = s.StaffID,                 // DB PK
                    EmployeeID = s.EmployeeID,           // UI-friendly string ID
                    FullName = s.FullName,
                    Role = s.Role,
                    ContactNumber = s.ContactNumber,
                    Email = s.Email,
                    Shift = string.IsNullOrEmpty(s.Shift) ? "Day" : s.Shift, // fallback if empty
                    Status = "On Duty", // placeholder for now
                    AccessLevel = s.AccessLevel,
                    Rate = s.Rate,
                    Address = s.Address
                }).ToList();

                // Fetch weekly schedules including StaffName and Role
                var weeklySchedules = connection.Query<WeeklySchedule>(@"
                    SELECT s.StaffID, s.FullName AS StaffName, s.Role, ws.DayOfWeek, ws.Shift, ws.TimeIn, ws.TimeOut
                    FROM weekly_schedules ws
                    JOIN staff s ON ws.StaffID = s.StaffID
                    ORDER BY ws.DayOfWeek, ws.TimeIn
                ").ToList();

                return new StaffViewModel
                {
                    StaffList = displayList,
                    TotalStaff = displayList.Count,
                    OnDutyCount = displayList.Count(x => x.Status == "On Duty"),
                    OffDutyCount = displayList.Count(x => x.Status != "On Duty"),
                    AdminCount = displayList.Count(x => x.AccessLevel == "Admin"),
                    AttendanceRecords = new List<AttendanceRecord>(),
                    WeeklySchedules = new List<WeeklySchedule>()
                };
            }
        }


        public void AddStaff(StaffViewModel staff)
        {
            using (var connection = _context.CreateConnection())
            {
                // Generate unique EmployeeID
                int lastId = connection.QueryFirstOrDefault<int>("SELECT IFNULL(MAX(StaffID),0) FROM staff");
                staff.EmployeeID = "E-" + (lastId + 1).ToString("D3");

                // Create username and password defaults
                staff.Username = staff.FullName.Replace(" ", "").ToLower();
                staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345");

                string sql = @"INSERT INTO staff 
                       (EmployeeID, FullName, Username, PasswordHash, Role, ContactNumber, Email, Shift, AccessLevel, Rate, Address)
                       VALUES 
                       (@EmployeeID, @FullName, @Username, @PasswordHash, @Role, @ContactNumber, @Email, @Shift, @AccessLevel, @Rate, @Address)";

                connection.Execute(sql, staff);
            }
        }
        public void UpdateStaff(StaffViewModel staff)
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = @"UPDATE staff SET 
                       FullName = @FullName,
                       Role = @Role,
                       ContactNumber = @ContactNumber,
                       Shift = @Shift,
                       AccessLevel = @AccessLevel
                       WHERE StaffID = @StaffID";

                connection.Execute(sql, staff);
            }
        }

        // Fetch a single staff by StaffID
        public StaffViewModel GetStaffById(int staffId)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = "SELECT * FROM Staff WHERE StaffID = @StaffID";
                return connection.QuerySingleOrDefault<StaffViewModel>(sql, new { StaffID = staffId });
            }
        }

        // Fetch attendance records for a specific staff
        public IEnumerable<AttendanceRecord> GetAttendanceByStaff(int staffId)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            SELECT s.FullName, s.Role, a.TimeIn, a.TimeOut, a.Status
            FROM attendance a
            JOIN staff s ON a.StaffID = s.StaffID
            WHERE a.StaffID = @StaffID
            ORDER BY a.TimeIn DESC";

                return connection.Query<AttendanceRecord>(sql, new { StaffID = staffId });
            }
        }

        // Fetch all attendance records
        public IEnumerable<AttendanceRecord> GetAllAttendance()
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            SELECT 
                s.StaffID,
                s.FullName AS Name, 
                s.Role, 
                a.TimeIn, 
                a.TimeOut, 
                a.Status
            FROM attendance a
            JOIN staff s ON a.StaffID = s.StaffID
            ORDER BY a.TimeIn DESC";

                var result = connection.Query<AttendanceRecord>(sql).ToList();

                return result ?? new List<AttendanceRecord>();
            }
        }



        // ================================
        // PAYROLL
        // ================================
        public IEnumerable<PayrollRecord> GetPayrollRecords(DateTime startDate, DateTime endDate)
        {
            using (var connection = _context.CreateConnection())
            {
                // Get basic payroll info from payroll table
                var sql = @"
            SELECT p.PayrollID, s.StaffID, s.FullName AS Name, s.Role, s.Rate, p.DaysWorked, p.PayDate
            FROM payroll p
            JOIN staff s ON p.StaffID = s.StaffID
            WHERE p.PayDate BETWEEN @StartDate AND @EndDate
            ORDER BY p.PayDate DESC";

                var records = connection.Query<PayrollRecord>(sql, new { StartDate = startDate, EndDate = endDate }).ToList();

                // Compute BasePay, Overtime, Bonus, Deductions, Gross, Net
                foreach (var r in records)
                {
                    r.BasePay = r.Rate * r.DaysWorked;

                    var overtimeHours = connection.Query<decimal>(
                        @"SELECT IFNULL(SUM(a.OvertimeHours),0)
                  FROM attendance a
                  WHERE a.StaffID = @StaffID
                  AND a.TimeIn BETWEEN @StartDate AND @EndDate",
                        new { StaffID = r.StaffID, StartDate = startDate, EndDate = endDate }).FirstOrDefault();
                    r.OvertimePay = (overtimeHours * r.Rate) / 8; // assuming 8h standard day

                    r.Bonus = connection.Query<decimal>(
                        @"SELECT IFNULL(SUM(a.Bonus),0)
                  FROM attendance a
                  WHERE a.StaffID = @StaffID
                  AND a.TimeIn BETWEEN @StartDate AND @EndDate",
                        new { StaffID = r.StaffID, StartDate = startDate, EndDate = endDate }).FirstOrDefault();

                    r.Deductions = connection.Query<decimal>(
                        @"SELECT IFNULL(SUM(a.Deductions),0)
                  FROM attendance a
                  WHERE a.StaffID = @StaffID
                  AND a.TimeIn BETWEEN @StartDate AND @EndDate",
                        new { StaffID = r.StaffID, StartDate = startDate, EndDate = endDate }).FirstOrDefault();

                    r.GrossPay = r.BasePay + r.OvertimePay + r.Bonus;
                    r.NetPay = r.GrossPay - r.Deductions;
                }

                return records;
            }
        }


        public List<WeeklySchedule> GetWeeklySchedules(DateTime weekStart)
        {
            DateTime weekEnd = weekStart.AddDays(6);

            using (var connection = _context.CreateConnection())
            {
                // 1. Get all staff
                var staffList = connection.Query<StaffViewModel>(
                "SELECT StaffID, FullName, Role FROM staff").ToList();

                // 2. Get schedules for the week
                var rawSchedules = connection.Query<WeeklyScheduleRaw>(
                    @"SELECT s.StaffID, st.FullName AS StaffName, st.Role, s.DayOfWeek,
                     s.ShiftStart AS TimeIn, s.ShiftEnd AS TimeOut
              FROM schedules s
              INNER JOIN staff st ON s.StaffID = st.StaffID
              WHERE s.WorkDate BETWEEN @Start AND @End",
                    new { Start = weekStart, End = weekEnd }).ToList();

                // 3. Pivot into WeeklySchedule
                var weeklySchedules = new List<WeeklySchedule>();

                foreach (var staff in staffList)
                {
                    var ws = new WeeklySchedule
                    {
                        StaffID = staff.StaffID,
                        StaffName = staff.FullName,  // <- this should be filled
                        Role = staff.Role,
                        Mon = "OFF",
                        Tue = "OFF",
                        Wed = "OFF",
                        Thu = "OFF",
                        Fri = "OFF",
                        Sat = "OFF",
                        Sun = "OFF"
                    };

                    // Get all shifts for this staff
                    var staffShifts = rawSchedules.Where(r => r.StaffID == staff.StaffID).ToList();

                    foreach (var shift in staffShifts)
                    {
                        // Format the shift time
                        string formattedShift = $"{shift.TimeIn:hh\\:mm} - {shift.TimeOut:hh\\:mm}";

                        switch (shift.DayOfWeek)
                        {
                            case "Monday": ws.Mon = formattedShift; break;
                            case "Tuesday": ws.Tue = formattedShift; break;
                            case "Wednesday": ws.Wed = formattedShift; break;
                            case "Thursday": ws.Thu = formattedShift; break;
                            case "Friday": ws.Fri = formattedShift; break;
                            case "Saturday": ws.Sat = formattedShift; break;
                            case "Sunday": ws.Sun = formattedShift; break;
                        }
                    }

                    weeklySchedules.Add(ws);
                }

                return weeklySchedules;
            }
        }

        public void AddSchedule(WeeklySchedule schedule, DateTime weekStart)
        {
            using (var connection = _context.CreateConnection())
            {
                var sqlInsert = @"INSERT INTO schedules 
                          (StaffID, WorkDate, ShiftStart, ShiftEnd) 
                          VALUES (@StaffID, @WorkDate, @ShiftStart, @ShiftEnd)";

                var days = new Dictionary<string, string>
        {
            {"Monday", schedule.Mon },
            {"Tuesday", schedule.Tue },
            {"Wednesday", schedule.Wed },
            {"Thursday", schedule.Thu },
            {"Friday", schedule.Fri },
            {"Saturday", schedule.Sat },
            {"Sunday", schedule.Sun }
        };

                foreach (var day in days)
                {
                    if (day.Value != "OFF")
                    {
                        var times = day.Value.Split(new[] { " - " }, StringSplitOptions.None);
                        TimeSpan start = TimeSpan.Parse(times[0]);
                        TimeSpan end = TimeSpan.Parse(times[1]);

                        int offset = 0;
                        switch (day.Key)
                        {
                            case "Monday": offset = 0; break;
                            case "Tuesday": offset = 1; break;
                            case "Wednesday": offset = 2; break;
                            case "Thursday": offset = 3; break;
                            case "Friday": offset = 4; break;
                            case "Saturday": offset = 5; break;
                            case "Sunday": offset = 6; break;
                            default: offset = 0; break;
                        }

                        DateTime workDate = weekStart.AddDays(offset);

                        // Check if schedule already exists
                        var exists = connection.QueryFirstOrDefault<int>(
                            "SELECT COUNT(*) FROM schedules WHERE StaffID = @StaffID AND WorkDate = @WorkDate",
                            new { StaffID = schedule.StaffID, WorkDate = workDate });

                        if (exists == 0)
                        {
                            connection.Execute(sqlInsert, new
                            {
                                StaffID = schedule.StaffID,
                                WorkDate = workDate,
                                ShiftStart = start,
                                ShiftEnd = end
                            });
                        }
                    }
                }
            }
        }

        public void DeleteSchedule(int staffId, DateTime weekStart)
        {
            DateTime weekEnd = weekStart.AddDays(6);
            using (var connection = _context.CreateConnection())
            {
                connection.Execute("DELETE FROM schedules WHERE StaffID = @StaffID AND WorkDate BETWEEN @Start AND @End",
                    new { StaffID = staffId, Start = weekStart, End = weekEnd });
            }
        }

        // Add a new attendance record
        public void AddAttendance(AttendanceRecord record)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"INSERT INTO attendance 
                    (StaffID, TimeIn, TimeOut, Status) 
                    VALUES 
                    (@StaffID, @TimeIn, @TimeOut, @Status)";

                connection.Execute(sql, new
                {
                    StaffID = record.StaffID,
                    TimeIn = record.TimeIn,
                    TimeOut = record.TimeOut,
                    Status = record.Status
                });
            }
        }

        public PayrollRecord GetPayrollRecordByStaffID(int staffID, DateTime startDate, DateTime endDate)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
        SELECT s.StaffID, s.FullName AS Name, s.Role, s.Rate,
               COUNT(a.AttendanceID) AS DaysWorked,
               SUM(s.Rate) AS BasePay,
               SUM(a.OvertimeHours * s.Rate / 8) AS OvertimePay,
               SUM(a.Bonus) AS Bonus,
               SUM(s.Rate + a.Bonus + (a.OvertimeHours * s.Rate / 8)) AS GrossPay,
               SUM(a.Deductions) AS Deductions
        FROM staff s
        LEFT JOIN attendance a ON s.StaffID = a.StaffID
            AND a.TimeIn BETWEEN @StartDate AND @EndDate
        WHERE s.StaffID = @StaffID
        GROUP BY s.StaffID, s.FullName, s.Role, s.Rate";

                var record = connection.QuerySingleOrDefault<PayrollRecord>(sql, new
                {
                    StaffID = staffID,
                    StartDate = startDate,
                    EndDate = endDate
                });

                if (record != null)
                    record.NetPay = record.GrossPay - record.Deductions;

                return record ?? new PayrollRecord();
            }
        }

        public string GetStaffEmail(int staffID)
        {
            using (var connection = _context.CreateConnection())
            {
                // Make sure your staff table has an Email column
                var sql = "SELECT Email FROM staff WHERE StaffID = @StaffID";
                return connection.QueryFirstOrDefault<string>(sql, new { StaffID = staffID });
            }
        }

        public void UpdatePayrollRecord(PayrollRecord record)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            UPDATE payroll
            SET IsProcessed = @IsProcessed,
                ProcessedDate = @ProcessedDate
            WHERE PayrollID = @PayrollID";

                connection.Execute(sql, new
                {
                    PayrollID = record.PayrollID,
                    IsProcessed = record.IsProcessed ? 1 : 0, // MySQL BIT
                    ProcessedDate = record.ProcessedDate
                });
            }
        }
    }
}
