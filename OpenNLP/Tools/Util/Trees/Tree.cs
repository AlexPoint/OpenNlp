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
         * Returns the last child of a tree, or <code>null</code> if none.
         *
         * @return The last child
         */
        public Tree lastChild()
        {
            Tree[] kids = children();
            if (kids.Length == 0)
            {
                return null;
            }
            return kids[kids.Length - 1];
        }

        /**
   * Finds the heads of the tree.  This code assumes that the label
   * does store and return sensible values for the category, word, and tag.
   * It will be a no-op otherwise.  The tree is modified.  The routine
   * assumes the Tree has word leaves and tag preterminals, and copies
   * their category to word and tag respectively, if they have a null
   * value.
   *
   * @param hf The headfinding algorithm to use
   */
        public void percolateHeads(HeadFinder hf) {
    Label nodeLabel = label();
    if (isLeaf()) {
      // Sanity check: word() is usually set by the TreeReader.
      if (nodeLabel is HasWord) {
        HasWord w = (HasWord) nodeLabel;
        if (w.word() == null) {
          w.setWord(nodeLabel.value());
        }
      }

    } else {
      foreach (Tree kid in children()) {
        kid.percolateHeads(hf);
      }

      /*final*/ Tree head = hf.determineHead(this);
      if (head != null) {
        /*final*/ Label headLabel = head.label();

        // Set the head tag.
        String headTag = (headLabel is HasTag) ? ((HasTag) headLabel).tag() : null;
        if (headTag == null && head.isLeaf()) {
          // below us is a leaf
          headTag = nodeLabel.value();
        }

        // Set the head word
        String headWord = (headLabel is HasWord) ? ((HasWord) headLabel).word() : null;
        if (headWord == null && head.isLeaf()) {
          // below us is a leaf
          // this might be useful despite case for leaf above in
          // case the leaf label type doesn't support word()
          headWord = headLabel.value();
        }

        // Set the head index
        int headIndex = (headLabel is HasIndex) ? ((HasIndex) headLabel).index() : -1;

        if (nodeLabel is HasWord) {
          ((HasWord) nodeLabel).setWord(headWord);
        }
        if (nodeLabel is HasTag) {
          ((HasTag) nodeLabel).setTag(headTag);
        }
        if (nodeLabel is HasIndex && headIndex >= 0) {
          ((HasIndex) nodeLabel).setIndex(headIndex);
        }

      } else {
        //System.err.println("Head is null: " + this);
      }
    }
  }

        /**
   * Makes a deep copy of not only the Tree structure but of the labels as well.
   * Uses the TreeFactory of the root node given by treeFactory().
   * Assumes that your labels give a non-null labelFactory().
   * (Added by Aria Haghighi.)
   *
   * @return A deep copy of the tree structure and its labels
   */
  public Tree deepCopy() {
    return deepCopy(treeFactory());
  }


  /**
   * Makes a deep copy of not only the Tree structure but of the labels as well.
   * The new tree will have nodes made by the given TreeFactory.
   * Each Label is copied using the labelFactory() returned
   * by the corresponding node's label.
   * It assumes that your labels give non-null labelFactory.
   * (Added by Aria Haghighi.)
   *
   * @param tf The TreeFactory used to make all nodes in the copied
   *           tree structure
   * @return A Tree that is a deep copy of the tree structure and
   *         Labels of the original tree.
   */
  public Tree deepCopy(TreeFactory tf) {
    return deepCopy(tf, label().labelFactory());
  }


  /**
   * Makes a deep copy of not only the Tree structure but of the labels as well.
   * Each tree is copied with the given TreeFactory.
   * Each Label is copied using the given LabelFactory.
   * That is, the tree and label factories can transform the nature of the
   * data representation.
   *
   * @param tf The TreeFactory used to make all nodes in the copied
   *           tree structure
   * @param lf The LabelFactory used to make all nodes in the copied
   *           tree structure
   * @return A Tree that is a deep copy of the tree structure and
   *         Labels of the original tree.
   */

  //@SuppressWarnings({"unchecked"})
  public Tree deepCopy(TreeFactory tf, LabelFactory lf) {
    Label lab = lf.newLabel(label());
    if (isLeaf()) {
      return tf.newLeaf(lab);
    }
    Tree[] kids = children();
    // NB: The below list may not be of type Tree but TreeGraphNode, so we leave it untyped
    var newKids = new List<Tree>();
    foreach (Tree kid in kids) {
      newKids.Add(kid.deepCopy(tf, lf));
    }
    return tf.newTreeNode(lab, newKids);
  }
        
        /**
         * insert <code>dtr</code> after <code>position</code> existing
         * daughters in <code>this</code>.
         */
        public void insertDtr(Tree dtr, int position)
        {
            Tree[] kids = children();
            if (position > kids.Length)
            {
                throw new ArgumentException("Can't insert tree after the " + position + "th daughter in " + this + "; only " + kids.Length + " daughters exist!");
            }
            Tree[] newKids = new Tree[kids.Length + 1];
            int i = 0;
            for (; i < position; i++)
            {
                newKids[i] = kids[i];
            }
            newKids[i] = dtr;
            for (; i < kids.Length; i++)
            {
                newKids[i + 1] = kids[i];
            }
            setChildren(newKids);
        }

        /**
   * Returns true if <code>this</code> dominates the Tree passed in
   * as an argument.  Object equality (==) rather than .equals() is used
   * to determine domination.
   * t.dominates(t) returns false.
   */
        public bool dominates(Tree t)
        {
            List<Tree> dPath = dominationPath(t);
            return dPath != null && dPath.Count > 1;
        }

        /**
   * Sets the label associated with the current node, if there is one.
   * The default implementation ignores the label.
   *
   * @param label The label
   */
  //@Override
  public void setLabel(Label label) {
    // a noop
  }

  /**
* Gets the yield of the tree.  The <code>Label</code> of all leaf nodes
* is returned
* as a list ordered by the natural left to right order of the
* leaves.  Null values, if any, are inserted into the list like any
* other value.
*
* @return a <code>List</code> of the data in the tree's leaves.
*/
  public List<Label> yield()
  {
      return yield(new List<Label>());
  }

  /**
   * Gets the yield of the tree.  The <code>Label</code> of all leaf nodes
   * is returned
   * as a list ordered by the natural left to right order of the
   * leaves.  Null values, if any, are inserted into the list like any
   * other value.
   * <p><i>Implementation notes:</i> c. 2003: This has been rewritten to thread, so only one List
   * is used. 2007: This method was duplicated to start to give type safety to Sentence.
   * This method will now make a Word for any Leaf which does not itself implement HasWord, and
   * put the Word into the Sentence, so the Sentence elements MUST implement HasWord.
   *
   * @param y The list in which the yield of the tree will be placed.
   *          Normally, this will be empty when the routine is called, but
   *          if not, the new yield is added to the end of the list.
   * @return a <code>List</code> of the data in the tree's leaves.
   */
  public List<Label> yield(List<Label> y) {
    if (isLeaf()) {
      y.Add(label());

    } else {
      Tree[] kids = children();
      foreach (Tree kid in kids) {
        kid.yield(y);
      }
    }
    return y;
  }


  /**
   * Adds the tree t at the index position among the daughters.  Note
   * that this method will throw an {@link ArrayIndexOutOfBoundsException} if
   * the daughter index is too big for the list of daughters.
   *
   * @param i the index position at which to add the new daughter
   * @param t the new daughter
   */
  public void addChild(int i, Tree t)
  {
      Tree[] kids = children();
      Tree[] newKids = new Tree[kids.Length + 1];
      if (i != 0)
      {
          Array.Copy(kids, 0, newKids, 0, i);
      }
      newKids[i] = t;
      if (i != kids.Length)
      {
          Array.Copy(kids, i, newKids, i + 1, kids.Length - i);
      }
      setChildren(newKids);
  }

  /**
   * Adds the tree t at the last index position among the daughters.
   *
   * @param t the new daughter
   */
  public void addChild(Tree t)
  {
      addChild(children().Length, t);
  }

        /**
         * Returns the path of nodes leading down to a dominated node,
         * including <code>this</code> and the dominated node itself.
         * Returns null if t is not dominated by <code>this</code>.  Object
         * equality (==) is the relevant criterion.
         * t.dominationPath(t) returns null.
         */
        public List<Tree> dominationPath(Tree t)
        {
            //Tree[] result = dominationPathHelper(t, 0);
            Tree[] result = dominationPath(t, 0);
            if (result == null)
            {
                return null;
            }
            return result.ToList();
        }

        private Tree[] dominationPathHelper(Tree t, int depth)
        {
            Tree[] kids = children();
            for (int i = kids.Length - 1; i >= 0; i--)
            {
                Tree t1 = kids[i];
                if (t1 == null)
                {
                    return null;
                }
                Tree[] result;
                if ((result = t1.dominationPath(t, depth + 1)) != null)
                {
                    result[depth] = this;
                    return result;
                }
            }
            return null;
        }

        private Tree[] dominationPath(Tree t, int depth)
        {
            if (this == t)
            {
                Tree[] result = new Tree[depth + 1];
                result[depth] = this;
                return result;
            }
            return dominationPathHelper(t, depth);
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
   * Set the children of this node to be the children given in the
   * array.  This is an <b>optional</b> operation; by default it is
   * unsupported.  Note for subclasses that if there are no
   * children, the children() method must return a Tree[] array of
   * length 0.  This class provides a
   * {@code EMPTY_TREE_ARRAY} canonical zero-length Tree[] array
   * to represent zero children, but it is <i>not</i> required that
   * leaf nodes use this particular zero-length array to represent
   * a leaf node.
   *
   * @param children The array of children, each a <code>Tree</code>
   * @see #setChildren(List)
   */
  public void setChildren(Tree[] children) {
    throw new InvalidOperationException();
  }


  /**
   * Set the children of this tree node to the given list.  This
   * method is implemented in the <code>Tree</code> class by
   * converting the <code>List</code> into a tree array and calling
   * the array-based method.  Subclasses which use a
   * <code>List</code>-based representation of tree children should
   * override this method.  This implementation allows the case
   * that the <code>List</code> is <code>null</code>: it yields a
   * node with no children (represented by a canonical zero-length
   * children() array).
   *
   * @param childTreesList A list of trees to become children of the node.
   *          This method does not retain the List that you pass it (copying
   *          is done), but it will retain the individual children (they are
   *          not copied).
   * @see #setChildren(Tree[])
   */
  public void setChildren(List<Tree> childTreesList) {
    if (childTreesList == null || !childTreesList.Any()) {
      setChildren(EMPTY_TREE_ARRAY);
    } else {
      Tree[] childTrees = childTreesList.ToArray();
      setChildren(childTrees);
    }
  }

  /**
* Replaces the <code>i</code>th child of <code>this</code> with the tree t.
* Note
* that this method will throw an {@link ArrayIndexOutOfBoundsException} if
* the child index is too big for the list of children.
*
* @param i The index position at which to replace the child
* @param t The new child
* @return The tree that was previously the ith d
*/
  public Tree setChild(int i, Tree t)
  {
      Tree[] kids = children();
      Tree old = kids[i];
      kids[i] = t;
      return old;
  }

  /**
* Destructively removes the child at some daughter index and returns it.
* Note
* that this method will throw an {@link ArrayIndexOutOfBoundsException} if
* the daughter index is too big for the list of daughters.
*
* @param i The daughter index
* @return The tree at that daughter index
*/
  public Tree removeChild(int i)
  {
      Tree[] kids = children();
      Tree kid = kids[i];
      Tree[] newKids = new Tree[kids.Length - 1];
      for (int j = 0; j < newKids.Length; j++)
      {
          if (j < i)
          {
              newKids[j] = kids[j];
          }
          else
          {
              newKids[j] = kids[j + 1];
          }
      }
      setChildren(newKids);
      return kid;
  }

        /**
   * Get the set of all subtrees inside the tree by returning a tree
   * rooted at each node.  These are <i>not</i> copies, but all share
   * structure.  The tree is regarded as a subtree of itself.
   * <p/>
   * <i>Note:</i> If you only want to form this Set so that you can
   * iterate over it, it is more efficient to simply use the Tree class's
   * own <code>iterator() method. This will iterate over the exact same
   * elements (but perhaps/probably in a different order).
   *
   * @return the <code>Set</code> of all subtrees in the tree.
   */
  public Set<Tree> subTrees() {
    return new HashSet<Tree>(subTrees(new HashSet<Tree>()));
  }

  /**
   * Get the list of all subtrees inside the tree by returning a tree
   * rooted at each node.  These are <i>not</i> copies, but all share
   * structure.  The tree is regarded as a subtree of itself.
   * <p/>
   * <i>Note:</i> If you only want to form this Collection so that you can
   * iterate over it, it is more efficient to simply use the Tree class's
   * own <code>iterator() method. This will iterate over the exact same
   * elements (but perhaps/probably in a different order).
   *
   * @return the <code>List</code> of all subtrees in the tree.
   */
  public List<Tree> subTreeList() {
    return subTrees(new List<Tree>()).ToList();
  }


  /**
   * Add the set of all subtrees inside a tree (including the tree itself)
   * to the given <code>Collection</code>.
   * <p/>
   * <i>Note:</i> If you only want to form this Collection so that you can
   * iterate over it, it is more efficient to simply use the Tree class's
   * own <code>iterator() method. This will iterate over the exact same
   * elements (but perhaps/probably in a different order).
   *
   * @param n A collection of nodes to which the subtrees will be added.
   * @return The collection parameter with the subtrees added.
   */
  public /*<T extends Collection<Tree>>*/ ICollection<Tree> subTrees(ICollection<Tree> n) {
    n.Add(this);
    Tree[] kids = children();
    foreach (Tree kid in kids) {
      kid.subTrees(n);
    }
    return n;
  }

        /**
   * Returns the label associated with the current node, or null
   * if there is no label.  The default implementation always
   * returns {@code null}.
   *
   * @return The label of the node
   */
        //@Override
        public Label label()
        {
            var labFact = new StringLabelFactory();
            var lab = labFact.newLabelFromString(parse.Label);
            return new CoreLabel(lab);
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

        /**
         * Return whether this node is a phrasal node or not.  A phrasal node
         * is defined to be a node which is not a leaf or a preterminal.
         * Worded positively, this means that it must have two or more children,
         * or one child that is not a leaf.
         *
         * @return <code>true</code> if the node is phrasal;
         *         <code>false</code> otherwise
         */
        public bool isPhrasal()
        {
            Tree[] kids = children();
            return !(kids == null || kids.Length == 0 || (kids.Length == 1 && kids[0].isLeaf()));
        }

        /**
   * Returns the first child of a tree, or <code>null</code> if none.
   *
   * @return The first child
   */
        public Tree firstChild()
        {
            Tree[] kids = children();
            if (kids.Length == 0)
            {
                return null;
            }
            return kids[0];
        }

        /**
   * Gets the preterminal yield (i.e., tags) of the tree.  All data in
   * preterminal nodes is returned as a list ordered by the natural left to
   * right order of the tree.  Null values, if any, are inserted into the
   * list like any other value.  Pre-leaves are nodes of height 1.
   *
   * @return a {@code List} of the data in the tree's pre-leaves.
   */
        public List<Label> preTerminalYield()
        {
            return preTerminalYield(new List<Label>());
        }

        /**
   * Gets the preterminal yield (i.e., tags) of the tree.  All data in
   * preleaf nodes is returned as a list ordered by the natural left to
   * right order of the tree.  Null values, if any, are inserted into the
   * list like any other value.  Pre-leaves are nodes of height 1.
   *
   * @param y The list in which the preterminals of the tree will be
   *          placed. Normally, this will be empty when the routine is called,
   *          but if not, the new yield is added to the end of the list.
   * @return a <code>List</code> of the data in the tree's pre-leaves.
   */
        public List<Label> preTerminalYield(List<Label> y) {
    if (isPreTerminal()) {
      y.Add(label());
    } else {
      Tree[] kids = children();
      foreach (Tree kid in kids) {
        kid.preTerminalYield(y);
      }
    }
    return y;
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

        /**
   * Finds the depth of the tree.  The depth is defined as the length
   * of the longest path from this node to a leaf node.  Leaf nodes
   * have depth zero.  POS tags have depth 1. Phrasal nodes have
   * depth &gt;= 2.
   *
   * @return the depth
   */
        public int depth() {
    if (isLeaf()) {
      return 0;
    }
    int maxDepth = 0;
    Tree[] kids = children();
    foreach (Tree kid in kids) {
      int curDepth = kid.depth();
      if (curDepth > maxDepth) {
        maxDepth = curDepth;
      }
    }
    return maxDepth + 1;
  }

        /**
         * Finds the distance from this node to the specified node.
         * return -1 if this is not an ancestor of node.
         *
         * @param node A subtree contained in this tree
         * @return the depth
         */
        public int depth(Tree node)
        {
            Tree p = node.parent(this);
            if (this == node) { return 0; }
            if (p == null) { return -1; }
            int depth = 1;
            while (this != p)
            {
                p = p.parent(this);
                depth++;
            }
            return depth;
        }

        /**
   * Return the child at some daughter index.  The children are numbered
   * starting with an index of 0.
   *
   * @param i The daughter index
   * @return The tree at that daughter index
   */
        public Tree getChild(int i)
        {
            Tree[] kids = children();
            return kids[i];
        }

        /**
   *  Returns the value of the nodes label as a String.  This is done by
   *  calling <code>toString()</code> on the value, if it exists. Otherwise,
   *  an empty string is returned.
   *
   *  @return The label of a tree node as a String
   */
        public String nodeString()
        {
            return (value() == null) ? "" : value();
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
