using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Santa_challenge.Kruskal;

namespace Santa_challenge
{
    /// <summary>
    /// Constant values taken from https://traveling-santa.reaktor.com/
    /// </summary>
    class Algorithm
    {
        /// <summary>
        /// Y
        /// </summary>
        public const double BaseLatitude = 68.073611;

        /// <summary>
        /// X
        /// </summary>
        public const double BaseLongitude = 29.315278;

        /// <summary>
        /// Max weight the Santa's sleight can carry, in grams 
        /// </summary>
        public const double MaxWeight = 10000000;

        /// <summary>
        /// Earths radius, in meters
        /// </summary>
        public const double EarthRadius = 6378000;

        /// <summary>
        /// Stop iteration cycle somewhere, otherwise nesting will go too deep
        /// </summary>
        public const int MAX_ITERATIONS = 20;

        public List<Wish> Wishes { get; set; }

        public Algorithm(WishList list)
        {
            Wishes = list.Wishes;
        }

        public List<string> GoThroughListInOrder()
        {
            return DivideSortedWishesByWeightToStringList();
        }


        /// <summary>
        /// Sorts list by distance to the base
        /// </summary>
        /// <returns></returns>
        public List<string> UseClosestNodes()
        {
            Wishes.Sort((a, b) => (a.DistanceToBase().CompareTo(b.DistanceToBase())));
            return DivideSortedWishesByWeightToStringList();
        }

        private List<string> DivideSortedWishesByWeightToStringList()
        {
            return AlgorithmHelpers.DivideSortedWishesByWeightToStringList(Wishes);
        }
        
        public List<string> UseKruskal(Stopwatch watch)
        {
            double depth = 20;


            int maxDistance = AlgorithmHelpers.GetInputForKruskalMaxDistance();

            // Calculate edges or load from file

            List<Edge> allEdges;

            if (FileHandler.EdgeFileExists(maxDistance))
            {
                allEdges = FileHandler.LoadEdges(maxDistance);
            }
            else
            {
                allEdges = KruskalFunctions.CreateAndSortEdgesForKruskalAlgorithm(maxDistance, watch, Wishes);
                FileHandler.SaveEdges(maxDistance, allEdges);
            }



            List<Wish> readyWishes = KruskalFunctions.CreateMinimumPathWithKruskalAlgorithm(allEdges, depth, watch, Wishes);



            return AlgorithmHelpers.DivideSortedWishesByWeightToStringList(readyWishes);
        }

        
    }
}
