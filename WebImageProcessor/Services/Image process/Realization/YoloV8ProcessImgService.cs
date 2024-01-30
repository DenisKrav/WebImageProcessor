using Compunet.YoloV8;
using Compunet.YoloV8.Plotting;
using SixLabors.ImageSharp;
using WebImageProcessor.Models;
using WebImageProcessor.Services.Image_Process.Interfeces;

namespace WebImageProcessor.Services.Image_process.Realization
{
    public class YoloV8ProcessImgService : IDetectObject
    {
        public async Task<Image> DrawBoxesOnImage(IFormFile file, IConfiguration appConfig)
        {
            // Тут не контролюється очищення пам'яті, але треба враховувати, що зюирач сміття усе одно буде спрацьовувати та
            // видаляти такі обьєкти, тобто чисто теоретично, це може прцювати нормально.
            Image resultImage;

            using (var stream = file.OpenReadStream())
            {
                using (Image image1 = Image.Load(stream))
                {
                    // Повертаємо потік на початок для завантаження другого зображення
                    stream.Seek(0, SeekOrigin.Begin);

                    using (Image image2 = Image.Load(stream))
                    {
                        YoloV8 predictor = new YoloV8(GetModelPath(appConfig));

                        var result = await predictor.DetectAsync(image1);

                        resultImage = await result.PlotImageAsync(image2);

                        return resultImage;
                    }
                }
            }
        }

        public static string GetModelPath(IConfiguration appConfig)
        {
            return string.IsNullOrEmpty(appConfig["NeuralMode:ClothesClassifierModel"]) ? 
                appConfig["NeuralMode:DefaultModel"] : appConfig["NeuralMode:ClothesClassifierModel"];
		}
    }
}
