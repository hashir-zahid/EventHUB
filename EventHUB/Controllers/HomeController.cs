using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EventHUB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GuestDashboard()
        {
            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult MemberDashboard()
        {
            return View();
        }
        public IActionResult Eventlist()
        {
            return View();
        }

    }
}
