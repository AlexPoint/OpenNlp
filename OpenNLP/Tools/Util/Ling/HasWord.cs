using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * Something that implements the <code>HasWord</code> interface
 * knows about words.
 *
 * @author Christopher Manning
 */

    public interface HasWord
    {
        /**
   * Return the word value of the label (or null if none).
   *
   * @return String the word value for the label
   */
        String word();


        /**
         * Set the word value for the label (if one is stored).
         *
         * @param word The word value for the label
         */
        void setWord(String word);
    }
}