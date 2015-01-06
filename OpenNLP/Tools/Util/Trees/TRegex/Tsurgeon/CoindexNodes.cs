using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class CoindexNodes : TsurgeonPattern
    {
        private static readonly string coindexationIntroductionString = "-";

        public CoindexNodes(TsurgeonPattern[] children) :
            base("coindex", children)
        {
        }

        //@Override
        public override void setRoot(TsurgeonPatternRoot root)
        {
            base.setRoot(root);
            root.setCoindexes();
        }

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private CoindexNodes node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, CoindexNodes node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                int newIndex = coindexer.generateIndex();
                foreach (TsurgeonMatcher child in childMatcher)
                {
                    Tree node = child.evaluate(tree, tregex);
                    node.label().setValue(node.label().value() + coindexationIntroductionString + newIndex);
                }
                return tree;
            }
        }
    }
}