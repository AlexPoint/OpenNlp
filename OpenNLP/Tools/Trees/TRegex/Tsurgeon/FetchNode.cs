using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class FetchNode : TsurgeonPattern
    {
        public FetchNode(string nodeName) :
            base(nodeName, TsurgeonPattern.EmptyTsurgeonPatternArray)
        {
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly FetchNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, FetchNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree result;
                NewNodeNames.TryGetValue(node.label, out result);
                if (result == null)
                {
                    result = tregex.GetNode(node.label);
                }
                return result;
            }
        }
    }
}