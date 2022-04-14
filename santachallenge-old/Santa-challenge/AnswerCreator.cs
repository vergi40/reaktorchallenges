using System.Collections.Generic;

namespace Santa_challenge
{
    class AnswerCreator
    {
        public Trip CurrentTrip { get; set; }
        
        public List<Trip> ReadyTrips { get; set; }


        public AnswerCreator()
        {
            ReadyTrips = new List<Trip>();
            CurrentTrip = new Trip();
        }
        
        public void NewTrip()
        {
            ReadyTrips.Add(CurrentTrip);
            CurrentTrip = new Trip();
        }

        public void AddWish(Wish wish)
        {
            CurrentTrip.Add(wish);
        }

        public bool DoesWishFitToCurrentTrip(Wish wish)
        {
            if (CurrentTrip.TotalWeight + wish.Weight < Algorithm.MaxWeight)
            {
                return true;
            }

            return false;
        }

        public List<string> PrintTrips()
        {
            List<string> print = new List<string>();

            foreach (Trip trip in ReadyTrips)
            {
                string row = "";
                foreach (Wish wish in trip.Wishes)
                {
                    row += wish.Id + Program.SEPARATOR + " ";
                }

                // csv format: delete last semicolon and whitespace
                row = row.Substring(0, row.Length - 2);
                print.Add(row);
            }

            return print;
        }
    }
}
