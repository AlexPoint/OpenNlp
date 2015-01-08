using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A general factory for {@link GrammaticalStructure} objects.
    /// 
    /// @author Galen Andrew
    /// @author John Bauer
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IGrammaticalStructureFactory
    {
        /// <summary>
        /// Vend a new {@link GrammaticalStructure} based on the given {@link Tree}.
        /// </summary>
        /// <param name="t">the tree to analyze</param>
        /// <returns>a GrammaticalStructure based on the tree</returns>
        GrammaticalStructure NewGrammaticalStructure(Tree t);
    }
}