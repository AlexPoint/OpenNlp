using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// A <code>TaggedWordFactory</code> acts as a factory for creating objects of
    /// class <code>TaggedWord</code>.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TaggedWordFactory : ILabelFactory
    {
        public static readonly int TAG_LABEL = 2;

        private readonly char divider;

        /// <summary>
        /// Create a new <code>TaggedWordFactory</code>. The divider will be taken as '/'.
        /// </summary>
        public TaggedWordFactory() :
            this('/')
        {
        }

        /// <summary>
        /// Create a new <code>TaggedWordFactory</code>.
        /// </summary>
        /// <param name="divider">
        /// This character will be used in calls to the one argument version 
        /// of <code>newLabel()</code>, to divide the word from the tag.
        /// Stuff after the last instance of this character will become the tag, 
        /// and stuff before it will become the label.
        /// </param>
        public TaggedWordFactory(char divider)
        {
            this.divider = divider;
        }

        /// <summary>
        /// Make a new label with this <code>string</code> as the value (word).
        /// Any other fields of the label would normally be null.
        /// </summary>
        /// <param name="labelStr">The string that will be used for value</param>
        /// <returns>The new TaggedWord (tag will be <code>null</code>)</returns>
        public ILabel NewLabel(string labelStr)
        {
            return new TaggedWord(labelStr);
        }
        
        /// <summary>
        /// Make a new label with this <code>string</code> as a value component.
        /// Any other fields of the label would normally be null.
        /// </summary>
        /// <param name="labelStr">The string that will be used for value</param>
        /// <param name="options">what to make (use labelStr as word or tag)</param>
        /// <returns>The new TaggedWord (tag or word will be <code>null</code>)</returns>
        public ILabel NewLabel(string labelStr, int options)
        {
            if (options == TAG_LABEL)
            {
                return new TaggedWord(null, labelStr);
            }
            return new TaggedWord(labelStr);
        }

        /// <summary>
        /// Create a new word, where the label is formed from the <code>string</code> passed in.
        /// The string is divided according to the divider character.
        /// We assume that we can always just divide on the rightmost divider character,
        /// rather than trying to parse up escape sequences.
        /// If the divider character isn't found in the word, 
        /// then the whole string becomes the word, and the tag is <code>null</code>.
        /// </summary>
        /// <param name="word">The word that will go into the <code>Word</code></param>
        /// <returns>The new TaggedWord</returns>
        public ILabel NewLabelFromString(string word)
        {
            int where = word.LastIndexOf(divider);
            if (where >= 0)
            {
                return new TaggedWord(word.Substring(0, where), word.Substring(where + 1));
            }
            else
            {
                return new TaggedWord(word);
            }
        }

        /// <summary>
        /// Create a new <code>TaggedWord Label</code>, where the label is formed from
        /// the <code>Label</code> object passed in.
        /// Depending on what fields each label has, other things will be <code>null</code>.
        /// </summary>
        /// <param name="oldLabel">The Label that the new label is being created from</param>
        /// <returns>a new label of a particular type</returns>
        public ILabel NewLabel(ILabel oldLabel)
        {
            return new TaggedWord(oldLabel);
        }
    }
}