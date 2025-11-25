using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LabamotoLaundryShop.Controllers
{
    public class AccountController : Controller
    {
        // ======================
        // ROLE CHOOSER
        // ======================
        public ActionResult ChooseRole()
        {
            return View();
        }

        // ======================
        // OWNER LOGIN
        // ======================
        public ActionResult OwnerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OwnerLogin(string Username, string Password)

        {
            string ownerUsername = "owner";
            string ownerPassword = "labamoto123";

            if (Username == ownerUsername && Password == ownerPassword)
            {
                Session["OwnerUsername"] = ownerUsername;
                return RedirectToAction("Dashboard", "Owner");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // ======================
        // STAFF LOGIN
        // ======================
        public ActionResult StaffLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StaffLogin(string Username, string Password)
        {
            string staffUsername = "staff";
            string staffPassword = "labamotostaff123";

            if (Username == staffUsername && Password == staffPassword)
            {
                Session["StaffUsername"] = staffUsername;
                return RedirectToAction("Dashboard", "Staff");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // ======================
        // LOGOUT
        // ======================
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("ChooseRole", "Account");
        }
    }
}
