using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /**
 * An extension of {@link ArrayCoreMap} with an immutable set of key,value
 * pairs that is used for equality and hashcode comparisons.
 * 
 * @author dramage
 */
    public class HashableCoreMap:ArrayCoreMap
    {
        /** Set of immutable keys */
  private readonly Set<Key<object>> immutableKeys;
  
  /** Pre-computed hashcode */
  private readonly int hashcode;
  
  /**
   * Creates an instance of HashableCoreMap with initial key,value pairs
   * for the immutable, hashable keys as provided in the given map.
   */
  //@SuppressWarnings("unchecked")
  public HashableCoreMap(Map<Class<? extends TypesafeMap.Key<?>>,Object> hashkey) {
    int keyHashcode = 0;
    int valueHashcode = 0;
    
    for (Map.Entry<Class<? extends TypesafeMap.Key<?>>,Object> entry : hashkey.entrySet()) {
      // NB it is important to compose these hashcodes in an order-independent
      // way, so we just add them all here.
      keyHashcode += entry.getKey().hashCode();
      valueHashcode += entry.getValue().hashCode();
      
      base.set((Class) entry.getKey(), entry.getValue());
    }
    
    this.immutableKeys = hashkey.keySet();
    this.hashcode = keyHashcode * 31 + valueHashcode;
  }
  
  /**
   * Creates an instance by copying values from the given other CoreMap,
   * using the values it associates with the given set of hashkeys for
   * the immutable, hashable keys used by hashcode and equals.
   */
  //@SuppressWarnings("unchecked")
  public HashableCoreMap<T>(ArrayCoreMap other, Set<Key<T>> hashkey):base(other) {
        
    int keyHashcode = 0;
    int valueHashcode = 0;
    
    for (Key<T> key : hashkey) {
      // NB it is important to compose these hashcodes in an order-independent
      // way, so we just add them all here.
      keyHashcode += key.hashCode();
      valueHashcode += base.get((Class)key).hashCode();
    }
    
    this.immutableKeys = hashkey;
    this.hashcode = keyHashcode * 31 + valueHashcode;
  }
  
  /**
   * Sets the value associated with the given key; if the the key is one
   * of the hashable keys, throws an exception.
   * 
   * @throws HashableCoreMapException Attempting to set the value for an
   *   immutable, hashable key.
   */
  //@Override
  public /*<VALUE> VALUE*/T set<T>(Key<T> key, T value) {
    
    if (immutableKeys.contains(key)) {
      throw new HashableCoreMapException("Attempt to change value " +
      		"of immutable field "+key.getSimpleName());
    }
    
    return base.set(key, value);
  }
  
  /**
   * Provides a hash code based on the immutable keys and values provided
   * to the constructor.
   */
  //@Override
  public int hashCode() {
    return hashcode;
  }
  
  /**
   * If the provided object is a HashableCoreMap, equality is based only
   * upon the values of the immutable hashkeys; otherwise, defaults to
   * behavior of the superclass's equals method.
   */
  //    @SuppressWarnings("unchecked")
  //@Override
  public bool equals(Object o) {
    if (o is HashableCoreMap) {
      HashableCoreMap other = (HashableCoreMap) o;
      if (!other.immutableKeys.equals(this.immutableKeys)) {
        return false;
      }
      foreach (Key<object> key in immutableKeys) {
        if (!this.get((Class)key).Equals(other.get((Class)key))) {
          return false;
        }
      }
      return true;
    } else {
      return base.equals(o);
    }
  }
  
  private static readonly long serialVersionUID = 1L;
  
  //
  // Exception type
  //
  
  /**
   * An exception thrown when attempting to change the value associated
   * with an (immutable) hash key in a HashableCoreMap.
   * 
   * @author dramage
   */
  public class HashableCoreMapException: SystemException {

    public HashableCoreMapException(String message):base(message) {}
    
    /**
     * 
     */
    private static readonly long serialVersionUID = 1L;
  }
    }
}
