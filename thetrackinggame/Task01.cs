using System;
using System.Linq;
using humanoidhunt;

namespace thetrackinggame
{
    /// <summary>
    /// https://tracking-game.reaktor.com/signal/vs/noise/play
    /// </summary>
    class Task01
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reading bytes from Base64 stream...");
            var input = FileSystem.GetInput("Input01", "thetrackinggame").First();
            var filtered = "";
            var faulty = "";
            for (int i = 0; i < input.Length - 1; i++)
            {
                var previous = input[i];
                var next = input[i + 1];

                if (previous == next)
                {
                    faulty += input[i];
                    continue;
                }
                filtered += input[i];
            }

            var a = input.Length;
            var b = filtered.Length;
            var bytes = Base64Decode(filtered);
            
        }

        
        
        
        // https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
