namespace WebImageProcessor.Services.Image_process.Interfeces
{
    public interface IPhotoInformation
    {
        public Task<(string, string)> AnalysePhotoAsync(IFormFile file, IConfiguration appConfi);
    }
}
