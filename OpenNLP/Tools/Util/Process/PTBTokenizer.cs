using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Process
{
    /**
 * A fast, rule-based tokenizer implementation, which produces Penn Treebank
 * style tokenization of English text. It was initially written to conform
 * to Penn Treebank tokenization conventions over ASCII text, but now provides
 * a range of tokenization options over a broader space of Unicode text.
 * It reads raw text and outputs
 * tokens of classes that implement edu.stanford.nlp.trees.HasWord
 * (typically a Word or a CoreLabel). It can
 * optionally return end-of-line as a token.
 * <p>
 * New code is encouraged to use the {@link #PTBTokenizer(Reader,LexedTokenFactory,String)}
 * constructor. The other constructors are historical.
 * You specify the type of result tokens with a
 * LexedTokenFactory, and can specify the treatment of tokens by mainly bool
 * options given in a comma separated String options
 * (e.g., "invertible,normalizeParentheses=true").
 * If the String is {@code null} or empty, you get the traditional
 * PTB3 normalization behaviour (i.e., you get ptb3Escaping=true).  If you
 * want no normalization, then you should pass in the String
 * "ptb3Escaping=false".  The known option names are:
 * <ol>
 * <li>invertible: Store enough information about the original form of the
 *     token and the whitespace around it that a list of tokens can be
 *     faithfully converted back to the original String.  Valid only if the
 *     LexedTokenFactory is an instance of CoreLabelTokenFactory.  The
 *     keys used in it are: TextAnnotation for the tokenized form,
 *     OriginalTextAnnotation for the original string, BeforeAnnotation and
 *     AfterAnnotation for the whitespace before and after a token, and
 *     perhaps CharacterOffsetBeginAnnotation and CharacterOffsetEndAnnotation to record
 *     token begin/after end character offsets, if they were specified to be recorded
 *     in TokenFactory construction.  (Like the String class, begin and end
 *     are done so end - begin gives the token length.) Default is false.
 * <li>tokenizeNLs: Whether end-of-lines should become tokens (or just
 *     be treated as part of whitespace). Default is false.
 * <li>ptb3Escaping: Enable all traditional PTB3 token transforms
 *     (like parentheses becoming -LRB-, -RRB-).  This is a macro flag that
 *     sets or clears all the options below. (Default setting of the various
 *     properties below that this flag controls is equivalent to it being set
 *     to true.)
 * <li>americanize: Whether to rewrite common British English spellings
 *     as American English spellings. (This is useful if your training
 *     material uses American English spelling, such as the Penn Treebank.)
 *     Default is true.
 * <li>normalizeSpace: Whether any spaces in tokens (phone numbers, fractions
 *     get turned into U+00A0 (non-breaking space).  It's dangerous to turn
 *     this off for most of our Stanford NLP software, which assumes no
 *     spaces in tokens. Default is true.
 * <li>normalizeAmpersandEntity: Whether to map the XML &amp;amp; to an
 *      ampersand. Default is true.
 * <li>normalizeCurrency: Whether to do some awful lossy currency mappings
 *     to turn common currency characters into $, #, or "cents", reflecting
 *     the fact that nothing else appears in the old PTB3 WSJ.  (No Euro!)
 *     Default is true.
 * <li>normalizeFractions: Whether to map certain common composed
 *     fraction characters to spelled out letter forms like "1/2".
 *     Default is true.
 * <li>normalizeParentheses: Whether to map round parentheses to -LRB-,
 *     -RRB-, as in the Penn Treebank. Default is true.
 * <li>normalizeOtherBrackets: Whether to map other common bracket characters
 *     to -LCB-, -LRB-, -RCB-, -RRB-, roughly as in the Penn Treebank.
 *     Default is true.
 * <li>asciiQuotes Whether to map all quote characters to the traditional ' and ".
 *     Default is false.
 * <li>latexQuotes: Whether to map quotes to ``, `, ', '', as in Latex
 *     and the PTB3 WSJ (though this is now heavily frowned on in Unicode).
 *     If true, this takes precedence over the setting of unicodeQuotes;
 *     if both are false, no mapping is done.  Default is true.
 * <li>unicodeQuotes: Whether to map quotes to the range U+2018 to U+201D,
 *     the preferred unicode encoding of single and double quotes.
 *     Default is false.
 * <li>ptb3Ellipsis: Whether to map ellipses to three dots (...), the
 *     old PTB3 WSJ coding of an ellipsis. If true, this takes precedence
 *     over the setting of unicodeEllipsis; if both are false, no mapping
 *     is done. Default is true.
 * <li>unicodeEllipsis: Whether to map dot and optional space sequences to
 *     U+2026, the Unicode ellipsis character. Default is false.
 * <li>ptb3Dashes: Whether to turn various dash characters into "--",
 *     the dominant encoding of dashes in the PTB3 WSJ. Default is true.
 * <li>keepAssimilations: true to tokenize "gonna", false to tokenize
 *                        "gon na".  Default is true.
 * <li>escapeForwardSlashAsterisk: Whether to put a backslash escape in front
 *     of / and * as the old PTB3 WSJ does for some reason (something to do
 *     with Lisp readers??). Default is true.
 * <li>untokenizable: What to do with untokenizable characters (ones not
 *     known to the tokenizer).  Six options combining whether to log a
 *     warning for none, the first, or all, and whether to delete them or
 *     to include them as single character tokens in the output: noneDelete,
 *     firstDelete, allDelete, noneKeep, firstKeep, allKeep.
 *     The default is "firstDelete".
 * <li>strictTreebank3: PTBTokenizer deliberately deviates from strict PTB3
 *      WSJ tokenization in two cases.  Setting this improves compatibility
 *      for those cases.  They are: (i) When an acronym is followed by a
 *      sentence end, such as "U.K." at the end of a sentence, the PTB3
 *      has tokens of "Corp" and ".", while by default PTBTokenizer duplicates
 *      the period returning tokens of "Corp." and ".", and (ii) PTBTokenizer
 *      will return numbers with a whole number and a fractional part like
 *      "5 7/8" as a single token, with a non-breaking space in the middle,
 *      while the PTB3 separates them into two tokens "5" and "7/8".
 *      (Exception: for only "U.S." the treebank does have the two tokens
 *      "U.S." and "." like our default; strictTreebank3 now does that too.)
 *      The default is false.
 * </ol>
 * <p>
 * A single instance of a PTBTokenizer is not thread safe, as it uses
 * a non-threadsafe JFlex object to do the processing.  Multiple
 * instances can be created safely, though.  A single instance of a
 * PTBTokenizerFactory is also not thread safe, as it keeps its
 * options in a local variable.
 * </p>
 *
 * @author Tim Grow (his tokenizer is a Java implementation of Professor
 *     Chris Manning's Flex tokenizer, pgtt-treebank.l)
 * @author Teg Grenager (grenager@stanford.edu)
 * @author Jenny Finkel (integrating in invertible PTB tokenizer)
 * @author Christopher Manning (redid API, added many options, maintenance)
 */
    public class PTBTokenizer<T> : AbstractTokenizer<T> where T:HasWord
    {
        // the underlying lexer
  private readonly PTBLexer lexer;


  /**
   * Constructs a new PTBTokenizer that returns Word tokens and which treats
   * carriage returns as normal whitespace.
   *
   * @param r The Reader whose contents will be tokenized
   * @return A PTBTokenizer that tokenizes a stream to objects of type
   *          {@link Word}
   */
  public static PTBTokenizer<Word> newPTBTokenizer(TextReader r) {
    return new PTBTokenizer<Word>(r, new WordTokenFactory(), "");
  }


  /**
   * Constructs a new PTBTokenizer that makes CoreLabel tokens.
   * It optionally returns carriage returns
   * as their own token. CRs come back as Words whose text is
   * the value of {@code PTBLexer.NEWLINE_TOKEN}.
   *
   * @param r The Reader to read tokens from
   * @param tokenizeNLs Whether to return newlines as separate tokens
   *         (otherwise they normally disappear as whitespace)
   * @param invertible if set to true, then will produce CoreLabels which
   *         will have fields for the string before and after, and the
   *         character offsets
   * @return A PTBTokenizer which returns CoreLabel objects
   */
  public static PTBTokenizer<CoreLabel> newPTBTokenizer(TextReader r, bool tokenizeNLs, bool invertible) {
    return new PTBTokenizer<CoreLabel>(r, tokenizeNLs, invertible, false, new CoreLabelTokenFactory());
  }


  /**
   * Constructs a new PTBTokenizer that optionally returns carriage returns
   * as their own token, and has a custom LexedTokenFactory.
   * If asked for, CRs come back as Words whose text is
   * the value of {@code PTBLexer.cr}.  This constructor translates
   * between the traditional bool options of PTBTokenizer and the new
   * options String.
   *
   * @param r The Reader to read tokens from
   * @param tokenizeNLs Whether to return newlines as separate tokens
   *         (otherwise they normally disappear as whitespace)
   * @param invertible if set to true, then will produce CoreLabels which
   *         will have fields for the string before and after, and the
   *         character offsets
   * @param suppressEscaping If true, all the traditional Penn Treebank
   *         normalizations are turned off.  Otherwise, they all happen.
   * @param tokenFactory The LexedTokenFactory to use to create
   *         tokens from the text.
   */
  private PTBTokenizer(TextReader r,
                       bool tokenizeNLs,
                       bool invertible,
                       bool suppressEscaping,
                       LexedTokenFactory<T> tokenFactory) {
    StringBuilder options = new StringBuilder();
    if (suppressEscaping) {
      options.Append("ptb3Escaping=false");
    } else {
      options.Append("ptb3Escaping=true"); // i.e., turn on all the historical PTB normalizations
    }
    if (tokenizeNLs) {
      options.Append(",tokenizeNLs");
    }
    if (invertible) {
      options.Append(",invertible");
    }
    lexer = new PTBLexer(r, tokenFactory, options.ToString());
  }


  /**
   * Constructs a new PTBTokenizer with a custom LexedTokenFactory.
   * Many options for tokenization and what is returned can be set via
   * the options String. See the class documentation for details on
   * the options String.  This is the new recommended constructor!
   *
   * @param r The Reader to read tokens from.
   * @param tokenFactory The LexedTokenFactory to use to create
   *         tokens from the text.
   * @param options Options to the lexer.  See the extensive documentation
   *         in the class javadoc.  The String may be null or empty,
   *         which means that all traditional PTB normalizations are
   *         done.  You can pass in "ptb3Escaping=false" and have no
   *         normalizations done (that is, the behavior of the old
   *         suppressEscaping=true option).
   */
  public PTBTokenizer(TextReader r,
                      LexedTokenFactory<T> tokenFactory,
                      String options) {
    lexer = new PTBLexer(r, tokenFactory, options);
  }


  /**
   * Internally fetches the next token.
   *
   * @return the next token in the token stream, or null if none exists.
   */
  //@Override
  //@SuppressWarnings("unchecked")
  protected override T getNext() {
    // if (lexer == null) {
    //   return null;
    // }
    try {
      return (T) lexer.next();
    } catch (IOException e) {
      throw new IOException("", e);
    }
    // cdm 2007: this shouldn't be necessary: PTBLexer decides for itself whether to return CRs based on the same flag!
    // get rid of CRs if necessary
    // while (!tokenizeNLs && PTBLexer.cr.equals(((HasWord) token).word())) {
    //   token = (T)lexer.next();
    // }

    // horatio: we used to catch exceptions here, which led to broken
    // behavior and made it very difficult to debug whatever the
    // problem was.
  }

  /**
   * Returns the string literal inserted for newlines when the -tokenizeNLs
   * options is set.
   *
   * @return string literal inserted for "\n".
   */
  public static String getNewlineToken() { return PTBLexer.NEWLINE_TOKEN; }

  /**
   * Returns a presentable version of the given PTB-tokenized text.
   * PTB tokenization splits up punctuation and does various other things
   * that makes simply joining the tokens with spaces look bad. So join
   * the tokens with space and run it through this method to produce nice
   * looking text. It's not perfect, but it works pretty well.
   *
   * @param ptbText A String in PTB3-escaped form
   * @return An approximation to the original String
   */
  public static String ptb2Text(String ptbText) {
    StringBuilder sb = new StringBuilder(ptbText.Length); // probably an overestimate
    PTB2TextLexer lexer = new PTB2TextLexer(new StringReader(ptbText));
    try {
      for (String token; (token = lexer.next()) != null; ) {
        sb.Append(token);
      }
    } catch (IOException e) {
      //e.printStackTrace();
    }
    return sb.ToString();
  }

  /**
   * Returns a presentable version of a given PTB token. For instance,
   * it transforms -LRB- into (.
   */
  public static String ptbToken2Text(String ptbText) {
    return ptb2Text(' ' + ptbText + ' ').Trim();
  }

  /**
   * Writes a presentable version of the given PTB-tokenized text.
   * PTB tokenization splits up punctuation and does various other things
   * that makes simply joining the tokens with spaces look bad. So join
   * the tokens with space and run it through this method to produce nice
   * looking text. It's not perfect, but it works pretty well.
   */
  public static int ptb2Text(TextReader ptbText, TextWriter w) /*throws IOException*/ {
    int numTokens = 0;
    PTB2TextLexer lexer = new PTB2TextLexer(ptbText);
    for (String token; (token = lexer.next()) != null; ) {
      numTokens++;
      w.Write(token);
    }
    return numTokens;
  }

  private static void untok(List<String> inputFileList, List<String> outputFileList, String charset) /*throws IOException*/ {
    long start = System.nanoTime();
    int numTokens = 0;
    int sz = inputFileList.size();
    if (sz == 0) {
      Reader r = new InputStreamReader(System.in, charset);
      BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(System.out, charset));
      numTokens = ptb2Text(r, writer);
      writer.close();
    } else {
      for (int j = 0; j < sz; j++) {
        Reader r = IOUtils.readerFromString(inputFileList.get(j), charset);
        BufferedWriter writer;
        if (outputFileList == null) {
          writer = new BufferedWriter(new OutputStreamWriter(System.out, charset));
        } else {
          writer = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(outputFileList.get(j)), charset));
        }
        numTokens += ptb2Text(r, writer);
        writer.close();
        r.close();
      }
    }
    long duration = System.nanoTime() - start;
    double wordsPerSec = (double) numTokens / ((double) duration / 1000000000.0);
    //System.err.printf("PTBTokenizer untokenized %d tokens at %.2f tokens per second.%n", numTokens, wordsPerSec);
  }

  /**
   * Returns a presentable version of the given PTB-tokenized words.
   * Pass in a List of Strings and this method will
   * join the words with spaces and call {@link #ptb2Text(String)} on the
   * output.
   *
   * @param ptbWords A list of String
   * @return A presentable version of the given PTB-tokenized words
   */
  public static String ptb2Text(List<String> ptbWords) {
    return ptb2Text(StringUtils.join(ptbWords));
  }


  /**
   * Returns a presentable version of the given PTB-tokenized words.
   * Pass in a List of Words or a Document and this method will
   * join the words with spaces and call {@link #ptb2Text(String)} on the
   * output. This method will take the word() values to prevent additional
   * text from creeping in (e.g., POS tags).
   *
   * @param ptbWords A list of HasWord objects
   * @return A presentable version of the given PTB-tokenized words
   */
  public static String labelList2Text(List<HasWord> ptbWords) {
    List<String> words = new List<String>();
    foreach (HasWord hw in ptbWords) {
      words.Add(hw.word());
    }

    return ptb2Text(words);
  }


  private static void tok(List<String> inputFileList, List<String> outputFileList, String charset, Regex parseInsidePattern, String options, bool preserveLines, bool dump, bool lowerCase) /*throws IOException*/ {
    long start = System.nanoTime();
    long numTokens = 0;
    int numFiles = inputFileList.size();
    if (numFiles == 0) {
      Reader stdin = IOUtils.readerFromStdin(charset);
      BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(System.out, charset));
      numTokens += tokReader(stdin, writer, parseInsidePattern, options, preserveLines, dump, lowerCase);
      IOUtils.closeIgnoringExceptions(writer);

    } else {
      for (int j = 0; j < numFiles; j++) {
        Reader r = IOUtils.readerFromString(inputFileList.get(j), charset);
        BufferedWriter out = (outputFileList == null) ?
          new BufferedWriter(new OutputStreamWriter(System.out, charset)) :
            new BufferedWriter(new OutputStreamWriter(new FileOutputStream(outputFileList.get(j)), charset));
        numTokens += tokReader(r, out, parseInsidePattern, options, preserveLines, dump, lowerCase);
        r.close();
        IOUtils.closeIgnoringExceptions(out);
      } // end for j going through inputFileList
    }

    long duration = System.nanoTime() - start;
    double wordsPerSec = (double) numTokens / ((double) duration / 1000000000.0);
    //System.err.printf("PTBTokenizer tokenized %d tokens at %.2f tokens per second.%n", numTokens, wordsPerSec);
  }

  private static int tokReader(TextReader r, TextWriter writer, Regex parseInsidePattern, String options, bool preserveLines, bool dump, bool lowerCase) /*throws IOException*/ {
    int numTokens = 0;
    bool beginLine = true;
    bool printing = (parseInsidePattern == null); // start off printing, unless you're looking for a start entity
    Matcher m = null;
    if (parseInsidePattern != null) {
      m = parseInsidePattern.matcher(""); // create once as performance hack
    }
    for (PTBTokenizer<CoreLabel> tokenizer = new PTBTokenizer<CoreLabel>(r, new CoreLabelTokenFactory(), options); tokenizer.hasNext(); ) {
      CoreLabel obj = tokenizer.next();
      // String origStr = obj.get(CoreAnnotations.TextAnnotation.class).replaceFirst("\n+$", ""); // DanC added this to fix a lexer bug, hopefully now corrected
      String origStr = (String)obj.get(typeof(CoreAnnotations.TextAnnotation));
      String str;
      if (lowerCase) {
        str = origStr.ToLower(Locale.ENGLISH);
        obj.set(typeof(CoreAnnotations.TextAnnotation), str);
      } else {
        str = origStr;
      }
      if (m != null && m.reset(origStr).matches()) {
        printing = m.group(1).isEmpty(); // turn on printing if no end element slash, turn it off it there is
      } else if (printing) {
        if (dump) {
          // after having checked for tags, change str to be exhaustive
          str = obj.toString();
        }
        if (preserveLines) {
          if (PTBLexer.NEWLINE_TOKEN.equals(origStr)) {
            beginLine = true;
            writer.newLine();
          } else {
            if ( ! beginLine) {
              writer.Write(' ');
            } else {
              beginLine = false;
            }
            // writer.Write(str.replace("\n", ""));
            writer.Write(str);
          }
        } else {
          writer.Write(str);
          writer.newLine();
        }
      }
      numTokens++;
    }
    return numTokens;
  }


  /** @return A PTBTokenizerFactory that vends Word tokens. */
  public static TokenizerFactory<Word> factory() {
    return PTBTokenizerFactory<Word>.newTokenizerFactory();
  }

  /** @return A PTBTokenizerFactory that vends CoreLabel tokens. */
  public static TokenizerFactory<CoreLabel> factory(bool tokenizeNLs, bool invertible) {
    return PTBTokenizerFactory.newPTBTokenizerFactory(tokenizeNLs, invertible);
  }


  /** @return A PTBTokenizerFactory that vends CoreLabel tokens with default tokenization. */
  public static TokenizerFactory<CoreLabel> coreLabelFactory() {
    return PTBTokenizerFactory.newPTBTokenizerFactory(new CoreLabelTokenFactory(), "");
  }

  /** Get a TokenizerFactory that does Penn Treebank tokenization.
   *  This is now the recommended factory method to use.
   *
   * @param factory A TokenFactory that determines what form of token is returned by the Tokenizer
   * @param options A String specifying options (see the class javadoc for details)
   * @param <T> The type of the tokens built by the LexedTokenFactory
   * @return A TokenizerFactory that does Penn Treebank tokenization
   */
  public static /*<T extends HasWord>*/ TokenizerFactory<T> factory<T>(LexedTokenFactory<T> factory, String options) where T: HasWord{
    return new PTBTokenizerFactory<T>(factory, options);

  }


  /** This class provides a factory which will vend instances of PTBTokenizer
   *  which wrap a provided Reader.  See the documentation for
   *  {@link PTBTokenizer} for details of the parameters and options.
   *
   *  @see PTBTokenizer
   *  @param <T> The class of the returned tokens
   */
  public /*static*/ class PTBTokenizerFactory<T> : TokenizerFactory<T> where T: HasWord{

    protected readonly LexedTokenFactory<T> factory;
    protected String options;


    /**
     * Constructs a new TokenizerFactory that returns Word objects and
     * treats carriage returns as normal whitespace.
     * THIS METHOD IS INVOKED BY REFLECTION BY SOME OF THE JAVANLP
     * CODE TO LOAD A TOKENIZER FACTORY.  IT SHOULD BE PRESENT IN A
     * TokenizerFactory.
     *
     * @return A TokenizerFactory that returns Word objects
     */
    public static TokenizerFactory<Word> newTokenizerFactory() {
      return newPTBTokenizerFactory(new WordTokenFactory(), "");
    }

    /**
     * Constructs a new PTBTokenizer that returns Word objects and
     * uses the options passed in.
     * THIS METHOD IS INVOKED BY REFLECTION BY SOME OF THE JAVANLP
     * CODE TO LOAD A TOKENIZER FACTORY.  IT SHOULD BE PRESENT IN A
     * TokenizerFactory.
     *
     * @param options A String of options
     * @return A TokenizerFactory that returns Word objects
     */
    public static PTBTokenizerFactory<Word> newWordTokenizerFactory(String options) {
      return new PTBTokenizerFactory<Word>(new WordTokenFactory(), options);
    }

    /**
     * Constructs a new PTBTokenizer that returns CoreLabel objects and
     * uses the options passed in.
     *
     * @param options A String of options
     * @return A TokenizerFactory that returns CoreLabel objects o
     */
    public static PTBTokenizerFactory<CoreLabel> newCoreLabelTokenizerFactory(String options) {
      return new PTBTokenizerFactory<CoreLabel>(new CoreLabelTokenFactory(), options);
    }

    /**
     * Constructs a new PTBTokenizer that uses the LexedTokenFactory and
     * options passed in.
     *
     * @param tokenFactory The LexedTokenFactory
     * @param options A String of options
     * @return A TokenizerFactory that returns objects of the type of the
     *         LexedTokenFactory
     */
    public static /*<T extends HasWord>*/ PTBTokenizerFactory<T> newPTBTokenizerFactory(LexedTokenFactory<T> tokenFactory, String options) {
      return new PTBTokenizerFactory<T>(tokenFactory, options);
    }

    public static PTBTokenizerFactory<CoreLabel> newPTBTokenizerFactory(bool tokenizeNLs, bool invertible) {
      return new PTBTokenizerFactory<CoreLabel>(tokenizeNLs, invertible, false, new CoreLabelTokenFactory());
    }


    // Constructors

    // This one is historical
    private PTBTokenizerFactory(bool tokenizeNLs, bool invertible, bool suppressEscaping, LexedTokenFactory<T> factory) {
      this.factory = factory;
      StringBuilder optionsSB = new StringBuilder();
      if (suppressEscaping) {
        optionsSB.Append("ptb3Escaping=false");
      } else {
        optionsSB.Append("ptb3Escaping=true"); // i.e., turn on all the historical PTB normalizations
      }
      if (tokenizeNLs) {
        optionsSB.Append(",tokenizeNLs");
      }
      if (invertible) {
        optionsSB.Append(",invertible");
      }
      this.options = optionsSB.ToString();
    }

    /** Make a factory for PTBTokenizers.
     *
     *  @param tokenFactory A factory for the token type that the tokenizer will return
     *  @param options Options to the tokenizer (see the class documentation for details)
     */
    private PTBTokenizerFactory(LexedTokenFactory<T> tokenFactory, String options) {
      this.factory = tokenFactory;
      this.options = options;
    }


    /** Returns a tokenizer wrapping the given Reader. */
    //@Override
    public IEnumerator<T> getIterator(TextReader r) {
      return getTokenizer(r);
    }

    /** Returns a tokenizer wrapping the given Reader. */
    //@Override
    public Tokenizer<T> getTokenizer(TextReader r) {
      return new PTBTokenizer<T>(r, factory, options);
    }

    //@Override
    public Tokenizer<T> getTokenizer(TextReader r, String extraOptions) {
      if (options == null || !options.Any()) {
        return new PTBTokenizer<T>(r, factory, extraOptions);
      } else {
        return new PTBTokenizer<T>(r, factory, options + ',' + extraOptions);
      }
    }

    //@Override
    public void setOptions(String options) {
      this.options = options;
    }
  } // end static class PTBTokenizerFactory

  /**
   * Command-line option specification.
   */
  private static Dictionary<String,int> optionArgDefs() {
    Dictionary<String,int> optionArgDefs = new Dictionary<string, int>();
    optionArgDefs["options"] = 1;
    optionArgDefs["ioFileList"] = 0;
    optionArgDefs["lowerCase"] = 0;
    optionArgDefs["dump"] = 0;
    optionArgDefs["untok"] = 0;
    optionArgDefs["encoding"] = 1;
    optionArgDefs["parseInside"] = 1;
    optionArgDefs["preserveLines"] = 0;
    return optionArgDefs;
  }

  /**
   * Reads files given as arguments and print their tokens, by default as
   * one per line.  This is useful either for testing or to run
   * standalone to turn a corpus into a one-token-per-line file of tokens.
   * This main method assumes that the input file is in utf-8 encoding,
   * unless an encoding is specified.
   * <p/>
   * Usage: <code>
   * java edu.stanford.nlp.process.PTBTokenizer [options] filename+
   * </code>
   * <p/>
   * Options:
   * <ul>
   * <li> -options options Set various tokenization options
   *       (see the documentation in the class javadoc)
   * <li> -preserveLines Produce space-separated tokens, except
   *       when the original had a line break, not one-token-per-line
   * <li> -encoding encoding Specifies a character encoding. If you do not
   *      specify one, the default is utf-8 (not the platform default).
   * <li> -lowerCase Lowercase all tokens (on tokenization)
   * <li> -parseInside regex Names an XML-style element or a regular expression
   *      over such elements.  The tokenizer will only tokenize inside elements
   *      that match this regex.  (This is done by regex matching, not an XML
   *      parser, but works well for simple XML documents, or other SGML-style
   *      documents, such as Linguistic Data Consortium releases, which adopt
   *      the convention that a line of a file is either XML markup or
   *      character data but never both.)
   * <li> -ioFileList file* The remaining command-line arguments are treated as
   *      filenames that themselves contain lists of pairs of input-output
   *      filenames (2 column, whitespace separated).
   * <li> -dump Print the whole of each CoreLabel, not just the value (word)
   * <li> -untok Heuristically untokenize tokenized text
   * <li> -h, -help Print usage info
   * </ul>
   *
   * @param args Command line arguments
   * @throws IOException If any file I/O problem
   */
  /*public static void main(String[] args) throws IOException {
    Properties options = StringUtils.argsToProperties(args, optionArgDefs());
    bool showHelp = PropertiesUtils.getBool(options, "help", false);
    showHelp = PropertiesUtils.getBool(options, "h", showHelp);
    if (showHelp) {
      System.err.println("Usage: java edu.stanford.nlp.process.PTBTokenizer [options]* filename*");
      System.err.println("  options: -h|-preserveLines|-lowerCase|-dump|-ioFileList|-encoding|-parseInside|-options");
      System.exit(0);
    }

    StringBuilder optionsSB = new StringBuilder();
    String tokenizerOptions = options.getProperty("options", null);
    if (tokenizerOptions != null) {
      optionsSB.Append(tokenizerOptions);
    }
    bool preserveLines = PropertiesUtils.getBool(options, "preserveLines", false);
    if (preserveLines) {
      optionsSB.Append(",tokenizeNLs");
    }
    bool inputOutputFileList = PropertiesUtils.getBool(options, "ioFileList", false);
    bool lowerCase = PropertiesUtils.getBool(options, "lowerCase", false);
    bool dump = PropertiesUtils.getBool(options, "dump", false);
    bool untok = PropertiesUtils.getBool(options, "untok", false);
    String charset = options.getProperty("encoding", "utf-8");
    String parseInsideKey = options.getProperty("parseInside", null);
    Pattern parseInsidePattern = null;
    if (parseInsideKey != null) {
      try {
        parseInsidePattern = Pattern.compile("<(/?)(?:" + parseInsideKey + ")(?:\\s[^>]*?)?>");
      } catch (PatternSyntaxException e) {
        // just go with null parseInsidePattern
      }
    }

    // Other arguments are filenames
    String parsedArgStr = options.getProperty("",null);
    String[] parsedArgs = (parsedArgStr == null) ? null : parsedArgStr.split("\\s+");

    List<String> inputFileList = new List<String>();
    List<String> outputFileList = null;
    if (inputOutputFileList && parsedArgs != null) {
      outputFileList = new List<String>();
      for (String fileName : parsedArgs) {
        BufferedReader r = IOUtils.readerFromString(fileName, charset);
        for (String inLine; (inLine = r.readLine()) != null; ) {
          String[] fields = inLine.split("\\s+");
          inputFileList.add(fields[0]);
          if (fields.length > 1) {
            outputFileList.add(fields[1]);
          } else {
            outputFileList.add(fields[0] + ".tok");
          }
        }
        r.close();
      }
    } else if (parsedArgs != null) {
      // Concatenate input files into a single output file
      inputFileList.addAll(Arrays.asList(parsedArgs));
    }

    if (untok) {
      untok(inputFileList, outputFileList, charset);
    } else {
      tok(inputFileList, outputFileList, charset, parseInsidePattern, optionsSB.toString(), preserveLines, dump, lowerCase);
    }
  } // end main*/
    }
}
