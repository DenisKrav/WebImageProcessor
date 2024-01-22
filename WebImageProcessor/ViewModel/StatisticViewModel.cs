namespace WebImageProcessor.ViewModel
{
    public class StatisticViewModel
    {
        public int TotalNumRegUsers { get; set; }
        public int TotalNumProcesImg { get; set; }

        public List<string>? MainObj {  get; set; }
        public List<int>? MainObjCount { get; set; }

        public List<string>? MainColors { get; set; }
        public List<int>? MainColorsCount { get; set; }

        public List<string>? MainObjOneUser { get; set; }
        public List<int>? MainObjOneUserCount { get; set; }

        public List<string>? MainColorsOneUser { get; set; }
        public List<int>? MainColorsOneUserCount { get; set; }
    }
}
