using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    
    /// <summary>
    /// Base implementation of {@link CoreMap} backed by two Java arrays.
    /// 
    /// Reasonable care has been put into ensuring that this class is both fast and
    /// has a light memory footprint.
    /// 
    /// Note that like the base classes in the Collections API, this implementation
    /// is <em>not thread-safe</em>. For speed reasons, these methods are not
    /// synchronized. A synchronized wrapper could be developed by anyone so inclined.
    /// 
    /// Equality is defined over the complete set of keys and values currently
    /// stored in the map.  Because this class is mutable, it should not be used
    /// as a key in a Map.
    /// 
    /// @author dramage
    /// @author rafferty
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class ArrayCoreMap : ICoreMap
    {
        /// <summary>Initial capacity of the array</summary>
        private const int InitialCapacity = 4;

        /// <summary>Array of keys</summary>
        private /*Class<? extends Key<?>>*/ Type[] keys;

        /** Array of values */
        private Object[] values;

        /// <summary>Total number of elements actually in keys,values</summary>
        public int psize { get; private set; }

        public int Size()
        {
            return psize;
        }
        
        /// <summary>
        /// Default constructor - initializes with default initial annotation capacity of 4.
        /// </summary>
        public ArrayCoreMap() : this(InitialCapacity)
        {
        }
        
        /// <summary>
        /// Initializes this ArrayCoreMap, pre-allocating arrays to hold up to capacity key,value pairs.
        /// This array will grow if necessary.
        /// </summary>
        /// <param name="capacity">Initial capacity of object in key,value pairs</param>
        public ArrayCoreMap(int capacity)
        {
            keys = new Type[capacity];
            values = new Object[capacity];
            // size starts at 0
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The ArrayCoreMap to copy. It may not be null.</param>
        public ArrayCoreMap(ArrayCoreMap other)
        {
            psize = other.psize;
            keys = other.keys.Take(psize).ToArray();
            values = other.values.Take(psize).ToArray();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The ArrayCoreMap to copy. It may not be null.</param>
        public ArrayCoreMap(ICoreMap other)
        {
            /*Set<Class<?>>*/
            var otherKeys = other.KeySet();

            psize = otherKeys.Count;
            keys = new Type[psize];
            values = new Object[psize];

            int i = 0;
            foreach (var key in otherKeys)
            {
                this.keys[i] = key;
                this.values[i] = other.Get(key);
                i++;
            }
        }

        public /*<VALUE> VALUE*/ object Get( /*Class<? extends Key<VALUE>>*/ Type key)
        {
            for (int i = 0; i < psize; i++)
            {
                if (key == keys[i])
                {
                    return values[i];
                }
            }
            return default(object);
        }

        public /*<VALUE>*/ bool Has( /*Class<? extends Key<VALUE>>*/ Type key)
        {
            for (int i = 0; i < psize; i++)
            {
                if (keys[i] == key)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual /*<VALUE> VALUE*/ object Set( /*Class<? extends Key<VALUE>>*/ Type key, object value)
        {

            // search array for existing value to replace
            for (int i = 0; i < psize; i++)
            {
                if (keys[i] == key)
                {
                    object rv = values[i];
                    values[i] = value;
                    return rv;
                }
            }
            // not found in arrays, add to end ...

            // increment capacity of arrays if necessary
            if (psize >= keys.Length)
            {
                int capacity = keys.Length + (keys.Length < 16 ? 4 : 8);
                var newKeys = new Type[capacity];
                var newValues = new Object[capacity];
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

        public Set<Type> KeySet()
        {
            return new Set<Type>(keys);

            /*return new AbstractSet<Class<?>>() {
      
      public Iterator<Class<?>> iterator() {
        return new Iterator<Class<?>>() {
          private int i; // = 0;

          public bool hasNext() {
            return i < size;
          }

          public Class<?> next() {
            try {
              return keys[i++];
            } catch (ArrayIndexOutOfBoundsException aioobe) {
              throw new NoSuchElementException("ArrayCoreMap keySet iterator exhausted");
            }
          }

          public void remove() {
            ArrayCoreMap.this.remove((Class)keys[i]);
          }
        };
      }

      public int size() {
        return size;
      }
    };*/
        }

        public /*<VALUE> VALUE*/ object Remove( /*Class<? extends Key<VALUE>>*/ Type key)
        {

            Object rv = null;
            for (int i = 0; i < psize; i++)
            {
                if (keys[i] == key)
                {
                    rv = values[i];
                    if (i < psize - 1)
                    {
                        Array.Copy(keys, i + 1, keys, i, psize - (i + 1));
                        Array.Copy(values, i + 1, values, i, psize - (i + 1));
                    }
                    psize--;
                    break;
                }
            }
            return rv;
        }

        public /*<VALUE>*/ bool ContainsKey( /*Class<? extends Key<VALUE>>*/ Type key)
        {
            for (int i = 0; i < psize; i++)
            {
                if (keys[i] == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reduces memory consumption to the minimum for representing 
        /// the values currently stored stored in this object.
        /// </summary>
        public void Compact()
        {
            if (keys.Length > psize)
            {
                var newKeys = new Type[psize];
                var newValues = new Object[psize];
                Array.Copy(keys, 0, newKeys, 0, psize);
                Array.Copy(values, 0, newValues, 0, psize);
                keys = /*ErasureUtils.uncheckedCast(*/ newKeys /*)*/;
                values = newValues;
            }
        }

        public void SetCapacity(int newSize)
        {
            if (psize > newSize)
            {
                throw new SystemException("You cannot set capacity to smaller than the current size.");
            }
            var newKeys = new Type[newSize];
            var newValues = new Object[newSize];
            Array.Copy(keys, 0, newKeys, 0, psize);
            Array.Copy(values, 0, newValues, 0, psize);
            keys = /*ErasureUtils.uncheckedCast(*/ newKeys /*)*/;
            values = newValues;
        }

        /**
   * Returns the number of elements in this map.
   * @return The number of elements in this map.
   */
        /*public int size() {
    return size;
  }*/
        
        /// <summary>
        /// Keeps track of which ArrayCoreMaps have had ToString called on them.
        /// We do not want to loop forever when there are cycles in the annotation graph.
        /// This is kept on a per-thread basis so that each thread 
        /// where ToString gets called can keep track of its own state.
        /// When a call to ToString is about to return, this is reset to null for that particular thread.
        /// </summary>
        private static readonly ThreadLocal<IdentityHashSet<ICoreMap>> ToStringCalled =
            new ThreadLocal<IdentityHashSet<ICoreMap>>()
            {
/*
            protected IdentityHashSet<CoreMap> initialValue() {
              return new IdentityHashSet<CoreMap>();
            }*/
            };

        public override string ToString()
        {
            IdentityHashSet<ICoreMap> calledSet = ToStringCalled.Value;
            if (calledSet == null)
            {
                calledSet = new IdentityHashSet<ICoreMap>();
            }

            if (calledSet.Contains(this))
            {
                return "[...]";
            }

            calledSet.Add(this);

            var s = new StringBuilder("[");
            for (int i = 0; i < psize; i++)
            {
                s.Append(keys[i].Name);
                s.Append('=');
                s.Append(values[i]);
                if (i < psize - 1)
                {
                    s.Append(' ');
                }
            }
            s.Append(']');

            bool createdCalledSet = calledSet == null || calledSet.IsEmpty();
            if (createdCalledSet)
            {
                ToStringCalled.Dispose(); /*remove();*/
            }
            else
            {
                // Remove the object from the already called set so that
                // potential later calls in this object graph have something
                // more description than [...]
                calledSet.Remove(this);
            }
            return s.ToString();
        }

        public string ToShorterString(string[] what)
        {
            var s = new StringBuilder("[");
            for (int i = 0; i < psize; i++)
            {
                string name = keys[i].GetType().Name /*.getSimpleName()*/;
                int annoIdx = name.LastIndexOf("Annotation");
                if (annoIdx >= 0)
                {
                    name = name.Substring(0, annoIdx);
                }
                bool include;
                if (what.Length > 0)
                {
                    include = false;
                    foreach (string item in what)
                    {
                        if (item.Equals(name))
                        {
                            include = true;
                            break;
                        }
                    }
                }
                else
                {
                    include = true;
                }
                if (include)
                {
                    if (s.Length > 1)
                    {
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

        /// <summary>
        /// This gives a very short string representation of a CoreMap
        /// by leaving it to the content to reveal what field is being printed.
        /// </summary>
        /// <param name="what">
        /// An array (varargs) of strings that say what annotation keys to print.
        /// These need to be provided in a shortened form where you are just giving 
        /// the part of the class name without package and up to "Annotation".
        /// That is, edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation -> PartOfSpeech.
        /// As a special case, an empty array means to print everything, not nothing.
        /// </param>
        /// <returns>
        /// Brief string where the field values are just separated by a character.
        /// If the string contains spaces, it is wrapped in "{...}".
        /// </returns>
        public string ToShortString(string[] what)
        {
            return ToShortString('/', what);
        }

        /// <summary>
        /// This gives a very short string representation of a CoreMap
        /// by leaving it to the content to reveal what field is being printed.
        /// </summary>
        /// <param name="separator">Character placed between fields in output</param>
        /// <param name="what">
        /// An array (varargs) of strings that say what annotation keys to print.
        /// These need to be provided in a shortened form where you are just giving 
        /// the part of the class name without package and up to "Annotation". 
        /// That is, edu.stanford.nlp.ling.CoreAnnotations.PartOfSpeechAnnotation -> PartOfSpeech.
        /// As a special case, an empty array means to print everything, not nothing.
        /// </param>
        /// <returns>
        /// Brief string where the field values are just separated by a character. 
        /// If the string contains spaces, it is wrapped in "{...}".
        /// </returns>
        public string ToShortString(char separator, string[] what)
        {
            var s = new StringBuilder();
            for (int i = 0; i < psize; i++)
            {
                bool include;
                if (what.Length > 0)
                {
                    string name = keys[i].GetType().Name /*.getSimpleName()*/;
                    int annoIdx = name.LastIndexOf("Annotation");
                    if (annoIdx >= 0)
                    {
                        name = name.Substring(0, annoIdx);
                    }
                    include = false;
                    foreach (string item in what)
                    {
                        if (item.Equals(name))
                        {
                            include = true;
                            break;
                        }
                    }
                }
                else
                {
                    include = true;
                }
                if (include)
                {
                    if (s.Length > 0)
                    {
                        s.Append(separator);
                    }
                    s.Append(values[i]);
                }
            }
            string answer = s.ToString();
            if (answer.IndexOf(' ') < 0)
            {
                return answer;
            }
            else
            {
                return '{' + answer + '}';
            }
        }

        /// <summary>
        /// Keeps track of which pairs of ArrayCoreMaps have had equals called on them.
        /// We do not want to loop forever when there are cycles in the annotation graph.
        /// This is kept on a per-thread basis so that each thread where equals gets called can keep track of its own state.
        /// When a call to ToString is about to return, this is reset to null for that particular thread.
        /// </summary>
        private static readonly ThreadLocal<Dictionary<Tuple<ICoreMap, ICoreMap>, Boolean>> EqualsCalled =
            new ThreadLocal<Dictionary<Tuple<ICoreMap, ICoreMap>, Boolean>>();

        /// <summary>Two CoreMaps are equal iff all keys and values are .equal.</summary>
        public override bool Equals(Object obj)
        {
            if (!(obj is ICoreMap))
            {
                return false;
            }

            if (obj is HashableCoreMap)
            {
                // overridden behavior for HashableCoreMap
                return obj.Equals(this);
            }

            if (obj is ArrayCoreMap)
            {
                // specialized equals for ArrayCoreMap
                return Equals((ArrayCoreMap) obj);
            }

            // TODO: make the general equality work in the situation of loops
            // in the object graph

            // general equality
            var other = (ICoreMap) obj;
            if (! this.KeySet().Equals(other.KeySet()))
            {
                return false;
            }
            foreach (var key in this.KeySet())
            {
                if (!other.Has(key))
                {
                    return false;
                }
                Object thisV = this.Get(key), otherV = other.Get(key);

                if (thisV == otherV)
                {
                    continue;
                }
                // the two values must be unequal, so if either is null, the other isn't
                if (thisV == null || otherV == null)
                {
                    return false;
                }

                if (! thisV.Equals(otherV))
                {
                    return false;
                }
            }

            return true;
        }


        private bool Equals(ArrayCoreMap other)
        {
            Dictionary<Tuple<ICoreMap, ICoreMap>, Boolean> calledMap = EqualsCalled.Value;
            bool createdCalledMap = (calledMap == null);
            if (createdCalledMap)
            {
                calledMap = new Dictionary<Tuple<ICoreMap, ICoreMap>, bool>();
                EqualsCalled.Value = calledMap;
            }

            // Note that for the purposes of recursion, we assume the two maps
            // are equals.  The two maps will therefore be equal if they
            // encounter each other again during the recursion unless there is
            // some other key that causes the equality to fail.
            // We do not need to later put false, as the entire call to equals
            // will unwind with false if any one equality check returns false.
            // TODO: since we only ever keep "true", we would rather use a
            // TwoDimensionalSet, but no such thing exists
            if (calledMap.ContainsKey(new Tuple<ICoreMap, ICoreMap>(this, other)))
            {
                return true;
            }
            bool result = true;
            calledMap.Add(new Tuple<ICoreMap, ICoreMap>(this, other), true);
            calledMap.Add(new Tuple<ICoreMap, ICoreMap>(other, this), true);

            if (this.psize != other.psize)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < this.psize; i++)
                {
                    // test if other contains this key,value pair
                    bool matched = false;
                    for (int j = 0; j < other.psize; j++)
                    {
                        if (this.keys[i] == other.keys[j])
                        {
                            if ((this.values[i] == null && other.values[j] != null) ||
                                (this.values[i] != null && other.values[j] == null))
                            {
                                matched = false;
                                break;
                            }

                            if ((this.values[i] == null && other.values[j] == null) ||
                                (this.values[i].Equals(other.values[j])))
                            {
                                matched = true;
                                break;
                            }
                        }
                    }

                    if (!matched)
                    {
                        result = false;
                        break;
                    }
                }
            }

            if (createdCalledMap)
            {
                EqualsCalled.Value = null;
            }
            return result;
        }

        /// <summary>
        /// Keeps track of which ArrayCoreMaps have had hashCode called on them.
        /// We do not want to loop forever when there are cycles in the annotation graph.
        /// This is kept on a per-thread basis so that each thread where hashCode gets called can keep track of its own state.
        /// When a call to ToString is about to return, this is reset to null for that particular thread.
        /// </summary>
        private static readonly ThreadLocal<IdentityHashSet<ICoreMap>> HashCodeCalled =
            new ThreadLocal<IdentityHashSet<ICoreMap>>();

        public override int GetHashCode()
        {
            IdentityHashSet<ICoreMap> calledSet = HashCodeCalled.Value;
            bool createdCalledSet = (calledSet == null);
            if (createdCalledSet)
            {
                calledSet = new IdentityHashSet<ICoreMap>();
                HashCodeCalled.Value = calledSet;
            }

            if (calledSet.Contains(this))
            {
                return 0;
            }

            calledSet.Add(this);

            int keysCode = 0;
            int valuesCode = 0;
            for (int i = 0; i < psize; i++)
            {
                keysCode += (i < keys.Length && values[i] != null ? keys[i].GetHashCode() : 0);
                valuesCode += (i < values.Length && values[i] != null ? values[i].GetHashCode() : 0);
            }

            if (createdCalledSet)
            {
                HashCodeCalled.Value = null;
            }
            else
            {
                // Remove the object after processing is complete so that if
                // there are multiple instances of this CoreMap in the overall
                // object graph, they each have their hash code calculated.
                // TODO: can we cache this for later?
                calledSet.Remove(this);
            }
            return keysCode*37 + valuesCode;
        }


        // TODO: make prettyLog work in the situation of loops
        // in the object graph

        /**
   * {@inheritDoc}
   */
        /*public void prettyLog(RedwoodChannels channels, string description) {
    Redwood.startTrack(description);

    // sort keys by class name
    List<Class> sortedKeys = new ArrayList<Class>(this.keySet());
    Collections.sort(sortedKeys,
        (a, b) -> a.getCanonicalName().compareTo(b.getCanonicalName()));

    // log key/value pairs
    for (Class key : sortedKeys) {
      string keyName = key.getCanonicalName().replace("class ", "");
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