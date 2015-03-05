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
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class ModCollinsHeadFinder : CollinsHeadFinder
    {
        public ModCollinsHeadFinder() : this(new PennTreebankLanguagePack()){}

        public ModCollinsHeadFinder(AbstractTreebankLanguagePack tlp) : base(tlp, tlp.PunctuationTags())
        {
            // avoid punctuation as head in readonly default rule

            NonTerminalInfo = new Dictionary<string, string[][]>();

            // This version from Collins' diss (1999: 236-238)
            // NNS, NN is actually sensible (money, etc.)!
            // QP early isn't; should prefer JJR NN RB
            // remove ADVP; it just shouldn't be there.
            // if two JJ, should take right one (e.g. South Korean)
            // NonTerminalInfo.Add(CoordinationTransformer.Adjective, new string[][]{{Left, PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.DollarSign, "QP"}, {Right, PartsOfSpeech.Adjective}, {Left, PartsOfSpeech.VerbPastParticiple, PartsOfSpeech.VerbGerundOrPresentParticiple, CoordinationTransformer.Adjective, "JJP", PartsOfSpeech.AdjectiveComparative, CoordinationTransformer.Noun, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Determiner, PartsOfSpeech.ForeignWord, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative, "SBAR", PartsOfSpeech.Adverb}});
            NonTerminalInfo.Add(CoordinationTransformer.Adjective,
                new string[][]
                {
                    new string[] {Left, PartsOfSpeech.DollarSign},
                    new string[]
                    {
                        RightDis, PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.Adjective,
                        QP, PartsOfSpeech.VerbPastParticiple, PartsOfSpeech.VerbGerundOrPresentParticiple
                    },
                    new string[] {Left, CoordinationTransformer.Adjective},
                    new string[]
                    {
                        RightDis, JJP, PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.AdjectiveSuperlative,
                        PartsOfSpeech.Determiner, PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative,
                        PartsOfSpeech.CardinalNumber, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.VerbPastTense
                    },
                    new string[] {Left, ADVP, CoordinationTransformer.Noun}
                });
            NonTerminalInfo.Add(JJP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.DollarSign,
                        QP, PartsOfSpeech.Adjective, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, CoordinationTransformer.Adjective, JJP, PartsOfSpeech.AdjectiveComparative,
                        CoordinationTransformer.Noun, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Determiner, PartsOfSpeech.ForeignWord,
                        PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative, SBAR, PartsOfSpeech.Adverb
                    }
                });
            // JJP is introduced for NML-like adjective phrases in Vadas' treebank; Chris wishes he hadn't used JJP which should be a POS-tag.
            // ADVP rule rewritten by Chris in Nov 2010 to be rightdis.  This is right! JJ.* is often head and rightmost.
            NonTerminalInfo.Add(ADVP, new string[][]
            {
                new string[] {Left, ADVP, PartsOfSpeech.PrepositionOrSubordinateConjunction},
                new string[]
                {
                    RightDis, PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative,
                    PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.AdjectiveSuperlative
                },
                new string[]
                {
                    RightDis, PartsOfSpeech.Particle, PartsOfSpeech.Determiner, PartsOfSpeech.NounSingularOrMass,
                    PartsOfSpeech.CardinalNumber, CoordinationTransformer.Noun, PartsOfSpeech.VerbPastParticiple,
                    PartsOfSpeech.ProperNounSingular, PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.ForeignWord,
                    PartsOfSpeech.NounPlural, CoordinationTransformer.Adjective, NML
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

            // NML is head in: (NAC-LOC (NML San Antonio) (, ,) (NNP Texas))
            // TODO: NNP should be head (rare cases, could be ignored):
            //   (NAC (NML New York) (NNP Court) (PP of Appeals))
            //   (NAC (NML Prudential Insurance) (NNP Co.) (PP Of America))
            // Chris: This could maybe still do with more thought, but NAC is rare.
            NonTerminalInfo.Add(NAC,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural, NML,
                        PartsOfSpeech.ProperNounSingular, PartsOfSpeech.ProperNounPlural, CoordinationTransformer.Noun, NAC,
                        PartsOfSpeech.ExistentialThere, PartsOfSpeech.DollarSign, PartsOfSpeech.CardinalNumber, QP,
                        PartsOfSpeech.PersonalPronoun, PartsOfSpeech.VerbGerundOrPresentParticiple,
                        PartsOfSpeech.Adjective,
                        PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.AdjectiveComparative, CoordinationTransformer.Adjective, JJP,
                        PartsOfSpeech.ForeignWord
                    }
                });

            // Added JJ to PP head table, since it is a head in several cases, e.g.:
            // (PP (JJ next) (PP to them))
            // When you have both JJ and IN daughters, it is invariably "such as" -- not so clear which should be head, but leave as IN
            // should prefer JJ? (PP (JJ such) (IN as) (NP (NN crocidolite)))  Michel thinks we should make JJ a head of PP
            // added SYM as used in new treebanks for symbols filling role of IN
            // Changed PP search to left -- just what you want for conjunction (and consistent with SemanticHeadFinder)
            NonTerminalInfo.Add(PP,
                new string[][]
                {
                    new string[]
                    {
                        Right, PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.To,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Particle, PartsOfSpeech.ForeignWord, PartsOfSpeech.Adjective, PartsOfSpeech.Symbol
                    },
                    new string[] {Left, PP}
                });

            NonTerminalInfo.Add(PRN,
                new string[][]
                {
                    new string[]
                    {
                        Left, VP, CoordinationTransformer.Noun, PP, SQ, S, SINV, SBAR, CoordinationTransformer.Adjective, JJP, ADVP, INTJ, WHNP,
                        NAC,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.Adjective,
                        PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular
                    }
                });
            NonTerminalInfo.Add(PRT, new string[][] {new string[] {Right, PartsOfSpeech.Particle}});
            // add '#' for pounds!!
            NonTerminalInfo.Add(QP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.DollarSign, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.Adjective,
                        PartsOfSpeech.CardinalNumber, PartsOfSpeech.Predeterminer, PartsOfSpeech.Determiner,
                        PartsOfSpeech.Adverb, NCD, QP, PartsOfSpeech.AdjectiveComparative,
                        PartsOfSpeech.AdjectiveSuperlative
                    }
                });
            // reduced relative clause can be any predicate VP, ADJP, NP, PP.
            // For choosing between NP and PP, really need to know which one is temporal and to choose the other.
            // It's not clear ADVP needs to be in the list at all (delete?).
            NonTerminalInfo.Add(RRC,
                new string[][] { new string[] { Left, RRC }, new string[] { Right, VP, CoordinationTransformer.Adjective, JJP, CoordinationTransformer.Noun, PP, ADVP } });

            // delete IN -- go for main part of sentence; add FRAG

            NonTerminalInfo.Add(S,
                new string[][] { new string[] { Left, PartsOfSpeech.To, VP, S, FRAG, SBAR, CoordinationTransformer.Adjective, JJP, UCP, CoordinationTransformer.Noun } });
            NonTerminalInfo.Add(SBAR,
                new string[][]
                {
                    new string[]
                    {
                        Left, WHNP, WHPP, WHADVP, WHADJP, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.Determiner, S, SQ, SINV, SBAR, FRAG
                    }
                });
            NonTerminalInfo.Add(SBARQ,
                new string[][] {new string[] {Left, SQ, S, SINV, SBARQ, FRAG, SBAR}});
            // cdm: if you have 2 VP under an SINV, you should really take the 2nd as syntactic head, because the first is a topicalized VP complement of the second, but for now I didn't change this, since it didn't help parsing.  (If it were changed, it'd need to be also changed to the opposite in SemanticHeadFinder.)
            NonTerminalInfo.Add(SINV,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal,
                        PartsOfSpeech.VerbPastParticiple, VP, S, SINV, CoordinationTransformer.Adjective, JJP, CoordinationTransformer.Noun
                    }
                });
            NonTerminalInfo.Add(SQ,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal, AUX,
                        AUXG, VP, SQ
                    }
                });
                // TODO: Should maybe put S before SQ for tag questions. Check.
            NonTerminalInfo.Add(UCP, new string[][] {new string[] {Right}});
            // below is weird!! Make 2 lists, one for good and one for bad heads??
            // VP: added AUX and AUXG to work with Charniak tags
            NonTerminalInfo.Add(VP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.To, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Modal, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, VP,
                        AUX, AUXG, CoordinationTransformer.Adjective, JJP,
                        PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural, PartsOfSpeech.Adjective, CoordinationTransformer.Noun,
                        PartsOfSpeech.ProperNounSingular
                    }
                });
            NonTerminalInfo.Add(WHADJP,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.WhAdverb, WHADVP, PartsOfSpeech.Adverb, PartsOfSpeech.Adjective, CoordinationTransformer.Adjective,
                        JJP, PartsOfSpeech.AdjectiveComparative
                    }
                });
            NonTerminalInfo.Add(WHADVP, new string[][] {new string[] {Right, PartsOfSpeech.WhAdverb, WHADVP}});
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
            NonTerminalInfo.Add(X,
                new string[][] { new string[] { Right, S, VP, CoordinationTransformer.Adjective, JJP, CoordinationTransformer.Noun, SBAR, PP, X } });
            NonTerminalInfo.Add(CoordinationTransformer.Noun,
                new string[][]
                {
                    new string[]
                    {
                        RightDis, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular,
                        PartsOfSpeech.ProperNounPlural, PartsOfSpeech.NounPlural, NML, NX,
                        PartsOfSpeech.PossessiveEnding, PartsOfSpeech.AdjectiveComparative
                    },
                    new string[] {Left, CoordinationTransformer.Noun, PartsOfSpeech.PersonalPronoun},
                    new string[] {RightDis, PartsOfSpeech.DollarSign, CoordinationTransformer.Adjective, JJP, PRN, PartsOfSpeech.ForeignWord},
                    new string[] {Right, PartsOfSpeech.CardinalNumber},
                    new string[]
                    {
                        RightDis, PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Adverb, QP,
                        PartsOfSpeech.Determiner, PartsOfSpeech.WhDeterminer, PartsOfSpeech.AdverbComparative, ADVP
                    }
                });
            NonTerminalInfo.Add(NX, NonTerminalInfo[CoordinationTransformer.Noun]);
            // TODO: seems JJ should be head of NML in this case:
            // (NP (NML (JJ former) (NML Red Sox) (JJ great)) (NNP Luis) (NNP Tiant)),
            // (although JJ great is tagged wrong)
            NonTerminalInfo.Add(NML, NonTerminalInfo[CoordinationTransformer.Noun]);


            NonTerminalInfo.Add(POSSP, new string[][] {new string[] {Right, PartsOfSpeech.PossessiveEnding}});

            /* HJT: Adding the following to deal with oddly formed data in (for example) the Brown corpus */
            NonTerminalInfo.Add(ROOT, new string[][] {new string[] {Left, S, SQ, SINV, SBAR, FRAG}});
            // Just to handle trees which have TOP instead of ROOT at the root
            NonTerminalInfo.Add(TOP, NonTerminalInfo[ROOT]);
            NonTerminalInfo.Add(TYPO, new string[][]
            {
                new string[]
                {
                    Left, PartsOfSpeech.NounSingularOrMass, CoordinationTransformer.Noun, NML, PartsOfSpeech.ProperNounSingular,
                    PartsOfSpeech.ProperNounPlural, PartsOfSpeech.To,
                    PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple, PartsOfSpeech.Modal,
                    PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                    PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, VP, CoordinationTransformer.Adjective,
                    JJP, FRAG
                }
            }); // for Brown (Roger)
            NonTerminalInfo.Add(ADV, new string[][]
            {
                new string[]
                {
                    Right, PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative,
                    PartsOfSpeech.ForeignWord,
                    ADVP, PartsOfSpeech.To, PartsOfSpeech.CardinalNumber, PartsOfSpeech.AdjectiveComparative,
                    PartsOfSpeech.Adjective, PartsOfSpeech.PrepositionOrSubordinateConjunction, CoordinationTransformer.Noun, "NML",
                    PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.NounSingularOrMass
                }
            });

            // SWBD
            NonTerminalInfo.Add(EDITED, new string[][] {new string[] {Left}});
                // crap rule for Switchboard (if don't delete EDITED nodes)
            // in sw2756, a PartsOfSpeech.VerbBaseForm. (copy "VP" to handle this problem, though should really fix it on reading)
            NonTerminalInfo.Add(PartsOfSpeech.VerbBaseForm,
                new string[][]
                {
                    new string[]
                    {
                        Left, PartsOfSpeech.To, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Modal, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, VP,
                        AUX, AUXG, CoordinationTransformer.Adjective, JJP,
                        PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural, PartsOfSpeech.Adjective, CoordinationTransformer.Noun,
                        PartsOfSpeech.ProperNounSingular
                    }
                });

            NonTerminalInfo.Add(META, new string[][] {new string[] {Left}});
                // rule for OntoNotes, but maybe should just be deleted in TreeReader??
            NonTerminalInfo.Add(XS, new string[][] {new string[] {Right, PartsOfSpeech.PrepositionOrSubordinateConjunction}});
                // rule for new structure in QP, introduced by Stanford in QPTreeTransformer
            // NonTerminalInfo.Add(null, new string[][] {{Left}});  // rule for OntoNotes from Michel, but it would be better to fix this in TreeReader or to use a default rule?

            // todo: Uncomment this line if we always want to take the leftmost if no head rule is defined for the mother category.
            // defaultRule = defaultLeftRule; // Don't exception, take leftmost if no rule defined for a certain parent category
        }

    }
}