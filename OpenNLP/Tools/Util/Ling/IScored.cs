using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
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
    /// Code...
    /// </summary>
    public interface IScored
    {
        /// <summary>
        /// Returns the score of this thing.
        /// </summary>
        double Score();
    }
}