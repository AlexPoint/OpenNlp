using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class DeleteNode : TsurgeonPattern
    {
        public DeleteNode(TsurgeonPattern[] children) :
            base("delete", children)
        {
        }

        public DeleteNode(List<TsurgeonPattern> children) :
            this(children.ToArray())
        {
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private DeleteNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, DeleteNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree result = tree;
                foreach (TsurgeonMatcher child in ChildMatcher)
                {
                    Tree nodeToDelete = child.Evaluate(tree, tregex);
                    if (nodeToDelete == tree)
                    {
                        result = null;
                    }
                    Tree parent = nodeToDelete.Parent(tree);
                    parent.RemoveChild(Trees.ObjectEqualityIndexOf(parent, nodeToDelete));
                }
                return result;
            }
        }
    }
}