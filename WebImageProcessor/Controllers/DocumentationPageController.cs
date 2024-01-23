using Microsoft.AspNetCore.Mvc;

namespace WebImageProcessor.Controllers
{
    public class DocumentationPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
