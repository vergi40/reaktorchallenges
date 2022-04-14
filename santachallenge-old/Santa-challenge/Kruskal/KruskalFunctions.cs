using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Santa_challenge.Kruskal
{
    static class KruskalFunctions
    {
        public static List<Edge> CreateAndSortEdgesForKruskalAlgorithm(int maxDistance, Stopwatch watch,
            List<Wish> wishes)
        {
            // 1. Create all possible edges (with some start guess for max length)
            Program.Print("Starting to create possible edges");
            var allEdges = CreateAllPossibleEdgesLimitedByDistance(watch, maxDistance, wishes);
            Program.TimeElapsedAndStartAgain(watch);
            Program.Print("Edges created, amount: " + allEdges.Count + ". Sorting edges by distance");

            // 2. Sort edges by their distance
            allEdges.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            Program.TimeElapsedAndStartAgain(watch);
            Program.Print("Edges sorted. Creating edge trees");

            return allEdges;
        }

        public static List<Wish> CreateMinimumPathWithKruskalAlgorithm(List<Edge> allEdges, double depth, Stopwatch watch, List<Wish> wishes)
        {
            // 3. Start creating edge trees. Link edge to existing tree if you can
            //  a. If ONE point of the edge exists in previous tree, link the edge there
            //  b. If point doesnt exist, create new tree
            //  c. -> Edge can exist only in one tree
            //  d. -> If edge has points in two trees, combine the trees
            var trees = CreateEdgeTreesAndLinkTogether(watch, allEdges);
            Program.TimeElapsedAndStartAgain(watch);
            Program.Print("Edge trees created, amount: " + trees.Count + ". Transforming to wishlist");

            // 4. We have now large list(s) full of edges, where everything is linked
            // Start going through the created path and add wishes to "readyWishes" list
            // Mark visited wishes with state "black"
            var readyWishes = IterateAndCollectWishesFromPaths(watch, trees, wishes.Count);

            // 5. If edge filter distance was small, there might be wishes that are'nt in ready list yet
            List<Wish> unresolvedWishes = new List<Wish>();
            foreach (var wish in wishes)
            {
                if (wish.State != ConnectionState.Black)
                {
                    unresolvedWishes.Add(wish);
                }
            }

            if (unresolvedWishes.Count > 0)
            {
                Program.Print("Wishes that didn't get added to wishlist: " + unresolvedWishes.Count + ". TODO iterate KruskalFunctions again");
                unresolvedWishes.Sort((a, b) => (a.DistanceToBase().CompareTo(b.DistanceToBase())));

                readyWishes.AddRange(unresolvedWishes);

                Program.Print("Debug: readyWishes.Count: " + readyWishes.Count);
            }



            Program.TimeElapsedAndStartAgain(watch);
            Program.Print("Wishlist ready. Creating correct order by weights");

            return readyWishes;
        }

        private static List<Edge> CreateAllPossibleEdgesLimitedByDistance(Stopwatch watch, double maxDistance, List<Wish> wishList)
        {
            List<Edge> allEdges = new List<Edge>();

            // For iteration state
            int counter = 1;
            const int intervalBetweenPrints = 1000000;

            // Iterate through all wish connections and create edges
            for (int i = 0; i < wishList.Count - 1; i++)
            {
                var wishA = wishList[i];
                for (int j = 1 + i; j < wishList.Count; j++)
                {
                    var wishB = wishList[j];

                    if (wishA.DistanceTo(wishB) < maxDistance)
                    {
                        allEdges.Add(new Edge(wishA, wishB));
                    }
                }

                if (allEdges.Count > counter * intervalBetweenPrints)
                {
                    Program.Print("Current Edge count: " + allEdges.Count + ". Time elapsed in current task: " +
                                  watch.ElapsedMilliseconds + "ms");

                    counter++;
                }
            }

            return allEdges;
        }

        private static List<EdgeTree> CreateEdgeTreesAndLinkTogether(Stopwatch watch, List<Edge> allEdges)
        {
            List<EdgeTree> trees = new List<EdgeTree>();

            // For iteration state
            int edgeCounter = 1;
            int intervalBetweenEdgePrints = allEdges.Count / 20;

            for (int i = 0; i < allEdges.Count; i++)
            {
                List<Guid> treesCurrentEdgeExists = new List<Guid>();


                foreach (var tree in trees)
                {
                    var relation = tree.IfContainsSamePointAddToTree(allEdges[i]);
                    if (relation == TreeEdgeRelation.ContainsOnePoint)
                    {
                        treesCurrentEdgeExists.Add(tree.Id);
                    }
                    else if (relation == TreeEdgeRelation.ContainsBothPoints)
                    {
                        treesCurrentEdgeExists.Add(tree.Id);
                        break;
                    }
                }

                if (treesCurrentEdgeExists.Count == 0)
                {
                    trees.Add(new EdgeTree(allEdges[i]));
                }
                else if (treesCurrentEdgeExists.Count == 2)
                {
                    // Combine
                    // Now we have two same edgetrees in two lists
                    // We want to copy the edges from another original edgetree to another
                    // and delete the newer one
                    var tree1Id = treesCurrentEdgeExists.First();
                    var tree2Id = treesCurrentEdgeExists.Last();

                    var treeToAdd = trees.Find(x => x.Id == tree1Id);
                    var treeToSubstract = trees.Find(x => x.Id == tree2Id);

                    var edgesFromSecondList = new List<Edge>();
                    edgesFromSecondList.AddRange(treeToSubstract.Edges);
                    edgesFromSecondList.Remove(allEdges[i]);

                    treeToAdd.Edges.AddRange(edgesFromSecondList);
                    trees.Remove(treeToSubstract);
                }
                else if (treesCurrentEdgeExists.Count > 2)
                {
                    throw new NotImplementedException();
                }

                if (i > intervalBetweenEdgePrints * edgeCounter)
                {
                    Program.Print("Edges handled: " + i.ToString() + ". Time elapsed in current task: " +
                                  watch.ElapsedMilliseconds + "ms");
                    edgeCounter++;
                }
            }

            return trees;
        }

        private static List<Wish> IterateAndCollectWishesFromPaths(Stopwatch watch, List<EdgeTree> trees, int wishesCount)
        {
            List<Wish> readyWishes = new List<Wish>();

            for (int i = 0; i < trees.Count; i++)
            {
                foreach (var edge in trees[i].Edges)
                {
                    if (readyWishes.Count == wishesCount) break;

                    AddStartPoint(edge, ref readyWishes);
                    AddEndPoint(edge, ref readyWishes);

                    IterateEndConnections(edge, ref readyWishes, 0, wishesCount);
                    IterateStartConnection(edge, ref readyWishes, 0, wishesCount);
                }

                if (trees.Count > 1)
                {
                    Program.Print("Trees handled: " + (i + 1).ToString() +
                                  ". Wishlist has ready items: " + readyWishes.Count +
                                  ". Time elapsed in current task: " + watch.ElapsedMilliseconds + "ms");
                }
            }

            return readyWishes;
        }

        private static void IterateStartConnection(Edge edge, ref List<Wish> readyWishes, int depth, int wishesCount)
        {
            // Break nested loop
            if (readyWishes.Count == wishesCount) return;
            if (depth > Algorithm.MAX_ITERATIONS) return;

            foreach (var connection in edge.StartConnections)
            {
                // Break nested loop
                if (readyWishes.Count == wishesCount) return;

                // Connection to other edge found
                foreach (var newEdge in connection.EdgeList)
                {
                    // Break nested loop
                    if (readyWishes.Count == wishesCount) return;

                    if (newEdge != edge)
                    {
                        // Looking good, add to list
                        if (connection == newEdge.StartPoint)
                        {
                            AddEndPoint(newEdge, ref readyWishes);
                            IterateEndConnections(newEdge, ref readyWishes, depth + 1, wishesCount);
                        }
                        else
                        {
                            AddStartPoint(newEdge, ref readyWishes);
                            IterateStartConnection(newEdge, ref readyWishes, depth + 1, wishesCount);
                        }
                    }
                }
            }
        }

        private static void IterateEndConnections(Edge edge, ref List<Wish> readyWishes, int depth, int wishesCount)
        {
            // Break nested loop
            if (readyWishes.Count == wishesCount) return;
            if (depth > Algorithm.MAX_ITERATIONS) return;

            foreach (var connection in edge.EndConnections)
            {
                // Break nested loop
                if (readyWishes.Count == wishesCount) return;

                // Connection to other edge found
                foreach (var newEdge in connection.EdgeList)
                {
                    // Break nested loop
                    if (readyWishes.Count == wishesCount) return;

                    if (newEdge != edge)
                    {
                        // Looking good, add to list

                        // Start point is the same as previous edge's end point
                        if (connection == newEdge.StartPoint)
                        {
                            AddEndPoint(newEdge, ref readyWishes);
                            IterateEndConnections(newEdge, ref readyWishes, depth + 1, wishesCount);
                        }
                        // End point is the same as previous edge's start point
                        else
                        {
                            AddStartPoint(newEdge, ref readyWishes);
                            IterateStartConnection(newEdge, ref readyWishes, depth + 1, wishesCount);
                        }
                    }

                }
            }
        }

        private static void AddEndPoint(Edge edge, ref List<Wish> readyWishes)
        {
            if (edge.EndState != ConnectionState.Black)
            {
                readyWishes.Add(edge.EndPoint);
                edge.EndState = ConnectionState.Black;
            }
        }

        private static void AddStartPoint(Edge edge, ref List<Wish> readyWishes)
        {
            if (edge.StartState != ConnectionState.Black)
            {
                readyWishes.Add(edge.StartPoint);
                edge.StartState = ConnectionState.Black;
            }
        }

        
    }
}
