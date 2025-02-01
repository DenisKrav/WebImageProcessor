using System.Drawing;

namespace WebImageProcessor.Tools
{
	public class ImageQuantization
	{
		public static Bitmap QuantizeImage(Bitmap image, int clusterCount)
		{
			List<System.Drawing.Color> colors = GetUniqueColors(image);
			List<System.Drawing.Color> clusteredColors = KMeansClustering(colors, clusterCount);

			Bitmap quantizedImage = new Bitmap(image.Width, image.Height);

			for (int x = 0; x < image.Width; x++)
			{
				for (int y = 0; y < image.Height; y++)
				{
					System.Drawing.Color originalColor = image.GetPixel(x, y);
					System.Drawing.Color closestColor = FindClosestColor(originalColor, clusteredColors);

					quantizedImage.SetPixel(x, y, closestColor);
				}
			}

			return quantizedImage;
		}

		private static List<System.Drawing.Color> GetUniqueColors(Bitmap image)
		{
			List<System.Drawing.Color> colors = new List<System.Drawing.Color>();

			for (int x = 0; x < image.Width; x++)
			{
				for (int y = 0; y < image.Height; y++)
				{
					System.Drawing.Color color = image.GetPixel(x, y);

					if (!colors.Contains(color))
					{
						colors.Add(color);
					}
				}
			}

			return colors;
		}

		private static List<System.Drawing.Color> KMeansClustering(List<System.Drawing.Color> colors, int clusterCount)
		{
			Random random = new Random();
			List<System.Drawing.Color> centroids = colors.OrderBy(c => random.Next()).Take(clusterCount).ToList();

			Dictionary<System.Drawing.Color, List<System.Drawing.Color>> clusters = new Dictionary<System.Drawing.Color, List<System.Drawing.Color>>();

			for (int iteration = 0; iteration < 50; iteration++) // Number of iterations
			{
				clusters.Clear();

				foreach (System.Drawing.Color color in colors)
				{
					System.Drawing.Color closestCentroid = FindClosestColor(color, centroids);

					if (!clusters.ContainsKey(closestCentroid))
					{
						clusters[closestCentroid] = new List<System.Drawing.Color>();
					}

					clusters[closestCentroid].Add(color);
				}

				centroids.Clear();

				foreach (var cluster in clusters)
				{
					System.Drawing.Color newCentroid = CalculateAverageColor(cluster.Value);
					centroids.Add(newCentroid);
				}
			}

			return centroids;
		}

		private static System.Drawing.Color FindClosestColor(System.Drawing.Color targetColor, List<System.Drawing.Color> colors)
		{
			double minDistance = double.MaxValue;
			System.Drawing.Color closestColor = System.Drawing.Color.Black;

			foreach (System.Drawing.Color color in colors)
			{
				double distance = CalculateColorDistance(targetColor, color);

				if (distance < minDistance)
				{
					minDistance = distance;
					closestColor = color;
				}
			}

			return closestColor;
		}

		private static double CalculateColorDistance(System.Drawing.Color color1, System.Drawing.Color color2)
		{
			int deltaR = color1.R - color2.R;
			int deltaG = color1.G - color2.G;
			int deltaB = color1.B - color2.B;

			return Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
		}

		private static System.Drawing.Color CalculateAverageColor(List<System.Drawing.Color> colors)
		{
			int sumR = 0, sumG = 0, sumB = 0;

			foreach (System.Drawing.Color color in colors)
			{
				sumR += color.R;
				sumG += color.G;
				sumB += color.B;
			}

			int averageR = sumR / colors.Count;
			int averageG = sumG / colors.Count;
			int averageB = sumB / colors.Count;

			return System.Drawing.Color.FromArgb(averageR, averageG, averageB);
		}
	}
}
