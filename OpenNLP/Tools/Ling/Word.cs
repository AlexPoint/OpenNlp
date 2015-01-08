using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// A <code>Word</code> object acts as a Label by containing a string.
    /// This class is in essence identical to a <code>StringLabel</code>, but
    /// it also uses the value to implement the <code>HasWord</code> interface.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class Word : StringLabel, IHasWord
    {
        /// <summary>string representation of an empty</summary>
        public static readonly string EmptyString = "*t*";

        /// <summary>Word representation of an empty</summary>
        public static readonly Word Empty = new Word(EmptyString);
        
        /// <summary>
        /// Construct a new word with a <code>null</code> value.
        /// </summary>
        public Word() : base()
        {
        }
        
        /// <summary>
        /// Construct a new word, with the given value.
        /// </summary>
        /// <param name="word">string value of the Word</param>
        public Word(string word) : base(word)
        {
        }

        /// <summary>
        /// Construct a new word, with the given value.
        /// </summary>
        /// <param name="word">string value of the Word</param>
        public Word(string word, int beginPosition, int endPosition) :
            base(word, beginPosition, endPosition)
        {
        }

        /// <summary>
        /// Creates a new word whose word value is the value of any 
        /// class that supports the <code>Label</code> interface.
        /// </summary>
        /// <param name="lab">The label to be used as the basis of the new Word</param>
        public Word(ILabel lab) : base(lab)
        {
        }

        public string GetWord()
        {
            return Value();
        }

        public void SetWord(string word)
        {
            SetValue(word);
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class WordFactoryHolder
        {
            public static readonly ILabelFactory lf = new WordFactory();
        }

        /// <summary>
        /// Return a factory for this kind of label (i.e., {@code Word}).
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>The label factory</returns>
        public override ILabelFactory LabelFactory()
        {
            return WordFactoryHolder.lf;
        }

        /// <summary>
        /// Return a factory for this kind of label.
        /// </summary>
        /// <returns>The label factory</returns>
        public new static ILabelFactory Factory()
        {
            return WordFactoryHolder.lf;
        }

    }
}