using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * <p>
 * A <code>TreeGraphNode</code> is simply a
 * {@link Tree <code>Tree</code>}
 * with some additional functionality.  For example, the
 * <code>parent()</code> method works without searching from the root.
 * Labels are always assumed to be
 * {@link CoreLabel <code>CoreLabel</code>}
 *
 * <p>This class makes the horrible mistake of changing the semantics of
 * equals and hashCode to go back to "==" and System.identityHashCode,
 * despite the semantics of the superclass's equality.</p>
 *
 * @author Bill MacCartney
 */

    public class TreeGraphNode : Tree, HasParent
    {
        /**
   * Label for this node.
   */
        protected CoreLabel plabel;

        /**
   * Parent of this node.
   */
        protected TreeGraphNode pparent; // = null;


        /**
   * Children of this node.
   */
        protected TreeGraphNode[] pchildren = ZeroTgnChildren;

        /**
   * The {@link GrammaticalStructure <code>GrammaticalStructure</code>} of which this
   * node is part.
   */
        protected GrammaticalStructure tg;

        /**
   * A leaf node should have a zero-length array for its
   * children. For efficiency, subclasses can use this array as a
   * return value for children() for leaf nodes if desired. Should
   * this be public instead?
   */
        protected static readonly TreeGraphNode[] ZeroTgnChildren = new TreeGraphNode[0];

        private static LabelFactory mlf = CoreLabel.Factory();

        /**
   * Create a new empty <code>TreeGraphNode</code>.
   */

        public TreeGraphNode()
        {
        }

        /**
   * Create a new <code>TreeGraphNode</code> with the supplied
   * label.
   *
   * @param label the label for this node.
   */

        public TreeGraphNode(Label label)
        {
            this.plabel = (CoreLabel) mlf.NewLabel(label);
        }

        /**
   * Create a new <code>TreeGraphNode</code> with the supplied
   * label and list of child nodes.
   *
   * @param label    the label for this node.
   * @param children the list of child <code>TreeGraphNode</code>s
   *                 for this node.
   */

        public TreeGraphNode(Label label, List<Tree> children) :
            this(label)
        {
            SetChildren(children);
        }

        /**
   * Create a new <code>TreeGraphNode</code> having the same tree
   * structure and label values as an existing tree (but no shared
   * storage).
   * @param t     the tree to copy
   * @param graph the graph of which this node is a part
   */

        public TreeGraphNode(Tree t, GrammaticalStructure graph) :
            this(t, (TreeGraphNode) null)
        {
            this.SetTreeGraph(graph);
        }

        // XXX TODO it's not really clear what graph the copy should be a part of
        public TreeGraphNode(TreeGraphNode t) :
            this(t, t.pparent)
        {
            this.SetTreeGraph(t.TreeGraph());
        }

        /**
   * Create a new <code>TreeGraphNode</code> having the same tree
   * structure and label values as an existing tree (but no shared
   * storage).  Operates recursively to construct an entire
   * subtree.
   *
   * @param t      the tree to copy
   * @param parent the parent node
   */

        protected TreeGraphNode(Tree t, TreeGraphNode parent)
        {
            this.pparent = parent;
            Tree[] tKids = t.Children();
            int numKids = tKids.Length;
            pchildren = new TreeGraphNode[numKids];
            for (int i = 0; i < numKids; i++)
            {
                pchildren[i] = new TreeGraphNode(tKids[i], this);
                if (t.IsPreTerminal())
                {
                    // add the tags to the leaves
                    pchildren[i].plabel.SetTag(t.Label().Value());
                }
            }
            this.plabel = (CoreLabel) mlf.NewLabel(t.Label());
        }

        /**
   * Implements equality for <code>TreeGraphNode</code>s.  Unlike
   * <code>Tree</code>s, <code>TreeGraphNode</code>s should be
   * considered equal only if they are ==.  <i>Implementation note:</i>
   * TODO: This should be changed via introducing a Tree interface with the current Tree and this class implementing it, since what is done here breaks the equals() contract.
   *
   * @param o The object to compare with
   * @return Whether two things are equal
   */
        //@Override
        public override bool Equals(Object o)
        {
            return o == this;
        }

        //@Override
        /*public override int GetHashCode() {
    return System.identityHashCode(this);
  }*/

        /**
   * Returns the label associated with the current node, or null
   * if there is no label.
   *
   * @return the label of the node
   */
        //@Override
        public override /*CoreLabel */ Label Label()
        {
            return plabel;
        }

        /**
   * Sets the label associated with the current node.
   *
   * @param label the new label to use.
   */

        public void SetLabel( /*readonly*/ CoreLabel label)
        {
            this.plabel = label;
        }

        /**
   * Get the index for the current node.
   */

        public int Index()
        {
            return plabel.Index();
        }

        /**
   * Set the index for the current node.
   */

        public void SetIndex(int index)
        {
            plabel.SetIndex(index);
        }

        /**
   * Get the parent for the current node.
   */
        //@Override
        public override /*TreeGraphNode*/ Tree Parent()
        {
            return pparent;
        }

        /**
   * Set the parent for the current node.
   */

        public void SetParent(TreeGraphNode parent)
        {
            this.pparent = parent;
        }

        /**
   * Returns an array of the children of this node.
   */
        //@Override
        public override Tree[] Children()
        {
            return pchildren;
        }

        /**
   * Sets the children of this <code>TreeGraphNode</code>.  If
   * given <code>null</code>, this method sets
   * the node's children to the canonical zero-length Tree[] array.
   *
   * @param children an array of child trees
   */
        //@Override
        public override void SetChildren(Tree[] children)
        {
            if (children == null || children.Length == 0)
            {
                this.pchildren = ZeroTgnChildren;
            }
            else
            {
                if (children is TreeGraphNode[])
                {
                    this.pchildren = (TreeGraphNode[]) children;
                }
                else
                {
                    this.pchildren = new TreeGraphNode[children.Length];
                    for (int i = 0; i < children.Length; i++)
                    {
                        this.pchildren[i] = (TreeGraphNode) children[i];
                    }
                }
            }
        }

        /** {@inheritDoc} */
        //@Override
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

        /**
   * Get the <code>GrammaticalStructure</code> of which this node is a
   * part.
   */

        protected GrammaticalStructure TreeGraph()
        {
            return tg;
        }

        /**
   * Set pointer to the <code>GrammaticalStructure</code> of which this node
   * is a part.  Operates recursively to set pointer for all
   * descendants too.
   */

        protected void SetTreeGraph(GrammaticalStructure tg)
        {
            this.tg = tg;
            foreach (TreeGraphNode child in pchildren)
            {
                child.SetTreeGraph(tg);
            }
        }

        /**
   * Uses the specified {@link HeadFinder <code>HeadFinder</code>}
   * to determine the heads for this node and all its descendants,
   * and to store references to the head word node and head tag node
   * in this node's {@link CoreLabel <code>CoreLabel</code>} and the
   * <code>CoreLabel</code>s of all its descendants.<p>
   * <p/>
   * Note that, in contrast to {@link Tree#percolateHeads
   * <code>Tree.percolateHeads()</code>}, which assumes {@link
   * edu.stanford.nlp.ling.CategoryWordTag
   * <code>CategoryWordTag</code>} labels and therefore stores head
   * words and head tags merely as <code>string</code>s, this
   * method stores references to the actual nodes.  This mitigates
   * potential problems in sentences which contain the same word
   * more than once.
   *
   * @param hf The headfinding algorithm to use
   */
        //@Override
        public override void PercolateHeads(HeadFinder hf)
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
                TreeGraphNode head = SafeCast(hf.DetermineHead(this, pparent));
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
                else
                {
                    //System.err.println("Head is null: " + this);
                }
            }
        }

        /**
   * Return the node containing the head word for this node (or
   * <code>null</code> if none), as recorded in this node's {@link
   * CoreLabel <code>CoreLabel</code>}.  (In contrast to {@link
   * edu.stanford.nlp.ling.CategoryWordTag
   * <code>CategoryWordTag</code>}, we store head words and head
   * tags as references to nodes, not merely as
   * <code>string</code>s.)
   *
   * @return the node containing the head word for this node
   */

        public TreeGraphNode HeadWordNode()
        {
            TreeGraphNode hwn = SafeCast(plabel.Get(typeof (TreeCoreAnnotations.HeadWordAnnotation)));
            if (hwn == null || (hwn.TreeGraph() != null && !(hwn.TreeGraph().Equals(this.TreeGraph()))))
            {
                return null;
            }
            return hwn;
        }

        /**
   * Store the node containing the head word for this node by
   * storing it in this node's {@link CoreLabel
   * <code>CoreLabel</code>}.  (In contrast to {@link
   * edu.stanford.nlp.ling.CategoryWordTag
   * <code>CategoryWordTag</code>}, we store head words and head
   * tags as references to nodes, not merely as
   * <code>string</code>s.)
   *
   * @param hwn the node containing the head word for this node
   */

        private void SetHeadWordNode( /*readonly*/ TreeGraphNode hwn)
        {
            plabel.Set(typeof (TreeCoreAnnotations.HeadWordAnnotation), hwn);
        }

        /**
   * Return the node containing the head tag for this node (or
   * <code>null</code> if none), as recorded in this node's {@link
   * CoreLabel <code>CoreLabel</code>}.  (In contrast to {@link
   * edu.stanford.nlp.ling.CategoryWordTag
   * <code>CategoryWordTag</code>}, we store head words and head
   * tags as references to nodes, not merely as
   * <code>string</code>s.)
   *
   * @return the node containing the head tag for this node
   */

        public TreeGraphNode HeadTagNode()
        {
            TreeGraphNode htn = SafeCast(plabel.Get(typeof (TreeCoreAnnotations.HeadTagAnnotation)));
            if (htn == null || (htn.TreeGraph() != null && !(htn.TreeGraph().Equals(this.TreeGraph()))))
            {
                return null;
            }
            return htn;
        }

        /**
   * Store the node containing the head tag for this node by
   * storing it in this node's {@link CoreLabel
   * <code>CoreLabel</code>}.  (In contrast to {@link
   * edu.stanford.nlp.ling.CategoryWordTag
   * <code>CategoryWordTag</code>}, we store head words and head
   * tags as references to nodes, not merely as
   * <code>string</code>s.)
   *
   * @param htn the node containing the head tag for this node
   */

        private void SetHeadTagNode( /*readonly*/ TreeGraphNode htn)
        {
            plabel.Set(typeof (TreeCoreAnnotations.HeadTagAnnotation), htn);
        }

        /**
   * Safely casts an <code>Object</code> to a
   * <code>TreeGraphNode</code> if possible, else returns
   * <code>null</code>.
   *
   * @param t any <code>Object</code>
   * @return <code>t</code> if it is a <code>TreeGraphNode</code>;
   *         <code>null</code> otherwise
   */

        private static TreeGraphNode SafeCast(Object t)
        {
            if (t == null || !(t is TreeGraphNode))
            {
                return null;
            }
            return (TreeGraphNode) t;
        }

        /**
   * Checks the node's ancestors to find the highest ancestor with the
   * same <code>headWordNode</code> as this node.
   */

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

            /*private TreeFactoryHolder() {
    }*/

        }

        /**
   * Returns a <code>TreeFactory</code> that produces
   * <code>TreeGraphNode</code>s.  The <code>Label</code> of
   * <code>this</code> is examined, and providing it is not
   * <code>null</code>, a <code>LabelFactory</code> which will
   * produce that kind of <code>Label</code> is supplied to the
   * <code>TreeFactory</code>.  If the <code>Label</code> is
   * <code>null</code>, a
   * <code>CoreLabel.factory()</code> will be used.  The factories
   * returned on different calls are different: a new one is
   * allocated each time.
   *
   * @return a factory to produce treegraphs
   */
        //@Override
        public override TreeFactory TreeFactory()
        {
            LabelFactory lf;
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

        /**
   * Return a <code>TreeFactory</code> that produces trees of type
   * <code>TreeGraphNode</code>.  The factory returned is always
   * the same one (a singleton).
   *
   * @return a factory to produce treegraphs
   */

        public static TreeFactory Factory()
        {
            return TreeFactoryHolder.tgnf;
        }

        /**
   * Return a <code>TreeFactory</code> that produces trees of type
   * <code>TreeGraphNode</code>, with the <code>Label</code> made
   * by the supplied <code>LabelFactory</code>.  The factory
   * returned is a different one each time.
   *
   * @param lf The <code>LabelFactory</code> to use
   * @return a factory to produce treegraphs
   */

        public static TreeFactory Factory(LabelFactory lf)
        {
            return new TreeGraphNodeFactory(lf);
        }

        /**
   * Returns a <code>string</code> representation of this node and
   * its subtree with one node per line, indented according to
   * <code>indentLevel</code>.
   *
   * @param indentLevel how many levels to indent (0 for root node)
   * @return <code>string</code> representation of this subtree
   */

        public string ToPrettyString(int indentLevel)
        {
            var buf = new StringBuilder("\n");
            for (int i = 0; i < indentLevel; i++)
            {
                buf.Append("  ");
            }
            if (pchildren == null || pchildren.Length == 0)
            {
                buf.Append(plabel.ToString(CoreLabel.OutputFormat.VALUE_INDEX_MAP));
            }
            else
            {
                buf.Append('(').Append(plabel.ToString(CoreLabel.OutputFormat.VALUE_INDEX_MAP));
                foreach (TreeGraphNode child in pchildren)
                {
                    buf.Append(' ').Append(child.ToPrettyString(indentLevel + 1));
                }
                buf.Append(')');
            }
            return buf.ToString();
        }

        /**
   * Returns a <code>string</code> representation of this node and
   * its subtree as a one-line parenthesized list.
   *
   * @return <code>string</code> representation of this subtree
   */

        public string ToOneLineString()
        {
            var buf = new StringBuilder();
            if (pchildren == null || pchildren.Length == 0)
            {
                buf.Append(plabel);
            }
            else
            {
                buf.Append('(').Append(plabel);
                foreach (TreeGraphNode child in pchildren)
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
                //throw new SystemException("Shouldn't be here!");
                return "";
            }
        }

        //@Override
        public override string ToString()
        {
            return plabel.ToString();
        }

        public string ToString(CoreLabel.OutputFormat format)
        {
            return plabel.ToString(format);
        }
        
        // Automatically generated by Eclipse
        private static readonly long serialVersionUID = 5080098143617475328L;
    }
}