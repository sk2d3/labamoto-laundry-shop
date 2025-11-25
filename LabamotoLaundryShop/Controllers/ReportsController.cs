using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IOrderService _orderService;

        public ReportsController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Reports
        public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            // Use today as default if no date range selected
            var start = startDate ?? DateTime.Today.AddDays(-7); // last 7 days
            var end = endDate ?? DateTime.Today;

            // Get transaction history
            var orders = _orderService.GetOrders(start, end)
                                      .OrderByDescending(o => o.OrderDate)
                                      .ToList();

            // Get income over time (grouped by date)
            var incomeData = _orderService.GetIncomeOverTime(start, end);

            // Pass data to View
            ViewBag.Orders = orders;
            ViewBag.IncomeData = incomeData;
            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");

            return View();
        }
    }
}
