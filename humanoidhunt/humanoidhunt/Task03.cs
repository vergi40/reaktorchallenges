using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace humanoidhunt
{
    // https://hunt.reaktor.com/android
    class Task03
    {
        private static bool PrintIterations { get; set; } = false;
        private static int PrintTimeout { get; } = 100;
        
        public static void Run()
        {
            Console.WriteLine("Constructing neural link image...");
            var watch = new Stopwatch();
            watch.Start();
            var input = FileSystem.GetInput("input-neurallink");
            var routes = new List<Route>();
            var map = new PathType[1000,1000];

            foreach (var line in input)
            {
                var coordinates = string.Concat(line.TakeWhile(s => s != ' ')).Split(',');
                var orders = line.Substring(line.IndexOf(' ') + 1);

                var route = new Route(int.Parse(coordinates[0]), int.Parse(coordinates[1]), orders.Split(',').ToList());
                route.UpdateToMap(map);
                routes.Add(route);
            }
            
            watch.Stop();
            Console.WriteLine($"Image complete. Time elapsed: {watch.ElapsedMilliseconds} ms.");
            PrintSnapShot(map);
            
            // Ask user if want to print each iteration
            Console.WriteLine($"Do you wan't to visualize path finding? [Y/y]es / [N/any]o");
            var key = Console.ReadKey();
            if (key.KeyChar.ToString().ToLower() == "y") PrintIterations = true;
            
            Console.WriteLine($"Starting route calculations...");
            watch.Restart();
            var startPositions = new List<(int x, int y)>();

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if (map[i, j] == PathType.Start)
                    {
                        startPositions.Add((i, j));
                    }
                }
            }

            var result = FindPath(map, startPositions);
            // Convert to instructions

            
        }


        private static void PrintSnapShotSmall(PathType[,] map, List<(int x, int y)> routeBehind = null, int yOffset = 0)
        {
            //QueuedConsole.WriteLine(Environment.NewLine);
            //QueuedConsole.WriteLine(Environment.NewLine);
            //QueuedConsole.WriteLine(Environment.NewLine);
            QueuedConsole.WriteLine("Snapshot:");
            for (int i = 95 + yOffset; i < 150 + yOffset; i++)
            {
                var line = "";
                for (int j = 80; j < 150; j++)
                {
                    if (routeBehind != null && routeBehind.Contains((i, j)))
                    {
                        line += "*";
                        continue;
                    }

                    line += ToFriendlyString(map[i, j]);
                }
                QueuedConsole.WriteLine(line);
            }
            QueuedConsole.WriteAll();
        }

        private static void PrintSnapShot(PathType[,] map, List<(int x, int y)> routeBehind = null)
        {
            Console.WriteLine("Snapshot:");
            for (int i = 80; i < 250; i++)
            {
                var line = "";
                for (int j = 80; j < 250; j++)
                {
                    if (routeBehind != null && routeBehind.Contains((i,j)))
                    {
                        line += "*";
                        continue;
                    }
                    
                    line += ToFriendlyString(map[i, j]);
                }
                Console.WriteLine(line);
            }
        }
        

        private static List<(int x, int y)> FindPath(PathType[,] map, List<(int x, int y)> startPositions)
        {
            var result = new List<(int x, int y)>();
            foreach (var start in startPositions)
            {
                var startX = start.x;
                var startY = start.y;
                
                var behind = new List<(int, int)>();
                behind.Add((startX, startY));

                // Check start directions
                var startDirections = new List<(int, int)>();
                if (map[startX, startY + 1] == PathType.Free)
                {
                    startDirections.Add((startX, startY + 1));
                }
                if (map[startX, startY - 1] == PathType.Free)
                {
                    startDirections.Add((startX, startY - 1));
                }
                if (map[startX + 1, startY] == PathType.Free)
                {
                    startDirections.Add((startX + 1, startY));
                }
                if (map[startX - 1, startY] == PathType.Free)
                {
                    startDirections.Add((startX - 1, startY));
                }

                var visited = behind.ToList();

                foreach (var startDirection in startDirections)
                {
                    result = IterateToFinish(map, visited, behind.ToList(), startDirection);
                    if (result.Any())
                    {
                        // Found!
                        break;
                    }
                }

                if (result.Any())
                {
                    // Found!
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="visited">Passed as reference</param>
        /// <param name="behind">Remember to pass new list</param>
        /// <param name="current"></param>
        /// <returns></returns>
        private static List<(int x, int y)> IterateToFinish(PathType[,] map, 
            List<(int x, int y)> visited, 
            List<(int x, int y)> behind, 
            (int x, int y) current)
        {
            if (map[current.x, current.y] == PathType.Finish)
            {
                behind.Add(current);
                return behind;
            }
            else if (visited.Contains(current))
            {
                return new List<(int x, int y)>();
            }
            else if (map[current.x, current.y] == PathType.Unknown || map[current.x, current.y] == PathType.Wall)
            {
                return new List<(int x, int y)>();
            }
            
            // Current position ok
            
            var nextList = CalculateNextDirections(behind, current).ToList();
            behind.Add(current);
            visited.Add(current);
            
            if(PrintIterations)
            {
                PrintSnapShotSmall(map, behind, 8);
                Thread.Sleep(PrintTimeout);
            }
            
            foreach (var next in nextList)
            {
                var result = IterateToFinish(map, visited, behind.ToList(), next);
                if (result.Count == 0)
                {
                    // There was some obstacle
                }
                else
                {
                    // Found path!
                    return result;
                }
            }

            return new List<(int x, int y)>();

        }

        private static IEnumerable<(int x, int y)> CalculateNextDirections(List<(int x, int y)> behind, (int x, int y) current)
        {
            var previousX = current.x - behind.Last().x;
            var previousY = current.y - behind.Last().y;
            if (previousX == 0)
            {
                // Last movement was vertical
                yield return (current.x, current.y + previousY);
                yield return (current.x + 1, current.y);
                yield return (current.x - 1, current.y);
            }
            else
            {
                yield return (current.x + previousX, current.y);
                yield return (current.x, current.y + 1);
                yield return (current.x, current.y - 1);
            }
        }

        enum PathType
        {
            Unknown,
            Free,
            Wall,
            Start,
            Finish
        }

        static string ToFriendlyString(PathType type)
        {
            switch (type)
            {
                case PathType.Unknown:
                    return ".";
                case PathType.Free:
                    return "O";
                case PathType.Wall:
                    return "X";
                case PathType.Start:
                    return "S";
                case PathType.Finish:
                    return "F";
                default: throw new ArgumentException();
            }
        }

        class Route
        {
            public int X { get; }
            public int Y { get; }
            public List<string> Directions { get; }

            public Route(int x, int y, List<string> directions)
            {
                X = x + 100;
                Y = y + 100;
                Directions = directions;
            }
            
            public void UpdateToMap(PathType[,] map)
            {
                var x = X;
                var y = Y;
                
                foreach (var direction in Directions)
                {
                    switch (direction)
                    {
                        case "U": 
                            y++;
                            break;
                        case "D":
                            y--;
                            break;
                        case "L":
                            x--;
                            break;
                        case "R":
                            x++;
                            break;
                    }

                    map[x, y] = PathType.Free;
                }

                if (Directions.Last() == "X") map[x, y] = PathType.Wall;
                else if (Directions.Last() == "S") map[x, y] = PathType.Start;
                else if (Directions.Last() == "F") map[x, y] = PathType.Finish;
            }
        }
    }


    // https://stackoverflow.com/questions/5272177/console-writeline-slow
    public static class QueuedConsole
    {
        private static StringBuilder _sb = new StringBuilder();
        private static int _lineCount;

        public static void WriteLine(string message)
        {
            _sb.AppendLine(message);
            ++_lineCount;
        }

        public static void WriteAll()
        {
            Console.WriteLine(_sb.ToString());
            _lineCount = 0;
            _sb.Clear();
        }
    }
}
