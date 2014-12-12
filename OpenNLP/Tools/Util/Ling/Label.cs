using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    public interface Label
    {
        /**
   * Return a String representation of just the "main" value of this label.
   *
   * @return the "value" of the label
   */
        String value();


        /**
         * Set the value for the label (if one is stored).
         *
         * @param value - the value for the label
         */
        void setValue(String value);


        /**
         * Return a String representation of the label.  For a multipart label,
         * this will return all parts.  The <code>toString()</code> method
         * causes a label to spill its guts.  It should always return an
         * empty string rather than <code>null</code> if there is no value.
         *
         * @return a text representation of the full label contents
         */
        String ToString();


        /**
         * Set the contents of this label to this <code>String</code>
         * representing the
         * complete contents of the label.  A class implementing label may
         * throw an <code>UnsupportedOperationException</code> for this
         * method (only).  Typically, this method would do
         * some appropriate decoding of the string in a way that sets
         * multiple fields in an inverse of the <code>toString()</code>
         * method.
         *
         * @param labelStr the String that translates into the content of the
         *                 label
         */
        void setFromString(String labelStr);

        /**
         * Returns a factory that makes labels of the exact same type as this one.
         * May return <code>null</code> if no appropriate factory is known.
         *
         * @return the LabelFactory for this kind of label
         */
        LabelFactory labelFactory();
    }
}
