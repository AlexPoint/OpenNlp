using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Given the start and end children of a particular node, takes all
    /// children between start and end (including the endpoints) and
    /// combines them in a new node with the given label.
    /// 
    /// @author John Bauer
    /// </summary>
    public class CreateSubtreeNode : TsurgeonPattern
    {
        private readonly AuxiliaryTree auxTree;

        public CreateSubtreeNode(TsurgeonPattern start, AuxiliaryTree tree) :
            this(start, null, tree)
        {
        }

        public CreateSubtreeNode(TsurgeonPattern start, TsurgeonPattern end, AuxiliaryTree tree) :
            base("combineSubtrees",
                (end == null) ? new TsurgeonPattern[] {start} : new TsurgeonPattern[] {start, end})
        {

            this.auxTree = tree;
            FindFoot();
        }

        /**
         * We want to support a command syntax where a simple node label can
         * be given (i.e., without using a tree literal).
         * 
         * Check if this syntax is being used, and simulate a foot if so.
         */

        private void FindFoot()
        {
            if (auxTree.Foot == null)
            {
                if (!auxTree.Tree.IsLeaf())
                    throw new TsurgeonParseException("No foot node found for " + auxTree);

                // Pretend this leaf is a foot node
                auxTree.Foot = auxTree.Tree;
            }
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly CreateSubtreeNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer,
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

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree startChild = ChildMatcher[0].Evaluate(tree, tregex);
                Tree endChild = (ChildMatcher.Length == 2) ? ChildMatcher[1].Evaluate(tree, tregex) : startChild;

                Tree parent = startChild.Parent(tree);

                // sanity check
                if (parent != endChild.Parent(tree))
                {
                    throw new TsurgeonRuntimeException("Parents did not match for trees when applied to " + this);
                }

                AuxiliaryTree treeCopy = node.auxTree.Copy(this);

                // Collect all the children of the parent of the node we care
                // about.  If the child is one of the nodes we care about, or
                // between those two nodes, we add it to a list of inner children.
                // When we reach the second endpoint, we turn that list of inner
                // children into a new node using the newly created label.  All
                // other children are kept in an outer list, with the new node
                // added at the appropriate location.
                var children = new List<Tree>();
                var innerChildren = new List<Tree>();
                bool insideSpan = false;
                foreach (Tree child in parent.Children())
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
                            treeCopy.Foot.SetChildren(innerChildren);
                            children.Add(treeCopy.Tree);
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

                parent.SetChildren(children);

                return tree;
            }
        }
    }
}