using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class InsertNode : TsurgeonPattern
    {
        private readonly TreeLocation location;

        // TODO Does the item being inserted need to be deep-copied before insertion?
        private readonly bool needsCopy = true;

        public InsertNode(TsurgeonPattern child, TreeLocation l) :
            base("insert", new TsurgeonPattern[] {child})
        {
            this.location = l;
        }

        public override void SetRoot(TsurgeonPatternRoot root)
        {
            base.SetRoot(root);
            location.SetRoot(root);
        }

        public InsertNode(AuxiliaryTree t, TreeLocation l) :
            this(new HoldTreeNode(t), l)
        {
            // Copy occurs in HoldTreeNode's `evaluate` method
            needsCopy = false;
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly TreeLocation.LocationMatcher locationMatcher;
            private readonly InsertNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, InsertNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
                locationMatcher = node.location.Matcher(newNodeNames, coindexer);
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree nodeToInsert = ChildMatcher[0].Evaluate(tree, tregex);
                Tuple<Tree, int> position = locationMatcher.Evaluate(tree, tregex);
                position.Item1.InsertDtr(this.node.needsCopy ? nodeToInsert.DeepCopy() : nodeToInsert,
                    position.Item2);
                return tree;
            }
        }

        public override string ToString()
        {
            return label + '(' + children[0] + ',' + location + ')';
        }
    }
}