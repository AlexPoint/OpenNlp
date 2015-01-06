using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * A <code>TreeReaderFactory</code> is a factory for creating objects of
 * class <code>TreeReader</code>, or some descendant class.
 *
 * @author Christopher Manning
 */

    public interface TreeReaderFactory
    {
        /**
   * Create a new <code>TreeReader</code> using the provided
   * <code>Reader</code>.
   *
   * @param in The <code>Reader</code> to build on
   * @return The new TreeReader
   */
        TreeReader NewTreeReader(TextReader reader);
    }
}