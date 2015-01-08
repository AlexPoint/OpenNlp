using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A <code>ConstituentFactory</code> is a factory for creating objects
    /// of class <code>Constituent</code>, or some descendent class.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IConstituentFactory
    {
        /// <summary>
        /// Build a constituent with this start and end.
        /// </summary>
        Constituent NewConstituent(int start, int end);

        /// <summary>
        /// Build a constituent with this start and end.
        /// </summary>
        Constituent NewConstituent(int start, int end, ILabel label, double score);
    }
}