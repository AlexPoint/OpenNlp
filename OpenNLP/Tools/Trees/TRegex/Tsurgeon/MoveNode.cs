using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Does a delete (NOT prune!) + insert operation
    /// 
    /// @author Roger Levy (rog@stanford.edu)
    /// </summary>
    public class MoveNode : TsurgeonPattern
    {
        private readonly TreeLocation location;

        public MoveNode(TsurgeonPattern child, TreeLocation l) :
            base("move", new TsurgeonPattern[] {child})
        {
            this.location = l;
        }

        public override void SetRoot(TsurgeonPatternRoot root)
        {
            base.SetRoot(root);
            location.SetRoot(root);
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly TreeLocation.LocationMatcher locationMatcher;
            private MoveNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, MoveNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
                locationMatcher = node.location.Matcher(newNodeNames, coindexer);
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree nodeToMove = ChildMatcher[0].Evaluate(tree, tregex);
                Tree oldParent = nodeToMove.Parent(tree);
                oldParent.RemoveChild(Trees.ObjectEqualityIndexOf(oldParent, nodeToMove));
                Tuple<Tree, int> position = locationMatcher.Evaluate(tree, tregex);
                position.Item1.InsertDtr(nodeToMove, position.Item2);
                return tree;
            }
        }

        public override string ToString()
        {
            return label + "(" + children[0] + " " + location + ")";
        }
    }
}