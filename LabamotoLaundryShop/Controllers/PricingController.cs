using iTextSharp.text;
using iTextSharp.text.pdf;
using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace LabamotoLaundryShop.Controllers
{
    public class PricingController : Controller
    {
        private readonly PricingRepository _repo;

        public PricingController()
        {
            _repo = new PricingRepository(new Data.DapperContext());
        }

        // ==========================
        // INDEX
        // ==========================
        public ActionResult Index(string currentAction = "RegularLaundry")
        {
            var model = new PricingViewModel
            {
                RegularLaundry = _repo.GetAllRegularLaundry(),
                SpecialItems = _repo.GetAllSpecialItems(),
                DryClean = _repo.GetAllDryClean(),
                AddOns = _repo.GetAllAddOns(),
                RushServiceFee = _repo.GetFee("RushService"),
                PickupFee = _repo.GetFee("PickupFee"),
                Discount = _repo.GetFee("Discount"),
                VAT = _repo.GetFee("VAT")
            };
            ViewBag.CurrentAction = currentAction;
            return View(model);
        }

        // ==========================
        // TAB ACTIONS (to fix 404 errors)
        // ==========================
        public ActionResult RegularLaundry()
        {
            var model = new PricingViewModel
            {
                RegularLaundry = _repo.GetAllRegularLaundry(),
                SpecialItems = _repo.GetAllSpecialItems(),
                DryClean = _repo.GetAllDryClean(),
                AddOns = _repo.GetAllAddOns(),
                RushServiceFee = _repo.GetFee("RushService"),
                PickupFee = _repo.GetFee("PickupFee"),
                Discount = _repo.GetFee("Discount"),
                VAT = _repo.GetFee("VAT")
            };
            return View("Index", model); // Reuse Index.cshtml
        }

        public ActionResult SpecialItems()
        {
            var model = new PricingViewModel
            {
                RegularLaundry = _repo.GetAllRegularLaundry(),
                SpecialItems = _repo.GetAllSpecialItems(),
                DryClean = _repo.GetAllDryClean(),
                AddOns = _repo.GetAllAddOns(),
                RushServiceFee = _repo.GetFee("RushService"),
                PickupFee = _repo.GetFee("PickupFee"),
                Discount = _repo.GetFee("Discount"),
                VAT = _repo.GetFee("VAT")
            };
            return View("Index", model);
        }

        public ActionResult DryClean()
        {
            var model = new PricingViewModel
            {
                RegularLaundry = _repo.GetAllRegularLaundry(),
                SpecialItems = _repo.GetAllSpecialItems(),
                DryClean = _repo.GetAllDryClean(),
                AddOns = _repo.GetAllAddOns(),
                RushServiceFee = _repo.GetFee("RushService"),
                PickupFee = _repo.GetFee("PickupFee"),
                Discount = _repo.GetFee("Discount"),
                VAT = _repo.GetFee("VAT")
            };
            return View("Index", model);
        }

        public ActionResult AddOns()
        {
            var model = new PricingViewModel
            {
                RegularLaundry = _repo.GetAllRegularLaundry(),
                SpecialItems = _repo.GetAllSpecialItems(),
                DryClean = _repo.GetAllDryClean(),
                AddOns = _repo.GetAllAddOns(),
                RushServiceFee = _repo.GetFee("RushService"),
                PickupFee = _repo.GetFee("PickupFee"),
                Discount = _repo.GetFee("Discount"),
                VAT = _repo.GetFee("VAT")
            };
            return View("Index", model);
        }

        // ==========================
        // REGULAR LAUNDRY CRUD
        // ==========================
        [HttpPost]
        public ActionResult AddPackage(PricingPackage model)
        {
            if (ModelState.IsValid)
                _repo.AddRegular(model);
            return RedirectToAction("Index", new { actionType = "RegularLaundry" });
        }

        [HttpPost]
        public ActionResult EditPackage(PricingPackage model)
        {
            if (ModelState.IsValid)
                _repo.EditRegular(model);
            return RedirectToAction("Index", new { actionType = "RegularLaundry" });
        }


        [HttpGet]
        public ActionResult TogglePackageStatus(int PackageID)
        {
            _repo.ToggleRegularStatus(PackageID);
            TempData["SuccessMessage"] = "Package updated successfully!";
            return RedirectToAction("Index", new { actionType = "RegularLaundry" });
        }

        // ==========================
        // SPECIAL ITEMS CRUD
        // ==========================
        [HttpPost]
        public ActionResult AddSpecialItem(SpecialItem item)
        {
            if (ModelState.IsValid)
                _repo.AddSpecialItem(item);
            return RedirectToAction("Index", new { currentAction = "SpecialItems" });
        }
        [HttpPost]
        public ActionResult EditSpecialItem(SpecialItem model)
        {
            if (ModelState.IsValid)
                _repo.UpdateSpecialItem(model);
            return RedirectToAction("Index", new { currentAction = "SpecialItems" });
        }

        [HttpPost]
        public ActionResult DeleteSpecialItem(int SpecialItemID)
        {
            _repo.DeleteSpecialItem(SpecialItemID);
            return RedirectToAction("Index", new { currentAction = "SpecialItems" });
        }

        // ==========================
        // DRY CLEAN CRUD
        // ==========================
        [HttpPost]
        public ActionResult AddDryClean(DryCleanItem item)
        {
            if (ModelState.IsValid)
                _repo.AddDryCleanItem(item);
            return RedirectToAction("Index", new { currentAction = "DryClean" });
        }
        [HttpPost]
        public ActionResult EditDryClean(DryCleanItem model)
        {
            if (ModelState.IsValid)
                _repo.UpdateDryClean(model);
            return RedirectToAction("Index", new { currentAction = "DryClean" });
        }

        // ==========================
        // ADD-ONS CRUD
        // ==========================
        [HttpPost]
        public ActionResult AddAddOn(AddOnItem item)
        {
            if (ModelState.IsValid)
                _repo.AddAddOn(item);
            return RedirectToAction("Index", new { currentAction = "AddOns" });
        }
        [HttpPost]
        public ActionResult EditAddOn(AddOnItem model)
        {
            if (ModelState.IsValid)
                _repo.UpdateAddOn(model);
            return RedirectToAction("Index", new { currentAction = "AddOns" });
        }

        // ==========================
        // FEES / SURCHARGES
        // ==========================
        //[HttpPost]
        //public ActionResult UpdateFees(decimal RushServiceFee, decimal PickupFee, decimal Discount, decimal VAT)
        //{
        //_repo.UpdateFee("RushService", RushServiceFee);
        //_repo.UpdateFee("PickupFee", PickupFee);
        //_repo.UpdateFee("Discount", Discount);
        //_repo.UpdateFee("VAT", VAT);
        //return RedirectToAction("Index", new { currentAction = "Fees" }); // Optional: if you have a fees tab
        //}
        [HttpPost]
        public ActionResult UpdateFees(decimal RushServiceFee, decimal PickupFee, decimal FreeDeliveryMinimum, decimal Discount, decimal  LoyaltyDiscount, decimal VAT)
        {
            _repo.UpdateFee("RushService", RushServiceFee);
            _repo.UpdateFee("PickupFee", PickupFee);
            _repo.UpdateFee("DeliveryMinimum", FreeDeliveryMinimum);
            _repo.UpdateFee("Discount", Discount);
            _repo.UpdateFee("LoyaltyDiscount", LoyaltyDiscount);
            _repo.UpdateFee("VAT", VAT);

            TempData["SuccessMessage"] = "Fees updated successfully!";
            return RedirectToAction("Index", new { currentAction = "AddOns" });
        }

        // ========================
        // 1️⃣ EXPORT REGULAR LAUNDRY
        // ========================
        public ActionResult ExportRegularLaundryPDF()
        {
            var regularData = _repo.GetRegularLaundry();

            string[] headers = { "Service Package", "Price per kg", "Unit", "Status" };

            return GeneratePDF(
                "Regular Laundry Pricing List",
                headers,
                regularData.Select(item => new[] {
            item.PackageName,
            item.PricePerKg.ToString("C"),
            item.Unit ?? "",
            item.Status ?? "Active"
                }).ToList()
            );
        }

        // ========================
        // 2️⃣ EXPORT SPECIAL ITEMS
        // ========================
        public ActionResult ExportSpecialItemsPDF()
        {
            var specialItems = _repo.GetSpecialItems();

            string[] headers = { "Item Type", "Price per pc", "Category", "Status" };

            return GeneratePDF(
                "Special Items Pricing List",
                headers,
                specialItems.Select(item => new[] {
            item.ItemName,
            item.PricePerPiece.ToString("C"),
            item.Category ?? "",
            item.Status ?? "Active"
                }).ToList()
            );
        }

        // ========================
        // 3️⃣ EXPORT DRY CLEAN
        // ========================
        public ActionResult ExportDryCleanPDF()
        {
            var dryClean = _repo.GetDryCleanItems();

            string[] headers = { "Item Type", "Price per pc", "Processing Time", "Status" };

            return GeneratePDF(
                "Dry Clean Pricing List",
                headers,
                dryClean.Select(item => new[] {
            item.ItemName,
            item.PricePerPiece.ToString("C"),
            item.ProcessingTime ?? "",
            item.Status ?? "Active"
                }).ToList()
            );
        }

        // ========================
        // 4️⃣ EXPORT ADD-ONS
        // ========================
        public ActionResult ExportAddOnsPDF()
        {
            var addOns = _repo.GetAddOns();

            string[] headers = { "Service Name", "Price", "Price Type", "Status" };

            return GeneratePDF(
                "Add-On Services Pricing List",
                headers,
                addOns.Select(item => new[] {
            item.ServiceName,
            item.Price ?? "",
            item.PriceType ?? "",
            item.Status ?? "Active"
                }).ToList()
            );
        }

        private FileContentResult GeneratePDF(string title, string[] headers, List<string[]> data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph(title));
                pdfDoc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString("g")));
                pdfDoc.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(headers.Length);
                table.WidthPercentage = 100;

                // Add header cells
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY
                    };
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var row in data)
                {
                    foreach (var cell in row)
                    {
                        table.AddCell(cell);
                    }
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", $"{title}.pdf");
            }
        }

        // ========================
        // IMPORT PRICING FROM CSV
        // ========================
        [HttpPost]
        public ActionResult ImportCSV(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (var reader = new StreamReader(file.InputStream))
                {
                    bool isFirstLine = true;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        // Skip header row if present
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            if (line.Contains("Service") || line.Contains("Price"))
                                continue;
                        }

                        var values = line.Split(',');

                        if (values.Length < 2)
                            continue; // skip invalid rows

                        // Create PricingPackage from CSV columns
                        var pricingItem = new PricingPackage
                        {
                            PackageName = values[0],
                            PricePerKg = decimal.TryParse(values[1], out decimal price) ? price : 0,
                            Unit = values.Length > 2 ? values[2] : "kg", // default to kg if not provided
                            Status = values.Length > 3 ? values[3] : "Active" // default to Active
                        };

                        _repo.AddOrUpdatePricing(pricingItem);
                    }
                }

                TempData["SuccessMessage"] = "Pricing imported successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a CSV file to import.";
            }

            return RedirectToAction("Index");
        }

    }
}
