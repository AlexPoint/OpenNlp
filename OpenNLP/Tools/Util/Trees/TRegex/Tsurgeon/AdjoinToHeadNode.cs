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
    public class AdjoinToHeadNode:AdjoinNode
    {
        public AdjoinToHeadNode(AuxiliaryTree t, TsurgeonPattern p):
    base("adjoinH", t, p){}

  //@Override
  public override TsurgeonMatcher matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer) {
    return new Matcher(newNodeNames, coindexer, this);
  }

  private class Matcher: TsurgeonMatcher
  {
      private AdjoinToHeadNode node;
      public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, AdjoinToHeadNode node) :
          base(node, newNodeNames, coindexer)
      {
          this.node = node;
      }

    //@Override
    public override Tree evaluate(Tree tree, TregexMatcher tregex) {
      // find match
      Tree targetNode = childMatcher[0].evaluate(tree, tregex);
      // put children underneath target in foot of auxilary tree
      AuxiliaryTree ft = node.adjunctionTree().copy(this);
      ft.foot.setChildren(targetNode.getChildrenAsList());
      // put children of auxiliary tree under target.  root of auxiliary tree is ignored.  root of original is maintained.
      targetNode.setChildren(ft.tree.getChildrenAsList());
      return tree;
    }
  }
    }
}
