using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /**  Pruning differs from deleting in that if a non-terminal node winds up having no children, it is pruned as well.
 * @author Roger Levy (rog@nlp.stanford.edu)
 */

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

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private PruneNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, PruneNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                bool prunedWholeTree = false;
                foreach (TsurgeonMatcher child in childMatcher)
                {
                    Tree nodeToPrune = child.evaluate(tree, tregex);
                    if (pruneHelper(tree, nodeToPrune) == null)
                        prunedWholeTree = true;
                }
                return prunedWholeTree ? null : tree;
            }
        }

        private static Tree pruneHelper(Tree root, Tree nodeToPrune)
        {
            if (nodeToPrune == root)
                return null;
            Tree parent = nodeToPrune.parent(root);
            parent.removeChild(Trees.objectEqualityIndexOf(parent, nodeToPrune));
            if (parent.children().Length == 0)
                return pruneHelper(root, parent);
            return root;
        }
    }
}