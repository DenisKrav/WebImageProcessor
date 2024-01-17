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
            StatisticViewModel viewModel = new StatisticViewModel();

            viewModel.TotalNumRegUsers = db.AppUsers.Count();
            viewModel.TotalNumProcesImg = db.UserRequests.Count();

            viewModel.MainColorsOnPhoto = new List<(string, int)>();
            viewModel.MainObjectsOnPhoto = new List<(string, int)>();

            // Додаємо у модель представлення усі значення кольорів та об'єктів, які були знайдені системою
            foreach (var request in db.UserRequests)
            {
                viewModel.MainColorsOnPhoto.AddRange(StringOperation.SplitStringWhithInf(request.ColorsInPhoto)
                    .Select(color => (color, 1))); // Кожне значення починається з кількістю 1
                viewModel.MainObjectsOnPhoto.AddRange(StringOperation.SplitStringWhithInf(request.ObjectsInPhoto)
                    .Select(obj => (obj, 1))); // Кожне значення починається з кількістю 1
            }

            // Групування елементів за кількістю, за спаданням, та вибір 5 перших елементів та їх значення кількості
            viewModel.MainColorsOnPhoto = viewModel.MainColorsOnPhoto
                .GroupBy(item => item.Item1)
                .Select(group => (group.Key, group.Sum(item => item.Item2)))
                .OrderByDescending(group => group.Item2)
                .Take(5)
                .ToList();

            viewModel.MainObjectsOnPhoto = viewModel.MainObjectsOnPhoto
                .GroupBy(item => item.Item1)
                .Select(group => (group.Key, group.Sum(item => item.Item2)))
                .OrderByDescending(group => group.Item2)
                .Take(5)
                .ToList();

            return View(viewModel);
        }       
    }
}
