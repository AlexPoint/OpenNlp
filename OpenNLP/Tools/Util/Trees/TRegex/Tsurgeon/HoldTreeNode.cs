using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Trees.TRegex;
using OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class HoldTreeNode : TsurgeonPattern
    {
        private readonly AuxiliaryTree subTree;

        public HoldTreeNode(AuxiliaryTree t) :
            base("hold", TsurgeonPattern.EMPTY_TSURGEON_PATTERN_ARRAY)
        {
            this.subTree = t;
        }

        //@Override
        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly HoldTreeNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, HoldTreeNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                return node.subTree.Copy(this).tree;
            }
        }

        //@Override
        public override string ToString()
        {
            return subTree.ToString();
        }
    }
}