using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// Something that implements the <code>HasOffset</code> interface
    /// bears a offset reference to the original text
    /// 
    /// @author Richard Eckart (Technische Universitat Darmstadt)
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IHasOffset
    {
        /// <summary>
        /// Return the beginning character offset of the label (or -1 if none).
        /// </summary>
        int BeginPosition();

        /// <summary>
        /// Set the beginning character offset for the label.
        /// Setting this key to "-1" can be used to indicate no valid value.
        /// </summary>
        void SetBeginPosition(int beginPos);

        /// <summary>
        /// Return the ending character offset of the label (or -1 if none).
        /// </summary>
        int EndPosition();

        /// <summary>
        /// Set the ending character offset of the label (or -1 if none).
        /// </summary>
        void SetEndPosition(int endPos);

    }
}