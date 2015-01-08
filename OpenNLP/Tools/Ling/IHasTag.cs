using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// Something that implements the <code>HasTag</code> interface knows about part-of-speech tags.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IHasTag
    {
        /// <summary>
        /// Return the tag value of the label (or null if none).
        /// </summary>
        string Tag();

        /// <summary>
        /// Set the tag value for the label (if one is stored).
        /// </summary>
        void SetTag(string tag);
    }
}