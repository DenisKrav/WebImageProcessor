using Compunet.YoloV8;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using System.Drawing;
using WebImageProcessor.Services.Image_process.Interfeces;
using Compunet.YoloV8.Data;
using WebImageProcessor.Tools;

namespace WebImageProcessor.Services.Image_process.Realization
{
    public class PhotoInfYoloV8Service : IPhotoInformation
    {
		/// <summary>
		/// Мето AnalysePhotoAsync аналізує які є обьєкти на фото і також визначає кольори у боксах. Головний принцип роботи 
        /// розпізнавання кольорів - перетворення зображення на більш піксельне, після чого воно квантонується (прибираються 
        /// напівтони), і тільки потім аналізуються кольори і вибираються найбільш зустрічаємі
		/// </summary>
		/// <param name="file"></param>
		/// <param name="appConfig"></param>
		/// <returns></returns>

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
                    result += GetMainColors(ImageQuantization.QuantizeImage(ConvertImageToPixel(bitmap), 16), x, y, width, height);
                }

                return result;
            }

        }

		//// Метод для отримання середнього значення кольору у боксі з одягом
		//static System.Drawing.Color GetAvgColorInBox(Bitmap bitmap, int startX, int startY, int width, int height)
		//{
		//    // Змінні для сумарних значень кольорів
		//    int totalR = 0, totalG = 0, totalB = 0;

		//    // Перебір пікселів у вказаному квадраті
		//    for (int x = startX; x < startX + width; x++)
		//    {
		//        for (int y = startY; y < startY + height; y++)
		//        {
		//            // Отримання кольору пікселя
		//            System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

		//            // Додавання кольорів для подальшого обрахунку середнього
		//            totalR += pixelColor.R;
		//            totalG += pixelColor.G;
		//            totalB += pixelColor.B;
		//        }
		//    }

		//    // Обчислення середнього кольору
		//    int averageR = totalR / (width * height);
		//    int averageG = totalG / (width * height);
		//    int averageB = totalB / (width * height);

		//    // Створення та повернення середнього кольору
		//    return System.Drawing.Color.FromArgb(averageR, averageG, averageB);
		//}

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static string GetMainColors(Bitmap bitmap, int startX, int startY, int width, int height)
		{
			List<string> colors = new List<string>();
            string result = "";

			// Перебір пікселів у вказаному квадраті
			for (int x = startX; x < startX + width; x++)
			{
				for (int y = startY; y < startY + height; y++)
				{
					// Отримання кольору пікселя
					System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

					// Додавання кольорів для подальшого обрахунку середнього
					colors.Add(ConvertRGBToHEX(pixelColor));
				}
			}

			// Групування та сортування кольорів за кількістю
			var groupedColors = colors
				.GroupBy(c => c)
				.OrderByDescending(group => group.Count())
				.Select(group => group.Key)
				.Take(10)
				.ToList();

            foreach (var c in groupedColors)
            {
                result += "|" + c;
            }

			return result;
		}

		public static Bitmap ConvertImageToPixel(Bitmap source)
		{
			int H_CELL = 100;
			int W_CELL = 100;

			Bitmap result = new Bitmap(source);
			Bitmap bitmap = new Bitmap(source);

			using (Graphics g = Graphics.FromImage(result))
			{
				for (int y = 0; y < bitmap.Height; y += H_CELL)
				{
					for (int x = 0; x < bitmap.Width; x += W_CELL)
					{
						Brush brush = new SolidBrush(bitmap.GetPixel(x, y));
						g.FillRectangle(brush, x, y, W_CELL, H_CELL);
					}
				}
			}

			return result;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////

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
