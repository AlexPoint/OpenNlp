using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class ReplaceNode : TsurgeonPattern
    {
        public ReplaceNode(TsurgeonPattern oldNode, TsurgeonPattern[] newNodes) :
            base("replace", new TsurgeonPattern[] {oldNode}.Union(newNodes).ToArray())
        {
        }

        public ReplaceNode(TsurgeonPattern oldNode, List<AuxiliaryTree> trees) :
            this(oldNode, trees.Select(n => convertAuxiliaryToHold(n)).ToArray())
        {
        }

        private static readonly Func<AuxiliaryTree, HoldTreeNode> convertAuxiliaryToHold = t => new HoldTreeNode(t);

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private ReplaceNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, ReplaceNode node) :
                base(node, newNodeNames, coindexer)
            {
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree oldNode = childMatcher[0].evaluate(tree, tregex);
                if (oldNode == tree)
                {
                    if (node.children.Length > 2)
                    {
                        throw new TsurgeonRuntimeException(
                            "Attempted to replace a root node with more than one node, unable to proceed");
                    }
                    return childMatcher[1].evaluate(tree, tregex);
                }
                Tree parent = oldNode.parent(tree);
                int i = parent.objectIndexOf(oldNode);
                parent.removeChild(i);
                for (int j = 1; j < node.children.Length; ++j)
                {
                    Tree newNode = childMatcher[j].evaluate(tree, tregex);
                    parent.insertDtr(newNode.deepCopy(), i + j - 1);
                }
                return tree;
            }
        }
    }
}