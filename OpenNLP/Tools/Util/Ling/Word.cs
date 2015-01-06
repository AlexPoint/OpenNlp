using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A <code>Word</code> object acts as a Label by containing a string.
 * This class is in essence identical to a <code>StringLabel</code>, but
 * it also uses the value to implement the <code>HasWord</code> interface.
 *
 * @author Christopher Manning
 * @version 2000/12/20
 */

    public class Word : StringLabel, HasWord
    {
        /**
   * string representation of an empty.
   */
        public static readonly string EMPTY_STRING = "*t*";

        /**
   * Word representation of an empty.
   */
        public static readonly Word EMPTY = new Word(EMPTY_STRING);

        /**
   * Construct a new word with a <code>null</code> value.
   */

        public Word() : base()
        {
        }

        /**
   * Construct a new word, with the given value.
   *
   * @param word string value of the Word
   */

        public Word(string word) : base(word)
        {
        }

        /**
   * Construct a new word, with the given value.
   *
   * @param word string value of the Word
   */

        public Word(string word, int beginPosition, int endPosition) :
            base(word, beginPosition, endPosition)
        {
        }


        /**
   * Creates a new word whose word value is the value of any
   * class that supports the <code>Label</code> interface.
   *
   * @param lab The label to be used as the basis of the new Word
   */

        public Word(Label lab) : base(lab)
        {
        }


        //@Override
        public string word()
        {
            return value();
        }


        //@Override
        public void setWord(string word)
        {
            setValue(word);
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class WordFactoryHolder
        {

            public static readonly LabelFactory lf = new WordFactory();

            //private WordFactoryHolder() { }

        }

        /**
   * Return a factory for this kind of label (i.e., {@code Word}).
   * The factory returned is always the same one (a singleton).
   *
   * @return The label factory
   */
        //@Override
        public override LabelFactory labelFactory()
        {
            return WordFactoryHolder.lf;
        }


        /**
   * Return a factory for this kind of label.
   *
   * @return The label factory
   */

        public new static LabelFactory factory()
        {
            return WordFactoryHolder.lf;
        }

        private static readonly long serialVersionUID = -4817252915997034058L;
    }
}