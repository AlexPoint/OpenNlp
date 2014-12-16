using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public interface CoreMap
    {
        /** Attempt to provide a briefer and more human readable String for the contents of
   *  a CoreMap.
   *  The method may not be capable of printing circular dependencies in CoreMaps.
   *
   *  @param what An array (varargs) of Strings that say what annotation keys
   *     to print.  These need to be provided in a shortened form where you
   *     are just giving the part of the class name without package and up to
   *     "Annotation". That is,
   *     edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation --&gt; PartOfSpeech
   *     . As a special case, an empty array means to print everything, not nothing.
   *  @return A more human readable String giving possibly partial contents of a
   *     CoreMap.
   */

  String toShorterString(List<String> what);
    }
}
