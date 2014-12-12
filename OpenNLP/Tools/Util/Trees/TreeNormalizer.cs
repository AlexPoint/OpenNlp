using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    public class TreeNormalizer
    {
        public TreeNormalizer() {
  }

  /**
   * Normalizes a leaf contents (and maybe intern it).
   *
   * @param leaf The String that decorates the leaf
   * @return The normalized form of this leaf String
   */
  public String normalizeTerminal(String leaf) {
    return leaf;
  }

  /**
   * Normalizes a nonterminal contents (and maybe intern it).
   *
   * @param category The String that decorates this nonterminal node
   * @return The normalized form of this nonterminal String
   */
  public String normalizeNonterminal(String category) {
    return category;
  }

  /**
   * Normalize a whole tree -- this method assumes that the argument
   * that it is passed is the root of a complete <code>Tree</code>.
   * It is normally implemented as a Tree-walking routine. <p>
   * This method may return <code>null</code>. This is interpreted to
   * mean that this is a tree that should not be included in further
   * processing.  PennTreeReader recognizes this return value, and
   * asks for another Tree from the input Reader.
   *
   * @param tree The tree to be normalized
   * @param tf   the TreeFactory to create new nodes (if needed)
   * @return Tree the normalized tree
   */
  public Tree normalizeWholeTree(Tree tree, TreeFactory tf) {
    return tree;
  }

  private static readonly long serialVersionUID = 1540681875853883387L;
    }
}
