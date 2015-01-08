using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Excises all nodes from the top to the bottom, and puts all the children of bottom node in where the top was.
    /// 
    /// @author Roger Levy (rog@stanford.edu)
    /// </summary>
    public class ExciseNode : TsurgeonPattern
    {
        /// <summary>
        /// Top should evaluate to a node that dominates bottom, but this is not checked!
        /// </summary>
        public ExciseNode(TsurgeonPattern top, TsurgeonPattern bottom) :
            base("excise", new TsurgeonPattern[] {top, bottom})
        {
        }

        /// <summary>
        /// Excises only the directed node
        /// </summary>
        public ExciseNode(TsurgeonPattern node) :
            base("excise", new TsurgeonPattern[] {node, node})
        {
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private ExciseNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, ExciseNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                Tree topNode = ChildMatcher[0].Evaluate(tree, tregex);
                Tree bottomNode = ChildMatcher[1].Evaluate(tree, tregex);
                if (topNode == tree)
                {
                    if (bottomNode.Children().Length == 1)
                    {
                        return bottomNode.Children()[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                Tree parent = topNode.Parent(tree);
                int i = Trees.ObjectEqualityIndexOf(parent, topNode);
                parent.RemoveChild(i);
                foreach (Tree child in bottomNode.Children())
                {
                    parent.AddChild(i, child);
                    i++;
                }
                return tree;
            }
        }
    }
}