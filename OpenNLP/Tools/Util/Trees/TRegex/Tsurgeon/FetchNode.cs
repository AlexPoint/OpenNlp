using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class FetchNode : TsurgeonPattern
    {
        public FetchNode(String nodeName) :
            base(nodeName, TsurgeonPattern.EMPTY_TSURGEON_PATTERN_ARRAY)
        {
        }


        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private FetchNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, FetchNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree result = newNodeNames[node.label];
                if (result == null)
                {
                    result = tregex.getNode(node.label);
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