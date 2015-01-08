using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// An object factored out to keep the state of a <code>Tsurgeon</code>
    /// operation separate from the <code>TsurgeonPattern</code> objects.
    /// This makes it easier to reset state between invocations and makes
    /// it easier to use in a threadsafe manner.
    /// 
    /// TODO: it would be nice to go through all the patterns and make sure
    /// they update <code>newNodeNames</code> or look for appropriate nodes
    /// in <code>newNodeNames</code> when possible.
    /// 
    /// It would also be nicer if the call to <code>matcher()</code> took
    /// the tree &amp; tregex instead of <code>evaluate()</code>, but that
    /// is a little more complicated because of the way the
    /// <code>TsurgeonMatcher</code> is used in <code>Tsurgeon</code>.
    /// Basically, you would need to move that code from
    /// <code>Tsurgeon</code> to <code>TsurgeonMatcher</code>.
    /// 
    /// @author John Bauer
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class TsurgeonMatcher
    {
        public Dictionary<string, Tree> NewNodeNames;
        public CoindexationGenerator Coindexer;

        public TsurgeonMatcher[] ChildMatcher;

        // TODO: ideally we should have the tree and the tregex matcher be part of this as well.
        // That would involve putting some of the functionality in Tsurgeon.java in this object
        public TsurgeonMatcher(TsurgeonPattern pattern, Dictionary<string, Tree> newNodeNames,
            CoindexationGenerator coindexer)
        {
            this.NewNodeNames = newNodeNames;
            this.Coindexer = coindexer;

            this.ChildMatcher = new TsurgeonMatcher[pattern.children.Length];
            for (int i = 0; i < pattern.children.Length; ++i)
            {
                this.ChildMatcher[i] = pattern.children[i].GetMatcher(newNodeNames, coindexer);
            }
        }

        /// <summary>
        /// Evaluates the surgery pattern against a {@link Tree} and a {@link TregexMatcher}
        /// that has been successfully matched against the tree.
        /// </summary>
        /// <param name="tree">
        /// The {@link Tree} that has been matched upon; typically this tree will be destructively modified.
        /// </param>
        /// <param name="tregex">The successfully matched {@link TregexMatcher}.</param>
        /// <returns>
        /// Some node in the tree; depends on implementation and use of the specific subclass.
        /// </returns>
        public abstract Tree Evaluate(Tree tree, TregexMatcher tregex);
    }
}