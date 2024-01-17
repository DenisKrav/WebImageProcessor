using SixLabors.ImageSharp;
using WebImageProcessor.Models;
using static OpenCvSharp.FileStorage;

namespace WebImageProcessor.Services.Image_Process.Interfeces
{
    public interface IDetectObject
    {
        public Task<Image> DrawBoxesOnImage(IFormFile file, IConfiguration appConfig); 
    }
}
