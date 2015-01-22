using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// This class implements the <code>TreeReader</code> interface to read Penn Treebank-style
    /// files. The reader is implemented as a push-down automaton (PDA) that parses the Lisp-style
    /// format in which the trees are stored. This reader is compatible with both PTB
    /// and PATB trees.
    /// 
    /// One small detail to note is that the <code>PennTreeReader</code>
    /// silently replaces \* with * and \/ with /.  Two possible designs
    /// for this were to make the <code>PennTreeReader</code> always do
    /// this or to make the <code>TreeNormalizers</code> do this.  We
    /// decided to put it in the <code>PennTreeReader</code> class itself
    /// to avoid the problem of people making new
    /// <code>TreeNormalizers</code> and forgetting to include the unescaping.
    /// 
    /// @author Christopher Manning
    /// @author Roger Levy
    /// @author Spence Green
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class PennTreeReader : ITreeReader
    {
        private readonly TextReader reader;
        private readonly ITokenizer<string> tokenizer;
        private readonly TreeNormalizer treeNormalizer;
        private readonly ITreeFactory treeFactory;
        
        private Tree currentTree;
        // misuse a list as a stack, since we want to avoid the synchronized and old Stack, but don't need the power and JDK 1.6 dependency of a Deque
        private List<Tree> stack;
        private const string LeftParen = "(";
        private const string RightParen = ")";

        /// <summary>
        /// Read parse trees from a <code>Reader</code>.
        /// For the defaulted arguments, you get a
        /// <code>SimpleTreeFactory</code>, no <code>TreeNormalizer</code>, and
        /// a <code>PennTreebankTokenizer</code>.
        /// </summary>
        /// <param name="input">The <code>Reader</code></param>
        public PennTreeReader(TextReader input) : this(input, new LabeledScoredTreeFactory())
        {
        }

                /// <summary>
        /// Read parse trees from a <code>Reader</code>.
        /// </summary>
        /// <param name="input">the Reader</param>
        /// <param name="tf">TreeFactory -- factory to create some kind of Tree</param>
        public PennTreeReader(TextReader input, ITreeFactory tf) :
            this(input, tf, null, new PennTreebankTokenizer(input))
        {
        }

        /// <summary>
        /// Read parse trees from a Reader.
        /// </summary>
        /// <param name="input">The Reader</param>
        /// <param name="tf">TreeFactory -- factory to create some kind of Tree</param>
        /// <param name="tn">the method of normalizing trees</param>
        public PennTreeReader(TextReader input, ITreeFactory tf, TreeNormalizer tn) :
            this(input, tf, tn, new PennTreebankTokenizer(input))
        {
        }

        /// <summary>
        /// Read parse trees from a Reader.
        /// </summary>
        /// <param name="input">The Reader</param>
        /// <param name="tf">TreeFactory -- factory to create some kind of Tree</param>
        /// <param name="tn">the method of normalizing trees</param>
        /// <param name="st">Tokenizer that divides up Reader</param>
        public PennTreeReader(TextReader input, ITreeFactory tf, TreeNormalizer tn, ITokenizer<string> st)
        {
            reader = input;
            treeFactory = tf;
            treeNormalizer = tn;
            tokenizer = st;

            // check for whacked out headers still present in Brown corpus in Treebank 3
            string first = (st.HasNext() ? st.Peek() : null);
            if (first != null && first.StartsWith("*x*x*x"))
            {
                int foundCount = 0;
                while (foundCount < 4 && st.HasNext())
                {
                    first = st.Next();
                    if (first != null && first.StartsWith("*x*x*x"))
                    {
                        foundCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Reads a single tree in standard Penn Treebank format from the
        /// input stream. The method supports additional parentheses around the
        /// tree (an unnamed ROOT node) so long as they are balanced. If the token stream
        /// ends before the current tree is complete, then the method will throw an
        /// <code>IOException</code>.
        /// 
        /// Note that the method will skip malformed trees and attempt to
        /// read additional trees from the input stream. It is possible, however,
        /// that a malformed tree will corrupt the token stream. In this case,
        /// an <code>IOException</code> will eventually be thrown.
        /// </summary>
        /// <returns>A single tree, or <code>null</code> at end of token stream.</returns>
        public Tree ReadTree()
        {
            Tree t = null;

            while (tokenizer.HasNext() && t == null)
            {

                //Setup PDA
                this.currentTree = null;
                this.stack = new List<Tree>();

                try
                {
                    t = GetTreeFromInputStream();
                }
                catch (Exception e)
                {
                    throw new IOException("End of token stream encountered before parsing could complete.");
                }

                if (t != null)
                {
                    // cdm 20100618: Don't do this!  This was never the historical behavior!!!
                    // Escape empty trees e.g. (())
                    // while(t != null && (t.value() == null || t.value().equals("")) && t.numChildren() <= 1)
                    //   t = t.firstChild();

                    if (treeNormalizer != null && treeFactory != null)
                    {
                        t = treeNormalizer.NormalizeWholeTree(t, treeFactory);
                    }
                    t.IndexLeaves(true);
                }
            }

            return t;
        }

        private static readonly Regex StarPattern = new Regex("\\\\\\*");
        private static readonly Regex SlashPattern = new Regex("\\\\/");


        private Tree GetTreeFromInputStream()
        {
            int wordIndex = 0;

            // FSA
            //label:
            while (tokenizer.HasNext())
            {
                string token = tokenizer.Next();

                switch (token)
                {
                    case LeftParen:

                        // cdm 20100225: This next line used to have "" instead of null, but the traditional and current tree normalizers depend on the label being null not "" when there is no label on a tree (like the outermost English PTB level)
                        string label = (tokenizer.Peek().Equals(LeftParen)) ? null : tokenizer.Next();
                        if (RightParen.Equals(label))
                        {
//Skip past empty trees
                            continue;
                        }
                        else if (treeNormalizer != null)
                        {
                            label = treeNormalizer.NormalizeNonterminal(label);
                        }

                        if (label != null)
                        {
                            label = StarPattern.Replace(label, "*");
                            label = SlashPattern.Replace(label, "/");
                        }

                        Tree newTree = treeFactory.NewTreeNode(label, null); // dtrs are added below

                        if (currentTree == null)
                            stack.Add(newTree);
                        else
                        {
                            currentTree.AddChild(newTree);
                            stack.Add(currentTree);
                        }

                        currentTree = newTree;

                        break;
                    case RightParen:
                        if (!stack.Any())
                        {
                            // Warn that file has too many right parens
                            //break label;
                            goto post_while_label;
                        }

                        //Accept
                        currentTree = stack.Last();
                        stack.RemoveAt(stack.Count - 1); // i.e., stack.pop()

                        if (!stack.Any()) return currentTree;

                        break;
                    default:

                        if (currentTree == null)
                        {
                            // A careful Reader should warn here, but it's kind of useful to
                            // suppress this because then the TreeReader doesn't print a ton of
                            // messages if there is a README file in a directory of Trees.
                            //break label;
                            goto post_while_label;
                        }

                        string terminal = (treeNormalizer == null) ? token : treeNormalizer.NormalizeTerminal(token);
                        terminal = StarPattern.Replace(terminal, "*");
                        terminal = SlashPattern.Replace(terminal, "/");
                        Tree leaf = treeFactory.NewLeaf(terminal);
                        if (leaf.Label() is IHasIndex)
                        {
                            var hi = (IHasIndex) leaf.Label();
                            hi.SetIndex(wordIndex);
                        }
                        if (leaf.Label() is IHasWord)
                        {
                            var hw = (IHasWord) leaf.Label();
                            hw.SetWord(leaf.Label().Value());
                        }
                        wordIndex++;

                        currentTree.AddChild(leaf);
                        // cdm: Note: this implementation just isn't as efficient as the old recursive descent parser (see 2008 code), where all the daughters are gathered before the tree is made....
                        break;
                }
            }
            post_while_label:
            {
            }

            //Reject
            return null;
        }

        /// <summary>
        /// Closes the underlying <code>Reader</code> used to create this class.
        /// </summary>
        public void Close()
        {
            reader.Close();
        }

    }
}