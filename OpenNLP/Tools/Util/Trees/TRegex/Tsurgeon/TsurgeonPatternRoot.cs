using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
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

        /**
   * If one of the children is a CoindexNodes (or something else that
   * wants coindexing), it can call this at the time of setRoot()
   */

        public void SetCoindexes()
        {
            coindexes = true;
        }

        //@Override
        public override TsurgeonMatcher GetMatcher()
        {
            CoindexationGenerator coindexer = null;
            if (coindexes)
            {
                coindexer = new CoindexationGenerator();
            }
            return GetMatcher(new Dictionary<string, Tree>(), coindexer);
        }

        //@Override
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

            /**
     * returns null if one of the surgeries eliminates the tree entirely.  The
     * operated-on tree is not to be trusted in this instance.
     */
            //@Override
            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                if (coindexer != null)
                {
                    coindexer.SetLastIndex(tree);
                }
                foreach (TsurgeonMatcher child in childMatcher)
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