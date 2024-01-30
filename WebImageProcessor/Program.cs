using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/LoginLogoutReg/Login";
                    options.AccessDeniedPath = "/Home/Index";
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			app.UseCookiePolicy();
			app.UseAuthorization();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=AdminHome}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}