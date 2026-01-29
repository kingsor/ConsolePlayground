using ConsoleSampleApp.Utility;
using System.Text.RegularExpressions;

namespace ConsoleSampleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting program...");

        //TestIndexAndRange();

        //TestRegex();

        SwitchArrayElements();
        
        TestDefaultClass();
    }

        private static void TestDefaultClass()
    {
        var arr = Default<int[]>.Value;
        var intElement = Default<int>.Value;
        var obj = Default<object>.Value;
        var list = Default<IList<object>>.Value;
    }

    private static void SwitchArrayElements()
    {
        int[] arr = { 1, 2, 3 };
        (arr[0], arr[1]) = (arr[1], arr[0]);
        Console.WriteLine(string.Format($"{arr[0]} {arr[1]} {arr[2]}")); // 2 1 3
    }

    private static void TestRegex()
    {
        // from here
        // https://www.tutorialspoint.com/csharp/csharp_regular_expressions.htm

        //string str = "make maze and manage to measure it";
        //var expr = @"\bm\S*e\b";

        //Console.WriteLine("Matching words start with 'm' and ends with 'e':");
        //showMatch(str, expr);

        var input = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
        var expr = @"mul\((\d+),(\d+)\)";

        Console.WriteLine("Matching token 'mul(num,num)':");
        var mc = showMatch(input, expr);

        var part1 = mc
            .Select(m => long.Parse(m.Groups[1].ValueSpan) * long.Parse(m.Groups[2].ValueSpan))
            .Sum()
            .ToString();

        Console.WriteLine($"Result part1: {part1}");

        input = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
        expr = @"(?<do>do\(\))|(?<mul>mul\((?<mul1>\d+),(?<mul2>\d+)\))|(?<dont>don't\(\))";

        mc = showMatch(input, expr);

        var sum = 0L;
        var state = true;
        foreach (Match m in mc)
        {
            switch (state, m.Groups["do"].Success, m.Groups["mul"].Success, m.Groups["dont"].Success)
            {
                case (false, true, _, _):
                    state = true;
                    break;

                case (true, _, _, true):
                    state = false;
                    break;

                case (true, _, true, _):
                    {
                        sum += long.Parse(m.Groups["mul1"].ValueSpan) * long.Parse(m.Groups["mul2"].ValueSpan);
                        break;
                    }

                default:
                    break;
            }
        }

        var part2 = sum.ToString();

        Console.WriteLine($"Result part2: {part2}");

        sum = 0L;
        state = true;

        foreach (Match m in mc)
        {
            if(state)
            {
                if (m.Groups["mul"].Success)
                {
                    sum += long.Parse(m.Groups["mul1"].ValueSpan) * long.Parse(m.Groups["mul2"].ValueSpan);
                }

                if (m.Groups["dont"].Success)
                {
                    state = false;
                }
            }
            else
            {
                if (m.Groups["do"].Success)
                {
                    state = true;
                }
            }
        }

        part2 = sum.ToString();

        Console.WriteLine($"Result part2: {part2}");

        expr = @"mul\((\d+),(\d+)\)|do\(\)|don't\(\)";

        mc = showMatch(input, expr);

        sum = 0L;
        state = true;

        foreach (Match m in mc)
        {
            var name = m.Groups[0].Value;

            if (state)
            {
                if (name.StartsWith("mul"))
                {
                    sum += long.Parse(m.Groups[1].ValueSpan) * long.Parse(m.Groups[2].ValueSpan);
                }

                if (name.StartsWith("don't"))
                {
                    state = false;
                }
            }
            else
            {
                if (name.StartsWith("do"))
                {
                    state = true;
                }
            }
        }

        part2 = sum.ToString();

        Console.WriteLine($"Result part2: {part2}");

        Console.ReadKey();
    }

    private static MatchCollection showMatch(string text, string expr)
    {
        Console.WriteLine("The Expression: " + expr);
        MatchCollection mc = Regex.Matches(text, expr);

        foreach (Match m in mc)
        {
            Console.WriteLine(m);
        }

        return mc;
    }

    private static void TestIndexAndRange()
    {
        // The index operator ^

        // The array used for all demonstrations across this post
        var arr = new[] { 0, 1, 2, 3, 4, 5 };
        Assert.IsTrue(arr.Length == 6);


        // Just to clarify   index   and   arr[index]  are equals
        for (var i = 0; i < arr.Length; i++)
        {
            Assert.IsTrue(arr[i] == i);
        }


        // [^1]   means last element 
        // equivalent to   [arr.Length - 1]
        Assert.IsTrue(arr[^1] == 5);
        Assert.IsTrue(arr[arr.Length - 1] == 5);


        // [^2] means second last element 
        // equivalent to   [arr.Length - 2]  and so on
        Assert.IsTrue(arr[^2] == 4);


        for (var i = 0; i < arr.Length; i++)
        {
            // arr[^i]   index from the end   reads:  arr[arr.Length -i]
            Assert.IsTrue(arr[^(arr.Length - i)] == i);
            Assert.IsTrue(arr[^(6 - i)] == i);

            // A little headache... but it makes sense!
            Assert.IsTrue(arr[^(i + 1)] == 5 - i);
        }


        // arr[^0] means index after last element   arr[arr.Length] 
        // and throw a IndexOutOfRangeException
        bool exThrown = false;
        try { int i = arr[^0]; } catch (IndexOutOfRangeException) { exThrown = true; }
        Assert.IsTrue(exThrown);


        //The range operator ..


        // [..] means range of all elements
        Assert.IsTrue(arr[..].SequenceEqual(arr));


        // range [1..4] returns {1, 2, 3 }
        // start of the range (1) is inclusive
        //   end of the range (4) is exclusive
        Assert.IsTrue(arr[1..4].SequenceEqual(new[] { 1, 2, 3 }));


        // [..3] returns { 0, 1, 2 }    from the beginning till 3 exclusive
        Assert.IsTrue(arr[..3].SequenceEqual(new[] { 0, 1, 2 }));


        // [3..] returns { 3, 4, 5 }    from 3 inclusive till the end
        Assert.IsTrue(arr[3..].SequenceEqual(new[] { 3, 4, 5 }));


        // Mixing index and range operators


        // [0..^0] means from the beginning till the end
        // It is equivalent to [..]
        // Remember that the upper bound ^0 is exclusive
        // so there is no risk of IndexOutOfRangeException here
        Assert.IsTrue(arr[0..^0].SequenceEqual(arr));


        // [2..^2]   means  [2..(6-2)]  means  [2..4]
        Assert.IsTrue(arr[2..^2].SequenceEqual(new[] { 2, 3 }));


        // [^4..^1]   means  [(6-4)..(6-1)]  means  [2..5]
        Assert.IsTrue(arr[^4..^1].SequenceEqual(new[] { 2, 3, 4 }));


    }

    }
}
