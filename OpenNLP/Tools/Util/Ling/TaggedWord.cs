using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A <code>TaggedWord</code> object contains a word and its tag.
 * The <code>value()</code> of a TaggedWord is the Word.  The tag
 * is secondary.
 *
 * @author Christopher Manning
 */

    public class TaggedWord : Word, HasTag
    {
        private string vTag;

        private const string Divider = "/";

        /**
   * Create a new <code>TaggedWord</code>.
   * It will have <code>null</code> for its content fields.
   */

        public TaggedWord() : base()
        {
        }

        /**
   * Create a new <code>TaggedWord</code>.
   *
   * @param word The word, which will have a <code>null</code> tag
   */

        public TaggedWord(string word) : base(word)
        {
        }

        /**
   * Create a new <code>TaggedWord</code>.
   *
   * @param word The word
   * @param tag  The tag
   */

        public TaggedWord(string word, string vTag) :
            base(word)
        {
            this.vTag = vTag;
        }

        /**
   * Create a new <code>TaggedWord</code>.
   *
   * @param oldLabel A Label.  If it implements the HasWord and/or
   *                 HasTag interface, then the corresponding value will be set
   */

        public TaggedWord(Label oldLabel) : base(oldLabel.Value())
        {
            if (oldLabel is HasTag)
            {
                this.vTag = ((HasTag) oldLabel).Tag();
            }
        }

        /**
   * Create a new <code>TaggedWord</code>.
   *
   * @param word This word is passed to the supertype constructor
   * @param tag  The <code>value()</code> of this label is set as the
   *             tag of this Label
   */

        public TaggedWord(Label word, Label tag) : base(word)
        {
            this.vTag = tag.Value();
        }

        public string Tag()
        {
            return vTag;
        }

        public void SetTag(string tag)
        {
            this.vTag = tag;
        }

        //@Override
        public override string ToString()
        {
            return ToString(Divider);
        }

        public string ToString(string divider)
        {
            return GetWord() + divider + vTag;
        }


        /**
   * Sets a TaggedWord from decoding
   * the <code>string</code> passed in.  The string is divided according
   * to the divider character (usually, "/").  We assume that we can
   * always just
   * divide on the rightmost divider character, rather than trying to
   * parse up escape sequences.  If the divider character isn't found
   * in the word, then the whole string becomes the word, and the tag
   * is <code>null</code>.
   *
   * @param taggedWord The word that will go into the <code>Word</code>
   */

        public override void SetFromString(string taggedWord)
        {
            SetFromString(taggedWord, Divider);
        }

        public void SetFromString(string taggedWord, string divider)
        {
            int where = taggedWord.LastIndexOf(divider);
            if (where >= 0)
            {
                SetWord(taggedWord.Substring(0, where));
                SetTag(taggedWord.Substring(where + 1));
            }
            else
            {
                SetWord(taggedWord);
                SetTag(null);
            }
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
        public override LabelFactory LabelFactory()
        {
            return LabelFactoryHolder.lf;
        }


        /**
   * Return a factory for this kind of label.
   *
   * @return The label factory
   */

        public new static LabelFactory Factory()
        {
            return LabelFactoryHolder.lf;
        }

        private static readonly long serialVersionUID = -7252006452127051085L;
    }
}