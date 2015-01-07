using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Graphs
{
    /// <summary>
    /// Finds connected components in the graph, currently uses inefficient list for
    /// variable 'verticesLeft'. It might give a problem for big graphs
    /// 
    /// @author sonalg 08/08/11
    /// 
    /// Code...
    /// </summary>
    public class ConnectedComponents
    {
        public static /*<V, E>*/ List<Set<V>> GetConnectedComponents<V, E>(Graph<V, E> graph)
        {
            var ccs = new List<Set<V>>();
            var todo = new List<V>();
            // TODO: why not a set?
            List<V> verticesLeft = graph.GetAllVertices().ToList();
            while (verticesLeft.Count > 0)
            {
                todo.Add(verticesLeft[0]);
                verticesLeft.RemoveAt(0);
                ccs.Add(Bfs(todo, graph, verticesLeft));
            }
            return ccs;
        }

        private static /*<V, E>*/ Set<V> Bfs<V, E>(List<V> todo, Graph<V, E> graph, List<V> verticesLeft)
        {
            Set<V> cc = new HashSet<V>();
            while (todo.Count > 0)
            {
                V node = todo.First();
                todo.RemoveAt(0);
                cc.Add(node);
                foreach (V neighbor in graph.GetNeighbors(node))
                {
                    if (verticesLeft.Contains(neighbor))
                    {
                        cc.Add(neighbor);
                        todo.Add(neighbor);
                        verticesLeft.Remove(neighbor);
                    }
                }
            }

            return cc;
        }
    }
}