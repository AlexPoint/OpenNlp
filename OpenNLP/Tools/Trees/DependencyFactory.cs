using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// A factory for dependencies of a certain type.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface IDependencyFactory
    {
        IDependency<ILabel, ILabel, Object> NewDependency(ILabel regent, ILabel dependent);

        IDependency<ILabel, ILabel, Object> NewDependency(ILabel regent, ILabel dependent, Object name);
    }
}