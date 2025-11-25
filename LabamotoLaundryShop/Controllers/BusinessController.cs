using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class BusinessController : Controller
    {
        private readonly BusinessSettingsRepository _repo;

        public BusinessController()
        {
            var context = new DapperContext();
            _repo = new BusinessSettingsRepository(context);
        }

        // GET: Business/Index
        public ActionResult Index()
        {
            // Load all settings from the database
            var model = _repo.GetSettings();

            // Set the active tab from TempData or default to "general-info"
            ViewBag.ActiveTab = TempData["ActiveTab"] ?? "general-info";

            return View(model);
        }


        // POST: Save All Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAll(BusinessSettingsViewModel model, string ActiveTab)
        {
            if (ModelState.IsValid)
            {
                // Convert checkbox values
                foreach (var item in model.GeneralInfo)
                {
                    if (item.SettingKey == "IncludeQRCode" || item.SettingKey == "PrintLogo")
                        item.SettingValue = item.SettingValue == "true" ? "true" : "false";
                }

                _repo.SaveGeneralInfo(model.GeneralInfo ?? new List<GeneralInfo>());
                _repo.SaveBusinessHours(model.BusinessHours ?? new List<BusinessHour>());
                _repo.SaveHolidays(model.Holidays ?? new List<Holiday>());
                _repo.SavePaymentMethods(model.PaymentMethods ?? new List<PaymentMethod>());
                _repo.SaveSystemConfig(model.SystemConfig ?? new List<SystemConfig>());

                TempData["SuccessMessage"] = "✅ Business settings saved successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "⚠️ Failed to save settings.";
            }

            // Keep the active tab after redirect
            TempData["ActiveTab"] = ActiveTab;

            return RedirectToAction("Index");
        }

        // ========================
        // BACKUP NOW
        // ========================
        [HttpPost]
        public ActionResult BackupNow()
        {
            try
            {
                // 1. Create the backup file
                string backupFile = _repo.CreateBackup();

                // 2. Update LastBackup in systemconfig
                var lastBackupSetting = _repo.GetSettings().SystemConfig
                    .Find(s => s.SettingKey == "LastBackup");

                if (lastBackupSetting != null)
                {
                    lastBackupSetting.SettingValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _repo.SaveSystemConfig(new List<SystemConfig> { lastBackupSetting });
                }

                // 3. Generate relative path for download link
                string relativePath = Url.Content("~/Backups/" + Path.GetFileName(backupFile));

                // 4. Show success message with download link
                TempData["SuccessMessage"] = $"✅ Backup created successfully: <a href='{relativePath}' target='_blank'>Download</a>";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"⚠️ Backup failed: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // ========================
        // EXPORT CSV
        // ========================
        [HttpGet]
        public ActionResult ExportCSV()
        {
            var orders = _repo.GetOrders(); // returns List<Order>
            var sb = new StringBuilder();
            sb.AppendLine("OrderID,OrderNumber,CustomerID,OrderDate,TotalAmount");

            foreach (var o in orders)
            {
                sb.AppendLine($"{o.OrderID},{o.OrderNumber},{o.CustomerID},{o.OrderDate:yyyy-MM-dd},{o.TotalAmount}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "orders.csv");
        }

        // ========================
        // IMPORT CSV
        // ========================
        [HttpPost]
        public ActionResult ImportCSV(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    using (var reader = new StreamReader(file.InputStream))
                    {
                        string line;
                        bool isHeader = true;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (isHeader)
                            {
                                isHeader = false; // skip header row
                                continue;
                            }

                            var values = line.Split(',');
                            if (values.Length >= 4) // basic validation
                            {
                                _repo.InsertOrderFromCSV(values);
                            }
                        }
                    }
                    TempData["SuccessMessage"] = "✅ CSV imported successfully!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"⚠️ CSV import failed: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "⚠️ No file selected.";
            }

            return RedirectToAction("Index");
        }
    }
}
