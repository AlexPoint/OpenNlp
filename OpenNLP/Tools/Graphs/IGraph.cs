using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Graphs
{
    
    /// <summary>
    /// @author Sonal Gupta
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    /// <typeparam name="V">Type of the vertices</typeparam>
    /// <typeparam name="E">Type of the edges</typeparam>
    public interface IGraph<V, E>
    {

        /// <summary>
        /// Adds vertices (if not already in the graph) and the edge between them.
        /// (If the graph is undirected, the choice of which vertex to call source and dest is arbitrary.)
        /// </summary>
        void Add(V source, V dest, E data);
        
        /// <summary>
        /// For adding a zero degree vertex
        /// </summary>
        bool AddVertex(V v);



        bool RemoveEdges(V source, V dest);

        bool RemoveEdge(V source, V dest, E data);

        /// <summary>
        /// Remove a vertex (and its edges) from the graph.
        /// </summary>
        /// <returns>true if successfully removes the node</returns>
        bool RemoveVertex(V vertex);

        bool RemoveVertices(ICollection<V> vertices);

        int GetNumVertices();

        /// <summary>
        /// For undirected graph, it is just the edges from the node
        /// </summary>
        List<E> GetOutgoingEdges(V v);

        /// <summary>
        /// For undirected graph, it is just the edges from the node
        /// </summary>
        List<E> GetIncomingEdges(V v);

        int GetNumEdges();

        /// <summary>
        /// For undirected graph, it is just the neighbors
        /// </summary>
        ReadOnlyCollection<V> GetParents(V vertex);
        
        /// <summary>
        /// For undirected graph, it is just the neighbors
        /// </summary>
        ReadOnlyCollection<V> GetChildren(V vertex);

        Set<V> GetNeighbors(V v);

        /// <summary>
        /// Clears the graph, removes all edges and nodes
        /// </summary>
        void Clear();

        bool ContainsVertex(V v);

        /// <summary>
        /// Only checks if there is an edge from source to dest.
        /// To check if it is connected in either direction, use isNeighbor
        /// </summary>
        bool IsEdge(V source, V dest);

        bool IsNeighbor(V source, V dest);

        ReadOnlyCollection<V> GetAllVertices();

        List<E> GetAllEdges();

        /// <summary>
        /// False if there are any vertices in the graph, true otherwise.
        /// Does not care about the number of edges.
        /// </summary>
        bool IsEmpty();

        /// <summary>
        /// Deletes nodes with zero incoming and zero outgoing edges
        /// </summary>
        void RemoveZeroDegreeNodes();

        ReadOnlyCollection<E> GetEdges(V source, V dest);

        /// <summary>
        /// For undirected graph, it should just be the degree
        /// </summary>
        int GetInDegree(V vertex);

        int GetOutDegree(V vertex);

        List<Set<V>> GetConnectedComponents();
    }
}