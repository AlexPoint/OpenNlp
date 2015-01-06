using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Graphs
{
    public static class DijkstraShortestPath
    {
        public static /*<V, E>*/ List<V> getShortestPath<V, E>(Graph<V, E> graph,
            V node1, V node2,
            bool directionSensitive)
        {
            if (node1.Equals(node2))
            {
                //return Collections.singletonList(node2);
                return new List<V>() {node2};
            }

            Set<V> visited = new HashSet<V>();

            Dictionary<V, V> previous = new Dictionary<V, V>();

            BinaryHeapPriorityQueue<V> unsettledNodes =
                new BinaryHeapPriorityQueue<V>();

            unsettledNodes.add(node1, 0);

            while (unsettledNodes.size() > 0)
            {
                double distance = unsettledNodes.getPriority();
                V u = unsettledNodes.removeFirst();
                visited.Add(u);

                if (u.Equals(node2))
                    break;

                unsettledNodes.remove(u);

                ReadOnlyCollection<V> candidates = ((directionSensitive)
                    ? graph.getChildren(u)
                    : new ReadOnlyCollection<V>(graph.getNeighbors(u)));
                foreach (V candidate in candidates)
                {
                    double alt = distance - 1;
                    // nodes not already present will have a priority of -inf
                    if (alt > unsettledNodes.getPriority(candidate) &&
                        !visited.Contains(candidate))
                    {
                        unsettledNodes.relaxPriority(candidate, alt);
                        previous.Add(candidate, u);
                    }
                }
            }
            if (!previous.ContainsKey(node2))
                return null;
            List<V> path = new List<V>();
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