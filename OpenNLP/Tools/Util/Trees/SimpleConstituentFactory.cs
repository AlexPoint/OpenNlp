using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A <code>ConstituentFactory</code> acts as a factory for creating objects
    /// of class <code>Constituent</code>, or some descendent class.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public class SimpleConstituentFactory : ConstituentFactory
    {
        public Constituent NewConstituent(int start, int end)
        {
            return new SimpleConstituent(start, end);
        }


        public Constituent NewConstituent(int start, int end, Label label, double score)
        {
            return new SimpleConstituent(start, end);
        }
    }
}