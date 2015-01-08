using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// A <code>TaggedWord</code> object contains a word and its tag.
    /// The <code>value()</code> of a TaggedWord is the Word.
    /// The tag is secondary.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TaggedWord : Word, IHasTag
    {
        private string vTag;

        private const string Divider = "/";

        /// <summary>
        /// Create a new <code>TaggedWord</code>. It will have <code>null</code> for its content fields.
        /// </summary>
        public TaggedWord() : base()
        {
        }

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// </summary>
        /// <param name="word">The word, which will have a <code>null</code> tag</param>
        public TaggedWord(string word) : base(word)
        {
        }

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// </summary>
        public TaggedWord(string word, string vTag) :
            base(word)
        {
            this.vTag = vTag;
        }

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// </summary>
        /// <param name="oldLabel">
        /// A Label.  If it implements the HasWord and/or HasTag interface, then the corresponding value will be set
        /// </param>
        public TaggedWord(ILabel oldLabel) : base(oldLabel.Value())
        {
            if (oldLabel is IHasTag)
            {
                this.vTag = ((IHasTag) oldLabel).Tag();
            }
        }

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// </summary>
        /// <param name="word">This word is passed to the supertype constructor</param>
        /// <param name="tag">The <code>value()</code> of this label is set as the tag of this Label</param>
        public TaggedWord(ILabel word, ILabel tag) : base(word)
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

        public override string ToString()
        {
            return ToString(Divider);
        }

        public string ToString(string divider)
        {
            return GetWord() + divider + vTag;
        }

        /// <summary>
        /// Sets a TaggedWord from decoding the <code>string</code> passed in.
        /// The string is divided according to the divider character (usually, "/").
        /// We assume that we can always just divide on the rightmost divider character,
        /// rather than trying to parse up escape sequences.
        /// If the divider character isn't found in the word, 
        /// then the whole string becomes the word, and the tag is <code>null</code>.
        /// </summary>
        /// <param name="taggedWord">The word that will go into the <code>Word</code></param>
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
            public static readonly ILabelFactory lf = new TaggedWordFactory();

        }

        /// <summary>
        /// Return a factory for this kind of label (i.e., <code>TaggedWord</code>).
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>The label factory</returns>
        public override ILabelFactory LabelFactory()
        {
            return LabelFactoryHolder.lf;
        }

        /// <summary>
        /// Return a factory for this kind of label.
        /// </summary>
        /// <returns>The label factory</returns>
        public new static ILabelFactory Factory()
        {
            return LabelFactoryHolder.lf;
        }
    }
}