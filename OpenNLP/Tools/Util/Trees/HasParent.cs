using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * Only to be implemented by Tree subclasses that actualy keep their
 * parent pointers.  For example, the base Tree class should
 * <b>not</b> implement this, but TreeGraphNode should.
 *
 * @author John Bauer
 */
    public interface HasParent
    {
        Tree parent();
    }
}
