using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * A factory for dependencies of a certain type.
 *
 * @author Christopher Manning
 */

    public interface DependencyFactory
    {
        Dependency<Label, Label, Object> newDependency(Label regent, Label dependent);

        Dependency<Label, Label, Object> newDependency(Label regent, Label dependent, Object name);
    }
}