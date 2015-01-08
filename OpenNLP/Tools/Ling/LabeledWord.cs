using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// A <code>LabeledWord</code> object contains a word and its tag.
    /// The <code>value()</code> of a TaggedWord is the Word.
    /// The tag is, and is a Label instead of a String
    /// </summary>
    public class LabeledWord : Word
    {
        private ILabel _tag;

        private const string Divider = "/";

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// It will have <code>null</code> for its content fields
        /// </summary>
        public LabeledWord() : base()
        {
        }

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// </summary>
        /// <param name="word">The word, which will have a <code>null</code> tag</param>
        public LabeledWord(string word) :
            base(word)
        {
        }

        /// <summary>
        /// Create a new <code>TaggedWord</code>.
        /// </summary>
        public LabeledWord(string word, ILabel tag) :
            base(word)
        {
            this._tag = tag;
        }

        public LabeledWord(ILabel word, ILabel tag) :
            base(word)
        {
            this._tag = tag;
        }

        public ILabel Tag()
        {
            return _tag;
        }

        public void SetTag(ILabel tag)
        {
            this._tag = tag;
        }

        public override string ToString()
        {
            return ToString(Divider);
        }

        public string ToString(string divider)
        {
            return GetWord() + divider + _tag;
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class LabelFactoryHolder
        {
            public static readonly ILabelFactory lf = new TaggedWordFactory();

        }

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