using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A <code>SimpleTree</code> is a minimal concrete implementation of an
    /// unlabeled, unscored <code>Tree</code>.  It has a tree structure, but
    /// nothing is stored at a node (no label or score).
    /// So, most of the time, this is the wrong class to use!
    /// Look at {@code LabeledScoredTreeNode}.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class SimpleTree : Tree
    {
        /// <summary>Daughters of the parse tree</summary>
        private Tree[] daughterTrees;

        /// <summary>
        /// Create an empty parse tree
        /// </summary>
        public SimpleTree()
        {
            daughterTrees = EmptyTreeArray;
        }

        /// <summary>
        /// Create parse tree with given root and null daughters
        /// </summary>
        /// <param name="label">root label of new tree to construct.
        /// For a SimpleTree this parameter is ignored</param>
        public SimpleTree(ILabel label) : this()
        {
        }

        /// <summary>
        /// Create parse tree with given root and array of daughter trees
        /// </summary>
        /// <param name="label">
        /// root label of tree to construct.  For a SimpleTree this parameter is ignored
        /// </param>
        /// <param name="daughterTreesList">list of daughter trees to construct</param>
        public SimpleTree(ILabel label, List<Tree> daughterTreesList)
        {
            SetChildren(daughterTreesList.ToArray());
        }

        /// <summary>
        /// Returns an array of children for the current node, or null if it is a leaf.
        /// </summary>
        public override Tree[] Children()
        {
            return daughterTrees;
        }

        /// <summary>
        /// Sets the children of this <code>Tree</code>.
        /// If given <code>null</code>, this method sets the Tree's children to a
        /// unique zero-length Tree[] array.
        /// </summary>
        /// <param name="children">An array of child trees</param>
        public override void SetChildren(Tree[] children)
        {
            if (children == null)
            {
                daughterTrees = EmptyTreeArray;
            }
            else
            {
                daughterTrees = children;
            }
        }


        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class TreeFactoryHolder
        {
            public static readonly ITreeFactory tf = new SimpleTreeFactory();
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of the
        /// <code>SimpleTree</code> type.
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        public override ITreeFactory TreeFactory()
        {
            return TreeFactoryHolder.tf;
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of the
        /// <code>SimpleTree</code> type.
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        public static ITreeFactory Factory()
        {
            return TreeFactoryHolder.tf;
        }
    }
}