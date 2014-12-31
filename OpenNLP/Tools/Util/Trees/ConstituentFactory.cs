using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * A <code>ConstituentFactory</code> is a factory for creating objects
 * of class <code>Constituent</code>, or some descendent class.
 * An interface.
 *
 * @author Christopher Manning
 */
    public interface ConstituentFactory
    {
        /**
   * Build a constituent with this start and end.
   *
   * @param start Start position
   * @param end   End position
   */
        Constituent newConstituent(int start, int end);

        /**
         * Build a constituent with this start and end.
         *
         * @param start Start position
         * @param end   End position
         * @param label Label
         * @param score Score
         */
        Constituent newConstituent(int start, int end, Label label, double score);
    }
}
