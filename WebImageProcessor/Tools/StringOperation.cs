namespace WebImageProcessor.Tools
{
    public class StringOperation
    {
        public static string[] SplitStringWhithInf(string input)
        {
            return input.Split("|", StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
