using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A <code>TreeFactory</code> acts as a factory for creating objects of
    /// class <code>Tree</code>, or some descendant class.
    /// Methods implementing this interface may assume that the <code>List</code>
    /// of children passed to them is a list that actually contains trees, but
    /// this can't be enforced in Java without polymorphic types.
    /// The methods with a String argument do not guarantee
    /// that the tree label() will be a String -- the TreeFactory may
    /// convert it into some other type.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ITreeFactory
    {

        /// <summary>
        /// Create a new tree leaf node, where the label is formed from
        /// the <code>string</code> passed in
        /// </summary>
        /// <param name="word">The word that will go into the tree label</param>
        /// <returns>The new leaf</returns>
        Tree NewLeaf(string word);

        /// <summary>
        /// Create a new tree non-leaf node, where the label is formed from
        /// the <code>string</code> passed in
        /// </summary>
        /// <param name="parent">The string that will go into the parent tree label</param>
        /// <param name="children">
        /// The list of daughters of this tree.
        /// The children may be a (possibly empty) <code>List</code> of children or <code>null</code>
        /// </param>
        /// <returns>The new interior tree node</returns>
        Tree NewTreeNode(string parent, List<Tree> children);

        /// <summary>
        /// Create a new tree leaf node, with the given label
        /// </summary>
        /// <param name="label">The label for the leaf node</param>
        /// <returns>The new leaf</returns>
        Tree NewLeaf(ILabel label);

        /// <summary>
        /// Create a new tree non-leaf node, with the given label
        /// </summary>
        /// <param name="label">The label for the parent tree node</param>
        /// <param name="children">
        /// The list of daughters of this tree.  
        /// The children may be a (possibly empty) <code>List</code> of children or <code>null</code>
        /// </param>
        /// <returns>The new interior tree node</returns>
        Tree NewTreeNode(ILabel label, List<Tree> children);
    }
}