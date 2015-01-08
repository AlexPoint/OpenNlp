using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A {@code LabeledScoredTreeNode} represents a tree composed of a root
    /// label, a score, and an array of daughter parse trees.  A parse tree derived from a rule
    /// provides information about the category of the root as well as a composite of the daughter categories.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class LabeledScoredTreeNode : Tree
    {
        /// <summary>
        /// Label of the parse tree.
        /// </summary>
        private ILabel _label; // = null;

        /// <summary>
        /// Score of <code>TreeNode</code>
        /// </summary>
        private double _score = double.NaN;

        /// <summary>
        /// Daughters of the parse tree.
        /// </summary>
        private Tree[] _daughterTrees; // = null;

        /// <summary>
        /// Create an empty parse tree.
        /// </summary>
        public LabeledScoredTreeNode()
        {
            SetChildren(EmptyTreeArray);
        }

        /// <summary>
        /// Create a leaf parse tree with given word.
        /// </summary>
        /// <param name="label">
        /// the <code>Label</code> representing the <i>word</i> for this new tree leaf.
        /// </param>
        public LabeledScoredTreeNode(ILabel label) : this(label, Double.NaN)
        {
        }

        /// <summary>
        /// Create a leaf parse tree with given word and score.
        /// </summary>
        /// <param name="label">The <code>Label</code> representing the <i>word</i> for</param>
        /// <param name="score">The score for the node this new tree leaf.</param>
        public LabeledScoredTreeNode(ILabel label, double score) : this()
        {
            this._label = label;
            this._score = score;
        }

        /// <summary>
        /// Create parse tree with given root and array of daughter trees.
        /// </summary>
        /// <param name="label">root label of tree to construct.</param>
        /// <param name="daughterTreesList">List of daughter trees to construct.</param>
        public LabeledScoredTreeNode(ILabel label, List<Tree> daughterTreesList)
        {
            this._label = label;
            SetChildren(daughterTreesList);
        }

        /// <summary>
        /// Returns an array of children for the current node, or null if it is a leaf.
        /// </summary>
        public override Tree[] Children()
        {
            return _daughterTrees;
        }

        /// <summary>
        /// Sets the children of this <code>Tree</code>.
        /// If given <code>null</code>, this method sets the Tree's children to
        /// the canonical zero-length Tree[] array.
        /// </summary>
        /// <param name="children">An array of child trees</param>
        public override void SetChildren(Tree[] children)
        {
            if (children == null)
            {
                _daughterTrees = EmptyTreeArray;
            }
            else
            {
                _daughterTrees = children;
            }
        }

        /// <summary>
        /// Returns the label associated with the current node, or null if there is no label
        /// </summary>
        public override ILabel Label()
        {
            return _label;
        }

        /// <summary>
        /// Sets the label associated with the current node, if there is one.
        /// </summary>
        public override void SetLabel(ILabel label)
        {
            this._label = label;
        }

        /// <summary>
        /// Returns the score associated with the current node, or Nan if there is no score
        /// </summary>
        public override double Score()
        {
            return _score;
        }

        /// <summary>
        /// Sets the score associated with the current node, if there is one
        /// </summary>
        public override void SetScore(double score)
        {
            this._score = score;
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of the
        /// same type as the current <code>Tree</code>.  That is, this
        /// implementation, will produce trees of type
        /// <code>LabeledScoredTree(Node|Leaf)</code>.
        /// The <code>Label</code> of <code>this</code>
        /// is examined, and providing it is not <code>null</code>, a
        /// <code>LabelFactory</code> which will produce that kind of
        /// <code>Label</code> is supplied to the <code>TreeFactory</code>.
        /// If the <code>Label</code> is <code>null</code>, a
        /// <code>StringLabelFactory</code> will be used.
        /// The factories returned on different calls a different: a new one is
        /// allocated each time.
        /// </summary>
        public override ITreeFactory TreeFactory()
        {
            ILabelFactory lf = (Label() == null) ? CoreLabel.Factory() : Label().LabelFactory();
            return new LabeledScoredTreeFactory(lf);
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class TreeFactoryHolder
        {
            public static readonly ITreeFactory tf = new LabeledScoredTreeFactory();
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of the
        /// <code>LabeledScoredTree{Node|Leaf}</code> type.
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>a factory to produce labeled, scored trees</returns>
        public static ITreeFactory Factory()
        {
            return TreeFactoryHolder.tf;
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of the
        /// <code>LabeledScoredTree{Node|Leaf}</code> type, with
        /// the <code>Label</code> made with the supplied <code>LabelFactory</code>.
        /// The factory returned is a different one each time
        /// </summary>
        /// <param name="lf">The LabelFactory to use</param>
        /// <returns>a factory to produce labeled, scored trees</returns>
        public static ITreeFactory Factory(ILabelFactory lf)
        {
            return new LabeledScoredTreeFactory(lf);
        }

        private const string Nf = "#.###";

        public override string NodeString()
        {
            var buff = new StringBuilder();
            buff.Append(base.NodeString());
            if (! double.IsNaN(_score))
            {
                buff.Append(" [").Append((-_score).ToString(Nf)).Append("]");
            }
            return buff.ToString();
        }
    }
}