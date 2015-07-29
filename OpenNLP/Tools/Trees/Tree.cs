using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// The abstract class <code>Tree</code> is used to collect all of the
    /// tree types, and acts as a generic extendable type.  This is the
    /// standard implementation of inheritance-based polymorphism.
    /// All <code>Tree</code> objects support accessors for their children (a
    /// <code>Tree[]</code>), their label (a <code>Label</code>), and their
    /// score (a <code>double</code>).  However, different concrete
    /// implementations may or may not include the latter two, in which
    /// case a default value is returned.  The class Tree defines no data
    /// fields.  The two abstract methods that must be implemented are:
    /// <code>children()</code>, and <code>treeFactory()</code>.
    /// Notes that <code>setChildren(Tree[])</code> is now an optional
    /// operation, whereas it was previously required to be
    /// implemented. There is now support for finding the parent of a
    /// tree.  This may be done by search from a tree root, or via a
    /// directly stored parent.  The <code>Tree</code> class now
    /// implements the <code>Collection</code> interface: in terms of
    /// this, each <i>node</i> of the tree is an element of the
    /// collection; hence one can explore the tree by using the methods of
    /// this interface.  A <code>Tree</code> is regarded as a read-only
    /// <code>Collection</code> (even though the <code>Tree</code> class
    /// has various methods that modify trees).  Moreover, the
    /// implementation is <i>not</i> thread-safe: no attempt is made to
    /// detect and report concurrent modifications.
    /// 
    /// @author Christopher Manning
    /// @author Dan Klein
    /// @author Sarah Spikes (sdspikes@cs.stanford.edu) - filled in types
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class Tree : AbstractCollection<Tree>, ILabel, ILabeled, IScored
    {
        /// <summary>
        ///  A leaf node should have a zero-length array for its children.
        /// For efficiency, classes can use this array as a return value 
        /// for children() for leaf nodes if desired.
        /// This can also be used elsewhere when you want an empty Tree array
        /// </summary>
        public static readonly Tree[] EmptyTreeArray = new Tree[0];

        /// <summary>
        /// Says whether a node is a leaf.
        /// Can be used on an arbitrary <code>Tree</code>.
        /// Being a leaf is defined as having no children.
        /// This must be implemented as returning a zero-length Tree[] array for children().
        /// </summary>
        /// <returns>true if this object is a leaf</returns>
        public bool IsLeaf()
        {
            return NumChildren() == 0;
        }

        /// <summary>
        /// Says how many children a tree node has in its local tree.
        /// Can be used on an arbitrary <code>Tree</code>.  Being a leaf is defined
        /// as having no children.
        /// </summary>
        /// <returns>The number of direct children of the tree node</returns>
        public int NumChildren()
        {
            return Children().Length;
        }

        /// <summary>
        /// Says whether the current node has only one child.
        /// Can be used on an arbitrary <code>Tree</code>
        /// </summary>
        /// <returns>Whether the node heads a unary rewrite</returns>
        public bool IsUnaryRewrite()
        {
            return NumChildren() == 1;
        }

        /// <summary>
        /// Return whether this node is a preterminal or not.
        /// A preterminal is defined to be a node with one child which is itself a leaf.
        /// </summary>
        public bool IsPreTerminal()
        {
            Tree[] kids = Children();
            return (kids.Length == 1) && (kids[0].IsLeaf());
        }

        /// <summary>
        /// Return whether all the children of this node are preterminals or not.
        /// A preterminal is defined to be a node with one child which is itself a leaf.
        /// Considered false if the node has no children
        /// </summary>
        public bool IsPrePreTerminal()
        {
            Tree[] kids = Children();
            if (kids.Length == 0)
            {
                return false;
            }
            foreach (Tree kid in kids)
            {
                if (! kid.IsPreTerminal())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Return whether this node is a phrasal node or not.
        /// A phrasal node is defined to be a node which is not a leaf or a preterminal.
        /// Worded positively, this means that it must have two or more children,
        /// or one child that is not a leaf.
        /// </summary>
        public bool IsPhrasal()
        {
            Tree[] kids = Children();
            return !(kids == null || kids.Length == 0 || (kids.Length == 1 && kids[0].IsLeaf()));
        }

        /// <summary>
        /// Implements equality for Tree's.
        /// Two Tree objects are equal if they have equal {@link #value}s,
        /// the same number of children, and their children are pairwise equal.
        /// </summary>
        public override bool Equals(Object o)
        {
            if (o == this)
            {
                return true;
            }
            if (!(o is Tree))
            {
                return false;
            }
            var t = (Tree) o;
            string value1 = this.Value();
            string value2 = t.Value();
            if (value1 != null || value2 != null)
            {
                if (value1 == null || value2 == null || !value1.Equals(value2))
                {
                    return false;
                }
            }
            Tree[] mykids = Children();
            Tree[] theirkids = t.Children();
            //if((mykids == null && (theirkids == null || theirkids.Length != 0)) || (theirkids == null && mykids.Length != 0) || (mykids.Length != theirkids.Length)){
            if (mykids.Length != theirkids.Length)
            {
                return false;
            }
            for (int i = 0; i < mykids.Length; i++)
            {
                if (!mykids[i].Equals(theirkids[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Implements a hashCode for Tree's.  
        /// Two trees should have the same hashcode if they are equal, 
        /// so we hash on the label value and the children's label values.
        /// </summary>
        public override int GetHashCode()
        {
            string v = this.Value();
            int hc = (v == null) ? 1 : v.GetHashCode();
            Tree[] kids = Children();
            for (int i = 0; i < kids.Length; i++)
            {
                v = kids[i].Value();
                int hc2 = (v == null) ? i : v.GetHashCode();
                hc ^= (hc2 << i);
            }
            return hc;
        }

        /// <summary>
        /// Returns the position of a Tree in the children list, if present,
        /// or -1 if it is not present.  Trees are checked for presence with
        /// object equality, ==.  Note that there are very few cases where an
        /// indexOf that used .equals() instead of == would be useful and
        /// correct.  In most cases, you want to figure out which child of
        /// the parent a known tree is, so looking for object equality will
        /// be faster and will avoid cases where you happen to have two
        /// subtrees that are exactly the same.
        /// </summary>
        /// <param name="tree">The tree to look for in children list</param>
        /// <returns>Its index in the list or -1</returns>
        public int ObjectIndexOf(Tree tree)
        {
            Tree[] kids = Children();
            for (int i = 0; i < kids.Length; i++)
            {
                if (kids[i] == tree)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns an array of children for the current node.
        /// If there are no children (if the node is a leaf), this must return a
        /// Tree[] array of length 0.  A null children() value for tree
        /// leaves was previously supported, but no longer is.
        /// A caller may assume that either <code>isLeaf()</code> returns
        /// true, or this node has a nonzero number of children.
        /// </summary>
        /// <returns>The children of the node</returns>
        public abstract Tree[] Children();

        /// <summary>
        /// Returns a List of children for the current node.  If there are no
        /// children, then a (non-null) <code>List&lt;Tree&gt;</code> of size 0 will
        /// be returned.  The list has new list structure but pointers to,
        /// not copies of the children.  That is, the returned list is mutable,
        /// and simply adding to or deleting items from it is safe, but beware
        /// changing the contents of the children.
        /// </summary>
        /// <returns>The children of the node</returns>
        public List<Tree> GetChildrenAsList()
        {
            return new List<Tree>(Children());
        }

        /// <summary>
        /// Set the children of this node to be the children given in the array.
        /// This is an <b>optional</b> operation; by default it is unsupported.
        /// Note for subclasses that if there are no children, 
        /// the children() method must return a Tree[] array of length 0.
        /// This class provides a {@code EMPTY_TREE_ARRAY} canonical zero-length Tree[] array
        /// to represent zero children, but it is <i>not</i> required that
        /// leaf nodes use this particular zero-length array to represent a leaf node.
        /// </summary>
        /// <param name="children">The array of children, each a <code>Tree</code></param>
        public virtual void SetChildren(Tree[] children)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Set the children of this tree node to the given list.
        /// This method is implemented in the <code>Tree</code> class by
        /// converting the <code>List</code> into a tree array and calling
        /// the array-based method.  Subclasses which use a
        /// <code>List</code>-based representation of tree children should
        /// override this method.  This implementation allows the case
        /// that the <code>List</code> is <code>null</code>: it yields a
        /// node with no children (represented by a canonical zero-length children() array).
        /// </summary>
        /// <param name="childTreesList">
        /// A list of trees to become children of the node.
        /// This method does not retain the List that you pass it (copying is done), 
        /// but it will retain the individual children (they are not copied)
        /// </param>
        public virtual void SetChildren(List<Tree> childTreesList)
        {
            if (childTreesList == null || !childTreesList.Any())
            {
                SetChildren(EmptyTreeArray);
            }
            else
            {
                /*int leng = childTreesList.Count;
                  Tree[] childTrees = new Tree[leng];
                  childTreesList.toArray(childTrees);
                  setChildren(childTrees);*/
                SetChildren(childTreesList.ToArray());
            }
        }

        /// <summary>
        /// Returns the label associated with the current node, or null
        /// if there is no label.  The default implementation always returns {@code null}.
        /// </summary>
        /// <returns>The label of the node</returns>
        public virtual ILabel Label()
        {
            return null;
        }

        /// <summary>
        /// Sets the label associated with the current node, if there is one.
        /// The default implementation ignores the label.
        /// </summary>
        public virtual void SetLabel(ILabel label)
        {
            // a noop
        }

        /// <summary>
        /// Returns the score associated with the current node, 
        /// or NaN if there is no score.  The default implementation returns NaN.
        /// </summary>
        public virtual double Score()
        {
            return Double.NaN;
        }

        /// <summary>
        /// Sets the score associated with the current node, if there is one.
        /// </summary>
        public virtual void SetScore(double score)
        {
            throw new InvalidOperationException(
                "You must use a tree type that implements scoring in order call setScore()");
        }

        /// <summary>
        /// Returns the first child of a tree, or <code>null</code> if none.
        /// </summary>
        public Tree FirstChild()
        {
            Tree[] kids = Children();
            if (kids.Length == 0)
            {
                return null;
            }
            return kids[0];
        }

        /// <summary>
        /// Returns the last child of a tree, or <code>null</code> if none.
        /// </summary>
        public Tree LastChild()
        {
            Tree[] kids = Children();
            if (kids.Length == 0)
            {
                return null;
            }
            return kids[kids.Length - 1];
        }

        /// <summary>
        /// Return the highest node of the (perhaps trivial) unary chain that this node is part of.
        /// In case this node is the only child of its parent, trace up the chain of
        /// unaries, and return the uppermost node of the chain (the node whose
        /// parent has multiple children, or the node that is the root of the tree).
        /// </summary>
        /// <param name="root">The root of the tree that contains this subtree</param>
        /// <returns>
        /// The uppermost node of the unary chain, if this node is in a unary
        /// chain, or else the current node
        /// </returns>
        public Tree UpperMostUnary(Tree root)
        {
            Tree p = Parent(root);
            if (p == null)
            {
                return this;
            }
            if (p.NumChildren() > 1)
            {
                return this;
            }
            return p.UpperMostUnary(root);
        }

        /// <summary>
        /// Assign a SpanAnnotation on each node of this tree. The index starts at zero
        /// </summary>
        public void SetSpans()
        {
            ConstituentsNodes(0);
        }

        /**
        * Returns SpanAnnotation of this node, or null if annotation is not assigned.
        * Use <code>setSpans()</code> to assign SpanAnnotations to a tree.
        *
        * @return an IntPair: the SpanAnnotation of this node.
        */
            /*public IntPair getSpan() {
        if(label() is CoreMap && ((CoreMap) label()).has(typeof(CoreAnnotations.SpanAnnotation)))
            return ((CoreMap) label()).get(typeof(CoreAnnotations.SpanAnnotation));
        return null;
        }*/

        /// <summary>
        /// Returns the Constituents generated by the parse tree. 
        /// Constituents are computed with respect to whitespace (e.g., at the word level)
        /// </summary>
        /// <returns>
        /// a Set of the constituents as constituents of type <code>Constituent</code>
        /// </returns>
        public Set<Constituent> Constituents()
        {
            return Constituents(new SimpleConstituentFactory());
        }

        /// <summary>
        /// Returns the Constituents generated by the parse tree.
        /// The Constituents of a sentence include the preterminal categories but not the leaves.
        /// </summary>
        /// <param name="cf">ConstituentFactory used to build the Constituent objects</param>
        /// <returns>a Set of the constituents as SimpleConstituent type
        /// (in the current implementation, a <code>HashSet</code></returns>
        public Set<Constituent> Constituents(IConstituentFactory cf)
        {
            return Constituents(cf, false);
        }

        /// <summary>
        /// Returns the Constituents generated by the parse tree.
        /// The Constituents of a sentence include the preterminal categories but not the leaves.
        /// </summary>
        /// <param name="cf">ConstituentFactory used to build the Constituent objects</param>
        /// <param name="maxDepth">
        /// The maximum depth at which to add constituents, where 0 is the root level.
        /// Negative maxDepth indicates no maximum.
        /// </param>
        /// <returns>
        /// a Set of the constituents as SimpleConstituent type (in the current implementation, a <code>HashSet</code>
        /// </returns>
        public Set<Constituent> Constituents(IConstituentFactory cf, int maxDepth)
        {
            Set<Constituent> constituentsSet = new Util.HashSet<Constituent>();
            Constituents(constituentsSet, 0, cf, false, null, maxDepth, 0);
            return constituentsSet;
        }

        /// <summary>
        /// Returns the Constituents generated by the parse tree.
        /// The Constituents of a sentence include the preterminal categories but not the leaves.
        /// </summary>
        /// <param name="cf">ConstituentFactory used to build the Constituent objects</param>
        /// <param name="charLevel">If true, compute bracketings irrespective of whitespace boundaries.</param>
        /// <returns>
        /// a Set of the constituents as SimpleConstituent type (in the current implementation, a <code>HashSet</code>
        /// </returns>
        public Set<Constituent> Constituents(IConstituentFactory cf, bool charLevel)
        {
            Set<Constituent> constituentsSet = new Util.HashSet<Constituent>();
            Constituents(constituentsSet, 0, cf, charLevel, null, -1, 0);
            return constituentsSet;
        }

        public Set<Constituent> Constituents(IConstituentFactory cf, bool charLevel, Predicate<Tree> filter)
        {
            Set<Constituent> constituentsSet = new Util.HashSet<Constituent>();
            Constituents(constituentsSet, 0, cf, charLevel, filter, -1, 0);
            return constituentsSet;
        }

        /// <summary>
        /// Same as int constituents but just puts the span as an IntPair
        /// in the CoreLabel of the nodes.
        /// </summary>
        /// <param name="left">The left position to begin labeling from</param>
        /// <returns>The index of the right frontier of the constituent</returns>
        private int ConstituentsNodes(int left)
        {
            if (IsLeaf())
            {
                if (Label() is CoreLabel)
                {
                    ((CoreLabel) Label()).Set(typeof (CoreAnnotations.SpanAnnotation), new IntPair(left, left));
                }
                else
                {
                    throw new InvalidOperationException("Can only set spans on trees which use CoreLabel");
                }
                return (left + 1);
            }
            int position = left;

            // enumerate through daughter trees
            Tree[] kids = Children();
            foreach (Tree kid in kids)
                position = kid.ConstituentsNodes(position);

            //Parent span
            if (Label() is CoreLabel)
            {
                ((CoreLabel) Label()).Set(typeof (CoreAnnotations.SpanAnnotation), new IntPair(left, position - 1));
            }
            else
            {
                throw new InvalidOperationException("Can only set spans on trees which use CoreLabel");
            }

            return position;
        }

        /// <summary>
        /// Adds the constituents derived from <code>this</code> tree to
        /// the ordered <code>Constituent</code> <code>Set</code>, beginning
        /// numbering from the second argument and returning the number of
        /// the right edge.  The reason for the return of the right frontier
        /// is in order to produce bracketings recursively by threading through
        /// the daughters of a given tree.
        /// </summary>
        /// <param name="constituentsSet">set of constituents to add results of bracketing this tree to</param>
        /// <param name="left">left position to begin labeling the bracketings with</param>
        /// <param name="cf">ConstituentFactory used to build the Constituent objects</param>
        /// <param name="charLevel">If true, compute constituents without respect to whitespace. Otherwise, preserve whitespace boundaries</param>
        /// <param name="filter">A filter to use to decide whether or not to add a tree as a constituent</param>
        /// <param name="maxDepth">The maximum depth at which to allow constituents.  Set to negative to indicate all depths allowed.</param>
        /// <param name="depth">The current depth</param>
        /// <returns>Index of right frontier of Constituent</returns>
        private int Constituents(Set<Constituent> constituentsSet, int left, IConstituentFactory cf, bool charLevel,
            Predicate<Tree> filter, int maxDepth, int depth)
        {

            if (IsPreTerminal())
                return left + ((charLevel) ? FirstChild().Value().Length : 1);

            int position = left;
            Tree[] kids = Children();
            foreach (Tree kid in kids)
            {
                position = kid.Constituents(constituentsSet, position, cf, charLevel, filter, maxDepth, depth + 1);
            }

            if ((filter == null || filter(this)) &&
                (maxDepth < 0 || depth <= maxDepth))
            {
                //Compute span of entire tree at the end of recursion
                constituentsSet.Add(cf.NewConstituent(left, position - 1, Label(), Score()));
            }
            return position;
        }
        
        /// <summary>
        /// Returns a new Tree that represents the local Tree at a certain node.
        /// That is, it builds a new tree that copies the mother and daughter
        /// nodes (but not their Labels), as non-Leaf nodes,
        /// but zeroes out their children.
        /// </summary>
        public Tree LocalTree()
        {
            Tree[] kids = Children();
            var newKids = new Tree[kids.Length];
            ITreeFactory tf = TreeFactory();
            for (int i = 0, n = kids.Length; i < n; i++)
            {
                newKids[i] = tf.NewTreeNode(kids[i].Label(), EmptyTreeArray.ToList());
            }
            return tf.NewTreeNode(Label(), newKids.ToList());
        }

        /// <summary>
        /// Returns a set of one level <code>Tree</code>s that ares the local trees
        /// of the tree.
        /// That is, it builds a new tree that copies the mother and daughter
        /// nodes (but not their Labels), for each phrasal node, but zeroes out their children.
        /// </summary>
        public Set<Tree> LocalTrees()
        {
            var set = new Set<Tree>();
            foreach (Tree st in this)
            {
                if (st.IsPhrasal())
                {
                    set.Add(st.LocalTree());
                }
            }
            return set;
        }

        /// <summary>
        /// Most instances of <code>Tree</code> will take a lot more than
        /// than the default <code>StringBuffer</code> size of 16 to print
        /// as an indented list of the whole tree, so we enlarge the default.
        /// </summary>
        private const int InitialPrintStringBuilderSize = 500;

        /// <summary>
        /// Appends the printed form of a parse tree (as a bracketed String) to a {@code StringBuilder}.
        /// The implementation of this may be more efficient than for
        /// {@code ToString()} on complex trees.
        /// </summary>
        /// <param name="sb">The {@code StringBuilder} to which the tree will be appended</param>
        /// <returns>Returns the {@code StringBuilder} passed in with extra stuff in it</returns>
        public StringBuilder ToStringBuilder(StringBuilder sb)
        {
            return ToStringBuilder(sb, true);
        }

        /// <summary>
        /// Appends the printed form of a parse tree (as a bracketed String)
        /// to a {@code StringBuilder}.
        /// The implementation of this may be more efficient than for
        /// {@code ToString()} on complex trees.
        /// </summary>
        /// <param name="sb">The {@code StringBuilder} to which the tree will be appended</param>
        /// <param name="printOnlyLabelValue">If true, print only the value() of each node's label</param>
        /// <returns>the {@code StringBuilder} passed in with extra stuff in it</returns>
        public StringBuilder ToStringBuilder(StringBuilder sb, bool printOnlyLabelValue)
        {
            if (IsLeaf())
            {
                if (Label() != null)
                {
                    if (printOnlyLabelValue)
                    {
                        sb.Append(Label().Value());
                    }
                    else
                    {
                        sb.Append(Label());
                    }
                }
                return sb;
            }
            else
            {
                sb.Append('(');
                if (Label() != null)
                {
                    if (printOnlyLabelValue)
                    {
                        if (Value() != null)
                        {
                            sb.Append(Label().Value());
                        }
                        // don't print a null, just nothing!
                    }
                    else
                    {
                        sb.Append(Label());
                    }
                }
                Tree[] kids = Children();
                if (kids != null)
                {
                    foreach (Tree kid in kids)
                    {
                        sb.Append(' ');
                        kid.ToStringBuilder(sb, printOnlyLabelValue);
                    }
                }
                return sb.Append(')');
            }
        }

        /// <summary>
        /// Converts parse tree to string in Penn Treebank format.
        /// 
        /// Implementation note: Internally, the method gains
        /// efficiency by chaining use of a single <code>StringBuilder</code> through all the printing.
        /// </summary>
        public override string ToString()
        {
            return ToStringBuilder(new StringBuilder(Tree.InitialPrintStringBuilderSize)).ToString();
        }


        private const int IndentIncr = 2;


        private static string MakeIndentString(int indent)
        {
            var sb = new StringBuilder(indent);
            for (int i = 0; i < IndentIncr; i++)
            {
                sb.Append(' ');
            }
            return sb.ToString();
        }

        /**
   * Indented list printing of a tree.  The tree is printed in an
   * indented list notation, with node labels followed by node scores.
   * string parameters are used rather than integer levels for efficiency.
   *
   * @param indent The base <code>string</code> (normally just spaces)
   *               to print before each line of tree
   * @param pad    The additional <code>string</code> (normally just more
   *               spaces) to add when going to a deeper level of <code>Tree</code>.
   * @param pw     The PrintWriter to print the tree to
   * @param printScores Whether to print the scores (log probs) of tree nodes
   */
        /*private void indentedListPrint(string indent, string pad, PrintWriter pw, bool printScores) {
    StringBuilder sb = new StringBuilder(indent);
    Label label = label();
    if (label != null) {
      sb.Append(label.ToString());
    }
    if (printScores) {
      sb.Append("  ");
      sb.Append(score());
    }
    pw.println(sb.ToString());
    Tree[] children = children();
    string newIndent = indent + pad;
    foreach (Tree child in children) {
      child.indentedListPrint(newIndent, pad, pw, printScores);
    }
  }*/
        

        /*private static void displayChildren(Tree[] trChildren, int indent, bool parentLabelNull, bool onlyLabelValue, PrintWriter pw) {
    bool firstSibling = true;
    bool leftSibIsPreTerm = true;  // counts as true at beginning
    foreach (Tree currentTree in trChildren) {
      currentTree.display(indent, parentLabelNull, firstSibling, leftSibIsPreTerm, false, onlyLabelValue, pw);
      leftSibIsPreTerm = currentTree.isPreTerminal();
      // CC is a special case for English, but leave it in so we can exactly match PTB3 tree formatting
      if (currentTree.value() != null && currentTree.value().startsWith("CC")) {
        leftSibIsPreTerm = false;
      }
      firstSibling = false;
    }
  }*/
        /// <summary>
        /// Returns the value of the nodes label as a string.
        /// This is done by calling <code>ToString()</code> on the value, if it exists.
        /// Otherwise, an empty string is returned.
        /// </summary>
        /// <returns>The label of a tree node as a String</returns>
        public virtual string NodeString()
        {
            return Value() ?? "";
        }

        /**
   * Display a node, implementing Penn Treebank style layout
   */
        /*private void display(int indent, bool parentLabelNull, bool firstSibling, bool leftSiblingPreTerminal, bool topLevel, bool onlyLabelValue, PrintWriter pw) {
    // the condition for staying on the same line in Penn Treebank
    bool suppressIndent = (parentLabelNull || (firstSibling && isPreTerminal()) || (leftSiblingPreTerminal && isPreTerminal() && (label() == null || !label().value().startsWith("CC"))));
    if (suppressIndent) {
      pw.print(" ");
      // pw.flush();
    } else {
      if (!topLevel) {
        pw.println();
      }
      for (int i = 0; i < indent; i++) {
        pw.print("  ");
        // pw.flush();
      }
    }
    if (isLeaf() || isPreTerminal()) {
      string terminalString = toStringBuilder(new StringBuilder(), onlyLabelValue).ToString();
      pw.print(terminalString);
      pw.flush();
      return;
    }
    pw.print("(");
    string nodeString;
    if (onlyLabelValue) {
      string value = value();
      nodeString = (value == null) ? "" : value;
    } else {
      nodeString = nodeString();
    }
    pw.print(nodeString);
    // pw.flush();
    bool parentIsNull = label() == null || label().value() == null;
    displayChildren(children(), indent + 1, parentIsNull, true, pw);
    pw.print(")");
    pw.flush();
  }*/


        /**
   * Print the tree as done in Penn Treebank merged files.
   * The formatting should be exactly the same, but we don't print the
   * trailing whitespace found in Penn Treebank trees.
   * The basic deviation from a bracketed indented tree is to in general
   * collapse the printing of adjacent preterminals onto one line of
   * tags and words.  Additional complexities are that conjunctions
   * (tag CC) are not collapsed in this way, and that the unlabeled
   * outer brackets are collapsed onto the same line as the next
   * bracket down.
   *
   * @param pw The tree is printed to this <code>PrintWriter</code>
   */
        /*public void pennPrint(PrintWriter pw) {
    pennPrint(pw, true);
  }

  public void pennPrint(PrintWriter pw, bool printOnlyLabelValue) {
    display(0, false, false, false, true, printOnlyLabelValue, pw);
    pw.println();
    pw.flush();
  }*/


        /**
   * Print the tree as done in Penn Treebank merged files.
   * The formatting should be exactly the same, but we don't print the
   * trailing whitespace found in Penn Treebank trees.
   * The basic deviation from a bracketed indented tree is to in general
   * collapse the printing of adjacent preterminals onto one line of
   * tags and words.  Additional complexities are that conjunctions
   * (tag CC) are not collapsed in this way, and that the unlabeled
   * outer brackets are collapsed onto the same line as the next
   * bracket down.
   *
   * @param ps The tree is printed to this <code>PrintStream</code>
   */
        /*public void pennPrint(PrintStream ps) {
    pennPrint(new PrintWriter(new OutputStreamWriter(ps), true));
  }

  public void pennPrint(PrintStream ps, bool printOnlyLabelValue) {
    pennPrint(new PrintWriter(new OutputStreamWriter(ps), true), printOnlyLabelValue);
  }*/

        /**
   * Calls <code>pennPrint()</code> and saves output to a String
   *
   * @return The indent S-expression representation of a Tree
   */
        /*public string pennString() {
    StringWriter sw = new StringWriter();
    pennPrint(new PrintWriter(sw));
    return sw.ToString();
  }*/

        /// <summary>
        /// Finds the depth of the tree.  The depth is defined as the length
        /// of the longest path from this node to a leaf node.  Leaf nodes
        /// have depth zero.  POS tags have depth 1. Phrasal nodes have
        /// depth >= 2.
        /// </summary>
        public int Depth()
        {
            if (IsLeaf())
            {
                return 0;
            }
            int maxDepth = 0;
            Tree[] kids = Children();
            foreach (Tree kid in kids)
            {
                int curDepth = kid.Depth();
                if (curDepth > maxDepth)
                {
                    maxDepth = curDepth;
                }
            }
            return maxDepth + 1;
        }

        /// <summary>
        /// Finds the distance from this node to the specified node.
        /// return -1 if this is not an ancestor of node.
        /// </summary>
        /// <param name="node">A subtree contained in this tree</param>
        public int Depth(Tree node)
        {
            Tree p = node.Parent(this);
            if (this == node)
            {
                return 0;
            }
            if (p == null)
            {
                return -1;
            }
            int depth = 1;
            while (this != p)
            {
                p = p.Parent(this);
                depth++;
            }
            return depth;
        }

        /// <summary>
        /// Returns the tree leaf that is the head of the tree.
        /// </summary>
        /// <param name="hf">The head-finding algorithm to use</param>
        /// <param name="parent">The parent of this tree</param>
        /// <returns>The head tree leaf if any, else <code>null</code></returns>
        public Tree HeadTerminal(IHeadFinder hf, Tree parent)
        {
            if (IsLeaf())
            {
                return this;
            }
            Tree head = hf.DetermineHead(this, parent);
            if (head != null)
            {
                return head.HeadTerminal(hf, parent);
            }
            return null;
        }

        /// <summary>
        /// Returns the tree leaf that is the head of the tree
        /// </summary>
        /// <param name="hf">The headfinding algorithm to use</param>
        /// <returns>The head tree leaf if any, else <code>null</code></returns>
        public Tree HeadTerminal(IHeadFinder hf)
        {
            return HeadTerminal(hf, null);
        }

        /// <summary>
        /// Returns the preterminal tree that is the head of the tree.
        /// See {@link #isPreTerminal()} for
        /// the definition of a preterminal node. Beware that some tree nodes may
        /// have no preterminal head.
        /// </summary>
        /// <param name="hf">The headfinding algorithm to use</param>
        /// <returns>The head preterminal tree, if any, else <code>null</code></returns>
        /// <exception cref="ArgumentException">if called on a leaf node</exception>
        public Tree HeadPreTerminal(IHeadFinder hf)
        {
            if (IsPreTerminal())
            {
                return this;
            }
            else if (IsLeaf())
            {
                throw new ArgumentException("Called headPreTerminal on a leaf: " + this);
            }
            else
            {
                Tree head = hf.DetermineHead(this);
                if (head != null)
                {
                    return head.HeadPreTerminal(hf);
                }
                return null;
            }
        }

        /// <summary>
        /// Finds the head words of each tree and assigns HeadWordAnnotation
        /// to each node pointing to the correct node.  This relies on the
        /// nodes being CoreLabels, so it throws an IllegalArgumentException
        /// if this is ever not true.
        /// </summary>
        public void PercolateHeadAnnotations(IHeadFinder hf)
        {
            if (!(Label() is CoreLabel))
            {
                throw new ArgumentException("Expected CoreLabels in the trees");
            }
            var nodeLabel = (CoreLabel) Label();

            if (IsLeaf())
            {
                return;
            }

            if (IsPreTerminal())
            {
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadWordAnnotation), Children()[0]);
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadTagAnnotation), this);
                return;
            }

            foreach (Tree kid in Children())
            {
                kid.PercolateHeadAnnotations(hf);
            }

           
            Tree head = hf.DetermineHead(this);
            if (head == null)
            {
                throw new NullReferenceException("HeadFinder " + hf + " returned null for " + this);
            }
            else if (head.IsLeaf())
            {
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadWordAnnotation), head);
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadTagAnnotation), head.Parent(this));
            }
            else if (head.IsPreTerminal())
            {
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadWordAnnotation), head.Children()[0]);
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadTagAnnotation), head);
            }
            else
            {
                if (!(head.Label() is CoreLabel))
                {
                    throw new SystemException("Horrible bug");
                }
                var headLabel = (CoreLabel) head.Label();
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadWordAnnotation),
                    headLabel.Get(typeof (TreeCoreAnnotations.HeadWordAnnotation)));
                nodeLabel.Set(typeof (TreeCoreAnnotations.HeadTagAnnotation),
                    headLabel.Get(typeof (TreeCoreAnnotations.HeadTagAnnotation)));
            }
        }
        
        /// <summary>
        /// Finds the heads of the tree.  This code assumes that the label
        /// does store and return sensible values for the category, word, and tag.
        /// It will be a no-op otherwise.  The tree is modified.  The routine
        /// assumes the Tree has word leaves and tag preterminals, and copies
        /// their category to word and tag respectively, if they have a null value.
        /// </summary>
        /// <param name="hf">The headfinding algorithm to use</param>
        public virtual void PercolateHeads(IHeadFinder hf)
        {
            ILabel nodeLabel = Label();
            if (IsLeaf())
            {
                // Sanity check: word() is usually set by the TreeReader.
                if (nodeLabel is IHasWord)
                {
                    var w = (IHasWord) nodeLabel;
                    if (w.GetWord() == null)
                    {
                        w.SetWord(nodeLabel.Value());
                    }
                }

            }
            else
            {
                foreach (Tree kid in Children())
                {
                    kid.PercolateHeads(hf);
                }

                Tree head = hf.DetermineHead(this);
                if (head != null)
                {
                    ILabel headLabel = head.Label();

                    // Set the head tag.
                    string headTag = (headLabel is IHasTag) ? ((IHasTag) headLabel).Tag() : null;
                    if (headTag == null && head.IsLeaf())
                    {
                        // below us is a leaf
                        headTag = nodeLabel.Value();
                    }

                    // Set the head word
                    string headWord = (headLabel is IHasWord) ? ((IHasWord) headLabel).GetWord() : null;
                    if (headWord == null && head.IsLeaf())
                    {
                        // below us is a leaf
                        // this might be useful despite case for leaf above in
                        // case the leaf label type doesn't support word()
                        headWord = headLabel.Value();
                    }

                    // Set the head index
                    int headIndex = (headLabel is IHasIndex) ? ((IHasIndex) headLabel).Index() : -1;

                    if (nodeLabel is IHasWord)
                    {
                        ((IHasWord) nodeLabel).SetWord(headWord);
                    }
                    if (nodeLabel is IHasTag)
                    {
                        ((IHasTag) nodeLabel).SetTag(headTag);
                    }
                    if (nodeLabel is IHasIndex && headIndex >= 0)
                    {
                        ((IHasIndex) nodeLabel).SetIndex(headIndex);
                    }

                }
            }
        }

        /// <summary>
        /// Return a Set of TaggedWord-TaggedWord dependencies, represented as
        /// Dependency objects, for the Tree.  This will only give
        /// useful results if the internal tree node labels support HasWord and
        /// HasTag, and head percolation has already been done (see percolateHeads()).
        /// </summary>
        /// <returns>Set of dependencies (each a Dependency)</returns>
        public Set<IDependency<ILabel, ILabel, Object>> Dependencies()
        {
            return Dependencies(Filters.AcceptFilter<IDependency<ILabel, ILabel, Object>>());
        }

        public Set<IDependency<ILabel, ILabel, Object>> Dependencies(Predicate<IDependency<ILabel, ILabel, Object>> f)
        {
            return Dependencies(f, true, true, false);
        }

        /// <summary>
        /// Convert a constituency label to a dependency label. Options are provided for selecting annotations to copy.
        /// </summary>
        private static ILabel MakeDependencyLabel(ILabel oldLabel, bool copyLabel, bool copyIndex, bool copyPosTag)
        {
            if (! copyLabel)
                return oldLabel;

            string wordForm = (oldLabel is IHasWord) ? ((IHasWord) oldLabel).GetWord() : oldLabel.Value();
            ILabel newLabel = oldLabel.LabelFactory().NewLabel(wordForm);
            if (newLabel is IHasWord) ((IHasWord) newLabel).SetWord(wordForm);
            if (copyPosTag && newLabel is IHasTag && oldLabel is IHasTag)
            {
                string tag = ((IHasTag) oldLabel).Tag();
                ((IHasTag) newLabel).SetTag(tag);
            }
            if (copyIndex && newLabel is IHasIndex && oldLabel is IHasIndex)
            {
                int index = ((IHasIndex) oldLabel).Index();
                ((IHasIndex) newLabel).SetIndex(index);
            }

            return newLabel;
        }

        /// <summary>
        /// Return a set of TaggedWord-TaggedWord dependencies, represented as
        /// Dependency objects, for the Tree.  This will only give
        /// useful results if the internal tree node labels support HasWord and
        /// head percolation has already been done (see percolateHeads()).
        /// </summary>
        /// <param name="f">
        /// Dependencies are excluded for which the Dependency is not accepted by the Filter
        /// </param>
        /// <returns>Set of dependencies (each a Dependency)</returns>
        public Set<IDependency<ILabel, ILabel, Object>> Dependencies(Predicate<IDependency<ILabel, ILabel, Object>> f,
            bool isConcrete, bool copyLabel, bool copyPosTag)
        {
            var deps = new Set<IDependency<ILabel, ILabel, object>>();
            foreach (Tree node in this)
            {
                // Skip leaves and unary re-writes
                if (node.IsLeaf() || node.Children().Length < 2)
                {
                    continue;
                }
                // Create the head label (percolateHeads has already been executed)
                ILabel headLabel = MakeDependencyLabel(node.Label(), copyLabel, isConcrete, copyPosTag);
                string headWord = ((IHasWord) headLabel).GetWord();
                if (headWord == null)
                {
                    headWord = headLabel.Value();
                }
                int headIndex = (isConcrete && (headLabel is IHasIndex)) ? ((IHasIndex) headLabel).Index() : -1;

                // every child with a different (or repeated) head is an argument
                bool seenHead = false;
                foreach (Tree child in node.Children())
                {
                    ILabel depLabel = MakeDependencyLabel(child.Label(), copyLabel, isConcrete, copyPosTag);
                    string depWord = ((IHasWord) depLabel).GetWord();
                    if (depWord == null)
                    {
                        depWord = depLabel.Value();
                    }
                    int depIndex = (isConcrete && (depLabel is IHasIndex)) ? ((IHasIndex) depLabel).Index() : -1;

                    if (!seenHead && headIndex == depIndex && headWord.Equals(depWord))
                    {
                        seenHead = true;
                    }
                    else
                    {
                        IDependency<ILabel, ILabel, Object> dependency = (isConcrete && depIndex != headIndex)
                            ? new UnnamedConcreteDependency(headLabel, depLabel)
                            : new UnnamedDependency(headLabel, depLabel);

                        if (f(dependency))
                        {
                            deps.Add(dependency);
                        }
                    }
                }
            }
            return deps;
        }

        /// <summary>
        /// Return a set of Label-Label dependencies, represented as
        /// Dependency objects, for the Tree.  The Labels are the ones of the leaf
        /// nodes of the tree, without mucking with them.
        /// </summary>
        /// <param name="f">
        /// Dependencies are excluded for which the Dependency is not accepted by the Filter
        /// </param>
        /// <param name="hf">
        /// The HeadFinder to use to identify the head of constituents.
        /// The code assumes that it can use <code>headPreTerminal(hf)</code> to find a
        /// tag and word to make a CoreLabel.
        /// </param>
        /// <returns>
        /// Set of dependencies (each a <code>Dependency</code> between two
        /// <code>CoreLabel</code>s, which each contain a tag(), word(),
        /// and value(), the last two of which are identical).
        /// </returns>
        public Set<IDependency<ILabel, ILabel, Object>> MapDependencies(Predicate<IDependency<ILabel, ILabel, Object>> f,
            IHeadFinder hf)
        {
            if (hf == null)
            {
                throw new ArgumentException("mapDependencies: need HeadFinder");
            }
            Set<IDependency<ILabel, ILabel, Object>> deps = new Util.HashSet<IDependency<ILabel, ILabel, object>>();
            foreach (Tree node in this)
            {
                if (node.IsLeaf() || node.Children().Length < 2)
                {
                    continue;
                }
                // Label l = node.label();
                //Tree hwt = node.headPreTerminal(hf);
                Tree hwt = node.HeadTerminal(hf);
                if (hwt == null)
                {
                    throw new InvalidDataException("mapDependencies: HeadFinder failed!");
                }

                foreach (Tree child in node.Children())
                {
                    // Label dl = child.label();
                    // Tree dwt = child.headPreTerminal(hf);
                    Tree dwt = child.HeadTerminal(hf);
                    if (dwt == null)
                    {
                        throw new InvalidDataException("mapDependencies: HeadFinder failed!");
                    }
                    if (dwt != hwt)
                    {
                        IDependency<ILabel, ILabel, Object> p = new UnnamedDependency(hwt.Label(), dwt.Label());
                        if (f(p))
                        {
                            deps.Add(p);
                        }
                    }
                }
            }
            return deps;
        }

        /// <summary>
        /// Return a set of Label-Label dependencies, represented as
        /// Dependency objects, for the Tree.  The Labels are the ones of the leaf
        /// nodes of the tree, without mucking with them. The head of the sentence is a
        /// dependent of a synthetic "root" label.
        /// </summary>
        /// <param name="f">Dependencies are excluded for which the Dependency is not accepted by the Filter</param>
        /// <param name="hf">
        /// The HeadFinder to use to identify the head of constituents.
        /// The code assumes that it can use <code>headPreTerminal(hf)</code> to find a tag and word to make a CoreLabel.
        /// </param>
        /// <param name="rootName">rootName Name of the root node</param>
        /// <returns>
        /// Set of dependencies (each a <code>Dependency</code> between two
        /// <code>CoreLabel</code>s, which each contain a tag(), word(),
        /// and value(), the last two of which are identical).
        /// </returns>
        public Set<IDependency<ILabel, ILabel, Object>> MapDependencies(Predicate<IDependency<ILabel, ILabel, Object>> f,
            IHeadFinder hf, string rootName)
        {
            Set<IDependency<ILabel, ILabel, Object>> deps = MapDependencies(f, hf);
            if (rootName != null)
            {
                ILabel hl = HeadTerminal(hf).Label();
                var rl = new CoreLabel();
                rl.Set(typeof (CoreAnnotations.TextAnnotation), rootName);
                rl.Set(typeof (CoreAnnotations.IndexAnnotation), -1);
                deps.Add(new NamedDependency(rl, hl, rootName));
            }
            return deps;
        }

        /// <summary>
        /// Gets the yield of the tree.  The <code>Label</code> of all leaf nodes
        /// is returned as a list ordered by the natural left to right order of the
        /// leaves.  Null values, if any, are inserted into the list like any other value.
        /// </summary>
        /// <returns>a <code>List</code> of the data in the tree's leaves</returns>
        public List<ILabel> Yield()
        {
            return Yield(new List<ILabel>());
        }

        /// <summary>
        /// Gets the yield of the tree.  The <code>Label</code> of all leaf nodes
        /// is returned as a list ordered by the natural left to right order of the
        /// leaves.  Null values, if any, are inserted into the list like any other value.
        /// 
        /// <i>Implementation notes:</i> c. 2003: This has been rewritten to thread, so only one List
        /// is used. 2007: This method was duplicated to start to give type safety to Sentence.
        /// This method will now make a Word for any Leaf which does not itself implement HasWord, and
        /// put the Word into the Sentence, so the Sentence elements MUST implement HasWord.
        /// </summary>
        /// <param name="y">
        /// The list in which the yield of the tree will be placed.
        /// Normally, this will be empty when the routine is called, 
        /// but if not, the new yield is added to the end of the list.
        /// </param>
        /// <returns>a <code>List</code> of the data in the tree's leaves.</returns>
        public List<ILabel> Yield(List<ILabel> y)
        {
            if (IsLeaf())
            {
                y.Add(Label());

            }
            else
            {
                Tree[] kids = Children();
                foreach (Tree kid in kids)
                {
                    kid.Yield(y);
                }
            }
            return y;
        }

        public List<Word> YieldWords()
        {
            return YieldWords(new List<Word>());
        }

        public List<Word> YieldWords(List<Word> y)
        {
            if (IsLeaf())
            {
                y.Add(new Word(Label()));
            }
            else
            {
                foreach (Tree kid in Children())
                {
                    kid.YieldWords(y);
                }
            }
            return y;
        }

        public /*<X extends HasWord>*/ List<IHasWord> YieldHasWord()
        {
            return YieldHasWord(new List<IHasWord>());
        }

        public /*<X extends HasWord>*/ List<IHasWord> YieldHasWord(List<IHasWord> y)
        {
            if (IsLeaf())
            {
                ILabel lab = Label();
                // cdm: this is new hacked in stuff in Mar 2007 so we can now have a
                // well-typed version of a Sentence, whose objects MUST implement HasWord
                //
                // wsg (Feb. 2010) - More hacks for trees with CoreLabels in which the type implements
                // HasWord but only the value field is populated. This can happen if legacy code uses
                // LabeledScoredTreeFactory but passes in a StringLabel to e.g. newLeaf().
                if (lab is IHasWord)
                {
                    if (lab is CoreLabel)
                    {
                        var cl = (CoreLabel) lab;
                        if (cl.GetWord() == null)
                            cl.SetWord(cl.Value());
                        y.Add((IHasWord) cl);
                    }
                    else
                    {
                        y.Add((IHasWord) lab);
                    }

                }
                else
                {
                    y.Add((IHasWord) new Word(lab));
                }

            }
            else
            {
                Tree[] kids = Children();
                foreach (Tree kid in kids)
                {
                    kid.Yield(y);
                }
            }
            return y;
        }

        /// <summary>
        /// Gets the yield of the tree.  The <code>Label</code> of all leaf nodes is returned
        /// as a list ordered by the natural left to right order of the
        /// leaves.  Null values, if any, are inserted into the list like any
        /// other value.  This has been rewritten to thread, so only one List is used.
        /// </summary>
        /// <param name="y">
        /// The list in which the yield of the tree will be placed.
        /// Normally, this will be empty when the routine is called, 
        /// but if not, the new yield is added to the end of the list.
        /// </param>
        /// <returns>a <code>List</code> of the data in the tree's leaves.</returns>
        public /*<T>*/ List<T> Yield<T>(List<T> y)
        {
            if (IsLeaf())
            {
                if (Label() is IHasWord)
                {
                    var hw = (IHasWord) Label();
                    hw.SetWord(Label().Value());
                }
                y.Add((T) Label());

            }
            else
            {
                Tree[] kids = Children();
                foreach (Tree kid in kids)
                {
                    kid.Yield(y);
                }
            }
            return y;
        }

        /// <summary>
        /// Gets the tagged yield of the tree.
        /// The <code>Label</code> of all leaf nodes is returned
        /// as a list ordered by the natural left to right order of the
        /// leaves.  Null values, if any, are inserted into the list like any other value.
        /// </summary>
        /// <returns>a <code>List</code> of the data in the tree's leaves</returns>
        public List<Ling.TaggedWord> TaggedYield()
        {
            return TaggedYield(new List<Ling.TaggedWord>());
        }

        public List<LabeledWord> LabeledYield()
        {
            return LabeledYield(new List<LabeledWord>());
        }

        /// <summary>
        /// Gets the tagged yield of the tree -- that is, get the preterminals
        /// as well as the terminals.  The <code>Label</code> of all leaf nodes
        /// is returned as a list ordered by the natural left to right order of the
        /// leaves.  Null values, if any, are inserted into the list like any
        /// other value.  This has been rewritten to thread, so only one List is used.
        /// 
        /// <i>Implementation note:</i> when we summon up enough courage, this
        /// method will be changed to take and return a List of TaggedWord.
        /// </summary>
        /// <param name="ty">
        /// The list in which the tagged yield of the tree will be placed. 
        /// Normally, this will be empty when the routine is called, 
        /// but if not, the new yield is added to the end of the list.
        /// </param>
        /// <returns>a <code>List</code> of the data in the tree's leaves</returns>
        public /*<X extends List<TaggedWord>>*/ List<Ling.TaggedWord> TaggedYield(List<Ling.TaggedWord> ty)
        {
            Tree[] kids = Children();
            // this inlines the content of isPreTerminal()
            if (kids.Length == 1 && kids[0].IsLeaf())
            {
                ty.Add(new Ling.TaggedWord(kids[0].Label(), Label()));
            }
            else
            {
                foreach (Tree kid in kids)
                {
                    kid.TaggedYield(ty);
                }
            }
            return ty;
        }

        public List<LabeledWord> LabeledYield(List<LabeledWord> ty)
        {
            Tree[] kids = Children();
            // this inlines the content of isPreTerminal()
            if (kids.Length == 1 && kids[0].IsLeaf())
            {
                ty.Add(new LabeledWord(kids[0].Label(), Label()));
            }
            else
            {
                foreach (Tree kid in kids)
                {
                    kid.LabeledYield(ty);
                }
            }
            return ty;
        }

        public List<CoreLabel> TaggedLabeledYield()
        {
            var ty = new List<CoreLabel>();
            TaggedLabeledYield(ty, -1);
            return ty;
        }

        private int TaggedLabeledYield(List<CoreLabel> ty, int termIdx)
        {
            if (IsPreTerminal())
            {
                var taggedWord = new CoreLabel();
                string tag = Value() ?? "";
                taggedWord.SetValue(tag);
                taggedWord.SetTag(tag);
                taggedWord.SetIndex(termIdx);
                taggedWord.SetWord(FirstChild().Value());
                ty.Add(taggedWord);

                return termIdx + 1;

            }
            else
            {
                foreach (Tree kid in GetChildrenAsList())
                {
                    termIdx = kid.TaggedLabeledYield(ty, termIdx);
                }
            }

            return termIdx;
        }

        /// <summary>
        /// Gets the preterminal yield (i.e., tags) of the tree.
        /// All data in preterminal nodes is returned as a list ordered by the natural left to right order of the tree.
        /// Null values, if any, are inserted into the list like any other value.  Pre-leaves are nodes of height 1.
        /// </summary>
        /// <returns>a {@code List} of the data in the tree's pre-leaves</returns>
        public List<ILabel> PreTerminalYield()
        {
            return PreTerminalYield(new List<ILabel>());
        }

        /// <summary>
        /// Gets the preterminal yield (i.e., tags) of the tree.
        /// All data in preleaf nodes is returned as a list ordered by the natural left to right order of the tree.
        /// Null values, if any, are inserted into the list like any other value.  Pre-leaves are nodes of height 1.
        /// </summary>
        /// <param name="y">
        /// The list in which the preterminals of the tree will be placed.
        /// Normally, this will be empty when the routine is called, 
        /// but if not, the new yield is added to the end of the list.
        /// </param>
        /// <returns>a <code>List</code> of the data in the tree's pre-leaves</returns>
        public List<ILabel> PreTerminalYield(List<ILabel> y)
        {
            if (IsPreTerminal())
            {
                y.Add(Label());
            }
            else
            {
                Tree[] kids = Children();
                foreach (Tree kid in kids)
                {
                    kid.PreTerminalYield(y);
                }
            }
            return y;
        }

        /// <summary>
        /// Gets the leaves of the tree.
        /// All leaves nodes are returned as a list ordered by the natural left to right order of the tree.
        /// Null values, if any, are inserted into the list like any other value.
        /// </summary>
        /// <returns>a <code>List</code> of the leaves</returns>
        public /*<T extends Tree>*/ List<Tree> GetLeaves()
        {
            return GetLeaves(new List<Tree>());
        }

        /// <summary>
        /// Gets the leaves of the tree.
        /// </summary>
        /// <param name="list">
        /// The list in which the leaves of the tree will be placed.
        /// Normally, this will be empty when the routine is called,
        /// but if not, the new yield is added to the end of the list.
        /// </param>
        /// <returns>a <code>List</code> of the leaves</returns>
        public /*<T extends Tree>*/ List<Tree> GetLeaves(List<Tree> list)
        {
            if (IsLeaf())
            {
                list.Add((Tree) this);
            }
            else
            {
                foreach (Tree kid in Children())
                {
                    kid.GetLeaves(list);
                }
            }
            return list;
        }

        /// <summary>
        /// Get the set of all node and leaf {@code Label}s,
        /// null or otherwise, contained in the tree.
        /// </summary>
        /// <returns>
        /// The {@code Collection} (actually, Set) of all values in the tree.
        /// </returns>
        public ICollection<ILabel> Labels()
        {
            var n = new Set<ILabel> {Label()};
            Tree[] kids = Children();
            foreach (Tree kid in kids)
            {
                n.AddAll(kid.Labels());
            }
            return n;
        }

        public void SetLabels(ICollection<ILabel> c)
        {
            throw new InvalidOperationException("Can't set Tree labels");
        }

        /// <summary>
        /// Return a flattened version of a tree.  In many circumstances, this
        /// will just return the tree, but if the tree is something like a
        /// binarized version of a dependency grammar tree, then it will be
        /// flattened back to a dependency grammar tree representation.  Formally,
        /// a node will be removed from the tree when: it is not a terminal or
        /// preterminal, and its <code>label()</code> is <code>equal()</code> to
        /// the <code>label()</code> of its parent, and all its children will
        /// then be promoted to become children of the parent (in the same
        /// position in the sequence of daughters.
        /// </summary>
        public Tree Flatten()
        {
            return Flatten(TreeFactory());
        }

        /// <summary>
        /// Return a flattened version of a tree.  In many circumstances, this
        /// will just return the tree, but if the tree is something like a
        /// binarized version of a dependency grammar tree, then it will be
        /// flattened back to a dependency grammar tree representation.  Formally,
        /// a node will be removed from the tree when: it is not a terminal or
        /// preterminal, and its <code>label()</code> is <code>equal()</code> to
        /// the <code>label()</code> of its parent, and all its children will
        /// then be promoted to become children of the parent (in the same
        /// position in the sequence of daughters.
        /// 
        /// Note: In the current implementation, the tree structure is mainly
        /// duplicated, but the links between preterminals and terminals aren't.
        /// </summary>
        public Tree Flatten(ITreeFactory tf)
        {
            if (IsLeaf() || IsPreTerminal())
            {
                return this;
            }
            Tree[] kids = Children();
            var newChildren = new List<Tree>(kids.Length);
            foreach (Tree child in kids)
            {
                if (child.IsLeaf() || child.IsPreTerminal())
                {
                    newChildren.Add(child);
                }
                else
                {
                    Tree newChild = child.Flatten(tf);
                    if (Label().Equals(newChild.Label()))
                    {
                        newChildren.AddRange(newChild.GetChildrenAsList());
                    }
                    else
                    {
                        newChildren.Add(newChild);
                    }
                }
            }
            return tf.NewTreeNode(Label(), newChildren);
        }

        /// <summary>
        /// Get the set of all subtrees inside the tree by returning a tree
        /// rooted at each node.  These are <i>not</i> copies, but all share
        /// structure.  The tree is regarded as a subtree of itself.
        /// 
        /// Note: If you only want to form this Set so that you can
        /// iterate over it, it is more efficient to simply use the Tree class's
        /// own iterator() method. This will iterate over the exact same
        /// elements (but perhaps/probably in a different order).
        /// </summary>
        /// <returns>the <code>Set</code> of all subtrees in the tree</returns>
        public Set<Tree> SubTrees()
        {
            return SubTrees(new Util.HashSet<Tree>());
        }

        /// <summary>
        /// Get the list of all subtrees inside the tree by returning a tree
        /// rooted at each node.  These are <i>not</i> copies, but all share
        /// structure.  The tree is regarded as a subtree of itself.
        /// 
        /// Note: If you only want to form this Collection so that you can
        /// iterate over it, it is more efficient to simply use the Tree class's
        /// own iterator() method. This will iterate over the exact same
        /// elements (but perhaps/probably in a different order).
        /// </summary>
        /// <returns>the <code>List</code> of all subtrees in the tree.</returns>
        public List<Tree> SubTreeList()
        {
            return SubTrees(new List<Tree>());
        }

        /// <summary>
        /// Add the set of all subtrees inside a tree (including the tree itself)
        /// to the given <code>Collection</code>.
        /// 
        /// Note: If you only want to form this Collection so that you can
        /// iterate over it, it is more efficient to simply use the Tree class's
        /// own iterator() method. This will iterate over the exact same
        /// elements (but perhaps/probably in a different order).
        /// </summary>
        /// <param name="n">A collection of nodes to which the subtrees will be added.</param>
        /// <returns>The collection parameter with the subtrees added.</returns>
        public /*<T extends Collection<Tree>>*/ T SubTrees<T>(T n) where T : ICollection<Tree>
        {
            n.Add(this);
            Tree[] kids = Children();
            foreach (Tree kid in kids)
            {
                kid.SubTrees(n);
            }
            return n;
        }

        /// <summary>
        /// Makes a deep copy of not only the Tree structure but of the labels as well.
        /// Uses the TreeFactory of the root node given by treeFactory().
        /// Assumes that your labels give a non-null labelFactory().
        /// </summary>
        /// <returns>A deep copy of the tree structure and its labels</returns>
        public Tree DeepCopy()
        {
            return DeepCopy(TreeFactory());
        }

        /// <summary>
        /// Makes a deep copy of not only the Tree structure but of the labels as well.
        /// The new tree will have nodes made by the given TreeFactory.
        /// Each Label is copied using the labelFactory() returned
        /// by the corresponding node's label.
        /// It assumes that your labels give non-null labelFactory.
        /// </summary>
        /// <param name="tf">The TreeFactory used to make all nodes in the copied tree structure</param>
        /// <returns>A Tree that is a deep copy of the tree structure and Labels of the original tree.</returns>
        public Tree DeepCopy(ITreeFactory tf)
        {
            return DeepCopy(tf, Label().LabelFactory());
        }

        /// <summary>
        /// Makes a deep copy of not only the Tree structure but of the labels as well.
        /// Each tree is copied with the given TreeFactory.
        /// Each Label is copied using the given LabelFactory.
        /// That is, the tree and label factories can transform the nature of the data representation.
        /// </summary>
        /// <param name="tf">The TreeFactory used to make all nodes in the copied tree structure</param>
        /// <param name="lf">The LabelFactory used to make all nodes in the copied tree structure</param>
        /// <returns>A Tree that is a deep copy of the tree structure and Labels of the original tree.</returns>
        public Tree DeepCopy(ITreeFactory tf, ILabelFactory lf)
        {
            ILabel lab = lf.NewLabel(Label());
            if (IsLeaf())
            {
                return tf.NewLeaf(lab);
            }
            Tree[] kids = Children();
            // NB: The below list may not be of type Tree but TreeGraphNode, so we leave it untyped
            var newKids = new List<Tree>(kids.Length);
            foreach (Tree kid in kids)
            {
                newKids.Add(kid.DeepCopy(tf, lf));
            }
            return tf.NewTreeNode(lab, newKids);
        }

        /// <summary>
        /// Create a deep copy of the tree structure.
        /// The entire structure is recursively copied, but label data themselves are not cloned.
        /// The copy is built using a <code>TreeFactory</code> that will
        /// produce a <code>Tree</code> like the input one.
        /// </summary>
        /// <returns>A deep copy of the tree structure (but not its labels)</returns>
        public Tree TreeSkeletonCopy()
        {
            return TreeSkeletonCopy(TreeFactory());
        }

        /// <summary>
        /// Create a deep copy of the tree structure.  The entire structure is
        /// recursively copied, but label data themselves are not cloned.
        /// By specifying an appropriate <code>TreeFactory</code>, this
        /// method can be used to change the type of a <code>Tree</code>.
        /// </summary>
        /// <param name="tf">The <code>TreeFactory</code> to be used for creating the returned <code>Tree</code></param>
        /// <returns>A deep copy of the tree structure (but not its labels).</returns>
        public Tree TreeSkeletonCopy(ITreeFactory tf)
        {
            Tree t;
            if (IsLeaf())
            {
                t = tf.NewLeaf(Label());
            }
            else
            {
                Tree[] kids = Children();
                var newKids = new List<Tree>(kids.Length);
                foreach (Tree kid in kids)
                {
                    newKids.Add(kid.TreeSkeletonCopy(tf));
                }
                t = tf.NewTreeNode(Label(), newKids);
            }
            return t;
        }

        /// <summary>
        /// Create a transformed Tree.  The tree is traversed in a depth-first,
        /// left-to-right order, and the <code>TreeTransformer</code> is called
        /// on each node.  It returns some <code>Tree</code>.  The transformed
        /// tree has a new tree structure (i.e., a "deep copy" is done), but it
        /// will usually share its labels with the original tree.
        /// </summary>
        /// <param name="transformer">The function that transforms tree nodes or subtrees</param>
        /// <returns>a transformation of this <code>Tree</code></returns>
        public Tree Transform(ITreeTransformer transformer)
        {
            return Transform(transformer, TreeFactory());
        }

        /// <summary>
        /// Create a transformed Tree.  The tree is traversed in a depth-first,
        /// left-to-right order, and the <code>TreeTransformer</code> is called
        /// on each node.  It returns some <code>Tree</code>.  The transformed
        /// tree has a new tree structure (i.e., a deep copy of the structure of the tree is done), but it
        /// will usually share its labels with the original tree.
        /// </summary>
        /// <param name="transformer">The function that transforms tree nodes or subtrees</param>
        /// <param name="tf">
        /// The <code>TreeFactory</code> which will be used for creating new nodes for the returned <code>Tree</code>
        /// </param>
        /// <returns>a transformation of this <code>Tree</code></returns>
        public Tree Transform(ITreeTransformer transformer,ITreeFactory tf)
        {
            Tree t;
            if (IsLeaf())
            {
                t = tf.NewLeaf(Label());
            }
            else
            {
                Tree[] kids = Children();
                var newKids = new List<Tree>(kids.Length);
                foreach (Tree kid in kids)
                {
                    newKids.Add(kid.Transform(transformer, tf));
                }
                t = tf.NewTreeNode(Label(), newKids);
            }
            return transformer.TransformTree(t);
        }

        /// <summary>
        /// Creates a (partial) deep copy of the tree, where all nodes that the
        /// filter does not accept are spliced out.  If the result is not a tree
        /// (that is, it's a forest), an empty root node is generated.
        /// </summary>
        /// <param name="nodeFilter">a Filter method which returns true to mean keep this node, false to mean delete it</param>
        /// <returns>a filtered copy of the tree</returns>
        public Tree SpliceOut(Predicate<Tree> nodeFilter)
        {
            return SpliceOut(nodeFilter, TreeFactory());
        }

        /// <summary>
        /// Creates a (partial) deep copy of the tree, where all nodes that the
        /// filter does not accept are spliced out.  That is, the particular
        /// modes for which the <code>Filter</code> returns <code>false</code>
        /// are removed from the <code>Tree</code>, but those nodes' children
        /// are kept (assuming they pass the <code>Filter</code>, and they are
        /// added in the appropriate left-to-right ordering as new children of
        /// the parent node.  If the root node is deleted, so that the result
        /// would not be a tree (that is, it's a forest), an empty root node is
        /// generated.  If nothing is accepted, <code>null</code> is returned.
        /// </summary>
        /// <param name="nodeFilter">
        /// a Filter method which returns true to mean keep this node, false to mean delete it
        /// </param>
        /// <param name="tf">
        /// A <code>TreeFactory</code> for making new trees. Used if the root node is deleted.
        /// </param>
        /// <returns>a filtered copy of the tree</returns>
        public Tree SpliceOut(Predicate<Tree> nodeFilter,ITreeFactory tf)
        {
            List<Tree> l = SpliceOutHelper(nodeFilter, tf);
            if (!l.Any())
            {
                return null;
            }
            else if (l.Count == 1)
            {
                return l[0];
            }
            // for a forest, make a new root
            return tf.NewTreeNode((ILabel) null, l);
        }


        private List<Tree> SpliceOutHelper(Predicate<Tree> nodeFilter, ITreeFactory tf)
        {
            // recurse over all children first
            Tree[] kids = Children();
            var l = new List<Tree>();
            foreach (Tree kid in kids)
            {
                l.AddRange(kid.SpliceOutHelper(nodeFilter, tf));
            }
            // check if this node is being spliced out
            if (nodeFilter(this))
            {
                // no, so add our children and return
                Tree t;
                if (l.Any())
                {
                    t = tf.NewTreeNode(Label(), l);
                }
                else
                {
                    t = tf.NewLeaf(Label());
                }
                l = new List<Tree>(1);
                l.Add(t);
                return l;
            }
            // we're out, so return our children
            return l;
        }

        /// <summary>
        /// Creates a deep copy of the tree, where all nodes that the filter
        /// does not accept and all children of such nodes are pruned.
        /// If all of a node's children are pruned, that node is cut as well.
        /// A <code>Filter</code> can assume
        /// that it will not be called with a <code>null</code> argument.
        /// 
        /// For example, the following code excises all PP nodes from a Tree:
        /// <code>
        /// Filter{Tree} f = new Filter{Tree} {
        ///     public bool accept(Tree t) {
        ///         return ! t.label().value().equals(PP);
        ///     }
        /// }; 
        /// tree.prune(f);
        /// </code>
        /// 
        /// If the root of the tree is pruned, null will be returned.
        /// </summary>
        /// <param name="filter">the filter to be applied</param>
        /// <returns>
        /// a filtered copy of the tree, including the possibility of
        /// <code>null</code> if the root node of the tree is filtered
        /// </returns>
        public Tree Prune(Predicate<Tree> filter)
        {
            return Prune(filter, TreeFactory());
        }

        /// <summary>
        /// Creates a deep copy of the tree, where all nodes that the filter
        /// does not accept and all children of such nodes are pruned.
        /// If all of a node's children are pruned, that node is cut as well.
        /// A <code>Filter</code> can assume
        /// that it will not be called with a <code>null</code> argument.
        /// </summary>
        /// <param name="filter">the filter to be applied</param>
        /// <param name="tf">the TreeFactory to be used to make new Tree nodes if needed</param>
        /// <returns>
        /// a filtered copy of the tree, including the possibility 
        /// of <code>null</code> if the root node of the tree is filtered
        /// </returns>
        public Tree Prune(Predicate<Tree> filter, ITreeFactory tf)
        {
            // is the current node to be pruned?
            if (! filter(this))
            {
                return null;
            }
            // if not, recurse over all children
            var l = new List<Tree>();
            Tree[] kids = Children();
            foreach (Tree kid in kids)
            {
                Tree prunedChild = kid.Prune(filter, tf);
                if (prunedChild != null)
                {
                    l.Add(prunedChild);
                }
            }
            // and check if this node has lost all its children
            if (!l.Any() && kids.Length != 0)
            {
                return null;
            }
            // if we're still ok, copy the node
            if (IsLeaf())
            {
                return tf.NewLeaf(Label());
            }
            return tf.NewTreeNode(Label(), l);
        }

        /// <summary>
        /// Returns first child if this is unary and if the label at the current
        /// node is either "ROOT" or empty.
        /// </summary>
        public Tree SkipRoot()
        {
            if (!IsUnaryRewrite())
                return this;
            string lab = Label().Value();
            return (lab == null || !lab.Any() || AbstractCollinsHeadFinder.ROOT.Equals(lab)) ? FirstChild() : this;
        }

        /// <summary>
        /// Return a <code>TreeFactory</code> that produces trees of the appropriate type.
        /// </summary>
        public abstract ITreeFactory TreeFactory();

        /// <summary>
        /// Return the parent of the tree node. 
        /// This routine may return <code>null</code> meaning simply 
        /// that the implementation doesn't know how to determine 
        /// the parent node, rather than there is no such node.
        /// </summary>
        /// <returns>The parent <code>Tree</code> node or <code>null</code></returns>
        public virtual Tree Parent()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Return the parent of the tree node.  This routine will traverse
        /// a tree (depth first) from the given <code>root</code>, and will
        /// correctly find the parent, regardless of whether the concrete
        /// class stores parents.  It will only return <code>null</code> if this
        /// node is the <code>root</code> node, or if this node is not
        /// contained within the tree rooted at <code>root</code>.
        /// </summary>
        /// <param name="root">The root node of the whole Tree</param>
        /// <returns>
        /// the parent <code>Tree</code> node if any; else <code>null</code>
        /// </returns>
        public Tree Parent(Tree root)
        {
            Tree[] kids = root.Children();
            return ParentHelper(root, kids, this);
        }


        private static Tree ParentHelper(Tree parent, Tree[] kids, Tree node)
        {
            foreach (Tree kid in kids)
            {
                if (kid == node)
                {
                    return parent;
                }
                Tree ret = node.Parent(kid);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the number of nodes the tree contains.
        /// This method implements the <code>size()</code> function required by the
        /// <code>Collections</code> interface.  The size of the tree is the
        /// number of nodes it contains (of all types, including the leaf nodes and the root).
        /// </summary>
        /// <returns>The size of the tree</returns>
        public int Size()
        {
            int size = 1;
            Tree[] kids = Children();
            foreach (Tree kid in kids)
            {
                size += kid.Size();
            }
            return size;
        }

        /// <summary>
        /// Return the ancestor tree node <code>height</code> nodes up from the current node.
        /// </summary>
        /// <param name="height">
        /// How many nodes up to go. A parameter of 0 means return this node, 
        /// 1 means to return the parent node and so on.
        /// </param>
        /// <param name="root">The root node that this Tree is embedded under</param>
        /// <returns>
        /// The ancestor at height <code>height</code>.  It returns null
        /// if it does not exist or the tree implementation does not keep track of parents
        /// </returns>
        public Tree Ancestor(int height, Tree root)
        {
            if (height < 0)
            {
                throw new ArgumentException("ancestor: height cannot be negative");
            }
            if (height == 0)
            {
                return this;
            }
            Tree par = Parent(root);
            if (par == null)
            {
                return null;
            }
            return par.Ancestor(height - 1, root);
        }


        /*private static class TreeIterator : Iterator<Tree> {

            private readonly List<Tree> treeStack;

            protected TreeIterator(Tree t) {
              treeStack = new List<Tree>();
              treeStack.Add(t);
            }

            public bool hasNext() {
              return (treeStack.Any());
            }

            public Tree next() {
              int lastIndex = treeStack.Count - 1;
              if (lastIndex < 0) {
                throw new NoSuchElementException("TreeIterator exhausted");
              }
              Tree tr = treeStack.Remove(lastIndex);
              Tree[] kids = tr.children();
              // so that we can efficiently use one List, we reverse them
              for (int i = kids.Length - 1; i >= 0; i--) {
                treeStack.Add(kids[i]);
              }
              return tr;
            }

            /**
             * Not supported
             #1#
            public void remove() {
              throw new InvalidOperationException();
            }

            public override string ToString() {
              return "TreeIterator";
            }

          }*/

        public class TreeIterator : IEnumerator<Tree>
        {

            private readonly List<Tree> treeStack;

            public TreeIterator(Tree t)
            {
                treeStack = new List<Tree>();
                treeStack.Add(t);
            }
            
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                if (!treeStack.Any())
                {
                    this.Current = null;
                    return false;
                }
                else
                {
                    this.Current = treeStack.Last();
                    treeStack.Remove(this.Current);
                    Tree[] kids = this.Current.Children();
                    // so that we can efficiently use one List, we reverse them
                    for (int i = kids.Length - 1; i >= 0; i--)
                    {
                        treeStack.Add(kids[i]);
                    }
                    return true;
                }
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


        public override IEnumerator<Tree> GetEnumerator()
        {
            return new TreeIterator(this);
        }

        /// <summary>
        /// Returns an iterator over all the nodes of the tree.
        /// This method implements the <code>iterator()</code> method required by the
        /// <code>Collections</code> interface.  It does a preorder
        /// (children after node) traversal of the tree.  (A possible
        /// extension to the class at some point would be to allow different
        /// traversal orderings via variant iterators.)
        /// </summary>
        /// <returns>An iterator over the nodes of the tree</returns>
        public TreeIterator Iterator()
        {
            return new TreeIterator(this);
        }

        public List<Tree> PostOrderNodeList()
        {
            var nodes = new List<Tree>();
            PostOrderRecurse(this, nodes);
            return nodes;
        }

        private static void PostOrderRecurse(Tree t, List<Tree> nodes)
        {
            foreach (Tree c in t.Children())
            {
                PostOrderRecurse(c, nodes);
            }
            nodes.Add(t);
        }

        public List<Tree> PreOrderNodeList()
        {
            var nodes = new List<Tree>();
            PreOrderRecurse(this, nodes);
            return nodes;
        }

        private static void PreOrderRecurse(Tree t, List<Tree> nodes)
        {
            nodes.Add(t);
            foreach (Tree c in t.Children())
            {
                PreOrderRecurse(c, nodes);
            }
        }

        /// <summary>
        /// This gives you a tree from a string representation 
        /// (as a bracketed Tree, of the kind produced by <code>ToString()</code>,
        /// <code>pennPrint()</code>, or as in the Penn Treebank).
        /// It's not the most efficient thing to do for heavy duty usage.
        /// The Tree returned is created by a
        /// LabeledScoredTreeReaderFactory. This means that "standard"
        /// normalizations (stripping functional categories, indices,
        /// empty nodes, and A-over-A nodes) will be done on it.
        /// </summary>
        /// <param name="str">The tree as a bracketed list in a string</param>
        public static Tree ValueOf(string str)
        {
            return ValueOf(str, new LabeledScoredTreeReaderFactory());
        }

        /// <summary>
        /// This gives you a tree from a string representation (as a
        /// bracketed Tree, of the kind produced by <code>ToString()</code>,
        /// <code>pennPrint()</code>, or as in the Penn Treebank.
        /// It's not the most efficient thing to do for heavy duty usage.
        /// </summary>
        /// <param name="str">The tree as a bracketed list in a string.</param>
        /// <param name="trf">The TreeFactory used to make the new Tree</param>
        /// <returns>The Tree</returns>
        /// <exception cref="SystemException">If the Tree format is not valid</exception>
        public static Tree ValueOf(string str, ITreeReaderFactory trf)
        {
            try
            {
                return trf.NewTreeReader(new StringReader(str)).ReadTree();
            }
            catch (IOException ioe)
            {
                throw new SystemException("Tree.valueOf() tree construction failed", ioe);
            }
        }

        /// <summary>
        /// Return the child at some daughter index.
        /// The children are numbered starting with an index of 0.
        /// </summary>
        /// <param name="i">The daughter index</param>
        /// <returns>The tree at that daughter index</returns>
        public Tree GetChild(int i)
        {
            Tree[] kids = Children();
            return kids[i];
        }

        /// <summary>
        /// Destructively removes the child at some daughter index and returns it.
        /// Note that this method will throw an {@link ArrayIndexOutOfBoundsException} if
        /// the daughter index is too big for the list of daughters.
        /// </summary>
        /// <param name="i">The daughter index</param>
        /// <returns>The tree at that daughter index</returns>
        public Tree RemoveChild(int i)
        {
            Tree[] kids = Children();
            Tree kid = kids[i];
            var newKids = new Tree[kids.Length - 1];
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
            SetChildren(newKids);
            return kid;
        }

        /// <summary>
        /// Adds the tree t at the index position among the daughters
        /// Note that this method will throw an {@link ArrayIndexOutOfBoundsException} if
        /// the daughter index is too big for the list of daughters.
        /// </summary>
        /// <param name="i">the index position at which to add the new daughter</param>
        /// <param name="t">the new daughter</param>
        public void AddChild(int i, Tree t)
        {
            Tree[] kids = Children();
            var newKids = new Tree[kids.Length + 1];
            if (i != 0)
            {
                Array.Copy(kids, 0, newKids, 0, i);
            }
            newKids[i] = t;
            if (i != kids.Length)
            {
                Array.Copy(kids, i, newKids, i + 1, kids.Length - i);
            }
            SetChildren(newKids);
        }

        /// <summary>
        /// Adds the tree t at the last index position among the daughters.
        /// </summary>
        /// <param name="t">the new daughter</param>
        public void AddChild(Tree t)
        {
            AddChild(Children().Length, t);
        }

        /// <summary>
        /// Replaces the <code>i</code>th child of <code>this</code> with the tree t.
        /// Note that this method will throw an {@link ArrayIndexOutOfBoundsException} if
        /// the child index is too big for the list of children.
        /// </summary>
        /// <param name="i">The index position at which to replace the child</param>
        /// <param name="t">The new child</param>
        /// <returns>The tree that was previously the ith d</returns>
        public Tree SetChild(int i, Tree t)
        {
            Tree[] kids = Children();
            Tree old = kids[i];
            kids[i] = t;
            return old;
        }

        /// <summary>
        /// Returns true if <code>this</code> dominates the Tree passed in
        /// as an argument.  Object equality (==) rather than .equals() is used
        /// to determine domination.
        /// t.dominates(t) returns false.
        /// </summary>
        public bool Dominates(Tree t)
        {
            List<Tree> dPath = DominationPath(t);
            return dPath != null && dPath.Count > 1;
        }
        
        /// <summary>
        /// Returns the path of nodes leading down to a dominated node,
        /// including <code>this</code> and the dominated node itself.
        /// Returns null if t is not dominated by <code>this</code>.
        /// Object equality (==) is the relevant criterion. t.dominationPath(t) returns null.
        /// </summary>
        public List<Tree> DominationPath(Tree t)
        {
            //Tree[] result = dominationPathHelper(t, 0);
            Tree[] result = DominationPath(t, 0);
            if (result == null)
            {
                return null;
            }
            return result.ToList();
        }

        private Tree[] DominationPathHelper(Tree t, int depth)
        {
            Tree[] kids = Children();
            for (int i = kids.Length - 1; i >= 0; i--)
            {
                Tree t1 = kids[i];
                if (t1 == null)
                {
                    return null;
                }
                Tree[] result;
                if ((result = t1.DominationPath(t, depth + 1)) != null)
                {
                    result[depth] = this;
                    return result;
                }
            }
            return null;
        }

        private Tree[] DominationPath(Tree t, int depth)
        {
            if (this == t)
            {
                var result = new Tree[depth + 1];
                result[depth] = this;
                return result;
            }
            return DominationPathHelper(t, depth);
        }

        /// <summary>
        /// Given nodes <code>t1</code> and <code>t2</code> which are
        /// dominated by this node, returns a list of all the nodes on the
        /// path from t1 to t2, inclusive, or null if none found.
        /// </summary>
        public List<Tree> PathNodeToNode(Tree t1, Tree t2)
        {
            if (!Contains(t1) || !Contains(t2))
            {
                return null;
            }
            if (t1 == t2)
            {
                return new List<Tree>() {t1};
            }
            if (t1.Dominates(t2))
            {
                return t1.DominationPath(t2);
            }
            if (t2.Dominates(t1))
            {
                List<Tree> path1 = t2.DominationPath(t1);
                path1.Reverse();
                return path1;
            }
            Tree jNode = JoinNode(t1, t2);
            if (jNode == null)
            {
                return null;
            }
            List<Tree> t1DomPath = jNode.DominationPath(t1);
            List<Tree> t2DomPath = jNode.DominationPath(t2);
            if (t1DomPath == null || t2DomPath == null)
            {
                return null;
            }
            var path = new List<Tree>();
            path.AddRange(t1DomPath);
            path.Reverse();
            path.Remove(jNode);
            path.AddRange(t2DomPath);
            return path;
        }

        /// <summary>
        /// Given nodes <code>t1</code> and <code>t2</code> which are
        /// dominated by this node, returns their "join node": the node
        /// <code>j</code> such that <code>j</code> dominates both
        /// <code>t1</code> and <code>t2</code>, and every other node which
        /// dominates both <code>t1</code> and <code>t2</code>
        /// dominates <code>j</code>.
        /// In the special case that t1 dominates t2, return t1, and vice versa.
        /// Return <code>null</code> if no such node can be found.
        /// </summary>
        public Tree JoinNode(Tree t1, Tree t2)
        {
            if (!Contains(t1) || !Contains(t2))
            {
                return null;
            }
            if (this == t1 || this == t2)
            {
                return this;
            }
            Tree joinNode = null;
            List<Tree> t1DomPath = DominationPath(t1);
            List<Tree> t2DomPath = DominationPath(t2);
            if (t1DomPath == null || t2DomPath == null)
            {
                return null;
            }
            IEnumerator<Tree> it1 = t1DomPath.GetEnumerator();
            IEnumerator<Tree> it2 = t2DomPath.GetEnumerator();
            while (it1.MoveNext() && it2.MoveNext())
            {
                Tree n1 = it1.Current;
                Tree n2 = it2.Current;
                if (n1 != n2)
                {
                    break;
                }
                joinNode = n1;
            }
            return joinNode;
        }

        /// <summary>
        /// Given nodes {@code t1} and {@code t2} which are
        /// dominated by this node, returns {@code true} iff
        /// {@code t1} c-commands {@code t2}.  (A node c-commands
        /// its sister(s) and any nodes below its sister(s).)
        /// </summary>
        public bool CCommands(Tree t1, Tree t2)
        {
            List<Tree> sibs = t1.Siblings(this);
            if (sibs == null)
            {
                return false;
            }
            foreach (Tree sib in sibs)
            {
                if (sib == t2 || sib.Contains(t2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the siblings of this Tree node.
        /// The siblings are all children of the parent of this node except this node.
        /// </summary>
        /// <param name="root">The root within which this tree node is contained</param>
        /// <returns>
        /// The siblings as a list, an empty list if there are no siblings.
        /// The returned list is a modifiable new list structure, but contains the actual children.
        /// </returns>
        public List<Tree> Siblings(Tree root)
        {
            Tree par = Parent(root);
            if (par == null)
            {
                return null;
            }
            List<Tree> siblings = par.GetChildrenAsList();
            siblings.Remove(this);
            return siblings;
        }

        /// <summary>
        /// Insert <code>dtr</code> after <code>position</code> existing daughters in <code>this</code>.
        /// </summary>
        public void InsertDtr(Tree dtr, int position)
        {
            Tree[] kids = Children();
            if (position > kids.Length)
            {
                throw new ArgumentException("Can't insert tree after the " + position + "th daughter in " + this +
                                            "; only " + kids.Length + " daughters exist!");
            }
            var newKids = new Tree[kids.Length + 1];
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
            SetChildren(newKids);
        }

        // --- composition methods to implement Label interface

        public string Value()
        {
            ILabel lab = Label();
            if (lab == null)
            {
                return null;
            }
            return lab.Value();
        }

        public void SetValue(string value)
        {
            ILabel lab = Label();
            if (lab != null)
            {
                lab.SetValue(value);
            }
        }
        
        public void SetFromString(string labelStr)
        {
            ILabel lab = Label();
            if (lab != null)
            {
                lab.SetFromString(labelStr);
            }
        }

        /// <summary>
        /// Returns a factory that makes labels of the same type as this one.
        /// May return <code>null</code> if no appropriate factory is known.
        /// </summary>
        public ILabelFactory LabelFactory()
        {
            ILabel lab = Label();
            if (lab == null)
            {
                return null;
            }
            return lab.LabelFactory();
        }

        /// <summary>
        /// Returns the positional index of the left edge of  <i>node</i> within the tree,
        /// as measured by characters.  Returns -1 if <i>node is not found.</i>
        /// </summary>
        public int LeftCharEdge(Tree node)
        {
            var i = new MutableWrapper<int>(0);
            if (LeftCharEdge(node, i))
            {
                return i.Value();
            }
            return -1;
        }

        private bool LeftCharEdge(Tree node, MutableWrapper<int> i)
        {
            if (this == node)
            {
                return true;
            }
            else if (IsLeaf())
            {
                i.SetValue(i.Value() + Value().Length);
                return false;
            }
            else
            {
                foreach (Tree child in Children())
                {
                    if (child.LeftCharEdge(node, i))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns the positional index of the right edge of  <i>node</i> within the tree,
        /// as measured by characters. Returns -1 if <i>node is not found.</i>
        /// rightCharEdge returns the index of the rightmost character + 1, so that
        /// rightCharEdge(getLeaves().get(i)) == leftCharEdge(getLeaves().get(i+1))
        /// </summary>
        /// <param name="node">The subtree to look for in this Tree</param>
        /// <returns>The positional index of the right edge of node</returns>
        public int RightCharEdge(Tree node)
        {
            List<Tree> s = GetLeaves();
            int length = 0;
            foreach (Tree leaf in s)
            {
                length += leaf.Label().Value().Length;
            }
            var i = new MutableWrapper<int>(length);
            if (RightCharEdge(node, i))
            {
                return i.Value();
            }
            return -1;
        }

        private bool RightCharEdge(Tree node, MutableWrapper<int> i)
        {
            if (this == node)
            {
                return true;
            }
            else if (IsLeaf())
            {
                i.SetValue(i.Value() - Label().Value().Length);
                return false;
            }
            else
            {
                for (int j = Children().Length - 1; j >= 0; j--)
                {
                    if (Children()[j].RightCharEdge(node, i))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Calculates the node's <i>number</i>, defined as the number of nodes traversed in a left-to-right, 
        /// depth-first search of the tree starting at <code>root</code> and ending at <code>this</code>.
        /// Returns -1 if <code>root</code> does not contain <code>this</code>.
        /// </summary>
        /// <param name="root">the root node of the relevant tree</param>
        /// <returns>the number of the current node, or -1 if <code>root</code> does not contain <code>this</code></returns>
        public int NodeNumber(Tree root)
        {
            var i = new MutableWrapper<int>(1);
            if (NodeNumberHelper(root, i))
                return i.Value();
            return -1;
        }

        private bool NodeNumberHelper(Tree t, MutableWrapper<int> i)
        {
            if (this == t)
                return true;
            i.SetValue(i.Value() + 1);
            for (int j = 0; j < t.Children().Length; j++)
            {
                if (NodeNumberHelper(t.Children()[j], i))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Fetches the <code>i</code>th node in the tree, with node numbers defined
        /// as in {@link #nodeNumber(Tree)}
        /// </summary>
        /// <param name="i">the node number to fetch</param>
        /// <returns>the <code>i</code>th node in the tree</returns>
        public Tree GetNodeNumber(int i)
        {
            return GetNodeNumberHelper(new MutableWrapper<int>(1), i);
        }

        private Tree GetNodeNumberHelper(MutableWrapper<int> i, int target)
        {
            int i1 = i.Value();
            if (i1 == target)
                return this;
            if (i1 > target)
                throw new IndexOutOfRangeException("Error -- tree does not contain " + i + " nodes.");
            i.SetValue(i.Value() + 1);
            for (int j = 0; j < Children().Length; j++)
            {
                Tree temp = Children()[j].GetNodeNumberHelper(i, target);
                if (temp != null)
                    return temp;
            }
            return null;
        }

        /// <summary>
        /// Assign sequential integer indices to the leaves of the tree
        /// rooted at this <code>Tree</code>, starting with 1.
        /// The leaves are traversed from left to right. 
        /// If the node is already indexed, then it uses the existing index.
        /// This will only work if the leaves extend CoreMap.
        /// </summary>
        public void IndexLeaves()
        {
            IndexLeaves(0, false);
        }

        /// <summary>
        /// Index the leaves, and optionally overwrite existing IndexAnnotations if they exist.
        /// </summary>
        /// <param name="overWrite">Whether to replace an existing index for a leaf.</param>
        public void IndexLeaves(bool overWrite)
        {
            IndexLeaves(0, overWrite);
        }

        /// <summary>
        /// Assign sequential integer indices to the leaves of the subtree
        /// rooted at this <code>Tree</code>, beginning with <code>startIndex</code>, 
        /// and traversing the leaves from left to right. 
        /// If node is already indexed, then it uses the existing index.
        /// This method only works if the labels of the tree implement CoreLabel!
        /// </summary>
        /// <param name="startIndex">index for this node</param>
        /// <param name="overWrite">Whether to replace an existing index for a leaf</param>
        /// <returns>the next index still unassigned</returns>
        public int IndexLeaves(int startIndex, bool overWrite)
        {
            if (IsLeaf())
            {

                /*CoreLabel afl = (CoreLabel) label();
                  Integer oldIndex = afl.get(CoreAnnotations.IndexAnnotation.class);
                  if (!overWrite && oldIndex != null && oldIndex >= 0) {
                    startIndex = oldIndex;
                  } else {
                    afl.set(CoreAnnotations.IndexAnnotation.class, startIndex);
                  }*/

                if (Label() is IHasIndex)
                {
                    var hi = (IHasIndex) Label();
                    int oldIndex = hi.Index();
                    if (!overWrite && oldIndex >= 0)
                    {
                        startIndex = oldIndex;
                    }
                    else
                    {
                        hi.SetIndex(startIndex);
                    }
                    startIndex++;
                }
            }
            else
            {
                foreach (Tree kid in Children())
                {
                    startIndex = kid.IndexLeaves(startIndex, overWrite);
                }
            }
            return startIndex;
        }

        /// <summary>
        /// Percolates terminal indices through a dependency tree. The terminals should be indexed, e.g.,
        /// by calling indexLeaves() on the tree.
        /// 
        /// This method assumes CoreLabels!
        /// </summary>
        public void PercolateHeadIndices()
        {
            if (IsPreTerminal())
            {
                int nodeIndex = ((IHasIndex) FirstChild().Label()).Index();
                ((IHasIndex) Label()).SetIndex(nodeIndex);
                return;
            }

            // Assign the head index to the first child that we encounter with a matching
            // surface form. Obviously a head can have the same surface form as its dependent,
            // and in this case the head index is ambiguous.
            string wordAnnotation = ((IHasWord) Label()).GetWord();
            if (wordAnnotation == null)
            {
                wordAnnotation = Value();
            }
            bool seenHead = false;
            foreach (Tree child in Children())
            {
                child.PercolateHeadIndices();
                string childWordAnnotation = ((IHasWord) child.Label()).GetWord();
                if (childWordAnnotation == null)
                {
                    childWordAnnotation = child.Value();
                }
                if (!seenHead && wordAnnotation.Equals(childWordAnnotation))
                {
                    seenHead = true;
                    int nodeIndex = ((IHasIndex) child.Label()).Index();
                    ((IHasIndex) Label()).SetIndex(nodeIndex);
                }
            }
        }

        public void IndexSpans()
        {
            IndexSpans(0);
        }

        public void IndexSpans(int startIndex)
        {
            IndexSpans(new MutableWrapper<int>(startIndex));
        }

        /// <summary>
        /// Assigns span indices (BeginIndexAnnotation and EndIndexAnnotation) to all nodes in a tree.
        /// The beginning index is equivalent to the IndexAnnotation of the first leaf in the constituent.
        /// The end index is equivalent to the first integer after the IndexAnnotation of the last leaf in the constituent.
        /// </summary>
        /// <param name="startIndex">Begin indexing at this value</param>
        public Tuple<int, int> IndexSpans(MutableWrapper<int> startIndex)
        {
            int start = int.MaxValue;
            int end = int.MinValue;

            if (IsLeaf())
            {
                start = startIndex.Value();
                end = startIndex.Value() + 1;
                startIndex.SetValue(startIndex.Value() + 1);
            }
            else
            {
                foreach (Tree kid in Children())
                {
                    Tuple<int, int> span = kid.IndexSpans(startIndex);
                    if (span.Item1 < start) start = span.Item1;
                    if (span.Item2 > end) end = span.Item2;
                }
            }

            ILabel lab = Label();
            if (lab is ICoreMap)
            {
                var afl = (ICoreMap) Label();
                afl.Set(typeof (CoreAnnotations.BeginIndexAnnotation), start);
                afl.Set(typeof (CoreAnnotations.EndIndexAnnotation), end);
            }
            return new Tuple<int, int>(start, end);
        }

    }
}