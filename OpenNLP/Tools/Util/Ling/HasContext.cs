using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * @author grenager
 */

    public interface HasContext
    {

        /**
         * @return the string before the word
         */
        string before();

        /**
         * Set the whitespace string before the word.
         * @param before the whitespace string before the word
         */
        void setBefore(string before);

        /**
         * Return the string which is the original character sequence of the token.
         *
         * @return The original character sequence of the token
         */
        string originalText();

        /**
         * Set the string which is the original character sequence of the token.
         *
         * @param originalText The original character sequence of the token
         */
        void setOriginalText(string originalText);

        /**
         * Return the whitespace string after the word.
         *
         * @return The whitespace string after the word
         */
        string after();

        /**
         * Set the whitespace string after the word.
         *
         * @param after The whitespace string after the word
         */
        void setAfter(string after);

    }
}