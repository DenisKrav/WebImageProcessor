namespace WebImageProcessor.Tools
{
    public class StringOperation
    {
        public static List<string> SplitStringWhithInf(string input)
        {
            return input.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
