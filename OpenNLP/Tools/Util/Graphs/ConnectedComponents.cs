using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Graphs
{
    /**
 * Finds connected components in the graph, currently uses inefficient list for
 * variable 'verticesLeft'. It might give a problem for big graphs
 *
 * @author sonalg 08/08/11
 */
    public class ConnectedComponents
    {
        public static /*<V, E>*/ List<Set<V>> getConnectedComponents<V, E>(Graph<V, E> graph) {
    List<Set<V>> ccs = new List<Set<V>>();
    List<V> todo = new List<V>();
    // TODO: why not a set?
    List<V> verticesLeft = graph.getAllVertices().ToList();
    while (verticesLeft.Count > 0) {
      todo.Add(verticesLeft[0]);
      verticesLeft.RemoveAt(0);
      ccs.Add(bfs(todo, graph, verticesLeft));
    }
    return ccs;
  }

  private static /*<V, E>*/ Set<V> bfs<V, E>(List<V> todo, Graph<V, E> graph, List<V> verticesLeft) {
    Set<V> cc = new HashSet<V>();
    while (todo.Count > 0)
    {
        V node = todo.First();
        todo.RemoveAt(0);
      cc.Add(node);
      foreach (V neighbor in graph.getNeighbors(node)) {
        if (verticesLeft.Contains(neighbor)) {
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
