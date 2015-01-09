using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Graphs
{
    public static class DijkstraShortestPath
    {
        public static /*<V, E>*/ List<V> GetShortestPath<V, E>(IGraph<V, E> graph,
            V node1, V node2,
            bool directionSensitive)
        {
            if (node1.Equals(node2))
            {
                //return Collections.singletonList(node2);
                return new List<V>() {node2};
            }

            Set<V> visited = new Util.HashSet<V>();

            var previous = new Dictionary<V, V>();

            var unsettledNodes = new BinaryHeapPriorityQueue<V>();

            unsettledNodes.Add(node1, 0);

            while (unsettledNodes.Size() > 0)
            {
                double distance = unsettledNodes.GetPriority();
                V u = unsettledNodes.RemoveFirst();
                visited.Add(u);

                if (u.Equals(node2))
                    break;

                unsettledNodes.Remove(u);

                ReadOnlyCollection<V> candidates = ((directionSensitive)
                    ? graph.GetChildren(u)
                    : new ReadOnlyCollection<V>(graph.GetNeighbors(u)));
                foreach (V candidate in candidates)
                {
                    double alt = distance - 1;
                    // nodes not already present will have a priority of -inf
                    if (alt > unsettledNodes.GetPriority(candidate) &&
                        !visited.Contains(candidate))
                    {
                        unsettledNodes.RelaxPriority(candidate, alt);
                        previous[candidate] = u;
                    }
                }
            }
            if (!previous.ContainsKey(node2))
                return null;
            var path = new List<V>();
            path.Add(node2);
            V n = node2;
            while (previous.ContainsKey(n))
            {
                path.Add(previous[n]);
                n = previous[n];
            }
            path.Reverse();
            return path;
        }
    }
}