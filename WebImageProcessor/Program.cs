using Microsoft.Extensions.Configuration;
using WebImageProcessor.Models;
using WebImageProcessor.Services.Image_process.Interfeces;
using WebImageProcessor.Services.Image_process.Realization;
using WebImageProcessor.Services.Image_Process.Interfeces;

namespace WebImageProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add file whith configuration, which contains paths 
            builder.Configuration.AddJsonFile("pathsSettings.json");

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ImageProcessorDbContext>();
            builder.Services.AddScoped<IDetectObject, YoloV8ProcessImgService>();
            builder.Services.AddScoped<IPhotoInformation, PhotoInfYoloV8Service>();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}