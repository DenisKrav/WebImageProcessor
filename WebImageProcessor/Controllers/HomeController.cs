using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebImageProcessor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile image)
        {
            if (image == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }
        }
    }
}