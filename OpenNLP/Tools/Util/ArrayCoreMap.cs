using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /**
 * <p>
 * Base implementation of {@link CoreMap} backed by two Java arrays.
 * </p>
 *
 * <p>
 * Reasonable care has been put into ensuring that this class is both fast and
 * has a light memory footprint.
 * </p>
 *
 * <p>
 * Note that like the base classes in the Collections API, this implementation
 * is <em>not thread-safe</em>. For speed reasons, these methods are not
 * synchronized. A synchronized wrapper could be developed by anyone so
 * inclined.
 * </p>
 *
 * <p>
 * Equality is defined over the complete set of keys and values currently
 * stored in the map.  Because this class is mutable, it should not be used
 * as a key in a Map.
 * </p>
 *
 * @author dramage
 * @author rafferty
 */

    public class ArrayCoreMap:CoreMap
    {
        /** Initial capacity of the array */
  private static readonly int INITIAL_CAPACITY = 4;

  /** Array of keys */
  private /*Class<? extends Key<?>>*/Type[] keys;

  /** Array of values */
  private Object[] values;

  /** Total number of elements actually in keys,values */
        public int psize { get; private set; }

        public int size()
        {
            return psize;
        }

        /**
   * Default constructor - initializes with default initial annotation
   * capacity of 4.
   */
  public ArrayCoreMap():this(INITIAL_CAPACITY) {}

  /**
   * Initializes this ArrayCoreMap, pre-allocating arrays to hold
   * up to capacity key,value pairs.  This array will grow if necessary.
   *
   * @param capacity Initial capacity of object in key,value pairs
   */
  public ArrayCoreMap(int capacity) {
    keys = new Type[capacity];
    values = new Object[capacity];
    // size starts at 0
  }

  /**
   * Copy constructor.
   * @param other The ArrayCoreMap to copy. It may not be null.
   */
  public ArrayCoreMap(ArrayCoreMap other) {
    psize = other.psize;
    keys = other.keys.Take(psize).ToArray();
    values = other.values.Take(psize).ToArray();
  }

  /**
   * Copy constructor.
   * @param other The ArrayCoreMap to copy. It may not be null.
   */
  //@SuppressWarnings("unchecked")
  public ArrayCoreMap(CoreMap other) {
    /*Set<Class<?>>*/var otherKeys = other.keySet();

    psize = otherKeys.Count;
    keys = new Type[psize];
    values = new Object[psize];

    int i = 0;
    foreach (var key in otherKeys) {
      this.keys[i] = key;
      this.values[i] = other.get(key);
      i++;
    }
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  //@SuppressWarnings("unchecked")
  public /*<VALUE> VALUE*/object get(/*Class<? extends Key<VALUE>>*/Type key) {
    for (int i = 0; i < psize; i++) {
      if (key == keys[i]) {
        return values[i];
      }
    }
    return default(object);
  }



  /**
   * {@inheritDoc}
   */
  //@Override
  public /*<VALUE>*/ bool has(/*Class<? extends Key<VALUE>>*/Type key) {
    for (int i = 0; i < psize; i++) {
      if (keys[i] == key) {
        return true;
      }
    }

    return false;
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  //@SuppressWarnings("unchecked")
  public /*<VALUE> VALUE*/object set(/*Class<? extends Key<VALUE>>*/Type key, object value) {

    // search array for existing value to replace
    for (int i = 0; i < psize; i++) {
      if (keys[i] == key) {
        object rv = values[i];
        values[i] = value;
        return rv;
      }
    }
    // not found in arrays, add to end ...

    // increment capacity of arrays if necessary
    if (psize >= keys.Length) {
      int capacity = keys.Length + (keys.Length < 16 ? 4: 8);
      Type[] newKeys = new Type[capacity];
      Object[] newValues = new Object[capacity];
      Array.Copy(keys, 0, newKeys, 0, psize);
      Array.Copy(values, 0, newValues, 0, psize);
      keys = newKeys;
      values = newValues;
    }

    // store value
    keys[psize] = key;
    values[psize] = value;
    psize++;

    return default(object);
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public Set<Type> keySet() {
      return new Set<Type>(keys);

    /*return new AbstractSet<Class<?>>() {
      //@Override
      public Iterator<Class<?>> iterator() {
        return new Iterator<Class<?>>() {
          private int i; // = 0;

          //@Override
          public bool hasNext() {
            return i < size;
          }

          //@Override
          public Class<?> next() {
            try {
              return keys[i++];
            } catch (ArrayIndexOutOfBoundsException aioobe) {
              throw new NoSuchElementException("ArrayCoreMap keySet iterator exhausted");
            }
          }

          //@Override
          //@SuppressWarnings("unchecked")
          public void remove() {
            ArrayCoreMap.this.remove((Class)keys[i]);
          }
        };
      }

      //@Override
      public int size() {
        return size;
      }
    };*/
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  //@SuppressWarnings("unchecked")
  public /*<VALUE> VALUE*/object remove(/*Class<? extends Key<VALUE>>*/Type key) {

    Object rv = null;
    for (int i = 0; i < psize; i++) {
      if (keys[i] == key) {
        rv = values[i];
        if (i < psize - 1) {
          Array.Copy(keys,   i+1, keys,   i, psize-(i+1));
          Array.Copy(values, i+1, values, i, psize-(i+1));
        }
        psize--;
        break;
      }
    }
    return rv;
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public /*<VALUE>*/ bool containsKey(/*Class<? extends Key<VALUE>>*/Type key) {
    for (int i = 0; i < psize; i++) {
      if (keys[i] == key) {
        return true;
      }
    }
    return false;
  }


  /**
   * Reduces memory consumption to the minimum for representing the values
   * currently stored stored in this object.
   */
  public void compact() {
    if (keys.Length > psize) {
      Type[] newKeys = new Type[psize];
      Object[] newValues = new Object[psize];
      Array.Copy(keys, 0, newKeys, 0, psize);
      Array.Copy(values, 0, newValues, 0, psize);
      keys = /*ErasureUtils.uncheckedCast(*/newKeys/*)*/;
      values = newValues;
    }
  }

  public void setCapacity(int newSize) {
    if (psize > newSize) { throw new SystemException("You cannot set capacity to smaller than the current size."); }
    Type[] newKeys = new Type[newSize];
    Object[] newValues = new Object[newSize];
    Array.Copy(keys, 0, newKeys, 0, psize);
    Array.Copy(values, 0, newValues, 0, psize);
    keys = /*ErasureUtils.uncheckedCast(*/newKeys/*)*/;
    values = newValues;
  }

  /**
   * Returns the number of elements in this map.
   * @return The number of elements in this map.
   */
  //@Override
  /*public int size() {
    return size;
  }*/

  /**
   * Keeps track of which ArrayCoreMaps have had toString called on
   * them.  We do not want to loop forever when there are cycles in
   * the annotation graph.  This is kept on a per-thread basis so that
   * each thread where toString gets called can keep track of its own
   * state.  When a call to toString is about to return, this is reset
   * to null for that particular thread.
   */
  private static readonly ThreadLocal<IdentityHashSet<CoreMap>> toStringCalled =
          new ThreadLocal<IdentityHashSet<CoreMap>>() {/*
            //@Override protected IdentityHashSet<CoreMap> initialValue() {
              return new IdentityHashSet<CoreMap>();
            }*/
          };

  /** Prints a full dump of a CoreMap. This method is robust to
   *  circularity in the CoreMap.
   *
   *  @return A String representation of the CoreMap
   */
  //@Override
  public String toString() {
    IdentityHashSet<CoreMap> calledSet = toStringCalled.Value;
    bool createdCalledSet = calledSet.isEmpty();

    if (calledSet.contains(this)) {
      return "[...]";
    }

    calledSet.add(this);

    StringBuilder s = new StringBuilder("[");
    for (int i = 0; i < psize; i++) {
      s.Append(keys[i].Name);
      s.Append('=');
      s.Append(values[i]);
      if (i < psize-1) {
        s.Append(' ');
      }
    }
    s.Append(']');

    if (createdCalledSet) {
      toStringCalled.Dispose();/*remove();*/
    } else {
      // Remove the object from the already called set so that
      // potential later calls in this object graph have something
      // more description than [...]
      calledSet.remove(this);
    }
    return s.ToString();
  }

  /**
   * {@inheritDoc}
   */
  //@Override
  public String toShorterString(String[] what) {
    StringBuilder s = new StringBuilder("[");
    for (int i = 0; i < psize; i++) {
      String name = keys[i].GetType().Name/*.getSimpleName()*/;
      int annoIdx = name.LastIndexOf("Annotation");
      if (annoIdx >= 0) {
        name = name.Substring(0, annoIdx);
      }
      bool include;
      if (what.Length > 0) {
        include = false;
        foreach (String item in what) {
          if (item.Equals(name)) {
            include = true;
            break;
          }
        }
      } else {
        include = true;
      }
      if (include) {
        if (s.Length > 1) {
          s.Append(' ');
        }
        s.Append(name);
        s.Append('=');
        s.Append(values[i]);
      }
    }
    s.Append(']');
    return s.ToString();
  }

  /** This gives a very short String representation of a CoreMap
   *  by leaving it to the content to reveal what field is being printed.
   *
   *  @param what An array (varargs) of Strings that say what annotation keys
   *     to print.  These need to be provided in a shortened form where you
   *     are just giving the part of the class name without package and up to
   *     "Annotation". That is,
   *     edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation
   *     -&gt; PartOfSpeech . As a special case, an empty array means
   *     to print everything, not nothing.
   *  @return Brief string where the field values are just separated by a
   *     character. If the string contains spaces, it is wrapped in "{...}".
   */
  public String toShortString(String[] what) {
    return toShortString('/', what);
  }

  /** This gives a very short String representation of a CoreMap
   *  by leaving it to the content to reveal what field is being printed.
   *
   *  @param separator Character placed between fields in output
   *  @param what An array (varargs) of Strings that say what annotation keys
   *     to print.  These need to be provided in a shortened form where you
   *     are just giving the part of the class name without package and up to
   *     "Annotation". That is,
   *     edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation
   *     -&gt; PartOfSpeech . As a special case, an empty array means
   *     to print everything, not nothing.
   *  @return Brief string where the field values are just separated by a
   *     character. If the string contains spaces, it is wrapped in "{...}".
   */
  public String toShortString(char separator, String[] what) {
    StringBuilder s = new StringBuilder();
    for (int i = 0; i < psize; i++) {
      bool include;
      if (what.Length > 0) {
        String name = keys[i].GetType().Name/*.getSimpleName()*/;
        int annoIdx = name.LastIndexOf("Annotation");
        if (annoIdx >= 0) {
          name = name.Substring(0, annoIdx);
        }
        include = false;
        foreach (String item in what) {
          if (item.Equals(name)) {
            include = true;
            break;
          }
        }
      } else {
        include = true;
      }
      if (include) {
        if (s.Length > 0) {
          s.Append(separator);
        }
        s.Append(values[i]);
      }
    }
    String answer = s.ToString();
    if (answer.IndexOf(' ') < 0) {
      return answer;
    } else {
      return '{' + answer + '}';
    }
  }

  /**
   * Keeps track of which pairs of ArrayCoreMaps have had equals
   * called on them.  We do not want to loop forever when there are
   * cycles in the annotation graph.  This is kept on a per-thread
   * basis so that each thread where equals gets called can keep
   * track of its own state.  When a call to toString is about to
   * return, this is reset to null for that particular thread.
   */
  private static readonly ThreadLocal<Dictionary<Tuple<CoreMap, CoreMap>, Boolean>> equalsCalled =
          new ThreadLocal<Dictionary<Tuple<CoreMap, CoreMap>, Boolean>>();


  /**
   * Two CoreMaps are equal iff all keys and values are .equal.
   */
  //@SuppressWarnings("unchecked")
  //@Override
  public bool equals(Object obj) {
    if (!(obj is CoreMap)) {
      return false;
    }

    if (obj is HashableCoreMap) {
      // overridden behavior for HashableCoreMap
      return obj.Equals(this);
    }

    if (obj is ArrayCoreMap) {
      // specialized equals for ArrayCoreMap
      return equals((ArrayCoreMap)obj);
    }

    // TODO: make the general equality work in the situation of loops
    // in the object graph

    // general equality
    CoreMap other = (CoreMap)obj;
    if ( ! this.keySet().Equals(other.keySet())) {
      return false;
    }
    foreach (var key in this.keySet()) {
      if (!other.has(key)) {
        return false;
      }
      Object thisV = this.get(key), otherV = other.get(key);

      if (thisV == otherV) {
        continue;
      }
      // the two values must be unequal, so if either is null, the other isn't
      if (thisV == null || otherV == null) {
        return false;
      }

      if ( ! thisV.Equals(otherV)) {
        return false;
      }
    }

    return true;
  }


  private bool equals(ArrayCoreMap other) {
    Dictionary<Tuple<CoreMap, CoreMap>, Boolean> calledMap = equalsCalled.Value;
    bool createdCalledMap = (calledMap == null);
    if (createdCalledMap) {
      calledMap = new Dictionary<Tuple<CoreMap, CoreMap>, bool>();
      equalsCalled.Value = calledMap;
    }

    // Note that for the purposes of recursion, we assume the two maps
    // are equals.  The two maps will therefore be equal if they
    // encounter each other again during the recursion unless there is
    // some other key that causes the equality to fail.
    // We do not need to later put false, as the entire call to equals
    // will unwind with false if any one equality check returns false.
    // TODO: since we only ever keep "true", we would rather use a
    // TwoDimensionalSet, but no such thing exists
    if (calledMap.ContainsKey(new Tuple<CoreMap, CoreMap>(this, other))) {
      return true;
    }
    bool result = true;
    calledMap.Add(new Tuple<CoreMap, CoreMap>(this, other), true);
    calledMap.Add(new Tuple<CoreMap, CoreMap>(other, this), true);

    if (this.psize != other.psize) {
      result = false;
    } else {
    for (int i = 0; i < this.psize; i++) {
      // test if other contains this key,value pair
      bool matched = false;
      for (int j = 0; j < other.psize; j++) {
        if (this.keys[i] == other.keys[j]) {
          if ((this.values[i] == null && other.values[j] != null) ||
              (this.values[i] != null && other.values[j] == null)) {
            matched = false;
            break;
          }

          if ((this.values[i] == null && other.values[j] == null) ||
              (this.values[i].Equals(other.values[j]))) {
            matched = true;
            break;
          }
        }
      }

      if (!matched) {
        result = false;
        break;
      }
    }
    }

    if (createdCalledMap) {
      equalsCalled.Value = null;
    }
    return result;
  }

  /**
   * Keeps track of which ArrayCoreMaps have had hashCode called on
   * them.  We do not want to loop forever when there are cycles in
   * the annotation graph.  This is kept on a per-thread basis so that
   * each thread where hashCode gets called can keep track of its own
   * state.  When a call to toString is about to return, this is reset
   * to null for that particular thread.
   */
  private static readonly ThreadLocal<IdentityHashSet<CoreMap>> hashCodeCalled =
          new ThreadLocal<IdentityHashSet<CoreMap>>();


  /**
   * Returns a composite hashCode over all the keys and values currently
   * stored in the map.  Because they may change over time, this class
   * is not appropriate for use as map keys.
   */
  //@Override
  public int hashCode() {
    IdentityHashSet<CoreMap> calledSet = hashCodeCalled.Value;
    bool createdCalledSet = (calledSet == null);
    if (createdCalledSet) {
      calledSet = new IdentityHashSet<CoreMap>();
      hashCodeCalled.Value = calledSet;
    }

    if (calledSet.contains(this)) {
      return 0;
    }

    calledSet.add(this);

    int keysCode = 0;
    int valuesCode = 0;
    for (int i = 0; i < psize; i++) {
      keysCode += (i < keys.Length && values[i] != null ? keys[i].GetHashCode() : 0);
      valuesCode += (i < values.Length && values[i] != null ? values[i].GetHashCode() : 0);
    }

    if (createdCalledSet) {
      hashCodeCalled.Value = null;
    } else {
      // Remove the object after processing is complete so that if
      // there are multiple instances of this CoreMap in the overall
      // object graph, they each have their hash code calculated.
      // TODO: can we cache this for later?
      calledSet.remove(this);
    }
    return keysCode * 37 + valuesCode;
  }

  //
  // serialization magic
  //

  /** Serialization version id */
  private static readonly long serialVersionUID = 1L;

  /**
   * Overridden serialization method: compacts our map before writing.
   *
   * @param out Stream to write to
   * @throws IOException If IO error
   */
  /*private void writeObject(ObjectOutputStream out) throws IOException {
    compact();
    out.defaultWriteObject();
  }*/

  // TODO: make prettyLog work in the situation of loops
  // in the object graph

  /**
   * {@inheritDoc}
   */
  //@Override
  //@SuppressWarnings("unchecked")
  /*public void prettyLog(RedwoodChannels channels, String description) {
    Redwood.startTrack(description);

    // sort keys by class name
    List<Class> sortedKeys = new ArrayList<Class>(this.keySet());
    Collections.sort(sortedKeys,
        (a, b) -> a.getCanonicalName().compareTo(b.getCanonicalName()));

    // log key/value pairs
    for (Class key : sortedKeys) {
      String keyName = key.getCanonicalName().replace("class ", "");
      Object value = this.get(key);
      if (PrettyLogger.dispatchable(value)) {
        PrettyLogger.log(channels, keyName, value);
      } else {
        channels.logf("%s = %s", keyName, value);
      }
    }
    Redwood.endTrack(description);
  }*/
    }
}
