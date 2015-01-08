using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
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

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly ReplaceNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, ReplaceNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree oldNode = ChildMatcher[0].Evaluate(tree, tregex);
                if (oldNode == tree)
                {
                    if (node.children.Length > 2)
                    {
                        throw new TsurgeonRuntimeException(
                            "Attempted to replace a root node with more than one node, unable to proceed");
                    }
                    return ChildMatcher[1].Evaluate(tree, tregex);
                }
                Tree parent = oldNode.Parent(tree);
                int i = parent.ObjectIndexOf(oldNode);
                parent.RemoveChild(i);
                for (int j = 1; j < node.children.Length; ++j)
                {
                    Tree newNode = ChildMatcher[j].Evaluate(tree, tregex);
                    parent.InsertDtr(newNode.DeepCopy(), i + j - 1);
                }
                return tree;
            }
        }
    }
}