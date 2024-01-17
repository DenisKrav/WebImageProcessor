using Compunet.YoloV8;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using System.Drawing;
using WebImageProcessor.Services.Image_process.Interfeces;
using Compunet.YoloV8.Data;

namespace WebImageProcessor.Services.Image_process.Realization
{
    public class PhotoInfYoloV8Service : IPhotoInformation
    {
        // Головний метод для знаходження середніх значнь кольорів в усіх боксах
        public async Task<(string, string)> AnalysePhotoAsync(IFormFile file, IConfiguration appConfig)
        {
            using (var stream = file.OpenReadStream())
            {
                using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(stream))
                {

                    YoloV8 predictor = new YoloV8(YoloV8ProcessImgService.GetModelPath(appConfig));
                    Bitmap bitmap = ConvertImageSLToBitmap(image);

                    var result = await predictor.DetectAsync(image);

                    if(result.Boxes.Count == 0) 
                    {
                        return ("", "");
                    }
                    else
                    {
                        return (CheckColorsAllBoxes(bitmap, result), CheckNameAllBoxes(result));
                    }
                }
            }
        }

        // Метод для перевірки усіх боксів з одягом
        static string CheckColorsAllBoxes(Bitmap bitmap, IDetectionResult detectionResult)
        {
            string result = "";

            if(detectionResult.Boxes.Count == 0) 
            {
                return result;
            }
            else
            {
                for(int i = 0; i < detectionResult.Boxes.Count; i++)
                {
                    int x = detectionResult.Boxes[i].Bounds.X;
                    int y = detectionResult.Boxes[i].Bounds.Y;
                    int width = detectionResult.Boxes[i].Bounds.Width;
                    int height = detectionResult.Boxes[i].Bounds.Height;
                    result += "|" + ConvertRGBToHEX(GetAvgColorInBox(bitmap, x, y, width, height));
                }

                return result;
            }

        }

        // Метод для от римання середнього значення кольору у боксі з одягом
        static System.Drawing.Color GetAvgColorInBox(Bitmap bitmap, int startX, int startY, int width, int height)
        {
            // Змінні для сумарних значень кольорів
            int totalR = 0, totalG = 0, totalB = 0;

            // Перебір пікселів у вказаному квадраті
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    // Отримання кольору пікселя
                    System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

                    // Додавання кольорів для подальшого обрахунку середнього
                    totalR += pixelColor.R;
                    totalG += pixelColor.G;
                    totalB += pixelColor.B;
                }
            }

            // Обчислення середнього кольору
            int averageR = totalR / (width * height);
            int averageG = totalG / (width * height);
            int averageB = totalB / (width * height);

            // Створення та повернення середнього кольору
            return System.Drawing.Color.FromArgb(averageR, averageG, averageB);
        }

        // Пертворення System.Drawing.Color у Bitmap
        public static Bitmap ConvertImageSLToBitmap(SixLabors.ImageSharp.Image image)
        {
            // Створення пустого MemoryStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Збереження Image в MemoryStream у форматі BMP
                image.Save(memoryStream, new JpegEncoder());

                // Перетворення MemoryStream у масив байтів
                byte[] byteArray = memoryStream.ToArray();

                // Створення MemoryStream для зчитування масиву байтів
                using (MemoryStream ms = new MemoryStream(byteArray))
                {
                    // Створення Bitmap із MemoryStream
                    Bitmap bitmap = new Bitmap(ms);

                    return bitmap;
                }
            }
        }

        // Метод для конвертації RGB у HEX
        public static string ConvertRGBToHEX(System.Drawing.Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static string CheckNameAllBoxes(IDetectionResult detectionResult)
        {
            string result = "";

            if (detectionResult.Boxes.Count == 0)
            {
                return "";
            }
            else
            {
                for (int i = 0; i < detectionResult.Boxes.Count; i++)
                {
                    result += "|" + detectionResult.Boxes[i].Class.Name;
                }

                return result;
            }
        }
    }
}
