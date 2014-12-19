using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * Something that implements the <code>HasLemma</code> interface
 * knows about lemmas.
 *
 * @author John Bauer
 */

    public interface HasLemma
    {

        /**
   * Return the lemma value of the label (or null if none).
   *
   * @return String the lemma value for the label
   */
        String lemma();


        /**
   * Set the lemma value for the label (if one is stored).
   *
   * @param lemma The lemma value for the label
   */
        void setLemma(String lemma);

    }
}
