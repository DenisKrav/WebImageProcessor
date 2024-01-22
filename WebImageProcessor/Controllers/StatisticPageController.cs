using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebImageProcessor.Models;
using WebImageProcessor.Tools;
using WebImageProcessor.ViewModel;

namespace WebImageProcessor.Controllers
{
    public class StatisticPageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _appConfig;
        private readonly ImageProcessorDbContext db;

        public StatisticPageController(ILogger<HomeController> logger, ImageProcessorDbContext context) 
        {
            _logger = logger;
            db = context;
        }

        public ActionResult Index()
        {
            string nickname = HttpContext.Request.Cookies["nickname"];

            var allRequests = db.UserRequests.ToList();
            var userRequests = string.IsNullOrEmpty(nickname) ? allRequests : allRequests.Where(r => r.Nickname == nickname).ToList();


            var viewModel = new StatisticViewModel
            {
                TotalNumRegUsers = db.AppUsers.Count(),
                TotalNumProcesImg = allRequests.Count(),
                MainColors = GetTopItems(allRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ColorsInPhoto))),
                MainColorsCount = GetTopItemsCount(allRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ColorsInPhoto))),
                MainObj = GetTopItems(allRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ObjectsInPhoto))),
                MainObjCount = GetTopItemsCount(allRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ObjectsInPhoto))),              
            };

            if(!string.IsNullOrEmpty(nickname))
            {
                viewModel.MainColorsOneUser = GetTopItems(userRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ColorsInPhoto)));
                viewModel.MainColorsOneUserCount = GetTopItemsCount(userRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ColorsInPhoto)));
                viewModel.MainObjOneUser = GetTopItems(userRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ObjectsInPhoto)));
                viewModel.MainObjOneUserCount = GetTopItemsCount(userRequests.SelectMany(r => StringOperation.SplitStringWhithInf(r.ObjectsInPhoto)));
            }

            ViewBag.StatisticData = viewModel;

            return View();
        }

        private List<string> GetTopItems(IEnumerable<string> items)
        {
            return items
                .GroupBy(item => item)
                .Select(group => group.Key)
                .OrderByDescending(group => group.Count())
                .Take(5)
                .ToList();
        }

        private List<int> GetTopItemsCount(IEnumerable<string> items)
        {
            return items
                .GroupBy(item => item)
                .Select(group => group.Count())
                .OrderByDescending(count => count)
                .Take(5)
                .ToList();
        }
    }
}
