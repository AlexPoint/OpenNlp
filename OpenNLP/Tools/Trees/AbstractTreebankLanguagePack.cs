using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util;
using OpenNLP.Tools.Util.International.Morph;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// This provides an implementation of parts of the TreebankLanguagePack
    /// API to reduce the load on fresh implementations.  Only the abstract
    /// methods below need to be implemented to give a reasonable solution for a new language.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public abstract class AbstractTreebankLanguagePack : ITreebankLanguagePack
    {
        //Grammatical function parameters
        /**
        * Default character for indicating that something is a grammatical fn; probably should be overridden by
        * lang specific ones
        */
        protected char GfCharacter;
        protected static readonly char DefaultGfChar = '-';

        /// <summary>
        /// Use this as the default encoding for Readers and Writers of Treebank data.
        /// </summary>
        public static readonly string DefaultEncoding = "UTF-8";

        /// <summary>
        /// Gives a handle to the TreebankLanguagePack.
        /// </summary>
        public AbstractTreebankLanguagePack() : this(DefaultGfChar)
        {
        }

        /// <summary>
        /// Gives a handle to the TreebankLanguagePack
        /// </summary>
        /// <param name="gfChar">The character that sets of grammatical functions in node labels</param>
        public AbstractTreebankLanguagePack(char gfChar)
        {
            this.GfCharacter = gfChar;
        }

        /// <summary>
        /// Returns a string array of punctuation tags for this treebank/language.
        /// </summary>
        /// <returns>The punctuation tags</returns>
        public abstract string[] PunctuationTags();

        /// <summary>
        /// Returns a string array of punctuation words for this treebank/language.
        /// </summary>
        /// <returns>The punctuation words</returns>
        public abstract string[] PunctuationWords();

        /// <summary>
        /// Returns a string array of sentence readonly punctuation tags for this treebank/language.
        /// </summary>
        /// <returns>The sentence readonly punctuation tags</returns>
        public abstract string[] SentenceFinalPunctuationTags();

        public abstract string[] SentenceFinalPunctuationWords();

        /// <summary>
        /// Returns a string array of punctuation tags that EVALB-style evaluation
        /// should ignore for this treebank/language.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        /// <returns>Whether this is a EVALB-ignored punctuation tag</returns>
        public virtual string[] EvalBIgnoredPunctuationTags()
        {
            return PunctuationTags();
        }

        /// <summary>
        /// Accepts a string that is a punctuation
        /// tag name, and rejects everything else.
        /// </summary>
        /// <returns>Whether this is a punctuation tag</returns>
        public bool IsPunctuationTag(string str)
        {
            return PunctTagStringAcceptFilter()(str);
        }

        /// <summary>
        /// Accepts a string that is a punctuation
        /// word, and rejects everything else.
        /// If one can't tell for sure (as for ' in the Penn Treebank), it
        /// maks the best guess that it can.
        /// </summary>
        /// <returns>Whether this is a punctuation word</returns>
        public bool IsPunctuationWord(string str)
        {
            return PunctWordStringAcceptFilter()(str);
        }

        /// <summary>
        /// Accepts a string that is a sentence end
        /// punctuation tag, and rejects everything else.
        /// </summary>
        /// <returns>Whether this is a sentence readonly punctuation tag</returns>
        public bool IsSentenceFinalPunctuationTag(string str)
        {
            return SFPunctTagStringAcceptFilter()(str);
        }

        /// <summary>
        /// Accepts a string that is a punctuation
        /// tag that should be ignored by EVALB-style evaluation,
        /// and rejects everything else.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        /// <returns>Whether this is a EVALB-ignored punctuation tag</returns>
        public bool IsEvalBIgnoredPunctuationTag(string str)
        {
            return EIPunctTagStringAcceptFilter()(str);
        }

        /// <summary>
        /// Return a filter that accepts a string that is a punctuation
        /// tag name, and rejects everything else.
        /// </summary>
        public Predicate<string> PunctuationTagAcceptFilter()
        {
            return PunctTagStringAcceptFilter();
        }

        /// <summary>
        /// Return a filter that rejects a string that is a punctuation
        /// tag name, and rejects everything else.
        /// </summary>
        public Predicate<string> PunctuationTagRejectFilter()
        {
            return Filters.NotFilter(PunctTagStringAcceptFilter());
        }

        /// <summary>
        /// Returns a filter that accepts a string that is a punctuation
        /// word, and rejects everything else.
        /// If one can't tell for sure (as for ' in the Penn Treebank), it
        /// makes the best guess that it can.
        /// </summary>
        public Predicate<string> PunctuationWordAcceptFilter()
        {
            return PunctWordStringAcceptFilter();
        }

        /// <summary>
        /// Returns a filter that accepts a string that is not a punctuation
        /// word, and rejects punctuation.
        /// If one can't tell for sure (as for ' in the Penn Treebank), it
        /// makes the best guess that it can.
        /// </summary>
        public Predicate<string> PunctuationWordRejectFilter()
        {
            return Filters.NotFilter(PunctWordStringAcceptFilter());
        }

        /// <summary>
        /// Returns a filter that accepts a string that is a sentence end
        /// punctuation tag, and rejects everything else.
        /// </summary>
        public Predicate<string> SentenceFinalPunctuationTagAcceptFilter()
        {
            return SFPunctTagStringAcceptFilter();
        }

        /// <summary>
        /// Returns a filter that accepts a string that is a punctuation
        /// tag that should be ignored by EVALB-style evaluation,
        /// and rejects everything else.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        public Predicate<string> EvalBIgnoredPunctuationTagAcceptFilter()
        {
            return EIPunctTagStringAcceptFilter();
        }

        /// <summary>
        /// Returns a filter that accepts everything except a string that is a
        /// punctuation tag that should be ignored by EVALB-style evaluation.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        public Predicate<string> EvalBIgnoredPunctuationTagRejectFilter()
        {
            return Filters.NotFilter(EIPunctTagStringAcceptFilter());
        }

        /// <summary>
        /// Return the input Charset encoding for the Treebank.
        /// See documentation for the <code>Charset</code> class.
        /// </summary>
        public string GetEncoding()
        {
            return DefaultEncoding;
        }

        public abstract ITokenizerFactory<IHasWord> GetTokenizerFactory();


        private static readonly char[] EmptyCharArray = new char[0];

        /// <summary>
        /// Return an array of characters at which a string should be
        /// truncated to give the basic syntactic category of a label.
        /// The idea here is that Penn treebank style labels follow a syntactic
        /// category with various functional and crossreferencing information
        /// introduced by special characters (such as "NP-SBJ=1").  This would
        /// be truncated to "NP" by the array containing '-' and "=".
        /// </summary>
        /// <returns>An array of characters that set off label name suffixes</returns>
        public virtual char[] LabelAnnotationIntroducingCharacters()
        {
            return EmptyCharArray;
        }

        /// <summary>
        /// Returns the index of the first character that is after the basic
        /// label.  That is, if category is "NP-LGS", it returns 2.
        /// This routine assumes category != null.
        /// This routine returns 0 iff the string is of length 0.
        /// This routine always returns a number &lt;= category.length(), and
        /// so it is safe to pass it as an argument to category.substring().
        /// 
        /// NOTE: the routine should never allow the first character of a label
        /// to be taken as the annotation introducing character, because in the
        /// Penn Treebank, "-" is a valid tag, but also the character used to
        /// set off functional and co-indexing annotations. If the first letter is
        /// such a character then a matched character is also not used, for
        /// -LRB- etc., iff there is an intervening character (so --PU becomes -).
        /// </summary>
        /// <param name="category">Phrasal category</param>
        /// <returns>The index of the first character that is after the basic label</returns>
        private int PostBasicCategoryIndex(string category)
        {
            bool sawAtZero = false;
            char seenAtZero = '\u0000';
            int i = 0;
            for (int leng = category.Length; i < leng; i++)
            {
                char ch = category[i];
                if (IsLabelAnnotationIntroducingCharacter(ch))
                {
                    if (i == 0)
                    {
                        sawAtZero = true;
                        seenAtZero = ch;
                    }
                    else if (sawAtZero && i > 1 && ch == seenAtZero)
                    {
                        sawAtZero = false;
                    }
                    else
                    {
                        // still skip past identical ones for weird negra-penn "---CJ" (should we just delete it?)
                        // if (i + 1 < leng && category.charAt(i + 1) == ch) {
                        // keep looping
                        // } else {
                        break;
                        // }
                    }
                }
            }
            return i;
        }

        /// <summary>
        /// Returns the basic syntactic category of a string.
        /// This implementation basically truncates
        /// stuff after an occurrence of one of the
        /// <code>labelAnnotationIntroducingCharacters()</code>.
        /// However, there is also special case stuff to deal with
        /// labelAnnotationIntroducingCharacters in category labels:
        /// (i) if the first char is in this set, it's never truncated
        /// (e.g., '-' or '=' as a token), and (ii) if it starts with
        /// one of this set, a second instance of the same item from this set is
        /// also excluded (to deal with '-LLB-', '-RCB-', etc.).
        /// </summary>
        /// <param name="category">The whole string name of the label</param>
        /// <returns>The basic category of the String</returns>
        public string BasicCategory(string category)
        {
            if (category == null)
            {
                return null;
            }
            return category.Substring(0, PostBasicCategoryIndex(category));
        }

        public string StripGf(string category)
        {
            if (category == null)
            {
                return null;
            }
            int index = category.LastIndexOf(GfCharacter);
            if (index > 0)
            {
                category = category.Substring(0, index);
            }
            return category;
        }

        /// <summary>
        /// Returns a {@link Function Function} object that maps strings to strings according
        /// to this TreebankLanguagePack's basicCategory() method.
        /// </summary>
        /// <returns>The String->String Function object</returns>
        public BasicCategoryStringFunction GetBasicCategoryFunction()
        {
            return new BasicCategoryStringFunction(this);
        }


        public class BasicCategoryStringFunction /*:Func<string,String>, Serializables*/
        {
            private readonly AbstractTreebankLanguagePack tlp;

            public BasicCategoryStringFunction(AbstractTreebankLanguagePack tlp)
            {
                this.tlp = tlp;
            }

            public string Apply(string input)
            {
                return tlp.BasicCategory(input);
            }

        }


        public class CategoryAndFunctionStringFunction /*implements Function<string,String>, Serializable */
        {
            private readonly ITreebankLanguagePack tlp;

            public CategoryAndFunctionStringFunction(ITreebankLanguagePack tlp)
            {
                this.tlp = tlp;
            }

            public string Apply(string input)
            {
                return tlp.CategoryAndFunction(input);
            }

        }

        /// <summary>
        /// Returns the syntactic category and 'function' of a string.
        /// This normally involves truncating numerical coindexation
        /// showing coreference, etc.  By 'function', this means
        /// keeping, say, Penn Treebank functional tags or ICE phrasal functions,
        /// perhaps returning them as <code>category-function</code>.
        /// 
        /// This implementation strips numeric tags after label introducing
        /// characters (assuming that non-numeric things are functional tags).
        /// </summary>
        /// <param name="category">The whole string name of the label</param>
        /// <returns>A string giving the category and function</returns>
        public string CategoryAndFunction(string category)
        {
            if (category == null)
            {
                return null;
            }
            string catFunc = category;
            int i = LastIndexOfNumericTag(catFunc);
            while (i >= 0)
            {
                catFunc = catFunc.Substring(0, i);
                i = LastIndexOfNumericTag(catFunc);
            }
            return catFunc;
        }

        /// <summary>
        /// Returns the index within this string of the last occurrence of a
        /// isLabelAnnotationIntroducingCharacter which is followed by only
        /// digits, corresponding to a numeric tag at the end of the string.
        /// Example: <code>lastIndexOfNumericTag("NP-TMP-1") returns 6</code>.
        /// </summary>
        /// <param name="category">A string category</param>
        /// <returns>
        /// The index within this string of the last occurrence of a
        /// isLabelAnnotationIntroducingCharacter which is followed by only digits
        /// </returns>
        private int LastIndexOfNumericTag(string category)
        {
            if (category == null)
            {
                return -1;
            }
            int last = -1;
            for (int i = category.Length - 1; i >= 0; i--)
            {
                if (IsLabelAnnotationIntroducingCharacter(category[i]))
                {
                    bool onlyDigitsFollow = false;
                    for (int j = i + 1; j < category.Length; j++)
                    {
                        onlyDigitsFollow = true;
                        if (!(char.IsDigit(category[j])))
                        {
                            onlyDigitsFollow = false;
                            break;
                        }
                    }
                    if (onlyDigitsFollow)
                    {
                        last = i;
                    }
                }
            }
            return last;
        }

        /// <summary>
        /// Returns a {@link Function Function} object that maps strings to strings according
        /// to this TreebankLanguagePack's categoryAndFunction() method.
        /// </summary>
        /// <returns>The String->String Function object</returns>
        public CategoryAndFunctionStringFunction GetCategoryAndFunctionFunction()
        {
            return new CategoryAndFunctionStringFunction(this);
        }

        /// <summary>
        /// Say whether this character is an annotation introducing character.
        /// </summary>
        /// <param name="ch">The character to check</param>
        /// <returns>Whether it is an annotation introducing character</returns>
        public bool IsLabelAnnotationIntroducingCharacter(char ch)
        {
            char[] cutChars = LabelAnnotationIntroducingCharacters();
            foreach (char cutChar in cutChars)
            {
                if (ch == cutChar)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Accepts a string that is a start symbol of the treebank.
        /// </summary>
        /// <returns>Whether this is a start symbol</returns>
        public bool IsStartSymbol(string str)
        {
            return StartSymbolAcceptFilter()(str);
        }


        /**
        * Return a filter that accepts a string that is a start symbol
        * of the treebank, and rejects everything else.
        *
        * @return The filter
        */
        /*public Predicate<string> startSymbolAcceptFilter() {
            return startSymbolAcceptFilter;
        }*/

        /// <summary>
        /// Returns a string array of treebank start symbols.
        /// </summary>
        public abstract string[] StartSymbols();

        /// <summary>
        /// Returns a string which is the first (perhaps unique) start symbol
        /// of the treebank, or null if none is defined.
        /// </summary>
        public string StartSymbol()
        {
            string[] ssyms = StartSymbols();
            if (ssyms == null || ssyms.Length == 0)
            {
                return null;
            }
            return ssyms[0];
        }

        public abstract string TreebankFileExtension();

        private Predicate<string> PunctTagStringAcceptFilter()
        {
            return Filters.CollectionAcceptFilter(PunctuationTags());
        }

        private Predicate<string> PunctWordStringAcceptFilter()
        {
            return Filters.CollectionAcceptFilter(PunctuationWords());
        }

        private Predicate<string> SFPunctTagStringAcceptFilter()
        {
            return Filters.CollectionAcceptFilter(SentenceFinalPunctuationTags());
        }

        private Predicate<string> EIPunctTagStringAcceptFilter()
        {
            return Filters.CollectionAcceptFilter(EvalBIgnoredPunctuationTags());
        }

        public Predicate<string> StartSymbolAcceptFilter()
        {
            return Filters.CollectionAcceptFilter(StartSymbols());
        }

        /**
        * Return a tokenizer which might be suitable for tokenizing text that
        * will be used with this Treebank/Language pair, without tokenizing carriage returns (i.e., treating them as white space).  The implementation in AbstractTreebankLanguagePack
        * returns a factory for {@link WhitespaceTokenizer}.
        *
        * @return A tokenizer
        */
        /*public TokenizerFactory<HasWord> getTokenizerFactory() {
            return WhitespaceTokenizer.factory(false);
        }*/

        /// <summary>
        /// Return a GrammaticalStructureFactory suitable for this language/treebank.
        /// (To be overridden in subclasses.)
        /// </summary>
        /// <returns>A GrammaticalStructureFactory suitable for this language/treebank</returns>
        public virtual IGrammaticalStructureFactory GrammaticalStructureFactory()
        {
            throw new InvalidOperationException("No GrammaticalStructureFactory defined for " /*+ getClass().getName()*/);
        }

        /// <summary>
        /// Return a GrammaticalStructureFactory suitable for this language/treebank.
        /// (To be overridden in subclasses.)
        /// </summary>
        /// <returns>A GrammaticalStructureFactory suitable for this language/treebank</returns>
        public IGrammaticalStructureFactory GrammaticalStructureFactory(Predicate<string> puncFilt)
        {
            return GrammaticalStructureFactory();
        }

        /// <summary>
        /// Return a GrammaticalStructureFactory suitable for this language/treebank.
        /// (To be overridden in subclasses.)
        /// </summary>
        /// <returns>A GrammaticalStructureFactory suitable for this language/treebank</returns>
        public IGrammaticalStructureFactory GrammaticalStructureFactory(Predicate<string> puncFilt,
            IHeadFinder typedDependencyHf)
        {
            return GrammaticalStructureFactory();
        }

        public virtual bool SupportsGrammaticalStructures()
        {
            return false;
        }

        public char GetGfCharacter()
        {
            return GfCharacter;
        }

        public void SetGfCharacter(char gfCharacter)
        {
            this.GfCharacter = gfCharacter;
        }

        public ITreeReaderFactory TreeReaderFactory()
        {
            return new PennTreeReaderFactory();
        }

        public ITokenizerFactory<Tree> TreeTokenizerFactory()
        {
            return new TreeTokenizerFactory(TreeReaderFactory());
        }

        public abstract IHeadFinder HeadFinder();
        public abstract IHeadFinder TypedDependencyHeadFinder();

        /// <summary>
        /// Returns a morphological feature specification for words in this language.
        /// </summary>
        public MorphoFeatureSpecification MorphFeatureSpec()
        {
            return null;
        }
    }
}