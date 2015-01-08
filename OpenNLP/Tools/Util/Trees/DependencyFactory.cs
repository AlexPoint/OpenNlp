using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A factory for dependencies of a certain type.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public interface IDependencyFactory
    {
        IDependency<ILabel, ILabel, Object> NewDependency(ILabel regent, ILabel dependent);

        IDependency<ILabel, ILabel, Object> NewDependency(ILabel regent, ILabel dependent, Object name);
    }
}