using LabamotoLaundryShop.Helpers;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories;
using LabamotoLaundryShop.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Net.Mail;

namespace LabamotoLaundryShop.Controllers
{
    public class StaffOwnerController : Controller
    {
        private readonly StaffRepository _staffRepo;

        public StaffOwnerController()
        {
            _staffRepo = new StaffRepository();
        }

        // ================================
        // INDEX - Display all staff
        // ================================
        public ActionResult Index()
        {
            var model = _staffRepo.GetStaffDashboardData();
            return View(model);
        }

        // ================================
        // CREATE STAFF
        // ================================
        [HttpPost]
        public ActionResult Create(StaffViewModel staff)
        {
            if (ModelState.IsValid)
            {
                _staffRepo.AddStaff(staff);
                TempData["Success"] = "Staff successfully added!";
            }
            else
            {
                TempData["Error"] = "Failed to add staff. Please check inputs.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditStaff(StaffViewModel model)
        {
            if (model != null)
            {
                System.Diagnostics.Debug.WriteLine("StaffID: " + model.StaffID);
                System.Diagnostics.Debug.WriteLine("FullName: " + model.FullName);

                _staffRepo.UpdateStaff(model);
                TempData["SuccessMessage"] = "Staff information updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update staff.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult AttendanceHistory(int id)
        {
            var staff = _staffRepo.GetStaffById(id);
            var attendance = _staffRepo.GetAttendanceByStaff(id);

            var model = new StaffViewModel
            {
                StaffID = staff.StaffID,
                FullName = staff.FullName,
                AttendanceRecords = attendance.ToList()
            };

            return View(model);
        }

        public ActionResult FullAttendanceLog(int staffId)
        {
            var staff = _staffRepo.GetStaffById(staffId);
            var attendance = _staffRepo.GetAttendanceByStaff(staffId); // all records

            var model = new StaffViewModel
            {
                StaffID = staff.StaffID,
                FullName = staff.FullName,
                AttendanceRecords = attendance.ToList()
            };

            return View(model); // create FullAttendanceLog.cshtml
        }


        // =============================
        // VIEW SCHEDULES
        // =============================
        public ActionResult ViewSchedules(int weekOffset = 0)
        {
            // 1️⃣ Calculate start of the week (Monday)
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.StartOfWeek(DayOfWeek.Monday).AddDays(7 * weekOffset);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            // 2️⃣ Get schedules for the week
            var schedules = _staffRepo.GetWeeklySchedules(startOfWeek);

            // 3️⃣ Prepare week display string
            ViewBag.WeekStart = startOfWeek;
            ViewBag.WeekEnd = endOfWeek;
            ViewBag.WeekOffset = weekOffset; // for Previous/Next buttons

            return View(schedules);
        }


        // =============================
        // COPY SCHEDULE TO NEXT WEEK
        // =============================
        [HttpPost]
        public ActionResult CopyScheduleNextWeek()
        {
            try
            {
                // Get current week schedules
                DateTime startOfWeek = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
                var currentWeekSchedules = _staffRepo.GetWeeklySchedules(startOfWeek);

                // Logic to copy each staff's schedule to next week
                foreach (var ws in currentWeekSchedules)
                {
                    // Example: loop through days and create new schedule entries for next week
                    // You need to implement AddSchedule in repository
                    _staffRepo.AddSchedule(ws, startOfWeek.AddDays(7));
                }

                TempData["SuccessMessage"] = "Schedule copied to next week successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to copy schedule: " + ex.Message;
            }

            return RedirectToAction("ViewSchedules");
        }

        // =============================
        // EXPORT SCHEDULE
        // =============================
        public ActionResult ExportSchedule()
        {
            try
            {
                // Get current week schedules
                DateTime startOfWeek = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
                var schedules = _staffRepo.GetWeeklySchedules(startOfWeek);

                // Generate CSV (simple example)
                var csv = "Staff,Mon,Tue,Wed,Thu,Fri,Sat,Sun\n";
                foreach (var ws in schedules)
                {
                    csv += $"{ws.StaffName},{ws.Mon},{ws.Tue},{ws.Wed},{ws.Thu},{ws.Fri},{ws.Sat},{ws.Sun}\n";
                }

                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(fileBytes, "text/csv", $"Schedules_{startOfWeek:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to export schedule: " + ex.Message;
                return RedirectToAction("ViewSchedules");
            }
        }

        // =============================
        // SEND SCHEDULE TO ALL STAFF
        // =============================
        public ActionResult SendSchedule()
        {
            try
            {
                DateTime startOfWeek = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
                var schedules = _staffRepo.GetWeeklySchedules(startOfWeek);

                // Loop through staff and send email (implement SendEmail in helper)
                foreach (var ws in schedules)
                {
                    string body = $"Hello {ws.StaffName},\n\nHere is your schedule for the week:\n" +
                                  $"Mon: {ws.Mon}, Tue: {ws.Tue}, Wed: {ws.Wed}, Thu: {ws.Thu}, Fri: {ws.Fri}, Sat: {ws.Sat}, Sun: {ws.Sun}";

                    EmailHelper.SendEmail(ws.StaffName, "jhoncarlosuan26@gmail.com", "Weekly Schedule", body);
                }

                TempData["SuccessMessage"] = "Schedules sent to all staff successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to send schedules: " + ex.Message;
            }

            return RedirectToAction("ViewSchedules");
        }


        public ActionResult AttendanceLog(DateTime? startDate, DateTime? endDate, string staffName, string status)
        {
            // Get all attendance records
            var attendance = _staffRepo.GetAllAttendance(); // returns IEnumerable<AttendanceRecord>

            // Apply filters
            if (startDate.HasValue)
                attendance = attendance.Where(a => a.TimeIn.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                attendance = attendance.Where(a => a.TimeIn.Date <= endDate.Value.Date);
            if (!string.IsNullOrEmpty(staffName))
                attendance = attendance.Where(a => a.Name == staffName);
            if (!string.IsNullOrEmpty(status))
                attendance = attendance.Where(a => a.Status == status);

            // Sort descending by TimeIn
            var model = attendance.OrderByDescending(a => a.TimeIn).ToList();

            return View(model);
        }

        // Export filtered attendance to Excel
        public ActionResult ExportAttendance(DateTime? startDate, DateTime? endDate, string staffName, string status)
        {
            // Get all attendance records
            var attendance = _staffRepo.GetAllAttendance();

            // Apply filters if needed
            if (startDate.HasValue)
                attendance = attendance.Where(a => a.TimeIn.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                attendance = attendance.Where(a => a.TimeIn.Date <= endDate.Value.Date);
            if (!string.IsNullOrEmpty(staffName))
                attendance = attendance.Where(a => a.Name == staffName);
            if (!string.IsNullOrEmpty(status))
                attendance = attendance.Where(a => a.Status == status);

            // Generate CSV
            var csv = "Date,Staff Name,Check In,Check Out,Hours Worked,Status\n";

            foreach (var record in attendance.OrderByDescending(a => a.TimeIn))
            {
                double hoursWorked = record.TimeOut.HasValue ? (record.TimeOut.Value - record.TimeIn).TotalHours : 0;
                csv += $"{record.TimeIn:yyyy-MM-dd},{record.Name},{record.TimeIn:HH:mm},{(record.TimeOut.HasValue ? record.TimeOut.Value.ToString("HH:mm") : "")},{(hoursWorked > 0 ? hoursWorked.ToString("0.##") : "")},{record.Status}\n";
            }

            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(csv);
            string fileName = $"Attendance_{DateTime.Now:yyyyMMddHHmm}.csv";

            return File(fileBytes, "text/csv", fileName);
        }

        // Print: redirect to a print-friendly page with filters
        public ActionResult PrintAttendanceLog(DateTime? startDate, DateTime? endDate, string staffName, string status)
        {
            // Get all attendance records
            var attendance = _staffRepo.GetAllAttendance().AsEnumerable();

            // Apply filters
            if (startDate.HasValue)
                attendance = attendance.Where(a => a.TimeIn.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                attendance = attendance.Where(a => a.TimeIn.Date <= endDate.Value.Date);
            if (!string.IsNullOrEmpty(staffName))
                attendance = attendance.Where(a => a.Name == staffName);
            if (!string.IsNullOrEmpty(status))
                attendance = attendance.Where(a => a.Status == status);

            // Sort by most recent
            var model = attendance.OrderByDescending(a => a.TimeIn).ToList();

            return View("PrintAttendanceLog", model); // make sure the view exists
        }

        // Manual Entry: redirect to a form to add new attendance
        public ActionResult ManualAttendanceEntry()
        {
            // Pass list of staff for selection
            var staffList = _staffRepo.GetStaffDashboardData().StaffList;
            ViewBag.StaffList = staffList;
            return View(); // create ManualAttendanceEntry.cshtml
        }

        [HttpPost]
        public ActionResult ManualAttendanceEntry(AttendanceRecord record)
        {
            if (record != null && record.StaffID > 0)
            {
                // You need to implement AddAttendance in repository
                _staffRepo.AddAttendance(record);
                TempData["SuccessMessage"] = "Attendance recorded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to record attendance.";
            }
            return RedirectToAction("AttendanceLog");
        }

        // GET: Payroll
        public ActionResult Payroll(DateTime? startDate, DateTime? endDate)
        {
            // Default to current month if not specified
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? start.AddMonths(1).AddDays(-1);

            // Fetch payroll records from repository
            var payrollRecords = _staffRepo.GetPayrollRecords(start, end);

            // Pass the dates to ViewBag so the view can keep the selected values
            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");

            return View(payrollRecords);
        }

        public ActionResult ViewPayroll(int staffID, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? start.AddMonths(1).AddDays(-1);

            var record = _staffRepo.GetPayrollRecordByStaffID(staffID, start, end);

            if (record == null)
                return HttpNotFound();

            return View(record);
        }

        public ActionResult ExportExcel(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? start.AddMonths(1).AddDays(-1);

            var payrollRecords = _staffRepo.GetPayrollRecords(start, end).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Employee,Role,Rate,Days,BasePay,OvertimePay,Bonus,GrossPay,Deductions,NetPay,PayDate");

            foreach (var r in payrollRecords)
            {
                sb.AppendLine($"{r.Name},{r.Role},{r.Rate},{r.DaysWorked},{r.BasePay},{r.OvertimePay},{r.Bonus},{r.GrossPay},{r.Deductions},{r.NetPay},{r.PayDate:yyyy-MM-dd}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, "text/csv", "Payroll.csv");
        }


        public ActionResult PrintPayslips(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? start.AddMonths(1).AddDays(-1);

            var payrollRecords = _staffRepo.GetPayrollRecords(start, end);

            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");

            return View(payrollRecords); // Create a separate view for printing
        }


        public ActionResult EmailPayslips(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = endDate ?? start.AddMonths(1).AddDays(-1);

            var payrollRecords = _staffRepo.GetPayrollRecords(start, end);

            foreach (var p in payrollRecords)
            {
                // Replace with actual staff email column
                var staffEmail = _staffRepo.GetStaffEmail(p.StaffID);

                if (!string.IsNullOrEmpty(staffEmail))
                {
                    var message = new MailMessage();
                    message.To.Add(staffEmail);
                    message.Subject = $"Payslip: {p.PayDate:MMM yyyy}";
                    message.Body = $@"
                        Hello {p.Name},

                        Here is your payslip for {p.PayDate:MMM yyyy}:

                        Gross Pay: ₱{p.GrossPay:N0}
                        Deductions: ₱{p.Deductions:N0}
                        Net Pay: ₱{p.NetPay:N0}

                        Thank you.";

                    message.From = new MailAddress("jhoncarlosuan26@gmail.com", "labamoto Laundry Shop");

                    using (var smtp = new SmtpClient("smtp.gmail.com"))
                    {
                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential("jhoncarlosuan26@gmail.com", "tvqyqwjmlswfdjrc");
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                    }
                }
            }

            TempData["SuccessMessage"] = "Payslips sent successfully!";
            return RedirectToAction("Payroll", new { startDate = start, endDate = end });
        }

        public ActionResult PayrollHistory(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today.AddMonths(-1);
            DateTime end = endDate ?? DateTime.Today;

            var records = _staffRepo.GetPayrollRecords(start, end).ToList();

            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");

            return View(records);
        }

        // POST: StaffOwner/ProcessPayroll
        [HttpPost]
        public ActionResult ProcessPayroll(DateTime? startDate, DateTime? endDate)
        {
            // 1️⃣ Declare start and end here so they are accessible everywhere
            DateTime start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end = endDate ?? start.AddMonths(1).AddDays(-1);

            try
            {
                // 2️⃣ Fetch payroll records that are not processed yet
                var payrollRecords = _staffRepo.GetPayrollRecords(start, end)
                                               .Where(p => !p.IsProcessed)
                                               .ToList();

                // 3️⃣ Mark each record as processed
                foreach (var record in payrollRecords)
                {
                    record.IsProcessed = true;
                    record.ProcessedDate = DateTime.Now;

                    // Save changes using Dapper repository
                    _staffRepo.UpdatePayrollRecord(record);
                }

                TempData["SuccessMessage"] = "Payroll approved and processed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to process payroll: " + ex.Message;
            }

            // 4️⃣ Redirect back to Payroll page with same filters
            return RedirectToAction("Payroll", new { startDate = start, endDate = end });
        }

    }
}
