using System.Collections.Generic;

namespace Santa_challenge
{
    class WishList
    {
        public List<Wish> Wishes { get; set; }

        public WishList(List<string> unformattedList)
        {
            Wishes = new List<Wish>();

            foreach (var listItem in unformattedList)
            {
                Wish wish = new Wish(listItem);
                Wishes.Add(wish);
            }
        }
    }
}
