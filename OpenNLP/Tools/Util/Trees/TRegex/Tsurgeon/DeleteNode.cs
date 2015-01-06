using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
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

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private DeleteNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, DeleteNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree result = tree;
                foreach (TsurgeonMatcher child in childMatcher)
                {
                    Tree nodeToDelete = child.evaluate(tree, tregex);
                    if (nodeToDelete == tree)
                    {
                        result = null;
                    }
                    Tree parent = nodeToDelete.parent(tree);
                    parent.removeChild(Trees.objectEqualityIndexOf(parent, nodeToDelete));
                }
                return result;
            }
        }
    }
}