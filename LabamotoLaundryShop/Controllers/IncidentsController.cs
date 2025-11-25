using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Services.Implementations;
using LabamotoLaundryShop.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class IncidentsController : Controller
    {
        private readonly IIncidentService _incidentService;
        private readonly IOrderService _orderService;

        public IncidentsController(IIncidentService incidentService, IOrderService orderService)
        {
            _incidentService = incidentService;
            _orderService = orderService;
        }

        // GET: Incidents
        public ActionResult Index(DateTime? startDate, DateTime? endDate, string status = "All", string severity = "All")
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var incidents = _incidentService.GetIncidents(start, end);

            if (!string.IsNullOrEmpty(status) && status != "All")
                incidents = incidents.Where(i => i.Status == status).ToList();

            if (!string.IsNullOrEmpty(severity) && severity != "All")
                incidents = incidents.Where(i => i.Severity == severity).ToList();

            ViewBag.Incidents = incidents;
            ViewBag.Orders = _orderService.GetOrders();

            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedSeverity = severity;

            return View();
        }

        // POST: Incidents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Incident incident)
        {
            if (ModelState.IsValid)
            {
                _incidentService.CreateIncident(incident);
                TempData["SuccessMessage"] = "Incident logged successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Orders = _orderService.GetOrders();
            return View(incident);
        }

        // GET: Incidents/View (for modal)
        public ActionResult View(int id)
        {
            var inc = _incidentService.GetIncidentById(id);
            if (inc == null) return HttpNotFound();

            // Fetch staff names
            var reporter = _incidentService.GetStaffById(inc.ReportedByStaffID ?? 0);
            var resolver = _incidentService.GetStaffById(inc.ResolvedByStaffID ?? 0);


            inc.ReportedByName = reporter?.FullName;
            inc.ResolvedByName = resolver?.FullName;

            return PartialView("_ViewIncidentModal", inc);
        }

        [HttpGet]
        public JsonResult GetIncident(int id)
        {
            var incident = _incidentService.GetIncidentByIdWithDetails(id);

            return Json(new
            {
                incidentId = incident.IncidentID,
                orderId = incident.OrderID,
                customerName = incident.CustomerName,
                reportedDate = incident.ReportedDate.ToString("MMM d, yyyy"),
                staffName = incident.StaffName,
                incidentType = incident.IncidentType,
                severity = incident.Severity,
                summary = incident.IssueSummary,
                description = incident.Description,
                value = incident.EstimatedItemValue
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStatus()
        {
            try
            {
                // Read form values
                var incidentIdStr = Request.Form["IncidentID"];
                if (!int.TryParse(incidentIdStr, out int incidentId))
                    return Json(new { success = false, message = "Invalid Incident ID." });

                var status = Request.Form["Status"];
                var resolutionType = Request.Form["ResolutionType"];
                var resolutionDetails = Request.Form["ResolutionDetails"];
                var actionTaken = Request.Form["ActionTaken"];
                var satisfaction = Request.Form["Satisfaction"];
                var internalNotes = Request.Form["InternalNotes"];
                var compensationStr = Request.Form["Compensation"];
                decimal.TryParse(compensationStr, out decimal compensation);

                // Get existing incident
                var incident = _incidentService.GetIncidentById(incidentId);
                if (incident == null)
                    return Json(new { success = false, message = "Incident not found." });

                // Update incident fields
                incident.Status = status;
                incident.Resolution = resolutionType;
                incident.ResolutionDetails = resolutionDetails;
                incident.ActionTaken = actionTaken;
                incident.Satisfaction = satisfaction;
                incident.InternalNotes = internalNotes;
                incident.Compensation = compensation;

                // Set ResolvedByStaffID and ResolvedDate if status is "Resolved"
                if (status == "Resolved")
                {
                    var currentStaffIdObj = Session["StaffID"];
                    if (currentStaffIdObj == null)
                        return Json(new { success = false, message = "Staff not logged in." });

                    incident.ResolvedByStaffID = (int)currentStaffIdObj;
                    incident.ResolvedDate = DateTime.Now;
                }

                // Save changes
                _incidentService.UpdateIncident(incident);

                return Json(new
                {
                    success = true,
                    incidentId = incident.IncidentID,
                    status = incident.Status
                });
            }
            catch (Exception ex)
            {
                // For debugging: you can log it somewhere
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { success = false, message = "Failed to update incident." });
            }
        }


    }
}
