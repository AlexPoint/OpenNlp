using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class CoindexNodes : TsurgeonPattern
    {
        private const string CoindexationIntroductionString = "-";

        public CoindexNodes(TsurgeonPattern[] children) :
            base("coindex", children)
        {
        }

        public override void SetRoot(TsurgeonPatternRoot root)
        {
            base.SetRoot(root);
            root.SetCoindexes();
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
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

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                int newIndex = Coindexer.GenerateIndex();
                foreach (TsurgeonMatcher child in ChildMatcher)
                {
                    Tree node = child.Evaluate(tree, tregex);
                    node.Label().SetValue(node.Label().Value() + CoindexationIntroductionString + newIndex);
                }
                return tree;
            }
        }
    }
}