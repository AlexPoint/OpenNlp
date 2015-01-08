using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Adjoin in a tree (like in TAG), but retain the target of adjunction as the root of the auxiliary tree.
    /// 
    /// @author Roger Levy (rog@nlp.stanford.edu)
    /// </summary>
    public class AdjoinToHeadNode : AdjoinNode
    {
        public AdjoinToHeadNode(AuxiliaryTree t, TsurgeonPattern p) :
            base("adjoinH", t, p)
        {
        }

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

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                // find match
                Tree targetNode = ChildMatcher[0].Evaluate(tree, tregex);
                // put children underneath target in foot of auxilary tree
                AuxiliaryTree ft = node.AdjunctionTree().Copy(this);
                ft.Foot.SetChildren(targetNode.GetChildrenAsList());
                // put children of auxiliary tree under target.  root of auxiliary tree is ignored.  root of original is maintained.
                targetNode.SetChildren(ft.Tree.GetChildrenAsList());
                return tree;
            }
        }
    }
}