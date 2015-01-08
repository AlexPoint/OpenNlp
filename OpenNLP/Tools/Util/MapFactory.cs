using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /// <summary>
    /// A factory class for vending different sorts of Maps.
    /// 
    /// @author Dan Klein (klein@cs.stanford.edu)
    /// @author Kayur Patel (kdpatel@cs)
    /// Code ...
    /// </summary>
    public abstract class MapFactory<K, V>
    {
        //public static readonly MapFactory HASH_MAP_FACTORY = new HashMapFactory();

        //public static readonly MapFactory IDENTITY_HASH_MAP_FACTORY = new IdentityHashMapFactory();

        //private static readonly MapFactory WEAK_HASH_MAP_FACTORY = new WeakHashMapFactory();

        //private static readonly MapFactory TREE_MAP_FACTORY = new TreeMapFactory();

        //private static readonly MapFactory LINKED_HASH_MAP_FACTORY = new LinkedHashMapFactory();

        //private static readonly MapFactory ARRAY_MAP_FACTORY = new ArrayMapFactory();

        /// <summary>
        /// Return a MapFactory that returns a HashMap.
        /// Implementation note: This method uses the same trick as the methods
        /// like emptyMap() introduced in the Collections class in JDK1.5 where
        /// callers can call this method with apparent type safety because this
        /// method takes the hit for the cast.
        /// </summary>
        public static /*<K,V>*/ MapFactory<K, V> hashMapFactory<K, V>()
        {
            return new HashMapFactory<K, V>();
        }

        /** Return a MapFactory that returns an IdentityHashMap.
   *  <i>Implementation note: This method uses the same trick as the methods
   *  like emptyMap() introduced in the Collections class in JDK1.5 where
   *  callers can call this method with apparent type safety because this
   *  method takes the hit for the cast.
   *
   *  @return A MapFactory that makes a HashMap.
   */
        /*public static /*<K,V>#1# MapFactory<K,V> identityHashMapFactory<K,V>() {
    return IDENTITY_HASH_MAP_FACTORY;
  }*/

        /** Return a MapFactory that returns a WeakHashMap.
   *  <i>Implementation note: This method uses the same trick as the methods
   *  like emptyMap() introduced in the Collections class in JDK1.5 where
   *  callers can call this method with apparent type safety because this
   *  method takes the hit for the cast.
   *
   *  @return A MapFactory that makes a WeakHashMap.
   */
        /*public static /*<K,V>#1# MapFactory<K,V> weakHashMapFactory<K,V>() {
    return WEAK_HASH_MAP_FACTORY;
  }*/

        /** Return a MapFactory that returns a TreeMap.
   *  <i>Implementation note: This method uses the same trick as the methods
   *  like emptyMap() introduced in the Collections class in JDK1.5 where
   *  callers can call this method with apparent type safety because this
   *  method takes the hit for the cast.
   *
   *  @return A MapFactory that makes an TreeMap.
   */
        /*public static /*<K,V>#1# MapFactory<K,V> treeMapFactory<K,V>() {
    return TREE_MAP_FACTORY;
  }*/

        /** 
   * Return a MapFactory that returns a TreeMap with the given Comparator.
   */
        /*public static /*<K,V>#1# MapFactory<K,V> treeMapFactory<K,V>(Comparator<? super K> comparator) {
    return new TreeMapFactory<K,V>(comparator);
  }*/

        /// <summary>
        /// Return a MapFactory that returns an LinkedHashMap.
        /// Implementation note: This method uses the same trick as the methods
        /// like emptyMap() introduced in the Collections class in JDK1.5 where
        /// callers can call this method with apparent type safety because this
        /// method takes the hit for the cast.
        /// </summary>
        public static /*<K,V>*/ MapFactory<K, V> linkedHashMapFactory<K, V>()
        {
            return new LinkedHashMapFactory<K, V>();
        }

        /// <summary>
        /// Return a MapFactory that returns an ArrayMap.
        /// Implementation note: This method uses the same trick as the methods
        /// like emptyMap() introduced in the Collections class in JDK1.5 where
        /// callers can call this method with apparent type safety because this
        /// method takes the hit for the cast.
        /// </summary>
        public static /*<K,V>*/ MapFactory<K, V> arrayMapFactory<K, V>()
        {
            return new ArrayMapFactory<K, V>();
        }



        private class HashMapFactory<K, V> : MapFactory<K, V>
        {
            public override Dictionary<K, V> NewMap()
            {
                return new Dictionary<K, V>();
            }

            public override Dictionary<K, V> NewMap(int initCapacity)
            {
                return new Dictionary<K, V>(initCapacity);
            }

            public override Set<K> NewSet()
            {
                return new Set<K>();
            }

            public override /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map)
            {
                map = new Dictionary<K1, V1>();
                return map;
            }

            public override /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity)
            {
                map = new Dictionary<K1, V1>(initCapacity);
                return map;
            }

        } // end class HashMapFactory


        /*private class IdentityHashMapFactory<K,V> : MapFactory<K,V> {

            public Dictionary<K,V> newMap() {
              return new IdentityHashDictionary<K,V>();
            }

            public Dictionary<K,V> newMap(int initCapacity) {
              return new IdentityHashDictionary<K,V>(initCapacity);
            }

            public Set<K> newSet() {
              return Collections.newSetFromMap(new IdentityHashDictionary<K, Boolean>());
            }

            public /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map) {
              map = new IdentityHashDictionary<K1,V1>();
              return map;
            }

            public /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map, int initCapacity) {
              map = new IdentityHashDictionary<K1,V1>(initCapacity);
              return map;
            }

          } // end class IdentityHashMapFactory*/


        /*private static class WeakHashMapFactory<K,V> : MapFactory<K,V> {

            public override Dictionary<K,V> newMap() {
              return new WeakHashDictionary<K,V>();
            }

            public override Dictionary<K,V> newMap(int initCapacity) {
              return new WeakHashDictionary<K,V>(initCapacity);
            }

            public override Set<K> newSet() {
              return Collections.newSetFromMap(new WeakHashDictionary<K, Boolean>());
            }

            public override /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map) {
              //map = new WeakHashDictionary<K1,V1>();
              map = new Dictionary<K1,V1>();
              return map;
            }

            public override /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map, int initCapacity) {
              //map = new WeakHashDictionary<K1,V1>(initCapacity);
              map = new Dictionary<K1,V1>(initCapacity);
              return map;
            }

          } // end class WeakHashMapFactory*/


        /*private static class TreeMapFactory<K,V> : MapFactory<K,V> {

            private readonly Comparator<? super K> comparator;

            public TreeMapFactory() {
              this.comparator = null;
            }

            public TreeMapFactory(Comparator<? super K> comparator) {
              this.comparator = comparator;
            }

            public override Dictionary<K,V> newMap() {
              //return comparator == null ? new TreeDictionary<K,V>() : new TreeDictionary<K,V>(comparator);
              return comparator == null ? new Dictionary<K,V>() : new Dictionary<K,V>(comparator);
            }

            public override Dictionary<K,V> newMap(int initCapacity) {
              return newMap();
            }

            public override Set<K> newSet() {
              return comparator == null ? new TreeSet<K>() : new TreeSet<K>(comparator);
            }

            public override/*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map) {
              if (comparator == null) {
                throw new UnsupportedOperationException();
              }
              map = new TreeDictionary<K1,V1>();
              return map;
            }

            public override/*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map, int initCapacity) {
              if (comparator == null) {
                throw new NotSupportedException();
              }
              //map = new TreeDictionary<K1,V1>();
              map = new Dictionary<K1,V1>();
              return map;
            }

          } // end class TreeMapFactory*/

        private class LinkedHashMapFactory<K, V> : MapFactory<K, V>
        {
            public override Dictionary<K, V> NewMap()
            {
                //return new LinkedHashDictionary<K,V>();
                return new Dictionary<K, V>();
            }

            public override Dictionary<K, V> NewMap(int initCapacity)
            {
                return NewMap();
            }

            public override Set<K> NewSet()
            {
                return new HashSet<K>();
            }

            public override /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map)
            {
                //map = new LinkedHashDictionary<K1,V1>();
                map = new Dictionary<K1, V1>();
                return map;
            }

            public override /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity)
            {
                //map = new LinkedHashDictionary<K1,V1>();
                map = new Dictionary<K1, V1>();
                return map;
            }

        } // end class LinkedHashMapFactory


        private class ArrayMapFactory<K, V> : MapFactory<K, V>
        {
            public override Dictionary<K, V> NewMap()
            {
                //return new ArrayDictionary<K,V>();
                return new Dictionary<K, V>();
            }

            public override Dictionary<K, V> NewMap(int initCapacity)
            {
                //return new ArrayDictionary<K,V>(initCapacity);
                return new Dictionary<K, V>(initCapacity);
            }

            public override Set<K> NewSet()
            {
                return new Set<K>();
            }

            public override /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map)
            {
                return new /*ArrayDictionary*/ Dictionary<K1, V1>();
            }

            public override /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity)
            {
                //map = new ArrayDictionary<K1,V1>(initCapacity);
                map = new Dictionary<K1, V1>(initCapacity);
                return map;
            }

        } // end class ArrayMapFactory

        /// <summary>
        /// Returns a new non-parameterized map of a particular sort
        /// </summary>
        public abstract Dictionary<K, V> NewMap();

        /// <summary>
        /// Returns a new non-parameterized map of a particular sort with an initial capacity.
        /// </summary>
        /// <param name="initCapacity">initial capacity of the map</param>
        /// <returns> A new non-parameterized map of a particular sort with an initial capacity</returns>
        public abstract Dictionary<K, V> NewMap(int initCapacity);

        /// <summary>
        /// A set with the same <code>K</code> parameterization of the Maps
        /// </summary>
        public abstract Set<K> NewSet();

        /// <summary>
        /// A method to get a parameterized (genericized) map out.
        /// </summary>
        /// <param name="map">A type-parameterized {@link Map} argument</param>
        /// <returns>A {@link Map} with type-parameterization identical to that of the argument</returns>
        public abstract /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map);

        public abstract /*<K1, V1>*/ Dictionary<K1, V1> SetMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity);
    }
}