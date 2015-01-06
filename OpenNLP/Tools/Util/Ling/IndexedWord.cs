using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * This class is mainly for use with RTE in terms of the methods it provides,
 * but on a more general level, it provides a {@link CoreLabel} that uses its
 * DocIDAnnotation, SentenceIndexAnnotation, and IndexAnnotation to implement
 * Comparable/compareTo, hashCode, and equals.  This means no other annotations,
 * including the identity of the word, are taken into account when using these
 * methods.
 * <br>
 * The actual implementation is to wrap a <code>CoreLabel<code/>.
 * This avoids breaking the <code>equals()</code> and
 * <code>hashCode()</code> contract and also avoids expensive copying
 * when used to represent the same data as the original
 * <code>CoreLabel</code>.
 *
 * @author rafferty
 *
 */

    public class IndexedWord : AbstractCoreLabel, IComparable<IndexedWord>
    {
        private static readonly long serialVersionUID = 3739633991145239829L;

        /**
   * The identifier that points to no word.
   */
        public static readonly IndexedWord NO_WORD = new IndexedWord(null, -1, -1);

        //private readonly CoreLabel label;
        private readonly CoreLabel label;

        /**
   * Default constructor; uses {@link CoreLabel} default constructor
   */

        public IndexedWord()
        {
            label = new CoreLabel();
        }


        /**
   * Copy Constructor - relies on {@link CoreLabel} copy constructor
   * It will set the value, and if the word is not set otherwise, set
   * the word to the value.
   *
   * @param w A Label to initialize this IndexedWord from
   */

        public IndexedWord(Label w)
        {
            if (w is CoreLabel)
            {
                this.label = (CoreLabel) w;
            }
            else
            {
                label = new CoreLabel(w);
                if (label.GetWord() == null)
                {
                    label.SetWord(label.Value());
                }
            }
        }

        /**
   * Construct an IndexedWord from a CoreLabel just as for a CoreMap.
   * <i>Implementation note:</i> this is a the same as the constructor
   * that takes a CoreMap, but is needed to ensure unique most specific
   * type inference for selecting a constructor at compile-time.
   *
   * @param w A Label to initialize this IndexedWord from
   */

        public IndexedWord(CoreLabel w)
        {
            label = w;
        }

        /**
   * Constructor for setting docID, sentenceIndex, and
   * index without any other annotations.
   *
   * @param docID The document ID (arbitrary string)
   * @param sentenceIndex The sentence number in the document (normally 0-based)
   * @param index The index of the word in the sentence (normally 0-based)
   */

        public IndexedWord(string docID, int sentenceIndex, int index)
        {
            label = new CoreLabel();
            label.Set(typeof (CoreAnnotations.DocIDAnnotation), docID);
            label.Set(typeof (CoreAnnotations.SentenceIndexAnnotation), sentenceIndex);
            label.Set(typeof (CoreAnnotations.IndexAnnotation), index);
        }

        public IndexedWord MakeCopy(int count)
        {
            var labelCopy = new CoreLabel(label);
            var copy = new IndexedWord(labelCopy);
            copy.SetCopyCount(count);
            return copy;
        }

        /**
   * TODO: would be nice to get rid of this.  Only used in two places in RTE.  
   */
        //public CoreLabel backingLabel() { return label; }

        public /*<VALUE> VALUE*/ object Get( /*Class<? extends TypesafeMap.Key<VALUE>>*/ Type key)
        {
            return label.Get(key);
        }

        public /*<VALUE>*/ bool Has( /*Class<? extends TypesafeMap.Key<VALUE>>*/ Type key)
        {
            return label.Has(key);
        }

        public /*<VALUE>*/ bool ContainsKey( /*Class<? extends TypesafeMap.Key<VALUE>>*/ Type key)
        {
            return label.ContainsKey(key);
        }

        public /*<VALUE> VALUE*/ object Set( /*Class<? extends TypesafeMap.Key<VALUE>>*/ Type key, /*VALUE*/object value)
        {
            return label.Set(key, value);
        }

        public /*<KEY extends TypesafeMap.Key<string>>*/ string GetString( /*Class<KEY>*/ Type key)
        {
            return label.GetString(key);
        }

        public /*<VALUE> VALUE*/ object Remove( /*Class<? extends Key<VALUE>>*/ Type key)
        {
            return label.Remove(key);
        }

        public Set< /*Class<?>*/ Type> KeySet()
        {
            return label.KeySet();
        }

        public int Size()
        {
            return label.Size();
        }

        //@Override
        public string Value()
        {
            return label.Value();
        }

        //@Override
        public void SetValue(string value)
        {
            label.SetValue(value);
        }

        //@Override
        public string Tag()
        {
            return label.Tag();
        }

        //@Override
        public void SetTag(string tag)
        {
            label.SetTag(tag);
        }

        //@Override
        public string GetWord()
        {
            return label.GetWord();
        }

        //@Override
        public void SetWord(string word)
        {
            label.SetWord(word);
        }

        //@Override
        public string Lemma()
        {
            return label.Lemma();
        }

        //@Override
        public void SetLemma(string lemma)
        {
            label.SetLemma(lemma);
        }

        //@Override
        public string Ner()
        {
            return label.Ner();
        }

        //@Override
        public void SetNER(string ner)
        {
            label.SetNER(ner);
        }

        //@Override
        public string DocID()
        {
            return label.DocID();
        }

        //@Override
        public void SetDocID(string docID)
        {
            label.SetDocID(docID);
        }

        //@Override
        public int Index()
        {
            return label.Index();
        }

        //@Override
        public void SetIndex(int index)
        {
            label.SetIndex(index);
        }

        //@Override
        public int SentIndex()
        {
            return label.SentIndex();
        }

        //@Override
        public void SetSentIndex(int sentIndex)
        {
            label.SetSentIndex(sentIndex);
        }

        //@Override
        public string OriginalText()
        {
            return label.OriginalText();
        }

        //@Override
        public void SetOriginalText(string originalText)
        {
            label.SetOriginalText(originalText);
        }

        //@Override
        public int BeginPosition()
        {
            return label.BeginPosition();
        }

        //@Override
        public int EndPosition()
        {
            return label.EndPosition();
        }

        //@Override
        public void SetBeginPosition(int beginPos)
        {
            label.SetBeginPosition(beginPos);
        }

        //@Override
        public void SetEndPosition(int endPos)
        {
            label.SetEndPosition(endPos);
        }

        public int CopyCount()
        {
            return label.CopyCount();
        }

        public void SetCopyCount(int count)
        {
            label.SetCopyCount(count);
        }

        public string ToPrimes()
        {
            int copy = label.CopyCount();
            return StringUtils.Repeat('\'', copy);
        }

        /**
   * This .equals is dependent only on docID, sentenceIndex, and index.
   * It doesn't consider the actual word value, but assumes that it is
   * validly represented by token position.
   * All IndexedWords that lack these fields will be regarded as equal.
   */
        //@Override
        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (!(o is IndexedWord)) return false;

            //now compare on appropriate keys
            var otherWord = (IndexedWord) o;
            var myInd = Get(typeof (CoreAnnotations.IndexAnnotation));
            var otherInd = otherWord.Get(typeof (CoreAnnotations.IndexAnnotation));
            if (myInd == null)
            {
                if (otherInd != null)
                    return false;
            }
            else if (! ((int) myInd).Equals((int) otherInd))
            {
                return false;
            }
            var mySentInd = Get(typeof (CoreAnnotations.SentenceIndexAnnotation));
            var otherSentInd = otherWord.Get(typeof (CoreAnnotations.SentenceIndexAnnotation));
            if (mySentInd == null)
            {
                if (otherSentInd != null)
                    return false;
            }
            else if (! ((int) mySentInd).Equals((int) otherSentInd))
            {
                return false;
            }
            string myDocID = GetString(typeof (CoreAnnotations.DocIDAnnotation));
            string otherDocID = otherWord.GetString(typeof (CoreAnnotations.DocIDAnnotation));
            if (myDocID == null)
            {
                if (otherDocID != null)
                    return false;
            }
            else if (! myDocID.Equals(otherDocID))
            {
                return false;
            }
            if (CopyCount() != otherWord.CopyCount())
            {
                return false;
            }
            return true;
        }


        /**
   * This hashCode uses only the docID, sentenceIndex, and index.
   * See compareTo for more info.
   */
        //@Override
        public override int GetHashCode()
        {
            bool sensible = false;
            int result = 0;
            if (Get(typeof (CoreAnnotations.DocIDAnnotation)) != null)
            {
                result = Get(typeof (CoreAnnotations.DocIDAnnotation)).GetHashCode();
                sensible = true;
            }
            if (Has(typeof (CoreAnnotations.SentenceIndexAnnotation)))
            {
                result = 29*result + Get(typeof (CoreAnnotations.SentenceIndexAnnotation)).GetHashCode();
                sensible = true;
            }
            if (Has(typeof (CoreAnnotations.IndexAnnotation)))
            {
                result = 29*result + Get(typeof (CoreAnnotations.IndexAnnotation)).GetHashCode();
                sensible = true;
            }
            if (! sensible)
            {
                //System.err.println("WARNING!!!  You have hashed an IndexedWord with no docID, sentIndex or wordIndex. You will almost certainly lose");
            }
            return result;
        }

        /**
   * NOTE: This compareTo is based on and made to be compatible with the one
   * from IndexedFeatureLabel.  You <em>must</em> have a DocIDAnnotation,
   * SentenceIndexAnnotation, and IndexAnnotation for this to make sense and
   * be guaranteed to work properly. Currently, it won't error out and will
   * try to return something sensible if these are not defined, but that really
   * isn't proper usage!
   *
   * This compareTo method is based not by value elements like the word(),
   *  but on passage position. It puts NO_WORD elements first, and then orders
   *  by document, sentence, and word index.  If these do not differ, it
   *  returns equal.
   *
   *  @param w The IndexedWord to compare with
   *  @return Whether this is less than w or not in the ordering
   */

        public int CompareTo(IndexedWord w)
        {
            if (this.Equals(IndexedWord.NO_WORD))
            {
                if (w.Equals(IndexedWord.NO_WORD))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            if (w.Equals(IndexedWord.NO_WORD))
            {
                return 1;
            }

            string docID = this.GetString(typeof (CoreAnnotations.DocIDAnnotation));
            int docComp = docID.CompareTo(w.GetString(typeof (CoreAnnotations.DocIDAnnotation)));
            if (docComp != 0) return docComp;

            int sentComp = SentIndex() - w.SentIndex();
            if (sentComp != 0) return sentComp;

            int indexComp = Index() - w.Index();
            if (indexComp != 0) return indexComp;

            return CopyCount() - w.CopyCount();
        }

        /**
   * Returns the value-tag of this label.
   */
        //@Override
        public override string ToString()
        {
            return label.ToString(CoreLabel.OutputFormat.VALUE_TAG);
            //return label.ToString();
        }

        public string ToString(CoreLabel.OutputFormat format)
        {
            return label.ToString(format);
            //return label.ToString();
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void SetFromString(string labelStr)
        {
            throw new InvalidOperationException("Cannot set from string");
        }


        public class LabFact : LabelFactory
        {
            public Label NewLabel(string labelStr)
            {
                var label = new CoreLabel();
                label.SetValue(labelStr);
                return new IndexedWord(label);
            }

            public Label NewLabel(string labelStr, int options)
            {
                return NewLabel(labelStr);
            }

            public Label NewLabelFromString(string encodedLabelStr)
            {
                throw new InvalidOperationException("This code branch left blank" +
                                                    " because we do not understand what this method should do.");
            }

            public Label NewLabel(Label oldLabel)
            {
                return new IndexedWord(oldLabel);
            }
        }

        /*public static LabelFactory factory() {
    return new LabelFactory() {

      public Label newLabel(string labelStr) {
        CoreLabel label = new CoreLabel();
        label.setValue(labelStr);
        return new IndexedWord(label);
      }

      public Label newLabel(string labelStr, int options) {
        return newLabel(labelStr);
      }

      public Label newLabel(Label oldLabel) {
        return new IndexedWord(oldLabel);
      }

      public Label newLabelFromString(string encodedLabelStr) {
        throw new UnsupportedOperationException("This code branch left blank" +
        " because we do not understand what this method should do.");
      }
    };
  }*/

        public static LabelFactory Factory()
        {
            return new LabFact();
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public LabelFactory LabelFactory()
        {
            return IndexedWord.Factory();
        }
    }
}