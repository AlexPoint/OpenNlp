using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * The base class for any annotation that can be marked on a {@link CoreMap},
 * parameterized by the type of the value associated with the annotation.
 * Subclasses of this class are the keys in the {@link CoreMap}, so they are
 * instantiated only by utility methods in {@link CoreAnnotations}.
 * 
 * @author dramage
 * @author rafferty
 */
    public interface CoreAnnotation<T>:Key<T>
    {
        /**
   * Returns the type associated with this annotation.  This method must
   * return the same class type as its value type parameter.  It feels like
   * one should be able to get away without this method, but because Java
   * erases the generic type signature, that info disappears at runtime.
   */
        Type getType();
    }
}
