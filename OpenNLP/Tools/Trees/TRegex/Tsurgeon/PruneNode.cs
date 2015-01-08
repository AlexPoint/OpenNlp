using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Pruning differs from deleting in that if a non-terminal node winds up having no children, it is pruned as well.
    /// 
    /// @author Roger Levy (rog@nlp.stanford.edu)
    /// </summary>
    public class PruneNode : TsurgeonPattern
    {
        public PruneNode(TsurgeonPattern[] children) :
            base("prune", children)
        {
        }

        public PruneNode(List<TsurgeonPattern> children) :
            this(children.ToArray())
        {
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private PruneNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, PruneNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                bool prunedWholeTree = false;
                foreach (TsurgeonMatcher child in ChildMatcher)
                {
                    Tree nodeToPrune = child.Evaluate(tree, tregex);
                    if (PruneHelper(tree, nodeToPrune) == null)
                        prunedWholeTree = true;
                }
                return prunedWholeTree ? null : tree;
            }
        }

        private static Tree PruneHelper(Tree root, Tree nodeToPrune)
        {
            if (nodeToPrune == root)
                return null;
            Tree parent = nodeToPrune.Parent(root);
            parent.RemoveChild(Trees.ObjectEqualityIndexOf(parent, nodeToPrune));
            if (parent.Children().Length == 0)
                return PruneHelper(root, parent);
            return root;
        }
    }
}