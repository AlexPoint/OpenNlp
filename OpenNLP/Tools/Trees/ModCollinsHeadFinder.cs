using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Implements a variant on the HeadFinder found in Michael Collins' 1999
    /// thesis. This starts with Collins' head finder. As in {@code CollinsHeadFinder}, 
    /// we've added a head rule for NX.
    /// 
    /// Changes:
    /// <ol>
    /// <li>The PRN rule used to just take the leftmost thing, we now have it
    /// choose the leftmost lexical category (not the common punctuation etc.)</li>
    /// <li>Delete IN as a possible head of S, and add FRAG (low priority)</li>
    /// <li>Place NN before QP in ADJP head rules (more to do for ADJP!)</li>
    /// <li>Place PDT before RB and after CD in QP rules.  Also prefer CD to
    /// DT or RB.  And DT to RB.</li>
    /// <li>Add DT, WDT as low priority choice for head of NP. Add PRP before PRN
    /// Add RBR as low priority choice of head for NP.</li>
    /// <li>Prefer NP or NX as head of NX, and otherwise default to rightmost not
    /// leftmost (NP-like headedness)</li>
    /// <li>VP: add JJ and NNP as low priority heads (many tagging errors)
    /// Place JJ above NP in priority, as it is to be preferred to NP object.</li>
    /// <li>PP: add PP as a possible head (rare conjunctions)</li>
    /// <li>Added rule for POSSP (can be introduced by parser)</li>
    /// <li>Added a sensible-ish rule for X.</li>
    /// <li>Added NML head rules, which are the same as for NP.</li>
    /// <li>NP head rule: NP and NML are treated almost identically (NP has precedence)</li>
    /// <li>NAC head rule: NML comes after NN/NNS but after NNP/NNPS</li>
    /// <li>PP head rule: JJ added</li>
    /// <li>Added JJP (appearing in David Vadas's annotation), which seems to play the same role as ADJP.</li>
    /// </ol>
    /// These rules are suitable for the Penn Treebank.
    /// 
    /// A case that you apparently just can't handle well in this framework is
    /// (NP (NP ... NP)).  If this is a conjunction, apposition or similar, then
    /// the leftmost NP is the head, but if the first is a measure phrase like
    /// (NP $ 38) (NP a share) then the second should probably be the head.
    /// 
    /// @author Christopher Manning
    /// @author Michel Galley
    /// 
    /// Code...
    /// </summary>
    public class ModCollinsHeadFinder : CollinsHeadFinder
    {
        public ModCollinsHeadFinder() : this(new PennTreebankLanguagePack())
        {
        }

        public ModCollinsHeadFinder(AbstractTreebankLanguagePack tlp) : base(tlp, tlp.PunctuationTags())
        {
            // avoid punctuation as head in readonly default rule

            nonTerminalInfo = new Dictionary<string, string[][]>();

            // This version from Collins' diss (1999: 236-238)
            // NNS, NN is actually sensible (money, etc.)!
            // QP early isn't; should prefer JJR NN RB
            // remove ADVP; it just shouldn't be there.
            // if two JJ, should take right one (e.g. South Korean)
            // nonTerminalInfo.Add("ADJP", new string[][]{{"left", "NNS", "NN", "$", "QP"}, {"right", "JJ"}, {"left", "VBN", "VBG", "ADJP", "JJP", "JJR", "NP", "JJS", "DT", "FW", "RBR", "RBS", "SBAR", "RB"}});
            nonTerminalInfo.Add("ADJP",
                new string[][]
                {
                    new string[] {"left", "$"}, new string[] {"rightdis", "NNS", "NN", "JJ", "QP", "VBN", "VBG"},
                    new string[] {"left", "ADJP"},
                    new string[] {"rightdis", "JJP", "JJR", "JJS", "DT", "RB", "RBR", "CD", "IN", "VBD"},
                    new string[] {"left", "ADVP", "NP"}
                });
            nonTerminalInfo.Add("JJP",
                new string[][]
                {
                    new string[]
                    {
                        "left", "NNS", "NN", "$", "QP", "JJ", "VBN", "VBG", "ADJP", "JJP", "JJR", "NP", "JJS", "DT", "FW",
                        "RBR", "RBS", "SBAR", "RB"
                    }
                });
                // JJP is introduced for NML-like adjective phrases in Vadas' treebank; Chris wishes he hadn't used JJP which should be a POS-tag.
            // ADVP rule rewritten by Chris in Nov 2010 to be rightdis.  This is right! JJ.* is often head and rightmost.
            nonTerminalInfo.Add("ADVP", new string[][]
            {
                new string[] {"left", "ADVP", "IN"},
                new string[] {"rightdis", "RB", "RBR", "RBS", "JJ", "JJR", "JJS"},
                new string[] {"rightdis", "RP", "DT", "NN", "CD", "NP", "VBN", "NNP", "CC", "FW", "NNS", "ADJP", "NML"}
            });
            nonTerminalInfo.Add("CONJP", new string[][] {new string[] {"right", "CC", "RB", "IN"}});
            nonTerminalInfo.Add("FRAG", new string[][] {new string[] {"right"}}); // crap
            nonTerminalInfo.Add("INTJ", new string[][] {new string[] {"left"}});
            nonTerminalInfo.Add("LST", new string[][] {new string[] {"right", "LS", ":"}});

            // NML is head in: (NAC-LOC (NML San Antonio) (, ,) (NNP Texas))
            // TODO: NNP should be head (rare cases, could be ignored):
            //   (NAC (NML New York) (NNP Court) (PP of Appeals))
            //   (NAC (NML Prudential Insurance) (NNP Co.) (PP Of America))
            // Chris: This could maybe still do with more thought, but NAC is rare.
            nonTerminalInfo.Add("NAC",
                new string[][]
                {
                    new string[]
                    {
                        "left", "NN", "NNS", "NML", "NNP", "NNPS", "NP", "NAC", "EX", "$", "CD", "QP", "PRP", "VBG", "JJ",
                        "JJS", "JJR", "ADJP", "JJP", "FW"
                    }
                });

            // Added JJ to PP head table, since it is a head in several cases, e.g.:
            // (PP (JJ next) (PP to them))
            // When you have both JJ and IN daughters, it is invariably "such as" -- not so clear which should be head, but leave as IN
            // should prefer JJ? (PP (JJ such) (IN as) (NP (NN crocidolite)))  Michel thinks we should make JJ a head of PP
            // added SYM as used in new treebanks for symbols filling role of IN
            // Changed PP search to left -- just what you want for conjunction (and consistent with SemanticHeadFinder)
            nonTerminalInfo.Add("PP",
                new string[][]
                {new string[] {"right", "IN", "TO", "VBG", "VBN", "RP", "FW", "JJ", "SYM"}, new string[] {"left", "PP"}});

            nonTerminalInfo.Add("PRN",
                new string[][]
                {
                    new string[]
                    {
                        "left", "VP", "NP", "PP", "SQ", "S", "SINV", "SBAR", "ADJP", "JJP", "ADVP", "INTJ", "WHNP", "NAC",
                        "VBP", "JJ", "NN", "NNP"
                    }
                });
            nonTerminalInfo.Add("PRT", new string[][] {new string[] {"right", "RP"}});
            // add '#' for pounds!!
            nonTerminalInfo.Add("QP",
                new string[][]
                {
                    new string[] {"left", "$", "IN", "NNS", "NN", "JJ", "CD", "PDT", "DT", "RB", "NCD", "QP", "JJR", "JJS"}
                });
            // reduced relative clause can be any predicate VP, ADJP, NP, PP.
            // For choosing between NP and PP, really need to know which one is temporal and to choose the other.
            // It's not clear ADVP needs to be in the list at all (delete?).
            nonTerminalInfo.Add("RRC",
                new string[][]
                {new string[] {"left", "RRC"}, new string[] {"right", "VP", "ADJP", "JJP", "NP", "PP", "ADVP"}});

            // delete IN -- go for main part of sentence; add FRAG

            nonTerminalInfo.Add("S",
                new string[][] {new string[] {"left", "TO", "VP", "S", "FRAG", "SBAR", "ADJP", "JJP", "UCP", "NP"}});
            nonTerminalInfo.Add("SBAR",
                new string[][]
                {
                    new string[]
                    {"left", "WHNP", "WHPP", "WHADVP", "WHADJP", "IN", "DT", "S", "SQ", "SINV", "SBAR", "FRAG"}
                });
            nonTerminalInfo.Add("SBARQ",
                new string[][] {new string[] {"left", "SQ", "S", "SINV", "SBARQ", "FRAG", "SBAR"}});
            // cdm: if you have 2 VP under an SINV, you should really take the 2nd as syntactic head, because the first is a topicalized VP complement of the second, but for now I didn't change this, since it didn't help parsing.  (If it were changed, it'd need to be also changed to the opposite in SemanticHeadFinder.)
            nonTerminalInfo.Add("SINV",
                new string[][]
                {new string[] {"left", "VBZ", "VBD", "VBP", "VB", "MD", "VBN", "VP", "S", "SINV", "ADJP", "JJP", "NP"}});
            nonTerminalInfo.Add("SQ",
                new string[][] {new string[] {"left", "VBZ", "VBD", "VBP", "VB", "MD", "AUX", "AUXG", "VP", "SQ"}});
                // TODO: Should maybe put S before SQ for tag questions. Check.
            nonTerminalInfo.Add("UCP", new string[][] {new string[] {"right"}});
            // below is weird!! Make 2 lists, one for good and one for bad heads??
            // VP: added AUX and AUXG to work with Charniak tags
            nonTerminalInfo.Add("VP",
                new string[][]
                {
                    new string[]
                    {
                        "left", "TO", "VBD", "VBN", "MD", "VBZ", "VB", "VBG", "VBP", "VP", "AUX", "AUXG", "ADJP", "JJP",
                        "NN", "NNS", "JJ", "NP", "NNP"
                    }
                });
            nonTerminalInfo.Add("WHADJP",
                new string[][] {new string[] {"left", "WRB", "WHADVP", "RB", "JJ", "ADJP", "JJP", "JJR"}});
            nonTerminalInfo.Add("WHADVP", new string[][] {new string[] {"right", "WRB", "WHADVP"}});
            nonTerminalInfo.Add("WHNP",
                new string[][] {new string[] {"left", "WDT", "WP", "WP$", "WHADJP", "WHPP", "WHNP"}});
            nonTerminalInfo.Add("WHPP", new string[][] {new string[] {"right", "IN", "TO", "FW"}});
            nonTerminalInfo.Add("X",
                new string[][] {new string[] {"right", "S", "VP", "ADJP", "JJP", "NP", "SBAR", "PP", "X"}});
            nonTerminalInfo.Add("NP",
                new string[][]
                {
                    new string[] {"rightdis", "NN", "NNP", "NNPS", "NNS", "NML", "NX", "POS", "JJR"},
                    new string[] {"left", "NP", "PRP"}, new string[] {"rightdis", "$", "ADJP", "JJP", "PRN", "FW"},
                    new string[] {"right", "CD"},
                    new string[] {"rightdis", "JJ", "JJS", "RB", "QP", "DT", "WDT", "RBR", "ADVP"}
                });
            nonTerminalInfo.Add("NX", nonTerminalInfo["NP"]);
            // TODO: seems JJ should be head of NML in this case:
            // (NP (NML (JJ former) (NML Red Sox) (JJ great)) (NNP Luis) (NNP Tiant)),
            // (although JJ great is tagged wrong)
            nonTerminalInfo.Add("NML", nonTerminalInfo["NP"]);


            nonTerminalInfo.Add("POSSP", new string[][] {new string[] {"right", "POS"}});

            /* HJT: Adding the following to deal with oddly formed data in (for example) the Brown corpus */
            nonTerminalInfo.Add("ROOT", new string[][] {new string[] {"left", "S", "SQ", "SINV", "SBAR", "FRAG"}});
            // Just to handle trees which have TOP instead of ROOT at the root
            nonTerminalInfo.Add("TOP", nonTerminalInfo["ROOT"]);
            nonTerminalInfo.Add("TYPO", new string[][]
            {
                new string[]
                {
                    "left", "NN", "NP", "NML", "NNP", "NNPS", "TO",
                    "VBD", "VBN", "MD", "VBZ", "VB", "VBG", "VBP", "VP", "ADJP", "JJP", "FRAG"
                }
            }); // for Brown (Roger)
            nonTerminalInfo.Add("ADV", new string[][]
            {
                new string[]
                {
                    "right", "RB", "RBR", "RBS", "FW",
                    "ADVP", "TO", "CD", "JJR", "JJ", "IN", "NP", "NML", "JJS", "NN"
                }
            });

            // SWBD
            nonTerminalInfo.Add("EDITED", new string[][] {new string[] {"left"}});
                // crap rule for Switchboard (if don't delete EDITED nodes)
            // in sw2756, a "VB". (copy "VP" to handle this problem, though should really fix it on reading)
            nonTerminalInfo.Add("VB",
                new string[][]
                {
                    new string[]
                    {
                        "left", "TO", "VBD", "VBN", "MD", "VBZ", "VB", "VBG", "VBP", "VP", "AUX", "AUXG", "ADJP", "JJP",
                        "NN", "NNS", "JJ", "NP", "NNP"
                    }
                });

            nonTerminalInfo.Add("META", new string[][] {new string[] {"left"}});
                // rule for OntoNotes, but maybe should just be deleted in TreeReader??
            nonTerminalInfo.Add("XS", new string[][] {new string[] {"right", "IN"}});
                // rule for new structure in QP, introduced by Stanford in QPTreeTransformer
            // nonTerminalInfo.Add(null, new string[][] {{"left"}});  // rule for OntoNotes from Michel, but it would be better to fix this in TreeReader or to use a default rule?

            // todo: Uncomment this line if we always want to take the leftmost if no head rule is defined for the mother category.
            // defaultRule = defaultLeftRule; // Don't exception, take leftmost if no rule defined for a certain parent category
        }

    }
}