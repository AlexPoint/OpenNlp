using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    public interface TreeFactory
    {
        /**
   * Create a new tree leaf node, where the label is formed from
   * the <code>string</code> passed in.
   *
   * @param word The word that will go into the tree label.
   * @return The new leaf
   */
        Tree newLeaf(string word);


        /**
         * Create a new tree non-leaf node, where the label is formed from
         * the <code>string</code> passed in.
         *
         * @param parent   The string that will go into the parent tree label.
         * @param children The list of daughters of this tree.  The children
         *                 may be a (possibly empty) <code>List</code> of children or
         *                 <code>null</code>
         * @return The new interior tree node
         */
        Tree newTreeNode(string parent, List<Tree> children);


        /**
         * Create a new tree leaf node, with the given label.
         *
         * @param label The label for the leaf node
         * @return The new leaf
         */
        Tree newLeaf(Label label);


        /**
         * Create a new tree non-leaf node, with the given label.
         *
         * @param label    The label for the parent tree node.
         * @param children The list of daughters of this tree.  The children
         *                 may be a (possibly empty) <code>List</code> of children or
         *                 <code>null</code>
         * @return The new interior tree node
         */
        Tree newTreeNode(Label label, List<Tree> children);
    }
}