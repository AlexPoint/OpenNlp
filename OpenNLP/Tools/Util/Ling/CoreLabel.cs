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
    public class CoreLabel: ArrayCoreMap, AbstractCoreLabel, HasWord, HasTag, HasCategory, HasLemma, HasContext, HasIndex, HasOffset
    {
        private static readonly long serialVersionUID = 2L;


  // /**
  //  * Should warnings be printed when converting from MapLabel family.
  //  */
  // private static readonly boolean VERBOSE = false;


  /** Default constructor, calls super() */
  public CoreLabel():base(){
  }

  /**
   * Initializes this CoreLabel, pre-allocating arrays to hold
   * up to capacity key,value pairs.  This array will grow if necessary.
   *
   * @param capacity Initial capacity of object in key,value pairs
   */
  public CoreLabel(int capacity):base(capacity){
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
  public CoreLabel(CoreLabel label):this((CoreMap) label){
  }

  /**
   * Returns a new CoreLabel instance based on the contents of the given
   * CoreMap.  It copies the contents of the other CoreMap.
   *
   * @param label The CoreMap to copy
   */
  //@SuppressWarnings({"unchecked"})
  public CoreLabel(CoreMap label):base(label.size()){
    foreach(var key in label.keySet()) {
      set(key, label.get(key));
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
  public CoreLabel(Label label):base(0){
    if (label is CoreMap) {
      CoreMap cl = (CoreMap) label;
      setCapacity(cl.size());
      foreach(var key in cl.keySet()) {
        set(key, cl.get(key));
      }
    } else {
      if (label is HasWord) {
         setWord(((HasWord)label).word());
      }
      setValue(label.value());
    }
  }

  /**
   * This constructor attempts to parse the String keys
   * into Class keys.  It's mainly useful for reading from
   * a file.  A best effort attempt is made to correctly
   * parse the keys according to the String lookup function
   * in {@link CoreAnnotations}.
   *
   * @param keys Array of Strings that are class names
   * @param values Array of values (as String)
   */
  /*public CoreLabel(String[] keys, String[] values):base(keys.Length){
    //this.map = new ArrayCoreMap();
    initFromStrings(keys, values);
  }*/


  /**
   * Class that all "generic" annotations extend.
   * This allows you to read in arbitrary values from a file as features, for example.
   */
  public /*static */interface GenericAnnotation<T> : CoreAnnotation<T> {  }
  //Unchecked is below because eclipse can't handle the level of type inference if we correctly parameterize GenericAnnotation with String
  //@SuppressWarnings("unchecked")
  /*public static /*readonly #1#Dictionary<String, typeof(GenericAnnotation)> genericKeys = Generics.newHashMap();
  //@SuppressWarnings("unchecked")
  public static /*readonly #1#Dictionary<Class<? extends GenericAnnotation>, String> genericValues = Generics.newHashMap();*/


  //@SuppressWarnings("unchecked")
  /*private void initFromStrings(String[] keys, String[] values) {
    for(int i = 0; i < Math.Min(keys.Length, values.Length); i++) {
      String key = keys[i];
      String value = values[i];
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
        //  GenericAnnotation<String> newKey = new GenericAnnotation<String>() {
        //    public Class<String> getType() { return String.class;} };
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


  public /*static*/ class CoreLabelFactory: LabelFactory {

    //@Override
    public Label newLabel(String labelStr) {
      CoreLabel label = new CoreLabel();
      label.setValue(labelStr);
      return label;
    }

    //@Override
    public Label newLabel(String labelStr, int options) {
      return newLabel(labelStr);
    }

    //@Override
    public Label newLabel(Label oldLabel) {
      if (oldLabel is CoreLabel) {
        return new CoreLabel((CoreLabel)oldLabel);

      } else {
        //Map the old interfaces to the correct key/value pairs
        //Don't need to worry about HasIndex, which doesn't appear in any legacy code
        CoreLabel label = new CoreLabel();
        if (oldLabel is HasWord)
          label.setWord(((HasWord) oldLabel).word());
        if (oldLabel is HasTag)
          label.setTag(((HasTag) oldLabel).tag());
        if (oldLabel is HasOffset) {
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
    public Label newLabelFromString(String encodedLabelStr) {
      throw new InvalidOperationException("This code branch left blank" +
      " because we do not understand what this method should do.");
    }

  }


  /**
   * Return a factory for this kind of label
   *
   * @return The label factory
   */
  public static LabelFactory factory() {
    return new CoreLabelFactory();
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public LabelFactory labelFactory() {
    return CoreLabel.factory();
  }

  /**
   * Return a non-null String value for a key.
   * This method is included for backwards compatibility with AbstractMapLabel.
   * It is guaranteed to not return null; if the key is not present or
   * has a null value, it returns the empty string ("").  It is only valid to
   * call this method when key is paired with a value of type String.
   *
   * @param <KEY> A key type with a String value
   * @param key The key to return the value of.
   * @return "" if the key is not in the map or has the value <code>null</code>
   *     and the String value of the key otherwise
   */
  //@Override
  public /*<KEY extends Key<String>>*/ String getString(Type key) {
    String value = (string)get(key);
    if (value == null) {
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
  public void setFromString(String labelStr) {
    throw new InvalidOperationException("Cannot set from string");
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setValue(String value) {
    set(typeof(CoreAnnotations.ValueAnnotation), value);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String value() {
    return (String)get(typeof(CoreAnnotations.ValueAnnotation));
  }

  /**
   * Set the word value for the label.  Also, clears the lemma, since
   * that may have changed if the word changed.
   */
  //@Override
  public void setWord(String word) {
    String originalWord = (String)get(typeof(CoreAnnotations.TextAnnotation));
    set(typeof(CoreAnnotations.TextAnnotation), word);
    // pado feb 09: if you change the word, delete the lemma.
    // gabor dec 2012: check if there was a real change -- this remove is actually rather expensive if it gets called a lot
    if (word != null && !word.Equals(originalWord) && containsKey(typeof(CoreAnnotations.LemmaAnnotation))) {
      remove(typeof(CoreAnnotations.LemmaAnnotation));
    }
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String word() {
    return (String)get(typeof(CoreAnnotations.TextAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setTag(String tag) {
    set(typeof(CoreAnnotations.PartOfSpeechAnnotation), tag);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String tag() {
    return (String)get(typeof(CoreAnnotations.PartOfSpeechAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setCategory(String category) {
    set(typeof(CoreAnnotations.CategoryAnnotation), category);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String category() {
    return (String)get(typeof(CoreAnnotations.CategoryAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setAfter(String after) {
    set(typeof(CoreAnnotations.AfterAnnotation), after);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String after() {
    return getString(typeof(CoreAnnotations.AfterAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setBefore(String before) {
    set(typeof(CoreAnnotations.BeforeAnnotation), before);
  }


  /**
   * {@inheritDoc}
   */
  //@Override
  public String before() {
    return getString(typeof(CoreAnnotations.BeforeAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setOriginalText(String originalText) {
    set(typeof(CoreAnnotations.OriginalTextAnnotation), originalText);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String originalText() {
    return getString(typeof(CoreAnnotations.OriginalTextAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String docID() {
    return (String)get(typeof(CoreAnnotations.DocIDAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setDocID(String docID) {
    //set(CoreAnnotations.DocIDAnnotation.class, docID);
  }

  /**
   * Return the named entity class of the label (or null if none).
   *
   * @return String the word value for the label
   */
  public String ner() {
    return (String)get(typeof(CoreAnnotations.NamedEntityTagAnnotation));
  }

  public void setNER(String ner) {
    set(typeof(CoreAnnotations.NamedEntityTagAnnotation), ner);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String lemma() {
    return (String)get(typeof(CoreAnnotations.LemmaAnnotation));
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setLemma(String lemma) {
    set(typeof(CoreAnnotations.LemmaAnnotation), lemma);
  }


  /**
   * {@inheritDoc}
   */
  //@Override
  public int index() {
    var n = get(typeof(CoreAnnotations.IndexAnnotation));
      if (n == null)
      {
          return -1;
      }
      else { return (int)n; }
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setIndex(int index) {
    set(typeof(CoreAnnotations.IndexAnnotation), index);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public int sentIndex() {
    var n = get(typeof(CoreAnnotations.SentenceIndexAnnotation));
    if(n == null)
      return -1;
    else
        return (int)n;
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setSentIndex(int sentIndex) {
    set(typeof(CoreAnnotations.SentenceIndexAnnotation), sentIndex);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public int beginPosition() {
    var i = get(typeof(CoreAnnotations.CharacterOffsetBeginAnnotation));
    if(i != null)
        return (int)i;
    else 
        return -1;
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public int endPosition() {
    var i = get(typeof(CoreAnnotations.CharacterOffsetEndAnnotation));
    if(i != null)
        return (int)i;
    else return -1;
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setBeginPosition(int beginPos) {
    set(typeof(CoreAnnotations.CharacterOffsetBeginAnnotation), beginPos);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public void setEndPosition(int endPos) {
    set(typeof(CoreAnnotations.CharacterOffsetEndAnnotation), endPos);
  }

  public int copyCount() {
    var copy = get(typeof(CoreAnnotations.CopyAnnotation));
    if (copy == null)
      return 0;
    else return (int)copy;
  }

  public void setCopyCount(int count) {
    set(typeof(CoreAnnotations.CopyAnnotation), count);
  }

  /**
   * Tag separator to use by default
   */
  public static readonly String TAG_SEPARATOR = "/";

  public enum OutputFormat {
    VALUE_INDEX, VALUE, VALUE_TAG, VALUE_TAG_INDEX, MAP, VALUE_MAP, VALUE_INDEX_MAP, WORD, WORD_INDEX
  };

  public static readonly OutputFormat DEFAULT_FORMAT = OutputFormat.VALUE_INDEX;

  //@Override
  public String toString() {
    return toString(DEFAULT_FORMAT);
  }

  /**
   * Returns a formatted string representing this label.  The
   * desired format is passed in as a <code>String</code>.
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
  public String toString(OutputFormat format) {
    StringBuilder buf = new StringBuilder();
    switch(format) {
    case OutputFormat.VALUE:
      buf.Append(value());
      break;
    case OutputFormat.MAP: {
      Dictionary<String, object> map2 = new Dictionary<string, object>();
      foreach(var key in this.keySet()) {
        map2.Add(key.Name, get(key));
      }
      buf.Append(map2);
      break;
    }
    case OutputFormat.VALUE_MAP: {
      buf.Append(value());
      Dictionary<Type, object> map2 = new Dictionary<Type, object>(asClassComparator);
      foreach(var key in this.keySet()) {
        map2.Add(key, get(key));
      }
      map2.Remove(typeof(CoreAnnotations.ValueAnnotation));
      buf.Append(map2);
      break;
    }
    case OutputFormat.VALUE_INDEX: {
      buf.Append(value());
      int index = (int)this.get(typeof(CoreAnnotations.IndexAnnotation));
      if (index != null) {
        buf.Append('-').Append(index);
      }
      buf.Append(toPrimes());
      break;
    }
    case OutputFormat.VALUE_TAG: {
      buf.Append(value());
      buf.Append(toPrimes());
      String ltag = tag();
      if (ltag != null) {
        buf.Append(TAG_SEPARATOR).Append(ltag);
      }
      break;
    }
    case OutputFormat.VALUE_TAG_INDEX: {
      buf.Append(value());
      String ltag = tag();
      if (ltag != null) {
        buf.Append(TAG_SEPARATOR).Append(ltag);
      }
      int index = (int)this.get(typeof(CoreAnnotations.IndexAnnotation));
      if (index != null) {
        buf.Append('-').Append(index);
      }
      buf.Append(toPrimes());
      break;
    }
    case OutputFormat.VALUE_INDEX_MAP: {
      buf.Append(value());
      int index = (int)this.get(typeof(CoreAnnotations.IndexAnnotation));
      if (index != null) {
        buf.Append('-').Append(index);
      }
      Dictionary<String,Object> map2 = new Dictionary<String,Object>();
      foreach(var key in this.keySet()) {
        String cls = key.Name;
        // special shortening of all the Annotation classes
        int idx = cls.IndexOf('$');
        if (idx >= 0) {
          cls = cls.Substring(idx + 1);
        }
        map2.Add(cls, this.get(key));
      }
      map2.Remove("IndexAnnotation");
      map2.Remove("ValueAnnotation");
      if (map2.Any()) {
        buf.Append(map2);
      }
      break;
    }
    case OutputFormat.WORD:
      // TODO: we should unify word() and value()
      buf.Append(word());
      break;
    case OutputFormat.WORD_INDEX: {
      buf.Append(this.get(typeof(CoreAnnotations.TextAnnotation)));
      int index = (int)this.get(typeof(CoreAnnotations.IndexAnnotation));
      if (index != null) {
        buf.Append('-').Append(index);
      }
      buf.Append(toPrimes());
      break;
    }
    default:
      throw new InvalidDataException("Unknown format " + format);
    }
    return buf.ToString();
  }

  public String toPrimes() {
    return StringUtils.repeat('\'', copyCount());
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
