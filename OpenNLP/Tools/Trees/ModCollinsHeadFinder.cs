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

            nonTerminalInfo = new Dictionary<string, string[][]>();

            // This version from Collins' diss (1999: 236-238)
            // NNS, NN is actually sensible (money, etc.)!
            // QP early isn't; should prefer JJR NN RB
            // remove ADVP; it just shouldn't be there.
            // if two JJ, should take right one (e.g. South Korean)
            // nonTerminalInfo.Add("ADJP", new string[][]{{"left", PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.DollarSign, "QP"}, {"right", PartsOfSpeech.Adjective}, {"left", PartsOfSpeech.VerbPastParticiple, PartsOfSpeech.VerbGerundOrPresentParticiple, "ADJP", "JJP", PartsOfSpeech.AdjectiveComparative, "NP", PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Determiner, PartsOfSpeech.ForeignWord, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative, "SBAR", PartsOfSpeech.Adverb}});
            nonTerminalInfo.Add("ADJP",
                new string[][]
                {
                    new string[] {"left", PartsOfSpeech.DollarSign},
                    new string[]
                    {
                        "rightdis", PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.Adjective,
                        "QP", PartsOfSpeech.VerbPastParticiple, PartsOfSpeech.VerbGerundOrPresentParticiple
                    },
                    new string[] {"left", "ADJP"},
                    new string[]
                    {
                        "rightdis", "JJP", PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.AdjectiveSuperlative,
                        PartsOfSpeech.Determiner, PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative,
                        PartsOfSpeech.CardinalNumber, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.VerbPastTense
                    },
                    new string[] {"left", "ADVP", "NP"}
                });
            nonTerminalInfo.Add("JJP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.DollarSign,
                        "QP", PartsOfSpeech.Adjective, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, "ADJP", "JJP", PartsOfSpeech.AdjectiveComparative,
                        "NP", PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Determiner, PartsOfSpeech.ForeignWord,
                        PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative, "SBAR", PartsOfSpeech.Adverb
                    }
                });
            // JJP is introduced for NML-like adjective phrases in Vadas' treebank; Chris wishes he hadn't used JJP which should be a POS-tag.
            // ADVP rule rewritten by Chris in Nov 2010 to be rightdis.  This is right! JJ.* is often head and rightmost.
            nonTerminalInfo.Add("ADVP", new string[][]
            {
                new string[] {"left", "ADVP", PartsOfSpeech.PrepositionOrSubordinateConjunction},
                new string[]
                {
                    "rightdis", PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative,
                    PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.AdjectiveSuperlative
                },
                new string[]
                {
                    "rightdis", PartsOfSpeech.Particle, PartsOfSpeech.Determiner, PartsOfSpeech.NounSingularOrMass,
                    PartsOfSpeech.CardinalNumber, "NP", PartsOfSpeech.VerbPastParticiple,
                    PartsOfSpeech.ProperNounSingular, PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.ForeignWord,
                    PartsOfSpeech.NounPlural, "ADJP", "NML"
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
                        "left", PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural, "NML",
                        PartsOfSpeech.ProperNounSingular, PartsOfSpeech.ProperNounPlural, "NP", "NAC",
                        PartsOfSpeech.ExistentialThere, PartsOfSpeech.DollarSign, PartsOfSpeech.CardinalNumber, "QP",
                        PartsOfSpeech.PersonalPronoun, PartsOfSpeech.VerbGerundOrPresentParticiple,
                        PartsOfSpeech.Adjective,
                        PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.AdjectiveComparative, "ADJP", "JJP",
                        PartsOfSpeech.ForeignWord
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
                {
                    new string[]
                    {
                        "right", PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.To,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Particle, PartsOfSpeech.ForeignWord, PartsOfSpeech.Adjective, PartsOfSpeech.Symbol
                    },
                    new string[] {"left", "PP"}
                });

            nonTerminalInfo.Add("PRN",
                new string[][]
                {
                    new string[]
                    {
                        "left", "VP", "NP", "PP", "SQ", "S", "SINV", "SBAR", "ADJP", "JJP", "ADVP", "INTJ", "WHNP",
                        "NAC",
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.Adjective,
                        PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular
                    }
                });
            nonTerminalInfo.Add("PRT", new string[][] {new string[] {"right", PartsOfSpeech.Particle}});
            // add '#' for pounds!!
            nonTerminalInfo.Add("QP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.DollarSign, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.Adjective,
                        PartsOfSpeech.CardinalNumber, PartsOfSpeech.Predeterminer, PartsOfSpeech.Determiner,
                        PartsOfSpeech.Adverb, "NCD", "QP", PartsOfSpeech.AdjectiveComparative,
                        PartsOfSpeech.AdjectiveSuperlative
                    }
                });
            // reduced relative clause can be any predicate VP, ADJP, NP, PP.
            // For choosing between NP and PP, really need to know which one is temporal and to choose the other.
            // It's not clear ADVP needs to be in the list at all (delete?).
            nonTerminalInfo.Add("RRC",
                new string[][]
                {new string[] {"left", "RRC"}, new string[] {"right", "VP", "ADJP", "JJP", "NP", "PP", "ADVP"}});

            // delete IN -- go for main part of sentence; add FRAG

            nonTerminalInfo.Add("S",
                new string[][]
                {new string[] {"left", PartsOfSpeech.To, "VP", "S", "FRAG", "SBAR", "ADJP", "JJP", "UCP", "NP"}});
            nonTerminalInfo.Add("SBAR",
                new string[][]
                {
                    new string[]
                    {
                        "left", "WHNP", "WHPP", "WHADVP", "WHADJP", PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.Determiner, "S", "SQ", "SINV", "SBAR", "FRAG"
                    }
                });
            nonTerminalInfo.Add("SBARQ",
                new string[][] {new string[] {"left", "SQ", "S", "SINV", "SBARQ", "FRAG", "SBAR"}});
            // cdm: if you have 2 VP under an SINV, you should really take the 2nd as syntactic head, because the first is a topicalized VP complement of the second, but for now I didn't change this, since it didn't help parsing.  (If it were changed, it'd need to be also changed to the opposite in SemanticHeadFinder.)
            nonTerminalInfo.Add("SINV",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal,
                        PartsOfSpeech.VerbPastParticiple, "VP", "S", "SINV", "ADJP", "JJP", "NP"
                    }
                });
            nonTerminalInfo.Add("SQ",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbPastTense,
                        PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Modal, "AUX",
                        "AUXG", "VP", "SQ"
                    }
                });
                // TODO: Should maybe put S before SQ for tag questions. Check.
            nonTerminalInfo.Add("UCP", new string[][] {new string[] {"right"}});
            // below is weird!! Make 2 lists, one for good and one for bad heads??
            // VP: added AUX and AUXG to work with Charniak tags
            nonTerminalInfo.Add("VP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.To, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Modal, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, "VP",
                        "AUX", "AUXG", "ADJP", "JJP",
                        PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural, PartsOfSpeech.Adjective, "NP",
                        PartsOfSpeech.ProperNounSingular
                    }
                });
            nonTerminalInfo.Add("WHADJP",
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.WhAdverb, "WHADVP", PartsOfSpeech.Adverb, PartsOfSpeech.Adjective, "ADJP",
                        "JJP", PartsOfSpeech.AdjectiveComparative
                    }
                });
            nonTerminalInfo.Add("WHADVP", new string[][] {new string[] {"right", PartsOfSpeech.WhAdverb, "WHADVP"}});
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
            nonTerminalInfo.Add("X",
                new string[][] {new string[] {"right", "S", "VP", "ADJP", "JJP", "NP", "SBAR", "PP", "X"}});
            nonTerminalInfo.Add("NP",
                new string[][]
                {
                    new string[]
                    {
                        "rightdis", PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular,
                        PartsOfSpeech.ProperNounPlural, PartsOfSpeech.NounPlural, "NML", "NX",
                        PartsOfSpeech.PossessiveEnding, PartsOfSpeech.AdjectiveComparative
                    },
                    new string[] {"left", "NP", PartsOfSpeech.PersonalPronoun},
                    new string[] {"rightdis", PartsOfSpeech.DollarSign, "ADJP", "JJP", "PRN", PartsOfSpeech.ForeignWord},
                    new string[] {"right", PartsOfSpeech.CardinalNumber},
                    new string[]
                    {
                        "rightdis", PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Adverb, "QP",
                        PartsOfSpeech.Determiner, PartsOfSpeech.WhDeterminer, PartsOfSpeech.AdverbComparative, "ADVP"
                    }
                });
            nonTerminalInfo.Add("NX", nonTerminalInfo["NP"]);
            // TODO: seems JJ should be head of NML in this case:
            // (NP (NML (JJ former) (NML Red Sox) (JJ great)) (NNP Luis) (NNP Tiant)),
            // (although JJ great is tagged wrong)
            nonTerminalInfo.Add("NML", nonTerminalInfo["NP"]);


            nonTerminalInfo.Add("POSSP", new string[][] {new string[] {"right", PartsOfSpeech.PossessiveEnding}});

            /* HJT: Adding the following to deal with oddly formed data in (for example) the Brown corpus */
            nonTerminalInfo.Add("ROOT", new string[][] {new string[] {"left", "S", "SQ", "SINV", "SBAR", "FRAG"}});
            // Just to handle trees which have TOP instead of ROOT at the root
            nonTerminalInfo.Add("TOP", nonTerminalInfo["ROOT"]);
            nonTerminalInfo.Add("TYPO", new string[][]
            {
                new string[]
                {
                    "left", PartsOfSpeech.NounSingularOrMass, "NP", "NML", PartsOfSpeech.ProperNounSingular,
                    PartsOfSpeech.ProperNounPlural, PartsOfSpeech.To,
                    PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple, PartsOfSpeech.Modal,
                    PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                    PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, "VP", "ADJP",
                    "JJP", "FRAG"
                }
            }); // for Brown (Roger)
            nonTerminalInfo.Add("ADV", new string[][]
            {
                new string[]
                {
                    "right", PartsOfSpeech.Adverb, PartsOfSpeech.AdverbComparative, PartsOfSpeech.AdverbSuperlative,
                    PartsOfSpeech.ForeignWord,
                    "ADVP", PartsOfSpeech.To, PartsOfSpeech.CardinalNumber, PartsOfSpeech.AdjectiveComparative,
                    PartsOfSpeech.Adjective, PartsOfSpeech.PrepositionOrSubordinateConjunction, "NP", "NML",
                    PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.NounSingularOrMass
                }
            });

            // SWBD
            nonTerminalInfo.Add("EDITED", new string[][] {new string[] {"left"}});
                // crap rule for Switchboard (if don't delete EDITED nodes)
            // in sw2756, a PartsOfSpeech.VerbBaseForm. (copy "VP" to handle this problem, though should really fix it on reading)
            nonTerminalInfo.Add(PartsOfSpeech.VerbBaseForm,
                new string[][]
                {
                    new string[]
                    {
                        "left", PartsOfSpeech.To, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbPastParticiple,
                        PartsOfSpeech.Modal, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbBaseForm,
                        PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbNon3rdPersSingPresent, "VP",
                        "AUX", "AUXG", "ADJP", "JJP",
                        PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.NounPlural, PartsOfSpeech.Adjective, "NP",
                        PartsOfSpeech.ProperNounSingular
                    }
                });

            nonTerminalInfo.Add("META", new string[][] {new string[] {"left"}});
                // rule for OntoNotes, but maybe should just be deleted in TreeReader??
            nonTerminalInfo.Add("XS", new string[][] {new string[] {"right", PartsOfSpeech.PrepositionOrSubordinateConjunction}});
                // rule for new structure in QP, introduced by Stanford in QPTreeTransformer
            // nonTerminalInfo.Add(null, new string[][] {{"left"}});  // rule for OntoNotes from Michel, but it would be better to fix this in TreeReader or to use a default rule?

            // todo: Uncomment this line if we always want to take the leftmost if no head rule is defined for the mother category.
            // defaultRule = defaultLeftRule; // Don't exception, take leftmost if no rule defined for a certain parent category
        }

    }
}