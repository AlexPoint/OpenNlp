using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{

    /// <summary>
    ///  An extension of {@link ArrayCoreMap} with an immutable set of key,value
    /// pairs that is used for equality and hashcode comparisons.
    /// 
    /// @author dramage
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class HashableCoreMap : ArrayCoreMap
    {
        /// <summary>Set of immutable keys</summary>
        private readonly Set<Type> immutableKeys;

        /// <summary>Pre-computed hashcode</summary>
        private readonly int hashcode;

        /// <summary>
        /// Creates an instance of HashableCoreMap with initial key,value pairs
        /// for the immutable, hashable keys as provided in the given map.
        /// </summary>
        public HashableCoreMap(Dictionary<Type, Object> hashkey)
        {
            int keyHashcode = 0;
            int valueHashcode = 0;

            foreach ( /*Map.Entry<Class<? extends TypesafeMap.Key<?>>,Object>*/var entry in hashkey /*.entrySet()*/)
            {
                // NB it is important to compose these hashcodes in an order-independent
                // way, so we just add them all here.
                keyHashcode += /*entry.getKey().hashCode()*/ entry.Key.GetHashCode();
                valueHashcode += /*entry.getValue().hashCode()*/ entry.Value.GetHashCode();

                base.Set( /*(Class)*/ entry.Key, entry.Value);
            }

            this.immutableKeys = new Set<Type>(hashkey.Keys);
            this.hashcode = keyHashcode*31 + valueHashcode;
        }

        /**
   * Creates an instance by copying values from the given other CoreMap,
   * using the values it associates with the given set of hashkeys for
   * the immutable, hashable keys used by hashcode and equals.
   */
        /*public HashableCoreMap<T>(ArrayCoreMap other, Set<Key<T>> hashkey):base(other) {
        
    int keyHashcode = 0;
    int valueHashcode = 0;
    
    foreach (Key<T> key in hashkey) {
      // NB it is important to compose these hashcodes in an order-independent
      // way, so we just add them all here.
      keyHashcode += key.hashCode();
      valueHashcode += base.get((Class)key).hashCode();
    }
    
    this.immutableKeys = hashkey;
    this.hashcode = keyHashcode * 31 + valueHashcode;
  }*/

        /// <summary>
        /// Sets the value associated with the given key; if the the key is one
        /// of the hashable keys, throws an exception.
        /// </summary>
        /// <exception cref="HashableCoreMapException">Attempting to set the value for an immutable, hashable key.</exception>
        public override /*<VALUE> VALUE*/ Object Set(Type key, Object value)
        {

            if (immutableKeys.Contains(key))
            {
                throw new HashableCoreMapException("Attempt to change value " +
                                                   "of immutable field " + key.GetType().Name);
            }

            return base.Set(key, value);
        }

        public override int GetHashCode()
        {
            return hashcode;
        }

        public override bool Equals(Object o)
        {
            if (o is HashableCoreMap)
            {
                var other = (HashableCoreMap) o;
                if (!other.immutableKeys.Equals(this.immutableKeys))
                {
                    return false;
                }
                foreach (Type key in immutableKeys)
                {
                    if (!this.Get( /*(Class)*/key).Equals(other.Get( /*(Class)*/key)))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return base.Equals(o);
            }
        }

        /// <summary>
        /// An exception thrown when attempting to change the value associated
        /// with an (immutable) hash key in a HashableCoreMap.
        /// 
        /// @author dramage
        /// </summary>
        public class HashableCoreMapException : SystemException
        {

            public HashableCoreMapException(string message) : base(message)
            {
            }

        }
    }
}