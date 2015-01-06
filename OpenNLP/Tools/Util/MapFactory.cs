using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /**
 * A factory class for vending different sorts of Maps.
 *
 * @author Dan Klein (klein@cs.stanford.edu)
 * @author Kayur Patel (kdpatel@cs)
 */

    public abstract class MapFactory<K, V>
    {
        // allow people to write subclasses
        protected MapFactory()
        {
        }

        private static readonly long serialVersionUID = 4529666940763477360L;

        //@SuppressWarnings("unchecked")
        //public static readonly MapFactory HASH_MAP_FACTORY = new HashMapFactory();

        //@SuppressWarnings("unchecked")
        //public static readonly MapFactory IDENTITY_HASH_MAP_FACTORY = new IdentityHashMapFactory();

        //@SuppressWarnings("unchecked")
        //private static readonly MapFactory WEAK_HASH_MAP_FACTORY = new WeakHashMapFactory();

        //@SuppressWarnings("unchecked")
        //private static readonly MapFactory TREE_MAP_FACTORY = new TreeMapFactory();

        //@SuppressWarnings("unchecked")
        //private static readonly MapFactory LINKED_HASH_MAP_FACTORY = new LinkedHashMapFactory();

        //@SuppressWarnings("unchecked")
        //private static readonly MapFactory ARRAY_MAP_FACTORY = new ArrayMapFactory();


        /** Return a MapFactory that returns a HashMap.
   *  <i>Implementation note: This method uses the same trick as the methods
   *  like emptyMap() introduced in the Collections class in JDK1.5 where
   *  callers can call this method with apparent type safety because this
   *  method takes the hit for the cast.
   *
   *  @return A MapFactory that makes a HashMap.
   */
        //@SuppressWarnings("unchecked")
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
        //@SuppressWarnings("unchecked")
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
        //@SuppressWarnings("unchecked")
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
        //@SuppressWarnings("unchecked")
        /*public static /*<K,V>#1# MapFactory<K,V> treeMapFactory<K,V>() {
    return TREE_MAP_FACTORY;
  }*/

        /** 
   * Return a MapFactory that returns a TreeMap with the given Comparator.
   */
        /*public static /*<K,V>#1# MapFactory<K,V> treeMapFactory<K,V>(Comparator<? super K> comparator) {
    return new TreeMapFactory<K,V>(comparator);
  }*/

        /** Return a MapFactory that returns an LinkedHashMap.
   *  <i>Implementation note: This method uses the same trick as the methods
   *  like emptyMap() introduced in the Collections class in JDK1.5 where
   *  callers can call this method with apparent type safety because this
   *  method takes the hit for the cast.
   *
   *  @return A MapFactory that makes an LinkedHashMap.
   */
        //@SuppressWarnings("unchecked")
        public static /*<K,V>*/ MapFactory<K, V> linkedHashMapFactory<K, V>()
        {
            return new LinkedHashMapFactory<K, V>();
        }

        /** Return a MapFactory that returns an ArrayMap.
   *  <i>Implementation note: This method uses the same trick as the methods
   *  like emptyMap() introduced in the Collections class in JDK1.5 where
   *  callers can call this method with apparent type safety because this
   *  method takes the hit for the cast.
   *
   *  @return A MapFactory that makes an ArrayMap.
   */
        //@SuppressWarnings("unchecked")
        public static /*<K,V>*/ MapFactory<K, V> arrayMapFactory<K, V>()
        {
            return new ArrayMapFactory<K, V>();
        }



        private class HashMapFactory<K, V> : MapFactory<K, V>
        {

            private new static readonly long serialVersionUID = -9222344631596580863L;

            //@Override
            public override Dictionary<K, V> newMap()
            {
                return new Dictionary<K, V>();
            }

            //@Override
            public override Dictionary<K, V> newMap(int initCapacity)
            {
                return new Dictionary<K, V>(initCapacity);
            }

            //@Override
            public override Set<K> newSet()
            {
                return new Set<K>();
            }

            //@Override
            public override /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map)
            {
                map = new Dictionary<K1, V1>();
                return map;
            }

            //@Override
            public override /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity)
            {
                map = new Dictionary<K1, V1>(initCapacity);
                return map;
            }

        } // end class HashMapFactory


        /*private class IdentityHashMapFactory<K,V> : MapFactory<K,V> {

    private static readonly long serialVersionUID = -9222344631596580863L;

    //@Override
    public Dictionary<K,V> newMap() {
      return new IdentityHashDictionary<K,V>();
    }

    //@Override
    public Dictionary<K,V> newMap(int initCapacity) {
      return new IdentityHashDictionary<K,V>(initCapacity);
    }

    //@Override
    public Set<K> newSet() {
      return Collections.newSetFromMap(new IdentityHashDictionary<K, Boolean>());
    }

    //@Override
    public /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map) {
      map = new IdentityHashDictionary<K1,V1>();
      return map;
    }

    //@Override
    public /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map, int initCapacity) {
      map = new IdentityHashDictionary<K1,V1>(initCapacity);
      return map;
    }

  } // end class IdentityHashMapFactory*/


        /*private static class WeakHashMapFactory<K,V> : MapFactory<K,V> {

    private static readonly long serialVersionUID = 4790014244304941000L;

    //@Override
    public override Dictionary<K,V> newMap() {
      return new WeakHashDictionary<K,V>();
    }

    //@Override
    public override Dictionary<K,V> newMap(int initCapacity) {
      return new WeakHashDictionary<K,V>(initCapacity);
    }

    //@Override
    public override Set<K> newSet() {
      return Collections.newSetFromMap(new WeakHashDictionary<K, Boolean>());
    }


    //@Override
    public override /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map) {
      //map = new WeakHashDictionary<K1,V1>();
      map = new Dictionary<K1,V1>();
      return map;
    }

    //@Override
    public override /*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map, int initCapacity) {
      //map = new WeakHashDictionary<K1,V1>(initCapacity);
      map = new Dictionary<K1,V1>(initCapacity);
      return map;
    }

  } // end class WeakHashMapFactory*/


        /*private static class TreeMapFactory<K,V> : MapFactory<K,V> {

    private static readonly long serialVersionUID = -9138736068025818670L;

    private readonly Comparator<? super K> comparator;

    public TreeMapFactory() {
      this.comparator = null;
    }

    public TreeMapFactory(Comparator<? super K> comparator) {
      this.comparator = comparator;
    }

    //@Override
    public override Dictionary<K,V> newMap() {
      //return comparator == null ? new TreeDictionary<K,V>() : new TreeDictionary<K,V>(comparator);
      return comparator == null ? new Dictionary<K,V>() : new Dictionary<K,V>(comparator);
    }

    //@Override
    public override Dictionary<K,V> newMap(int initCapacity) {
      return newMap();
    }

    //@Override
    public override Set<K> newSet() {
      return comparator == null ? new TreeSet<K>() : new TreeSet<K>(comparator);
    }


    //@Override
    public override/*<K1, V1>#1# Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1,V1> map) {
      if (comparator == null) {
        throw new UnsupportedOperationException();
      }
      map = new TreeDictionary<K1,V1>();
      return map;
    }

    //@Override
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

            private new static readonly long serialVersionUID = -9138736068025818671L;

            //@Override
            public override Dictionary<K, V> newMap()
            {
                //return new LinkedHashDictionary<K,V>();
                return new Dictionary<K, V>();
            }

            //@Override
            public override Dictionary<K, V> newMap(int initCapacity)
            {
                return newMap();
            }

            //@Override
            public override Set<K> newSet()
            {
                return new HashSet<K>();
            }


            //@Override
            public override /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map)
            {
                //map = new LinkedHashDictionary<K1,V1>();
                map = new Dictionary<K1, V1>();
                return map;
            }

            //@Override
            public override /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity)
            {
                //map = new LinkedHashDictionary<K1,V1>();
                map = new Dictionary<K1, V1>();
                return map;
            }

        } // end class LinkedHashMapFactory


        private class ArrayMapFactory<K, V> : MapFactory<K, V>
        {

            private new static readonly long serialVersionUID = -5855812734715185523L;

            //@Override
            public override Dictionary<K, V> newMap()
            {
                //return new ArrayDictionary<K,V>();
                return new Dictionary<K, V>();
            }

            //@Override
            public override Dictionary<K, V> newMap(int initCapacity)
            {
                //return new ArrayDictionary<K,V>(initCapacity);
                return new Dictionary<K, V>(initCapacity);
            }

            //@Override
            public override Set<K> newSet()
            {
                return new Set<K>();
            }

            //@Override
            public override /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map)
            {
                return new /*ArrayDictionary*/ Dictionary<K1, V1>();
            }

            //@Override
            public override /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity)
            {
                //map = new ArrayDictionary<K1,V1>(initCapacity);
                map = new Dictionary<K1, V1>(initCapacity);
                return map;
            }

        } // end class ArrayMapFactory


        /**
   * Returns a new non-parameterized map of a particular sort.
   *
   * @return A new non-parameterized map of a particular sort
   */
        public abstract Dictionary<K, V> newMap();

        /**
   * Returns a new non-parameterized map of a particular sort with an initial capacity.
   *
   * @param initCapacity initial capacity of the map
   * @return A new non-parameterized map of a particular sort with an initial capacity
   */
        public abstract Dictionary<K, V> newMap(int initCapacity);

        /**
   * A set with the same <code>K</code> parameterization of the Maps.
   */
        public abstract Set<K> newSet();

        /**
   * A method to get a parameterized (genericized) map out.
   *
   * @param map A type-parameterized {@link Map} argument
   * @return A {@link Map} with type-parameterization identical to that of
   *         the argument.
   */
        public abstract /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map);

        public abstract /*<K1, V1>*/ Dictionary<K1, V1> setMap<K1, V1>(Dictionary<K1, V1> map, int initCapacity);
    }
}