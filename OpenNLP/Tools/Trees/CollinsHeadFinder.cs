using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Implements the HeadFinder found in Michael Collins' 1999 thesis.
    /// Except: we've added a head rule for NX, which returns the leftmost item.
    /// No rule for the head of NX is found in any of the versions of
    /// Collins' head table that we have (did he perhaps use the NP rules
    /// for NX? -- no Bikel, CL, 2005 says it defaults to leftmost).
    /// These rules are suitable for the Penn Treebank.
    /// 
    /// May 2004: Added support for AUX and AUXG to the VP rules; these cause
    /// no interference in Penn Treebank parsing, but means that these rules
    /// also work for the BLLIP corpus (or Charniak parser output in general).
    /// Feb 2005: Fixes to coordination reheading so that punctuation cannot
    /// become head.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class CollinsHeadFinder : AbstractCollinsHeadFinder
    {
        private static readonly string[] EmptyStringArray = {};

        public CollinsHeadFinder() : this(new PennTreebankLanguagePack())
        {
        }

        /// <summary>
        /// This constructor provides the traditional behavior, where there is
        /// no special avoidance of punctuation categories.
        /// </summary>
        /// <param name="tlp">TreebankLanguagePack used for basic category function</param>
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
                        "left", PartsOfSpeech.NounPlural, "QP", PartsOfSpeech.NounSingularOrMass,
                        PartsOfSpeech.DollarSign, "ADVP", PartsOfSpeech.Adjective, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, "ADJP", PartsOfSpeech.AdjectiveComparative, "NP",
                        PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Determiner, PartsOfSpeech.ForeignWord,
                        PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative, "SBAR", PartsOfSpeech.Adverb
                    }
                });
            nonTerminalInfo.Add("ADVP",
                new string[][]
                {
                    new string[]
                    {
                        "right", PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative,
                        PartsOfSpeech.ForeignWord, "ADVP", PartsOfSpeech.To, PartsOfSpeech.CardinalNumber,
                        PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.Adjective,
                        PartsOfSpeech.PrepositionOrSubordinateConjunction, "NP", PartsOfSpeech.AdjectiveSuperlative,
                        PartsOfSpeech.NounSingularOrMass
                    }
                });
            nonTerminalInfo.Add("CONJP",
                new string[][]
                {
                    new string[]
                    {
                        "right", PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.Adverb,
                        PartsOfSpeech.PrepositionOrSubordinateConjunction
                    }
                });
            nonTerminalInfo.Add("FRAG", new string[][] {new string[] {"right"}}); // crap
            nonTerminalInfo.Add("INTJ", new string[][] {new string[] {"left"}});
            nonTerminalInfo.Add("LST",
                new string[][] {new string[] {"right", PartsOfSpeech.ListItemMarker, PartsOfSpeech.ColonSemiColon}});
            nonTerminalInfo.Add("NAC",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural,
                        PartsOfSpeech.ProperNounSingular, PartsOfSpeech.ProperNounPlural, "NP", "NAC",
                        PartsOfSpeech.ExistentialThere, PartsOfSpeech.DollarSign, PartsOfSpeech.CardinalNumber, "QP",
                        PartsOfSpeech.PersonalPronoun, PartsOfSpeech.VerbGerundOrPresentParticiple,
                        PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative,
                        PartsOfSpeech.AdjectiveComparative, "ADJP", PartsOfSpeech.ForeignWord
                    }
                });
            nonTerminalInfo.Add("NX", new string[][] {new string[] {"left"}}); // crap
            nonTerminalInfo.Add("PP",
                new string[][]
                {
                    new string[]
                    {
                        "right", PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.To,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Particle, PartsOfSpeech.ForeignWord
                    }
                });
            // should prefer JJ? (PP (JJ such) (IN as) (NP (NN crocidolite)))
            nonTerminalInfo.Add("PRN", new string[][] {new string[] {"left"}});
            nonTerminalInfo.Add("PRT", new string[][] {new string[] {"right", PartsOfSpeech.Particle}});
            nonTerminalInfo.Add("QP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.DollarSign, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.Adjective,
                        PartsOfSpeech.Adverb, PartsOfSpeech.Determiner, PartsOfSpeech.CardinalNumber, "NCD", "QP",
                        PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.AdjectiveSuperlative
                    }
                });
            nonTerminalInfo.Add("RRC", new string[][] {new string[] {"right", "VP", "NP", "ADVP", "ADJP", "PP"}});
            nonTerminalInfo.Add("S",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.To, PartsOfSpeech.PrepositionOrSubordinateConjunction, "VP", "S", "SBAR",
                        "ADJP", "UCP", "NP"
                    }
                });
            nonTerminalInfo.Add("SBAR",
                new string[][]
                {
                    new string[]
                    {
                        "left", "WHNP", "WHPP", "WHADVP", "WHADJP", PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.Determiner, "S", "SQ", "SINV", "SBAR", "FRAG"
                    }
                });
            nonTerminalInfo.Add("SBARQ", new string[][] {new string[] {"left", "SQ", "S", "SINV", "SBARQ", "FRAG"}});
            nonTerminalInfo.Add("SINV",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal, "VP",
                        "S", "SINV", "ADJP", "NP"
                    }
                });
            nonTerminalInfo.Add("SQ",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal, "VP",
                        "SQ"
                    }
                });
            nonTerminalInfo.Add("UCP", new string[][] {new string[] {"right"}});
            nonTerminalInfo.Add("VP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.To, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Modal, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, "AUX",
                        "AUXG", "VP", "ADJP", PartsOfSpeech.NounSingularOrMass,
                        PartsOfSpeech.NounPlural, "NP"
                    }
                });
            nonTerminalInfo.Add("WHADJP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.WhAdverb, PartsOfSpeech.Adjective,
                        "ADJP"
                    }
                });
            nonTerminalInfo.Add("WHADVP", new string[][] {new string[] {"right", PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.WhAdverb}});
            nonTerminalInfo.Add("WHNP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.WhDeterminer, PartsOfSpeech.WhPronoun, PartsOfSpeech.PossessiveWhPronoun,
                        "WHADJP", "WHPP", "WHNP"
                    }
                });
            nonTerminalInfo.Add("WHPP",
                new string[][]
                {
                    new string[]
                    {
                        "right", PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.To,
                        PartsOfSpeech.ForeignWord
                    }
                });
            nonTerminalInfo.Add("X", new string[][] {new string[] {"right"}}); // crap rule
            nonTerminalInfo.Add("NP",
                new string[][]
                {
                    new string[]
                    {
                        "rightdis", PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular,
                        PartsOfSpeech.ProperNounPlural, PartsOfSpeech.NounPlural, "NX", PartsOfSpeech.PossessiveEnding,
                        PartsOfSpeech.AdjectiveComparative
                    },
                    new string[] {"left", "NP"},
                    new string[] {"rightdis", PartsOfSpeech.DollarSign, "ADJP", "PRN"},
                    new string[] {"right", PartsOfSpeech.CardinalNumber},
                    new string[]
                    {
                        "rightdis", PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Adverb, "QP"
                    }
                });
            nonTerminalInfo.Add("TYPO", new string[][] {new string[] {"left"}}); // another crap rule, for Brown (Roger)
            nonTerminalInfo.Add("EDITED", new string[][] {new string[] {"left"}});
                // crap rule for Switchboard (if don't delete EDITED nodes)
            nonTerminalInfo.Add("XS", new string[][] {new string[] {"right", PartsOfSpeech.PrepositionOrSubordinateConjunction}}); // rule for new structure in QP
        }

        protected override int PostOperationFix(int headIdx, Tree[] daughterTrees)
        {
            if (headIdx >= 2)
            {
                string prevLab = tlp.BasicCategory(daughterTrees[headIdx - 1].Value());
                if (prevLab.Equals(PartsOfSpeech.CoordinatingConjunction) || prevLab.Equals("CONJP"))
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