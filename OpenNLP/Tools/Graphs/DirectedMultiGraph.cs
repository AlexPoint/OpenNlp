using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Graphs
{
    /// <summary>
    /// Simple graph library; this is directed for now. This class focuses on time
    /// efficiency rather than memory efficiency.
    /// 
    /// @author sonalg
    /// @author John Bauer
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    /// <typeparam name="V">Type of vertices</typeparam>
    /// <typeparam name="E">Type of edges.</typeparam>
    public class DirectedMultiGraph<V, E> : IGraph<V, E>
    {
        private readonly Dictionary<V, Dictionary<V, List<E>>> outgoingEdges;

        private readonly Dictionary<V, Dictionary<V, List<E>>> incomingEdges;

        private readonly MapFactory<V, Dictionary<V, List<E>>> outerMapFactory;
        private readonly MapFactory<V, List<E>> innerMapFactory;

        public DirectedMultiGraph() :
            this(
            MapFactory<V, Dictionary<V, List<E>>>.hashMapFactory<V, Dictionary<V, List<E>>>(),
            MapFactory<V, List<E>>.hashMapFactory<V, List<E>>())
        {
        }

        public DirectedMultiGraph(MapFactory<V, Dictionary<V, List<E>>> outerMapFactory,
            MapFactory<V, List<E>> innerMapFactory)
        {
            this.outerMapFactory = outerMapFactory;
            this.innerMapFactory = innerMapFactory;
            this.outgoingEdges = outerMapFactory.NewMap();
            this.incomingEdges = outerMapFactory.NewMap();
        }

        /// <summary>
        /// Creates a copy of the given graph. This will copy the entire data structure (this may be slow!), 
        /// but will not copy any of the edge or vertex objects.
        /// </summary>
        /// <param name="graph">The graph to copy into this object.</param>
        public DirectedMultiGraph(DirectedMultiGraph<V, E> graph) :
            this(graph.outerMapFactory, graph.innerMapFactory)
        {
            foreach ( /*Map.Entry<V, Dictionary<V, List<E>>>*/var map in graph.outgoingEdges /*.entrySet()*/)
            {
                Dictionary<V, List<E>> edgesCopy = innerMapFactory.NewMap();
                foreach ( /*Map.Entry<V, List<E>>*/var entry in map.Value)
                {
                    edgesCopy.Add(entry.Key, new List<E>(entry.Value));
                }
                this.outgoingEdges.Add(map.Key, edgesCopy);
            }
            foreach ( /*Map.Entry<V, Dictionary<V, List<E>>>*/var map in graph.incomingEdges)
            {
                Dictionary<V, List<E>> edgesCopy = innerMapFactory.NewMap();
                foreach ( /*Map.Entry<V, List<E>>*/var entry in map.Value)
                {
                    edgesCopy.Add(entry.Key, new List<E>(entry.Value));
                }
                this.incomingEdges.Add(map.Key, edgesCopy);
            }
        }

        /**
         * Be careful hashing these. They are mutable objects, and changing the object
         * will throw off the hash code, messing up your hash table
         */

        public override int GetHashCode()
        {
            return outgoingEdges.GetHashCode();
        }

        public override bool Equals(Object that)
        {
            if (that == this)
                return true;
            if (!(that is DirectedMultiGraph<V, E>))
                return false;
            return outgoingEdges.Equals(((DirectedMultiGraph<V, E>) that).outgoingEdges);
        }

        /// <summary>
        /// For adding a zero degree vertex
        /// </summary>
        public bool AddVertex(V v)
        {
            if (outgoingEdges.ContainsKey(v))
                return false;
            outgoingEdges.Add(v, innerMapFactory.NewMap());
            incomingEdges.Add(v, innerMapFactory.NewMap());
            return true;
        }

        private Dictionary<V, List<E>> GetOutgoingEdgesMap(V v)
        {
            Dictionary<V, List<E>> map;
            outgoingEdges.TryGetValue(v, out map);
            if (map == null)
            {
                map = innerMapFactory.NewMap();
                outgoingEdges.Add(v, map);
                incomingEdges.Add(v, innerMapFactory.NewMap());
            }
            return map;
        }

        private Dictionary<V, List<E>> GetIncomingEdgesMap(V v)
        {
            Dictionary<V, List<E>> map;
            incomingEdges.TryGetValue(v, out map);
            if (map == null)
            {
                outgoingEdges.Add(v, innerMapFactory.NewMap());
                map = innerMapFactory.NewMap();
                incomingEdges.Add(v, map);
            }
            return map;
        }

        /// <summary>
        /// adds vertices (if not already in the graph) and the edge between them
        /// </summary>
        public void Add(V source, V dest, E data)
        {
            Dictionary<V, List<E>> outgoingMap = GetOutgoingEdgesMap(source);
            Dictionary<V, List<E>> incomingMap = GetIncomingEdgesMap(dest);

            List<E> outgoingList;
            outgoingMap.TryGetValue(dest, out outgoingList);
            if (outgoingList == null)
            {
                outgoingList = new List<E>();
                outgoingMap.Add(dest, outgoingList);
            }

            List<E> incomingList;
            incomingMap.TryGetValue(source, out incomingList);
            if (incomingList == null)
            {
                incomingList = new List<E>();
                incomingMap.Add(source, incomingList);
            }

            outgoingList.Add(data);
            incomingList.Add(data);
        }

        public bool RemoveEdges(V source, V dest)
        {
            if (!outgoingEdges.ContainsKey(source))
            {
                return false;
            }
            if (!incomingEdges.ContainsKey(dest))
            {
                return false;
            }
            if (!outgoingEdges[source].ContainsKey(dest))
            {
                return false;
            }
            outgoingEdges[source].Remove(dest);
            incomingEdges[dest].Remove(source);
            return true;
        }

        public bool RemoveEdge(V source, V dest, E data)
        {
            if (!outgoingEdges.ContainsKey(source))
            {
                return false;
            }
            if (!incomingEdges.ContainsKey(dest))
            {
                return false;
            }
            if (!outgoingEdges[source].ContainsKey(dest))
            {
                return false;
            }
            bool foundOut = outgoingEdges.ContainsKey(source) && outgoingEdges[source].ContainsKey(dest) &&
                            outgoingEdges[source][dest].Remove(data);
            bool foundIn = incomingEdges.ContainsKey(dest) && incomingEdges[dest].ContainsKey(source) &&
                           incomingEdges[dest][source].Remove(data);
            if (foundOut && !foundIn)
            {
                throw new InvalidDataException("Edge found in outgoing but not incoming");
            }
            if (foundIn && !foundOut)
            {
                throw new InvalidDataException("Edge found in incoming but not outgoing");
            }
            // TODO: cut down the number of .get calls
            if (outgoingEdges.ContainsKey(source) &&
                (!outgoingEdges[source].ContainsKey(dest) || outgoingEdges[source][dest].Count == 0))
            {
                outgoingEdges[source].Remove(dest);
            }
            if (incomingEdges.ContainsKey(dest) &&
                (!incomingEdges[dest].ContainsKey(source) || incomingEdges[dest][source].Count == 0))
            {
                incomingEdges[dest].Remove(source);
            }
            return foundOut;
        }

        /// <summary>
        /// remove a vertex (and its edges) from the graph.
        /// </summary>
        /// <returns>true if successfully removes the node</returns>
        public bool RemoveVertex(V vertex)
        {
            if (!outgoingEdges.ContainsKey(vertex))
            {
                return false;
            }
            foreach (var other in outgoingEdges[vertex])
            {
                incomingEdges[other.Key].Remove(vertex);
            }
            foreach (var other in incomingEdges[vertex])
            {
                outgoingEdges[other.Key].Remove(vertex);
            }
            outgoingEdges.Remove(vertex);
            incomingEdges.Remove(vertex);
            return true;
        }

        public bool RemoveVertices(ICollection<V> vertices)
        {
            bool changed = false;
            foreach (V v in vertices)
            {
                if (RemoveVertex(v))
                {
                    changed = true;
                }
            }
            return changed;
        }

        public int GetNumVertices()
        {
            return outgoingEdges.Count;
        }

        public List<E> GetOutgoingEdges(V v)
        {
            if (!outgoingEdges.ContainsKey(v))
            {
                //noinspection unchecked
                return new List<E>();
            }
            return outgoingEdges[v].Values.SelectMany(l => l).ToList();
        }

        public List<E> GetIncomingEdges(V v)
        {
            if (!incomingEdges.ContainsKey(v))
            {
                //noinspection unchecked
                return new List<E>();
            }
            return incomingEdges[v].Values.SelectMany(l => l).ToList();
        }

        public int GetNumEdges()
        {
            int count = 0;
            foreach ( /*Map.Entry<V, Dictionary<V, List<E>>>*/var sourceEntry in outgoingEdges)
            {
                foreach ( /*Map.Entry<V, List<E>>*/var destEntry in sourceEntry.Value /*.entrySet()*/)
                {
                    count += destEntry.Value.Count;
                }
            }
            return count;
        }

        public ReadOnlyCollection<V> GetParents(V vertex)
        {
            Dictionary<V, List<E>> parentMap;
            incomingEdges.TryGetValue(vertex, out parentMap);
            if (parentMap == null)
                return null;
            return new ReadOnlyCollection<V>(parentMap.Keys.ToList());
        }

        public ReadOnlyCollection<V> GetChildren(V vertex)
        {
            Dictionary<V, List<E>> childMap;
            outgoingEdges.TryGetValue(vertex, out childMap);
            if (childMap == null)
                return null;
            return new ReadOnlyCollection<V>(childMap.Keys.ToList());
        }

        /// <summary>
        /// Gets both parents and children nodes
        /// </summary>
        public Set<V> GetNeighbors(V v)
        {
            // TODO: pity we have to copy the sets... is there a combination set?
            ReadOnlyCollection<V> children = GetChildren(v);
            ReadOnlyCollection<V> parents = GetParents(v);

            if (children == null && parents == null)
                return null;
            Set<V> neighbors = innerMapFactory.NewSet();
            neighbors.AddAll(children);
            neighbors.AddAll(parents);
            return neighbors;
        }

        /// <summary>
        /// clears the graph, removes all edges and nodes
        /// </summary>
        public void Clear()
        {
            incomingEdges.Clear();
            outgoingEdges.Clear();
        }

        public bool ContainsVertex(V v)
        {
            return outgoingEdges.ContainsKey(v);
        }

        /// <summary>
        /// Only checks if there is an edge from source to dest.
        /// To check if it is connected in either direction, use isNeighbor
        /// </summary>
        public bool IsEdge(V source, V dest)
        {
            Dictionary<V, List<E>> childrenMap;
            outgoingEdges.TryGetValue(source, out childrenMap);
            if (childrenMap == null || !childrenMap.Any())
                return false;
            List<E> edges;
            childrenMap.TryGetValue(dest, out edges);
            if (edges == null || !edges.Any())
                return false;
            return edges.Count > 0;
        }

        public bool IsNeighbor(V source, V dest)
        {
            return IsEdge(source, dest) || IsEdge(dest, source);
        }

        public ReadOnlyCollection<V> GetAllVertices()
        {
            return new ReadOnlyCollection<V>(outgoingEdges.Keys.ToList());
        }

        public List<E> GetAllEdges()
        {
            var edges = new List<E>();
            foreach (Dictionary<V, List<E>> e in outgoingEdges.Values)
            {
                foreach (List<E> ee in e.Values)
                {
                    edges.AddRange(ee);
                }
            }
            return edges;
        }

        /// <summary>
        /// False if there are any vertices in the graph, true otherwise.
        /// Does not care about the number of edges.
        /// </summary>
        public bool IsEmpty()
        {
            return !outgoingEdges.Any();
        }

        /// <summary>
        /// Deletes nodes with zero incoming and zero outgoing edges
        /// </summary>
        public void RemoveZeroDegreeNodes()
        {
            var toDelete = new List<V>();
            foreach (var vertex in outgoingEdges)
            {
                if (!outgoingEdges[vertex.Key].Any() && !incomingEdges[vertex.Key].Any())
                {
                    toDelete.Add(vertex.Key);
                }
            }
            foreach (var vertex in toDelete)
            {
                outgoingEdges.Remove(vertex);
                incomingEdges.Remove(vertex);
            }
        }

        public ReadOnlyCollection<E> GetEdges(V source, V dest)
        {
            Dictionary<V, List<E>> childrenMap = outgoingEdges[source];
            if (childrenMap == null)
            {
                return new ReadOnlyCollection<E>(new E[] {});
            }
            List<E> edges = childrenMap[dest];
            if (edges == null)
            {
                return new ReadOnlyCollection<E>(new E[] {});
            }
            return new ReadOnlyCollection<E>(edges);
        }

        /// <summary>
        /// Direction insensitive (the paths can go "up" or through the parents)
        /// </summary>
        public List<V> GetShortestPath(V node1, V node2)
        {
            if (!outgoingEdges.ContainsKey(node1) || !outgoingEdges.ContainsKey(node2))
            {
                return null;
            }
            return GetShortestPath(node1, node2, false);
        }

        public List<E> GetShortestPathEdges(V node1, V node2)
        {
            return ConvertPath(GetShortestPath(node1, node2), false);
        }
        
        /// <summary>
        /// Can specify the direction sensitivity
        /// </summary>
        /// <param name="directionSensitive">whether the path can go through the parents</param>
        /// <returns>the list of nodes you get through to get there</returns>
        public List<V> GetShortestPath(V node1, V node2, bool directionSensitive)
        {
            if (!outgoingEdges.ContainsKey(node1) || !outgoingEdges.ContainsKey(node2))
            {
                return null;
            }
            return DijkstraShortestPath.GetShortestPath(this, node1, node2, directionSensitive);
        }

        public List<E> GetShortestPathEdges(V node1, V node2, bool directionSensitive)
        {
            return ConvertPath(GetShortestPath(node1, node2, directionSensitive), directionSensitive);
        }

        public List<E> ConvertPath(List<V> nodes, bool directionSensitive)
        {
            if (nodes == null)
                return null;

            if (nodes.Count <= 1)
                return new List<E>();

            var path = new List<E>();
            var nodeIterator = nodes.GetEnumerator();
            V previous = nodeIterator.Current;
            while (nodeIterator.MoveNext())
            {
                V next = nodeIterator.Current;
                E connection = default(E);
                var edges = GetEdges(previous, next);
                if (edges.Count == 0 && !directionSensitive)
                {
                    edges = GetEdges(next, previous);
                }
                if (edges.Count > 0)
                {
                    connection = edges[0];
                }
                else
                {
                    throw new ArgumentException("Path given with missing " + "edge connection");
                }
                path.Add(connection);
                previous = next;
            }
            return path;
        }

        public int GetInDegree(V vertex)
        {
            if (!ContainsVertex(vertex))
            {
                return 0;
            }
            int result = 0;
            Dictionary<V, List<E>> incoming = incomingEdges[vertex];
            foreach (List<E> edges in incoming.Values)
            {
                result += edges.Count;
            }
            return result;
        }

        public int GetOutDegree(V vertex)
        {
            int result = 0;
            Dictionary<V, List<E>> outgoing = outgoingEdges[vertex];
            if (outgoing == null)
            {
                return 0;
            }
            foreach (List<E> edges in outgoing.Values)
            {
                result += edges.Count;
            }
            return result;
        }

        public List<Set<V>> GetConnectedComponents()
        {
            return ConnectedComponents.GetConnectedComponents(this);
        }

        /*public Iterator<E> incomingEdgeIterator(readonly V vertex) {
        return new EdgeIterator<V, E>(incomingEdges, vertex);
        }

        public Iterable<E> incomingEdgeIterable(readonly V vertex) {
        return () -> new EdgeIterator<V, E>(incomingEdges, vertex);
        }

        public Iterator<E> outgoingEdgeIterator(readonly V vertex) {
        return new EdgeIterator<V, E>(outgoingEdges, vertex);
        }

        public Iterable<E> outgoingEdgeIterable(readonly V vertex) {
        return () -> new EdgeIterator<V, E>(outgoingEdges, vertex);
        }

        public Iterator<E> edgeIterator() {
        return new EdgeIterator<V, E>(this);
        }

        public Iterable<E> edgeIterable() {
        return () -> new EdgeIterator<V, E>(DirectedMultiGraph.this);
        }*/

        /*static class EdgeIterator<V, E> : Iterator<E> {
            private Iterator<Dictionary<V, List<E>>> vertexIterator;
            private Iterator<List<E>> connectionIterator;
            private Iterator<E> edgeIterator;
            private bool hasNext = true;


            public EdgeIterator(DirectedMultiGraph<V, E> graph) {
              vertexIterator = graph.outgoingEdges.Values.iterator();
            }

            public EdgeIterator(Dictionary<V, Dictionary<V, List<E>>> source, V startVertex) {
              Dictionary<V, List<E>> neighbors = source[startVertex];
              if (neighbors == null) {
                return;
              }
              vertexIterator = null;
              connectionIterator = neighbors.values().iterator();
            }

            public bool hasNext() {
              primeIterator();
              return hasNext;
            }

            public E next() {
              if (!hasNext()) {
                throw new NoSuchElementException("Graph edge iterator exhausted.");
              }
              return edgeIterator.next();
            }

            private void primeIterator() {
              if (edgeIterator != null && edgeIterator.hasNext()) {
                hasNext = true;  // technically, we shouldn't need to put this here, but let's be safe
              } else if (connectionIterator != null && connectionIterator.hasNext()) {
                edgeIterator = connectionIterator.next().iterator();
                primeIterator();
              } else if (vertexIterator != null && vertexIterator.hasNext()) {
                connectionIterator = vertexIterator.next().values().iterator();
                primeIterator();
              } else {
                hasNext = false;
              }
            }

            public void remove() {
              edgeIterator.Remove();
            }
          }*/
        
        /// <summary>
        /// Cast this multi-graph as a map from vertices, to the outgoing data along edges out of those vertices.
        /// </summary>
        /// <returns>A map representation of the graph.</returns>
        public Dictionary<V, List<E>> ToMap()
        {
            Dictionary<V, List<E>> map = innerMapFactory.NewMap();
            foreach (V vertex in GetAllVertices())
            {
                map.Add(vertex, GetOutgoingEdges(vertex));
            }
            return map;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("{\n");
            s.Append("Vertices:\n");
            foreach (var vertex in outgoingEdges)
            {
                s.Append("  ").Append(vertex.Key).Append('\n');
            }
            s.Append("Edges:\n");
            foreach (var source in outgoingEdges)
            {
                foreach (var dest in outgoingEdges[source.Key])
                {
                    foreach (var edge in outgoingEdges[source.Key][dest.Key])
                    {
                        s.Append("  ")
                            .Append(source.Key)
                            .Append(" -> ")
                            .Append(dest.Key)
                            .Append(" : ")
                            .Append(edge)
                            .Append('\n');
                    }
                }
            }
            s.Append('}');
            return s.ToString();
        }
    }
}