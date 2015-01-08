using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// Scored: This is a simple interface that says that an object can answer
    /// requests for the score, or goodness of the object.
    /// 
    /// JavaNLP includes companion classes {@link ScoredObject} which is a simple
    /// composite of another object and a score, and {@link ScoredComparator}
    /// which compares Scored objects.
    /// 
    /// @author Dan Klein
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IScored
    {
        /// <summary>
        /// Returns the score of this thing.
        /// </summary>
        double Score();
    }
}