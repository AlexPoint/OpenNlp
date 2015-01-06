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
         * @return the String before the word
         */
        String before();

        /**
         * Set the whitespace String before the word.
         * @param before the whitespace String before the word
         */
        void setBefore(String before);

        /**
         * Return the String which is the original character sequence of the token.
         *
         * @return The original character sequence of the token
         */
        String originalText();

        /**
         * Set the String which is the original character sequence of the token.
         *
         * @param originalText The original character sequence of the token
         */
        void setOriginalText(String originalText);

        /**
         * Return the whitespace String after the word.
         *
         * @return The whitespace String after the word
         */
        String after();

        /**
         * Set the whitespace String after the word.
         *
         * @param after The whitespace String after the word
         */
        void setAfter(String after);

    }
}