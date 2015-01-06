using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * Various static utilities for the <code>Tree</code> class.
 *
 * @author Roger Levy
 * @author Dan Klein
 * @author Aria Haghighi (tree path methods)
 */

    public static class Trees
    {
        private static readonly LabeledScoredTreeFactory defaultTreeFactory = new LabeledScoredTreeFactory();

        //private Trees() {}


        /**
   * Returns the positional index of the left edge of a tree <i>t</i>
   * within a given root, as defined by the size of the yield of all
   * material preceding <i>t</i>.
   */

        public static int leftEdge(Tree t, Tree root)
        {
            var i = 0;
            if (leftEdge(t, root, i))
            {
                return i;
            }
            else
            {
                throw new SystemException("Tree is not a descendant of root.");
//      return -1;
            }
        }

        private static bool leftEdge(Tree t, Tree t1, int i)
        {
            if (t == t1)
            {
                return true;
            }
            else if (t1.isLeaf())
            {
                int j = t1.yield().Count; // so that empties don't add size
                i = i + j;
                return false;
            }
            else
            {
                foreach (Tree kid in t1.children())
                {
                    if (leftEdge(t, kid, i))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /**
   * Returns the positional index of the right edge of a tree
   * <i>t</i> within a given root, as defined by the size of the yield
   * of all material preceding <i>t</i> plus all the material
   * contained in <i>t</i>.
   */

        public static int rightEdge(Tree t, Tree root)
        {
            var i = root.yield().Count;
            if (rightEdge(t, root, i))
            {
                return i;
            }
            else
            {
                throw new SystemException("Tree is not a descendant of root.");
//      return root.yield().size() + 1;
            }
        }

        private static bool rightEdge(Tree t, Tree t1, int i)
        {
            if (t == t1)
            {
                return true;
            }
            else if (t1.isLeaf())
            {
                int j = t1.yield().Count; // so that empties don't add size
                i = i - j;
                return false;
            }
            else
            {
                Tree[] kids = t1.children();
                for (int j = kids.Length - 1; j >= 0; j--)
                {
                    if (rightEdge(t, kids[j], i))
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

        /**
   * returns the leaves in a Tree in the order that they're found.
   */

        public static List<Tree> leaves(Tree t)
        {
            List<Tree> l = new List<Tree>();
            leaves(t, l);
            return l;
        }

        private static void leaves(Tree t, List<Tree> l)
        {
            if (t.isLeaf())
            {
                l.Add(t);
            }
            else
            {
                foreach (Tree kid in t.children())
                {
                    leaves(kid, l);
                }
            }
        }

        public static List<Tree> preTerminals(Tree t)
        {
            List<Tree> l = new List<Tree>();
            preTerminals(t, l);
            return l;
        }

        private static void preTerminals(Tree t, List<Tree> l)
        {
            if (t.isPreTerminal())
            {
                l.Add(t);
            }
            else
            {
                foreach (Tree kid in t.children())
                {
                    preTerminals(kid, l);
                }
            }
        }


        /**
   * returns the labels of the leaves in a Tree in the order that they're found.
   */

        public static List<Label> leafLabels(Tree t)
        {
            List<Label> l = new List<Label>();
            leafLabels(t, l);
            return l;
        }

        private static void leafLabels(Tree t, List<Label> l)
        {
            if (t.isLeaf())
            {
                l.Add(t.label());
            }
            else
            {
                foreach (Tree kid in t.children())
                {
                    leafLabels(kid, l);
                }
            }
        }

        /**
   * returns the labels of the leaves in a Tree, augmented with POS tags.  assumes that
   * the labels are CoreLabels.
   */

        public static List<CoreLabel> taggedLeafLabels(Tree t)
        {
            List<CoreLabel> l = new List<CoreLabel>();
            taggedLeafLabels(t, l);
            return l;
        }

        private static void taggedLeafLabels(Tree t, List<CoreLabel> l)
        {
            if (t.isPreTerminal())
            {
                CoreLabel fl = (CoreLabel) t.getChild(0).label();
                fl.set(typeof (CoreAnnotations.TagLabelAnnotation), t.label());
                l.Add(fl);
            }
            else
            {
                foreach (Tree kid in t.children())
                {
                    taggedLeafLabels(kid, l);
                }
            }
        }


        /**
   * returns the maximal projection of <code>head</code> in
   * <code>root</code> given a {@link HeadFinder}
   */

        public static Tree maximalProjection(Tree head, Tree root, HeadFinder hf)
        {
            Tree projection = head;
            if (projection == root)
            {
                return root;
            }
            Tree parent = projection.parent(root);
            while (hf.determineHead(parent) == projection)
            {
                projection = parent;
                if (projection == root)
                {
                    return root;
                }
                parent = projection.parent(root);
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

        /**
   * returns the syntactic category of the tree as a list of the syntactic categories of the mother and the daughters
   */

        public static List<string> localTreeAsCatList(Tree t)
        {
            List<string> l = new List<string>(t.children().Length + 1);
            l.Add(t.label().value());
            for (int i = 0; i < t.children().Length; i++)
            {
                l.Add(t.children()[i].label().value());
            }
            return l;
        }

        /**
   * Returns the index of <code>daughter</code> in <code>parent</code> by ==.
   * Returns -1 if <code>daughter</code> not found.
   */

        public static int objectEqualityIndexOf(Tree parent, Tree daughter)
        {
            for (int i = 0; i < parent.children().Length; i++)
            {
                if (daughter == parent.children()[i])
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


        public static string treeToLatex(Tree t)
        {
            StringBuilder connections = new StringBuilder();
            StringBuilder hierarchy = new StringBuilder();
            treeToLatexHelper(t, connections, hierarchy, 0, 1, 0);
            return "\\tree" + hierarchy + '\n' + connections + '\n';
        }

        private static int treeToLatexHelper(Tree t, StringBuilder c, StringBuilder h,
            int n, int nextN, int indent)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indent; i++)
                sb.Append("  ");
            h.Append('\n').Append(sb);
            h.Append("{\\")
                .Append(t.isLeaf() ? "" : "n")
                .Append("tnode{z")
                .Append(n)
                .Append("}{")
                .Append(t.label())
                .Append('}');
            if (!t.isLeaf())
            {
                for (int k = 0; k < t.children().Length; k++)
                {
                    h.Append(", ");
                    c.Append("\\nodeconnect{z").Append(n).Append("}{z").Append(nextN).Append("}\n");
                    nextN = treeToLatexHelper(t.children()[k], c, h, nextN, nextN + 1, indent + 1);
                }
            }
            h.Append('}');
            return nextN;
        }

        public static string treeToLatexEven(Tree t)
        {
            StringBuilder connections = new StringBuilder();
            StringBuilder hierarchy = new StringBuilder();
            int maxDepth = t.depth();
            treeToLatexEvenHelper(t, connections, hierarchy, 0, 1, 0, 0, maxDepth);
            return "\\tree" + hierarchy + '\n' + connections + '\n';
        }

        private static int treeToLatexEvenHelper(Tree t, StringBuilder c, StringBuilder h, int n,
            int nextN, int indent, int curDepth, int maxDepth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indent; i++)
                sb.Append("  ");
            h.Append('\n').Append(sb);
            int tDepth = t.depth();
            if (tDepth == 0 && tDepth + curDepth < maxDepth)
            {
                for (int pad = 0; pad < maxDepth - tDepth - curDepth; pad++)
                {
                    h.Append("{\\ntnode{pad}{}, ");
                }
            }
            h.Append("{\\ntnode{z").Append(n).Append("}{").Append(t.label()).Append('}');
            if (!t.isLeaf())
            {
                for (int k = 0; k < t.children().Length; k++)
                {
                    h.Append(", ");
                    c.Append("\\nodeconnect{z").Append(n).Append("}{z").Append(nextN).Append("}\n");
                    nextN = treeToLatexEvenHelper(t.children()[k], c, h, nextN, nextN + 1, indent + 1, curDepth + 1,
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

        private static string texTree(Tree t)
        {
            return treeToLatex(t);
        }

        private static string escape(string s)
        {
            StringBuilder sb = new StringBuilder();
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


        /**
   * Get lowest common ancestor of all the nodes in the list with the tree rooted at root
   */

        public static Tree getLowestCommonAncestor(List<Tree> nodes, Tree root)
        {
            List<List<Tree>> paths = new List<List<Tree>>();
            int min = int.MaxValue;
            foreach (Tree t in nodes)
            {
                List<Tree> path = pathFromRoot(t, root);
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


        /**
   * returns a list of categories that is the path from Tree from to Tree
   * to within Tree root.  If either from or to is not in root,
   * returns null.  Otherwise includes both from and to in the list.
   */

        public static List<string> pathNodeToNode(Tree from, Tree to, Tree root)
        {
            List<Tree> fromPath = pathFromRoot(from, root);
            //System.out.println(treeListToCatList(fromPath));
            if (fromPath == null)
                return null;

            List<Tree> toPath = pathFromRoot(to, root);
            //System.out.println(treeListToCatList(toPath));
            if (toPath == null)
                return null;

            //System.out.println(treeListToCatList(fromPath));
            //System.out.println(treeListToCatList(toPath));

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

            //System.out.println(treeListToCatList(fromPath));
            //System.out.println(treeListToCatList(toPath));
            List<string> totalPath = new List<string>();

            for (int i = fromPath.Count - 1; i >= last; i--)
            {
                Tree t = fromPath[i];
                totalPath.Add("up-" + t.label().value());
            }

            if (lastNode != null)
                totalPath.Add("up-" + lastNode.label().value());

            foreach (Tree t in toPath)
            {
                totalPath.Add("down-" + t.label().value());
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


        /**
   * returns list of tree nodes to root from t.  Includes root and
   * t. Returns null if tree not found dominated by root
   */

        public static List<Tree> pathFromRoot(Tree t, Tree root)
        {
            if (t == root)
            {
                //if (t.equals(root)) {
                List<Tree> l = new List<Tree>(1);
                l.Add(t);
                return l;
            }
            else if (root == null)
            {
                return null;
            }
            return root.dominationPath(t);
        }


        /**
   * replaces all instances (by ==) of node with node1.  Doesn't affect
   * the node t itself
   */

        public static void replaceNode(Tree node, Tree node1, Tree t)
        {
            if (t.isLeaf())
                return;
            Tree[] kids = t.children();
            List<Tree> newKids = new List<Tree>(kids.Length);
            foreach (Tree kid in kids)
            {
                if (kid != node)
                {
                    newKids.Add(kid);
                    replaceNode(node, node1, kid);
                }
                else
                {
                    newKids.Add(node1);
                }
            }
            t.setChildren(newKids);
        }


        /**
   * returns the node of a tree which represents the lowest common
   * ancestor of nodes t1 and t2 dominated by root. If either t1 or
   * or t2 is not dominated by root, returns null.
   */

        public static Tree getLowestCommonAncestor(Tree t1, Tree t2, Tree root)
        {
            List<Tree> t1Path = pathFromRoot(t1, root);
            List<Tree> t2Path = pathFromRoot(t2, root);
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

        /**
   * Outputs the labels on the trees, not just the words.
   */
        /*public static void outputTreeLabels(Tree tree) {
    outputTreeLabels(tree, 0);
  }

  public static void outputTreeLabels(Tree tree, int depth) {
    for (int i = 0; i < depth; ++i) {
      System.out.print(" ");
    }
    System.out.println(tree.label());
    for (Tree child : tree.children()) {
      outputTreeLabels(child, depth + 1);
    }
  }*/

        /**
   * Converts the tree labels to CoreLabels.
   * We need this because we store additional info in the CoreLabel, like token span.
   * @param tree
   */

        public static void convertToCoreLabels(Tree tree)
        {
            Label l = tree.label();
            if (!(l is CoreLabel))
            {
                CoreLabel cl = new CoreLabel();
                cl.setValue(l.value());
                tree.setLabel(cl);
            }

            foreach (Tree kid in tree.children())
            {
                convertToCoreLabels(kid);
            }
        }


        /**
   * Set the sentence index of all the leaves in the tree
   * (only works on CoreLabel)
   */

        public static void setSentIndex(Tree tree, int sentIndex)
        {
            List<Label> leaves = tree.yield();
            foreach (Label leaf in leaves)
            {
                if (!(leaf is CoreLabel))
                {
                    throw new ArgumentException("Only works on CoreLabel");
                }
                ((CoreLabel) leaf).setSentIndex(sentIndex);
            }
        }
    }
}