using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /**
 * An abstract class for patterns to manipulate {@link Tree}s when
 * successfully matched on with a {@link TregexMatcher}.
 *
 * @author Roger Levy
 */

    public abstract class TsurgeonPattern
    {
        public static readonly TsurgeonPattern[] EMPTY_TSURGEON_PATTERN_ARRAY = new TsurgeonPattern[0];

        public readonly String label;
        public readonly TsurgeonPattern[] children;

        private TsurgeonPattern root; // TODO: can remove?

        public virtual void setRoot(TsurgeonPatternRoot root)
        {
            this.root = root;
            foreach (TsurgeonPattern child in children)
            {
                child.setRoot(root);
            }
        }

        /**
   * In some cases, the order of the children has special meaning.
   * For example, in the case of ReplaceNode, the first child will
   * evaluate to the node to be replaced, and the other(s) will
   * evaluate to the replacement.
   */

        public TsurgeonPattern(String label, TsurgeonPattern[] children)
        {
            this.label = label;
            this.children = children;
        }

        //@Override
        public override String ToString()
        {
            StringBuilder resultSB = new StringBuilder();
            resultSB.Append(label);
            if (children.Length > 0)
            {
                resultSB.Append('(');
                for (int i = 0; i < children.Length; i++)
                {
                    resultSB.Append(children[i]);
                    if (i < children.Length - 1)
                    {
                        resultSB.Append(", ");
                    }
                }
                resultSB.Append(')');
            }
            return resultSB.ToString();
        }

        public virtual TsurgeonMatcher matcher()
        {
            throw new InvalidOperationException("Only the root node can produce the top level matcher");
        }

        public abstract TsurgeonMatcher matcher(Dictionary<String, Tree> newNodeNames, CoindexationGenerator coindexer);
    }
}