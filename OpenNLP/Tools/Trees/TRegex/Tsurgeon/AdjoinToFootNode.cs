using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Adjoin in a tree (like in TAG), but retain the target of adjunction as the foot of the auxiliary tree.
    /// 
    /// @author Roger Levy (rog@nlp.stanford.edu)
    /// </summary>
    public class AdjoinToFootNode : AdjoinNode
    {
        public AdjoinToFootNode(AuxiliaryTree t, TsurgeonPattern p) :
            base("adjoinF", t, p)
        {
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly AdjoinToFootNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, AdjoinToFootNode node)
                : base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                // find match and get its parent
                Tree targetNode = ChildMatcher[0].Evaluate(tree, tregex);
                Tree parent = targetNode.Parent(tree);
                // substitute original node for foot of auxiliary tree.  Foot node is ignored
                AuxiliaryTree ft = node.AdjunctionTree().Copy(this);
                Tree parentOfFoot = ft.Foot.Parent(ft.Tree);
                if (parentOfFoot == null)
                {
                    return tree;
                }
                int i = parentOfFoot.ObjectIndexOf(ft.Foot);
                if (parent == null)
                {
                    parentOfFoot.SetChild(i, targetNode);
                    return ft.Tree;
                }
                else
                {
                    int j = parent.ObjectIndexOf(targetNode);
                    parent.SetChild(j, ft.Tree);
                    parentOfFoot.SetChild(i, targetNode);
                    return tree;
                }
            }
        }
    }
}