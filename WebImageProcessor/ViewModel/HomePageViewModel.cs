using SixLabors.ImageSharp;

namespace WebImageProcessor.ViewModel
{
    public class HomePageViewModel
    {
        public byte[]? Image { get; set; }
        public string[]? Objects {  get; set; }
        public string[]? Colors { get; set; }
    }
}