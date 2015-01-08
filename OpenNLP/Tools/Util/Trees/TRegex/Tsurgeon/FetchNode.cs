using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
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
                Tree result = NewNodeNames[node.label];
                if (result == null)
                {
                    result = tregex.GetNode(node.label);
                }
                /*if (result == null) {
                    System.err.println("Warning -- null node fetched by Tsurgeon operation for node: " + this +
                                       " (either no node labeled this, or the labeled node didn't match anything)");
                  }*/
                return result;
            }
        }
    }
}