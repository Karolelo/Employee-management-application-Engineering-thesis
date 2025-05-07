// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

public class Xmull
{
    private static string pattern = @"mul\((\d,\d\))";
    
    public static Tuple<int, int>[] Find_mull(string toFind)
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

    public static int MultipleyTuples(Tuple<int, int>[] tuples)
    {
        int sum = 0;
        
        foreach (var tuple in tuples)
        {
            sum += MultiplayTuple(tuple);
        }

        return sum;
    }

    private static int MultiplayTuple(Tuple<int, int> tuple)
    {
        return tuple.Item1 * tuple.Item2;
    }

    public static void Main(string[] args)
    {
        var x = Find_mull("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(21:37]then(mul(11,8)mul(8,5))");
        Console.WriteLine(MultipleyTuples(x));
    }
}
