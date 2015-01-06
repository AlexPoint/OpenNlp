using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Process
{
    /**
 * Constructs {@link CoreLabel}s from strings optionally with
 * beginning and ending (character after the end) offset positions in
 * an original text.  The makeToken method will put the token in the
 * OriginalTextAnnotation AND TextAnnotation keys (2 places!),
 * and optionally records
 * begin and position after offsets in BeginPositionAnnotation and
 * EndPositionAnnotation.  If the tokens are built in PTBTokenizer with
 * an "invertible" tokenizer, you will also get a BeforeAnnotation and for
 * the last token an AfterAnnotation.You can also get an empty CoreLabel token
 *
 * @author Anna Rafferty
 * @author Sonal Gupta (now implements CoreTokenFactory, you can make tokens using many options)
 */

    public class CoreLabelTokenFactory : CoreTokenFactory<CoreLabel>, LexedTokenFactory<CoreLabel> /*, Serializable*/
    {
        private readonly bool addIndices;

        /**
   * Constructor for a new token factory which will add in the word, the
   * "current" annotation, and the begin/end position annotations.
   */

        public CoreLabelTokenFactory() : this(true)
        {
        }

        /**
   * Constructor that allows one to choose if index annotation
   * indicating begin/end position will be included in the label.
   *
   * @param addIndices if true, begin and end position annotations will be included (this is the default)
   */

        public CoreLabelTokenFactory(bool addIndices) : base()
        {
            this.addIndices = addIndices;
        }

        /**
   * Constructs a CoreLabel as a string with a corresponding BEGIN and END position.
   * (Does not take substring).
   */

        public CoreLabel MakeToken(string tokenText, int begin, int length)
        {
            return MakeToken(tokenText, tokenText, begin, length);
        }

        /**
   * Constructs a CoreLabel as a string with a corresponding BEGIN and END position, 
   * when the original OriginalTextAnnotation is different from TextAnnotation
   * (Does not take substring).
   */

        public CoreLabel MakeToken(string tokenText, string originalText, int begin, int length)
        {
            CoreLabel cl = addIndices ? new CoreLabel(5) : new CoreLabel();
            cl.SetValue(tokenText);
            cl.SetWord(tokenText);
            cl.SetOriginalText(originalText);
            if (addIndices)
            {
                cl.Set(typeof (CoreAnnotations.CharacterOffsetBeginAnnotation), begin);
                cl.Set(typeof (CoreAnnotations.CharacterOffsetEndAnnotation), begin + length);
            }
            return cl;
        }

        public CoreLabel MakeToken()
        {
            var l = new CoreLabel();
            return l;
        }

        /*public CoreLabel makeToken(string[] keys, string[] values) {
    CoreLabel l = new CoreLabel(keys, values);
    return l;
  }*/

        public CoreLabel MakeToken(CoreLabel labelToBeCopied)
        {
            var l = new CoreLabel(labelToBeCopied);
            return l;
        }

        private static readonly long serialVersionUID = 4L;
    }
}