using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// Executes the give children only if the named Tregex node exists in
    /// the TregexMatcher at match time (allows for OR relations or
    /// optional relations)
    /// 
    /// @author John Bauer (horatio@gmail.com)
    /// </summary>
    public class IfExistsNode : TsurgeonPattern
    {
        private readonly string name;
        private readonly bool invert;

        public IfExistsNode(string name, bool invert, TsurgeonPattern[] children) :
            base("if " + (invert ? "not " : "") + "exists " + name, children)
        {
            this.name = name;
            this.invert = invert;
        }

        public override TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private readonly IfExistsNode node;

            public Matcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer, IfExistsNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            public override Tree Evaluate(Tree tree, TregexMatcher tregex)
            {
                if (node.invert ^ (tregex.GetNode(node.name) != null))
                {
                    foreach (TsurgeonMatcher child in ChildMatcher)
                    {
                        child.Evaluate(tree, tregex);
                    }
                }
                return tree;
            }
        }
    }
}