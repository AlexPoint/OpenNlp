using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /**
 * PriorityQueue with explicit double priority values.  Larger doubles are higher priorities.  BinaryHeap-backed.
 *
 * For each entry, uses ~ 24 (entry) + 16? (Map.Entry) + 4 (List entry) = 44 bytes?
 *
 * @author Dan Klein
 * @author Christopher Manning
 * @param <E> Type of elements in the priority queue
 */

    public class BinaryHeapPriorityQueue<E> : PriorityQueue<E>
    {
        /**
   * An {@code Entry} stores an object in the queue along with
   * its current location (array position) and priority.
   * uses ~ 8 (self) + 4 (key ptr) + 4 (index) + 8 (priority) = 24 bytes?
   */

        public sealed class Entry<E>
        {
            public E key;
            public int index;
            public double priority;

            //@Override
            public override string ToString()
            {
                return key + " at " + index + " (" + priority + ')';
            }
        }

        //@Override
        public bool HasNext()
        {
            return Size() > 0;
        }

        //@Override
        public E Next()
        {
            if (Size() == 0)
            {
                throw new KeyNotFoundException("Empty PQ");
            }
            return RemoveFirst();
        }

        //@Override
        public void Remove()
        {
            throw new KeyNotFoundException();
        }

        /**
   * {@code indexToEntry} maps linear array locations (not
   * priorities) to heap entries.
   */
        private readonly List<Entry<E>> indexToEntry;

        /**
   * {@code keyToEntry} maps heap objects to their heap
   * entries.
   */
        private readonly Dictionary<E, Entry<E>> keyToEntry;

        private Entry<E> Parent(Entry<E> entry)
        {
            int index = entry.index;
            return (index > 0 ? GetEntry((index - 1)/2) : null);
        }

        private Entry<E> LeftChild(Entry<E> entry)
        {
            int leftIndex = entry.index*2 + 1;
            return (leftIndex < Size() ? GetEntry(leftIndex) : null);
        }

        private Entry<E> RightChild(Entry<E> entry)
        {
            int index = entry.index;
            int rightIndex = index*2 + 2;
            return (rightIndex < Size() ? GetEntry(rightIndex) : null);
        }

        private int Compare(Entry<E> entryA, Entry<E> entryB)
        {
            int result = Compare(entryA.priority, entryB.priority);
            if (result != 0)
            {
                return result;
            }
            if ((entryA.key is IComparable<E>) && (entryB.key is IComparable<E>))
            {
                var key = (IComparable<E>) (entryA.key);
                return key.CompareTo(entryB.key);
            }
            return result;
        }

        private static int Compare(double a, double b)
        {
            double diff = a - b;
            if (diff > 0.0)
            {
                return 1;
            }
            if (diff < 0.0)
            {
                return -1;
            }
            return 0;
        }

        /**
   * Structural swap of two entries.
   *
   */

        private void Swap(Entry<E> entryA, Entry<E> entryB)
        {
            int indexA = entryA.index;
            int indexB = entryB.index;
            entryA.index = indexB;
            entryB.index = indexA;
            indexToEntry.Insert(indexA, entryB);
            indexToEntry.Insert(indexB, entryA);
        }

        /**
   * Remove the last element of the heap (last in the index array).
   */

        private void RemoveLastEntry()
        {
            var last = indexToEntry.Last();
            indexToEntry.Remove(last);
            keyToEntry.Remove(last.key);
        }

        /**
   * Get the entry by key (null if none).
   */

        private Entry<E> GetEntry(E key)
        {
            return keyToEntry[key];
        }

        /**
   * Get entry by index, exception if none.
   */

        private Entry<E> GetEntry(int index)
        {
            Entry<E> entry = indexToEntry[index];
            return entry;
        }

        private Entry<E> MakeEntry(E key)
        {
            var entry = new Entry<E>();
            entry.index = Size();
            entry.key = key;
            entry.priority = Double.NegativeInfinity;
            indexToEntry.Add(entry);
            keyToEntry.Add(key, entry);
            return entry;
        }

        /**
   * iterative heapify up: move item o at index up until correctly placed
   */

        private void HeapifyUp(Entry<E> entry)
        {
            while (true)
            {
                if (entry.index == 0)
                {
                    break;
                }
                Entry<E> parentEntry = Parent(entry);
                if (Compare(entry, parentEntry) <= 0)
                {
                    break;
                }
                Swap(entry, parentEntry);
            }
        }

        /**
   * On the assumption that
   * leftChild(entry) and rightChild(entry) satisfy the heap property,
   * make sure that the heap at entry satisfies this property by possibly
   * percolating the element entry downwards.  I've replaced the obvious
   * recursive formulation with an iterative one to gain (marginal) speed
   */

        private void HeapifyDown( /*final*/ Entry<E> entry)
        {
            Entry<E> bestEntry; // initialized below

            do
            {
                bestEntry = entry;

                Entry<E> leftEntry = LeftChild(entry);
                if (leftEntry != null)
                {
                    if (Compare(bestEntry, leftEntry) < 0)
                    {
                        bestEntry = leftEntry;
                    }
                }

                Entry<E> rightEntry = RightChild(entry);
                if (rightEntry != null)
                {
                    if (Compare(bestEntry, rightEntry) < 0)
                    {
                        bestEntry = rightEntry;
                    }
                }

                if (bestEntry != entry)
                {
                    // Swap min and current
                    Swap(bestEntry, entry);
                    // at start of next loop, we set currentIndex to largestIndex
                    // this indexation now holds current, so it is unchanged
                }
            } while (bestEntry != entry);
            // System.err.println("Done with heapify down");
            // verify();
        }

        private void Heapify(Entry<E> entry)
        {
            HeapifyUp(entry);
            HeapifyDown(entry);
        }


        /**
   * Finds the E with the highest priority, removes it,
   * and returns it.
   *
   * @return the E with highest priority
   */
        //@Override
        public E RemoveFirst()
        {
            E first = GetFirst();
            Remove(first);
            return first;
        }

        /**
   * Finds the E with the highest priority and returns it, without
   * modifying the queue.
   *
   * @return the E with minimum key
   */
        //@Override
        public E GetFirst()
        {
            if (IsEmpty())
            {
                throw new KeyNotFoundException();
            }
            return GetEntry(0).key;
        }

        /** {@inheritDoc} */
        //@Override
        public double GetPriority()
        {
            if (IsEmpty())
            {
                throw new KeyNotFoundException();
            }
            return GetEntry(0).priority;
        }

        /**
   * Searches for the object in the queue and returns it.  May be useful if
   * you can create a new object that is .equals() to an object in the queue
   * but is not actually identical, or if you want to modify an object that is
   * in the queue.
   *
   * @return null if the object is not in the queue, otherwise returns the
   * object.
   */

        public E GetObject(E key)
        {
            if (! Contains(key)) return default(E);
            Entry<E> e = GetEntry(key);
            return e.key;
        }

        /** {@inheritDoc} */
        //@Override
        public double GetPriority(E key)
        {
            Entry<E> entry = GetEntry(key);
            if (entry == null)
            {
                return Double.NegativeInfinity;
            }
            return entry.priority;
        }

        /**
   * Adds an object to the queue with the minimum priority
   * (Double.NEGATIVE_INFINITY).  If the object is already in the queue
   * with worse priority, this does nothing.  If the object is
   * already present, with better priority, it will NOT cause an
   * a decreasePriority.
   *
   * @param key an <code>E</code> value
   * @return whether the key was present before
   */
        //@Override
        public bool Add(E key)
        {
            if (Contains(key))
            {
                return false;
            }
            MakeEntry(key);
            return true;
        }

        /** {@inheritDoc} */
        //@Override
        public bool Add(E key, double priority)
        {
//    System.err.println("Adding " + key + " with priority " + priority);
            if (Add(key))
            {
                RelaxPriority(key, priority);
                return true;
            }
            return false;
        }


        //@SuppressWarnings("unchecked")
        //@Override
        public bool Remove(Object key)
        {
            E eKey = (E) key;
            Entry<E> entry = GetEntry(eKey);
            if (entry == null)
            {
                return false;
            }
            RemoveEntry(entry);
            return true;
        }

        private void RemoveEntry(Entry<E> entry)
        {
            Entry<E> lastEntry = GetLastEntry();
            if (entry != lastEntry)
            {
                Swap(entry, lastEntry);
                RemoveLastEntry();
                Heapify(lastEntry);
            }
            else
            {
                RemoveLastEntry();
            }
        }

        private Entry<E> GetLastEntry()
        {
            return GetEntry(Size() - 1);
        }

        /**
   * Promotes a key in the queue, adding it if it wasn't there already.  If the specified priority is worse than the current priority, nothing happens.  Faster than add if you don't care about whether the key is new.
   *
   * @param key an <code>Object</code> value
   * @return whether the priority actually improved.
   */
        //@Override
        public bool RelaxPriority(E key, double priority)
        {
            Entry<E> entry = GetEntry(key);
            if (entry == null)
            {
                entry = MakeEntry(key);
            }
            if (Compare(priority, entry.priority) <= 0)
            {
                return false;
            }
            entry.priority = priority;
            HeapifyUp(entry);
            return true;
        }

        /**
   * Demotes a key in the queue, adding it if it wasn't there already.  If the specified priority is better than the current priority, nothing happens.  If you decrease the priority on a non-present key, it will get added, but at it's old implicit priority of Double.NEGATIVE_INFINITY.
   *
   * @param key an <code>Object</code> value
   * @return whether the priority actually improved.
   */

        public bool DecreasePriority(E key, double priority)
        {
            Entry<E> entry = GetEntry(key);
            if (entry == null)
            {
                entry = MakeEntry(key);
            }
            if (Compare(priority, entry.priority) >= 0)
            {
                return false;
            }
            entry.priority = priority;
            HeapifyDown(entry);
            return true;
        }

        /**
   * Changes a priority, either up or down, adding the key it if it wasn't there already.
   *
   * @param key an <code>Object</code> value
   * @return whether the priority actually changed.
   */
        //@Override
        public bool ChangePriority(E key, double priority)
        {
            Entry<E> entry = GetEntry(key);
            if (entry == null)
            {
                entry = MakeEntry(key);
            }
            if (Compare(priority, entry.priority) == 0)
            {
                return false;
            }
            entry.priority = priority;
            Heapify(entry);
            return true;
        }

        /**
   * Checks if the queue is empty.
   *
   * @return a <code>bool</code> value
   */
        //@Override
        public bool IsEmpty()
        {
            return !indexToEntry.Any();
        }

        /**
   * Get the number of elements in the queue.
   *
   * @return queue size
   */
        //@Override
        public int Size()
        {
            return indexToEntry.Count;
        }

        /**
   * Returns whether the queue contains the given key.
   */
        //@SuppressWarnings("SuspiciousMethodCalls")
        //@Override
        public bool Contains(E key)
        {
            return keyToEntry.ContainsKey(key);
        }

        //@Override
        public List<E> ToSortedList()
        {
            var sortedList = new List<E>(Size());
            BinaryHeapPriorityQueue<E> queue = this.DeepCopy();
            while (!queue.IsEmpty())
            {
                sortedList.Add(queue.RemoveFirst());
            }
            return sortedList;
        }

        public BinaryHeapPriorityQueue<E> DeepCopy(MapFactory<E, Entry<E>> mapFactory)
        {
            var queue = new BinaryHeapPriorityQueue<E>(mapFactory);
            foreach (Entry<E> entry in keyToEntry.Values)
            {
                queue.RelaxPriority(entry.key, entry.priority);
            }
            return queue;
        }

        public BinaryHeapPriorityQueue<E> DeepCopy()
        {
            return DeepCopy(MapFactory<E, Entry<E>>.hashMapFactory<E, Entry<E>>());
        }

        //@Override
        public IEnumerator<E> Iterator()
        {
            return new ReadOnlyCollection<E>(ToSortedList()).GetEnumerator();
        }

        /**
   * Clears the queue.
   */
        //@Override
        public void Clear()
        {
            indexToEntry.Clear();
            keyToEntry.Clear();
        }

        //  private void verify() {
        //    for (int i = 0; i < indexToEntry.size(); i++) {
        //      if (i != 0) {
        //        // check ordering
        //        if (compare(getEntry(i), parent(getEntry(i))) < 0) {
        //          System.err.println("Error in the ordering of the heap! ("+i+")");
        //          System.exit(0);
        //        }
        //      }
        //      // check placement
        //      if (i != ((Entry)indexToEntry[i)).index)
        //        System.err.println("Error in placement in the heap!");
        //    }
        //  }

        //@Override
        public override string ToString()
        {
            return ToString(0);
        }

        /** {@inheritDoc} */
        //@Override
        public string ToString(int maxKeysToPrint)
        {
            if (maxKeysToPrint <= 0) maxKeysToPrint = int.MaxValue;
            List<E> sortedKeys = ToSortedList();
            var sb = new StringBuilder("[");
            for (int i = 0; i < maxKeysToPrint && i < sortedKeys.Count; i++)
            {
                E key = sortedKeys[i];
                sb.Append(key).Append('=').Append(GetPriority(key));
                if (i < maxKeysToPrint - 1 && i < sortedKeys.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(']');
            return sb.ToString();
        }

        public string ToVerticalString()
        {
            List<E> sortedKeys = ToSortedList();
            var sb = new StringBuilder();
            foreach (var sortedKey in sortedKeys)
            {
                sb.Append(sortedKey);
                sb.Append('\t');
                sb.Append(GetPriority(sortedKey));
                if (sortedKeys.Count > sortedKeys.IndexOf(sortedKey) + 1)
                {
                    sb.Append('\n');
                }
            }
            /*for (Iterator<E> keyI = sortedKeys.iterator(); keyI.hasNext();) {
      E key = keyI.next();
      sb.Append(key);
      sb.Append('\t');
      sb.Append(getPriority(key));
      if (keyI.hasNext()) {
        sb.Append('\n');
      }
    }*/
            return sb.ToString();
        }


        public BinaryHeapPriorityQueue() :
            this(MapFactory<E, Entry<E>>.hashMapFactory<E, Entry<E>>())
        {
        }

        /*public BinaryHeapPriorityQueue(int initCapacity) {
      this(MapFactory<E, Entry<E>>.hashMapFactory<E, Entry<E>>(initCapacity));
  }*/

        public BinaryHeapPriorityQueue(MapFactory<E, Entry<E>> mapFactory)
        {
            indexToEntry = new List<Entry<E>>();
            keyToEntry = mapFactory.newMap();
        }

        public BinaryHeapPriorityQueue(MapFactory<E, Entry<E>> mapFactory, int initCapacity)
        {
            indexToEntry = new List<Entry<E>>(initCapacity);
            keyToEntry = mapFactory.newMap(initCapacity);
        }
    }
}