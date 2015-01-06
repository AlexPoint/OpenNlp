using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * Something that implements the <code>HasOffset</code> interface
 * bears a offset reference to the original text
 *
 * @author Richard Eckart (Technische Universitat Darmstadt)
 */

    public interface HasOffset
    {

        /**
   * Return the beginning character offset of the label (or -1 if none).
   *
   * @return the beginning position for the label
   */
        int beginPosition();


        /**
   * Set the beginning character offset for the label.
   * Setting this key to "-1" can be used to
   * indicate no valid value.
   *
   * @param beginPos The beginning position
   */
        void setBeginPosition(int beginPos);

        /**
   * Return the ending character offset of the label (or -1 if none).
   *
   * @return the end position for the label
   */
        int endPosition();

        /**
   * Set the ending character offset of the label (or -1 if none).
   *
   * @param endPos The end character offset for the label
   */
        void setEndPosition(int endPos);

    }
}