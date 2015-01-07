using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /// <summary>
    /// A <code>WordFactory</code> acts as a factory for creating objects of class <code>Word</code>.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public class WordFactory : LabelFactory
    {
        
        /// <summary>
        /// Create a new word, where the label is formed from the <code>string</code> passed in.
        /// </summary>
        /// <param name="word">The word that will go into the <code>Word</code></param>
        /// <returns>The new label</returns>
        public Label NewLabel(string word)
        {
            return new Word(word);
        }

        /// <summary>
        /// Create a new word, where the label is formed from the <code>string</code> passed in.
        /// </summary>
        /// <param name="word">The word that will go into the <code>Word</code></param>
        /// <param name="options">is ignored by a WordFactory</param>
        /// <returns>The new label</returns>
        public Label NewLabel(string word, int options)
        {
            return new Word(word);
        }

        /// <summary>
        /// Create a new word, where the label is formed from the <code>string</code> passed in.
        /// </summary>
        /// <param name="word">The word that will go into the <code>Word</code></param>
        /// <returns>The new label</returns>
        public Label NewLabelFromString(string word)
        {
            return new Word(word);
        }

        /// <summary>
        /// Create a new <code>Word Label</code>, where the label is formed from
        /// the <code>Label</code> object passed in.
        /// Depending on what fields each label has, other things will be <code>null</code>.
        /// </summary>
        /// <param name="oldLabel">The Label that the new label is being created from</param>
        /// <returns>a new label of a particular type</returns>
        public Label NewLabel(Label oldLabel)
        {
            return new Word(oldLabel);
        }
    }
}