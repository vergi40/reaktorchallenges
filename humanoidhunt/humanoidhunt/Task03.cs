using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanoidhunt
{
    // https://hunt.reaktor.com/android
    class Task03
    {
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
            Console.WriteLine("Snapshot:");
            for (int i = 100; i < 200; i++)
            {
                var line = "";
                for (int j = 80; j < 250; j++)
                {
                    line += ToFriendlyString(map[i, j]);
                }
                Console.WriteLine(line);
            }
            
            Console.WriteLine($"Starting route calculations...");
            watch.Restart();
            var startX = -1;
            var startY = -1;

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    if (map[i, j] == PathType.Start)
                    {
                        startX = i;
                        startY = j;
                        break;
                    }
                }

                if (startX != -1) break;
            }

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

            var result = new List<(int x, int y)>();
            foreach (var startDirection in startDirections)
            {
                result = IterateToFinish(map, behind, startDirection);
                if (result.Any())
                {
                    // Found!
                    break;
                }
            }
            
            // Convert to instructions

            
        }

        private static List<(int x, int y)> IterateToFinish(PathType[,] map, List<(int x, int y)> behind, (int x, int y) current)
        {
            if (map[current.x, current.y] == PathType.Finish)
            {
                behind.Add(current);
                return behind;
            }
            else if (behind.Contains(current))
            {
                return new List<(int x, int y)>();
            }
            else if (map[current.x, current.y] == PathType.Unknown || map[current.x, current.y] == PathType.Wall)
            {
                return new List<(int x, int y)>();
            }

            var nextList = CalculateNextDirections(behind, current).ToList();
            behind.Add(current);
            foreach (var next in nextList)
            {
                var result = IterateToFinish(map, behind, next);
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
}
