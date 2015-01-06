using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A <code>ValueLabel</code> object acts as a Label with linguistic
 * attributes.  This is an abstract class, which doesn't actually store
 * or return anything.  It returns <code>null</code> to any requests. However,
 * it does
 * stipulate that equals() and compareTo() are defined solely with respect to
 * value(); this should not be changed by subclasses.
 * Other fields of a ValueLabel subclass should be regarded
 * as secondary facets (it is almost impossible to override equals in
 * a useful way while observing the contract for equality defined for Object,
 * in particular, that equality must by symmetric).
 * This class is designed to be extended.
 *
 * @author Christopher Manning
 */

    public abstract class ValueLabel : Label, IComparable<ValueLabel>
    {
        protected ValueLabel()
        {
        }


        /**
   * Return the value of the label (or null if none).
   * The default value returned by an <code>ValueLabel</code> is
   * always <code>null</code>
   *
   * @return the value for the label
   */

        public virtual string Value()
        {
            return null;
        }


        /**
   * Set the value for the label (if one is stored).
   *
   * @param value - the value for the label
   */

        public virtual void SetValue(string value)
        {
        }


        /**
   * Return a string representation of the label.  This will just
   * be the <code>value()</code> if it is non-<code>null</code>,
   * and the empty string otherwise.
   *
   * @return The string representation
   */
        //@Override
        public override string ToString()
        {
            string val = Value();
            return (val == null) ? "" : val;
        }


        public virtual void SetFromString(string labelStr)
        {
            throw new NotSupportedException();
        }


        /**
   * Equality for <code>ValueLabel</code>s is defined in the first instance
   * as equality of their <code>string</code> <code>value()</code>.
   * Now rewritten to correctly enforce the contract of equals in Object.
   * Equality for a <code>ValueLabel</code> is determined simply by String
   * equality of its <code>value()</code>.  Subclasses should not redefine
   * this to include other aspects of the <code>ValueLabel</code>, or the
   * contract for <code>equals()</code> is broken.
   *
   * @param obj the object against which equality is to be checked
   * @return true if <code>this</code> and <code>obj</code> are equal
   */
        //@Override
        public override bool Equals(Object obj)
        {
            string val = Value();
            return (obj is ValueLabel) &&
                   (val == null ? ((Label) obj).Value() == null : val.Equals(((Label) obj).Value()));
        }


        /**
   * Return the hashCode of the string value providing there is one.
   * Otherwise, returns an arbitrary constant for the case of
   * <code>null</code>.
   */
        //@Override
        public override int GetHashCode()
        {
            string val = Value();
            return val == null ? 3 : val.GetHashCode();
        }


        /**
   * Orders by <code>value()</code>'s lexicographic ordering.
   *
   * @param valueLabel object to compare to
   * @return result (positive if this is greater than obj)
   */

        public int CompareTo(ValueLabel valueLabel)
        {
            return Value().CompareTo(valueLabel.Value());
        }


        /**
   * Returns a factory that makes Labels of the appropriate sort.
   *
   * @return the <code>LabelFactory</code>
   */
        public abstract LabelFactory LabelFactory();


        private static readonly long serialVersionUID = -1413303679077285530L;
    }
}