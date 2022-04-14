using System;
using System.Collections.Generic;

namespace Santa_challenge.Kruskal
{
    public enum TreeEdgeRelation
    {
        NoSimilarPoints,
        ContainsOnePoint,
        ContainsBothPoints
    }

    class EdgeTree
    {
        public const int MAX_CONNECTIONS = 3;

        public Guid Id { get; }
        public List<Edge> Edges { get; set; }


        public EdgeTree(Edge edge)
        {
            Id = Guid.NewGuid();
            edge.StartState = ConnectionState.Gray;
            edge.EndState = ConnectionState.Gray;
            Edges = new List<Edge>{edge};
        }

        /// <summary>
        /// Can contain ONE point, otherwise false
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public TreeEdgeRelation IfContainsSamePointAddToTree(Edge edge)
        {
            var edgeContainingStartPoint = ExistingEdgeContainsStartPoint(edge);
            var edgeContainingEndPoint = ExistingEdgeContainsEndPoint(edge);

            // Skip redundant cases first
            if (edgeContainingStartPoint != null && edgeContainingEndPoint != null)
            {
                return TreeEdgeRelation.ContainsBothPoints;
            }
            else if(edgeContainingStartPoint == null && edgeContainingEndPoint == null)
            {
                return TreeEdgeRelation.NoSimilarPoints;
            }
            else if (edgeContainingStartPoint != null)
            {
                LinkStartPoint(edge, edgeContainingStartPoint);
                edge.StartState = ConnectionState.Gray;
            }
            else
            {
                LinkEndPoint(edge, edgeContainingEndPoint);
                edge.EndState = ConnectionState.Gray;
            }

            Edges.Add(edge);
            return TreeEdgeRelation.ContainsOnePoint;
        }

        // Higher level linking

        private void LinkStartPoint(Edge newEdge, Edge existingEdge)
        {
            if (existingEdge.StartPoint == newEdge.StartPoint)
            {
                LinkStartToStart(newEdge, existingEdge);
            }
            else if (existingEdge.EndPoint == newEdge.StartPoint)
            {
                LinkNewStartToOldEnd(newEdge, existingEdge);
            }
        }

        private void LinkEndPoint(Edge newEdge, Edge existingEdge)
        {
            if (existingEdge.StartPoint == newEdge.EndPoint)
            {
                LinkNewEndToOldStart(newEdge, existingEdge);
            }

            if (existingEdge.EndPoint == newEdge.EndPoint)
            {
                LinkEndToEnd(newEdge, existingEdge);
            }
        }

        private void LinkEndToEnd(Edge newEdge, Edge existingEdge)
        {
            if(existingEdge.EndConnections.Count < MAX_CONNECTIONS)
                existingEdge.EndConnections.Add(newEdge.EndPoint);

            if(newEdge.EndConnections.Count < MAX_CONNECTIONS)
                newEdge.EndConnections.Add(existingEdge.EndPoint);
        }

        // Lower level linking

        private void LinkStartToStart(Edge newEdge, Edge existingEdge)
        {
            if(existingEdge.StartConnections.Count < MAX_CONNECTIONS)
                existingEdge.StartConnections.Add(newEdge.StartPoint);

            if(newEdge.StartConnections.Count < MAX_CONNECTIONS)
                newEdge.StartConnections.Add(existingEdge.StartPoint);
        }

        private void LinkNewEndToOldStart(Edge newEdge, Edge existingEdge)
        {
            if(existingEdge.StartConnections.Count < MAX_CONNECTIONS)
                existingEdge.StartConnections.Add(newEdge.EndPoint);

            if(newEdge.EndConnections.Count < MAX_CONNECTIONS)
                newEdge.EndConnections.Add(existingEdge.StartPoint);
        }
        private void LinkNewStartToOldEnd(Edge newEdge, Edge existingEdge)
        {
            if(existingEdge.EndConnections.Count < MAX_CONNECTIONS)
                existingEdge.EndConnections.Add(newEdge.StartPoint);

            if(newEdge.StartConnections.Count < MAX_CONNECTIONS)
                newEdge.StartConnections.Add(existingEdge.EndPoint);
        }

        

        private Edge ExistingEdgeContainsStartPoint(Edge newEdge)
        {
            foreach (var edge in Edges)
            {
                if (edge.StartPoint == newEdge.StartPoint)
                {
                    return edge;
                }

                if (edge.EndPoint == newEdge.StartPoint)
                {
                    return edge;
                }
            }

            return null;
        }

        private Edge ExistingEdgeContainsEndPoint(Edge newEdge)
        {
            foreach (var edge in Edges)
            {
                if (edge.StartPoint == newEdge.EndPoint)
                {
                    return edge;
                }

                if (edge.EndPoint == newEdge.EndPoint)
                {
                    return edge;
                }
            }

            return null;
        }


        public void Add(Edge edge)
        {
            //if (ContainsPoint(edge.StartPoint))
            //{
            //    edge.StartState = ConnectionState.Black;
            //}
            //else if (ContainsPoint(edge.EndPoint))
            //{
            //    edge.EndState = ConnectionState.Black;
            //}

            Edges.Add(edge);
        }
        
    }
}
