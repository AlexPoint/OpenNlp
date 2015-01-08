using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// This is a simple interface for a function that alters a
    /// local <code>Tree</code>.
    /// 
    /// @author Christopher Manning.
    /// </summary>
    public interface ITreeTransformer
    {
        /// <summary>
        /// Does whatever one needs to do to a particular tree.
        /// This routine is passed a whole <code>Tree</code>, and could itself
        /// work recursively, but the canonical usage is to invoke this method
        /// via the <code>Tree.transform()</code> method, which will apply the
        /// transformer in a bottom-up manner to each local <code>Tree</code>,
        /// and hence the implementation of <code>TreeTransformer</code> should
        /// merely examine and change a local (one-level) <code>Tree</code>.
        /// </summary>
        /// <param name="t">
        /// A tree.  Classes implementing this interface can assume
        /// that the tree passed in is not <code>null</code>
        /// </param>
        /// <returns>the transformed <code>Tree</code></returns>
        Tree TransformTree(Tree t);
    }
}