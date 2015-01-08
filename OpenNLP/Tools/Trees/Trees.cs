using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Various static utilities for the <code>Tree</code> class.
    /// 
    /// @author Roger Levy
    /// @author Dan Klein
    /// @author Aria Haghighi (tree path methods)
    /// </summary>
    public static class Trees
    {
        private static readonly LabeledScoredTreeFactory defaultTreeFactory = new LabeledScoredTreeFactory();
        
        /// <summary>
        /// Returns the positional index of the left edge of a tree <i>t</i>
        /// within a given root, as defined by the size of the yield of all
        /// material preceding <i>t</i>.
        /// </summary>
        public static int LeftEdge(Tree t, Tree root)
        {
            var i = 0;
            if (LeftEdge(t, root, i))
            {
                return i;
            }
            else
            {
                throw new SystemException("Tree is not a descendant of root.");
            }
        }

        private static bool LeftEdge(Tree t, Tree t1, int i)
        {
            if (t == t1)
            {
                return true;
            }
            else if (t1.IsLeaf())
            {
                int j = t1.Yield().Count; // so that empties don't add size
                i = i + j;
                return false;
            }
            else
            {
                foreach (Tree kid in t1.Children())
                {
                    if (LeftEdge(t, kid, i))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Returns the positional index of the right edge of a tree
        /// <i>t</i> within a given root, as defined by the size of the yield
        /// of all material preceding <i>t</i> plus all the material contained in <i>t</i>.
        /// </summary>
        public static int RightEdge(Tree t, Tree root)
        {
            var i = root.Yield().Count;
            if (RightEdge(t, root, i))
            {
                return i;
            }
            else
            {
                throw new SystemException("Tree is not a descendant of root.");
            }
        }

        private static bool RightEdge(Tree t, Tree t1, int i)
        {
            if (t == t1)
            {
                return true;
            }
            else if (t1.IsLeaf())
            {
                int j = t1.Yield().Count; // so that empties don't add size
                i = i - j;
                return false;
            }
            else
            {
                Tree[] kids = t1.Children();
                for (int j = kids.Length - 1; j >= 0; j--)
                {
                    if (RightEdge(t, kids[j], i))
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        /**
   * Returns a lexicalized Tree whose Labels are CategoryWordTag
   * instances, all corresponds to the input tree.
   */
        /*public static Tree lexicalize(Tree t, HeadFinder hf) {
    Function<Tree,Tree> a =
      TreeFunctions.getLabeledTreeToCategoryWordTagTreeFunction();
    Tree t1 = a.apply(t);
    t1.percolateHeads(hf);
    return t1;
  }*/

        /// <summary>
        /// Returns the leaves in a Tree in the order that they're found
        /// </summary>
        public static List<Tree> Leaves(Tree t)
        {
            var l = new List<Tree>();
            Leaves(t, l);
            return l;
        }

        private static void Leaves(Tree t, List<Tree> l)
        {
            if (t.IsLeaf())
            {
                l.Add(t);
            }
            else
            {
                foreach (Tree kid in t.Children())
                {
                    Leaves(kid, l);
                }
            }
        }

        public static List<Tree> PreTerminals(Tree t)
        {
            var l = new List<Tree>();
            PreTerminals(t, l);
            return l;
        }

        private static void PreTerminals(Tree t, List<Tree> l)
        {
            if (t.IsPreTerminal())
            {
                l.Add(t);
            }
            else
            {
                foreach (Tree kid in t.Children())
                {
                    PreTerminals(kid, l);
                }
            }
        }


        /// <summary>
        /// Returns the labels of the leaves in a Tree in the order that they're found
        /// </summary>
        public static List<ILabel> LeafLabels(Tree t)
        {
            var l = new List<ILabel>();
            LeafLabels(t, l);
            return l;
        }

        private static void LeafLabels(Tree t, List<ILabel> l)
        {
            if (t.IsLeaf())
            {
                l.Add(t.Label());
            }
            else
            {
                foreach (Tree kid in t.Children())
                {
                    LeafLabels(kid, l);
                }
            }
        }

        /// <summary>
        /// Returns the labels of the leaves in a Tree, augmented with POS tags.  a
        /// Assumes that the labels are CoreLabels.
        /// </summary>
        public static List<CoreLabel> TaggedLeafLabels(Tree t)
        {
            var l = new List<CoreLabel>();
            TaggedLeafLabels(t, l);
            return l;
        }

        private static void TaggedLeafLabels(Tree t, List<CoreLabel> l)
        {
            if (t.IsPreTerminal())
            {
                var fl = (CoreLabel) t.GetChild(0).Label();
                fl.Set(typeof (CoreAnnotations.TagLabelAnnotation), t.Label());
                l.Add(fl);
            }
            else
            {
                foreach (Tree kid in t.Children())
                {
                    TaggedLeafLabels(kid, l);
                }
            }
        }

        /// <summary>
        /// Returns the maximal projection of <code>head</code> in
        /// <code>root</code> given a {@link HeadFinder}
        /// </summary>
        public static Tree MaximalProjection(Tree head, Tree root, IHeadFinder hf)
        {
            Tree projection = head;
            if (projection == root)
            {
                return root;
            }
            Tree parent = projection.Parent(root);
            while (hf.DetermineHead(parent) == projection)
            {
                projection = parent;
                if (projection == root)
                {
                    return root;
                }
                parent = projection.Parent(root);
            }
            return projection;
        }

        /* applies a TreeVisitor to all projections (including the node itself) of a node in a Tree.
  *  Does nothing if head is not in root.
  * @return the maximal projection of head in root.
  */
        /*public static Tree applyToProjections(TreeVisitor v, Tree head, Tree root, HeadFinder hf) {
    Tree projection = head;
    Tree parent = projection.parent(root);
    if (parent == null && projection != root) {
      return null;
    }
    v.visitTree(projection);
    if (projection == root) {
      return root;
    }
    while (hf.determineHead(parent) == projection) {
      projection = parent;
      v.visitTree(projection);
      if (projection == root) {
        return root;
      }
      parent = projection.parent(root);
    }
    return projection;
  }*/

        /**
   * gets the <code>n</code>th terminal in <code>tree</code>.  The first terminal is number zero.
   */
        /*public static Tree getTerminal(Tree tree, int n) {
    return getTerminal(tree, new MutableInteger(0), n);
  }

  static Tree getTerminal(Tree tree, MutableInteger i, int n) {
    if (i.intValue() == n) {
      if (tree.isLeaf()) {
        return tree;
      } else {
        return getTerminal(tree.children()[0], i, n);
      }
    } else {
      if (tree.isLeaf()) {
        i.set(i.intValue() + tree.yield().size());
        return null;
      } else {
        foreach (Tree kid in tree.children()) {
          Tree result = getTerminal(kid, i, n);
          if (result != null) {
            return result;
          }
        }
        return null;
      }
    }
  }*/

        /**
   * gets the <code>n</code>th preterminal in <code>tree</code>.  The first terminal is number zero.
   */
        /*public static Tree getPreTerminal(Tree tree, int n) {
    return getPreTerminal(tree, new MutableInteger(0), n);
  }

  static Tree getPreTerminal(Tree tree, MutableInteger i, int n) {
    if (i.intValue() == n) {
      if (tree.isPreTerminal()) {
        return tree;
      } else {
        return getPreTerminal(tree.children()[0], i, n);
      }
    } else {
      if (tree.isPreTerminal()) {
        i.set(i.intValue() + tree.yield().size());
        return null;
      } else {
        foreach (Tree kid in tree.children()) {
          Tree result = getPreTerminal(kid, i, n);
          if (result != null) {
            return result;
          }
        }
        return null;
      }
    }
  }*/

        /// <summary>
        /// Returns the syntactic category of the tree as a list of the syntactic categories of the mother and the daughters
        /// </summary>
        public static List<string> LocalTreeAsCatList(Tree t)
        {
            var l = new List<string>(t.Children().Length + 1);
            l.Add(t.Label().Value());
            for (int i = 0; i < t.Children().Length; i++)
            {
                l.Add(t.Children()[i].Label().Value());
            }
            return l;
        }

        /// <summary>
        /// Returns the index of <code>daughter</code> in <code>parent</code> by ==.
        /// Returns -1 if <code>daughter</code> not found.
        /// </summary>
        public static int ObjectEqualityIndexOf(Tree parent, Tree daughter)
        {
            for (int i = 0; i < parent.Children().Length; i++)
            {
                if (daughter == parent.Children()[i])
                {
                    return i;
                }
            }
            return -1;
        }

        /** Returns a string reporting what kinds of Tree and Label nodes this
   *  Tree contains.
   *
   *  @param t The tree to examine.
   *  @return A human-readable string reporting what kinds of Tree and Label nodes this
   *      Tree contains.
   */
        /*public static string toStructureDebugString(Tree t) {
    string tCl = StringUtils.getShortClassName(t);
    string tfCl = StringUtils.getShortClassName(t.treeFactory());
    string lCl = StringUtils.getShortClassName(t.label());
    string lfCl = StringUtils.getShortClassName(t.label().labelFactory());
    Set<string> otherClasses = Generics.newHashSet();
    string leafLabels = null;
    string tagLabels = null;
    string phraseLabels = null;
    string leaves = null;
    string nodes = null;
    for (Tree st : t) {
      string stCl = StringUtils.getShortClassName(st);
      string stfCl = StringUtils.getShortClassName(st.treeFactory());
      string slCl = StringUtils.getShortClassName(st.label());
      string slfCl = StringUtils.getShortClassName(st.label().labelFactory());
      if ( ! tCl.equals(stCl)) {
        otherClasses.Add(stCl);
      }
      if ( ! tfCl.equals(stfCl)) {
        otherClasses.Add(stfCl);
      }
      if ( ! lCl.equals(slCl)) {
        otherClasses.Add(slCl);
      }
      if ( ! lfCl.equals(slfCl)) {
        otherClasses.Add(slfCl);
      }
      if (st.isPhrasal()) {
        if (nodes == null) {
          nodes = stCl;
        } else if ( ! nodes.equals(stCl)) {
          nodes = "mixed";
        }
        if (phraseLabels == null) {
          phraseLabels = slCl;
        } else if ( ! phraseLabels.equals(slCl)) {
          phraseLabels = "mixed";
        }
      } else if (st.isPreTerminal()) {
        if (nodes == null) {
          nodes = stCl;
        } else if ( ! nodes.equals(stCl)) {
          nodes = "mixed";
        }
        if (tagLabels == null) {
          tagLabels = StringUtils.getShortClassName(slCl);
        } else if ( ! tagLabels.equals(slCl)) {
          tagLabels = "mixed";
        }
      } else if (st.isLeaf()) {
        if (leaves == null) {
          leaves = stCl;
        } else if ( ! leaves.equals(stCl)) {
          leaves = "mixed";
        }
        if (leafLabels == null) {
          leafLabels = slCl;
        } else if ( ! leafLabels.equals(slCl)) {
          leafLabels = "mixed";
        }
      } else {
        throw new IllegalStateException("Bad tree state: " + t);
      }
    } // end for Tree st : this
    StringBuilder sb = new StringBuilder();
    sb.Append("Tree with root of class ").Append(tCl).Append(" and factory ").Append(tfCl);
    sb.Append(" and root label class ").Append(lCl).Append(" and factory ").Append(lfCl);
    if ( ! otherClasses.isEmpty()) {
      sb.Append(" and the following classes also found within the tree: ").Append(otherClasses);
      return " with " + nodes + " interior nodes and " + leaves +
        " leaves, and " + phraseLabels + " phrase labels, " +
        tagLabels + " tag labels, and " + leafLabels + " leaf labels.";
    } else {
      sb.Append(" (and uniform use of these Tree and Label classes throughout the tree).");
    }
    return sb.ToString();
  }*/


        /** Turns a sentence into a flat phrasal tree.
   *  The structure is S -> tag*.  And then each tag goes to a word.
   *  The tag is either found from the label or made "WD".
   *  The tag and phrasal node have a StringLabel.
   *
   *  @param s The Sentence to make the Tree from
   *  @return The one phrasal level Tree
   */
        /*public static Tree toFlatTree(List<HasWord> s) {
    return toFlatTree(s, new StringLabelFactory());
  }*/

        /** Turns a sentence into a flat phrasal tree.
   *  The structure is S -> tag*.  And then each tag goes to a word.
   *  The tag is either found from the label or made "WD".
   *  The tag and phrasal node have a StringLabel.
   *
   *  @param s The Sentence to make the Tree from
   *  @param lf The LabelFactory with which to create the new Tree labels
   *  @return The one phrasal level Tree
   */
        /*public static Tree toFlatTree(List<? extends HasWord> s, LabelFactory lf) {
    List<Tree> daughters = new List<Tree>(s.size());
    for (HasWord word : s) {
      Tree wordNode = new LabeledScoredTreeNode(lf.newLabel(word.word()));
      if (word instanceof TaggedWord) {
        TaggedWord taggedWord = (TaggedWord) word;
        wordNode = new LabeledScoredTreeNode(new StringLabel(taggedWord.tag()), Collections.singletonList(wordNode));
      } else {
        wordNode = new LabeledScoredTreeNode(lf.newLabel("WD"), Collections.singletonList(wordNode));
      }
      daughters.Add(wordNode);
    }
    return new LabeledScoredTreeNode(new StringLabel("S"), daughters);
  }*/


        public static string TreeToLatex(Tree t)
        {
            var connections = new StringBuilder();
            var hierarchy = new StringBuilder();
            TreeToLatexHelper(t, connections, hierarchy, 0, 1, 0);
            return "\\tree" + hierarchy + '\n' + connections + '\n';
        }

        private static int TreeToLatexHelper(Tree t, StringBuilder c, StringBuilder h,
            int n, int nextN, int indent)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < indent; i++)
                sb.Append("  ");
            h.Append('\n').Append(sb);
            h.Append("{\\")
                .Append(t.IsLeaf() ? "" : "n")
                .Append("tnode{z")
                .Append(n)
                .Append("}{")
                .Append(t.Label())
                .Append('}');
            if (!t.IsLeaf())
            {
                for (int k = 0; k < t.Children().Length; k++)
                {
                    h.Append(", ");
                    c.Append("\\nodeconnect{z").Append(n).Append("}{z").Append(nextN).Append("}\n");
                    nextN = TreeToLatexHelper(t.Children()[k], c, h, nextN, nextN + 1, indent + 1);
                }
            }
            h.Append('}');
            return nextN;
        }

        public static string TreeToLatexEven(Tree t)
        {
            var connections = new StringBuilder();
            var hierarchy = new StringBuilder();
            int maxDepth = t.Depth();
            TreeToLatexEvenHelper(t, connections, hierarchy, 0, 1, 0, 0, maxDepth);
            return "\\tree" + hierarchy + '\n' + connections + '\n';
        }

        private static int TreeToLatexEvenHelper(Tree t, StringBuilder c, StringBuilder h, int n,
            int nextN, int indent, int curDepth, int maxDepth)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < indent; i++)
                sb.Append("  ");
            h.Append('\n').Append(sb);
            int tDepth = t.Depth();
            if (tDepth == 0 && tDepth + curDepth < maxDepth)
            {
                for (int pad = 0; pad < maxDepth - tDepth - curDepth; pad++)
                {
                    h.Append("{\\ntnode{pad}{}, ");
                }
            }
            h.Append("{\\ntnode{z").Append(n).Append("}{").Append(t.Label()).Append('}');
            if (!t.IsLeaf())
            {
                for (int k = 0; k < t.Children().Length; k++)
                {
                    h.Append(", ");
                    c.Append("\\nodeconnect{z").Append(n).Append("}{z").Append(nextN).Append("}\n");
                    nextN = TreeToLatexEvenHelper(t.Children()[k], c, h, nextN, nextN + 1, indent + 1, curDepth + 1,
                        maxDepth);
                }
            }
            if (tDepth == 0 && tDepth + curDepth < maxDepth)
            {
                for (int pad = 0; pad < maxDepth - tDepth - curDepth; pad++)
                {
                    h.Append('}');
                }
            }
            h.Append('}');
            return nextN;
        }

        private static string TexTree(Tree t)
        {
            return TreeToLatex(t);
        }

        private static string Escape(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '^')
                    sb.Append('\\');
                sb.Append(c);
                if (c == '^')
                    sb.Append("{}");
            }
            return sb.ToString();
        }

        /*public static Tree normalizeTree(Tree tree, TreeNormalizer tn, TreeFactory tf) {
    foreach (Tree node in tree) {
      if (node.isLeaf()) {
        node.label().setValue(tn.normalizeTerminal(node.label().value()));
      } else {
        node.label().setValue(tn.normalizeNonterminal(node.label().value()));
      }
    }
    return tn.normalizeWholeTree(tree, tf);
  }*/


        /**
   * Gets the <i>i</i>th leaf of a tree from the left.
   * The leftmost leaf is numbered 0.
   *
   * @return The <i>i</i><sup>th</sup> leaf as a Tree, or <code>null</code>
   *     if there is no such leaf.
   */
        /*public static Tree getLeaf(Tree tree, int i) {
    int count = -1;
    foreach (Tree next in tree) {
      if (next.isLeaf()) {
        count++;
      }
      if (count == i) {
        return next;
      }
    }
    return null;
  }*/


        /// <summary>
        /// Get lowest common ancestor of all the nodes in the list with the tree rooted at root
        /// </summary>
        public static Tree GetLowestCommonAncestor(List<Tree> nodes, Tree root)
        {
            var paths = new List<List<Tree>>();
            int min = int.MaxValue;
            foreach (Tree t in nodes)
            {
                List<Tree> path = PathFromRoot(t, root);
                if (path == null) return null;
                min = Math.Min(min, path.Count);
                paths.Add(path);
            }
            Tree commonAncestor = null;
            for (int i = 0; i < min; ++i)
            {
                Tree ancestor = paths[0][i];
                bool quit = false;
                foreach (List<Tree> path in paths)
                {
                    if (!path[i].Equals(ancestor))
                    {
                        quit = true;
                        break;
                    }
                }
                if (quit) break;
                commonAncestor = ancestor;
            }
            return commonAncestor;
        }

        /// <summary>
        /// Returns a list of categories that is the path from Tree from to Tree
        /// to within Tree root.  If either from or to is not in root,
        /// Returns null.  Otherwise includes both from and to in the list.
        /// </summary>
        public static List<string> PathNodeToNode(Tree from, Tree to, Tree root)
        {
            List<Tree> fromPath = PathFromRoot(from, root);
            if (fromPath == null)
                return null;

            List<Tree> toPath = PathFromRoot(to, root);
            if (toPath == null)
                return null;

            int last = 0;
            int min = fromPath.Count <= toPath.Count ? fromPath.Count : toPath.Count;

            Tree lastNode = null;
//     while((! (fromPath.isEmpty() || toPath.isEmpty())) &&  fromPath.get(0).equals(toPath.get(0))) {
//       lastNode = (Tree) fromPath.remove(0);
//       toPath.remove(0);
//     }
            while (last < min && fromPath[last].Equals(toPath[last]))
            {
                lastNode = fromPath[last];
                last++;
            }

            var totalPath = new List<string>();

            for (int i = fromPath.Count - 1; i >= last; i--)
            {
                Tree t = fromPath[i];
                totalPath.Add("up-" + t.Label().Value());
            }

            if (lastNode != null)
                totalPath.Add("up-" + lastNode.Label().Value());

            foreach (Tree t in toPath)
            {
                totalPath.Add("down-" + t.Label().Value());
            }


//     for(ListIterator i = fromPath.listIterator(fromPath.size()); i.hasPrevious(); ){
//       Tree t = (Tree) i.previous();
//       totalPath.Add("up-" + t.label().value());
//     }

//     if(lastNode != null)
//     totalPath.Add("up-" + lastNode.label().value());

//     for(ListIterator j = toPath.listIterator(); j.hasNext(); ){
//       Tree t = (Tree) j.next();
//       totalPath.Add("down-" + t.label().value());
//     }

            return totalPath;
        }

        /// <summary>
        /// Returns list of tree nodes to root from t.
        /// Includes root and t. Returns null if tree not found dominated by root
        /// </summary>
        public static List<Tree> PathFromRoot(Tree t, Tree root)
        {
            if (t == root)
            {
                //if (t.equals(root)) {
                var l = new List<Tree>(1) {t};
                return l;
            }
            else if (root == null)
            {
                return null;
            }
            return root.DominationPath(t);
        }

        /// <summary>
        /// Replaces all instances (by ==) of node with node1.
        /// Doesn't affect the node t itself
        /// </summary>
        public static void ReplaceNode(Tree node, Tree node1, Tree t)
        {
            if (t.IsLeaf())
                return;
            Tree[] kids = t.Children();
            var newKids = new List<Tree>(kids.Length);
            foreach (Tree kid in kids)
            {
                if (kid != node)
                {
                    newKids.Add(kid);
                    ReplaceNode(node, node1, kid);
                }
                else
                {
                    newKids.Add(node1);
                }
            }
            t.SetChildren(newKids);
        }


        /// <summary>
        /// Returns the node of a tree which represents the lowest common
        /// ancestor of nodes t1 and t2 dominated by root. If either t1 or
        /// or t2 is not dominated by root, returns null.
        /// </summary>
        public static Tree GetLowestCommonAncestor(Tree t1, Tree t2, Tree root)
        {
            List<Tree> t1Path = PathFromRoot(t1, root);
            List<Tree> t2Path = PathFromRoot(t2, root);
            if (t1Path == null || t2Path == null) return null;

            int min = Math.Min(t1Path.Count, t2Path.Count);
            Tree commonAncestor = null;
            for (int i = 0; i < min && t1Path[i].Equals(t2Path[i]); ++i)
            {
                commonAncestor = t1Path[i];
            }

            return commonAncestor;
        }

        /*/**
   * Simple tree reading utility method.  Given a tree formatted as a PTB string, returns a Tree made by a specific TreeFactory.
   #1#
  public static Tree readTree(string ptbTreeString, TreeFactory treeFactory) {
    try {
      PennTreeReader ptr = new PennTreeReader(new StringReader(ptbTreeString), treeFactory);
      return ptr.readTree();
    } catch (IOException ex) {
      throw new SystemException(ex);
    }
  }*/

        /**
   * Simple tree reading utility method.  Given a tree formatted as a PTB string, returns a Tree made by the default TreeFactory (LabeledScoredTreeFactory)
   */
        /*public static Tree readTree(string str) {
    return readTree(str, defaultTreeFactory);
  }*/

        /// <summary>
        /// Converts the tree labels to CoreLabels.
        /// We need this because we store additional info in the CoreLabel, like token span.
        /// </summary>
        public static void ConvertToCoreLabels(Tree tree)
        {
            ILabel l = tree.Label();
            if (!(l is CoreLabel))
            {
                var cl = new CoreLabel();
                cl.SetValue(l.Value());
                tree.SetLabel(cl);
            }

            foreach (Tree kid in tree.Children())
            {
                ConvertToCoreLabels(kid);
            }
        }

        /// <summary>
        /// Set the sentence index of all the leaves in the tree (only works on CoreLabel)
        /// </summary>
        public static void SetSentIndex(Tree tree, int sentIndex)
        {
            List<ILabel> leaves = tree.Yield();
            foreach (ILabel leaf in leaves)
            {
                if (!(leaf is CoreLabel))
                {
                    throw new ArgumentException("Only works on CoreLabel");
                }
                ((CoreLabel) leaf).SetSentIndex(sentIndex);
            }
        }
    }
}