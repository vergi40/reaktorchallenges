using System.Collections.Generic;

namespace Santa_challenge
{
    class Trip
    {
        public List<Wish> Wishes { get; set; }

        public double TotalWeight { get; set; }


        public Trip()
        {
            Wishes = new List<Wish>();
        }

        public void Add(Wish wish)
        {
            Wishes.Add(wish);
            TotalWeight += wish.Weight;
        }
        
    }
}
