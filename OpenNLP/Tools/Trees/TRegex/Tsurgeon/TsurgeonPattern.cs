using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// An abstract class for patterns to manipulate {@link Tree}s when
    /// successfully matched on with a {@link TregexMatcher}.
    /// 
    /// @author Roger Levy
    /// </summary>
    public abstract class TsurgeonPattern
    {
                public static readonly TsurgeonPattern[] EmptyTsurgeonPatternArray = new TsurgeonPattern[0];

        public readonly string label;
        public readonly TsurgeonPattern[] children;

        private TsurgeonPattern root; // TODO: can remove?

        public virtual void SetRoot(TsurgeonPatternRoot root)
        {
            this.root = root;
            foreach (TsurgeonPattern child in children)
            {
                child.SetRoot(root);
            }
        }

        /// <summary>
        /// In some cases, the order of the children has special meaning.
        /// For example, in the case of ReplaceNode, the first child will
        /// evaluate to the node to be replaced, and the other(s) will
        /// evaluate to the replacement.
        /// </summary>
        public TsurgeonPattern(string label, TsurgeonPattern[] children)
        {
            this.label = label;
            this.children = children;
        }

        public override string ToString()
        {
            var resultSb = new StringBuilder();
            resultSb.Append(label);
            if (children.Length > 0)
            {
                resultSb.Append('(');
                for (int i = 0; i < children.Length; i++)
                {
                    resultSb.Append(children[i]);
                    if (i < children.Length - 1)
                    {
                        resultSb.Append(", ");
                    }
                }
                resultSb.Append(')');
            }
            return resultSb.ToString();
        }

        public virtual TsurgeonMatcher GetMatcher()
        {
            throw new InvalidOperationException("Only the root node can produce the top level matcher");
        }

        public abstract TsurgeonMatcher GetMatcher(Dictionary<string, Tree> newNodeNames, CoindexationGenerator coindexer);
    }
}