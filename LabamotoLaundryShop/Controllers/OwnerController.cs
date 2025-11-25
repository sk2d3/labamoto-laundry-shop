using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Services.Interfaces;
using LabamotoLaundryShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class OwnerController : Controller
    {
        // Owner Dashboard Page
        private readonly IOrderService _orderService;

        public OwnerController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public ActionResult Dashboard()
        {
            // Check if logged in
            if (Session["OwnerUsername"] == null)
                return RedirectToAction("OwnerLogin", "Account");

            ViewBag.OwnerName = Session["OwnerUsername"].ToString();

            var viewModel = new OwnerDashboardViewModel
            {
                TotalIncomeToday = _orderService.GetTotalIncomeToday(),
                TotalIncomeWeekly = _orderService.GetTotalIncomeWeekly(),
                TotalIncomeMonthly = _orderService.GetTotalIncomeMonthly(),
                ActiveOrders = _orderService.GetActiveOrdersCount(),
                TodaysOrders = _orderService.GetTodaysOrdersCount(),
                QueuedCount = _orderService.GetOrdersCountByStatus("Queued"),
                WashingCount = _orderService.GetOrdersCountByStatus("Washing"),
                DryingCount = _orderService.GetOrdersCountByStatus("Drying"),
                ReadyCount = _orderService.GetOrdersCountByStatus("Ready"),
                Alerts = new List<AlertViewModel>
                {
                    new AlertViewModel { Message = "Low detergent stock!", Link = Url.Action("Index", "Inventory") },
                    new AlertViewModel { Message = "New customer registered", Link = Url.Action("Index", "Customers") },
                    new AlertViewModel { Message = "Pending payroll approvals", Link = Url.Action("Index", "StaffOwner") }
                }
            };



            return View(viewModel);
        }
    }
}
