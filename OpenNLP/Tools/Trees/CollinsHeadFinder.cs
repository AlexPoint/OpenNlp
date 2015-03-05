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

            NonTerminalInfo = new Dictionary<string, string[][]>();
            // This version from Collins' diss (1999: 236-238)
            NonTerminalInfo.Add(CoordinationTransformer.Adjective,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.NounPlural, QP, PartsOfSpeech.NounSingularOrMass,
                        PartsOfSpeech.DollarSign, ADVP, PartsOfSpeech.Adjective, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, CoordinationTransformer.Adjective, PartsOfSpeech.AdjectiveComparative, CoordinationTransformer.Noun,
                        PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Determiner, PartsOfSpeech.ForeignWord,
                        PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative, SBAR, PartsOfSpeech.Adverb
                    }
                });
            NonTerminalInfo.Add(ADVP,
                new string[][]
                {
                    new string[]
                    {
                        Right, PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative,
                        PartsOfSpeech.ForeignWord, ADVP, PartsOfSpeech.To, PartsOfSpeech.CardinalNumber,
                        PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.Adjective,
                        PartsOfSpeech.PrepositionOrSubordinateConjunction, CoordinationTransformer.Noun, PartsOfSpeech.AdjectiveSuperlative,
                        PartsOfSpeech.NounSingularOrMass
                    }
                });
            NonTerminalInfo.Add(CONJP,
                new string[][]
                {
                    new string[]
                    {
                        Right, PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.Adverb,
                        PartsOfSpeech.PrepositionOrSubordinateConjunction
                    }
                });
            NonTerminalInfo.Add(FRAG, new string[][] {new string[] {Right}}); // crap
            NonTerminalInfo.Add(INTJ, new string[][] {new string[] {Left}});
            NonTerminalInfo.Add(LST,
                new string[][] {new string[] {Right, PartsOfSpeech.ListItemMarker, PartsOfSpeech.ColonSemiColon}});
            NonTerminalInfo.Add(NAC,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural,
                        PartsOfSpeech.ProperNounSingular, PartsOfSpeech.ProperNounPlural, CoordinationTransformer.Noun, NAC,
                        PartsOfSpeech.ExistentialThere, PartsOfSpeech.DollarSign, PartsOfSpeech.CardinalNumber, QP,
                        PartsOfSpeech.PersonalPronoun, PartsOfSpeech.VerbGerundOrPresentParticiple,
                        PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative,
                        PartsOfSpeech.AdjectiveComparative, CoordinationTransformer.Adjective, PartsOfSpeech.ForeignWord
                    }
                });
            NonTerminalInfo.Add(NX, new string[][] {new string[] {Left}}); // crap
            NonTerminalInfo.Add(PP,
                new string[][]
                {
                    new string[]
                    {
                        Right, PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.To,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Particle, PartsOfSpeech.ForeignWord
                    }
                });
            // should prefer JJ? (PP (JJ such) (IN as) (NP (NN crocidolite)))
            NonTerminalInfo.Add(PRN, new string[][] {new string[] {Left}});
            NonTerminalInfo.Add(PRT, new string[][] {new string[] {Right, PartsOfSpeech.Particle}});
            NonTerminalInfo.Add(QP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.DollarSign, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.Adjective,
                        PartsOfSpeech.Adverb, PartsOfSpeech.Determiner, PartsOfSpeech.CardinalNumber, NCD, QP,
                        PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.AdjectiveSuperlative
                    }
                });
            NonTerminalInfo.Add(RRC,
                new string[][]
                {
                    new string[]
                    {
                        Right, AbstractCollinsHeadFinder.VerbPhrase, CoordinationTransformer.Noun, ADVP,
                        CoordinationTransformer.Adjective, PP
                    }
                });
            NonTerminalInfo.Add(S,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.To, PartsOfSpeech.PrepositionOrSubordinateConjunction, AbstractCollinsHeadFinder.VerbPhrase, S, SBAR,
                        CoordinationTransformer.Adjective, UCP, CoordinationTransformer.Noun
                    }
                });
            NonTerminalInfo.Add(SBAR,
                new string[][]
                {
                    new string[]
                    {
                        Left, WHNP, WHPP, WHADVP, WHADJP, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.Determiner, S, SQ, SINV, SBAR, FRAG
                    }
                });
            NonTerminalInfo.Add(SBARQ, new string[][] {new string[] {Left, SQ, S, SINV, SBARQ, FRAG}});
            NonTerminalInfo.Add(SINV,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal, AbstractCollinsHeadFinder.VerbPhrase,
                        S, SINV, CoordinationTransformer.Adjective, CoordinationTransformer.Noun
                    }
                });
            NonTerminalInfo.Add(SQ,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal, AbstractCollinsHeadFinder.VerbPhrase,
                        SQ
                    }
                });
            NonTerminalInfo.Add("UCP", new string[][] {new string[] {Right}});
            NonTerminalInfo.Add(AbstractCollinsHeadFinder.VerbPhrase,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.To, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Modal, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, AUX,
                        AUXG, AbstractCollinsHeadFinder.VerbPhrase, CoordinationTransformer.Adjective, PartsOfSpeech.NounSingularOrMass,
                        PartsOfSpeech.NounPlural, CoordinationTransformer.Noun
                    }
                });
            NonTerminalInfo.Add(WHADJP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.WhAdverb, PartsOfSpeech.Adjective,
                        CoordinationTransformer.Adjective
                    }
                });
            NonTerminalInfo.Add(WHADVP, new string[][] {new string[] {Right, PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.WhAdverb}});
            NonTerminalInfo.Add(WHNP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.WhDeterminer, PartsOfSpeech.WhPronoun, PartsOfSpeech.PossessiveWhPronoun,
                        WHADJP, WHPP, WHNP
                    }
                });
            NonTerminalInfo.Add(WHPP,
                new string[][]
                {
                    new string[]
                    {
                        Right, PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.To,
                        PartsOfSpeech.ForeignWord
                    }
                });
            NonTerminalInfo.Add(X, new string[][] {new string[] {Right}}); // crap rule
            NonTerminalInfo.Add(CoordinationTransformer.Noun,
                new string[][]
                {
                    new string[]
                    {
                        RightDis, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular,
                        PartsOfSpeech.ProperNounPlural, PartsOfSpeech.NounPlural, NX, PartsOfSpeech.PossessiveEnding,
                        PartsOfSpeech.AdjectiveComparative
                    },
                    new string[] {Left, CoordinationTransformer.Noun},
                    new string[] {RightDis, PartsOfSpeech.DollarSign, CoordinationTransformer.Adjective, PRN},
                    new string[] {Right, PartsOfSpeech.CardinalNumber},
                    new string[]
                    {
                        RightDis, PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Adverb, QP
                    }
                });
            NonTerminalInfo.Add(TYPO, new string[][] {new string[] {Left}}); // another crap rule, for Brown (Roger)
            NonTerminalInfo.Add(EDITED, new string[][] {new string[] {Left}});
                // crap rule for Switchboard (if don't delete EDITED nodes)
            NonTerminalInfo.Add(XS, new string[][] {new string[] {Right, PartsOfSpeech.PrepositionOrSubordinateConjunction}}); // rule for new structure in QP
        }

        protected override int PostOperationFix(int headIdx, Tree[] daughterTrees)
        {
            if (headIdx >= 2)
            {
                string prevLab = Tlp.BasicCategory(daughterTrees[headIdx - 1].Value());
                if (prevLab.Equals(PartsOfSpeech.CoordinatingConjunction) || prevLab.Equals(AbstractCollinsHeadFinder.CONJP))
                {
                    int newHeadIdx = headIdx - 2;
                    Tree t = daughterTrees[newHeadIdx];
                    while (newHeadIdx >= 0 && t.IsPreTerminal() &&
                           Tlp.IsPunctuationTag(t.Value()))
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