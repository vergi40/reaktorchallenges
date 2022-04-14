using System.Collections.Generic;
using System.Xml.Serialization;

namespace Santa_challenge.Kruskal
{
    /// <summary>
    /// https://www.codeproject.com/Tips/772779/%2FTips%2F772779%2FKruskal-Algorithm-in-Csharp
    /// </summary>
    [XmlRoot]
    public class XmlEdgeList
    {
        public List<Edge> Edges { get; set; }
        public XmlEdgeList() { }
    }


    public class Edge
    {
        public Wish StartPoint { get; set; }

        public List<Wish> StartConnections { get; set; }

        public Wish EndPoint { get; set; }

        public List<Wish> EndConnections { get; set; }

        [XmlIgnore]
        public Edge NextEdge { get; set; }

        public double Weight { get; set; }

        [XmlIgnore]
        public ConnectionState StartState
        {
            get => StartPoint.State;
            set => StartPoint.State = value;
        }

        [XmlIgnore]
        public ConnectionState EndState
        {
            get => EndPoint.State;
            set => EndPoint.State = value;
        }

        /// <summary>
        /// For xml
        /// </summary>
        public Edge() { }

        public Edge(Wish start, Wish end)
        {
            StartPoint = start;
            EndPoint = end;
            Weight = start.DistanceTo(end);

            StartConnections = new List<Wish>();
            EndConnections = new List<Wish>();

            StartPoint.EdgeList.Add(this);
            EndPoint.EdgeList.Add(this);
        }
    }

    
}
