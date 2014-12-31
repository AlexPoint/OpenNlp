using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * A <code>TreeReader</code> adds functionality to another <code>Reader</code>
 * by reading in Trees, or some descendant class.
 *
 * @author Christopher Manning
 * @author Roger Levy (mod. 2003/01)
 * @version 2003/01
 */
    public interface TreeReader
    {
        /**
   * Reads a single tree.
   *
   * @return A single tree, or <code>null</code> at end of file.
   * @throws java.io.IOException If I/O problem
   */
  Tree readTree()/* throws IOException*/;


  /**
   * Close the Reader behind this <code>TreeReader</code>.
   */
  //@Override
  void close()/* throws IOException*/;
    }
}
