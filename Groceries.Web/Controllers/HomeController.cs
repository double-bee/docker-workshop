using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Groceries.Web.Models;
using System.Net.Http;

namespace Groceries.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGroceryService _groceryService;

        public HomeController(IGroceryService groceryService)
        {
            _groceryService = groceryService;
        }

        public async Task<IActionResult> Index()
        {
            
            return Content("De website werkt.");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
