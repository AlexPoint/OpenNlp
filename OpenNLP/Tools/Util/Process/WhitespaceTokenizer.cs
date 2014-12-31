using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Process
{
    /**
 * A WhitespaceTokenizer is a tokenizer that splits on and discards only
 * whitespace characters.
 * This implementation returns Word objects. It has a parameter for whether
 * to make EOL a token or whether to treat EOL characters as whitespace.
 * If an EOL is a token, the class returns it as a Word with String value "\n".
 * <p/>
 * <i>Implementation note:</i> This was rewritten in Apr 2006 to discard the
 * old StreamTokenizer based implementation and to replace it with a
 * Unicode compliant JFlex-based version.
 *
 * @author Joseph Smarr (jsmarr@stanford.edu)
 * @author Teg Grenager (grenager@stanford.edu)
 * @author Roger Levy
 * @author Christopher Manning
 */
    public class WhitespaceTokenizer<T>: AbstractTokenizer<T> where T: HasWord
    {
        private WhitespaceLexer lexer;
  private readonly bool eolIsSignificant;

  /**
   * A factory which vends WhitespaceTokenizers.
   *
   * @author Christopher Manning
   */
  public /*static*/ class WhitespaceTokenizerFactory<T> : TokenizerFactory<T> where T: HasWord{

    private bool tokenizeNLs;
    private LexedTokenFactory<T> factory;

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
      return new WhitespaceTokenizerFactory<Word>(new WordTokenFactory(),
                                                  false);
    }

    public WhitespaceTokenizerFactory(LexedTokenFactory<T> factory):
      this(factory, false){}

    public WhitespaceTokenizerFactory(LexedTokenFactory<T> factory,
                                      String options) {
      this.factory = factory;
      Dictionary<String,String> prop = StringUtils.stringToProperties(options);
      this.tokenizeNLs = PropertiesUtils.getBool(prop, "tokenizeNLs", false);
    }

    public WhitespaceTokenizerFactory(LexedTokenFactory<T> factory,
                                      bool tokenizeNLs) {
      this.factory = factory;
      this.tokenizeNLs = tokenizeNLs;
    }

    public IEnumerator<T> getIterator(TextReader r) {
      return getTokenizer(r);
    }

    public Tokenizer<T> getTokenizer(TextReader r) {
      return new WhitespaceTokenizer<T>(factory, r, tokenizeNLs);
    }

    public Tokenizer<T> getTokenizer(TextReader r, String extraOptions) {
      Dictionary<String,String> prop = StringUtils.stringToProperties(extraOptions);
      bool tokenizeNewlines =
        PropertiesUtils.getBool(prop, "tokenizeNLs", this.tokenizeNLs);

      return new WhitespaceTokenizer<T>(factory, r, tokenizeNewlines);
    }


    public void setOptions(String options) {
      Dictionary<String,String> prop = StringUtils.stringToProperties(options);
      tokenizeNLs = PropertiesUtils.getBool(prop, "tokenizeNLs", tokenizeNLs);
    }
  } // end class WhitespaceTokenizerFactory

  public static WhitespaceTokenizerFactory<CoreLabel> newCoreLabelTokenizerFactory(String options){
    return new WhitespaceTokenizerFactory<CoreLabel>(new CoreLabelTokenFactory(), options);
  }

  public static WhitespaceTokenizerFactory<CoreLabel> newCoreLabelTokenizerFactory() {
    return new WhitespaceTokenizerFactory<CoreLabel>(new CoreLabelTokenFactory());
  }

  /**
   * Internally fetches the next token.
   *
   * @return the next token in the token stream, or null if none exists.
   */
  //@SuppressWarnings("unchecked")
  //@Override
  protected override T getNext() {
    T token = default(T);
    if (lexer == null) {
      return token;
    }
    try {
      token = (T) lexer.next();
      while (token != null && token.word().Equals(WhitespaceLexer.NEWLINE)) {
        if (eolIsSignificant) {
          return token;
        } else {
          token = (T) lexer.next();
        }
      }
    } catch (IOException e) {
      // do nothing, return null
    }
    return token;
  }

  /**
   * Constructs a new WhitespaceTokenizer
   * @param r The Reader that is its source.
   * @param eolIsSignificant Whether eol tokens should be returned.
   */
  public WhitespaceTokenizer(LexedTokenFactory<T> factory,
                             TextReader r, bool eolIsSignificant) {
    this.eolIsSignificant = eolIsSignificant;
    // The conditional below is perhaps currently needed in LexicalizedParser, since
    // it passes in a null arg while doing type-checking for sentence escaping
    // but StreamTokenizer barfs on that.  But maybe shouldn't be here.
    if (r != null) {
      lexer = new WhitespaceLexer(r, factory);
    }
  }

  public static WhitespaceTokenizer<CoreLabel> newCoreLabelWhitespaceTokenizer(TextReader r) {
    return new WhitespaceTokenizer<CoreLabel>(new CoreLabelTokenFactory(), r, false);
  }

  public static WhitespaceTokenizer<CoreLabel> newCoreLabelWhitespaceTokenizer(TextReader r, bool tokenizeNLs) {
    return new WhitespaceTokenizer<CoreLabel>(new CoreLabelTokenFactory(), r, tokenizeNLs);
  }

  public static WhitespaceTokenizer<Word>
    newWordWhitespaceTokenizer(TextReader r)
  {
    return newWordWhitespaceTokenizer(r, false);
  }

  public static WhitespaceTokenizer<Word>
    newWordWhitespaceTokenizer(TextReader r, bool eolIsSignificant)
  {
    return new WhitespaceTokenizer<Word>(new WordTokenFactory(), r,
                                         eolIsSignificant);
  }

  /* ----
   * Sets the source of this Tokenizer to be the Reader r.

  private void setSource(Reader r) {
    lexer = new WhitespaceLexer(r);
  }
  ---- */

  public static TokenizerFactory<Word> factory() {
    return new WhitespaceTokenizerFactory<Word>(new WordTokenFactory(),
                                                false);
  }

  public static TokenizerFactory<Word> factory(bool eolIsSignificant) {
    return new WhitespaceTokenizerFactory<Word>(new WordTokenFactory(),
                                                eolIsSignificant);
  }

  /**
   * Reads a file from the argument and prints its tokens one per line.
   * This is mainly as a testing aid, but it can also be quite useful
   * standalone to turn a corpus into a one token per line file of tokens.
   * <p/>
   * Usage: <code>java edu.stanford.nlp.process.WhitespaceTokenizer filename
   * </code>
   *
   * @param args Command line arguments
   * @throws IOException If can't open files, etc.
   */
  /*public static void main(String[] args) throws IOException {

    bool eolIsSignificant = (args.length > 0 && args[0].equals("-cr"));
    Reader reader = ((args.length > 0 &&
                      !args[args.length - 1].equals("-cr")) ?
                     new InputStreamReader(new FileInputStream
                                           (args[args.length - 1]), "UTF-8") :
                     new InputStreamReader(System.in, "UTF-8"));
    WhitespaceTokenizer<Word> tokenizer =
      new WhitespaceTokenizer<Word>(new WordTokenFactory(), reader,
                                    eolIsSignificant);
    PrintWriter pw =
      new PrintWriter(new OutputStreamWriter(System.out, "UTF-8"), true);
    while (tokenizer.hasNext()) {
      Word w = tokenizer.next();
      if (w.value().equals(WhitespaceLexer.NEWLINE)) {
        pw.println("***CR***");
      } else {
        pw.println(w);
      }
    }
  }*/
    }
}
