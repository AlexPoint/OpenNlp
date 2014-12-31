using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.International.Morph;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Util.Trees
{
    public abstract class AbstractTreebankLanguagePack: TreebankLanguagePack
    {
        /**
   * So changed versions deserialize correctly.
   */
  //private static readonly long serialVersionUID = -6506749780512708352L;


  //Grammatical function parameters
  /**
   * Default character for indicating that something is a grammatical fn; probably should be overridden by
   * lang specific ones
   */
  protected char gfCharacter;
  protected static readonly char DEFAULT_GF_CHAR = '-';


  /**
   * Use this as the default encoding for Readers and Writers of
   * Treebank data.
   */
  public static readonly String DEFAULT_ENCODING = "UTF-8";


  /**
   * Gives a handle to the TreebankLanguagePack.
   */
  public AbstractTreebankLanguagePack():this(DEFAULT_GF_CHAR){}


  /**
   * Gives a handle to the TreebankLanguagePack.
   *
   * @param gfChar The character that sets of grammatical functions in node labels.
   */
  public AbstractTreebankLanguagePack(char gfChar) {
    this.gfCharacter = gfChar;
  }

  /**
   * Returns a String array of punctuation tags for this treebank/language.
   *
   * @return The punctuation tags
   */
  //@Override
  public abstract String[] punctuationTags();

  /**
   * Returns a String array of punctuation words for this treebank/language.
   *
   * @return The punctuation words
   */
  //@Override
  public abstract String[] punctuationWords();

  /**
   * Returns a String array of sentence readonly punctuation tags for this
   * treebank/language.
   *
   * @return The sentence readonly punctuation tags
   */
  //@Override
  public abstract String[] sentenceFinalPunctuationTags();
    
        public abstract string[] sentenceFinalPunctuationWords();

        /**
   * Returns a String array of punctuation tags that EVALB-style evaluation
   * should ignore for this treebank/language.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return Whether this is a EVALB-ignored punctuation tag
   */
  //@Override
  public virtual String[] evalBIgnoredPunctuationTags() {
    return punctuationTags();
  }


  /**
   * Accepts a String that is a punctuation
   * tag name, and rejects everything else.
   *
   * @return Whether this is a punctuation tag
   */
  //@Override
  public bool isPunctuationTag(String str) {
    return punctTagStringAcceptFilter()(str);
  }


  /**
   * Accepts a String that is a punctuation
   * word, and rejects everything else.
   * If one can't tell for sure (as for ' in the Penn Treebank), it
   * maks the best guess that it can.
   *
   * @return Whether this is a punctuation word
   */
  //@Override
  public bool isPunctuationWord(String str) {
    return punctWordStringAcceptFilter()(str);
  }


  /**
   * Accepts a String that is a sentence end
   * punctuation tag, and rejects everything else.
   *
   * @return Whether this is a sentence readonly punctuation tag
   */
  //@Override
  public bool isSentenceFinalPunctuationTag(String str) {
    return sFPunctTagStringAcceptFilter()(str);
  }


  /**
   * Accepts a String that is a punctuation
   * tag that should be ignored by EVALB-style evaluation,
   * and rejects everything else.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return Whether this is a EVALB-ignored punctuation tag
   */
  //@Override
  public bool isEvalBIgnoredPunctuationTag(String str) {
    return eIPunctTagStringAcceptFilter()(str);
  }


  /**
   * Return a filter that accepts a String that is a punctuation
   * tag name, and rejects everything else.
   *
   * @return The filter
   */
  //@Override
  public Predicate<String> punctuationTagAcceptFilter() {
    return punctTagStringAcceptFilter();
  }


  /**
   * Return a filter that rejects a String that is a punctuation
   * tag name, and rejects everything else.
   *
   * @return The filter
   */
  //@Override
  public Predicate<String> punctuationTagRejectFilter() {
    return Filters.notFilter(punctTagStringAcceptFilter());
  }


  /**
   * Returns a filter that accepts a String that is a punctuation
   * word, and rejects everything else.
   * If one can't tell for sure (as for ' in the Penn Treebank), it
   * makes the best guess that it can.
   *
   * @return The Filter
   */
  //@Override
  public Predicate<String> punctuationWordAcceptFilter() {
    return punctWordStringAcceptFilter();
  }


  /**
   * Returns a filter that accepts a String that is not a punctuation
   * word, and rejects punctuation.
   * If one can't tell for sure (as for ' in the Penn Treebank), it
   * makes the best guess that it can.
   *
   * @return The Filter
   */
  //@Override
  public Predicate<String> punctuationWordRejectFilter() {
    return Filters.notFilter(punctWordStringAcceptFilter());
  }


  /**
   * Returns a filter that accepts a String that is a sentence end
   * punctuation tag, and rejects everything else.
   *
   * @return The Filter
   */
  //@Override
  public Predicate<String> sentenceFinalPunctuationTagAcceptFilter() {
    return sFPunctTagStringAcceptFilter();
  }


  /**
   * Returns a filter that accepts a String that is a punctuation
   * tag that should be ignored by EVALB-style evaluation,
   * and rejects everything else.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return The Filter
   */
  //@Override
  public Predicate<String> evalBIgnoredPunctuationTagAcceptFilter() {
    return eIPunctTagStringAcceptFilter();
  }


  /**
   * Returns a filter that accepts everything except a String that is a
   * punctuation tag that should be ignored by EVALB-style evaluation.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return The Filter
   */
  //@Override
  public Predicate<String> evalBIgnoredPunctuationTagRejectFilter() {
    return Filters.notFilter(eIPunctTagStringAcceptFilter());
  }


  /**
   * Return the input Charset encoding for the Treebank.
   * See documentation for the <code>Charset</code> class.
   *
   * @return Name of Charset
   */
  //@Override
  public String getEncoding() {
    return DEFAULT_ENCODING;
  }

        public abstract TokenizerFactory<HasWord> getTokenizerFactory();


        private static readonly char[] EMPTY_CHAR_ARRAY = new char[0];

  /**
   * Return an array of characters at which a String should be
   * truncated to give the basic syntactic category of a label.
   * The idea here is that Penn treebank style labels follow a syntactic
   * category with various functional and crossreferencing information
   * introduced by special characters (such as "NP-SBJ=1").  This would
   * be truncated to "NP" by the array containing '-' and "=".
   *
   * @return An array of characters that set off label name suffixes
   */
  //@Override
  public virtual char[] labelAnnotationIntroducingCharacters() {
    return EMPTY_CHAR_ARRAY;
  }


  /**
   * Returns the index of the first character that is after the basic
   * label.  That is, if category is "NP-LGS", it returns 2.
   * This routine assumes category != null.
   * This routine returns 0 iff the String is of length 0.
   * This routine always returns a number &lt;= category.length(), and
   * so it is safe to pass it as an argument to category.substring().
   * <p>
   * NOTE: the routine should never allow the first character of a label
   * to be taken as the annotation introducing character, because in the
   * Penn Treebank, "-" is a valid tag, but also the character used to
   * set off functional and co-indexing annotations. If the first letter is
   * such a character then a matched character is also not used, for
   * -LRB- etc., iff there is an intervening character (so --PU becomes -).
   *
   * @param category Phrasal category
   * @return The index of the first character that is after the basic
   *     label
   */
  private int postBasicCategoryIndex(String category) {
    bool sawAtZero = false;
    char seenAtZero = '\u0000';
    int i = 0;
    for (int leng = category.Length; i < leng; i++) {
      char ch = category[i];
      if (isLabelAnnotationIntroducingCharacter(ch)) {
        if (i == 0) {
          sawAtZero = true;
          seenAtZero = ch;
        } else if (sawAtZero && i > 1 && ch == seenAtZero) {
          sawAtZero = false;
        } else {
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

  /**
   * Returns the basic syntactic category of a String.
   * This implementation basically truncates
   * stuff after an occurrence of one of the
   * <code>labelAnnotationIntroducingCharacters()</code>.
   * However, there is also special case stuff to deal with
   * labelAnnotationIntroducingCharacters in category labels:
   * (i) if the first char is in this set, it's never truncated
   * (e.g., '-' or '=' as a token), and (ii) if it starts with
   * one of this set, a second instance of the same item from this set is
   * also excluded (to deal with '-LLB-', '-RCB-', etc.).
   *
   * @param category The whole String name of the label
   * @return The basic category of the String
   */
  //@Override
  public String basicCategory(String category) {
    if (category == null) {
      return null;
    }
    return category.Substring(0, postBasicCategoryIndex(category));
  }


  //@Override
  public String stripGF(String category) {
    if(category == null) {
      return null;
    }
    int index = category.LastIndexOf(gfCharacter);
    if(index > 0) {
      category = category.Substring(0, index);
    }
    return category;
  }

  /**
   * Returns a {@link Function Function} object that maps Strings to Strings according
   * to this TreebankLanguagePack's basicCategory() method.
   *
   * @return The String->String Function object
   */
  //@Override
  public BasicCategoryStringFunction getBasicCategoryFunction()
  {
    return new BasicCategoryStringFunction(this);
  }


  public class BasicCategoryStringFunction /*:Func<String,String>, Serializables*/ {

    private static readonly long serialVersionUID = 1L;

    private AbstractTreebankLanguagePack tlp;

    public BasicCategoryStringFunction(AbstractTreebankLanguagePack tlp) {
      this.tlp = tlp;
    }

    //@Override
    public String apply(String input) {
      return tlp.basicCategory(input);
    }

  }


  public /*static*/ class CategoryAndFunctionStringFunction /*implements Function<String,String>, Serializable */{

    private static readonly long serialVersionUID = 1L;

    private TreebankLanguagePack tlp;

    public CategoryAndFunctionStringFunction(TreebankLanguagePack tlp) {
      this.tlp = tlp;
    }

    //@Override
    public String apply(String input) {
      return tlp.categoryAndFunction(input);
    }

  }


  /**
   * Returns the syntactic category and 'function' of a String.
   * This normally involves truncating numerical coindexation
   * showing coreference, etc.  By 'function', this means
   * keeping, say, Penn Treebank functional tags or ICE phrasal functions,
   * perhaps returning them as <code>category-function</code>.
   * <p/>
   * This implementation strips numeric tags after label introducing
   * characters (assuming that non-numeric things are functional tags).
   *
   * @param category The whole String name of the label
   * @return A String giving the category and function
   */
  //@Override
  public String categoryAndFunction(String category) {
    if (category == null) {
      return null;
    }
    String catFunc = category;
    int i = lastIndexOfNumericTag(catFunc);
    while (i >= 0) {
      catFunc = catFunc.Substring(0, i);
      i = lastIndexOfNumericTag(catFunc);
    }
    return catFunc;
  }

  /**
   * Returns the index within this string of the last occurrence of a
   * isLabelAnnotationIntroducingCharacter which is followed by only
   * digits, corresponding to a numeric tag at the end of the string.
   * Example: <code>lastIndexOfNumericTag("NP-TMP-1") returns
   * 6</code>.
   *
   * @param category A String category
   * @return The index within this string of the last occurrence of a
   *     isLabelAnnotationIntroducingCharacter which is followed by only
   *     digits
   */
  private int lastIndexOfNumericTag(String category) {
    if (category == null) {
      return -1;
    }
    int last = -1;
    for (int i = category.Length - 1; i >= 0; i--) {
      if (isLabelAnnotationIntroducingCharacter(category[i])) {
        bool onlyDigitsFollow = false;
        for (int j = i + 1; j < category.Length; j++) {
          onlyDigitsFollow = true;
          if (!(char.IsDigit(category[j]))) {
            onlyDigitsFollow = false;
            break;
          }
        }
        if (onlyDigitsFollow) {
          last = i;
        }
      }
    }
    return last;
  }

  /**
   * Returns a {@link Function Function} object that maps Strings to Strings according
   * to this TreebankLanguagePack's categoryAndFunction() method.
   *
   * @return The String->String Function object
   */
  //@Override
  public CategoryAndFunctionStringFunction getCategoryAndFunctionFunction()
  {
    return new CategoryAndFunctionStringFunction(this);
  }


  /**
   * Say whether this character is an annotation introducing
   * character.
   *
   * @param ch The character to check
   * @return Whether it is an annotation introducing character
   */
  //@Override
  public bool isLabelAnnotationIntroducingCharacter(char ch) {
    char[] cutChars = labelAnnotationIntroducingCharacters();
    foreach (char cutChar in cutChars) {
      if (ch == cutChar) {
        return true;
      }
    }
    return false;
  }


  /**
   * Accepts a String that is a start symbol of the treebank.
   *
   * @return Whether this is a start symbol
   */
  //@Override
  public bool isStartSymbol(String str) {
    return startSymbolAcceptFilter()(str);
  }


  /**
   * Return a filter that accepts a String that is a start symbol
   * of the treebank, and rejects everything else.
   *
   * @return The filter
   */
  //@Override
  /*public Predicate<String> startSymbolAcceptFilter() {
    return startSymbolAcceptFilter;
  }*/


  /**
   * Returns a String array of treebank start symbols.
   *
   * @return The start symbols
   */
  //@Override
  public abstract String[] startSymbols();


  /**
   * Returns a String which is the first (perhaps unique) start symbol
   * of the treebank, or null if none is defined.
   *
   * @return The start symbol
   */
  //@Override
  public String startSymbol() {
    String[] ssyms = startSymbols();
    if (ssyms == null || ssyms.Length == 0) {
      return null;
    }
    return ssyms[0];
  }

        public abstract string treebankFileExtension();

        private Predicate<string> punctTagStringAcceptFilter()
        {
            return Filters.collectionAcceptFilter(punctuationTags());
        }

        private Predicate<String> punctWordStringAcceptFilter()
        {
            return Filters.collectionAcceptFilter(punctuationWords());
        }

        private Predicate<String> sFPunctTagStringAcceptFilter()
        {
            return Filters.collectionAcceptFilter(sentenceFinalPunctuationTags());
        }

        private Predicate<String> eIPunctTagStringAcceptFilter()
        {
            return Filters.collectionAcceptFilter(evalBIgnoredPunctuationTags());
        }

        public Predicate<String> startSymbolAcceptFilter()
        {
            return Filters.collectionAcceptFilter(startSymbols());
        }

  /**
   * Return a tokenizer which might be suitable for tokenizing text that
   * will be used with this Treebank/Language pair, without tokenizing carriage returns (i.e., treating them as white space).  The implementation in AbstractTreebankLanguagePack
   * returns a factory for {@link WhitespaceTokenizer}.
   *
   * @return A tokenizer
   */
  //@Override
  /*public TokenizerFactory<HasWord> getTokenizerFactory() {
    return WhitespaceTokenizer.factory(false);
  }*/

  /**
   * Return a GrammaticalStructureFactory suitable for this language/treebank.
   * (To be overridden in subclasses.)
   *
   * @return A GrammaticalStructureFactory suitable for this language/treebank
   */
  //@Override
  public virtual GrammaticalStructureFactory grammaticalStructureFactory() {
    throw new InvalidOperationException("No GrammaticalStructureFactory defined for " /*+ getClass().getName()*/);
  }

  /**
   * Return a GrammaticalStructureFactory suitable for this language/treebank.
   * (To be overridden in subclasses.)
   *
   * @return A GrammaticalStructureFactory suitable for this language/treebank
   */
  //@Override
  public GrammaticalStructureFactory grammaticalStructureFactory(Predicate<String> puncFilt) {
    return grammaticalStructureFactory();
  }

  /**
   * Return a GrammaticalStructureFactory suitable for this language/treebank.
   * (To be overridden in subclasses.)
   *
   * @return A GrammaticalStructureFactory suitable for this language/treebank
   */
  //@Override
  public GrammaticalStructureFactory grammaticalStructureFactory(Predicate<String> puncFilt, HeadFinder typedDependencyHeadFinder) {
    return grammaticalStructureFactory();
  }

  //@Override
  public virtual bool supportsGrammaticalStructures() {
    return false;
  }

  public char getGfCharacter() {
    return gfCharacter;
  }


  //@Override
  public void setGfCharacter(char gfCharacter) {
    this.gfCharacter = gfCharacter;
  }

  /** {@inheritDoc} */
  //@Override
  public TreeReaderFactory treeReaderFactory() {
    return new PennTreeReaderFactory();
  }

  /** {@inheritDoc} */
  //@Override
  public TokenizerFactory<Tree> treeTokenizerFactory() {
    return new TreeTokenizerFactory(treeReaderFactory());
  }

        public abstract HeadFinder headFinder();
        public abstract HeadFinder typedDependencyHeadFinder();

        /**
   * Returns a morphological feature specification for words in this language.
   */
  //@Override
  public MorphoFeatureSpecification morphFeatureSpec() {
    return null;
  }
    }
}
