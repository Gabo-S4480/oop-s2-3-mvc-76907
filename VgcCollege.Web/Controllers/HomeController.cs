using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
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
            // Redirection logic based on User Roles
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "StudentProfiles");
                }

                if (User.IsInRole("Faculty"))
                {
                    return RedirectToAction("MyDashboard", "FacultyProfiles");
                }

                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("MyGrades", "StudentProfiles");
                }
            }

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