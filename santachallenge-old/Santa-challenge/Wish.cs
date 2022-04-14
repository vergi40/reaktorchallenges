using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml.Serialization;
using Santa_challenge.Kruskal;

namespace Santa_challenge
{
    public class Wish
    {
        /// <summary>
        /// Represents identification of the child and present
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// X
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// In grams.
        /// </summary>
        public double Weight { get; set; }

        [XmlIgnore]
        public ConnectionState State { get; set; }

        [XmlIgnore]
        public List<Edge> EdgeList { get; set; }

        /// <summary>
        /// For xml
        /// </summary>
        public Wish() { }

        public Wish(string unformattedRow)
        {
            var itemsArray = unformattedRow.Split(Program.SEPARATOR);

            Id = itemsArray[0];
            Latitude = double.Parse(itemsArray[1], CultureInfo.InvariantCulture);
            Longitude = double.Parse(itemsArray[2], CultureInfo.InvariantCulture);
            Weight = double.Parse(itemsArray[3], CultureInfo.InvariantCulture);

            State = ConnectionState.White;
            EdgeList = new List<Edge>();

        }

        public double DistanceTo(Wish wish)
        {
            Vector a = new Vector(Longitude, Latitude);
            var b = new Vector(wish.Longitude, wish.Latitude);

            var resultVector = b - a;

            // Remember that world is round
            if (Math.Abs(b.X - a.X) > 180)
            {
                if (a.X < 0)
                {
                    a.X += 180;
                }
                else
                {
                    a.X -= 180;
                }

                // TODO check that this works
                resultVector = b - a;
                return resultVector.Length;
            }

            return resultVector.Length;
        }

        public double DistanceToBase()
        {
            return AlgorithmHelpers.DistanceToBase(this);
        }


    }

    /// <summary>
    /// White state: no connections
    /// Gray, has one wish connected
    /// Black, has two or more wishes connected
    /// </summary>
    public enum ConnectionState
    {
        White,
        Gray,
        Black
    }
}
