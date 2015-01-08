using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    
    /// <summary>
    /// Base type for all annotatable core objects. Should usually be instantiated as
    /// {@link ArrayCoreMap}. Many common key definitions live in
    /// {@link edu.stanford.nlp.ling.CoreAnnotations}, but others may be defined elsewhere.
    /// See {@link edu.stanford.nlp.ling.CoreAnnotations} for details.
    /// 
    /// Note that implementations of this interface must take care to implement
    /// equality correctly: by default, two CoreMaps are .equal if they contain the
    /// same keys and all corresponding values are .equal. Subclasses that wish to
    /// change this behavior (such as {@link HashableCoreMap}) must make sure that
    /// all other CoreMap implementations have a special case in their .equals to use
    /// that equality definition when appropriate. Similarly, care must be taken when
    /// defining hashcodes. The default hashcode is 37 * sum of all keys' hashcodes
    /// plus the sum of all values' hashcodes. However, use of this class as HashMap
    /// keys is discouraged because the hashcode can change over time. Consider using
    /// a {@link HashableCoreMap}.
    /// 
    /// @author dramage
    /// @author rafferty
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public interface ICoreMap : ITypesafeMap
    {
        /// <summary>
        /// Attempt to provide a briefer and more human readable string for the contents of a CoreMap.
        /// The method may not be capable of printing circular dependencies in CoreMaps.
        /// </summary>
        /// <param name="what">
        /// An array (varargs) of strings that say what annotation keys to print.
        /// These need to be provided in a shortened form where you are just giving 
        /// the part of the class name without package and up to "Annotation".
        /// That is, edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation --> PartOfSpeech. 
        /// As a special case, an empty array means to print everything, not nothing.
        /// </param>
        /// <returns>A more human readable string giving possibly partial contents of a  CoreMap.</returns>
        string ToShorterString(string[] what);
    }
}