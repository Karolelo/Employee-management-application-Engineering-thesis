using System.Text.RegularExpressions;

namespace Mnożenie;

public class Xmull
{
    private static string pattern = @"mull\\(\d,\d))";
    
    public static Tuple<int, int>[] find_mull(string toFind)
    {
        var tuples = Array.Empty<Tuple<int, int>>();
        
        var mulles = Regex.Matches(toFind, pattern);
        
        foreach (var mull in mulles)
        {
            string[] split = mull.ToString().Split(',');
            int a = int.Parse(split[0].Last().ToString());
            int b = int.Parse(split[1].First().ToString());
            
            tuples = tuples.Append(new Tuple<int, int>(a, b)).ToArray();
        }

       return tuples;
    }

   
}