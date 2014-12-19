using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Trees;

namespace OpenNLP.Tools.Util.Trees
{
    public class Tree
    {
        private Parse parse;
        /**
   * A leaf node should have a zero-length array for its
   * children. For efficiency, classes can use this array as a
   * return value for children() for leaf nodes if desired.
   * This can also be used elsewhere when you want an empty Tree array.
   */
  public static readonly Tree[] EMPTY_TREE_ARRAY = new Tree[0];

        public Tree() { }

        // TODO: build SP Tree from an OpenNlp Parse object
        public Tree(Parse p)
        {
            parse = p;
        }

        /**
   * Says whether a node is a leaf.  Can be used on an arbitrary
   * <code>Tree</code>.  Being a leaf is defined as having no
   * children.  This must be implemented as returning a zero-length
   * Tree[] array for children().
   *
   * @return true if this object is a leaf
   */

        public bool isLeaf()
        {
            return numChildren() == 0;
        }

        /**
   * Says how many children a tree node has in its local tree.
   * Can be used on an arbitrary <code>Tree</code>.  Being a leaf is defined
   * as having no children.
   *
   * @return The number of direct children of the tree node
   */

        public int numChildren()
        {
            return parse.ChildCount;
        }

        /**
   * Returns an array of children for the current node.  If there
   * are no children (if the node is a leaf), this must return a
   * Tree[] array of length 0.  A null children() value for tree
   * leaves was previously supported, but no longer is.
   * A caller may assume that either <code>isLeaf()</code> returns
   * true, or this node has a nonzero number of children.
   *
   * @return The children of the node
   * @see #getChildrenAsList()
   */

        public /*abstract*/ Tree[] children()
        {
            return parse.GetChildren().Select(ch => new Tree(ch)).ToArray();
        }

        /**
   * Return a <code>TreeFactory</code> that produces trees of the
   * appropriate type.
   *
   * @return A factory to produce Trees
   */

        public TreeFactory treeFactory()
        {
            return new SimpleTreeFactory();
        }

        /**
   * Returns the label associated with the current node, or null
   * if there is no label.  The default implementation always
   * returns {@code null}.
   *
   * @return The label of the node
   */
        //@Override
        public string label()
        {
            return parse.Label;
            //return null;
        }

        //@Override
        public String value()
        {
            /*Label lab = label();
    if (lab == null) {
      return null;
    }
    return lab.value();*/
            return parse.Label;
        }

        /**
* Return whether this node is a preterminal or not.  A preterminal is
* defined to be a node with one child which is itself a leaf.
*
* @return true if the node is a preterminal; false otherwise
*/

        public bool isPreTerminal()
        {
            Tree[] kids = children();
            return (kids.Length == 1) && (kids[0].isLeaf());
        }

        /**
   * Returns an iterator over all the nodes of the tree.  This method
   * implements the <code>iterator()</code> method required by the
   * <code>Collections</code> interface.  It does a preorder
   * (children after node) traversal of the tree.  (A possible
   * extension to the class at some point would be to allow different
   * traversal orderings via variant iterators.)
   *
   * @return An iterator over the nodes of the tree
   */
        //@Override
        public TreeIterator iterator()
        {
            return new TreeIterator(this);
        }

        public void setValue(String value)
        {
            var lab = label();
            if (lab != null)
            {
                parse.Label = value;
            }
        }

        /**
   * Return the parent of the tree node.  This routine may return
   * <code>null</code> meaning simply that the implementation doesn't
   * know how to determine the parent node, rather than there is no
   * such node.
   *
   * @return The parent <code>Tree</code> node or <code>null</code>
   * @see Tree#parent(Tree)
   */
        public Tree parent()
        {
            return new Tree(parse.Parent);
        }

        /**
   * Return the parent of the tree node.  This routine will traverse
   * a tree (depth first) from the given <code>root</code>, and will
   * correctly find the parent, regardless of whether the concrete
   * class stores parents.  It will only return <code>null</code> if this
   * node is the <code>root</code> node, or if this node is not
   * contained within the tree rooted at <code>root</code>.
   *
   * @param root The root node of the whole Tree
   * @return the parent <code>Tree</code> node if any;
   *         else <code>null</code>
   */
        public Tree parent(Tree root)
        {
            Tree[] kids = root.children();
            return parentHelper(root, kids, this);
        }


        private static Tree parentHelper(Tree parent, Tree[] kids, Tree node) {
    foreach (Tree kid in kids) {
      if (kid == node) {
        return parent;
      }
      Tree ret = node.parent(kid);
      if (ret != null) {
        return ret;
      }
    }
    return null;
  }

        /**
   * Returns the position of a Tree in the children list, if present,
   * or -1 if it is not present.  Trees are checked for presence with
   * object equality, ==.  Note that there are very few cases where an
   * indexOf that used .equals() instead of == would be useful and
   * correct.  In most cases, you want to figure out which child of
   * the parent a known tree is, so looking for object equality will
   * be faster and will avoid cases where you happen to have two
   * subtrees that are exactly the same.
   *
   * @param tree The tree to look for in children list
   * @return Its index in the list or -1
   */
        public int objectIndexOf(Tree tree)
        {
            Tree[] kids = children();
            for (int i = 0; i < kids.Length; i++)
            {
                if (kids[i] == tree)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
   * Returns a List of children for the current node.  If there are no
   * children, then a (non-null) <code>List&lt;Tree&gt;</code> of size 0 will
   * be returned.  The list has new list structure but pointers to,
   * not copies of the children.  That is, the returned list is mutable,
   * and simply adding to or deleting items from it is safe, but beware
   * changing the contents of the children.
   *
   * @return The children of the node
   */
        public List<Tree> getChildrenAsList()
        {
            return new List<Tree>(children());
        }
    }

    public class TreeIterator : IEnumerator<Tree>
    {

        private readonly List<Tree> treeStack;

        public TreeIterator(Tree t)
        {
            treeStack = new List<Tree>();
            treeStack.Add(t);
        }

        //@Override
        public bool hasNext()
        {
            return (!treeStack.Any());
        }

        //@Override
        public Tree next()
        {
            int lastIndex = treeStack.Count - 1;
            if (lastIndex < 0)
            {
                throw new IndexOutOfRangeException("TreeIterator exhausted");
            }
            Tree tr = treeStack[lastIndex];
            treeStack.Remove(tr);
            Tree[] kids = tr.children();
            // so that we can efficiently use one List, we reverse them
            for (int i = kids.Length - 1; i >= 0; i--)
            {
                treeStack.Add(kids[i]);
            }
            return tr;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public Tree Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
