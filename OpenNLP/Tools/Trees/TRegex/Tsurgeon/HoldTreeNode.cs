using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class HoldTreeNode : TsurgeonPattern
    {
        private readonly AuxiliaryTree subTree;

        public HoldTreeNode(AuxiliaryTree t) :
            base("hold", TsurgeonPattern.EmptyTsurgeonPatternArray)
        {
            this.subTree = t;
        }

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

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                return node.subTree.Copy(this).Tree;
            }
        }

        public override string ToString()
        {
            return subTree.ToString();
        }
    }
}