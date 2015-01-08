using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class TsurgeonPatternRoot : TsurgeonPattern
    {
        public TsurgeonPatternRoot(TsurgeonPattern child) : this(new TsurgeonPattern[] {child})
        {
        }

        public TsurgeonPatternRoot(TsurgeonPattern[] children) : base("operations: ", children)
        {
            SetRoot(this);
        }

        private bool coindexes = false;

        /// <summary>
        /// If one of the children is a CoindexNodes (or something else that
        /// wants coindexing), it can call this at the time of setRoot()
        /// </summary>
        public void SetCoindexes()
        {
            coindexes = true;
        }

        public override TsurgeonMatcher GetMatcher()
        {
            CoindexationGenerator coindexer = null;
            if (coindexes)
            {
                coindexer = new CoindexationGenerator();
            }
            return GetMatcher(new Dictionary<string, Tree>(), coindexer);
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }


        private class Matcher : TsurgeonMatcher
        {
            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer,
                TsurgeonPatternRoot tRoot) :
                    base(tRoot, newNodeNames, coindexer)
            {
            }

            /// <summary>
            /// Returns null if one of the surgeries eliminates the tree entirely.
            /// The operated-on tree is not to be trusted in this instance.
            /// </summary>
            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                if (Coindexer != null)
                {
                    Coindexer.SetLastIndex(tree);
                }
                foreach (TsurgeonMatcher child in ChildMatcher)
                {
                    tree = child.Evaluate(tree, tregex);
                    if (tree == null)
                    {
                        return null;
                    }
                }
                return tree;
            }
        }
    }
}