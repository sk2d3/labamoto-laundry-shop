using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class StaffController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["StaffUsername"] == null)
                return RedirectToAction("StaffLogin", "Account");

            return View();
        }
    }
}