using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Graphs
{
    /**
 *
 * @author Sonal Gupta
 * @param <V> Type of the vertices
 * @param <E> Type of the edges
 */

    public interface Graph<V, E>
    {
        /**
   * Adds vertices (if not already in the graph) and the edge between them.
   * (If the graph is undirected, the choice of which vertex to call
   * source and dest is arbitrary.)
   *
   * @param source
   * @param dest
   * @param data
   */
        void Add(V source, V dest, E data);
        /**
         * For adding a zero degree vertex
         *
         * @param v
         */

        bool AddVertex(V v);



        bool RemoveEdges(V source, V dest);

        bool RemoveEdge(V source, V dest, E data);

        /**
         * remove a vertex (and its edges) from the graph.
         *
         * @param vertex
         * @return true if successfully removes the node
         */
        bool RemoveVertex(V vertex);

        bool RemoveVertices(ICollection<V> vertices);

        int GetNumVertices();

        /**
         * for undirected graph, it is just the edges from the node
         * @param v
         */
        List<E> GetOutgoingEdges(V v);

        /**
         * for undirected graph, it is just the edges from the node
         * @param v
         */
        List<E> GetIncomingEdges(V v);

        int GetNumEdges();

        /**
         * for undirected graph, it is just the neighbors
         * @param vertex
         */
        ReadOnlyCollection<V> GetParents(V vertex);

        /**
         * for undirected graph, it is just the neighbors
         * @param vertex
         */

        ReadOnlyCollection<V> GetChildren(V vertex);

        Set<V> GetNeighbors(V v);

        /**
         * clears the graph, removes all edges and nodes
         */
        void Clear();

        bool ContainsVertex(V v);

        /**
         * only checks if there is an edge from source to dest. To check if it is
         * connected in either direction, use isNeighbor
         *
         * @param source
         * @param dest
         */
        bool IsEdge(V source, V dest);

        bool IsNeighbor(V source, V dest);

        ReadOnlyCollection<V> GetAllVertices();

        List<E> GetAllEdges();

        /**
         * False if there are any vertices in the graph, true otherwise. Does not care
         * about the number of edges.
         */
        bool IsEmpty();

        /**
         * Deletes nodes with zero incoming and zero outgoing edges
         */
        void RemoveZeroDegreeNodes();

        ReadOnlyCollection<E> GetEdges(V source, V dest);


        /**
         * for undirected graph, it should just be the degree
         * @param vertex
         */
        int GetInDegree(V vertex);

        int GetOutDegree(V vertex);

        List<Set<V>> GetConnectedComponents();
    }
}