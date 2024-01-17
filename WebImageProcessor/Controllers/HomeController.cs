using Compunet.YoloV8;
using Compunet.YoloV8.Plotting;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp.ImgHash;
using OpenCvSharp.ML;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Text;
using WebImageProcessor.Models;
using WebImageProcessor.Services.Image_process.Interfeces;
using WebImageProcessor.Services.Image_Process.Interfeces;
using WebImageProcessor.Tools;
using WebImageProcessor.ViewModel;

namespace WebImageProcessor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _appConfig;
        private readonly IDetectObject _detectObject;
        private readonly IPhotoInformation _photoInf;
        private readonly ImageProcessorDbContext db;

        public HomeController(ILogger<HomeController> logger, IConfiguration appConfig, IDetectObject detectObject, IPhotoInformation photoInf, ImageProcessorDbContext context)
        {
            _logger = logger;
            _appConfig = appConfig;
            _detectObject = detectObject;
            _photoInf = photoInf;
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            HomePageViewModel viewModel = new HomePageViewModel();

            // Перетворення обробленого зображення у масив байтів, для передачі клієнту
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Отримання обробленого зображення
                using Image processedImg = await _detectObject.DrawBoxesOnImage(file, _appConfig);

                // Збереження зображення у потоку пам'яті в png форматі
                processedImg.Save(memoryStream, new PngEncoder());

                viewModel.Image = memoryStream.ToArray();
            }

            (string, string) colorsAndImg = await _photoInf.AnalysePhotoAsync(file, _appConfig);
            viewModel.Colors = StringOperation.SplitStringWhithInf(colorsAndImg.Item1);
            viewModel.Objects = StringOperation.SplitStringWhithInf(colorsAndImg.Item2);

            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["nickname"]))
            {
                UserRequest userRequest = new UserRequest();
                userRequest.Nickname = HttpContext.Request.Cookies["nickname"];
                userRequest.ColorsInPhoto = colorsAndImg.Item1;
                userRequest.ObjectsInPhoto = colorsAndImg.Item2;

                db.UserRequests.Add(userRequest);
                db.SaveChanges();
            }

            return View(viewModel);
        }

    }
}