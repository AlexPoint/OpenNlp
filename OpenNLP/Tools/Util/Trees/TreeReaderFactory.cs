using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A <code>TreeReaderFactory</code> is a factory for creating objects of
    /// class <code>TreeReader</code>, or some descendant class.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public interface TreeReaderFactory
    {
        /// <summary>
        /// Create a new <code>TreeReader</code> using the provided <code>Reader</code>
        /// </summary>
        /// <param name="reader">The <code>Reader</code> to build on</param>
        /// <returns>The new TreeReader</returns>
        TreeReader NewTreeReader(TextReader reader);
    }
}