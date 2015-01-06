using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * Implements the HeadFinder found in Michael Collins' 1999 thesis.
 * Except: we've added a head rule for NX, which returns the leftmost item.
 * No rule for the head of NX is found in any of the versions of
 * Collins' head table that we have (did he perhaps use the NP rules
 * for NX? -- no Bikel, CL, 2005 says it defaults to leftmost).
 * These rules are suitable for the Penn Treebank.
 * <p>
 * May 2004: Added support for AUX and AUXG to the VP rules; these cause
 * no interference in Penn Treebank parsing, but means that these rules
 * also work for the BLLIP corpus (or Charniak parser output in general).
 * Feb 2005: Fixes to coordination reheading so that punctuation cannot
 * become head.
 *
 * @author Christopher Manning
 */

    public class CollinsHeadFinder : AbstractCollinsHeadFinder
    {
        private static readonly string[] EmptyStringArray = {};

        public CollinsHeadFinder() : this(new PennTreebankLanguagePack())
        {
        }

        /** This constructor provides the traditional behavior, where there is
   *  no special avoidance of punctuation categories.
   *
   *  @param tlp TreebankLanguagePack used for basic category function
   */

        public CollinsHeadFinder(AbstractTreebankLanguagePack tlp) : this(tlp, EmptyStringArray)
        {
        }

        public CollinsHeadFinder(AbstractTreebankLanguagePack tlp, string[] categoriesToAvoid)
            : base(tlp, categoriesToAvoid)
        {

            nonTerminalInfo = new Dictionary<string, string[][]>();
            // This version from Collins' diss (1999: 236-238)
            nonTerminalInfo.Add("ADJP",
                new string[][]
                {
                    new string[]
                    {
                        "left", "NNS", "QP", "NN", "$", "ADVP", "JJ", "VBN", "VBG", "ADJP", "JJR", "NP", "JJS", "DT", "FW",
                        "RBR", "RBS", "SBAR", "RB"
                    }
                });
            nonTerminalInfo.Add("ADVP",
                new string[][]
                {
                    new string[]
                    {"right", "RB", "RBR", "RBS", "FW", "ADVP", "TO", "CD", "JJR", "JJ", "IN", "NP", "JJS", "NN"}
                });
            nonTerminalInfo.Add("CONJP", new string[][] {new string[] {"right", "CC", "RB", "IN"}});
            nonTerminalInfo.Add("FRAG", new string[][] {new string[] {"right"}}); // crap
            nonTerminalInfo.Add("INTJ", new string[][] {new string[] {"left"}});
            nonTerminalInfo.Add("LST", new string[][] {new string[] {"right", "LS", ":"}});
            nonTerminalInfo.Add("NAC",
                new string[][]
                {
                    new string[]
                    {
                        "left", "NN", "NNS", "NNP", "NNPS", "NP", "NAC", "EX", "$", "CD", "QP", "PRP", "VBG", "JJ", "JJS",
                        "JJR", "ADJP", "FW"
                    }
                });
            nonTerminalInfo.Add("NX", new string[][] {new string[] {"left"}}); // crap
            nonTerminalInfo.Add("PP", new string[][] {new string[] {"right", "IN", "TO", "VBG", "VBN", "RP", "FW"}});
            // should prefer JJ? (PP (JJ such) (IN as) (NP (NN crocidolite)))
            nonTerminalInfo.Add("PRN", new string[][] {new string[] {"left"}});
            nonTerminalInfo.Add("PRT", new string[][] {new string[] {"right", "RP"}});
            nonTerminalInfo.Add("QP",
                new string[][]
                {new string[] {"left", "$", "IN", "NNS", "NN", "JJ", "RB", "DT", "CD", "NCD", "QP", "JJR", "JJS"}});
            nonTerminalInfo.Add("RRC", new string[][] {new string[] {"right", "VP", "NP", "ADVP", "ADJP", "PP"}});
            nonTerminalInfo.Add("S",
                new string[][] {new string[] {"left", "TO", "IN", "VP", "S", "SBAR", "ADJP", "UCP", "NP"}});
            nonTerminalInfo.Add("SBAR",
                new string[][]
                {
                    new string[]
                    {"left", "WHNP", "WHPP", "WHADVP", "WHADJP", "IN", "DT", "S", "SQ", "SINV", "SBAR", "FRAG"}
                });
            nonTerminalInfo.Add("SBARQ", new string[][] {new string[] {"left", "SQ", "S", "SINV", "SBARQ", "FRAG"}});
            nonTerminalInfo.Add("SINV",
                new string[][] {new string[] {"left", "VBZ", "VBD", "VBP", "VB", "MD", "VP", "S", "SINV", "ADJP", "NP"}});
            nonTerminalInfo.Add("SQ",
                new string[][] {new string[] {"left", "VBZ", "VBD", "VBP", "VB", "MD", "VP", "SQ"}});
            nonTerminalInfo.Add("UCP", new string[][] {new string[] {"right"}});
            nonTerminalInfo.Add("VP",
                new string[][]
                {
                    new string[]
                    {
                        "left", "TO", "VBD", "VBN", "MD", "VBZ", "VB", "VBG", "VBP", "AUX", "AUXG", "VP", "ADJP", "NN",
                        "NNS", "NP"
                    }
                });
            nonTerminalInfo.Add("WHADJP", new string[][] {new string[] {"left", "CC", "WRB", "JJ", "ADJP"}});
            nonTerminalInfo.Add("WHADVP", new string[][] {new string[] {"right", "CC", "WRB"}});
            nonTerminalInfo.Add("WHNP",
                new string[][] {new string[] {"left", "WDT", "WP", "WP$", "WHADJP", "WHPP", "WHNP"}});
            nonTerminalInfo.Add("WHPP", new string[][] {new string[] {"right", "IN", "TO", "FW"}});
            nonTerminalInfo.Add("X", new string[][] {new string[] {"right"}}); // crap rule
            nonTerminalInfo.Add("NP",
                new string[][]
                {
                    new string[] {"rightdis", "NN", "NNP", "NNPS", "NNS", "NX", "POS", "JJR"}, new string[] {"left", "NP"},
                    new string[] {"rightdis", "$", "ADJP", "PRN"}, new string[] {"right", "CD"},
                    new string[] {"rightdis", "JJ", "JJS", "RB", "QP"}
                });
            nonTerminalInfo.Add("TYPO", new string[][] {new string[] {"left"}}); // another crap rule, for Brown (Roger)
            nonTerminalInfo.Add("EDITED", new string[][] {new string[] {"left"}});
                // crap rule for Switchboard (if don't delete EDITED nodes)
            nonTerminalInfo.Add("XS", new string[][] {new string[] {"right", "IN"}}); // rule for new structure in QP
        }

        //@Override
        protected override int PostOperationFix(int headIdx, Tree[] daughterTrees)
        {
            if (headIdx >= 2)
            {
                string prevLab = tlp.BasicCategory(daughterTrees[headIdx - 1].Value());
                if (prevLab.Equals("CC") || prevLab.Equals("CONJP"))
                {
                    int newHeadIdx = headIdx - 2;
                    Tree t = daughterTrees[newHeadIdx];
                    while (newHeadIdx >= 0 && t.IsPreTerminal() &&
                           tlp.IsPunctuationTag(t.Value()))
                    {
                        newHeadIdx--;
                    }
                    if (newHeadIdx >= 0)
                    {
                        headIdx = newHeadIdx;
                    }
                }
            }
            return headIdx;
        }

    }
}