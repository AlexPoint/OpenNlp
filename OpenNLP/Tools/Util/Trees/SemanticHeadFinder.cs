using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Trees.TRegex;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * Implements a 'semantic head' variant of the the HeadFinder found
 * in Michael Collins' 1999 thesis.
 * This version chooses the semantic head verb rather than the verb form
 * for cases with verbs.  And it makes similar themed changes to other
 * categories: e.g., in question phrases, like "Which Brazilian game", the
 * head is made "game" not "Which" as in common PTB head rules.<p/>
 * <p/>
 * By default the SemanticHeadFinder uses a treatment of copula where the
 * complement of the copula is taken as the head.  That is, a sentence like
 * "Bill is big" will be analyzed as <p/>
 * <p/>
 * <code>nsubj</code>(big, Bill) <br/>
 * <code>cop</code>(big, is) <p/>
 * <p/>
 * This analysis is used for questions and declaratives for adjective
 * complements and declarative nominal complements.  However Wh-sentences
 * with nominal complements do not receive this treatment.
 * "Who is the president?" is analyzed with "the president" as nsubj and "who"
 * as "attr" of the copula:<p/><p>
 * <code>nsubj</code>(is, president)<br/>
 * <code>attr</code>(is, Who) <p/>
 * <p/>
 * (Such nominal copula sentences are complex: arguably, depending on the
 * circumstances, several analyses are possible, with either the overt NP able
 * to be any of the subject, the predicate, or one of two referential entities
 * connected by an equational copula.  These uses aren't differentiated.)
 * <p/>
 * Existential sentences are treated as follows:  <br/>
 * "There is a man" <br/>
 * <code>expl</code>(is, There) <br/>
 * <code>det</code>(man-4, a-3) <br/>
 * <code>nsubj</code>(is-2, man-4)<br/>
 *
 * @author John Rappaport
 * @author Marie-Catherine de Marneffe
 * @author Anna Rafferty
 */

    public class SemanticHeadFinder : ModCollinsHeadFinder
    {
        //private static readonly bool DEBUG = System.getProperty("SemanticHeadFinder", null) != null;

        /* A few times the apostrophe is missing on "'s", so we have "s" */
        /* Tricky auxiliaries: "na" is from "gonna", "ve" from "Weve", etc.  "of" as non-standard for "have" */

        private static readonly String[] auxiliaries =
        {
            "will", "wo", "shall", "sha", "may", "might", "should", "would",
            "can", "could", "ca", "must", "has", "have", "had", "having", "get", "gets", "getting", "got", "gotten",
            "do", "does", "did", "to", "'ve", "ve", "v", "'d", "d", "'ll", "ll", "na", "of", "hav", "hvae", "as"
        };

        private static readonly String[] beGetVerbs =
        {
            "be", "being", "been", "am", "are", "r", "is", "ai", "was",
            "were", "'m", "m", "'re", "'s", "s", "art", "ar", "get", "getting", "gets", "got"
        };

        public static readonly String[] copulaVerbs =
        {
            "be", "being", "been", "am", "are", "r", "is", "ai", "was",
            "were", "'m", "m", "ar", "art", "'re", "'s", "s", "wase"
        };

        // include Charniak tags so can do BLLIP right
        private static readonly String[] verbTags = {"TO", "MD", "VB", "VBD", "VBP", "VBZ", "VBG", "VBN", "AUX", "AUXG"};
        // These ones are always auxiliaries, even if the word is "too", "my", or whatever else appears in web text.
        private static readonly String[] unambiguousAuxTags = {"TO", "MD", "AUX", "AUXG"};


        private readonly Set<String> verbalAuxiliaries;
        private readonly Set<String> copulars;
        private readonly Set<String> passiveAuxiliaries;
        private readonly Set<String> verbalTags;
        private readonly Set<String> unambiguousAuxiliaryTags;

        private readonly bool makeCopulaHead;


        public SemanticHeadFinder() : this(new PennTreebankLanguagePack(), true)
        {
        }

        public SemanticHeadFinder(bool noCopulaHead) : this(new PennTreebankLanguagePack(), noCopulaHead)
        {
        }


        /** Create a SemanticHeadFinder.
   *
   * @param tlp The TreebankLanguagePack, used by the superclass to get basic
   *     category of constituents.
   * @param noCopulaHead If true, a copular verb
   *     (be, seem, appear, stay, remain, resemble, become)
   *     is not treated as head when it has an AdjP or NP complement.  If false,
   *     a copula verb is still always treated as a head.  But it will still
   *     be treated as an auxiliary in periphrastic tenses with a VP complement.
   */

        public SemanticHeadFinder(AbstractTreebankLanguagePack tlp, bool noCopulaHead) : base(tlp)
        {
            ruleChanges();

            // make a distinction between auxiliaries and copula verbs to
            // get the NP has semantic head in sentences like "Bill is an honest man".  (Added "sha" for "shan't" May 2009
            verbalAuxiliaries = new HashSet<string>(auxiliaries);

            passiveAuxiliaries = new HashSet<string>(beGetVerbs);

            //copula verbs having an NP complement
            copulars = new HashSet<string>();
            if (noCopulaHead)
            {
                copulars.AddAll(copulaVerbs);
            }

            // TODO: reverse the polarity of noCopulaHead
            this.makeCopulaHead = !noCopulaHead;

            verbalTags = new HashSet<string>(verbTags);
            unambiguousAuxiliaryTags = new HashSet<string>(unambiguousAuxTags);
        }

        //@Override
        public override bool makesCopulaHead()
        {
            return makeCopulaHead;
        }

        //makes modifications of Collins' rules to better fit with semantic notions of heads
        private void ruleChanges()
        {
            //  NP: don't want a POS to be the head
            // verbs are here so that POS isn't favored in the case of bad parses
            nonTerminalInfo["NP"] =
                new String[][]
                {
                    new string[] {"rightdis", "NN", "NNP", "NNPS", "NNS", "NX", "NML", "JJR", "WP"},
                    new string[] {"left", "NP", "PRP"}, new string[] {"rightdis", "$", "ADJP", "FW"},
                    new string[] {"right", "CD"},
                    new string[] {"rightdis", "JJ", "JJS", "QP", "DT", "WDT", "NML", "PRN", "RB", "RBR", "ADVP"},
                    new string[] {"rightdis", "VP", "VB", "VBZ", "VBD", "VBP"},
                    new string[] {"left", "POS"}
                };
            nonTerminalInfo["NX"] = nonTerminalInfo["NP"];
            nonTerminalInfo["NML"] = nonTerminalInfo["NP"];
            // WHNP clauses should have the same sort of head as an NP
            // but it a WHNP has a NP and a WHNP under it, the WHNP should be the head.  E.g.,  (WHNP (WHNP (WP$ whose) (JJ chief) (JJ executive) (NN officer))(, ,) (NP (NNP James) (NNP Gatward))(, ,))
            nonTerminalInfo["WHNP"] = new String[][]
            {
                new string[] {"rightdis", "NN", "NNP", "NNPS", "NNS", "NX", "NML", "JJR", "WP"},
                new string[] {"left", "WHNP", "NP"}, new string[] {"rightdis", "$", "ADJP", "PRN", "FW"},
                new string[] {"right", "CD"}, new string[] {"rightdis", "JJ", "JJS", "RB", "QP"},
                new string[] {"left", "WHPP", "WHADJP", "WP$", "WDT"}
            };
            //WHADJP
            nonTerminalInfo["WHADJP"] = new String[][]
            {new string[] {"left", "ADJP", "JJ", "JJR", "WP"}, new string[] {"right", "RB"}, new string[] {"right"}};
            //WHADJP
            nonTerminalInfo["WHADVP"] = new String[][] {new string[] {"rightdis", "WRB", "WHADVP", "RB", "JJ"}};
            // if not WRB or WHADVP, probably has flat NP structure, allow JJ for "how long" constructions
            // QP: we don't want the first CD to be the semantic head (e.g., "three billion": head should be "billion"), so we go from right to left
            nonTerminalInfo["QP"] =
                new String[][]
                {
                    new string[]
                    {"right", "$", "NNS", "NN", "CD", "JJ", "PDT", "DT", "IN", "RB", "NCD", "QP", "JJR", "JJS"}
                };

            // S, SBAR and SQ clauses should prefer the main verb as the head
            // S: "He considered him a friend" -> we want a friend to be the head
            nonTerminalInfo["S"] = new String[][]
            {new string[] {"left", "VP", "S", "FRAG", "SBAR", "ADJP", "UCP", "TO"}, new string[] {"right", "NP"}};

            nonTerminalInfo["SBAR"] = new String[][]
            {
                new string[]
                {"left", "S", "SQ", "SINV", "SBAR", "FRAG", "VP", "WHNP", "WHPP", "WHADVP", "WHADJP", "IN", "DT"}
            };
            // VP shouldn't be needed in SBAR, but occurs in one buggy tree in PTB3 wsj_1457 and otherwise does no harm

            nonTerminalInfo["SQ"] = new String[][]
            {new string[] {"left", "VP", "SQ", "ADJP", "VB", "VBZ", "VBD", "VBP", "MD", "AUX", "AUXG"}};


            // UCP take the first element as head
            nonTerminalInfo["UCP"] = new String[][] {new string[] {"left"}};

            // CONJP: we want different heads for "but also" and "but not" and we don't want "not" to be the head in "not to mention"; now make "mention" head of "not to mention"
            nonTerminalInfo["CONJP"] = new String[][] {new string[] {"right", "CC", "VB", "JJ", "RB", "IN"}};

            // FRAG: crap rule needs to be change if you want to parse
            // glosses; but it is correct to have ADJP and ADVP before S
            // because of weird parses of reduced sentences.
            nonTerminalInfo["FRAG"] = new String[][]
            {
                new string[] {"left", "IN"}, new string[] {"right", "RB"}, new string[] {"left", "NP"},
                new string[] {"left", "ADJP", "ADVP", "FRAG", "S", "SBAR", "VP"}
            };

            // PRN: sentence first
            nonTerminalInfo["PRN"] = new String[][]
            {
                new string[]
                {
                    "left", "VP", "SQ", "S", "SINV", "SBAR", "NP", "ADJP", "PP", "ADVP", "INTJ", "WHNP", "NAC", "VBP",
                    "JJ", "NN", "NNP"
                }
            };

            // add the constituent XS (special node to add a layer in a QP tree introduced in our QPTreeTransformer)
            nonTerminalInfo["XS"] = new String[][] {new string[] {"right", "IN"}};

            // add a rule to deal with the CoNLL data
            nonTerminalInfo["EMBED"] = new String[][] {new string[] {"right", "INTJ"}};

        }


        private bool shouldSkip(Tree t, bool origWasInterjection)
        {
            return t.isPreTerminal() &&
                   (tlp.isPunctuationTag(t.value()) || ! origWasInterjection && "UH".Equals(t.value())) ||
                   "INTJ".Equals(t.value()) && ! origWasInterjection;
        }

        private int findPreviousHead(int headIdx, Tree[] daughterTrees, bool origWasInterjection)
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
                String label = tlp.basicCategory(daughterTrees[newHeadIdx].value());
                if (",".Equals(label) || ":".Equals(label))
                {
                    seenSeparator = true;
                }
                else if (daughterTrees[newHeadIdx].isPreTerminal() &&
                         (tlp.isPunctuationTag(label) || ! origWasInterjection && "UH".Equals(label)) ||
                         "INTJ".Equals(label) && ! origWasInterjection)
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

        /**
   * Overwrite the postOperationFix method.  For "a, b and c" or similar: we want "a" to be the head.
   */
        //@Override
        protected override int postOperationFix(int headIdx, Tree[] daughterTrees)
        {
            if (headIdx >= 2)
            {
                String prevLab = tlp.basicCategory(daughterTrees[headIdx - 1].value());
                if (prevLab.Equals("CC") || prevLab.Equals("CONJP"))
                {
                    bool origWasInterjection = "UH".Equals(tlp.basicCategory(daughterTrees[headIdx].value()));
                    int newHeadIdx = headIdx - 2;
                    // newHeadIdx is now left of conjunction.  Now try going back over commas, etc. for 3+ conjuncts
                    // Don't allow INTJ unless conjoined with INTJ - important in informal genres "Oh and don't forget to call!"
                    while (newHeadIdx >= 0 && shouldSkip(daughterTrees[newHeadIdx], origWasInterjection))
                    {
                        newHeadIdx--;
                    }
                    // We're now at newHeadIdx < 0 or have found a left head
                    // Now consider going back some number of punct that includes a , or : tagged thing and then find non-punct
                    while (newHeadIdx >= 2)
                    {
                        int nextHead = findPreviousHead(newHeadIdx, daughterTrees, origWasInterjection);
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

        /**
   * We use this to avoid making a -TMP or -ADV the head of a copular phrase.
   * For example, in the sentence "It is hands down the best dessert ...",
   * we want to avoid using "hands down" as the head.
   */

        private static readonly Predicate<Tree> REMOVE_TMP_AND_ADV = tree =>
        {
            if (tree == null)
            {
                return false;
            }
            Label label = tree.label();
            if (label == null)
            {
                return false;
            }
            if (label.value().Contains("-TMP") || label.value().Contains("-ADV"))
            {
                return false;
            }
            TregexPattern noVerbOverTempTregex =
                TregexPattern.compile("/^VP/ < NP-TMP !< /^V/ !< NNP|NN|NNPS|NNS|NP|JJ|ADJP|S");
            if (label.value().StartsWith("VP") && noVerbOverTempTregex.matcher(tree).matches())
            {
                return false;
            }
            return true;
        };

        /**
   * Determine which daughter of the current parse tree is the
   * head.  It assumes that the daughters already have had their
   * heads determined.  Uses special rule for VP heads
   *
   * @param t The parse tree to examine the daughters of.
   *          This is assumed to never be a leaf
   * @return The parse tree that is the head
   */
        //@Override
        protected override Tree determineNonTrivialHead(Tree t, Tree parent)
        {
            String motherCat = tlp.basicCategory(t.label().value());

            /*if (DEBUG) {
      System.err.println("At " + motherCat + ", my parent is " + parent);
    }*/

            // Some conj expressions seem to make more sense with the "not" or
            // other key words as the head.  For example, "and not" means
            // something completely different than "and".  Furthermore,
            // downstream code was written assuming "not" would be the head...
            if (motherCat.Equals("CONJP"))
            {
                var headOfConjpTregex = new TregexPattern[]
                {
                    TregexPattern.compile("CONJP < (CC <: /^(?i:but|and)$/ $+ (RB=head <: /^(?i:not)$/))"),
                    TregexPattern.compile(
                        "CONJP < (CC <: /^(?i:but)$/ [ ($+ (RB=head <: /^(?i:also|rather)$/)) | ($+ (ADVP=head <: (RB <: /^(?i:also|rather)$/))) ])"),
                    TregexPattern.compile(
                        "CONJP < (CC <: /^(?i:and)$/ [ ($+ (RB=head <: /^(?i:yet)$/)) | ($+ (ADVP=head <: (RB <: /^(?i:yet)$/))) ])"),
                };
                foreach (TregexPattern pattern in headOfConjpTregex)
                {
                    TregexMatcher matcher = pattern.matcher(t);
                    if (matcher.matchesAt(t))
                    {
                        return matcher.getNode("head");
                    }
                }
                // if none of the above patterns match, use the standard method
            }

            if (motherCat.Equals("SBARQ") || motherCat.Equals("SINV"))
            {
                if (!makeCopulaHead)
                {
                    var headOfCopulaTregex = new TregexPattern[]
                    {
                        // Matches phrases such as "what is wrong"
                        TregexPattern.compile("SBARQ < (WHNP $++ (/^VB/ < " + EnglishPatterns.copularWordRegex +
                                              " $++ ADJP=head))"),

                        // matches WHNP $+ VB<copula $+ NP
                        // for example, "Who am I to judge?"
                        // !$++ ADJP matches against "Why is the dog pink?"
                        TregexPattern.compile("SBARQ < (WHNP=head $++ (/^VB/ < " + EnglishPatterns.copularWordRegex +
                                              " $+ NP !$++ ADJP))"),

                        // Actually somewhat limited in scope, this detects "Tuesday it is",
                        // "Such a great idea this was", etc
                        TregexPattern.compile("SINV < (NP=head $++ (NP $++ (VP < (/^(?:VB|AUX)/ < " +
                                              EnglishPatterns.copularWordRegex + "))))"),
                    };
                    foreach (TregexPattern pattern in headOfCopulaTregex)
                    {
                        TregexMatcher matcher = pattern.matcher(t);
                        if (matcher.matchesAt(t))
                        {
                            return matcher.getNode("head");
                        }
                    }
                }
                // if none of the above patterns match, use the standard method
            }

            Tree[] tmpFilteredChildren = null;

            // do VPs with auxiliary as special case
            if ((motherCat.Equals("VP") || motherCat.Equals("SQ") || motherCat.Equals("SINV")))
            {
                Tree[] kids = t.children();
                // try to find if there is an auxiliary verb

                /*if (DEBUG) {
        System.err.println("Semantic head finder: at VP");
        System.err.println("Class is " + t.getClass().getName());
        t.pennPrint(System.err);
        //System.err.println("hasVerbalAuxiliary = " + hasVerbalAuxiliary(kids, verbalAuxiliaries));
      }*/

                // looks for auxiliaries
                if (hasVerbalAuxiliary(kids, verbalAuxiliaries, true) || hasPassiveProgressiveAuxiliary(kids))
                {
                    // String[] how = new String[] {"left", "VP", "ADJP", "NP"};
                    // Including NP etc seems okay for copular sentences but is
                    // problematic for other auxiliaries, like 'he has an answer'
                    // But maybe doing ADJP is fine!
                    String[] how = {"left", "VP", "ADJP"};
                    if (tmpFilteredChildren == null)
                    {
                        //tmpFilteredChildren = ArrayUtils.filter(kids, REMOVE_TMP_AND_ADV);
                        tmpFilteredChildren = kids.Where(k => REMOVE_TMP_AND_ADV(k)).ToArray();
                    }
                    Tree pti = traverseLocate(tmpFilteredChildren, how, false);
                    /*if (DEBUG) {
          System.err.println("Determined head (case 1) for " + t.value() + " is: " + pti);
        }*/
                    if (pti != null)
                    {
                        return pti;
                        // } else {
                        // System.err.println("------");
                        // System.err.println("SemanticHeadFinder failed to reassign head for");
                        // t.pennPrint(System.err);
                        // System.err.println("------");
                    }
                }

                // looks for copular verbs
                if (hasVerbalAuxiliary(kids, copulars, false) && ! isExistential(t, parent) && ! isWHQ(t, parent))
                {
                    String[] how;
                    if (motherCat.Equals("SQ"))
                    {
                        how = new String[] {"right", "VP", "ADJP", "NP", "WHADJP", "WHNP"};
                    }
                    else
                    {
                        how = new String[] {"left", "VP", "ADJP", "NP", "WHADJP", "WHNP"};
                    }
                    // Avoid undesirable heads by filtering them from the list of potential children
                    if (tmpFilteredChildren == null)
                    {
                        //tmpFilteredChildren = ArrayUtils.filter(kids, REMOVE_TMP_AND_ADV);
                        tmpFilteredChildren = kids.Where(k => REMOVE_TMP_AND_ADV(k)).ToArray();
                    }
                    Tree pti = traverseLocate(tmpFilteredChildren, how, false);
                    // In SQ, only allow an NP to become head if there is another one to the left (then it's probably predicative)
                    if (motherCat.Equals("SQ") && pti != null && pti.label() != null &&
                        pti.label().value().StartsWith("NP"))
                    {
                        bool foundAnotherNp = false;
                        foreach (Tree kid in kids)
                        {
                            if (kid == pti)
                            {
                                break;
                            }
                            else if (kid.label() != null && kid.label().value().StartsWith("NP"))
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

                    /*if (DEBUG) {
          System.err.println("Determined head (case 2) for " + t.value() + " is: " + pti);
        }s*/
                    if (pti != null)
                    {
                        return pti;
                    }
                    else
                    {
                        /*if (DEBUG) {
            System.err.println("------");
            System.err.println("SemanticHeadFinder failed to reassign head for");
            t.pennPrint(System.err);
            System.err.println("------");
          }*/
                    }
                }
            }

            Tree hd = base.determineNonTrivialHead(t, parent);

            /* ----
    // This should now be handled at the AbstractCollinsHeadFinder level, so see if we can comment this out
    // Heuristically repair punctuation heads
    Tree[] hdChildren = hd.children();
    if (hdChildren != null && hdChildren.length > 0 &&
        hdChildren[0].isLeaf()) {
      if (tlp.isPunctuationWord(hdChildren[0].label().value())) {
         Tree[] tChildren = t.children();
         if (DEBUG) {
           System.err.printf("head is punct: %s\n", hdChildren[0].label());
         }
         for (int i = tChildren.length - 1; i >= 0; i--) {
           if (!tlp.isPunctuationWord(tChildren[i].children()[0].label().value())) {
             hd = tChildren[i];
             if (DEBUG) {
               System.err.printf("New head of %s is %s%n", hd.label(), hd.children()[0].label());
             }
             break;
           }
         }
      }
    }
    */

            /*if (DEBUG) {
      System.err.println("Determined head (case 3) for " + t.value() + " is: " + hd);
    }*/
            return hd;
        }

        /* Checks whether the tree t is an existential constituent
   * There are two cases:
   * -- affirmative sentences in which "there" is a left sister of the VP
   * -- questions in which "there" is a daughter of the SQ.
   *
   */

        private bool isExistential(Tree t, Tree parent)
        {
            /*if (DEBUG) {
      System.err.println("isExistential: " + t + ' ' + parent);
    }*/
            bool toReturn = false;
            String motherCat = tlp.basicCategory(t.label().value());
            // affirmative case
            if (motherCat.Equals("VP") && parent != null)
            {
                //take t and the sisters
                Tree[] kids = parent.children();
                // iterate over the sisters before t and checks if existential
                foreach (Tree kid in kids)
                {
                    if (!kid.value().Equals("VP"))
                    {
                        List<Label> tags = kid.preTerminalYield();
                        foreach (Label tag in tags)
                        {
                            if (tag.value().Equals("EX"))
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
            else if (motherCat.StartsWith("SQ") && parent != null)
            {
                //take the daughters
                Tree[] kids = parent.children();
                // iterate over the daughters and checks if existential
                foreach (Tree kid in kids)
                {
                    if (!kid.value().StartsWith("VB"))
                    {
//not necessary to look into the verb
                        List<Label> tags = kid.preTerminalYield();
                        foreach (Label tag in tags)
                        {
                            if (tag.value().Equals("EX"))
                            {
                                toReturn = true;
                            }
                        }
                    }
                }
            }

            /*if (DEBUG) {
      System.err.println("decision " + toReturn);
    }*/

            return toReturn;
        }


        /* Is the tree t a WH-question?
   *  At present this is only true if the tree t is a SQ having a WH.* sister
   *  and headed by a SBARQ.
   * (It was changed to looser definition in Feb 2006.)
   *
   */

        private static bool isWHQ(Tree t, Tree parent)
        {
            if (t == null)
            {
                return false;
            }
            bool toReturn = false;
            if (t.value().StartsWith("SQ"))
            {
                if (parent != null && parent.value().Equals("SBARQ"))
                {
                    Tree[] kids = parent.children();
                    foreach (Tree kid in kids)
                    {
                        // looks for a WH.*
                        if (kid.value().StartsWith("WH"))
                        {
                            toReturn = true;
                        }
                    }
                }
            }

            /*if (DEBUG) {
      System.err.println("in isWH, decision: " + toReturn + " for node " + t);
    }s*/

            return toReturn;
        }

        private bool isVerbalAuxiliary(Tree preterminal, Set<String> verbalSet, bool allowJustTagMatch)
        {
            if (preterminal.isPreTerminal())
            {
                Label kidLabel = preterminal.label();
                String tag = null;
                if (kidLabel is HasTag)
                {
                    tag = ((HasTag) kidLabel).tag();
                }
                if (tag == null)
                {
                    tag = preterminal.value();
                }
                Label wordLabel = preterminal.firstChild().label();
                String word = null;
                if (wordLabel is HasWord)
                {
                    word = ((HasWord) wordLabel).word();
                }
                if (word == null)
                {
                    word = wordLabel.value();
                }

                /*if (DEBUG) {
        System.err.println("Checking " + preterminal.value() + " head is " + word + '/' + tag);
      }*/
                String lcWord = word.ToLower();
                if (allowJustTagMatch && unambiguousAuxiliaryTags.Contains(tag) ||
                    verbalTags.Contains(tag) && verbalSet.Contains(lcWord))
                {
                    /*if (DEBUG) {
          System.err.println("isAuxiliary found desired type of aux");
        }*/
                    return true;
                }
            }
            return false;
        }

        /**
   * Returns true if this tree is a preterminal that is a verbal auxiliary.
   *
   * @param t A tree to examine for being an auxiliary.
   * @return Whether it is a verbal auxiliary (be, do, have, get)
   */

        public bool isVerbalAuxiliary(Tree t)
        {
            return isVerbalAuxiliary(t, verbalAuxiliaries, true);
        }


        // now overly complex so it deals with coordinations.  Maybe change this class to use tregrex?
        private bool hasPassiveProgressiveAuxiliary(Tree[] kids)
        {
            /*if (DEBUG) {
      System.err.println("Checking for passive/progressive auxiliary");
    }*/
            bool foundPassiveVP = false;
            bool foundPassiveAux = false;
            foreach (Tree kid in kids)
            {
                /*if (DEBUG) {
        System.err.println("  checking in " + kid);
      }*/
                if (isVerbalAuxiliary(kid, passiveAuxiliaries, false))
                {
                    foundPassiveAux = true;
                }
                else if (kid.isPhrasal())
                {
                    Label kidLabel = kid.label();
                    String cat = null;
                    if (kidLabel is HasCategory)
                    {
                        cat = ((HasCategory) kidLabel).category();
                    }
                    if (cat == null)
                    {
                        cat = kid.value();
                    }
                    if (! cat.StartsWith("VP"))
                    {
                        continue;
                    }
                    /*if (DEBUG) {
          System.err.println("hasPassiveProgressiveAuxiliary found VP");
        }*/
                    Tree[] kidkids = kid.children();
                    bool foundParticipleInVp = false;
                    foreach (Tree kidkid in kidkids)
                    {
                        /*if (DEBUG) {
            System.err.println("  hasPassiveProgressiveAuxiliary examining " + kidkid);
          }*/
                        if (kidkid.isPreTerminal())
                        {
                            Label kidkidLabel = kidkid.label();
                            String tag = null;
                            if (kidkidLabel is HasTag)
                            {
                                tag = ((HasTag) kidkidLabel).tag();
                            }
                            if (tag == null)
                            {
                                tag = kidkid.value();
                            }
                            // we allow in VBD because of frequent tagging mistakes
                            if ("VBN".Equals(tag) || "VBG".Equals(tag) || "VBD".Equals(tag))
                            {
                                foundPassiveVP = true;
                                /*if (DEBUG) {
                System.err.println("hasPassiveAuxiliary found VBN/VBG/VBD VP");
              }*/
                                break;
                            }
                            else if ("CC".Equals(tag) && foundParticipleInVp)
                            {
                                foundPassiveVP = true;
                                /*if (DEBUG) {
                System.err.println("hasPassiveAuxiliary [coordination] found (VP (VP[VBN/VBG/VBD] CC");
              }*/
                                break;
                            }
                        }
                        else if (kidkid.isPhrasal())
                        {
                            String catcat = null;
                            if (kidLabel is HasCategory)
                            {
                                catcat = ((HasCategory) kidLabel).category();
                            }
                            if (catcat == null)
                            {
                                catcat = kid.value();
                            }
                            if ("VP".Equals(catcat))
                            {
                                /*if (DEBUG) {
                System.err.println("hasPassiveAuxiliary found (VP (VP)), recursing");
              }*/
                                foundParticipleInVp = vpContainsParticiple(kidkid);
                            }
                            else if (("CONJP".Equals(catcat) || "PRN".Equals(catcat)) && foundParticipleInVp)
                            {
                                // occasionally get PRN in CONJ-like structures
                                foundPassiveVP = true;
                                /*if (DEBUG) {
                System.err.println("hasPassiveAuxiliary [coordination] found (VP (VP[VBN/VBG/VBD] CONJP");
              }*/
                                break;
                            }
                        }
                    }
                }
                if (foundPassiveAux && foundPassiveVP)
                {
                    break;
                }
            } // end for (Tree kid : kids)
            /*if (DEBUG) {
      System.err.println("hasPassiveProgressiveAuxiliary returns " + (foundPassiveAux && foundPassiveVP));
    }*/
            return foundPassiveAux && foundPassiveVP;
        }

        private static bool vpContainsParticiple(Tree t)
        {
            foreach (Tree kid in t.children())
            {
                /*if (DEBUG) {
        System.err.println("vpContainsParticiple examining " + kid);
      }*/
                if (kid.isPreTerminal())
                {
                    Label kidLabel = kid.label();
                    String tag = null;
                    if (kidLabel is HasTag)
                    {
                        tag = ((HasTag) kidLabel).tag();
                    }
                    if (tag == null)
                    {
                        tag = kid.value();
                    }
                    if ("VBN".Equals(tag) || "VBG".Equals(tag) || "VBD".Equals(tag))
                    {
                        /*if (DEBUG) {
            System.err.println("vpContainsParticiple found VBN/VBG/VBD VP");
          }*/
                        return true;
                    }
                }
            }
            return false;
        }


        /** This looks to see whether any of the children is a preterminal headed by a word
   *  which is within the set verbalSet (which in practice is either
   *  auxiliary or copula verbs).  It only returns true if it's a preterminal head, since
   *  you don't want to pick things up in phrasal daughters.  That is an error.
   *
   * @param kids The child trees
   * @param verbalSet The set of words
   * @param allowTagOnlyMatch If true, it's sufficient to match on an unambiguous auxiliary tag.
   *                          Make true iff verbalSet is "all auxiliaries"
   * @return Returns true if one of the child trees is a preterminal verb headed
   *      by a word in verbalSet
   */

        private bool hasVerbalAuxiliary(Tree[] kids, Set<String> verbalSet, bool allowTagOnlyMatch)
        {
            /*if (DEBUG) {
      System.err.println("Checking for verbal auxiliary");
    }*/
            foreach (Tree kid in kids)
            {
                /*if (DEBUG) {
        System.err.println("  checking in " + kid);
      }*/
                if (isVerbalAuxiliary(kid, verbalSet, allowTagOnlyMatch))
                {
                    return true;
                }
            }
            /*if (DEBUG) {
      System.err.println("hasVerbalAuxiliary returns false");
    }*/
            return false;
        }


        private static readonly long serialVersionUID = 5721799188009249808L;
    }
}