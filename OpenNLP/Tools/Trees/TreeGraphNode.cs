using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A <code>TreeGraphNode</code> is simply a
    /// {@link Tree <code>Tree</code>}
    /// with some additional functionality.  For example, the
    /// <code>parent()</code> method works without searching from the root.
    /// Labels are always assumed to be
    /// {@link CoreLabel <code>CoreLabel</code>}
    /// 
    /// This class makes the horrible mistake of changing the semantics of
    /// equals and hashCode to go back to "==" and System.identityHashCode,
    /// despite the semantics of the superclass's equality.
    /// 
    /// @author Bill MacCartney
    /// </summary>
    public class TreeGraphNode : Tree, IHasParent
    {
        /// <summary>
        /// Label for this node
        /// </summary>
        protected CoreLabel _label;

        /// <summary>
        /// Parent of this node
        /// </summary>
        protected TreeGraphNode _parent; // = null;

        /// <summary>
        /// Children of this node
        /// </summary>
        protected TreeGraphNode[] _children = ZeroTgnChildren;

        /// <summary>
        /// The {@link GrammaticalStructure <code>GrammaticalStructure</code>} of which this node is part
        /// </summary>
        protected GrammaticalStructure tg;

        /// <summary>
        /// A leaf node should have a zero-length array for its children. 
        /// For efficiency, subclasses can use this array as a return value 
        /// for children() for leaf nodes if desired. Should this be public instead?
        /// </summary>
        protected static readonly TreeGraphNode[] ZeroTgnChildren = new TreeGraphNode[0];

        private static readonly ILabelFactory Mlf = CoreLabel.Factory();

        /// <summary>
        /// Create a new empty <code>TreeGraphNode</code>
        /// </summary>
        public TreeGraphNode()
        {
        }

        /// <summary>
        /// Create a new <code>TreeGraphNode</code> with the supplied label
        /// </summary>
        /// <param name="label">the label for this node</param>
        public TreeGraphNode(ILabel label)
        {
            this._label = (CoreLabel) Mlf.NewLabel(label);
        }

        /// <summary>
        /// Create a new <code>TreeGraphNode</code> with the supplied
        /// label and list of child nodes
        /// </summary>
        /// <param name="label">the label for this node</param>
        /// <param name="children">the list of child <code>TreeGraphNode</code>s for this node</param>
        public TreeGraphNode(ILabel label, List<Tree> children) :
            this(label)
        {
            SetChildren(children);
        }

        /// <summary>
        /// Create a new <code>TreeGraphNode</code> having the same tree
        /// structure and label values as an existing tree (but no shared storage)
        /// </summary>
        /// <param name="t">the tree to copy</param>
        /// <param name="graph">the graph of which this node is a part</param>
        public TreeGraphNode(Tree t, GrammaticalStructure graph) :
            this(t, (TreeGraphNode) null)
        {
            this.SetTreeGraph(graph);
        }

        // TODO it's not really clear what graph the copy should be a part of
        public TreeGraphNode(TreeGraphNode t) :
            this(t, t._parent)
        {
            this.SetTreeGraph(t.TreeGraph());
        }

        /// <summary>
        /// Create a new <code>TreeGraphNode</code> having the same tree structure 
        /// and label values as an existing tree (but no shared storage).
        /// Operates recursively to construct an entire subtree
        /// </summary>
        /// <param name="t">the tree to copy</param>
        /// <param name="parent">the parent node</param>
        protected TreeGraphNode(Tree t, TreeGraphNode parent)
        {
            this._parent = parent;
            Tree[] tKids = t.Children();
            int numKids = tKids.Length;
            _children = new TreeGraphNode[numKids];
            for (int i = 0; i < numKids; i++)
            {
                _children[i] = new TreeGraphNode(tKids[i], this);
                if (t.IsPreTerminal())
                {
                    // add the tags to the leaves
                    _children[i]._label.SetTag(t.Label().Value());
                }
            }
            this._label = (CoreLabel) Mlf.NewLabel(t.Label());
        }

        // TODO: This should be changed via introducing a Tree interface with the current Tree and this class implementing it, since what is done here breaks the equals() contract.
        /// <summary>
        /// Implements equality for <code>TreeGraphNode</code>s.
        /// Unlike <code>Tree</code>s, <code>TreeGraphNode</code>s should be
        /// considered equal only if they are ==.  <i>Implementation note:</i>
        /// </summary>
        public override bool Equals(Object o)
        {
            return o == this;
        }

        public override int GetHashCode() {
            return RuntimeHelpers.GetHashCode(this);
          }

        /// <summary>
        /// Returns the label associated with the current node, or null if there is no label.
        /// </summary>
        public override /*CoreLabel */ ILabel Label()
        {
            return _label;
        }

        /// <summary>
        /// Sets the label associated with the current node
        /// </summary>
        public void SetLabel(CoreLabel label)
        {
            this._label = label;
        }

        /// <summary>
        /// Get the index for the current node.
        /// </summary>
        public int Index()
        {
            return _label.Index();
        }

        /// <summary>
        /// Set the index for the current node
        /// </summary>
        public void SetIndex(int index)
        {
            _label.SetIndex(index);
        }

        /// <summary>
        /// Get the parent for the current node
        /// </summary>
        public override /*TreeGraphNode*/ Tree Parent()
        {
            return _parent;
        }

        /// <summary>
        /// Set the parent for the current node
        /// </summary>
        public void SetParent(TreeGraphNode parent)
        {
            this._parent = parent;
        }

        /// <summary>
        /// Returns an array of the children of this node
        /// </summary>
        public override Tree[] Children()
        {
            return _children;
        }

        /// <summary>
        /// Sets the children of this <code>TreeGraphNode</code>.
        /// If given <code>null</code>, this method sets
        /// the node's children to the canonical zero-length Tree[] array.
        /// </summary>
        /// <param name="children">an array of child trees</param>
        public override void SetChildren(Tree[] children)
        {
            if (children == null || children.Length == 0)
            {
                this._children = ZeroTgnChildren;
            }
            else
            {
                if (children is TreeGraphNode[])
                {
                    this._children = (TreeGraphNode[]) children;
                }
                else
                {
                    this._children = new TreeGraphNode[children.Length];
                    for (int i = 0; i < children.Length; i++)
                    {
                        this._children[i] = (TreeGraphNode) children[i];
                    }
                }
            }
        }

        public void SetChildren(List<TreeGraphNode> childTreesList)
        {
            if (childTreesList == null || !childTreesList.Any())
            {
                SetChildren(ZeroTgnChildren);
            }
            else
            {
                TreeGraphNode[] childTrees = childTreesList.ToArray();
                SetChildren(childTrees);
            }
        }

        /// <summary>
        /// Get the <code>GrammaticalStructure</code> of which this node is a part
        /// </summary>
        protected GrammaticalStructure TreeGraph()
        {
            return tg;
        }

        /// <summary>
        /// Set pointer to the <code>GrammaticalStructure</code> of which this node
        /// is a part.  Operates recursively to set pointer for all descendants too
        /// </summary>
        protected void SetTreeGraph(GrammaticalStructure tg)
        {
            this.tg = tg;
            foreach (TreeGraphNode child in _children)
            {
                child.SetTreeGraph(tg);
            }
        }

        /// <summary>
        /// Uses the specified {@link HeadFinder <code>HeadFinder</code>}
        /// to determine the heads for this node and all its descendants,
        /// and to store references to the head word node and head tag node
        /// in this node's {@link CoreLabel <code>CoreLabel</code>} and the
        /// <code>CoreLabel</code>s of all its descendants.
        /// 
        /// Note that, in contrast to {@link Tree#percolateHeads
        /// <code>Tree.percolateHeads()</code>}, which assumes {@link
        /// edu.stanford.nlp.ling.CategoryWordTag
        /// <code>CategoryWordTag</code>} labels and therefore stores head
        /// words and head tags merely as <code>string</code>s, this
        /// method stores references to the actual nodes.  This mitigates
        /// potential problems in sentences which contain the same word more than once.
        /// </summary>
        /// <param name="hf">The headfinding algorithm to use</param>
        public override void PercolateHeads(IHeadFinder hf)
        {
            if (IsLeaf())
            {
                TreeGraphNode hwn = HeadWordNode();
                if (hwn == null)
                {
                    SetHeadWordNode(this);
                }
            }
            else
            {
                foreach (Tree child in Children())
                {
                    child.PercolateHeads(hf);
                }
                TreeGraphNode head = SafeCast(hf.DetermineHead(this, _parent));
                if (head != null)
                {

                    TreeGraphNode hwn = head.HeadWordNode();
                    if (hwn == null && head.IsLeaf())
                    {
                        // below us is a leaf
                        SetHeadWordNode(head);
                    }
                    else
                    {
                        SetHeadWordNode(hwn);
                    }

                    TreeGraphNode htn = head.HeadTagNode();
                    if (htn == null && head.IsLeaf())
                    {
                        // below us is a leaf
                        SetHeadTagNode(this);
                    }
                    else
                    {
                        SetHeadTagNode(htn);
                    }

                }
            }
        }

        /// <summary>
        /// Return the node containing the head word for this node (or
        /// <code>null</code> if none), as recorded in this node's {@link
        /// CoreLabel <code>CoreLabel</code>}.  (In contrast to {@link
        /// edu.stanford.nlp.ling.CategoryWordTag
        /// <code>CategoryWordTag</code>}, we store head words and head
        /// tags as references to nodes, not merely as <code>string</code>s.)
        /// </summary>
        /// <returns>the node containing the head word for this node</returns>
        public TreeGraphNode HeadWordNode()
        {
            TreeGraphNode hwn = SafeCast(_label.Get(typeof (TreeCoreAnnotations.HeadWordAnnotation)));
            if (hwn == null || (hwn.TreeGraph() != null && !(hwn.TreeGraph().Equals(this.TreeGraph()))))
            {
                return null;
            }
            return hwn;
        }

        /// <summary>
        /// Store the node containing the head word for this node by 
        /// storing it in this node's {@link CoreLabel
        /// <code>CoreLabel</code>}.  (In contrast to {@link
        /// edu.stanford.nlp.ling.CategoryWordTag
        /// <code>CategoryWordTag</code>}, we store head words and head
        /// tags as references to nodes, not merely as <code>string</code>s.)
        /// </summary>
        /// <param name="hwn">the node containing the head word for this node</param>
        private void SetHeadWordNode(TreeGraphNode hwn)
        {
            _label.Set(typeof (TreeCoreAnnotations.HeadWordAnnotation), hwn);
        }

        /// <summary>
        /// Return the node containing the head tag for this node (or
        /// <code>null</code> if none), as recorded in this node's {@link
        /// CoreLabel <code>CoreLabel</code>}.  (In contrast to {@link
        /// edu.stanford.nlp.ling.CategoryWordTag
        /// <code>CategoryWordTag</code>}, we store head words and head
        /// tags as references to nodes, not merely as <code>string</code>s.)
        /// </summary>
        /// <returns>the node containing the head tag for this node</returns>
        public TreeGraphNode HeadTagNode()
        {
            TreeGraphNode htn = SafeCast(_label.Get(typeof (TreeCoreAnnotations.HeadTagAnnotation)));
            if (htn == null || (htn.TreeGraph() != null && !(htn.TreeGraph().Equals(this.TreeGraph()))))
            {
                return null;
            }
            return htn;
        }

        /// <summary>
        /// Store the node containing the head tag for this node by
        /// storing it in this node's {@link CoreLabel <code>CoreLabel</code>}.
        /// (In contrast to {@link edu.stanford.nlp.ling.CategoryWordTag
        /// <code>CategoryWordTag</code>}, we store head words and head
        /// tags as references to nodes, not merely as
        /// <code>string</code>s.)
        /// </summary>
        /// <param name="htn">the node containing the head tag for this node</param>
        private void SetHeadTagNode(TreeGraphNode htn)
        {
            _label.Set(typeof (TreeCoreAnnotations.HeadTagAnnotation), htn);
        }

        /// <summary>
        /// Safely casts an <code>Object</code> to a <code>TreeGraphNode</code>
        /// if possible, else returns <code>null</code>
        /// </summary>
        /// <param name="t">any <code>Object</code></param>
        /// <returns>
        /// <code>t</code> if it is a <code>TreeGraphNode</code>;<code>null</code> otherwise
        /// </returns>
        private static TreeGraphNode SafeCast(Object t)
        {
            if (t == null || !(t is TreeGraphNode))
            {
                return null;
            }
            return (TreeGraphNode) t;
        }

        /// <summary>
        /// Checks the node's ancestors to find the highest ancestor with the
        /// same <code>headWordNode</code> as this node
        /// </summary>
        public TreeGraphNode HighestNodeWithSameHead()
        {
            TreeGraphNode node = this;
            while (true)
            {
                TreeGraphNode parent = SafeCast(node.Parent());
                if (parent == null || parent.HeadWordNode() != node.HeadWordNode())
                {
                    return node;
                }
                node = parent;
            }
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class TreeFactoryHolder
        {
            public static readonly TreeGraphNodeFactory tgnf = new TreeGraphNodeFactory();
        }
        
        /// <summary>
        /// Returns a <code>TreeFactory</code> that produces 
        /// <code>TreeGraphNode</code>s.  The <code>Label</code> of
        /// <code>this</code> is examined, and providing it is not
        /// <code>null</code>, a <code>LabelFactory</code> which will
        /// produce that kind of <code>Label</code> is supplied to the
        /// <code>TreeFactory</code>.  If the <code>Label</code> is
        /// <code>null</code>, a <code>CoreLabel.factory()</code> will be used.  
        /// The factories returned on different calls are different: a new one is
        /// allocated each time.
        /// </summary>
        /// <returns>a factory to produce treegraphs</returns>
        public override ITreeFactory TreeFactory()
        {
            ILabelFactory lf;
            if (Label() != null)
            {
                lf = Label().LabelFactory();
            }
            else
            {
                lf = CoreLabel.Factory();
            }
            return new TreeGraphNodeFactory(lf);
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of type <code>TreeGraphNode</code>.
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>a factory to produce treegraphs</returns>
        public static ITreeFactory Factory()
        {
            return TreeFactoryHolder.tgnf;
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of type
        /// <code>TreeGraphNode</code>, with the <code>Label</code> made
        /// by the supplied <code>LabelFactory</code>.  The factory
        /// returned is a different one each time.
        /// </summary>
        /// <param name="lf">The <code>LabelFactory</code> to use</param>
        /// <returns>a factory to produce treegraphs</returns>
        public static ITreeFactory Factory(ILabelFactory lf)
        {
            return new TreeGraphNodeFactory(lf);
        }

        /// <summary>
        /// Returns a <code>string</code> representation of this node and
        /// its subtree with one node per line, indented according to <code>indentLevel</code>.
        /// </summary>
        /// <param name="indentLevel">how many levels to indent (0 for root node)</param>
        /// <returns><code>string</code> representation of this subtree</returns>
        public string ToPrettyString(int indentLevel)
        {
            var buf = new StringBuilder("\n");
            for (int i = 0; i < indentLevel; i++)
            {
                buf.Append("  ");
            }
            if (_children == null || _children.Length == 0)
            {
                buf.Append(_label.ToString(CoreLabel.OutputFormat.VALUE_INDEX_MAP));
            }
            else
            {
                buf.Append('(').Append(_label.ToString(CoreLabel.OutputFormat.VALUE_INDEX_MAP));
                foreach (TreeGraphNode child in _children)
                {
                    buf.Append(' ').Append(child.ToPrettyString(indentLevel + 1));
                }
                buf.Append(')');
            }
            return buf.ToString();
        }

        /// <summary>
        /// Returns a <code>string</code> representation of this node and
        /// its subtree as a one-line parenthesized list 
        /// </summary>
        /// <returns><code>string</code> representation of this subtree</returns>
        public string ToOneLineString()
        {
            var buf = new StringBuilder();
            if (_children == null || _children.Length == 0)
            {
                buf.Append(_label);
            }
            else
            {
                buf.Append('(').Append(_label);
                foreach (TreeGraphNode child in _children)
                {
                    buf.Append(' ').Append(child.ToOneLineString());
                }
                buf.Append(')');
            }
            return buf.ToString();
        }

        public string ToPrimes()
        {
            var coreLabel = Label() as CoreLabel;
            if (coreLabel != null)
            {
                int copy = coreLabel.CopyCount();
                return StringUtils.Repeat('\'', copy);
            }
            else
            {
                return "";
            }
        }

        public override string ToString()
        {
            return _label.ToString();
        }

        public string ToString(CoreLabel.OutputFormat format)
        {
            return _label.ToString(format);
        }
        
    }
}