using ArtGallery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArtGallery.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["ActiveNav"] = "Home";
            return View();
        }

        [AllowAnonymous]
        [Route("/Forbidden")]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
