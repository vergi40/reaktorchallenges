using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanoidhunt
{
    public class FileSystem
    {
        /// <summary>
        /// Returns file path to /humanoidhunt solution folder
        /// </summary>
        /// <returns></returns>
        public static string GetSolutionPath()
        {
            // Hack
            var exePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var solution = Path.Combine(exePath, @"..\..\..\..");
            return Path.GetFullPath(solution);
        }

        public static List<string> GetInput(string fileName)
        {
            if (!Path.HasExtension(fileName)) fileName += ".txt";
            var path = Path.Combine(GetSolutionPath(), "humanoidhunt", fileName);
            var content = File.ReadAllLines(path).ToList();
            return content;
        }
    }
}
