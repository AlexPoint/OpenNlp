using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A <code>LabeledWord</code> object contains a word and its tag.
 * The <code>value()</code> of a TaggedWord is the Word.  The tag
 * is, and is a Label instead of a String
 */

    public class LabeledWord : Word
    {
        private Label vTag;

        private static readonly string DIVIDER = "/";

        /**
   * Create a new <code>TaggedWord</code>.
   * It will have <code>null</code> for its content fields.
   */

        public LabeledWord() : base()
        {
        }

        /**
   * Create a new <code>TaggedWord</code>.
   *
   * @param word The word, which will have a <code>null</code> tag
   */

        public LabeledWord(string word) :
            base(word)
        {
        }

        /**
   * Create a new <code>TaggedWord</code>.
   *
   * @param word The word
   * @param tag  The tag
   */

        public LabeledWord(string word, Label tag) :
            base(word)
        {
            this.vTag = tag;
        }

        public LabeledWord(Label word, Label tag) :
            base(word)
        {
            this.vTag = tag;
        }

        public Label tag()
        {
            return vTag;
        }

        public void setTag(Label tag)
        {
            this.vTag = tag;
        }

        //@Override
        public override string ToString()
        {
            return ToString(DIVIDER);
        }

        public string ToString(string divider)
        {
            return word() + divider + vTag;
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class LabelFactoryHolder
        {

            //private LabelFactoryHolder() {}

            public static readonly LabelFactory lf = new TaggedWordFactory();

        }

        /**
   * Return a factory for this kind of label
   * (i.e., <code>TaggedWord</code>).
   * The factory returned is always the same one (a singleton).
   *
   * @return The label factory
   */
        //@Override
        public override LabelFactory labelFactory()
        {
            return LabelFactoryHolder.lf;
        }


        /**
   * Return a factory for this kind of label.
   *
   * @return The label factory
   */

        public new static LabelFactory factory()
        {
            return LabelFactoryHolder.lf;
        }

        private static readonly long serialVersionUID = -7252006452127051085L;
    }
}