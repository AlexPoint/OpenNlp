using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
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
