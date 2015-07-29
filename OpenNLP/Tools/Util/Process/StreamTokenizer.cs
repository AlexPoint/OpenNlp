using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// The <code>StreamTokenizer</code> class takes an input stream and
    /// parses it into "tokens", allowing the tokens to be
    /// Read one at a time. The parsing process is controlled by a table
    /// and a number of flags that can be set to various states. The
    /// stream tokenizer can recognize identifiers, numbers, quoted
    /// strings, and various comment styles.
    /// 
    /// Each byte Read from the input stream is regarded as a character
    /// in the range <code>'&#92;u0000'</code> through <code>'&#92;u00FF'</code>.
    /// The character value is used to look up five possible attributes of
    /// the character: <i>white space</i>, <i>alphabetic</i>,
    /// <i>numeric</i>, <i>string quote</i>, and <i>comment character</i>.
    /// Each character can have zero or more of these attributes.
    /// 
    /// In addition, an instance has four flags. These flags indicate:
    /// <ul>
    /// <li>Whether line terminators are to be returned as tokens or treated
    /// as white space that merely separates tokens.</li>
    /// <li>Whether C-style comments are to be recognized and skipped.</li>
    /// <li>Whether C++-style comments are to be recognized and skipped.</li>
    /// <li>Whether the characters of identifiers are converted to lowercase.</li>
    /// </ul>
    /// 
    /// A typical application first constructs an instance of this class,
    /// sets up the syntax tables, and then repeatedly loops calling the
    /// <code>nextToken</code> method in each iteration of the loop until
    /// it returns the value <code>TT_EOF</code>.
    /// </summary>
    public class StreamTokenizer : IEnumerable<int>
    {

        /* Only one of these will be non-null */
        private readonly TextReader reader = null;

        private readonly List<char> buf = new List<char>();

        /**
         * The next character to be considered by the nextToken method.  May also
         * be NEED_CHAR to indicate that a new character should be Read, or SKIP_LF
         * to indicate that a new character should be Read and, if it is a '\n'
         * character, it should be discarded and a second new character should be
         * Read.
         */
        private int peekc = NeedChar;

        private const int NeedChar = Int32.MaxValue;
        private const int SkipLf = Int32.MaxValue - 1;

        private bool pushedBack;
        /** The line number of the last token Read */

        private bool eolIsSignificantP = false;

        private readonly byte[] characterType = new byte[256];
        private const byte CtWhitespace = 1;
        private const byte CtDigit = 2;
        private const byte CtAlpha = 4;
        private const byte CtQuote = 8;
        private const byte CtComment = 16;

        public int LineNumber { get; private set; }

        /// <summary>
        /// After a call to the <code>nextToken</code> method, this field
        /// contains the type of the token just Read. For a single character
        /// token, its value is the single character, converted to an integer.
        /// For a quoted string token, its value is the quote character.
        /// Otherwise, its value is one of the following:
        /// <ul>
        /// <li><code>TT_WORD</code> indicates that the token is a word.</li>
        /// <li><code>TT_NUMBER</code> indicates that the token is a number.</li>
        /// <li><code>TT_EOL</code> indicates that the end of line has been Read.
        /// The field can only have this value if the <code>eolIsSignificant</code> method 
        /// has been called with the argument <code>true</code></li>
        /// <li><code>TT_EOF</code> indicates that the end of the input stream has been reached.</li>
        /// </ul>
        /// 
        /// The initial value of this field is -4.
        /// </summary>
        public int Ttype = TtNothing;

        /// <summary>
        /// A constant indicating that the end of the stream has been Read.
        /// </summary>
        public const int TtEof = -1;

        /// <summary>
        /// A constant indicating that the end of the line has been Read.
        /// </summary>
        public const int TtEol = '\n';

        /// <summary>
        /// A constant indicating that a number token has been Read.
        /// </summary>
        public const int TtNumber = -2;

        /// <summary>
        /// A constant indicating that a word token has been Read.
        /// </summary>
        public const int TtWord = -3;

        /// <summary>
        /// A constant indicating that no token has been Read, used for
        /// initializing ttype.  FIXME This could be made public and
        /// made available as the part of the API in a future release.
        /// </summary>
        private const int TtNothing = -4;

        /// <summary>
        /// If the current token is a word token, this field contains a
        /// string giving the characters of the word token. When the current
        /// token is a quoted string token, this field contains the body of the string.
        /// 
        /// The current token is a word when the value of the
        /// <code>ttype</code> field is <code>TT_WORD</code>. The current token is
        /// a quoted string token when the value of the <code>ttype</code> field is
        /// a quote character.
        /// 
        /// The initial value of this field is null.
        /// </summary>
        public string StringValue { get; private set; }

        /// <summary>
        /// If the current token is a number, this field contains the value
        /// of that number. The current token is a number when the value of
        /// the <code>ttype</code> field is <code>TT_NUMBER</code>.
        /// 
        /// The initial value of this field is 0.0.
        /// </summary>
        public double NumberValue { get; private set; }

        /// <summary>
        /// Private constructor that initializes everything except the streams.
        /// </summary>
        private StreamTokenizer()
        {
            SlashSlashComments = false;
            SlashStarComments = false;
            WordChars('a', 'z');
            WordChars('A', 'Z');
            WordChars(128 + 32, 255);
            WhitespaceChars(0, ' ');
            CommentChar('/');
            QuoteChar('"');
            QuoteChar('\'');
            ParseNumbers();
            LineNumber = 1;
        }

        /// <summary>
        /// Create a tokenizer that parses the given character stream.
        /// </summary>
        /// <param name="r">a Reader object providing the input stream.</param>
        /// <exception cref="ArgumentNullException">when r is null</exception>
        public StreamTokenizer(TextReader r)
            : this()
        {
            if (r == null)
            {
                throw new ArgumentNullException();
            }
            reader = r;
        }

        /// <summary>
        /// Resets this tokenizer's syntax table so that all characters are
        /// "ordinary." See the <code>ordinaryChar</code> method
        /// for more information on a character being ordinary.
        /// </summary>
        public void ResetSyntax()
        {
            Array.Clear(characterType, 0, characterType.Length);
        }

        /// <summary>
        /// Specifies that all characters <i>c</i> in the range
        /// <code>low &lt;= <i>c</i> &lt;= high</code>
        /// are word constituents. A word token consists of a word constituent
        /// followed by zero or more word constituents or number constituents.
        /// </summary>
        /// <param name="low">the low end of the range.</param>
        /// <param name="hi">the high end of the range.</param>
        public void WordChars(int low, int hi)
        {
            if (low < 0)
            {
                low = 0;
            }
            if (hi >= characterType.Length)
            {
                hi = characterType.Length - 1;
            }
            while (low <= hi)
            {
                characterType[low++] |= CtAlpha;
            }
        }

        /// <summary>
        /// Specifies that all characters <i>c</i> in the range
        /// <code>low &lt;= <i>c</i> &lt;= high</code>
        /// are white space characters. White space characters serve only to
        /// separate tokens in the input stream.
        /// 
        /// Any other attribute settings for the characters in the specified range are cleared.
        /// </summary>
        /// <param name="low">the low end of the range</param>
        /// <param name="hi">the high end of the range</param>
        public void WhitespaceChars(int low, int hi)
        {
            if (low < 0)
            {
                low = 0;
            }
            if (hi >= characterType.Length)
            {
                hi = characterType.Length - 1;
            }
            while (low <= hi)
            {
                characterType[low++] = CtWhitespace;
            }
        }

        /// <summary>
        /// Specifies that all characters <i>c</i> in the range
        /// <code>low &lt;= <i>c</i> &lt;= high</code>
        /// are "ordinary" in this tokenizer. See the
        /// <code>ordinaryChar</code> method for more information on a character being ordinary.
        /// </summary>
        /// <param name="low">the low end of the range.</param>
        /// <param name="hi">the high end of the range.</param>
        public void OrdinaryChars(int low, int hi)
        {
            if (low < 0)
            {
                low = 0;
            }
            if (hi >= characterType.Length)
            {
                hi = characterType.Length - 1;
            }
            while (low <= hi)
            {
                characterType[low++] = 0;
            }
        }

        /// <summary>
        /// Specifies that the character argument is "ordinary" in this tokenizer.
        /// It removes any special significance the character has as a comment character, 
        /// word component, string delimiter, white space, or number character.
        /// When such a character is encountered by the parser, the parser treats it as a
        /// single-character token and sets <code>ttype</code> field to the
        /// character value.
        /// 
        /// Making a line terminator character "ordinary" may interfere
        /// with the ability of a <code>StreamTokenizer</code> to count
        /// lines. The <code>lineno</code> method may no longer reflect
        /// the presence of such terminator characters in its line count.
        /// </summary>
        public void OrdinaryChar(int ch)
        {
            if (ch >= 0 && ch < characterType.Length)
                characterType[ch] = 0;
        }

        /// <summary>
        /// Specified that the character argument starts a single-line
        /// comment. All characters from the comment character to the end of
        /// the line are ignored by this stream tokenizer.
        /// 
        /// Any other attribute settings for the specified character are cleared.
        /// </summary>
        public void CommentChar(int ch)
        {
            if (ch >= 0 && ch < characterType.Length)
            {
                characterType[ch] = CtComment;
            }
        }

        /// <summary>
        /// Specifies that matching pairs of this character delimit string constants in this tokenizer.
        /// 
        /// When the <code>nextToken</code> method encounters a string
        /// constant, the <code>ttype</code> field is set to the string
        /// delimiter and the <code>sval</code> field is set to the body of the string.
        /// 
        /// If a string quote character is encountered, then a string is
        /// recognized, consisting of all characters after (but not including)
        /// the string quote character, up to (but not including) the next
        /// occurrence of that same string quote character, or a line
        /// terminator, or end of file. The usual escape sequences such as
        /// <code>"&#92;n"</code> and <code>"&#92;t"</code> are recognized and
        /// converted to single characters as the string is parsed.
        /// 
        /// Any other attribute settings for the specified character are cleared.
        /// </summary>
        public void QuoteChar(int ch)
        {
            if (ch >= 0 && ch < characterType.Length)
                characterType[ch] = CtQuote;
        }

        /// <summary>
        /// Specifies that numbers should be parsed by this tokenizer.
        /// The syntax table of this tokenizer is modified so that each of the twelve
        /// characters:
        /// <blockquote><pre>0 1 2 3 4 5 6 7 8 9 . -</pre></blockquote>
        /// has the "numeric" attribute.
        /// 
        /// When the parser encounters a word token that has the format of a
        /// double precision floating-point number, it treats the token as a
        /// number rather than a word, by setting the <code>ttype</code>
        /// field to the value <code>TT_NUMBER</code> and putting the numeric
        /// value of the token into the <code>nval</code> field.
        /// </summary>
        public void ParseNumbers()
        {
            for (int i = '0'; i <= '9'; i++)
            {
                characterType[i] |= CtDigit;
            }
            characterType['.'] |= CtDigit;
            characterType['-'] |= CtDigit;
        }

        /// <summary>
        /// Determines whether or not ends of line are treated as tokens.
        /// If the flag argument is true, this tokenizer treats end of lines
        /// as tokens; the <code>nextToken</code> method returns
        /// <code>TT_EOL</code> and also sets the <code>ttype</code> field to
        /// this value when an end of line is Read.
        /// 
        /// A line is a sequence of characters ending with either a
        /// carriage-return character (<code>'&#92;r'</code>) or a newline
        /// character (<code>'&#92;n'</code>). In addition, a carriage-return
        /// character followed immediately by a newline character is treated
        /// as a single end-of-line token.
        /// 
        /// If the <code>flag</code> is false, end-of-line characters are
        /// treated as white space and serve only to separate tokens.
        /// </summary>
        /// <param name="flag">
        /// <code>true</code> indicates that end-of-line characters
        /// are separate tokens; <code>false</code> indicates that
        /// end-of-line characters are white space.
        /// </param>
        public void EolIsSignificant(bool flag)
        {
            eolIsSignificantP = flag;
        }

        /// <summary>
        /// Determines whether or not the tokenizer recognizes C-style comments.
        /// If the flag argument is <code>true</code>, this stream tokenizer
        /// recognizes C-style comments. All text between successive
        /// occurrences of <code>/*</code> and <code>*&#47;</code> are discarded.
        /// 
        /// If the flag argument is <code>false</code>, then C-style comments are not treated specially.
        /// </summary>
        public bool SlashStarComments { get; set; }

        /// <summary>
        /// Determines whether or not the tokenizer recognizes C++-style comments.
        /// If the flag argument is <code>true</code>, this stream tokenizer
        /// recognizes C++-style comments. Any occurrence of two consecutive
        /// slash characters (<code>'/'</code>) is treated as the beginning of
        /// a comment that extends to the end of the line.
        /// 
        /// If the flag argument is <code>false</code>, then C++-style
        /// comments are not treated specially.
        /// </summary>
        public bool SlashSlashComments { get; set; }

        /**
         * Determines whether or not word token are automatically lowercased.
         * If the flag argument is <code>true</code>, then the value in the
         * <code>sval</code> field is lowercased whenever a word token is
         * returned (the <code>ttype</code> field has the
         * value <code>TT_WORD</code> by the <code>nextToken</code> method
         * of this tokenizer.
         * <p>
         * If the flag argument is <code>false</code>, then the
         * <code>sval</code> field is not modified.
         *
         * @param   fl   <code>true</code> indicates that all word tokens should
         *               be lowercased.
         */

        /// <summary>
        /// Determines whether or not word token are automatically lowercased.
        /// If the flag argument is <code>true</code>, then the value in the
        /// <code>sval</code> field is lowercased whenever a word token is
        /// returned (the <code>ttype</code> field has the
        /// value <code>TT_WORD</code> by the <code>nextToken</code> method of this tokenizer.
        /// 
        /// If the flag argument is <code>false</code>, then the
        /// <code>sval</code> field is not modified.
        /// </summary>
        public bool LowerCaseMode { private get; set; }

        /// <summary>
        /// Read the next character
        /// </summary>
        private int Read()
        {
            if (reader != null)
            {
                return reader.Read();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Parses the next token from the input stream of this tokenizer.
        /// The type of the next token is returned in the <code>ttype</code>
        /// field. Additional information about the token may be in the
        /// <code>nval</code> field or the <code>sval</code> field of this tokenizer.
        /// 
        /// Typical clients of this class first set up the syntax tables and then sit in a loop
        /// calling nextToken to parse successive tokens until TT_EOF is returned.
        /// </summary>
        /// <returns>the value of the <code>ttype</code> field</returns>
        public int NextToken()
        {
            if (pushedBack)
            {
                pushedBack = false;
                return Ttype;
            }
            byte[] ct = characterType;
            StringValue = null;

            int c = peekc;
            if (c < 0)
                c = NeedChar;
            if (c == SkipLf)
            {
                c = Read();
                if (c < 0)
                    return Ttype = TtEof;
                if (c == '\n')
                    c = NeedChar;
            }
            if (c == NeedChar)
            {
                c = Read();
                if (c < 0)
                    return Ttype = TtEof;
            }
            Ttype = c; /* Just to be safe */

            /* Set peekc so that the next invocation of nextToken will Read
             * another character unless peekc is reset in this invocation
             */
            peekc = NeedChar;

            int ctype = c < 256 ? ct[c] : CtAlpha;
            while ((ctype & CtWhitespace) != 0)
            {
                if (c == '\r')
                {
                    LineNumber++;
                    if (eolIsSignificantP)
                    {
                        peekc = SkipLf;
                        return Ttype = TtEol;
                    }
                    c = Read();
                    if (c == '\n')
                        c = Read();
                }
                else
                {
                    if (c == '\n')
                    {
                        LineNumber++;
                        if (eolIsSignificantP)
                        {
                            return Ttype = TtEol;
                        }
                    }
                    c = Read();
                }
                if (c < 0)
                    return Ttype = TtEof;
                ctype = c < 256 ? ct[c] : CtAlpha;
            }

            if ((ctype & CtDigit) != 0)
            {
                bool neg = false;
                if (c == '-')
                {
                    c = Read();
                    if (c != '.' && (c < '0' || c > '9'))
                    {
                        peekc = c;
                        return Ttype = '-';
                    }
                    neg = true;
                }
                double v = 0;
                int decexp = 0;
                int seendot = 0;
                while (true)
                {
                    if (c == '.' && seendot == 0)
                        seendot = 1;
                    else if ('0' <= c && c <= '9')
                    {
                        v = v*10 + (c - '0');
                        decexp += seendot;
                    }
                    else
                        break;
                    c = Read();
                }
                peekc = c;
                if (decexp != 0)
                {
                    double denom = 10;
                    decexp--;
                    while (decexp > 0)
                    {
                        denom *= 10;
                        decexp--;
                    }
                    /* Do one division of a likely-to-be-more-accurate number */
                    v = v/denom;
                }
                NumberValue = neg ? -v : v;
                return Ttype = TtNumber;
            }

            if ((ctype & CtAlpha) != 0)
            {
                int i = 0;
                do
                {
                    buf[i++] = (char) c;
                    c = Read();
                    ctype = c < 0 ? CtWhitespace : c < 256 ? ct[c] : CtAlpha;
                } while ((ctype & (CtAlpha | CtDigit)) != 0);
                peekc = c;
                StringValue = new string(buf.ToArray(), 0, i);
                if (LowerCaseMode)
                    StringValue = StringValue.ToLower();
                return Ttype = TtWord;
            }

            if ((ctype & CtQuote) != 0)
            {
                Ttype = c;
                int i = 0;
                /* Invariants (because \Octal needs a lookahead):
                 *   (i)  c contains char value
                 *   (ii) d contains the lookahead
                 */
                int d = Read();
                while (d >= 0 && d != Ttype && d != '\n' && d != '\r')
                {
                    if (d == '\\')
                    {
                        c = Read();
                        int first = c; /* To allow \377, but not \477 */
                        if (c >= '0' && c <= '7')
                        {
                            c = c - '0';
                            int c2 = Read();
                            if ('0' <= c2 && c2 <= '7')
                            {
                                c = (c << 3) + (c2 - '0');
                                c2 = Read();
                                if ('0' <= c2 && c2 <= '7' && first <= '3')
                                {
                                    c = (c << 3) + (c2 - '0');
                                    d = Read();
                                }
                                else
                                    d = c2;
                            }
                            else
                                d = c2;
                        }
                        else
                        {
                            switch (c)
                            {
                                case 'a':
                                    c = 0x7;
                                    break;
                                case 'b':
                                    c = '\b';
                                    break;
                                case 'f':
                                    c = 0xC;
                                    break;
                                case 'n':
                                    c = '\n';
                                    break;
                                case 'r':
                                    c = '\r';
                                    break;
                                case 't':
                                    c = '\t';
                                    break;
                                case 'v':
                                    c = 0xB;
                                    break;
                            }
                            d = Read();
                        }
                    }
                    else
                    {
                        c = d;
                        d = Read();
                    }
                    buf[i++] = (char) c;
                }

                /* If we broke out of the loop because we found a matching quote
                 * character then arrange to Read a new character next time
                 * around; otherwise, save the character.
                 */
                peekc = (d == Ttype) ? NeedChar : d;

                StringValue = new string(buf.ToArray(), 0, i);
                return Ttype;
            }

            if (c == '/' && (SlashSlashComments || SlashStarComments))
            {
                c = Read();
                if (c == '*' && SlashStarComments)
                {
                    int prevc = 0;
                    while ((c = Read()) != '/' || prevc != '*')
                    {
                        if (c == '\r')
                        {
                            LineNumber++;
                            c = Read();
                            if (c == '\n')
                            {
                                c = Read();
                            }
                        }
                        else
                        {
                            if (c == '\n')
                            {
                                LineNumber++;
                                c = Read();
                            }
                        }
                        if (c < 0)
                            return Ttype = TtEof;
                        prevc = c;
                    }
                    return NextToken();
                }
                else if (c == '/' && SlashSlashComments)
                {
                    while ((c = Read()) != '\n' && c != '\r' && c >= 0) ;
                    peekc = c;
                    return NextToken();
                }
                else
                {
                    /* Now see if it is still a single line comment */
                    if ((ct['/'] & CtComment) != 0)
                    {
                        while ((c = Read()) != '\n' && c != '\r' && c >= 0) ;
                        peekc = c;
                        return NextToken();
                    }
                    else
                    {
                        peekc = c;
                        return Ttype = '/';
                    }
                }
            }

            if ((ctype & CtComment) != 0)
            {
                while ((c = Read()) != '\n' && c != '\r' && c >= 0) ;
                peekc = c;
                return NextToken();
            }

            return Ttype = c;
        }

        /// <summary>
        /// Causes the next call to the <code>nextToken</code> method of this
        /// tokenizer to return the current value in the <code>ttype</code>
        /// field, and not to modify the value in the <code>nval</code> or
        /// <code>sval</code> field.
        /// </summary>
        public void PushBack()
        {
            if (Ttype != TtNothing) /* No-op if nextToken() not called */
            {
                pushedBack = true;
            }
        }
        
        public override string ToString()
        {
            string ret;
            switch (Ttype)
            {
                case TtEof:
                    ret = "EOF";
                    break;
                case TtEol:
                    ret = "EOL";
                    break;
                case TtWord:
                    ret = StringValue;
                    break;
                case TtNumber:
                    ret = "n=" + NumberValue;
                    break;
                case TtNothing:
                    ret = "NOTHING";
                    break;
                default:
                {
                    /* 
                         * ttype is the first character of either a quoted string or
                         * is an ordinary character. ttype can definitely not be less
                         * than 0, since those are reserved values used in the previous
                         * case statements
                         */
                    if (Ttype < 256 &&
                        ((characterType[Ttype] & CtQuote) != 0))
                    {
                        ret = StringValue;
                        break;
                    }

                    var s = new char[3];
                    s[0] = s[2] = '\'';
                    s[1] = (char) Ttype;
                    ret = new string(s);
                    break;
                }
            }
            return "Token[" + ret + "], line " + LineNumber;
        }


        public IEnumerator<int> GetEnumerator()
        {
            ResetSyntax();
            while (true)
            {
                int token = NextToken();
                if (token == TtEof)
                {
                    yield break;
                }
                yield return token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}