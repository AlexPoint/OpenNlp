using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A <code>TreeReaderFactory</code> is a factory for creating objects of
    /// class <code>TreeReader</code>, or some descendant class.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ITreeReaderFactory
    {
        /// <summary>
        /// Create a new <code>TreeReader</code> using the provided <code>Reader</code>
        /// </summary>
        /// <param name="reader">The <code>Reader</code> to build on</param>
        /// <returns>The new TreeReader</returns>
        ITreeReader NewTreeReader(TextReader reader);
    }
}