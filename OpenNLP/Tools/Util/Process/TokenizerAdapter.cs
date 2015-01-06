using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Process
{
    /**
 * This class adapts between a <code>java.io.StreamTokenizer</code>
 * and a <code>edu.stanford.nlp.process.Tokenizer</code>.
 *
 * @author Christopher Manning
 * @version 2004/04/01
 */

    public class TokenizerAdapter : AbstractTokenizer<string>
    {
        protected readonly StreamTokenizer st;

        protected string eolString = "<EOL>";


        /**
   * Create a new <code>TokenizerAdaptor</code>.  In general, it is
   * recommended that the passed in <code>StreamTokenizer</code> should
   * have had <code>resetSyntax()</code> done to it, so that numbers are
   * returned as entered as tokens of type <code>string</code>, though this
   * code will cope as best it can.
   *
   * @param st The internal <code>java.io.StreamTokenizer</code>
   */

        public TokenizerAdapter(StreamTokenizer st)
        {
            this.st = st;
        }


        /**
   * Internally fetches the next token.
   *
   * @return the next token in the token stream, or null if none exists.
   */
        //@Override
        protected override string getNext()
        {
            try
            {
                int nextTok = st.NextToken();
                switch (nextTok)
                {
                    case StreamTokenizer.TT_EOL:
                        return eolString;
                    case StreamTokenizer.TT_EOF:
                        return null;
                    case StreamTokenizer.TT_WORD:
                        return st.StringValue;
                    case StreamTokenizer.TT_NUMBER:
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


        /**
   * Set the <code>string</code> returned when the inner tokenizer
   * returns an end-of-line token.  This will only happen if the
   * inner tokenizer has been set to <code>eolIsSignificant(true)</code>.
   *
   * @param eolString The string used to represent eol.  It is not allowed
   *                  to be <code>null</code> (which would confuse line ends and file end)
   */

        public void setEolString(string eolString)
        {
            if (eolString == null)
            {
                throw new ArgumentException("eolString cannot be null");
            }
            this.eolString = eolString;
        }


        /**
   * Say whether the <code>string</code> is the end-of-line token for
   * this tokenizer.
   *
   * @param str The string being tested
   * @return Whether it is the end-of-line token
   */

        public bool isEol(string str)
        {
            return eolString.Equals(str);
        }
    }
}