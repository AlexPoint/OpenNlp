using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Trees.TRegex;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Implements a 'semantic head' variant of the the HeadFinder found
    /// in Michael Collins' 1999 thesis.
    /// This version chooses the semantic head verb rather than the verb form
    /// for cases with verbs.  And it makes similar themed changes to other
    /// categories: e.g., in question phrases, like "Which Brazilian game", the
    /// head is made "game" not "Which" as in common PTB head rules.<p/>
    /// 
    /// By default the SemanticHeadFinder uses a treatment of copula where the
    /// complement of the copula is taken as the head.  That is, a sentence like
    /// "Bill is big" will be analyzed as
    /// 
    /// <code>nsubj</code>(big, Bill)
    /// <code>cop</code>(big, is)
    /// 
    /// This analysis is used for questions and declaratives for adjective
    /// complements and declarative nominal complements.  However Wh-sentences
    /// with nominal complements do not receive this treatment.
    /// "Who is the president?" is analyzed with "the president" as nsubj and "who"
    /// as "attr" of the copula:
    /// <code>nsubj</code>(is, president)
    /// <code>attr</code>(is, Who)
    /// 
    /// (Such nominal copula sentences are complex: arguably, depending on the
    /// circumstances, several analyses are possible, with either the overt NP able
    /// to be any of the subject, the predicate, or one of two referential entities
    /// connected by an equational copula.  These uses aren't differentiated.)
    /// 
    /// Existential sentences are treated as follows:
    /// "There is a man"
    /// <code>expl</code>(is, There) <br/>
    /// <code>det</code>(man-4, a-3) <br/>
    /// <code>nsubj</code>(is-2, man-4)<br/>
    /// 
    /// @author John Rappaport
    /// @author Marie-Catherine de Marneffe
    /// @author Anna Rafferty
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class SemanticHeadFinder : ModCollinsHeadFinder
    {
        /* A few times the apostrophe is missing on "'s", so we have "s" */
        /* Tricky auxiliaries: "na" is from "gonna", "ve" from "Weve", etc.  "of" as non-standard for "have" */

        private static readonly string[] Auxiliaries =
        {
            "will", "wo", "shall", "sha", "may", "might", "should", "would",
            "can", "could", "ca", "must", "has", "have", "had", "having", "get", "gets", "getting", "got", "gotten",
            "do", "does", "did", "to", "'ve", "ve", "v", "'d", "d", "'ll", "ll", "na", "of", "hav", "hvae", "as"
        };

        private static readonly string[] BeGetVerbs =
        {
            "be", "being", "been", "am", "are", "r", "is", "ai", "was",
            "were", "'m", "m", "'re", "'s", "s", "art", "ar", "get", "getting", "gets", "got"
        };

        public static readonly string[] CopulaVerbs =
        {
            "be", "being", "been", "am", "are", "r", "is", "ai", "was",
            "were", "'m", "m", "ar", "art", "'re", "'s", "s", "wase"
        };

        // include Charniak tags so can do BLLIP right
        private static readonly string[] VerbTags = { PartsOfSpeech.To, PartsOfSpeech.Modal, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.Verb3rdPersSingPresent, PartsOfSpeech.VerbGerundOrPresentParticiple, PartsOfSpeech.VerbPastParticiple, "AUX", "AUXG" };
        // These ones are always auxiliaries, even if the word is "too", "my", or whatever else appears in web text.
        private static readonly string[] UnambiguousAuxTags = {PartsOfSpeech.To, PartsOfSpeech.Modal, "AUX", "AUXG"};


        private readonly Set<string> verbalAuxiliaries;
        private readonly Set<string> copulars;
        private readonly Set<string> passiveAuxiliaries;
        private readonly Set<string> verbalTags;
        private readonly Set<string> unambiguousAuxiliaryTags;

        private readonly bool makeCopulaHead;


        public SemanticHeadFinder() : this(new PennTreebankLanguagePack(), true)
        {
        }

        public SemanticHeadFinder(bool noCopulaHead) : this(new PennTreebankLanguagePack(), noCopulaHead)
        {
        }

        /// <summary>
        /// Create a SemanticHeadFinder
        /// </summary>
        /// <param name="tlp">
        /// The TreebankLanguagePack, used by the superclass to get basic category of constituents
        /// </param>
        /// <param name="noCopulaHead">
        /// If true, a copular verb (be, seem, appear, stay, remain, resemble, become)
        /// is not treated as head when it has an AdjP or NP complement.  If false,
        /// a copula verb is still always treated as a head.  But it will still
        /// be treated as an auxiliary in periphrastic tenses with a VP complement.
        /// </param>
        public SemanticHeadFinder(AbstractTreebankLanguagePack tlp, bool noCopulaHead) : base(tlp)
        {
            RuleChanges();

            // make a distinction between auxiliaries and copula verbs to
            // get the NP has semantic head in sentences like "Bill is an honest man".  (Added "sha" for "shan't" May 2009
            verbalAuxiliaries = new Util.HashSet<string>(Auxiliaries);

            passiveAuxiliaries = new Util.HashSet<string>(BeGetVerbs);

            //copula verbs having an NP complement
            copulars = new Util.HashSet<string>();
            if (noCopulaHead)
            {
                copulars.AddAll(CopulaVerbs);
            }

            // TODO: reverse the polarity of noCopulaHead
            this.makeCopulaHead = !noCopulaHead;

            verbalTags = new Util.HashSet<string>(VerbTags);
            unambiguousAuxiliaryTags = new Util.HashSet<string>(UnambiguousAuxTags);
        }

        public override bool MakesCopulaHead()
        {
            return makeCopulaHead;
        }

        /// <summary>
        /// Makes modifications of Collins' rules to better fit with semantic notions of heads
        /// </summary>
        private void RuleChanges()
        {
            //  NP: don't want a POS to be the head
            // verbs are here so that POS isn't favored in the case of bad parses
            NonTerminalInfo[CoordinationTransformer.Noun] =
                new string[][]
                {
                    new string[]
                    {
                        RightDis, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular,
                        PartsOfSpeech.ProperNounPlural, PartsOfSpeech.NounPlural, NX, NML,
                        PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.WhPronoun
                    },
                    new string[] {Left, CoordinationTransformer.Noun, PartsOfSpeech.PersonalPronoun},
                    new string[] {RightDis, PartsOfSpeech.DollarSign, CoordinationTransformer.Adjective, PartsOfSpeech.ForeignWord},
                    new string[] {Right, PartsOfSpeech.CardinalNumber},
                    new string[]
                    {
                        RightDis, PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative, QP,
                        PartsOfSpeech.Determiner, PartsOfSpeech.WhDeterminer, NML, PRN, PartsOfSpeech.Adverb,
                        PartsOfSpeech.AdverbComparative, ADVP
                    },
                    new string[]
                    {
                        RightDis, AbstractCollinsHeadFinder.VerbPhrase, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Verb3rdPersSingPresent,
                        PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbNon3rdPersSingPresent
                    },
                    new string[] {Left, PartsOfSpeech.PossessiveEnding}
                };
            NonTerminalInfo[NX] = NonTerminalInfo[CoordinationTransformer.Noun];
            NonTerminalInfo[NML] = NonTerminalInfo[CoordinationTransformer.Noun];
            // WHNP clauses should have the same sort of head as an NP
            // but it a WHNP has a NP and a WHNP under it, the WHNP should be the head.  E.g.,  (WHNP (WHNP (WP$ whose) (JJ chief) (JJ executive) (NN officer))(, ,) (NP (NNP James) (NNP Gatward))(, ,))
            NonTerminalInfo[WHNP] = new string[][]
            {
                new string[]
                {
                    RightDis, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular,
                    PartsOfSpeech.ProperNounPlural, PartsOfSpeech.NounPlural, NX, NML,
                    PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.WhPronoun
                },
                new string[] {Left, WHNP, CoordinationTransformer.Noun},
                new string[] {RightDis, PartsOfSpeech.DollarSign, CoordinationTransformer.Adjective, PRN, PartsOfSpeech.ForeignWord},
                new string[] {Right, PartsOfSpeech.CardinalNumber},
                new string[]
                {RightDis, PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveSuperlative, PartsOfSpeech.Adverb, QP},
                new string[] {Left, WHPP, WHADJP, PartsOfSpeech.PossessiveWhPronoun, PartsOfSpeech.WhDeterminer}
            };
            //WHADJP
            NonTerminalInfo[WHADJP] = new string[][]
            {
                new string[]
                {Left, CoordinationTransformer.Adjective, PartsOfSpeech.Adjective, PartsOfSpeech.AdjectiveComparative, PartsOfSpeech.WhPronoun},
                new string[] {Right, PartsOfSpeech.Adverb}, new string[] {Right}
            };
            //WHADJP
            NonTerminalInfo[WHADVP] = new string[][] {new string[] {RightDis, PartsOfSpeech.WhAdverb, WHADVP, PartsOfSpeech.Adverb, PartsOfSpeech.Adjective}};
            // if not WRB or WHADVP, probably has flat NP structure, allow JJ for "how long" constructions
            // QP: we don't want the first CD to be the semantic head (e.g., "three billion": head should be "billion"), so we go from right to left
            NonTerminalInfo[QP] =
                new string[][]
                {
                    new string[]
                    {
                        Right, PartsOfSpeech.DollarSign, PartsOfSpeech.NounPlural, PartsOfSpeech.NounSingularOrMass,
                        PartsOfSpeech.CardinalNumber, PartsOfSpeech.Adjective, PartsOfSpeech.Predeterminer,
                        PartsOfSpeech.Determiner, PartsOfSpeech.PrepositionOrSubordinateConjunction,
                        PartsOfSpeech.Adverb, NCD, QP, PartsOfSpeech.AdjectiveComparative,
                        PartsOfSpeech.AdjectiveSuperlative
                    }
                };

            // S, SBAR and SQ clauses should prefer the main verb as the head
            // S: "He considered him a friend" -> we want a friend to be the head
            NonTerminalInfo[S] = new string[][]
            {
                new string[] {Left, AbstractCollinsHeadFinder.VerbPhrase, S, FRAG, SBAR, CoordinationTransformer.Adjective, UCP, PartsOfSpeech.To},
                new string[] {Right, CoordinationTransformer.Noun}
            };

            NonTerminalInfo[SBAR] = new string[][]
            {
                new string[]
                {
                    Left, S, SQ, SINV, SBAR, FRAG, AbstractCollinsHeadFinder.VerbPhrase, WHNP, WHPP, WHADVP, WHADJP,
                    PartsOfSpeech.PrepositionOrSubordinateConjunction, PartsOfSpeech.Determiner
                }
            };
            // VP shouldn't be needed in SBAR, but occurs in one buggy tree in PTB3 wsj_1457 and otherwise does no harm

            NonTerminalInfo[SQ] = new string[][]
            {
                new string[]
                {
                    Left, AbstractCollinsHeadFinder.VerbPhrase, SQ, CoordinationTransformer.Adjective, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Verb3rdPersSingPresent,
                    PartsOfSpeech.VerbPastTense, PartsOfSpeech.VerbNon3rdPersSingPresent, PartsOfSpeech.Modal, AUX,
                    AUXG
                }
            };


            // UCP take the first element as head
            NonTerminalInfo[UCP] = new string[][] {new string[] {Left}};

            // CONJP: we want different heads for "but also" and "but not" and we don't want "not" to be the head in "not to mention"; now make "mention" head of "not to mention"
            NonTerminalInfo[CONJP] = new string[][]
            {
                new string[]
                {
                    Right, PartsOfSpeech.CoordinatingConjunction, PartsOfSpeech.VerbBaseForm, PartsOfSpeech.Adjective,
                    PartsOfSpeech.Adverb, PartsOfSpeech.PrepositionOrSubordinateConjunction
                }
            };

            // FRAG: crap rule needs to be change if you want to parse
            // glosses; but it is correct to have ADJP and ADVP before S
            // because of weird parses of reduced sentences.
            NonTerminalInfo[FRAG] = new string[][]
            {
                new string[] {Left, PartsOfSpeech.PrepositionOrSubordinateConjunction},
                new string[] {Right, PartsOfSpeech.Adverb}, new string[] {Left, CoordinationTransformer.Noun},
                new string[] {Left, CoordinationTransformer.Adjective, ADVP, FRAG, S, SBAR, AbstractCollinsHeadFinder.VerbPhrase}
            };

            // PRN: sentence first
            NonTerminalInfo[PRN] = new string[][]
            {
                new string[]
                {
                    Left, AbstractCollinsHeadFinder.VerbPhrase, SQ, S, SINV, SBAR, CoordinationTransformer.Noun, CoordinationTransformer.Adjective, PP, ADVP, INTJ, WHNP, NAC,
                    PartsOfSpeech.VerbNon3rdPersSingPresent,
                    PartsOfSpeech.Adjective, PartsOfSpeech.NounSingularOrMass, PartsOfSpeech.ProperNounSingular
                }
            };

            // add the constituent XS (special node to add a layer in a QP tree introduced in our QPTreeTransformer)
            NonTerminalInfo[XS] = new string[][] {new string[] {Right, PartsOfSpeech.PrepositionOrSubordinateConjunction}};

            // add a rule to deal with the CoNLL data
            NonTerminalInfo[EMBED] = new string[][] {new string[] {Right, INTJ}};

        }
        
        private bool ShouldSkip(Tree t, bool origWasInterjection)
        {
            return t.IsPreTerminal() &&
                   (Tlp.IsPunctuationTag(t.Value()) || ! origWasInterjection && PartsOfSpeech.Interjection.Equals(t.Value())) ||
                   INTJ.Equals(t.Value()) && ! origWasInterjection;
        }

        private int FindPreviousHead(int headIdx, Tree[] daughterTrees, bool origWasInterjection)
        {
            bool seenSeparator = false;
            int newHeadIdx = headIdx;
            while (newHeadIdx >= 0)
            {
                newHeadIdx = newHeadIdx - 1;
                if (newHeadIdx < 0)
                {
                    return newHeadIdx;
                }
                string label = Tlp.BasicCategory(daughterTrees[newHeadIdx].Value());
                if (PartsOfSpeech.Comma.Equals(label) || PartsOfSpeech.ColonSemiColon.Equals(label))
                {
                    seenSeparator = true;
                }
                else if (daughterTrees[newHeadIdx].IsPreTerminal() &&
                         (Tlp.IsPunctuationTag(label) || ! origWasInterjection && PartsOfSpeech.Interjection.Equals(label)) ||
                         INTJ.Equals(label) && ! origWasInterjection)
                {
                    // keep looping
                }
                else
                {
                    if (! seenSeparator)
                    {
                        newHeadIdx = -1;
                    }
                    break;
                }
            }
            return newHeadIdx;
        }

        /// <summary>
        /// Overwrite the postOperationFix method.  For "a, b and c" or similar: we want "a" to be the head.
        /// </summary>
        protected override int PostOperationFix(int headIdx, Tree[] daughterTrees)
        {
            if (headIdx >= 2)
            {
                string prevLab = Tlp.BasicCategory(daughterTrees[headIdx - 1].Value());
                if (prevLab.Equals(PartsOfSpeech.CoordinatingConjunction) || prevLab.Equals(CONJP))
                {
                    bool origWasInterjection = PartsOfSpeech.Interjection.Equals(Tlp.BasicCategory(daughterTrees[headIdx].Value()));
                    int newHeadIdx = headIdx - 2;
                    // newHeadIdx is now left of conjunction.  Now try going back over commas, etc. for 3+ conjuncts
                    // Don't allow INTJ unless conjoined with INTJ - important in informal genres "Oh and don't forget to call!"
                    while (newHeadIdx >= 0 && ShouldSkip(daughterTrees[newHeadIdx], origWasInterjection))
                    {
                        newHeadIdx--;
                    }
                    // We're now at newHeadIdx < 0 or have found a left head
                    // Now consider going back some number of punct that includes a , or : tagged thing and then find non-punct
                    while (newHeadIdx >= 2)
                    {
                        int nextHead = FindPreviousHead(newHeadIdx, daughterTrees, origWasInterjection);
                        if (nextHead < 0)
                        {
                            break;
                        }
                        newHeadIdx = nextHead;
                    }
                    if (newHeadIdx >= 0)
                    {
                        headIdx = newHeadIdx;
                    }
                }
            }
            return headIdx;
        }

        // Note: The first two SBARQ patterns only work when the SQ
        // structure has already been removed in CoordinationTransformer.
        /*static readonly TregexPattern[] headOfCopulaTregex = {
    // Matches phrases such as "what is wrong"
    TregexPattern.compile("SBARQ < (WHNP $++ (/^VB/ < " + EnglishPatterns.copularWordRegex + " $++ ADJP=head))"),

    // matches WHNP $+ VB<copula $+ NP
    // for example, "Who am I to judge?"
    // !$++ ADJP matches against "Why is the dog pink?"
    TregexPattern.compile("SBARQ < (WHNP=head $++ (/^VB/ < " + EnglishPatterns.copularWordRegex + " $+ NP !$++ ADJP))"),

    // Actually somewhat limited in scope, this detects "Tuesday it is",
    // "Such a great idea this was", etc
    TregexPattern.compile("SINV < (NP=head $++ (NP $++ (VP < (/^(?:VB|AUX)/ < " + EnglishPatterns.copularWordRegex + "))))"),
  };*/

        /*static readonly TregexPattern[] headOfConjpTregex = {
    TregexPattern.compile("CONJP < (CC <: /^(?i:but|and)$/ $+ (RB=head <: /^(?i:not)$/))"),
    TregexPattern.compile("CONJP < (CC <: /^(?i:but)$/ [ ($+ (RB=head <: /^(?i:also|rather)$/)) | ($+ (ADVP=head <: (RB <: /^(?i:also|rather)$/))) ])"),
    TregexPattern.compile("CONJP < (CC <: /^(?i:and)$/ [ ($+ (RB=head <: /^(?i:yet)$/)) | ($+ (ADVP=head <: (RB <: /^(?i:yet)$/))) ])"),
  };*/

        /*private static readonly TregexPattern noVerbOverTempTregex =
            TregexPattern.compile("/^VP/ < NP-TMP !< /^V/ !< NNP|NN|NNPS|NNS|NP|JJ|ADJP|S");*/

        /// <summary>
        /// We use this to avoid making a -TMP or -ADV the head of a copular phrase.
        /// For example, in the sentence "It is hands down the best dessert ...",
        /// we want to avoid using "hands down" as the head.
        /// </summary>
        private static readonly Predicate<Tree> RemoveTmpAndAdv = tree =>
        {
            if (tree == null)
            {
                return false;
            }
            ILabel label = tree.Label();
            if (label == null)
            {
                return false;
            }
            if (label.Value().Contains("-TMP") || label.Value().Contains("-ADV"))
            {
                return false;
            }
            TregexPattern noVerbOverTempTregex =
                TregexPattern.Compile("/^VP/ < NP-TMP !< /^V/ !< NNP|NN|NNPS|NNS|NP|JJ|ADJP|S");
            if (label.Value().StartsWith(AbstractCollinsHeadFinder.VerbPhrase) && noVerbOverTempTregex.Matcher(tree).Matches())
            {
                return false;
            }
            return true;
        };

        /// <summary>
        /// Determine which daughter of the current parse tree is the head.
        /// It assumes that the daughters already have had their heads determined.
        /// Uses special rule for VP heads
        /// </summary>
        /// <param name="t">
        /// The parse tree to examine the daughters of.
        /// This is assumed to never be a leaf
        /// </param>
        /// <returns>The parse tree that is the head</returns>
        protected override Tree DetermineNonTrivialHead(Tree t, Tree parent)
        {
            string motherCat = Tlp.BasicCategory(t.Label().Value());
            
            // Some conj expressions seem to make more sense with the "not" or
            // other key words as the head.  For example, "and not" means
            // something completely different than "and".  Furthermore,
            // downstream code was written assuming "not" would be the head...
            if (motherCat.Equals(CONJP))
            {
                var headOfConjpTregex = new TregexPattern[]
                {
                    TregexPattern.Compile("CONJP < (CC <: /^(?i:but|and)$/ $+ (RB=head <: /^(?i:not)$/))"),
                    TregexPattern.Compile(
                        "CONJP < (CC <: /^(?i:but)$/ [ ($+ (RB=head <: /^(?i:also|rather)$/)) | ($+ (ADVP=head <: (RB <: /^(?i:also|rather)$/))) ])"),
                    TregexPattern.Compile(
                        "CONJP < (CC <: /^(?i:and)$/ [ ($+ (RB=head <: /^(?i:yet)$/)) | ($+ (ADVP=head <: (RB <: /^(?i:yet)$/))) ])"),
                };
                foreach (TregexPattern pattern in headOfConjpTregex)
                {
                    TregexMatcher matcher = pattern.Matcher(t);
                    if (matcher.MatchesAt(t))
                    {
                        return matcher.GetNode("head");
                    }
                }
                // if none of the above patterns match, use the standard method
            }

            if (motherCat.Equals(SBARQ) || motherCat.Equals(SINV))
            {
                if (!makeCopulaHead)
                {
                    var headOfCopulaTregex = new TregexPattern[]
                    {
                        // Matches phrases such as "what is wrong"
                        TregexPattern.Compile("SBARQ < (WHNP $++ (/^VB/ < " + EnglishPatterns.CopularWordRegex +
                                              " $++ ADJP=head))"),

                        // matches WHNP $+ VB<copula $+ NP
                        // for example, "Who am I to judge?"
                        // !$++ ADJP matches against "Why is the dog pink?"
                        TregexPattern.Compile("SBARQ < (WHNP=head $++ (/^VB/ < " + EnglishPatterns.CopularWordRegex +
                                              " $+ NP !$++ ADJP))"),

                        // Actually somewhat limited in scope, this detects "Tuesday it is",
                        // "Such a great idea this was", etc
                        TregexPattern.Compile("SINV < (NP=head $++ (NP $++ (VP < (/^(?:VB|AUX)/ < " +
                                              EnglishPatterns.CopularWordRegex + "))))"),
                    };
                    foreach (TregexPattern pattern in headOfCopulaTregex)
                    {
                        TregexMatcher matcher = pattern.Matcher(t);
                        if (matcher.MatchesAt(t))
                        {
                            return matcher.GetNode("head");
                        }
                    }
                }
                // if none of the above patterns match, use the standard method
            }

            Tree[] tmpFilteredChildren = null;

            // do VPs with auxiliary as special case
            if ((motherCat.Equals(AbstractCollinsHeadFinder.VerbPhrase) || motherCat.Equals("SQ") || motherCat.Equals("SINV")))
            {
                Tree[] kids = t.Children();
                // try to find if there is an auxiliary verb
                // looks for auxiliaries
                if (HasVerbalAuxiliary(kids, verbalAuxiliaries, true) || HasPassiveProgressiveAuxiliary(kids))
                {
                    // string[] how = new string[] {Left, AbstractCollinsHeadFinder.VerbPhrase, CoordinationTransformer.Adjective, CoordinationTransformer.Noun};
                    // Including NP etc seems okay for copular sentences but is
                    // problematic for other auxiliaries, like 'he has an answer'
                    // But maybe doing ADJP is fine!
                    string[] how = { Left, AbstractCollinsHeadFinder.VerbPhrase, CoordinationTransformer.Adjective };
                    
                    //tmpFilteredChildren = ArrayUtils.filter(kids, REMOVE_TMP_AND_ADV);
                    tmpFilteredChildren = kids.Where(k => RemoveTmpAndAdv(k)).ToArray();
                    
                    Tree pti = TraverseLocate(tmpFilteredChildren, how, false);
                    if (pti != null)
                    {
                        return pti;
                    }
                }

                // looks for copular verbs
                if (HasVerbalAuxiliary(kids, copulars, false) && ! IsExistential(t, parent) && ! IsWhQ(t, parent))
                {
                    string[] how;
                    if (motherCat.Equals("SQ"))
                    {
                        how = new string[] { Right, AbstractCollinsHeadFinder.VerbPhrase, CoordinationTransformer.Adjective, CoordinationTransformer.Noun, WHADJP, WHNP };
                    }
                    else
                    {
                        how = new string[] { Left, AbstractCollinsHeadFinder.VerbPhrase, CoordinationTransformer.Adjective, CoordinationTransformer.Noun, WHADJP, WHNP };
                    }
                    // Avoid undesirable heads by filtering them from the list of potential children
                    if (tmpFilteredChildren == null)
                    {
                        //tmpFilteredChildren = ArrayUtils.filter(kids, REMOVE_TMP_AND_ADV);
                        tmpFilteredChildren = kids.Where(k => RemoveTmpAndAdv(k)).ToArray();
                    }
                    Tree pti = TraverseLocate(tmpFilteredChildren, how, false);
                    // In SQ, only allow an NP to become head if there is another one to the left (then it's probably predicative)
                    if (motherCat.Equals(SQ) && pti != null && pti.Label() != null &&
                        pti.Label().Value().StartsWith(CoordinationTransformer.Noun))
                    {
                        bool foundAnotherNp = false;
                        foreach (Tree kid in kids)
                        {
                            if (kid == pti)
                            {
                                break;
                            }
                            else if (kid.Label() != null && kid.Label().Value().StartsWith(CoordinationTransformer.Noun))
                            {
                                foundAnotherNp = true;
                                break;
                            }
                        }
                        if (! foundAnotherNp)
                        {
                            pti = null;
                        }
                    }

                    if (pti != null)
                    {
                        return pti;
                    }
                }
            }

            Tree hd = base.DetermineNonTrivialHead(t, parent);

            /* ----
    // This should now be handled at the AbstractCollinsHeadFinder level, so see if we can comment this out
    // Heuristically repair punctuation heads
    Tree[] hdChildren = hd.children();
    if (hdChildren != null && hdChildren.length > 0 &&
        hdChildren[0].isLeaf()) {
      if (tlp.isPunctuationWord(hdChildren[0].label().value())) {
         Tree[] tChildren = t.children();
         for (int i = tChildren.length - 1; i >= 0; i--) {
           if (!tlp.isPunctuationWord(tChildren[i].children()[0].label().value())) {
             hd = tChildren[i];
             break;
           }
         }
      }
    }
    */
            return hd;
        }

        /// <summary>
        /// Checks whether the tree t is an existential constituent
        /// There are two cases:
        /// -- affirmative sentences in which "there" is a left sister of the VP
        /// -- questions in which "there" is a daughter of the SQ.
        /// </summary>
        private bool IsExistential(Tree t, Tree parent)
        {
            bool toReturn = false;
            string motherCat = Tlp.BasicCategory(t.Label().Value());
            // affirmative case
            if (motherCat.Equals(AbstractCollinsHeadFinder.VerbPhrase) && parent != null)
            {
                //take t and the sisters
                Tree[] kids = parent.Children();
                // iterate over the sisters before t and checks if existential
                foreach (Tree kid in kids)
                {
                    if (!kid.Value().Equals(AbstractCollinsHeadFinder.VerbPhrase))
                    {
                        List<ILabel> tags = kid.PreTerminalYield();
                        foreach (ILabel tag in tags)
                        {
                            if (tag.Value().Equals(PartsOfSpeech.ExistentialThere))
                            {
                                toReturn = true;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
                // question case
            else if (motherCat.StartsWith(SQ) && parent != null)
            {
                //take the daughters
                Tree[] kids = parent.Children();
                // iterate over the daughters and checks if existential
                foreach (Tree kid in kids)
                {
                    if (!PartsOfSpeech.IsVerb(kid.Value()))
                    {
                        //not necessary to look into the verb
                        List<ILabel> tags = kid.PreTerminalYield();
                        foreach (ILabel tag in tags)
                        {
                            if (tag.Value().Equals(PartsOfSpeech.ExistentialThere))
                            {
                                toReturn = true;
                            }
                        }
                    }
                }
            }
            return toReturn;
        }


        /// <summary>
        /// Is the tree t a WH-question?
        /// At present this is only true if the tree t is a SQ having a WH.* sister and headed by a SBARQ.
        /// (It was changed to looser definition in Feb 2006.)
        /// </summary>
        private static bool IsWhQ(Tree t, Tree parent)
        {
            if (t == null)
            {
                return false;
            }
            bool toReturn = false;
            if (t.Value().StartsWith(SQ))
            {
                if (parent != null && parent.Value().Equals(SBARQ))
                {
                    Tree[] kids = parent.Children();
                    foreach (Tree kid in kids)
                    {
                        // looks for a WH.*
                        if (kid.Value().StartsWith("WH"))
                        {
                            toReturn = true;
                        }
                    }
                }
            }
            return toReturn;
        }

        private bool IsVerbalAuxiliary(Tree preterminal, Set<string> verbalSet, bool allowJustTagMatch)
        {
            if (preterminal.IsPreTerminal())
            {
                ILabel kidLabel = preterminal.Label();
                string tag = null;
                if (kidLabel is IHasTag)
                {
                    tag = ((IHasTag) kidLabel).Tag();
                }
                if (tag == null)
                {
                    tag = preterminal.Value();
                }
                ILabel wordLabel = preterminal.FirstChild().Label();
                string word = null;
                if (wordLabel is IHasWord)
                {
                    word = ((IHasWord) wordLabel).GetWord();
                }
                if (word == null)
                {
                    word = wordLabel.Value();
                }

                string lcWord = word.ToLower();
                if (allowJustTagMatch && unambiguousAuxiliaryTags.Contains(tag) ||
                    verbalTags.Contains(tag) && verbalSet.Contains(lcWord))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if this tree is a preterminal that is a verbal auxiliary.
        /// </summary>
        /// <param name="t">A tree to examine for being an auxiliary.</param>
        /// <returns>Whether it is a verbal auxiliary (be, do, have, get)</returns>
        public bool IsVerbalAuxiliary(Tree t)
        {
            return IsVerbalAuxiliary(t, verbalAuxiliaries, true);
        }


        // now overly complex so it deals with coordinations.  Maybe change this class to use tregrex?
        private bool HasPassiveProgressiveAuxiliary(Tree[] kids)
        {
            bool foundPassiveVp = false;
            bool foundPassiveAux = false;
            foreach (Tree kid in kids)
            {
                if (IsVerbalAuxiliary(kid, passiveAuxiliaries, false))
                {
                    foundPassiveAux = true;
                }
                else if (kid.IsPhrasal())
                {
                    ILabel kidLabel = kid.Label();
                    string cat = null;
                    if (kidLabel is IHasCategory)
                    {
                        cat = ((IHasCategory) kidLabel).Category();
                    }
                    if (cat == null)
                    {
                        cat = kid.Value();
                    }
                    if (!cat.StartsWith(AbstractCollinsHeadFinder.VerbPhrase))
                    {
                        continue;
                    }
                    Tree[] kidkids = kid.Children();
                    bool foundParticipleInVp = false;
                    foreach (Tree kidkid in kidkids)
                    {
                        if (kidkid.IsPreTerminal())
                        {
                            ILabel kidkidLabel = kidkid.Label();
                            string tag = null;
                            if (kidkidLabel is IHasTag)
                            {
                                tag = ((IHasTag) kidkidLabel).Tag();
                            }
                            if (tag == null)
                            {
                                tag = kidkid.Value();
                            }
                            // we allow in VBD because of frequent tagging mistakes
                            if (PartsOfSpeech.VerbPastParticiple == tag 
                                || PartsOfSpeech.VerbGerundOrPresentParticiple == tag 
                                || PartsOfSpeech.VerbPastTense == tag)
                            {
                                foundPassiveVp = true;
                                break;
                            }
                            else if (PartsOfSpeech.CoordinatingConjunction == tag && foundParticipleInVp)
                            {
                                foundPassiveVp = true;
                                break;
                            }
                        }
                        else if (kidkid.IsPhrasal())
                        {
                            string catcat = null;
                            if (kidLabel is IHasCategory)
                            {
                                catcat = ((IHasCategory) kidLabel).Category();
                            }
                            if (catcat == null)
                            {
                                catcat = kid.Value();
                            }
                            if (AbstractCollinsHeadFinder.VerbPhrase.Equals(catcat))
                            {
                                foundParticipleInVp = VpContainsParticiple(kidkid);
                            }
                            else if ((CONJP.Equals(catcat) || PRN.Equals(catcat)) && foundParticipleInVp)
                            {
                                // occasionally get PRN in CONJ-like structures
                                foundPassiveVp = true;
                                break;
                            }
                        }
                    }
                }
                if (foundPassiveAux && foundPassiveVp)
                {
                    break;
                }
            } // end for (Tree kid : kids)
            return foundPassiveAux && foundPassiveVp;
        }

        private static bool VpContainsParticiple(Tree t)
        {
            foreach (Tree kid in t.Children())
            {
                if (kid.IsPreTerminal())
                {
                    ILabel kidLabel = kid.Label();
                    string tag = null;
                    if (kidLabel is IHasTag)
                    {
                        tag = ((IHasTag) kidLabel).Tag();
                    }
                    if (tag == null)
                    {
                        tag = kid.Value();
                    }
                    if (PartsOfSpeech.VerbPastParticiple == tag 
                        || PartsOfSpeech.VerbGerundOrPresentParticiple == tag 
                        || PartsOfSpeech.VerbPastTense == tag)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This looks to see whether any of the children is a preterminal headed by a word
        /// which is within the set verbalSet (which in practice is either
        /// auxiliary or copula verbs).  It only returns true if it's a preterminal head, since
        /// you don't want to pick things up in phrasal daughters.  That is an error.
        /// </summary>
        /// <param name="kids">The child trees</param>
        /// <param name="verbalSet">The set of words</param>
        /// <param name="allowTagOnlyMatch">
        /// If true, it's sufficient to match on an unambiguous auxiliary tag.
        /// Make true iff verbalSet is "all auxiliaries"
        /// </param>
        /// <returns>
        /// true if one of the child trees is a preterminal verb headed by a word in verbalSet
        /// </returns>
        private bool HasVerbalAuxiliary(Tree[] kids, Set<string> verbalSet, bool allowTagOnlyMatch)
        {
            foreach (Tree kid in kids)
            {
                if (IsVerbalAuxiliary(kid, verbalSet, allowTagOnlyMatch))
                {
                    return true;
                }
            }
            return false;
        }

    }
}