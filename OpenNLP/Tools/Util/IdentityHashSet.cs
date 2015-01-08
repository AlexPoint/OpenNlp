using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{

    /// <summary>
    /// This class provides a <code>IdentityHashMap</code>-backed
    /// implementation of the <code>Set</code> interface.
    /// This means that whether an object is an element of the set depends 
    /// on whether it is == (rather than <code>equals()</code>) to an element of the set.
    /// This is different from a normal <code>HashSet</code>, where set membership
    /// depends on <code>equals()</code>, rather than ==.
    /// 
    /// Each element in the set is a key in the backing IdentityHashMap; each key
    /// maps to a static token, denoting that the key does, in fact, exist.
    /// 
    /// Most operations are O(1), assuming no hash collisions.  In the worst
    /// case (where all hashes collide), operations are O(n).
    /// 
    /// @author Bill MacCartney
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class IdentityHashSet<T> : Set<T>
        where T : class
    {
        // todo: The Java bug database notes that "From 1.6, an identity hash set can be created by Collections.newSetFromMap(new IdentityHashMap())."

        // INSTANCE VARIABLES -------------------------------------------------

        // the IdentityHashMap which backs this set
        private readonly IdentityDictionary<T, Boolean> map;
        

        // CONSTRUCTORS ---------------------------------------------------------

        /** Construct a new, empty IdentityHashSet whose backing IdentityHashMap
   *  has the default expected maximum size (21);
   */

        public IdentityHashSet()
        {
            map = new IdentityDictionary<T, Boolean>();
        }

        /** Construct a new, empty IdentityHashSet whose backing IdentityHashMap
   *  has the specified expected maximum size.  Putting more than the
   *  expected number of elements into the set may cause the internal data
   *  structure to grow, which may be somewhat time-consuming.
   *
   * @param expectedMaxSize the expected maximum size of the set.
   */
        /*public IdentityHashSet(int expectedMaxSize) {
    map = new IdentityDictionary<T, Boolean>(expectedMaxSize);
  }*/
        
        /// <summary>
        /// Construct a new IdentityHashSet with the same elements as the supplied
        /// Collection (eliminating any duplicates, of course); the backing
        /// IdentityHashMap will have the default expected maximum size (21).
        /// </summary>
        /// <param name="c">
        /// a Collection containing the elements with which this set will be initialized.
        /// </param>
        public IdentityHashSet(ICollection<T> c)
        {
            map = new IdentityDictionary<T, Boolean>();
            AddAll(c);
        }


        // PUBLIC METHODS ---------------------------------------------------------

        /// <summary>
        /// dds the specified element to this set if it is not already present.
        /// Remember that this set implementation uses == (not <code>equals()</code>)
        /// to test whether an element is present in the set.
        /// </summary>
        /// <param name="o">element to add to this set</param>
        /// <returns>true if the element was added, false otherwise</returns>
        public override bool Add(T o)
        {
            if (map.ContainsKey(o))
            {
                return false;
            }
            else
            {
                InternalAdd(o);
                return true;
            }
        }

        /// <summary>
        /// Removes all of the elements from this set.
        /// </summary>
        public void Clear()
        {
            map.Clear();
        }

        /** Returns a shallow copy of this <code>IdentityHashSet</code> instance:
   *  the elements themselves are not cloned.
   *
   *  @return a shallow copy of this set.
   */
        /*public Object clone() {
    Iterator<T> it = iterator();
    IdentityHashSet<T> clone = new IdentityHashSet<T>(/*size() * 2#1#);
    while (it.hasNext()) {
      clone.internalAdd(it.next());
    }
    return clone;
  }*/

        /// <summary>
        /// Returns true if this set contains the specified element.
        /// Remember that this set implementation uses == (not <code>equals()</code>)
        /// to test whether an element is present in the set.
        /// </summary>
        /// <param name="o">Element whose presence in this set is to be</param>
        /// <returns><code>true</code> if this set contains the specified element.</returns>
        public bool Contains(T o)
        {
            return map.ContainsKey(o);
        }

        /// <summary>
        /// Returns <code>true</code> if this set contains no elements.
        /// </summary>
        public override bool IsEmpty()
        {
            return !map.Any();
        }

        /** Returns an iterator over the elements in this set. The elements are
   *  returned in no particular order.
   *
   *  @return an <code>Iterator</code> over the elements in this set.
   */
        /*public Iterator<E> iterator() {
    return map.keySet().iterator();
  }*/

        /// <summary>
        /// Removes the specified element from this set if it is present.
        /// Remember that this set implementation uses == (not <code>equals()</code>) 
        /// to test whether an element is present in the set.
        /// </summary>
        /// <param name="o">Object to be removed from this set, if present.</param>
        /// <returns><code>true</code> if the set contained the specified element.</returns>
        public override bool Remove(T o)
        {
            return map.Remove(o);
        }

        /// <summary>
        /// Returns the number of elements in this set (its cardinality).
        /// </summary>
        public int Size()
        {
            return map.Count;
        }


        // PRIVATE METHODS -----------------------------------------------------------

        /// <summary>
        /// Adds the supplied element to this set.
        /// This private method is used internally [by clone()] instead of add(), because add() 
        /// can be overridden to do unexpected things.
        /// </summary>
        /// <param name="o">the element to add to this set</param>
        private void InternalAdd(T o)
        {
            map.Add(o, true);
        }

        /** Serialize this Object in a manner which is binary-compatible with the
   *  JDK.
   #1#
  /*private void writeObject(ObjectOutputStream s) throws IOException {
    Iterator<E> it = iterator();
    s.writeInt(size() * 2);             // expectedMaxSize
    s.writeInt(size());
    while (it.hasNext())
      s.writeObject(it.next());
  }

  /** Deserialize this Object in a manner which is binary-compatible with
   *  the JDK.
   #1#
  private void readObject(ObjectInputStream s)
    throws IOException, ClassNotFoundException {
    int size, expectedMaxSize;
    Object o;

    expectedMaxSize = s.readInt();
    size = s.readInt();

    map = new IdentityHashMap<E, Boolean>(expectedMaxSize);
    for (int i = 0; i < size; i++) {
      o = s.readObject();
      internalAdd(ErasureUtils.<E>uncheckedCast(o));
    }
  }*/
    }
}