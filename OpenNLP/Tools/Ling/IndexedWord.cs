using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Ling
{
    /// <summary>
    /// This class is mainly for use with RTE in terms of the methods it provides,
    /// but on a more general level, it provides a {@link CoreLabel} that uses its
    /// DocIDAnnotation, SentenceIndexAnnotation, and IndexAnnotation to implement
    /// Comparable/compareTo, hashCode, and equals.  This means no other annotations,
    /// including the identity of the word, are taken into account when using these
    /// methods.
    /// 
    /// The actual implementation is to wrap a <code>CoreLabel</code>.
    /// This avoids breaking the <code>equals()</code> and
    /// <code>hashCode()</code> contract and also avoids expensive copying
    /// when used to represent the same data as the original <code>CoreLabel</code>.
    /// 
    /// @author rafferty
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class IndexedWord : IAbstractCoreLabel, IComparable<IndexedWord>
    {
        /// <summary>
        /// The identifier that points to no word.
        /// </summary>
        public static readonly IndexedWord NoWord = new IndexedWord(null, -1, -1);

        private readonly CoreLabel label;

        
        /// <summary>
        /// Default constructor; uses {@link CoreLabel} default constructor
        /// </summary>
        public IndexedWord()
        {
            label = new CoreLabel();
        }

        /// <summary>
        /// Copy Constructor - relies on {@link CoreLabel} copy constructor.
        /// It will set the value, and if the word is not set otherwise, set
        /// the word to the value.
        /// </summary>
        /// <param name="w">A Label to initialize this IndexedWord from</param>
        public IndexedWord(ILabel w)
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

        /// <summary>
        /// Construct an IndexedWord from a CoreLabel just as for a CoreMap.
        /// <i>Implementation note:</i> this is a the same as the constructor
        /// that takes a CoreMap, but is needed to ensure unique most specific
        /// type inference for selecting a constructor at compile-time.
        /// </summary>
        /// <param name="w">A Label to initialize this IndexedWord from</param>
        public IndexedWord(CoreLabel w)
        {
            label = w;
        }
        
        /// <summary>
        /// Constructor for setting docID, sentenceIndex, and
        /// index without any other annotations.
        /// </summary>
        /// <param name="docId">The document ID (arbitrary string)</param>
        /// <param name="sentenceIndex">The sentence number in the document (normally 0-based)</param>
        /// <param name="index">The index of the word in the sentence (normally 0-based)</param>
        public IndexedWord(string docId, int sentenceIndex, int index)
        {
            label = new CoreLabel();
            label.Set(typeof (CoreAnnotations.DocIdAnnotation), docId);
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

        public string Value()
        {
            return label.Value();
        }

        public void SetValue(string value)
        {
            label.SetValue(value);
        }

        public string Tag()
        {
            return label.Tag();
        }

        public void SetTag(string tag)
        {
            label.SetTag(tag);
        }

        public string GetWord()
        {
            return label.GetWord();
        }

        public void SetWord(string word)
        {
            label.SetWord(word);
        }

        public string Lemma()
        {
            return label.Lemma();
        }

        public void SetLemma(string lemma)
        {
            label.SetLemma(lemma);
        }

        public string Ner()
        {
            return label.Ner();
        }

        public void SetNer(string ner)
        {
            label.SetNer(ner);
        }

        public string DocId()
        {
            return label.DocId();
        }

        public void SetDocId(string docId)
        {
            label.SetDocId(docId);
        }

        public int Index()
        {
            return label.Index();
        }

        public void SetIndex(int index)
        {
            label.SetIndex(index);
        }

        public int SentIndex()
        {
            return label.SentIndex();
        }

        public void SetSentIndex(int sentIndex)
        {
            label.SetSentIndex(sentIndex);
        }

        public string OriginalText()
        {
            return label.OriginalText();
        }

        public void SetOriginalText(string originalText)
        {
            label.SetOriginalText(originalText);
        }

        public int BeginPosition()
        {
            return label.BeginPosition();
        }

        public int EndPosition()
        {
            return label.EndPosition();
        }

        public void SetBeginPosition(int beginPos)
        {
            label.SetBeginPosition(beginPos);
        }

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

        /// <summary>
        /// This .equals is dependent only on docID, sentenceIndex, and index.
        /// It doesn't consider the actual word value, but assumes that it is
        /// validly represented by token position.
        /// All IndexedWords that lack these fields will be regarded as equal.
        /// </summary>
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
            string myDocId = GetString(typeof (CoreAnnotations.DocIdAnnotation));
            string otherDocId = otherWord.GetString(typeof (CoreAnnotations.DocIdAnnotation));
            if (myDocId == null)
            {
                if (otherDocId != null)
                    return false;
            }
            else if (! myDocId.Equals(otherDocId))
            {
                return false;
            }
            if (CopyCount() != otherWord.CopyCount())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This hashCode uses only the docID, sentenceIndex, and index. See compareTo for more info.
        /// </summary>
        public override int GetHashCode()
        {
            bool sensible = false;
            int result = 0;
            if (Get(typeof (CoreAnnotations.DocIdAnnotation)) != null)
            {
                result = Get(typeof (CoreAnnotations.DocIdAnnotation)).GetHashCode();
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
            return result;
        }

        /// <summary>
        /// NOTE: This compareTo is based on and made to be compatible with the one
        /// from IndexedFeatureLabel.  You <em>must</em> have a DocIDAnnotation,
        /// SentenceIndexAnnotation, and IndexAnnotation for this to make sense and
        /// be guaranteed to work properly. Currently, it won't error out and will
        /// try to return something sensible if these are not defined, but that really
        /// isn't proper usage!
        /// 
        /// This compareTo method is based not by value elements like the word(),
        /// but on passage position. It puts NO_WORD elements first, and then orders
        /// by document, sentence, and word index.  If these do not differ, it returns equal.
        /// </summary>
        public int CompareTo(IndexedWord w)
        {
            if (this.Equals(IndexedWord.NoWord))
            {
                if (w.Equals(IndexedWord.NoWord))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            if (w.Equals(IndexedWord.NoWord))
            {
                return 1;
            }

            string docId = this.GetString(typeof (CoreAnnotations.DocIdAnnotation));
            int docComp = docId.CompareTo(w.GetString(typeof (CoreAnnotations.DocIdAnnotation)));
            if (docComp != 0) return docComp;

            int sentComp = SentIndex() - w.SentIndex();
            if (sentComp != 0) return sentComp;

            int indexComp = Index() - w.Index();
            if (indexComp != 0) return indexComp;

            return CopyCount() - w.CopyCount();
        }

        /// <summary>Returns the value-tag of this label.</summary>
        public override string ToString()
        {
            return label.ToString(CoreLabel.OutputFormat.VALUE_TAG);
            //return label.ToString();
        }

        public string ToString(CoreLabel.OutputFormat format)
        {
            return label.ToString(format);
        }

        public void SetFromString(string labelStr)
        {
            throw new InvalidOperationException("Cannot set from string");
        }


        public class LabFact : ILabelFactory
        {
            public ILabel NewLabel(string labelStr)
            {
                var label = new CoreLabel();
                label.SetValue(labelStr);
                return new IndexedWord(label);
            }

            public ILabel NewLabel(string labelStr, int options)
            {
                return NewLabel(labelStr);
            }

            public ILabel NewLabelFromString(string encodedLabelStr)
            {
                throw new InvalidOperationException("This code branch left blank" +
                                                    " because we do not understand what this method should do.");
            }

            public ILabel NewLabel(ILabel oldLabel)
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

        public static ILabelFactory Factory()
        {
            return new LabFact();
        }

        public ILabelFactory LabelFactory()
        {
            return IndexedWord.Factory();
        }
    }
}