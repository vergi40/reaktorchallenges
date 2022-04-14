using System;
using System.Collections.Generic;
using System.Windows;

namespace Santa_challenge
{
    static class AlgorithmHelpers
    {
        public static double DistanceToBase(Wish wish)
        {
            Vector a = new Vector(Algorithm.BaseLongitude, Algorithm.BaseLatitude);
            var b = new Vector(wish.Longitude, wish.Latitude);

            var resultVector = b - a;
            return resultVector.Length;
        }

        public static List<string> DivideSortedWishesByWeightToStringList(List<Wish> wishes)
        {
            AnswerCreator creator = new AnswerCreator();

            creator.AddWish(wishes[0]);
            for (int i = 1; i < wishes.Count; i++)
            {
                if (creator.DoesWishFitToCurrentTrip(wishes[i]))
                {
                    creator.AddWish(wishes[i]);
                }
                else
                {
                    creator.NewTrip();
                    creator.AddWish(wishes[i]);
                }
            }

            creator.NewTrip();
            return creator.PrintTrips();
        }

        public static int GetInputForKruskalMaxDistance()
        {
            int maxDistance;

            Program.Print("Give max length for edge creation (1 - 180)");
            Program.Print("[a] 10");
            Program.Print("[b] 15");
            Program.Print("[c] 30");
            Program.Print("[d] 60");
            Program.Print("[e] 90");
            Program.Print("Custom - type number");

            string answer = Console.ReadLine();
            if (answer == "a")
            {
                maxDistance = 10;
            }
            else if (answer == "b")
            {
                maxDistance = 15;
            }
            else if (answer == "c")
            {
                maxDistance = 30;
            }
            else if (answer == "d")
            {
                maxDistance = 60;
            }
            else if (answer == "e")
            {
                maxDistance = 90;
            }
            else
            {
                int convertToInt;
                try
                {
                    convertToInt = Convert.ToInt32(answer);
                }
                catch (OverflowException)
                {
                    Program.Print("Number outside of Int32 range");
                    throw;
                }
                catch (FormatException)
                {
                    Program.Print("Answer is not in a recognizable format.");
                    throw;
                }

                maxDistance = convertToInt;
            }

            return maxDistance;
        }
    }
}
