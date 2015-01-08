using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// Something that implements the <code>HasWord</code> interface knows about words.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IHasWord
    {
        /// <summary>
        /// Return the word value of the label (or null if none).
        /// </summary>
        string GetWord();

        /// <summary>
        /// Set the word value for the label (if one is stored).
        /// </summary>
        void SetWord(string word);
    }
}