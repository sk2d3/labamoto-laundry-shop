using LabamotoLaundryShop.Models;
using LabamotoLaundryShop.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LabamotoLaundryShop.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerRepository _repo;

        // Constructor
        public CustomersController()
        {
            _repo = new CustomerRepository(new Data.DapperContext());
        }

        // GET: Customers
        public ActionResult Index()
        {
            var customers = _repo.GetAllCustomers().ToList(); // convert to List

            var model = new CustomerViewModel
            {
                Customers = customers,
                TotalCustomers = customers.Count,
                VIPCustomers = customers.Count(c => c.CustomerType == "VIP"),
                CorporateAccounts = customers.Count(c => c.CustomerType == "Corporate"),
                NewThisMonth = customers.Count(c => c.LastVisit.HasValue && c.LastVisit.Value.Month == DateTime.Now.Month)
            };

            return View(model);
        }

        // POST: Customers/Create
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var repo = new CustomerRepository(new Data.DapperContext());

                // Make sure TotalOrders and TotalSpent are passed correctly
                repo.CreateCustomer(customer);

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to add customer.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var repo = new CustomerRepository(new Data.DapperContext());

                // Call repository method to update customer
                repo.UpdateCustomer(customer);

                // Redirect back to the Index page
                return RedirectToAction("Index");
            }

            // If validation fails, reload the page
            TempData["Error"] = "Failed to update customer.";
            return RedirectToAction("Index");
        }

        public ActionResult ExportExcel()
        {
            var customers = _repo.GetAllCustomers().ToList(); // Get all customers

            var sb = new StringBuilder();
            sb.AppendLine("CustomerID,FullName,ContactNumber,Email,CustomerType,TotalOrders,TotalSpent");

            foreach (var c in customers)
            {
                sb.AppendLine($"{c.CustomerID},{c.FullName},{c.ContactNumber},{c.Email},{c.CustomerType},{c.TotalOrders},{c.TotalSpent}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, "text/csv", "Customers.csv");
        }
    }
}
