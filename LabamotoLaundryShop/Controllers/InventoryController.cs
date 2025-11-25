using Dapper;
using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryRepository _inventoryRepo;
        private readonly PurchaseOrderRepository _poRepo;

        public InventoryController()
        {
            var context = new DapperContext();
            _inventoryRepo = new InventoryRepository(context);
            _poRepo = new PurchaseOrderRepository(context);
        }

        // GET: Inventory
        public ActionResult Index(string category = "")
        {
            var items = _inventoryRepo.GetAllInventory();
            var categories = _inventoryRepo.GetCategories();

            if (!string.IsNullOrEmpty(category))
            {
                items = items.Where(i => i.Category == category).ToList();
            }

            var model = new InventoryViewModel
            {
                InventoryItems = items,
                TotalItems = items.Count,
                LowStockItems = items.Count(i => i.Status == "Low"),
                OutOfStockItems = items.Count(i => i.Status == "OUT"),
                MonthlyUsageCost = ComputeMonthlyUsageCost(items)
            };

            ViewBag.Categories = categories; // pass categories to Razor
            ViewBag.SelectedCategory = category; // preserve selection

            return View(model);
        }

        // POST: Inventory/Add
        [HttpPost]
        public ActionResult Add(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                _inventoryRepo.AddInventoryItem(item);
                TempData["Success"] = "✅ Inventory item added successfully!";
            }
            else
            {
                TempData["Error"] = "⚠️ Failed to add inventory item.";
            }

            return RedirectToAction("Index");
        }

        // POST: Inventory/Update
        [HttpPost]
        public ActionResult Update(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                _inventoryRepo.UpdateInventoryItem(item);
                TempData["Success"] = "✅ Inventory item updated successfully!";
            }
            else
            {
                TempData["Error"] = "⚠️ Failed to update inventory item.";
            }

            return RedirectToAction("Index");
        }

        // POST: Inventory/CreatePurchaseOrder
        [HttpPost]
        public ActionResult CreatePurchaseOrder(PurchaseOrder order)
        {
            if (ModelState.IsValid)
            {
                // Auto-calculate total cost if not provided
                if (order.TotalCost <= 0 && order.UnitCost > 0 && order.OrderQuantity > 0)
                {
                    order.TotalCost = Math.Round(order.UnitCost * order.OrderQuantity, 2);
                }

                _poRepo.CreatePurchaseOrder(order);
                TempData["Success"] = "✅ Purchase order created successfully!";
            }
            else
            {
                TempData["Error"] = "⚠️ Failed to create purchase order.";
            }

            return RedirectToAction("Index");
        }

        public ActionResult PurchaseHistory(string category = "", string supplier = "", string status = "")
        {
            var orders = _poRepo.GetAllPurchaseOrders();

            if (!string.IsNullOrEmpty(category))
                orders = orders.Where(x => x.Category == category).ToList();
            if (!string.IsNullOrEmpty(supplier))
                orders = orders.Where(x => x.Supplier == supplier).ToList();
            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(x => x.Status == status).ToList();

            return View(orders);
        }

        // GET: Inventory/GenerateReport
        // GET: Inventory/GenerateReport
        public ActionResult GenerateReport(int? period, string type, string category, bool charts = false, bool print = false)
        {
            // Get all inventory items
            var inventoryItems = _inventoryRepo.GetAllInventory();

            // Filter by category if selected
            if (!string.IsNullOrEmpty(category))
            {
                inventoryItems = inventoryItems.Where(i => i.Category == category).ToList();
            }

            // Optional: filter by period if you have a LastUpdated or CreatedAt field
            if (period.HasValue)
            {
                DateTime startDate = DateTime.Now.AddDays(-period.Value);
                // Example: inventoryItems = inventoryItems.Where(i => i.LastUpdated >= startDate).ToList();
            }

            // Pass selected filters to ViewBag for Razor
            ViewBag.SelectedPeriod = period ?? 30;
            ViewBag.SelectedType = type ?? "full";
            ViewBag.SelectedCategory = category ?? "";
            ViewBag.Categories = _inventoryRepo.GetCategories();
            ViewBag.PrintMode = print; // important

            if (charts)
            {
                // Create chart ViewModel
                var chartData = inventoryItems
                    .GroupBy(i => i.Category)
                    .Select(g => new InventoryChartData
                    {
                        Category = g.Key,
                        TotalStock = g.Sum(x => x.CurrentStock),
                        LowStockCount = g.Count(x => x.Status == "Low"),
                        OutOfStockCount = g.Count(x => x.Status == "OUT")
                    }).ToList();

                return View("ReportCharts", chartData);
            }

            return View(inventoryItems);
        }


        // Helper: compute monthly usage cost for dashboard
        private decimal ComputeMonthlyUsageCost(List<InventoryItem> items)
        {
            decimal total = 0;

            foreach (var item in items)
            {
                if (item.CurrentStock > 0 && item.MinLevel > 0 && item.UnitCost > 0)
                {
                    decimal dailyUsage = item.MinLevel / 30; // min level as one-month threshold
                    decimal monthlyUsage = dailyUsage * 30;
                    total += monthlyUsage * item.UnitCost;
                }
            }

            return Math.Round(total, 2);
        }

        // Helper: suggest order quantity (to display in modal)
        public int SuggestOrderQuantity(decimal currentStock, decimal minLevel)
        {
            return (int)Math.Ceiling(minLevel - currentStock) > 0 ? (int)Math.Ceiling(minLevel - currentStock) : 1;
        }

        public ActionResult FilterPurchaseOrders(string category, string supplier, string status)
        {
            var orders = _poRepo.GetAllPurchaseOrders();

            if (!string.IsNullOrEmpty(category))
                orders = orders.Where(x => x.Category == category).ToList();
            if (!string.IsNullOrEmpty(supplier))
                orders = orders.Where(x => x.Supplier == supplier).ToList();
            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(x => x.Status == status).ToList();

            return PartialView("_PurchaseOrderRows", orders);
        }

        public ActionResult ViewPurchaseOrder(int id)
        {
            var po = _poRepo.GetAllPurchaseOrders().FirstOrDefault(x => x.Id == id);
            if (po == null) return HttpNotFound();

            return View(po); // Returns a view showing details
        }

        public ActionResult ExportExcel(string category = "", string supplier = "", string status = "")
        {
            var orders = _poRepo.GetAllPurchaseOrders();

            // Apply filters
            if (!string.IsNullOrEmpty(category))
                orders = orders.Where(x => x.Category == category).ToList();
            if (!string.IsNullOrEmpty(supplier))
                orders = orders.Where(x => x.Supplier == supplier).ToList();
            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(x => x.Status == status).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("PO#,Date,Item,Supplier,Quantity,TotalCost,Status");

            foreach (var po in orders)
            {
                sb.AppendLine($"{po.Id},{po.CreatedAt:MMM d, yyyy},{po.ItemName},{po.Supplier},{po.OrderQuantity} {po.Unit},{po.TotalCost},{po.Status}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, "text/csv", "PurchaseOrders.csv");
        }

        // PRINT TO PDF
        public ActionResult PrintPDF(int? period, string type, string category)
        {
            var model = BuildReport(period, type, category);

            return new Rotativa.ViewAsPdf("ReportPDF", model)
            {
                FileName = $"Inventory_Report_{DateTime.Now:yyyyMMdd}.pdf",
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        // REPORT BUILDER (Used by GenerateReport and PDF)
        private InventoryReportVM BuildReport(int? period, string type, string category)
        {
            var items = _inventoryRepo.GetAllInventory();

            // Filter category
            if (!string.IsNullOrEmpty(category))
                items = items.Where(i => i.Category == category).ToList();

            return new InventoryReportVM
            {
                Items = items,
                SelectedPeriod = period ?? 30,
                SelectedType = type ?? "full",
                SelectedCategory = category ?? ""
            };
        }

        [HttpPost]
        [Route("Inventory/EmailReport")]
        public ActionResult EmailReport(int period, string type, string category, string recipientEmail)
        {
            // Build the report model
            var model = BuildInventoryReportViewModel(period, type, category);

            // Generate PDF
            var pdfResult = new Rotativa.ViewAsPdf("ReportPDF", model)
            {
                FileName = $"Inventory_Report_{DateTime.Now:yyyyMMdd}.pdf",
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4
            };

            byte[] pdfBytes = pdfResult.BuildFile(ControllerContext);

            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("jhoncarlosuan26@gmail.com", "Labamoto Laundry Shop");
                    message.To.Add(recipientEmail);
                    message.Subject = $"Inventory Report ({type.ToUpper()})";
                    message.Body = "Attached is the latest inventory report.";
                    message.IsBodyHtml = true;
                    message.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Inventory_Report.pdf"));

                    using (var smtp = new SmtpClient("smtp.gmail.com"))
                    {
                        smtp.Port = 587;
                        smtp.Credentials = new System.Net.NetworkCredential("jhoncarlosuan26@gmail.com", "tvqyqwjmlswfdjrc");
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                    }
                }

                TempData["SuccessMessage"] = "✅ Inventory report emailed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"⚠️ Failed to send email. {ex.Message}";
            }

            return RedirectToAction("GenerateReport", new { period, type, category });
        }

        // Helper to build the report VM
        private InventoryReportVM BuildInventoryReportViewModel(int period, string type, string category)
        {
            var items = _inventoryRepo.GetAllInventory();
            if (!string.IsNullOrEmpty(category))
                items = items.Where(x => x.Category == category).ToList();

            return new InventoryReportVM
            {
                Items = items,
                SelectedPeriod = period,
                SelectedType = type,
                SelectedCategory = category
            };
        }

    }
}
