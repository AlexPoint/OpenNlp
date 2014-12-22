using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Trees.TRegex;
using OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class HoldTreeNode: TsurgeonPattern
    {
        AuxiliaryTree subTree;

  public HoldTreeNode(AuxiliaryTree t):
    base("hold", TsurgeonPattern.EMPTY_TSURGEON_PATTERN_ARRAY){
    this.subTree = t;
  }

  //@Override
  public override TsurgeonMatcher matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer) {
    return new Matcher(newNodeNames, coindexer, this);
  }

  private class Matcher : TsurgeonMatcher
  {
      private HoldTreeNode node;

      public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, HoldTreeNode node) :
          base(node, newNodeNames, coindexer)
      {
          this.node = node;
      }

    //@Override
    public override Tree evaluate(Tree tree, TregexMatcher tregex) {
      return node.subTree.copy(this).tree;
    }
  }

  //@Override
  public override String ToString() {
    return subTree.ToString();
  }
    }
}
