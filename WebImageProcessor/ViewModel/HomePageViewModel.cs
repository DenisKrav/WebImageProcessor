using SixLabors.ImageSharp;

namespace WebImageProcessor.ViewModel
{
    public class HomePageViewModel
    {
        public byte[]? Image { get; set; }
        public IEnumerable<string>? Objects {  get; set; }
        public IEnumerable<string>? Colors { get; set; }
    }
}