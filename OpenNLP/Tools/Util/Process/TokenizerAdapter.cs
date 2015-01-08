using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /// <summary>
    /// This class adapts between a <code>java.io.StreamTokenizer</code>
    /// and a <code>edu.stanford.nlp.process.Tokenizer</code>.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TokenizerAdapter : AbstractTokenizer<string>
    {
        protected readonly StreamTokenizer st;

        protected string EolString = "<EOL>";

        /// <summary>
        /// Create a new <code>TokenizerAdaptor</code>.
        /// In general, it is recommended that the passed in <code>StreamTokenizer</code> should
        /// have had <code>resetSyntax()</code> done to it, so that numbers are
        /// returned as entered as tokens of type <code>string</code>, though this
        /// code will cope as best it can.
        /// </summary>
        /// <param name="st">The internal <code>java.io.StreamTokenizer</code></param>
        public TokenizerAdapter(StreamTokenizer st)
        {
            this.st = st;
        }

        /// <summary>
        /// Internally fetches the next token.
        /// </summary>
        /// <returns>The next token in the token stream, or null if none exists.</returns>
        protected override string GetNext()
        {
            try
            {
                int nextTok = st.NextToken();
                switch (nextTok)
                {
                    case StreamTokenizer.TtEol:
                        return EolString;
                    case StreamTokenizer.TtEof:
                        return null;
                    case StreamTokenizer.TtWord:
                        return st.StringValue;
                    case StreamTokenizer.TtNumber:
                        return st.NumberValue.ToString();
                    default:
                        char[] t = {(char) nextTok}; // (array initialization)
                        return new string(t);
                }
            }
            catch (IOException ioe)
            {
                // do nothing, return null
                return null;
            }
        }

        /// <summary>
        /// Set the <code>string</code> returned when the inner tokenizer
        /// returns an end-of-line token.  This will only happen if the
        /// inner tokenizer has been set to <code>eolIsSignificant(true)</code>.
        /// </summary>
        /// <param name="eolString">
        /// The string used to represent eol.
        /// It is not allowed to be <code>null</code> (which would confuse line ends and file end)
        /// </param>
        /// <exception cref="ArgumentException">When eolString is null</exception>
        public void SetEolString(string eolString)
        {
            if (eolString == null)
            {
                throw new ArgumentException("eolString cannot be null");
            }
            this.EolString = eolString;
        }

        /// <summary>
        /// Say whether the <code>string</code> is the end-of-line token for this tokenizer.
        /// </summary>
        /// <param name="str">The string being tested</param>
        /// <returns>Whether it is the end-of-line token</returns>
        public bool IsEol(string str)
        {
            return EolString.Equals(str);
        }
    }
}