using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /**
 * Executes the give children only if the named Tregex node exists in
 * the TregexMatcher at match time (allows for OR relations or
 * optional relations)
 *
 * @author John Bauer (horatio@gmail.com)
 */

    public class IfExistsNode : TsurgeonPattern
    {
        private readonly String name;
        private readonly bool invert;

        public IfExistsNode(String name, bool invert, TsurgeonPattern[] children) :
            base("if " + (invert ? "not " : "") + "exists " + name, children)
        {
            this.name = name;
            this.invert = invert;
        }

        //@Override
        public override TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer)
        {
            return new Matcher(newNodeNames, coindexer, this);
        }

        private class Matcher : TsurgeonMatcher
        {
            private IfExistsNode node;

            public Matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer, IfExistsNode node) :
                base(node, newNodeNames, coindexer)
            {
                this.node = node;
            }

            //@Override
            public override Tree evaluate(Tree tree, TregexMatcher tregex)
            {
                if (node.invert ^ (tregex.getNode(node.name) != null))
                {
                    foreach (TsurgeonMatcher child in childMatcher)
                    {
                        child.evaluate(tree, tregex);
                    }
                }
                return tree;
            }
        }
    }
}