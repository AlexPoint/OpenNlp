using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /** Adjoin in a tree (like in TAG), but retain the target of adjunction as the root of the auxiliary tree.
 * @author Roger Levy (rog@nlp.stanford.edu)
 */

    public class AdjoinToHeadNode : AdjoinNode
    {
        public AdjoinToHeadNode(AuxiliaryTree t, TsurgeonPattern p) :
            base("adjoinH", t, p)
        {
        }

        //@Override
        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly AdjoinToHeadNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, AdjoinToHeadNode node)
                :
                    base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                // find match
                Tree targetNode = childMatcher[0].Evaluate(tree, tregex);
                // put children underneath target in foot of auxilary tree
                AuxiliaryTree ft = node.AdjunctionTree().Copy(this);
                ft.foot.SetChildren(targetNode.GetChildrenAsList());
                // put children of auxiliary tree under target.  root of auxiliary tree is ignored.  root of original is maintained.
                targetNode.SetChildren(ft.tree.GetChildrenAsList());
                return tree;
            }
        }
    }
}