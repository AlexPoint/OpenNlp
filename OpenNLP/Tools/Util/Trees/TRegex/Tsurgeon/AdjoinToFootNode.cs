using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /** Adjoin in a tree (like in TAG), but retain the target of adjunction as the foot of the auxiliary tree.
 * @author Roger Levy (rog@nlp.stanford.edu)
 */

    public class AdjoinToFootNode : AdjoinNode
    {
        public AdjoinToFootNode(AuxiliaryTree t, TsurgeonPattern p) :
            base("adjoinF", t, p)
        {
        }

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private AdjoinToFootNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, AdjoinToFootNode node)
                :
                    base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                // find match and get its parent
                Tree targetNode = childMatcher[0].evaluate(tree, tregex);
                Tree parent = targetNode.parent(tree);
                // substitute original node for foot of auxiliary tree.  Foot node is ignored
                AuxiliaryTree ft = node.adjunctionTree().copy(this);
                // System.err.println("ft=" + ft + "; ft.foot=" + ft.foot + "; ft.tree=" + ft.tree);
                Tree parentOfFoot = ft.foot.parent(ft.tree);
                if (parentOfFoot == null)
                {
                    //System.err.println("Warning: adjoin to foot for depth-1 auxiliary tree has no effect.");
                    return tree;
                }
                int i = parentOfFoot.objectIndexOf(ft.foot);
                if (parent == null)
                {
                    parentOfFoot.setChild(i, targetNode);
                    return ft.tree;
                }
                else
                {
                    int j = parent.objectIndexOf(targetNode);
                    parent.setChild(j, ft.tree);
                    parentOfFoot.setChild(i, targetNode);
                    return tree;
                }
            }
        }
    }
}