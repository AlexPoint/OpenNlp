using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A class for tree normalization.  The default one does no normalization.
    /// Other tree normalizers will change various node labels, or perhaps the
    /// whole tree geometry (by doing such things as deleting functional tags or
    /// empty elements).  Another operation that a <code>TreeNormalizer</code>
    /// may wish to perform is interning the <code>string</code>s passed to
    /// it.  Can be reused as a Singleton.  Designed to be extended.
    /// 
    /// The <code>TreeNormalizer</code> methods are in two groups.
    /// The contract for this class is that first normalizeTerminal or
    /// normalizeNonterminal will be called on each <code>string</code> that will
    /// be put into a <code>Tree</code>, when they are read from files or
    /// otherwise created.  Then <code>normalizeWholeTree</code> will
    /// be called on the <code>Tree</code>.  It normally walks the
    /// <code>Tree</code> making whatever modifications it wishes to. A
    /// <code>TreeNormalizer</code> need not make a deep copy of a
    /// <code>Tree</code>.  It is assumed to be able to work destructively,
    /// because afterwards we will only use the normalized <code>Tree</code>.
    /// 
    /// <i>Implementation note:</i> This is a very old legacy class used in conjunction
    /// with PennTreeReader.  It seems now that it would be better to move the
    /// string normalization into the tokenizer, and then we are just left with a
    /// (possibly destructive) TreeTransformer.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TreeNormalizer
    {
        /// <summary>
        /// Normalizes a leaf contents (and maybe intern it).
        /// </summary>
        /// <param name="leaf">The string that decorates the leaf</param>
        /// <returns>The normalized form of this leaf String</returns>
        public virtual string NormalizeTerminal(string leaf)
        {
            return leaf;
        }

        /// <summary>
        /// Normalizes a nonterminal contents (and maybe intern it)
        /// </summary>
        /// <param name="category">The string that decorates this nonterminal node</param>
        /// <returns>The normalized form of this nonterminal String</returns>
        public virtual string NormalizeNonterminal(string category)
        {
            return category;
        }

        /// <summary>
        /// Normalize a whole tree -- this method assumes that the argument
        /// that it is passed is the root of a complete <code>Tree</code>.
        /// It is normally implemented as a Tree-walking routine.
        /// This method may return <code>null</code>. This is interpreted to
        /// mean that this is a tree that should not be included in further
        /// processing.  PennTreeReader recognizes this return value, and
        /// asks for another Tree from the input Reader.
        /// </summary>
        /// <param name="tree">The tree to be normalized</param>
        /// <param name="tf">the TreeFactory to create new nodes (if needed)</param>
        /// <returns>the normalized tree</returns>
        public virtual Tree NormalizeWholeTree(Tree tree, ITreeFactory tf)
        {
            return tree;
        }

    }
}