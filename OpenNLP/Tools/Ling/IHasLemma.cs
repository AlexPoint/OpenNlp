using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// Something that implements the <code>HasLemma</code> interface knows about lemmas.
    /// 
    /// @author John Bauer
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IHasLemma
    {
        /// <summary>
        /// Return the lemma value of the label (or null if none).
        /// </summary>
        string Lemma();

        /// <summary>
        /// Set the lemma value for the label (if one is stored).
        /// </summary>
        void SetLemma(string lemma);

    }
}