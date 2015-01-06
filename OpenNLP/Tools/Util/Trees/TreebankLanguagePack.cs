using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OpenNLP.Tools.Util.International.Morph;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * This interface specifies language/treebank specific information for a
 * Treebank, which a parser or other treebank user might need to know. <p>
 * <p/>
 * Some of this is fixed for a (treebank,language) pair, but some of it
 * reflects feature extraction decisions, so it can be sensible to have
 * multiple implementations of this interface for the same
 * (treebank,language) pair. <p>
 * <p/>
 * So far this covers punctuation, character encodings, and characters
 * reserved for label annotations.  It should probably be expanded to
 * cover other stuff (unknown words?). <p>
 * <p/>
 * Various methods in this class return arrays.  You should treat them
 * as read-only, even though one cannot enforce that in Java. <p>
 * <p/>
 * Implementations in this class do not call basicCategory() on arguments
 * before testing them, so if needed, you should explicitly call
 * basicCategory() yourself before passing arguments to these routines for
 * testing.
 * <p/>
 * This class should be able to be an immutable singleton.  It contains
 * data on various things, but no state.  At some point we should make it
 * a real immutable singleton.
 *
 * @author Christopher Manning
 * @version 1.1, Mar 2003
 */

    public interface TreebankLanguagePack
    {
        /**
   * Use this as the default encoding for Readers and Writers of
   * Treebank data.
   */
        //static readonly string DEFAULT_ENCODING = "UTF-8";


        /**
   * Accepts a string that is a punctuation
   * tag name, and rejects everything else.
   *
   * @param str The string to check
   * @return Whether this is a punctuation tag
   */
        bool IsPunctuationTag(string str);


        /**
   * Accepts a string that is a punctuation
   * word, and rejects everything else.
   * If one can't tell for sure (as for ' in the Penn Treebank), it
   * maks the best guess that it can.
   *
   * @param str The string to check
   * @return Whether this is a punctuation word
   */
        bool IsPunctuationWord(string str);


        /**
   * Accepts a string that is a sentence end
   * punctuation tag, and rejects everything else.
   *
   * @param str The string to check
   * @return Whether this is a sentence readonly punctuation tag
   */
        bool IsSentenceFinalPunctuationTag(string str);


        /**
   * Accepts a string that is a punctuation
   * tag that should be ignored by EVALB-style evaluation,
   * and rejects everything else.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @param str The string to check
   * @return Whether this is a EVALB-ignored punctuation tag
   */
        bool IsEvalBIgnoredPunctuationTag(string str);


        /**
   * Return a filter that accepts a string that is a punctuation
   * tag name, and rejects everything else.
   *
   * @return The filter
   */
        Predicate<string> PunctuationTagAcceptFilter();


        /**
   * Return a filter that rejects a string that is a punctuation
   * tag name, and accepts everything else.
   *
   * @return The filter
   */
        Predicate<string> PunctuationTagRejectFilter();

        /**
   * Returns a filter that accepts a string that is a punctuation
   * word, and rejects everything else.
   * If one can't tell for sure (as for ' in the Penn Treebank), it
   * maks the best guess that it can.
   *
   * @return The Filter
   */
        Predicate<string> PunctuationWordAcceptFilter();


        /**
   * Returns a filter that accepts a string that is not a punctuation
   * word, and rejects punctuation.
   * If one can't tell for sure (as for ' in the Penn Treebank), it
   * makes the best guess that it can.
   *
   * @return The Filter
   */
        Predicate<string> PunctuationWordRejectFilter();


        /**
   * Returns a filter that accepts a string that is a sentence end
   * punctuation tag, and rejects everything else.
   *
   * @return The Filter
   */
        Predicate<string> SentenceFinalPunctuationTagAcceptFilter();


        /**
   * Returns a filter that accepts a string that is a punctuation
   * tag that should be ignored by EVALB-style evaluation,
   * and rejects everything else.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return The Filter
   */
        Predicate<string> EvalBIgnoredPunctuationTagAcceptFilter();


        /**
   * Returns a filter that accepts everything except a string that is a
   * punctuation tag that should be ignored by EVALB-style evaluation.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return The Filter
   */
        Predicate<string> EvalBIgnoredPunctuationTagRejectFilter();


        /**
   * Returns a string array of punctuation tags for this treebank/language.
   *
   * @return The punctuation tags
   */
        string[] PunctuationTags();


        /**
   * Returns a string array of punctuation words for this treebank/language.
   *
   * @return The punctuation words
   */
        string[] PunctuationWords();


        /**
   * Returns a string array of sentence readonly punctuation tags for this
   * treebank/language.  The first in the list is assumed to be the most
   * basic one.
   *
   * @return The sentence readonly punctuation tags
   */
        string[] SentenceFinalPunctuationTags();


        /**
   * Returns a string array of sentence readonly punctuation words for
   * this treebank/language.
   *
   * @return The punctuation words
   */
        string[] SentenceFinalPunctuationWords();

        /**
   * Returns a string array of punctuation tags that EVALB-style evaluation
   * should ignore for this treebank/language.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return Whether this is a EVALB-ignored punctuation tag
   */
        string[] EvalBIgnoredPunctuationTags();


        /**
   * Return a GrammaticalStructureFactory suitable for this language/treebank.
   *
   * @return A GrammaticalStructureFactory suitable for this language/treebank
   */
        GrammaticalStructureFactory GrammaticalStructureFactory();


        /**
   * Return a GrammaticalStructureFactory suitable for this language/treebank.
   *
   * @param puncFilter A filter which should reject punctuation words (as Strings)
   * @return A GrammaticalStructureFactory suitable for this language/treebank
   */
        GrammaticalStructureFactory GrammaticalStructureFactory(Predicate<string> puncFilter);


        /**
   * Return a GrammaticalStructureFactory suitable for this language/treebank.
   *
   * @param puncFilter A filter which should reject punctuation words (as Strings)
   * @param typedDependencyHF A HeadFinder which finds heads for typed dependencies
   * @return A GrammaticalStructureFactory suitable for this language/treebank
   */

        GrammaticalStructureFactory GrammaticalStructureFactory(Predicate<string> puncFilter,
            HeadFinder typedDependencyHF);

        /**
   * Whether or not we have typed dependencies for this language.  If
   * this method returns false, a call to grammaticalStructureFactory
   * will cause an exception.
   */
        bool SupportsGrammaticalStructures();

        /**
   * Return the charset encoding of the Treebank.  See
   * documentation for the <code>Charset</code> class.
   *
   * @return Name of Charset
   */
        string GetEncoding();


        /**
   * Return a tokenizer factory which might be suitable for tokenizing text
   * that will be used with this Treebank/Language pair.  This is for
   * real text of this language pair, not for reading stuff inside the
   * treebank files.
   *
   * @return A tokenizer
   */
        TokenizerFactory<HasWord> GetTokenizerFactory();

        /**
   * Return an array of characters at which a string should be
   * truncated to give the basic syntactic category of a label.
   * The idea here is that Penn treebank style labels follow a syntactic
   * category with various functional and crossreferencing information
   * introduced by special characters (such as "NP-SBJ=1").  This would
   * be truncated to "NP" by the array containing '-' and "=". <br>
   * Note that these are never deleted as the first character as a label
   * (so they are okay as one character tags, etc.), but only when
   * subsequent characters.
   *
   * @return An array of characters that set off label name suffixes
   */
        char[] LabelAnnotationIntroducingCharacters();


        /**
   * Say whether this character is an annotation introducing
   * character.
   *
   * @param ch A char
   * @return Whether this char introduces functional annotations
   */
        bool IsLabelAnnotationIntroducingCharacter(char ch);


        /**
   * Returns the basic syntactic category of a string by truncating
   * stuff after a (non-word-initial) occurrence of one of the
   * <code>labelAnnotationIntroducingCharacters()</code>.  This
   * function should work on phrasal category and POS tag labels,
   * but needn't (and couldn't be expected to) work on arbitrary
   * Word strings.
   *
   * @param category The whole string name of the label
   * @return The basic category of the String
   */
        string BasicCategory(string category);

        /**
   * Returns the category for a string with everything following
   * the gf character (which may be language specific) stripped.
   *
   * @param category The string name of the label (may previously have had basic category called on it)
   * @return The string stripped of grammatical functions
   */
        string StripGf(string category);


        /**
   * Returns a {@link Function Function} object that maps Strings to Strings according
   * to this TreebankLanguagePack's basicCategory method.
   *
   * @return the String->String Function object
   */
        /*Func<string,String>*/
        AbstractTreebankLanguagePack.BasicCategoryStringFunction GetBasicCategoryFunction();

        /**
   * Returns the syntactic category and 'function' of a string.
   * This normally involves truncating numerical coindexation
   * showing coreference, etc.  By 'function', this means
   * keeping, say, Penn Treebank functional tags or ICE phrasal functions,
   * perhaps returning them as <code>category-function</code>.
   *
   * @param category The whole string name of the label
   * @return A string giving the category and function
   */
        string CategoryAndFunction(string category);

        /**
   * Returns a {@link Function Function} object that maps Strings to Strings according
   * to this TreebankLanguagePack's categoryAndFunction method.
   *
   * @return the String->String Function object
   */
        /*Func<string,String>*/
        AbstractTreebankLanguagePack.CategoryAndFunctionStringFunction GetCategoryAndFunctionFunction();

        /**
   * Accepts a string that is a start symbol of the treebank.
   *
   * @param str The str to test
   * @return Whether this is a start symbol
   */
        bool IsStartSymbol(string str);


        /**
   * Return a filter that accepts a string that is a start symbol
   * of the treebank, and rejects everything else.
   *
   * @return The filter
   */
        Predicate<string> StartSymbolAcceptFilter();


        /**
   * Returns a string array of treebank start symbols.
   *
   * @return The start symbols
   */
        string[] StartSymbols();

        /**
   * Returns a string which is the first (perhaps unique) start symbol
   * of the treebank, or null if none is defined.
   *
   * @return The start symbol
   */
        string StartSymbol();


        /**
   * Returns the extension of treebank files for this treebank.
   * This should be passed as an argument to Treebank loading classes.
   * It might be "mrg" or "fid" or whatever.  Don't include the period.
   *
   * @return the extension on files for this treebank
   */
        string TreebankFileExtension();

        /**
   * Sets the grammatical function indicating character to gfCharacter.
   *
   * @param gfCharacter Sets the character in label names that sets of
   *         grammatical function marking (from the phrase label).
   */
        void SetGfCharacter(char gfCharacter);

        /** Returns a TreeReaderFactory suitable for general purpose use
   *  with this language/treebank.
   *
   *  @return A TreeReaderFactory suitable for general purpose use
   *  with this language/treebank.
   */
        //TreeReaderFactory treeReaderFactory();

        /** Return a TokenizerFactory for Trees of this language/treebank.
   *
   * @return A TokenizerFactory for Trees of this language/treebank.
   */
        TokenizerFactory<Tree> TreeTokenizerFactory();

        /**
   * The HeadFinder to use for your treebank.
   *
   * @return A suitable HeadFinder
   */
        /*abstract*/
        HeadFinder HeadFinder();


        /**
   * The HeadFinder to use when making typed dependencies.
   *
   * @return A suitable HeadFinder
   */
        /*abstract*/
        HeadFinder TypedDependencyHeadFinder();


        /**
   * The morphological feature specification for the language.
   *
   * @return A language-specific MorphoFeatureSpecification
   */
        /*abstract*/ //MorphoFeatureSpecification morphFeatureSpec();
    }
}