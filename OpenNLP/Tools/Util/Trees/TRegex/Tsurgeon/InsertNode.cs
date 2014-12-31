using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class InsertNode: TsurgeonPattern
    {
        TreeLocation location;

  /**
   * Does the item being inserted need to be deep-copied before
   * insertion?
   */
  bool needsCopy = true;

  public InsertNode(TsurgeonPattern child, TreeLocation l):
    base("insert", new TsurgeonPattern[] { child }){
    this.location = l;
  }

  //@Override
  public override void setRoot(TsurgeonPatternRoot root) {
    base.setRoot(root);
    location.setRoot(root);
  }

  public InsertNode(AuxiliaryTree t, TreeLocation l):
    this(new HoldTreeNode(t), l){

    // Copy occurs in HoldTreeNode's `evaluate` method
    needsCopy = false;
  }

  //@Override
  public override TsurgeonMatcher matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer) {
    return new Matcher(newNodeNames, coindexer, this);
  }

  private class Matcher : TsurgeonMatcher {
    TreeLocation.LocationMatcher locationMatcher;
      private InsertNode node;

    public Matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer, InsertNode node):
      base(node, newNodeNames, coindexer)
    {
        this.node = node;
      locationMatcher = node.location.matcher(newNodeNames, coindexer);
    }

    //@Override
    public override Tree evaluate(Tree tree, TregexMatcher tregex) {
      Tree nodeToInsert = childMatcher[0].evaluate(tree, tregex);
      Tuple<Tree,int> position = locationMatcher.evaluate(tree, tregex);
      position.Item1.insertDtr(this.node.needsCopy ? nodeToInsert.deepCopy() : nodeToInsert, 
                                 position.Item2);
      return tree;
    }
  }

  //@Override
  public override String ToString() {
    return label + '(' + children[0] + ',' + location + ')';
  }
    }
}
