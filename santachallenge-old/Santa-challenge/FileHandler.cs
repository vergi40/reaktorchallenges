using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Santa_challenge.Kruskal;
using File = System.IO.File;

namespace Santa_challenge
{
    public enum ListSize
    {
        Small100,
        Medium1000,
        Large4000,
        Full
    }
    static class FileHandler
    {
        public const string EDGE_FILE_NAME = "AllEdges";
        public const string EDGE_FILE_EXTENSION = ".xml";

        public static List<string> ReadNiceList(ListSize size = ListSize.Full)
        {
            string fileName = "nicelist.txt";

            if (size == ListSize.Small100)
            {
                fileName = "nicelist-100.txt";
            }
            else if (size == ListSize.Medium1000)
            {
                fileName = "nicelist-1000.txt";
            }
            else if (size == ListSize.Large4000)
            {
                fileName = "nicelist-4000.txt";
            }
            string filePath = GetDirectoryPath();

            return ReadFileToStringList(Path.Combine(filePath, fileName));
        }

        public static List<string> ReadFileToStringList(string filePath)
        {
            var fileContent = File.ReadAllLines(filePath);
            var list = new List<string>(fileContent);

            return list;
        }

        private static string GetDirectoryPath()
        {
            string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetDirectoryName(filePath);
        }

        public static void WriteAnswerFile(List<string> answerList)
        {
            var filePath = Path.Combine(GetDirectoryPath(), "answer.txt");
            File.WriteAllLines(filePath, answerList);
        }

        public static bool EdgeFileExists(int maxDistance)
        {
            string filePath = GetEdgeFilePath(maxDistance);
            if (File.Exists(filePath))
            {
                return true;
            }

            return false;
        }

        private static string GetEdgeFilePath(int maxDistance)
        {
            string fileName = EDGE_FILE_NAME + "_" + maxDistance.ToString() + EDGE_FILE_EXTENSION;
            string directoryPath = GetDirectoryPath();
            return Path.Combine(directoryPath, fileName);
        }

        public static List<Edge> LoadEdges(int maxDistance)
        {
            var temp = new XmlEdgeList();

            XmlSerializer xs = new XmlSerializer(typeof(XmlEdgeList));
            using (var streamReader = new StreamReader(GetEdgeFilePath(maxDistance)))
            {
                temp = (XmlEdgeList)xs.Deserialize(streamReader);
            }

            return temp.Edges;
        }

        public static void SaveEdges(int maxDistance, List<Edge> allEdges)
        {
            XmlSerializer xs = new XmlSerializer(typeof(XmlEdgeList));
            TextWriter tw = new StreamWriter(GetEdgeFilePath(maxDistance));

            var temp = new XmlEdgeList();
            temp.Edges = allEdges;
            xs.Serialize(tw, temp);
        }
    }
}
