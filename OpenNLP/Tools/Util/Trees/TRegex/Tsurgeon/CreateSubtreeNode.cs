using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /**
 * Given the start and end children of a particular node, takes all
 * children between start and end (including the endpoints) and
 * combines them in a new node with the given label.
 *
 * @author John Bauer
 */

    public class CreateSubtreeNode : TsurgeonPattern
    {
        private AuxiliaryTree auxTree;

        public CreateSubtreeNode(TsurgeonPattern start, AuxiliaryTree tree) :
            this(start, null, tree)
        {
        }

        public CreateSubtreeNode(TsurgeonPattern start, TsurgeonPattern end, AuxiliaryTree tree) :
            base("combineSubtrees",
                (end == null) ? new TsurgeonPattern[] {start} : new TsurgeonPattern[] {start, end})
        {

            this.auxTree = tree;
            findFoot();
        }

        /**
   * We want to support a command syntax where a simple node label can
   * be given (i.e., without using a tree literal).
   *
   * Check if this syntax is being used, and simulate a foot if so.
   */

        private void findFoot()
        {
            if (auxTree.foot == null)
            {
                if (!auxTree.tree.isLeaf())
                    throw new TsurgeonParseException("No foot node found for " + auxTree);

                // Pretend this leaf is a foot node
                auxTree.foot = auxTree.tree;
            }
        }

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private CreateSubtreeNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer,
                CreateSubtreeNode node) :
                    base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            /**
     * Combines all nodes between start and end into one subtree, then
     * replaces those nodes with the new subtree in the corresponding
     * location under parent
     */
            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree startChild = childMatcher[0].evaluate(tree, tregex);
                Tree endChild = (childMatcher.Length == 2) ? childMatcher[1].evaluate(tree, tregex) : startChild;

                Tree parent = startChild.parent(tree);

                // sanity check
                if (parent != endChild.parent(tree))
                {
                    throw new TsurgeonRuntimeException("Parents did not match for trees when applied to " + this);
                }

                AuxiliaryTree treeCopy = node.auxTree.copy(this);

                // Collect all the children of the parent of the node we care
                // about.  If the child is one of the nodes we care about, or
                // between those two nodes, we add it to a list of inner children.
                // When we reach the second endpoint, we turn that list of inner
                // children into a new node using the newly created label.  All
                // other children are kept in an outer list, with the new node
                // added at the appropriate location.
                List<Tree> children = new List<Tree>();
                List<Tree> innerChildren = new List<Tree>();
                bool insideSpan = false;
                foreach (Tree child in parent.children())
                {
                    if (child == startChild || child == endChild)
                    {
                        if (!insideSpan && startChild != endChild)
                        {
                            insideSpan = true;
                            innerChildren.Add(child);
                        }
                        else
                        {
                            insideSpan = false;
                            innerChildren.Add(child);

                            // All children have been collected; place these beneath the foot of the auxiliary tree
                            treeCopy.foot.setChildren(innerChildren);
                            children.Add(treeCopy.tree);
                        }
                    }
                    else if (insideSpan)
                    {
                        innerChildren.Add(child);
                    }
                    else
                    {
                        children.Add(child);
                    }
                }

                parent.setChildren(children);

                return tree;
            }
        }
    }
}