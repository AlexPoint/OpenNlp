using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// An interface for finding the "head" daughter of a phrase structure tree.
    /// This could potentially be any sense of "head", but has mainly been used
    /// to find the lexical head for lexicalized PCFG parsing.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IHeadFinder
    {
        /// <summary>
        /// Determine which daughter of the current parse tree is the head.
        /// </summary>
        /// <param name="t">The parse tree to examine the daughters of</param>
        /// <returns>
        /// The daughter tree that is the head. This will always be non-null.
        /// An Exception will be thrown if no head can be determined.
        /// </returns>
        Tree DetermineHead(Tree t);

        /// <summary>
        /// Determine which daughter of the current parse tree is the head
        /// given the parent of the tree.
        /// </summary>
        /// <param name="t">The parse tree to examine the daughters of</param>
        /// <param name="parent">The parent of tree t</param>
        /// <returns>
        /// The daughter tree that is the head. This will always be non-null. 
        /// An Exception will be thrown if no head can be determined.
        /// </returns>
        Tree DetermineHead(Tree t, Tree parent);
    }
}