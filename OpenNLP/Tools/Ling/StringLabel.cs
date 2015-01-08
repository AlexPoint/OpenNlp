using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// A <code>StringLabel</code> object acts as a Label by containing a
    /// single String, which it sets or returns in response to requests.
    /// The hashCode() and compareTo() methods for this class assume that this
    /// string value is non-null.  equals() is correctly implemented
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class StringLabel : ValueLabel, IHasOffset
    {
        private string str;

        /// <summary>Start position of the word in the original input string</summary>
        private int _beginPosition = -1;

        /// <summary>End position of the word in the original input string</summary>
        private int _endPosition = -1;


        /// <summary>
        /// Create a new <code>StringLabel</code> with a null content (i.e., str).
        /// </summary>
        public StringLabel()
        {
        }

        /// <summary>
        /// Create a new <code>StringLabel</code> with the given content.
        /// </summary>
        /// <param name="str">The new label's content</param>
        public StringLabel(string str)
        {
            this.str = str;
        }

        /// <summary>
        /// Create a new <code>StringLabel</code> with the given content.
        /// </summary>
        /// <param name="str">The new label's content</param>
        /// <param name="beginPosition">Start offset in original text</param>
        /// <param name="endPosition">End offset in original text</param>
        public StringLabel(string str, int beginPosition, int endPosition)
        {
            this.str = str;
            SetBeginPosition(beginPosition);
            SetEndPosition(endPosition);
        }

        /// <summary>
        /// Create a new <code>StringLabel</code> with the
        /// <code>value()</code> of another label as its label.
        /// </summary>
        /// <param name="label">The other label</param>
        public StringLabel(ILabel label)
        {
            this.str = label.Value();
            if (label is IHasOffset)
            {
                var ofs = (IHasOffset) label;
                SetBeginPosition(ofs.BeginPosition());
                SetEndPosition(ofs.EndPosition());
            }
        }

        /// <summary>
        /// Return the word value of the label (or null if none).
        /// </summary>
        public override string Value()
        {
            return str;
        }

        /// <summary>
        /// Set the value for the label.
        /// </summary>
        public override void SetValue(string value)
        {
            str = value;
        }

        /// <summary>
        /// Set the label from a string.
        /// </summary>
        public override void SetFromString(string str)
        {
            this.str = str;
        }

        public override string ToString()
        {
            return str;
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class StringLabelFactoryHolder
        {
            public static readonly ILabelFactory lf = new StringLabelFactory();
        }

        /// <summary>
        /// Return a factory for this kind of label (i.e., <code>StringLabel</code>).
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>The label factory</returns>
        public override ILabelFactory LabelFactory()
        {
            return StringLabelFactoryHolder.lf;
        }

        /// <summary>
        /// Return a factory for this kind of label.
        /// </summary>
        /// <returns>The label factory</returns>
        public static ILabelFactory Factory()
        {
            return StringLabelFactoryHolder.lf;
        }

        public int BeginPosition()
        {
            return _beginPosition;
        }

        public int EndPosition()
        {
            return _endPosition;
        }

        public void SetBeginPosition(int beginPosition)
        {
            this._beginPosition = beginPosition;
        }

        public void SetEndPosition(int endPosition)
        {
            this._endPosition = endPosition;
        }
    }
}