using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /** Does a delete (NOT prune!) + insert operation
 * @author Roger Levy (rog@stanford.edu)
 */

    public class MoveNode : TsurgeonPattern
    {
        private TreeLocation location;

        public MoveNode(TsurgeonPattern child, TreeLocation l) :
            base("move", new TsurgeonPattern[] {child})
        {
            this.location = l;
        }

        //@Override
        public override void setRoot(TsurgeonPatternRoot root)
        {
            base.setRoot(root);
            location.setRoot(root);
        }

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private TreeLocation.LocationMatcher locationMatcher;
            private MoveNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, MoveNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
                locationMatcher = node.location.matcher(newNodeNames, coindexer);
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree nodeToMove = childMatcher[0].evaluate(tree, tregex);
                Tree oldParent = nodeToMove.parent(tree);
                oldParent.removeChild(Trees.objectEqualityIndexOf(oldParent, nodeToMove));
                Tuple<Tree, int> position = locationMatcher.evaluate(tree, tregex);
                position.Item1.insertDtr(nodeToMove, position.Item2);
                return tree;
            }
        }

        //@Override
        public override string ToString()
        {
            return label + "(" + children[0] + " " + location + ")";
        }
    }
}