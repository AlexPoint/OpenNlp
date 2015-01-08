using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Adjoin in a tree (like in TAG).
    /// 
    /// @author Roger Levy (rog@nlp.stanford.edu)
    /// </summary>
    public class AdjoinNode : TsurgeonPattern
    {
        private readonly AuxiliaryTree padjunctionTree;

        public AdjoinNode(AuxiliaryTree t, TsurgeonPattern p) :
            this("adjoin", t, p)
        {
        }

        public AdjoinNode(string name, AuxiliaryTree t, TsurgeonPattern p) :
            base(name, new TsurgeonPattern[] {p})
        {
            if (t == null || p == null)
            {
                throw new NullReferenceException("AdjoinNode: illegal null argument, t=" + t + ", p=" + p);
            }
            padjunctionTree = t;
        }

        protected AuxiliaryTree AdjunctionTree()
        {
            return padjunctionTree;
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly AdjoinNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, AdjoinNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                // find match and get its parent
                Tree targetNode = ChildMatcher[0].Evaluate(tree, tregex);
                Tree parent = targetNode.Parent(tree);
                // put children underneath target in foot of auxilary tree
                AuxiliaryTree ft = node.padjunctionTree.Copy(this);
                ft.Foot.SetChildren(targetNode.GetChildrenAsList());
                // replace match with root of auxiliary tree
                if (parent == null)
                {
                    return ft.Tree;
                }
                else
                {
                    int i = parent.ObjectIndexOf(targetNode);
                    parent.SetChild(i, ft.Tree);
                    return tree;
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + "<-" + padjunctionTree;
        }
    }
}