using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace santachallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reading nicelist...");
            var input = utils.FileSystem.GetInput("nicelist", "santachallenge");
            var culture = CultureInfo.CurrentCulture;
            
            // (index,latitude,longitude,weight)
            // longitude - itä-länsi
            var nicelist = new List<(string, double, double, int)>();

            foreach (var row in input)
            {
                var content = row.Split(";");
                nicelist.Add((
                    content[0],
                    double.Parse(content[1].Replace('.', ',')),
                    double.Parse(content[2].Replace('.', ',')),
                    int.Parse(content[3])
                    ));
            }

            var minLatitude = nicelist.Min(item => item.Item2);
            var maxLatitude = nicelist.Max(item => item.Item2);
            var minLongitude = nicelist.Min(item => item.Item3);
            var maxLongitude = nicelist.Max(item => item.Item3);
        }
    }
}
