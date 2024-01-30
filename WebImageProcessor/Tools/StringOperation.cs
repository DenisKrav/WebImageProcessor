using System.Xml;

namespace WebImageProcessor.Tools
{
    public class StringOperation
    {
        public static List<string> SplitStringWhithInf(string? input)
        {
            return string.IsNullOrEmpty(input) ? new List<string> {"empty"} : input.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
