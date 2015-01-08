using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    
    /// <summary>
    /// The base class for any annotation that can be marked on a {@link CoreMap},
    /// parameterized by the type of the value associated with the annotation.
    /// Subclasses of this class are the keys in the {@link CoreMap}, so they are
    /// instantiated only by utility methods in {@link CoreAnnotations}.
    /// 
    /// @author dramage
    /// @author rafferty
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ICoreAnnotation<T> /*:Key<T>*/
    {
        /// <summary>
        /// Returns the type associated with this annotation.
        /// This method must return the same class type as its value type parameter.
        /// It feels like one should be able to get away without this method, but because Java
        /// erases the generic type signature, that info disappears at runtime.
        /// </summary>
        Type GetAnnotationType();
    }
}