using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * Something that implements the <code>HasCategory</code> interface
 * knows about categories.
 *
 * @author Christopher Manning
 */
    public interface HasCategory
    {

        /**
         * Return the category value of the label (or null if none).
         *
         * @return String the category value for the label
         */
        String category();


        /**
         * Set the category value for the label (if one is stored).
         *
         * @param category The category value for the label
         */
        void setCategory(String category);

    }
}
