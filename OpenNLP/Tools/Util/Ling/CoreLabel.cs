using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /**
 * A CoreLabel represents a single word with ancillary information
 * attached using CoreAnnotations.  If the proper annotations are set,
 * the CoreLabel also provides convenient methods to access tags,
 * lemmas, etc.
 * <p>
 * A CoreLabel is a Map from keys (which are Class objects) to values,
 * whose type is determined by the key.  That is, it is a heterogeneous
 * typesafe Map (see Josh Bloch, Effective Java, 2nd edition).
 * <p>
 * The CoreLabel class in particular bridges the gap between old-style JavaNLP
 * Labels and the new CoreMap infrastructure.  Instances of this class can be
 * used (almost) anywhere that the now-defunct FeatureLabel family could be
 * used.  This data structure is backed by an {@link ArrayCoreMap}.
 *
 * @author dramage
 * @author rafferty
 */

    public class CoreLabel : ArrayCoreMap, AbstractCoreLabel, HasWord, HasTag, HasCategory, HasLemma, HasContext,
        HasIndex, HasOffset
    {
        private static readonly long serialVersionUID = 2L;


        // /**
        //  * Should warnings be printed when converting from MapLabel family.
        //  */
        // private static readonly boolean VERBOSE = false;


        /** Default constructor, calls super() */

        public CoreLabel() : base()
        {
        }

        /**
   * Initializes this CoreLabel, pre-allocating arrays to hold
   * up to capacity key,value pairs.  This array will grow if necessary.
   *
   * @param capacity Initial capacity of object in key,value pairs
   */

        public CoreLabel(int capacity) : base(capacity)
        {
        }

        /**
   * Returns a new CoreLabel instance based on the contents of the given
   * CoreLabel.  It copies the contents of the other CoreLabel.
   * <i>Implementation note:</i> this is a the same as the constructor
   * that takes a CoreMap, but is needed to ensure unique most specific
   * type inference for selecting a constructor at compile-time.
   *
   * @param label The CoreLabel to copy
   */

        public CoreLabel(CoreLabel label) : this((CoreMap) label)
        {
        }

        /**
   * Returns a new CoreLabel instance based on the contents of the given
   * CoreMap.  It copies the contents of the other CoreMap.
   *
   * @param label The CoreMap to copy
   */
        //@SuppressWarnings({"unchecked"})
        public CoreLabel(CoreMap label) : base(label.Size())
        {
            foreach (var key in label.KeySet())
            {
                Set(key, label.Get(key));
            }
        }

        /**
   * Returns a new CoreLabel instance based on the contents of the given
   * label.   Warning: The behavior of this method is a bit disjunctive!
   * If label is a CoreMap (including CoreLabel), then its entire
   * contents is copied into this label.  But, otherwise, just the
   * value() and word iff it implements HasWord is copied.
   *
   * @param label Basis for this label
   */
        //@SuppressWarnings("unchecked")
        public CoreLabel(Label label) : base(0)
        {
            if (label is CoreMap)
            {
                CoreMap cl = (CoreMap) label;
                SetCapacity(cl.Size());
                foreach (var key in cl.KeySet())
                {
                    Set(key, cl.Get(key));
                }
            }
            else
            {
                if (label is HasWord)
                {
                    setWord(((HasWord) label).word());
                }
                setValue(label.value());
            }
        }

        /**
   * This constructor attempts to parse the string keys
   * into Class keys.  It's mainly useful for reading from
   * a file.  A best effort attempt is made to correctly
   * parse the keys according to the string lookup function
   * in {@link CoreAnnotations}.
   *
   * @param keys Array of Strings that are class names
   * @param values Array of values (as String)
   */
        /*public CoreLabel(string[] keys, string[] values):base(keys.Length){
    //this.map = new ArrayCoreMap();
    initFromStrings(keys, values);
  }*/


        /**
   * Class that all "generic" annotations extend.
   * This allows you to read in arbitrary values from a file as features, for example.
   */

        public /*static */ interface GenericAnnotation<T> : CoreAnnotation<T>
        {
        }

        //Unchecked is below because eclipse can't handle the level of type inference if we correctly parameterize GenericAnnotation with String
        //@SuppressWarnings("unchecked")
        /*public static /*readonly #1#Dictionary<string, typeof(GenericAnnotation)> genericKeys = Generics.newHashMap();
  //@SuppressWarnings("unchecked")
  public static /*readonly #1#Dictionary<Class<? extends GenericAnnotation>, string> genericValues = Generics.newHashMap();*/


        //@SuppressWarnings("unchecked")
        /*private void initFromStrings(string[] keys, string[] values) {
    for(int i = 0; i < Math.Min(keys.Length, values.Length); i++) {
      string key = keys[i];
      string value = values[i];
      AnnotationLookup.KeyLookup lookup = AnnotationLookup.getCoreKey(key);

      //now work with the key we got above
      if (lookup == null) {
        if (key != null) {
          throw new InvalidOperationException("Unknown key " + key);
        }

        // It used to be that the following code let you put unknown keys
        // in the CoreLabel.  However, you can't create classes dynamically
        // at run time, which meant only one of these classes could ever
        // exist, which meant multiple unknown keys would clobber each
        // other and be very annoying.  It's easier just to not allow
        // it at all.
        // If it becomes possible to create classes dynamically,
        // we could add this code back.
        //if(genericKeys.containsKey(key)) {
        //  this.set(genericKeys.get(key), value);
        //} else {
        //  GenericAnnotation<string> newKey = new GenericAnnotation<string>() {
        //    public Class<string> getType() { return string.class;} };
        //  this.set(newKey.getClass(), values[i]);
        //  genericKeys.put(keys[i], newKey.getClass());
        //  genericValues.put(newKey.getClass(), keys[i]);
        //}
        // unknown key; ignore
        //if (VERBOSE) {
        //  System.err.println("CORE: CoreLabel.fromAbstractMapLabel: " +
        //      "Unknown key "+key);
        //}
      } else {
        try {
          Class<?> valueClass = AnnotationLookup.getValueType(lookup.coreKey);
          if(valueClass.Equals(String.class)) {
            this.set(lookup.coreKey, values[i]);
          } else if(valueClass == Integer.class) {
            this.set(lookup.coreKey, Integer.parseInt(values[i]));
          } else if(valueClass == Double.class) {
            this.set(lookup.coreKey, Double.parseDouble(values[i]));
          } else if(valueClass == Long.class) {
            this.set(lookup.coreKey, Long.parseLong(values[i]));
          }
        } catch (Exception e) {
          /*e.printStackTrace();
          // unexpected value type
          System.err.println("CORE: CoreLabel.initFromStrings: "
              + "Bad type for " + key
              + ". Value was: " + value
              + "; expected "+AnnotationLookup.getValueType(lookup.coreKey));#1#
        }
      }
    }
  }*/


        public /*static*/ class CoreLabelFactory : LabelFactory
        {

            //@Override
            public Label newLabel(string labelStr)
            {
                CoreLabel label = new CoreLabel();
                label.setValue(labelStr);
                return label;
            }

            //@Override
            public Label newLabel(string labelStr, int options)
            {
                return newLabel(labelStr);
            }

            //@Override
            public Label newLabel(Label oldLabel)
            {
                if (oldLabel is CoreLabel)
                {
                    return new CoreLabel((CoreLabel) oldLabel);

                }
                else
                {
                    //Map the old interfaces to the correct key/value pairs
                    //Don't need to worry about HasIndex, which doesn't appear in any legacy code
                    CoreLabel label = new CoreLabel();
                    if (oldLabel is HasWord)
                        label.setWord(((HasWord) oldLabel).word());
                    if (oldLabel is HasTag)
                        label.setTag(((HasTag) oldLabel).tag());
                    if (oldLabel is HasOffset)
                    {
                        label.setBeginPosition(((HasOffset) oldLabel).beginPosition());
                        label.setEndPosition(((HasOffset) oldLabel).endPosition());
                    }
                    if (oldLabel is HasCategory)
                        label.setCategory(((HasCategory) oldLabel).category());
                    if (oldLabel is HasIndex)
                        label.setIndex(((HasIndex) oldLabel).index());

                    label.setValue(oldLabel.value());

                    return label;
                }
            }

            //@Override
            public Label newLabelFromString(string encodedLabelStr)
            {
                throw new InvalidOperationException("This code branch left blank" +
                                                    " because we do not understand what this method should do.");
            }

        }


        /**
   * Return a factory for this kind of label
   *
   * @return The label factory
   */

        public static LabelFactory factory()
        {
            return new CoreLabelFactory();
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public LabelFactory labelFactory()
        {
            return CoreLabel.factory();
        }

        /**
   * Return a non-null string value for a key.
   * This method is included for backwards compatibility with AbstractMapLabel.
   * It is guaranteed to not return null; if the key is not present or
   * has a null value, it returns the empty string ("").  It is only valid to
   * call this method when key is paired with a value of type string.
   *
   * @param <KEY> A key type with a string value
   * @param key The key to return the value of.
   * @return "" if the key is not in the map or has the value <code>null</code>
   *     and the string value of the key otherwise
   */
        //@Override
        public /*<KEY extends Key<string>>*/ string getString(Type key)
        {
            string value = (string) Get(key);
            if (value == null)
            {
                return "";
            }
            return value;
        }


        /**
   * {@inheritDoc}
   */
//  public int size() {
//    return map.size();
//  }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setFromString(string labelStr)
        {
            throw new InvalidOperationException("Cannot set from string");
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setValue(string value)
        {
            Set(typeof (CoreAnnotations.ValueAnnotation), value);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string value()
        {
            return (String) Get(typeof (CoreAnnotations.ValueAnnotation));
        }

        /**
   * Set the word value for the label.  Also, clears the lemma, since
   * that may have changed if the word changed.
   */
        //@Override
        public void setWord(string word)
        {
            string originalWord = (String) Get(typeof (CoreAnnotations.TextAnnotation));
            Set(typeof (CoreAnnotations.TextAnnotation), word);
            // pado feb 09: if you change the word, delete the lemma.
            // gabor dec 2012: check if there was a real change -- this remove is actually rather expensive if it gets called a lot
            if (word != null && !word.Equals(originalWord) && ContainsKey(typeof (CoreAnnotations.LemmaAnnotation)))
            {
                Remove(typeof (CoreAnnotations.LemmaAnnotation));
            }
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string word()
        {
            return (String) Get(typeof (CoreAnnotations.TextAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setTag(string tag)
        {
            Set(typeof (CoreAnnotations.PartOfSpeechAnnotation), tag);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string tag()
        {
            return (String) Get(typeof (CoreAnnotations.PartOfSpeechAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setCategory(string category)
        {
            Set(typeof (CoreAnnotations.CategoryAnnotation), category);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string category()
        {
            return (String) Get(typeof (CoreAnnotations.CategoryAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setAfter(string after)
        {
            Set(typeof (CoreAnnotations.AfterAnnotation), after);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string after()
        {
            return getString(typeof (CoreAnnotations.AfterAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setBefore(string before)
        {
            Set(typeof (CoreAnnotations.BeforeAnnotation), before);
        }


        /**
   * {@inheritDoc}
   */
        //@Override
        public string before()
        {
            return getString(typeof (CoreAnnotations.BeforeAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setOriginalText(string originalText)
        {
            Set(typeof (CoreAnnotations.OriginalTextAnnotation), originalText);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string originalText()
        {
            return getString(typeof (CoreAnnotations.OriginalTextAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string docID()
        {
            return (String) Get(typeof (CoreAnnotations.DocIDAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setDocID(string docID)
        {
            //set(CoreAnnotations.DocIDAnnotation.class, docID);
        }

        /**
   * Return the named entity class of the label (or null if none).
   *
   * @return string the word value for the label
   */

        public string ner()
        {
            return (String) Get(typeof (CoreAnnotations.NamedEntityTagAnnotation));
        }

        public void setNER(string ner)
        {
            Set(typeof (CoreAnnotations.NamedEntityTagAnnotation), ner);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public string lemma()
        {
            return (String) Get(typeof (CoreAnnotations.LemmaAnnotation));
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setLemma(string lemma)
        {
            Set(typeof (CoreAnnotations.LemmaAnnotation), lemma);
        }


        /**
   * {@inheritDoc}
   */
        //@Override
        public int index()
        {
            var n = Get(typeof (CoreAnnotations.IndexAnnotation));
            if (n == null)
            {
                return -1;
            }
            else
            {
                return (int) n;
            }
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setIndex(int index)
        {
            Set(typeof (CoreAnnotations.IndexAnnotation), index);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public int sentIndex()
        {
            var n = Get(typeof (CoreAnnotations.SentenceIndexAnnotation));
            if (n == null)
                return -1;
            else
                return (int) n;
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setSentIndex(int sentIndex)
        {
            Set(typeof (CoreAnnotations.SentenceIndexAnnotation), sentIndex);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public int beginPosition()
        {
            var i = Get(typeof (CoreAnnotations.CharacterOffsetBeginAnnotation));
            if (i != null)
                return (int) i;
            else
                return -1;
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public int endPosition()
        {
            var i = Get(typeof (CoreAnnotations.CharacterOffsetEndAnnotation));
            if (i != null)
                return (int) i;
            else return -1;
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setBeginPosition(int beginPos)
        {
            Set(typeof (CoreAnnotations.CharacterOffsetBeginAnnotation), beginPos);
        }

        /**
   * {@inheritDoc}
   */
        //@Override
        public void setEndPosition(int endPos)
        {
            Set(typeof (CoreAnnotations.CharacterOffsetEndAnnotation), endPos);
        }

        public int copyCount()
        {
            var copy = Get(typeof (CoreAnnotations.CopyAnnotation));
            if (copy == null)
                return 0;
            else return (int) copy;
        }

        public void setCopyCount(int count)
        {
            Set(typeof (CoreAnnotations.CopyAnnotation), count);
        }

        /**
   * Tag separator to use by default
   */
        public static readonly string TAG_SEPARATOR = "/";

        public enum OutputFormat
        {
            VALUE_INDEX,
            VALUE,
            VALUE_TAG,
            VALUE_TAG_INDEX,
            MAP,
            VALUE_MAP,
            VALUE_INDEX_MAP,
            WORD,
            WORD_INDEX
        };

        public static readonly OutputFormat DEFAULT_FORMAT = OutputFormat.VALUE_INDEX;

        //@Override
        public string ToString()
        {
            return ToString(DEFAULT_FORMAT);
        }

        /**
   * Returns a formatted string representing this label.  The
   * desired format is passed in as a <code>string</code>.
   * Currently supported formats include:
   * <ul>
   * <li>"value": just prints the value</li>
   * <li>"{map}": prints the complete map</li>
   * <li>"value{map}": prints the value followed by the contained
   * map (less the map entry containing key <code>CATEGORY_KEY</code>)</li>
   * <li>"value-index": extracts a value and an integer index from
   * the contained map using keys  <code>INDEX_KEY</code>,
   * respectively, and prints them with a hyphen in between</li>
   * <li>"value-tag"
   * <li>"value-tag-index"
   * <li>"value-index{map}": a combination of the above; the index is
   * displayed first and then not shown in the map that is displayed</li>
   * <li>"word": Just the value of HEAD_WORD_KEY in the map</li>
   * </ul>
   * <p/>
   * Map is printed in alphabetical order of keys.
   */
        //@SuppressWarnings("unchecked")
        public string ToString(OutputFormat format)
        {
            StringBuilder buf = new StringBuilder();
            switch (format)
            {
                case OutputFormat.VALUE:
                    buf.Append(value());
                    break;
                case OutputFormat.MAP:
                {
                    Dictionary<string, object> map2 = new Dictionary<string, object>();
                    foreach (var key in this.KeySet())
                    {
                        map2.Add(key.Name, Get(key));
                    }
                    buf.Append(map2);
                    break;
                }
                case OutputFormat.VALUE_MAP:
                {
                    buf.Append(value());
                    Dictionary<Type, object> map2 = new Dictionary<Type, object>(asClassComparator);
                    foreach (var key in this.KeySet())
                    {
                        map2.Add(key, Get(key));
                    }
                    map2.Remove(typeof (CoreAnnotations.ValueAnnotation));
                    buf.Append(map2);
                    break;
                }
                case OutputFormat.VALUE_INDEX:
                {
                    buf.Append(value());
                    var index = this.Get(typeof (CoreAnnotations.IndexAnnotation));
                    if (index != null)
                    {
                        buf.Append('-').Append((int) index);
                    }
                    buf.Append(toPrimes());
                    break;
                }
                case OutputFormat.VALUE_TAG:
                {
                    buf.Append(value());
                    buf.Append(toPrimes());
                    string ltag = tag();
                    if (ltag != null)
                    {
                        buf.Append(TAG_SEPARATOR).Append(ltag);
                    }
                    break;
                }
                case OutputFormat.VALUE_TAG_INDEX:
                {
                    buf.Append(value());
                    string ltag = tag();
                    if (ltag != null)
                    {
                        buf.Append(TAG_SEPARATOR).Append(ltag);
                    }
                    var index = this.Get(typeof (CoreAnnotations.IndexAnnotation));
                    if (index != null)
                    {
                        buf.Append('-').Append((int) index);
                    }
                    buf.Append(toPrimes());
                    break;
                }
                case OutputFormat.VALUE_INDEX_MAP:
                {
                    buf.Append(value());
                    var index = this.Get(typeof (CoreAnnotations.IndexAnnotation));
                    if (index != null)
                    {
                        buf.Append('-').Append((int) index);
                    }
                    Dictionary<string, Object> map2 = new Dictionary<string, Object>();
                    foreach (var key in this.KeySet())
                    {
                        string cls = key.Name;
                        // special shortening of all the Annotation classes
                        int idx = cls.IndexOf('$');
                        if (idx >= 0)
                        {
                            cls = cls.Substring(idx + 1);
                        }
                        map2.Add(cls, this.Get(key));
                    }
                    map2.Remove("IndexAnnotation");
                    map2.Remove("ValueAnnotation");
                    if (map2.Any())
                    {
                        buf.Append(map2);
                    }
                    break;
                }
                case OutputFormat.WORD:
                    // TODO: we should unify word() and value()
                    buf.Append(word());
                    break;
                case OutputFormat.WORD_INDEX:
                {
                    buf.Append(this.Get(typeof (CoreAnnotations.TextAnnotation)));
                    var index = this.Get(typeof (CoreAnnotations.IndexAnnotation));
                    if (index != null)
                    {
                        buf.Append('-').Append((int) index);
                    }
                    buf.Append(toPrimes());
                    break;
                }
                default:
                    throw new InvalidDataException("Unknown format " + format);
            }
            return buf.ToString();
        }

        public string toPrimes()
        {
            return StringUtils.Repeat('\'', copyCount());
        }

        private class NameComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(Type obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        private static readonly IEqualityComparer<Type> asClassComparator = new NameComparer();
    }
}