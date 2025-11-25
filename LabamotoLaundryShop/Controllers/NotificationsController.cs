using LabamotoLaundryShop.Data;
using LabamotoLaundryShop.Models;
using System;
using System.Collections.Generic;
using LabamotoLaundryShop.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly NotificationRepository _repo;

        public NotificationsController()
        {
            _repo = new NotificationRepository(new DapperContext());
        }

        public ActionResult Index()
        {
            var templates = _repo.GetAllTemplates();
            var settings = _repo.GetSettings();

            var model = new NotificationsViewModel
            {
                Templates = templates.ToList(),
                Settings = settings
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveSettings(NotificationsViewModel model)
        {
            // Save your settings to the database here
            TempData["Success"] = "Settings saved successfully!";
            return RedirectToAction("Index"); // Redirect back to the page
        }


        [HttpPost]
        public ActionResult PurchaseCredits(int credits)
        {
            if (credits <= 0)
            {
                TempData["Success"] = "Please enter a valid number of credits.";
                return RedirectToAction("Index");
            }

            _repo.AddSmsCredits(credits);

            TempData["Success"] = $"{credits} SMS Credits added successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult TestSendSMS(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message cannot be empty.";
                return RedirectToAction("Index");
            }

            bool success = SmsService.SendTest(message); // your SMS logic

            if (success)
                TempData["Success"] = "Test SMS sent successfully!";
            else
                TempData["Error"] = "Failed to send test SMS.";

            return RedirectToAction("Index"); // redirect back to your page
        }

    }
}