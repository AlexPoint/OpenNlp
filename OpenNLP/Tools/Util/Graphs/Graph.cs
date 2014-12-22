using System;
using System.Collections.Generic;
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
    public interface Graph<V,E>
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
        void add(V source, V dest, E data);
        /**
         * For adding a zero degree vertex
         *
         * @param v
         */

        bool addVertex(V v);



        bool removeEdges(V source, V dest);

        bool removeEdge(V source, V dest, E data);

        /**
         * remove a vertex (and its edges) from the graph.
         *
         * @param vertex
         * @return true if successfully removes the node
         */
        bool removeVertex(V vertex);

        bool removeVertices(ICollection<V> vertices);

        int getNumVertices();

        /**
         * for undirected graph, it is just the edges from the node
         * @param v
         */
        List<E> getOutgoingEdges(V v);

        /**
         * for undirected graph, it is just the edges from the node
         * @param v
         */
        List<E> getIncomingEdges(V v);

        int getNumEdges();

        /**
         * for undirected graph, it is just the neighbors
         * @param vertex
         */
        Set<V> getParents(V vertex);

        /**
         * for undirected graph, it is just the neighbors
         * @param vertex
         */

        Set<V> getChildren(V vertex);

        Set<V> getNeighbors(V v);

        /**
         * clears the graph, removes all edges and nodes
         */
        void clear();

        bool containsVertex(V v);

        /**
         * only checks if there is an edge from source to dest. To check if it is
         * connected in either direction, use isNeighbor
         *
         * @param source
         * @param dest
         */
        bool isEdge(V source, V dest);

        bool isNeighbor(V source, V dest);

        Set<V> getAllVertices();

        List<E> getAllEdges();

        /**
         * False if there are any vertices in the graph, true otherwise. Does not care
         * about the number of edges.
         */
        bool isEmpty();

        /**
         * Deletes nodes with zero incoming and zero outgoing edges
         */
        void removeZeroDegreeNodes();

        List<E> getEdges(V source, V dest);


        /**
         * for undirected graph, it should just be the degree
         * @param vertex
         */
        int getInDegree(V vertex);

        int getOutDegree(V vertex);

        List<Set<V>> getConnectedComponents();
    }
}
