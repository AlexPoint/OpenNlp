using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A <code>StringLabel</code> object acts as a Label by containing a
 * single String, which it sets or returns in response to requests.
 * The hashCode() and compareTo() methods for this class assume that this
 * string value is non-null.  equals() is correctly implemented
 *
 * @author Christopher Manning
 * @version 2000/12/20
 */

    public class StringLabel : ValueLabel, HasOffset
    {
        private string str;

        /**
   * Start position of the word in the original input string
   */
        private int pbeginPosition = -1;

        /**
   * End position of the word in the original input string
   */
        private int pendPosition = -1;


        /**
   * Create a new <code>StringLabel</code> with a null content (i.e., str).
   */

        public StringLabel()
        {
        }


        /**
   * Create a new <code>StringLabel</code> with the given content.
   *
   * @param str The new label's content
   */

        public StringLabel(string str)
        {
            this.str = str;
        }

        /**
   * Create a new <code>StringLabel</code> with the given content.
   *
   * @param str The new label's content
   * @param beginPosition Start offset in original text
   * @param endPosition End offset in original text
   */

        public StringLabel(string str, int beginPosition, int endPosition)
        {
            this.str = str;
            setBeginPosition(beginPosition);
            setEndPosition(endPosition);
        }


        /**
   * Create a new <code>StringLabel</code> with the
   * <code>value()</code> of another label as its label.
   *
   * @param label The other label
   */

        public StringLabel(Label label)
        {
            this.str = label.value();
            if (label is HasOffset)
            {
                HasOffset ofs = (HasOffset) label;
                setBeginPosition(ofs.beginPosition());
                setEndPosition(ofs.endPosition());
            }
        }


        /**
   * Return the word value of the label (or null if none).
   *
   * @return string the word value for the label
   */
        //@Override
        public override string value()
        {
            return str;
        }


        /**
   * Set the value for the label.
   *
   * @param value The value for the label
   */
        //@Override
        public override void setValue( /*final */ string value)
        {
            str = value;
        }


        /**
   * Set the label from a string.
   *
   * @param str The str for the label
   */
        //@Override
        public override void setFromString( /*final */ string str)
        {
            this.str = str;
        }

        //@Override
        public override string ToString()
        {
            return str;
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class StringLabelFactoryHolder
        {

            //private StringLabelFactoryHolder() {}

            public static readonly LabelFactory lf = new StringLabelFactory();
        }

        /**
   * Return a factory for this kind of label
   * (i.e., <code>StringLabel</code>).
   * The factory returned is always the same one (a singleton).
   *
   * @return The label factory
   */
        //@Override
        public override LabelFactory labelFactory()
        {
            return StringLabelFactoryHolder.lf;
        }


        /**
   * Return a factory for this kind of label.
   *
   * @return The label factory
   */

        public static LabelFactory factory()
        {
            return StringLabelFactoryHolder.lf;
        }

        public int beginPosition()
        {
            return pbeginPosition;
        }

        public int endPosition()
        {
            return pendPosition;
        }

        public void setBeginPosition(int beginPosition)
        {
            this.pbeginPosition = beginPosition;
        }

        public void setEndPosition(int endPosition)
        {
            this.pendPosition = endPosition;
        }

        private static readonly long serialVersionUID = -4153619273767524247L;
    }
}