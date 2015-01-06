using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Graphs
{
    /**
 * Simple graph library; this is directed for now. This class focuses on time
 * efficiency rather than memory efficiency.
 *
 * @author sonalg
 * @author John Bauer
 *
 * @param <V>
 *          Type of vertices
 * @param <E>
 *          Type of edges.
 */

    public class DirectedMultiGraph<V, E> : Graph<V, E>
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
            this.outgoingEdges = outerMapFactory.newMap();
            this.incomingEdges = outerMapFactory.newMap();
        }

        /**
   * Creates a copy of the given graph. This will copy the entire data structure (this may be slow!), but will not copy
   * any of the edge or vertex objects.
   *
   * @param graph The graph to copy into this object.
   */

        public DirectedMultiGraph(DirectedMultiGraph<V, E> graph) :
            this(graph.outerMapFactory, graph.innerMapFactory)
        {
            foreach ( /*Map.Entry<V, Dictionary<V, List<E>>>*/var map in graph.outgoingEdges /*.entrySet()*/)
            {
                Dictionary<V, List<E>> edgesCopy = innerMapFactory.newMap();
                foreach ( /*Map.Entry<V, List<E>>*/var entry in map.Value)
                {
                    edgesCopy.Add(entry.Key, new List<E>(entry.Value));
                }
                this.outgoingEdges.Add(map.Key, edgesCopy);
            }
            foreach ( /*Map.Entry<V, Dictionary<V, List<E>>>*/var map in graph.incomingEdges)
            {
                Dictionary<V, List<E>> edgesCopy = innerMapFactory.newMap();
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

        public int hashCode()
        {
            return outgoingEdges.GetHashCode();
        }

        public bool equals(Object that)
        {
            if (that == this)
                return true;
            if (!(that is DirectedMultiGraph<V, E>))
                return false;
            return outgoingEdges.Equals(((DirectedMultiGraph<V, E>) that).outgoingEdges);
        }

        /**
   * For adding a zero degree vertex
   *
   * @param v
   */
        //@Override
        public bool addVertex(V v)
        {
            if (outgoingEdges.ContainsKey(v))
                return false;
            outgoingEdges.Add(v, innerMapFactory.newMap());
            incomingEdges.Add(v, innerMapFactory.newMap());
            return true;
        }

        private Dictionary<V, List<E>> getOutgoingEdgesMap(V v)
        {
            Dictionary<V, List<E>> map;
            outgoingEdges.TryGetValue(v, out map);
            if (map == null)
            {
                map = innerMapFactory.newMap();
                outgoingEdges.Add(v, map);
                incomingEdges.Add(v, innerMapFactory.newMap());
            }
            return map;
        }

        private Dictionary<V, List<E>> getIncomingEdgesMap(V v)
        {
            Dictionary<V, List<E>> map;
            incomingEdges.TryGetValue(v, out map);
            if (map == null)
            {
                outgoingEdges.Add(v, innerMapFactory.newMap());
                map = innerMapFactory.newMap();
                incomingEdges.Add(v, map);
            }
            return map;
        }

        /**
   * adds vertices (if not already in the graph) and the edge between them
   *
   * @param source
   * @param dest
   * @param data
   */
        //@Override
        public void add(V source, V dest, E data)
        {
            Dictionary<V, List<E>> outgoingMap = getOutgoingEdgesMap(source);
            Dictionary<V, List<E>> incomingMap = getIncomingEdgesMap(dest);

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

        //@Override
        public bool removeEdges(V source, V dest)
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

        //@Override
        public bool removeEdge(V source, V dest, E data)
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

        /**
   * remove a vertex (and its edges) from the graph.
   *
   * @param vertex
   * @return true if successfully removes the node
   */
        //@Override
        public bool removeVertex(V vertex)
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

        //@Override
        public bool removeVertices(ICollection<V> vertices)
        {
            bool changed = false;
            foreach (V v in vertices)
            {
                if (removeVertex(v))
                {
                    changed = true;
                }
            }
            return changed;
        }

        //@Override
        public int getNumVertices()
        {
            return outgoingEdges.Count;
        }

        //@Override
        public List<E> getOutgoingEdges(V v)
        {
            if (!outgoingEdges.ContainsKey(v))
            {
                //noinspection unchecked
                return new List<E>();
            }
            return outgoingEdges[v].Values.SelectMany(l => l).ToList();
        }

        //@Override
        public List<E> getIncomingEdges(V v)
        {
            if (!incomingEdges.ContainsKey(v))
            {
                //noinspection unchecked
                return new List<E>();
            }
            return incomingEdges[v].Values.SelectMany(l => l).ToList();
        }

        //@Override
        public int getNumEdges()
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

        //@Override
        public ReadOnlyCollection<V> getParents(V vertex)
        {
            Dictionary<V, List<E>> parentMap;
            incomingEdges.TryGetValue(vertex, out parentMap);
            if (parentMap == null)
                return null;
            return new ReadOnlyCollection<V>(parentMap.Keys.ToList());
        }

        //@Override
        public ReadOnlyCollection<V> getChildren(V vertex)
        {
            Dictionary<V, List<E>> childMap;
            outgoingEdges.TryGetValue(vertex, out childMap);
            if (childMap == null)
                return null;
            return new ReadOnlyCollection<V>(childMap.Keys.ToList());
        }

        /**
   * Gets both parents and children nodes
   *
   * @param v
   */
        //@Override
        public Set<V> getNeighbors(V v)
        {
            // TODO: pity we have to copy the sets... is there a combination set?
            ReadOnlyCollection<V> children = getChildren(v);
            ReadOnlyCollection<V> parents = getParents(v);

            if (children == null && parents == null)
                return null;
            Set<V> neighbors = innerMapFactory.newSet();
            neighbors.AddAll(children);
            neighbors.AddAll(parents);
            return neighbors;
        }

        /**
   * clears the graph, removes all edges and nodes
   */
        //@Override
        public void clear()
        {
            incomingEdges.Clear();
            outgoingEdges.Clear();
        }

        //@Override
        public bool containsVertex(V v)
        {
            return outgoingEdges.ContainsKey(v);
        }

        /**
   * only checks if there is an edge from source to dest. To check if it is
   * connected in either direction, use isNeighbor
   *
   * @param source
   * @param dest
   */
        //@Override
        public bool isEdge(V source, V dest)
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

        //@Override
        public bool isNeighbor(V source, V dest)
        {
            return isEdge(source, dest) || isEdge(dest, source);
        }

        //@Override
        public ReadOnlyCollection<V> getAllVertices()
        {
            return new ReadOnlyCollection<V>(outgoingEdges.Keys.ToList());
        }

        //@Override
        public List<E> getAllEdges()
        {
            List<E> edges = new List<E>();
            foreach (Dictionary<V, List<E>> e in outgoingEdges.Values)
            {
                foreach (List<E> ee in e.Values)
                {
                    edges.AddRange(ee);
                }
            }
            return edges;
        }

        /**
   * False if there are any vertices in the graph, true otherwise. Does not care
   * about the number of edges.
   */
        //@Override
        public bool isEmpty()
        {
            return !outgoingEdges.Any();
        }

        /**
   * Deletes nodes with zero incoming and zero outgoing edges
   */
        //@Override
        public void removeZeroDegreeNodes()
        {
            List<V> toDelete = new List<V>();
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

        //@Override
        public ReadOnlyCollection<E> getEdges(V source, V dest)
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

        /**
   * direction insensitive (the paths can go "up" or through the parents)
   */

        public List<V> getShortestPath(V node1, V node2)
        {
            if (!outgoingEdges.ContainsKey(node1) || !outgoingEdges.ContainsKey(node2))
            {
                return null;
            }
            return getShortestPath(node1, node2, false);
        }

        public List<E> getShortestPathEdges(V node1, V node2)
        {
            return convertPath(getShortestPath(node1, node2), false);
        }

        /**
   * can specify the direction sensitivity
   *
   * @param node1
   * @param node2
   * @param directionSensitive
   *          - whether the path can go through the parents
   * @return the list of nodes you get through to get there
   */

        public List<V> getShortestPath(V node1, V node2, bool directionSensitive)
        {
            if (!outgoingEdges.ContainsKey(node1) || !outgoingEdges.ContainsKey(node2))
            {
                return null;
            }
            return DijkstraShortestPath.getShortestPath(this, node1, node2, directionSensitive);
        }

        public List<E> getShortestPathEdges(V node1, V node2, bool directionSensitive)
        {
            return convertPath(getShortestPath(node1, node2, directionSensitive), directionSensitive);
        }

        public List<E> convertPath(List<V> nodes, bool directionSensitive)
        {
            if (nodes == null)
                return null;

            if (nodes.Count <= 1)
                return new List<E>();

            List<E> path = new List<E>();
            var nodeIterator = nodes.GetEnumerator();
            V previous = nodeIterator.Current;
            while (nodeIterator.MoveNext())
            {
                V next = nodeIterator.Current;
                E connection = default(E);
                var edges = getEdges(previous, next);
                if (edges.Count == 0 && !directionSensitive)
                {
                    edges = getEdges(next, previous);
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

        //@Override
        public int getInDegree(V vertex)
        {
            if (!containsVertex(vertex))
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

        //@Override
        public int getOutDegree(V vertex)
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

        //@Override
        public List<Set<V>> getConnectedComponents()
        {
            return ConnectedComponents.getConnectedComponents(this);
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

    @Override
    public bool hasNext() {
      primeIterator();
      return hasNext;
    }

    @Override
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

    @Override
    public void remove() {
      edgeIterator.Remove();
    }
  }*/

        /**
   * Cast this multi-graph as a map from vertices, to the outgoing data along edges out of those vertices.
   *
   * @return A map representation of the graph.
   */

        public Dictionary<V, List<E>> toMap()
        {
            Dictionary<V, List<E>> map = innerMapFactory.newMap();
            foreach (V vertex in getAllVertices())
            {
                map.Add(vertex, getOutgoingEdges(vertex));
            }
            return map;
        }

        //@Override
        public override String ToString()
        {
            StringBuilder s = new StringBuilder();
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

        private static readonly long serialVersionUID = 609823567298345145L;
    }
}