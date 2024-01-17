using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebImageProcessor.Models;
using WebImageProcessor.ViewModel;

namespace WebImageProcessor.Controllers
{
    public class StatisticPageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _appConfig;
        private readonly ImageProcessorDbContext db;

        public StatisticPageController(ILogger<HomeController> logger, IConfiguration appConfig, ImageProcessorDbContext context) 
        {
            _logger = logger;
            _appConfig = appConfig;
            db = context;
        }

        public ActionResult Index()
        {
            StatisticViewModel viewModel = new StatisticViewModel();

            viewModel.TotalNumRegUsers = db.AppUsers.Count();
            viewModel.TotalNumProcesImg = db.UserRequests.Count();

            return View(viewModel);
        }       
    }
}
