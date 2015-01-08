using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A <code>ConstituentFactory</code> is a factory for creating objects
    /// of class <code>Constituent</code>, or some descendent class.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
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