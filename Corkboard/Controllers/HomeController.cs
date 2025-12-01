using System.Diagnostics;
using Corkboard.Models;
using Corkboard.Models.ViewModels.HomeController;
using Microsoft.AspNetCore.Mvc;

namespace Corkboard.Controllers
{
    /// <summary>
    /// Controller for general application pages such as home, privacy, and error views.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">Logger for diagnostic information.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the application home page.
        /// </summary>
        /// <returns>Home view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>Privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page with request tracking information.
        /// </summary>
        /// <returns>Error view with diagnostic information.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
