using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// This interface specifies language/treebank specific information for a
    /// Treebank, which a parser or other treebank user might need to know.
    /// 
    /// Some of this is fixed for a (treebank,language) pair, but some of it
    /// reflects feature extraction decisions, so it can be sensible to have
    /// multiple implementations of this interface for the same (treebank,language) pair.
    /// 
    /// So far this covers punctuation, character encodings, and characters
    /// reserved for label annotations.  It should probably be expanded to
    /// cover other stuff (unknown words?).
    /// 
    /// Various methods in this class return arrays.  You should treat them
    /// as read-only, even though one cannot enforce that in Java.
    /// 
    /// Implementations in this class do not call basicCategory() on arguments
    /// before testing them, so if needed, you should explicitly call
    /// basicCategory() yourself before passing arguments to these routines for testing.
    /// 
    /// This class should be able to be an immutable singleton.  It contains
    /// data on various things, but no state.  At some point we should make it
    /// a real immutable singleton.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ITreebankLanguagePack
    {
        /// <summary>
        /// Accepts a string that is a punctuation tag name, and rejects everything else
        /// </summary>
        bool IsPunctuationTag(string str);

        /// <summary>
        /// Accepts a string that is a punctuation word, and rejects everything else.
        /// If one can't tell for sure (as for ' in the Penn Treebank), it
        /// maks the best guess that it can.
        /// </summary>
        bool IsPunctuationWord(string str);

        /// <summary>
        /// Accepts a string that is a sentence end
        /// punctuation tag, and rejects everything else.
        /// </summary>
        bool IsSentenceFinalPunctuationTag(string str);

        /// <summary>
        /// Accepts a string that is a punctuation
        /// tag that should be ignored by EVALB-style evaluation,
        /// and rejects everything else.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        bool IsEvalBIgnoredPunctuationTag(string str);

        /// <summary>
        /// Return a filter that accepts a string that is a punctuation
        /// tag name, and rejects everything else.
        /// </summary>
        Predicate<string> PunctuationTagAcceptFilter();

        /// <summary>
        /// Return a filter that rejects a string that is a punctuation
        /// tag name, and accepts everything else
        /// </summary>
        Predicate<string> PunctuationTagRejectFilter();

        /// <summary>
        /// Returns a filter that accepts a string that is a punctuation
        /// word, and rejects everything else.
        /// If one can't tell for sure (as for ' in the Penn Treebank), it
        /// maks the best guess that it can.
        /// </summary>
        Predicate<string> PunctuationWordAcceptFilter();

        /// <summary>
        /// Returns a filter that accepts a string that is not a punctuation
        /// word, and rejects punctuation.
        /// If one can't tell for sure (as for ' in the Penn Treebank), it
        /// makes the best guess that it can.
        /// </summary>
        Predicate<string> PunctuationWordRejectFilter();

        /// <summary>
        /// Returns a filter that accepts a string that is a sentence end
        /// punctuation tag, and rejects everything else
        /// </summary>
        Predicate<string> SentenceFinalPunctuationTagAcceptFilter();

        /// <summary>
        /// Returns a filter that accepts a string that is a punctuation
        /// tag that should be ignored by EVALB-style evaluation,
        /// and rejects everything else.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        Predicate<string> EvalBIgnoredPunctuationTagAcceptFilter();

        /// <summary>
        /// Returns a filter that accepts everything except a string that is a
        /// punctuation tag that should be ignored by EVALB-style evaluation.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        Predicate<string> EvalBIgnoredPunctuationTagRejectFilter();

        /// <summary>
        /// Returns a string array of punctuation tags for this treebank/language
        /// </summary>
        string[] PunctuationTags();

        /// <summary>
        /// Returns a string array of punctuation words for this treebank/language.
        /// </summary>
        string[] PunctuationWords();

        /// <summary>
        /// Returns a string array of sentence readonly punctuation tags for this
        /// treebank/language.  The first in the list is assumed to be the most basic one
        /// </summary>
        /// <returns>The sentence readonly punctuation tags</returns>
        string[] SentenceFinalPunctuationTags();

        /// <summary>
        /// Returns a string array of sentence readonly punctuation words for this treebank/language
        /// </summary>
        /// <returns>The punctuation words</returns>
        string[] SentenceFinalPunctuationWords();

        /// <summary>
        /// Returns a string array of punctuation tags that EVALB-style evaluation
        /// should ignore for this treebank/language.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        /// <returns>Whether this is a EVALB-ignored punctuation tag</returns>
        string[] EvalBIgnoredPunctuationTags();

        /// <summary>
        /// Return a GrammaticalStructureFactory suitable for this language/treebank
        /// </summary>
        IGrammaticalStructureFactory GrammaticalStructureFactory();

        /// <summary>
        /// Return a GrammaticalStructureFactory suitable for this language/treebank
        /// </summary>
        /// <param name="puncFilter">A filter which should reject punctuation words (as Strings)</param>
        IGrammaticalStructureFactory GrammaticalStructureFactory(Predicate<string> puncFilter);

        /// <summary>
        /// Return a GrammaticalStructureFactory suitable for this language/treebank
        /// </summary>
        /// <param name="puncFilter">A filter which should reject punctuation words (as Strings)</param>
        /// <param name="typedDependencyHf">A HeadFinder which finds heads for typed dependencies</param>
        /// <returns>A GrammaticalStructureFactory suitable for this language/treebank</returns>
        IGrammaticalStructureFactory GrammaticalStructureFactory(Predicate<string> puncFilter,
            IHeadFinder typedDependencyHf);

        /// <summary>
        ///  Whether or not we have typed dependencies for this language.
        /// If this method returns false, a call to grammaticalStructureFactory
        /// will cause an exception
        /// </summary>
        bool SupportsGrammaticalStructures();

        /// <summary>
        /// Return the charset encoding of the Treebank.
        /// See documentation for the <code>Charset</code> class
        /// </summary>
        string GetEncoding();

        /// <summary>
        /// Return a tokenizer factory which might be suitable for tokenizing text
        /// that will be used with this Treebank/Language pair.  This is for
        /// real text of this language pair, not for reading stuff inside the treebank files
        /// </summary>
        ITokenizerFactory<IHasWord> GetTokenizerFactory();

        /// <summary>
        /// Return an array of characters at which a string should be
        /// truncated to give the basic syntactic category of a label.
        /// The idea here is that Penn treebank style labels follow a syntactic
        /// category with various functional and crossreferencing information
        /// introduced by special characters (such as "NP-SBJ=1").  This would
        /// be truncated to "NP" by the array containing '-' and "=".
        /// 
        /// Note that these are never deleted as the first character as a label
        /// (so they are okay as one character tags, etc.), but only when subsequent characters
        /// </summary>
        /// <returns>An array of characters that set off label name suffixes</returns>
        char[] LabelAnnotationIntroducingCharacters();

        /// <summary>
        /// Say whether this character is an annotation introducing character
        /// </summary>
        bool IsLabelAnnotationIntroducingCharacter(char ch);

        /// <summary>
        /// Returns the basic syntactic category of a string by truncating
        /// stuff after a (non-word-initial) occurrence of one of the
        /// <code>labelAnnotationIntroducingCharacters()</code>.
        /// This function should work on phrasal category and POS tag labels,
        /// but needn't (and couldn't be expected to) work on arbitrary Word strings.
        /// </summary>
        /// <param name="category">The whole string name of the label</param>
        /// <returns>The basic category of the String</returns>
        string BasicCategory(string category);

        /// <summary>
        /// Returns the category for a string with everything following
        /// the gf character (which may be language specific) stripped
        /// </summary>
        /// <param name="category">The string name of the label (may previously have had basic category called on it)</param>
        /// <returns>The string stripped of grammatical functions</returns>
        string StripGf(string category);

        /// <summary>
        /// Returns a {@link Function Function} object that maps strings to strings according
        /// to this TreebankLanguagePack's basicCategory method
        /// </summary>
        AbstractTreebankLanguagePack.BasicCategoryStringFunction GetBasicCategoryFunction();

        /// <summary>
        /// Returns the syntactic category and 'function' of a string.
        /// This normally involves truncating numerical coindexation
        /// showing coreference, etc.  By 'function', this means
        /// keeping, say, Penn Treebank functional tags or ICE phrasal functions,
        /// perhaps returning them as <code>category-function</code>.
        /// </summary>
        /// <param name="category">The whole string name of the label</param>
        /// <returns>A string giving the category and function</returns>
        string CategoryAndFunction(string category);

        /// <summary>
        /// Returns a {@link Function Function} object that maps strings to strings according
        /// to this TreebankLanguagePack's categoryAndFunction method.
        /// </summary>
        AbstractTreebankLanguagePack.CategoryAndFunctionStringFunction GetCategoryAndFunctionFunction();

        /// <summary>
        /// Accepts a string that is a start symbol of the treebank
        /// </summary>
        bool IsStartSymbol(string str);

        /// <summary>
        /// Return a filter that accepts a string that is a start symbol
        /// of the treebank, and rejects everything else.
        /// </summary>
        Predicate<string> StartSymbolAcceptFilter();

        /// <summary>
        /// Returns a string array of treebank start symbols.
        /// </summary>
        string[] StartSymbols();

        /// <summary>
        /// Returns a string which is the first (perhaps unique) start symbol
        /// of the treebank, or null if none is defined.
        /// </summary>
        /// <returns>The start symbol</returns>
        string StartSymbol();

        /// <summary>
        /// Returns the extension of treebank files for this treebank.
        /// This should be passed as an argument to Treebank loading classes.
        /// It might be "mrg" or "fid" or whatever.  Don't include the period.
        /// </summary>
        string TreebankFileExtension();

        /// <summary>
        /// Sets the grammatical function indicating character to gfCharacter
        /// </summary>
        /// <param name="gfCharacter">
        /// Sets the character in label names that sets of
        /// grammatical function marking (from the phrase label)
        /// </param>
        void SetGfCharacter(char gfCharacter);

        /** Returns a TreeReaderFactory suitable for general purpose use
   *  with this language/treebank.
   *
   *  @return A TreeReaderFactory suitable for general purpose use
   *  with this language/treebank.
   */
        //TreeReaderFactory treeReaderFactory();

        /// <summary>
        /// Return a TokenizerFactory for Trees of this language/treebank
        /// </summary>
        ITokenizerFactory<Tree> TreeTokenizerFactory();

        /// <summary>
        /// The HeadFinder to use for your treebank
        /// </summary>
        IHeadFinder HeadFinder();

        /// <summary>
        /// The HeadFinder to use when making typed dependencies
        /// </summary>s
        IHeadFinder TypedDependencyHeadFinder();


        /**
   * The morphological feature specification for the language.
   *
   * @return A language-specific MorphoFeatureSpecification
   */
        /*abstract*/ //MorphoFeatureSpecification morphFeatureSpec();
    }
}