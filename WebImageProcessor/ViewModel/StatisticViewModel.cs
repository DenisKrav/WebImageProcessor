namespace WebImageProcessor.ViewModel
{
    public class StatisticViewModel
    {
        public int TotalNumRegUsers { get; set; }
        public int TotalNumProcesImg { get; set; }
        public List<(string, int)>? MainObjectsOnPhoto {  get; set; }
        public List<(string, int)>? MainColorsOnPhoto { get; set; }
    }
}
