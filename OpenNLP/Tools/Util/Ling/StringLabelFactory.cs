using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A <code>StringLabelFactory</code> object makes a simple
 * <code>StringLabel</code> out of a <code>string</code>.
 *
 * @author Christopher Manning
 */

    public class StringLabelFactory : LabelFactory
    {
        /**
   * Make a new label with this <code>string</code> as the "name".
   *
   * @param labelStr A string that determines the content of the label.
   *                 For a StringLabel, it is exactly the given string
   * @return The created label
   */

        public Label NewLabel(string labelStr)
        {
            return new StringLabel(labelStr);
        }


        /**
         * Make a new label with this <code>string</code> as the "name".
         *
         * @param labelStr A string that determines the content of the label.
         *                 For a StringLabel, it is exactly the given string
         * @param options  The options are ignored by a StringLabelFactory
         * @return The created label
         */

        public Label NewLabel(string labelStr, int options)
        {
            return new StringLabel(labelStr);
        }


        /**
         * Make a new label with this <code>string</code> as the "name".
         * This version does no decoding -- StringLabels just have a value.
         *
         * @param labelStr A string that determines the content of the label.
         *                 For a StringLabel, it is exactly the given string
         * @return The created label
         */

        public Label NewLabelFromString(string labelStr)
        {
            return new StringLabel(labelStr);
        }


        /**
         * Create a new <code>StringLabel</code>, where the label is
         * formed from
         * the <code>Label</code> object passed in.  Depending on what fields
         * each label has, other things will be <code>null</code>.
         *
         * @param oldLabel The Label that the new label is being created from
         * @return a new label of a particular type
         */

        public Label NewLabel(Label oldLabel)
        {
            return new StringLabel(oldLabel);
        }
    }
}