using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using utils;

namespace humanoidhunt
{
    class Task02
    {
        // https://hunt.reaktor.com/nanobots
        public static void Run()
        {
            Console.WriteLine("Reading control signal...");
            var input = FileSystem.GetInput("input-nanobots").Single();
            var password = "";

            var dict = new Dictionary<char, int>();
            foreach (var symbol in input)
            {
                if (dict.ContainsKey(symbol)) dict[symbol]++;
                else
                {
                    dict.Add(symbol, 1);
                }
            }

            // Select key with highest value
            var temp = from x in dict where x.Value == dict.Max(v => v.Value) select x.Key;
            var first = temp.Single();
            password += first;

            var next = first;
            while (next != ';')
            {
                // Find characters immediately previous char indexes
                dict = new Dictionary<char, int>();
                foreach (var targetIndex in FindOccurenceIndexes(input, next))
                {
                    var symbol = input[targetIndex + 1];
                    if (dict.ContainsKey(symbol)) dict[symbol]++;
                    else
                    {
                        dict.Add(symbol, 1);
                    }
                }

                // Select key with highest value
                var tempNext = from x in dict where x.Value == dict.Max(v => v.Value) select x.Key;
                next = tempNext.Single();
                password += next;
            }

            Console.WriteLine($"Password: {password}");
        }

        private static IEnumerable<int> FindOccurenceIndexes(string input, char target)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == target)
                {
                    yield return i;
                }
            }
        }
    }
}
