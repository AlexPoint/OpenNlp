//Copyright (C) 2005 Richard J. Northedge
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

//This file is based on the Cache.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Jeremy LaCivita and Thomas Morton
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	///  Provides fixed size, pre-allocated, least recently used replacement cache.
	///  </summary>
	public class Cache : IDictionary
	{
		/// <summary>
		/// The element in the linked list which was most recently used.
		/// </summary>
		private DoubleLinkedListElement mFirstElement;

		/// <summary>
		/// The element in the linked list which was least recently used.
		/// </summary>
		private DoubleLinkedListElement mLastElement;

		/// <summary>
		/// Temporary holder of the key of the least-recently-used element.
		/// </summary>
		private object mLastKey;

		/// <summary>
		/// Temporary value used in swap.
		/// </summary>
		private ObjectWrapper mTempSwapWrapper;

		/// <summary>
		/// Holds the object wrappers which the keys are mapped to.
		/// </summary>
		private ObjectWrapper[] mWrappers;

		/// <summary>
		/// Hashtable which stores the keys and values of the cache.
		/// </summary>
		private Hashtable mMap;

		/// <summary>
		/// The size of the cache.
		/// </summary>
		private int mCacheSize;

		public virtual ICollection Keys
		{
			get
			{
				return mMap.Keys;
			}
			
		}
		public virtual int Count
		{
			get
			{
				return mMap.Count;
			}
			
		}
		public virtual ICollection Values
		{
			get
			{
				return mMap.Values;
			}
			
		}
		
		/// <summary>
		/// Creates a new cache of the specified size.
		/// </summary>
		/// <param name="size">
		/// The size of the cache.
		/// </param>
		public Cache(int size)
		{
			mMap = new Hashtable(size);
			mWrappers = new ObjectWrapper[size];
			mCacheSize = size;
			object item = new object();
			mFirstElement = new DoubleLinkedListElement(null, null, item);
			object tempObject;
			tempObject = new ObjectWrapper(null, mFirstElement);
			mMap[item] = tempObject;
			mWrappers[0] = new ObjectWrapper(null, mFirstElement);
			
			DoubleLinkedListElement element = mFirstElement;
			for (int currentItem = 1; currentItem < mCacheSize; currentItem++)
			{
				item = new object();
				element = new DoubleLinkedListElement(element, null, item);
				mWrappers[currentItem] = new ObjectWrapper(null, element);
				object tempObject2;
				tempObject2 = mWrappers[currentItem];
				mMap[item] = tempObject2;
				element.Previous.Next = element;
			}
			mLastElement = element;
		}
		
		public virtual void Clear()
		{
			mMap.Clear();
			DoubleLinkedListElement element = mFirstElement;

			for (int currentItem = 0; currentItem < mCacheSize; currentItem++)
			{
				mWrappers[currentItem].Item = null;
				object o = new object();
				mMap.Add(o, mWrappers[currentItem]);
				element.Item = o;
				element = element.Next;
			}
		}
		
		public object this[object key]
		{
			get
			{
				ObjectWrapper wrapper = (ObjectWrapper) mMap[key];

				if (wrapper != null)
				{
					// Move it to the front
					DoubleLinkedListElement element = (DoubleLinkedListElement) wrapper.ListItem;
				
					//move to front
					if (element != mFirstElement)
					{
						//remove list item
						element.Previous.Next = element.Next;
						if (element.Next != null)
						{
							element.Next.Previous = element.Previous;
						}
						else
						{
							//were moving last
							mLastElement = element.Previous;
						}
						//put list item in front
						element.Next = mFirstElement;
						mFirstElement.Previous = element;
						element.Previous = null;
					
						//update first
						mFirstElement = element;
					}
					return wrapper.Item;
				}
				else
				{
					return null;
				}
			}

			set
			{
				
				ObjectWrapper wrapper = (ObjectWrapper) mMap[key];
				if (wrapper != null)
				{
					/* this should never be the case, we only do a put on a cache miss which 
					means the current value wasn't in the cache.  However if the user 
					screws up or wants to use this as a fixed size hash and puts the same 
					thing in the list twice things break
					*/

					//System.err.println("Cache.put: inserting same object into cache!!!!");
					// Move wrapper's partner in the list to front
					DoubleLinkedListElement element = wrapper.ListItem;
				
					//move to front
					if (element != mFirstElement)
					{
						//remove list item
						element.Previous.Next = element.Next;
						if (element.Next != null)
						{
							element.Next.Previous = element.Previous;
						}
						else
						{
							//were moving last
							mLastElement = element.Previous;
						}
					
						//put list item in front
						element.Next = mFirstElement;
						mFirstElement.Previous = element;
						element.Previous = null;
					
						//update first
						mFirstElement = element;
					}
					return; 
				}
				// Put wrapper in the front and remove the last one
				mLastKey = mLastElement.Item; // store key to remove from hash later
				mLastElement.Item = key; //update list element with new key
			
				// connect list item to front of list
				mLastElement.Next = mFirstElement;
				mFirstElement.Previous = mLastElement;
			
				// update first and last value
				mFirstElement = mLastElement;
				mLastElement = mLastElement.Previous;
				mFirstElement.Previous = null;
				mLastElement.Next = null;
			
				// remove old value from cache
				mTempSwapWrapper = (ObjectWrapper) mMap[mLastKey];
				mMap.Remove(mLastKey);
				
				//update wrapper
				mTempSwapWrapper.Item = value;
				mTempSwapWrapper.ListItem = mFirstElement;
			
				object tempObject;
				tempObject = mTempSwapWrapper;
				mMap[key] = tempObject;
			}
		}
		
		public bool ContainsKey(object key)
		{
			return mMap.ContainsKey(key);
		}
		
		public bool Contains(object dataValue)
		{
			return mMap.Contains(dataValue);
		}
		
		public Set<IDictionaryEnumerator> EntrySet()
		{
			IDictionaryEnumerator hashEnumerator = mMap.GetEnumerator();
            Set<IDictionaryEnumerator> hashSet = new Set<IDictionaryEnumerator>();
			while(hashEnumerator.MoveNext())
			{
				Hashtable tempHash = new Hashtable();
				tempHash.Add(hashEnumerator.Key, hashEnumerator.Value);
				hashSet.Add(tempHash.GetEnumerator());
			}
			return hashSet;
		}
		
		public bool IsEmpty()
		{
			return (mMap.Count == 0);
		}
		
		public void PutAll(IDictionary source)
		{
			object[] keys = new object[source.Keys.Count];
			object[] values = new object[source.Values.Count];

			source.Keys.CopyTo(keys, 0);
			source.Values.CopyTo(values, 0);

			for (int index = 0; index < source.Keys.Count; index++)
			{
				mMap[keys.GetValue(index)] = values.GetValue(index);
			}
		}
		
		public virtual void Remove(object key)
		{
			mMap.Remove(key);
		}

		public bool IsSynchronized
		{
			get
			{
				return mMap.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return mMap.SyncRoot;
			}
		}
        
		public void CopyTo(System.Array copyArray, int arrayIndex)
		{
			mMap.CopyTo(copyArray, arrayIndex);
		}

		public void Add(object key, object dataValue)
		{
			throw new NotSupportedException("cannot add to a fixed size cache");
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return mMap.GetEnumerator();
		}    

		IEnumerator IEnumerable.GetEnumerator()
		{
			return mMap.GetEnumerator();
		}
	}
	
	class ObjectWrapper
	{
		public virtual object Item
		{
			get
			{
				return mItem;
			}
			
			set
			{
				mItem = value;
			}
			
		}
		virtual public DoubleLinkedListElement ListItem
		{
			get
			{
				return mListItem;
			}
			
			set
			{
				mListItem = value;
			}
			
		}
		
		private object mItem;
		private DoubleLinkedListElement mListItem;
		
		public ObjectWrapper(object item, DoubleLinkedListElement listItem)
		{
			mItem = item;
			mListItem = listItem;
		}
		
		public override bool Equals(object compare)
		{
			return mItem.Equals(compare);
		}

		public override int GetHashCode()
		{
			return mItem.GetHashCode();
		}
	}
	
	/// <summary>
	/// An entry in a double-linked list.
	/// </summary>
	public class DoubleLinkedListElement
	{
		private DoubleLinkedListElement mPrevious;
		private DoubleLinkedListElement mNext;
		private object mItem;
		
		public DoubleLinkedListElement Previous
		{
			get
			{
				return mPrevious;
			}
			set
			{
				mPrevious = value;
			}
		}

		public DoubleLinkedListElement Next
		{
			get
			{
				return mNext;
			}
			set
			{
				mNext = value;
			}
		}

		public object Item
		{
			get
			{
				return mItem;
			}
			set
			{
				mItem = value;
			}
		}

		public DoubleLinkedListElement(DoubleLinkedListElement previousElement, DoubleLinkedListElement nextElement, object item)
		{
			mPrevious = previousElement;
			mNext = nextElement;
			mItem = item;
			
			if (previousElement != null)
			{
				previousElement.Next = this;
			}
			
			if (nextElement != null)
			{
				nextElement.Previous = this;
			}
		}
	}
	
	/// <summary>
	/// A double-linked list implementation.
	/// </summary>
	public class DoubleLinkedList
	{
		virtual public DoubleLinkedListElement GetFirst()
		{
			Current = First;
			return First;
		}

		virtual public DoubleLinkedListElement GetLast()
		{
			Current = Last;
			return Last;
		}
		virtual public DoubleLinkedListElement GetCurrent()
		{
			return Current;
		}
		
		internal DoubleLinkedListElement First;
		internal DoubleLinkedListElement Last;
		internal DoubleLinkedListElement Current;
		
		public DoubleLinkedList()
		{
			First = null;
			Last = null;
			Current = null;
		}
		
		public virtual void AddFirst(object item)
		{
			First = new DoubleLinkedListElement(null, First, item);
			
			if (Current.Next == null)
			{
				Last = Current;
			}
		}
		
		public virtual void AddLast(object item)
		{
			Last = new DoubleLinkedListElement(Last, null, item);
			
			if (Current.Previous == null)
			{
				First = Current;
			}
		}
		
		public virtual void Insert(object item)
		{
			if (Current == null)
			{
				Current = new DoubleLinkedListElement(null, null, item);
			}
			else
			{
				Current = new DoubleLinkedListElement(Current.Previous, Current, item);
			}
			
			if (Current.Previous == null)
			{
				First = Current;
			}
			
			if (Current.Next == null)
			{
				Last = Current;
			}
		}
		
		public virtual DoubleLinkedListElement Next()
		{
			if (Current.Next != null)
			{
				Current = Current.Next;
			}
			return Current;
		}
		
		public virtual DoubleLinkedListElement Previous()
		{
			if (Current.Previous != null)
			{
				Current = Current.Previous;
			}
			return Current;
		}
		
		public override string ToString()
		{
			DoubleLinkedListElement element = First;
			StringBuilder buffer = new StringBuilder();
			buffer.Append("[").Append(element.Item.ToString());
			
			element = element.Next;
			
			while (element != null)
			{
				buffer.Append(", ").Append(element.Item.ToString());
				element = element.Next;
			}
			
			buffer.Append("]");
			
			return buffer.ToString();
		}
	}

    public class LinkedListNodeWrapper<K, V>
    {
        V mItem;
        LinkedListNode<K> mNode;

        public LinkedListNodeWrapper(K key, V value)
        {
            mItem = value;
            mNode = new LinkedListNode<K>(key);
        }

        public V Item
        {
            get
            {
                return mItem;
            }
            set
            {
                mItem = value;
            }
        }

        public LinkedListNode<K> Node
        {
            get
            {
                return mNode;
            }
        }
    }

    public class Cache<K, V> : IDictionary<K, V>
    {
        /// <summary>
        /// Hashtable which stores the keys of the cache.
        /// </summary>
        private Dictionary<K, LinkedListNodeWrapper<K, V>> mMap;

        /// <summary>
        /// Double-linked list which stores the values of the cache.
        /// </summary>
        private LinkedList<K> mList;

        /// <summary>
        /// The size of the cache.
        /// </summary>
        private int mCacheSize;

        /// <summary>
        /// Creates a new cache of the specified size.
        /// </summary>
        /// <param name="size">
        /// The size of the cache.
        /// </param>
        public Cache(int size)
        {
            mCacheSize = size;
            mMap = new Dictionary<K, LinkedListNodeWrapper<K, V>>(size);
            mList = new LinkedList<K>();
        }

        #region IDictionary<K,V> Members

        public void Add(K key, V value)
        {
            this[key] = value;
        }

        public bool ContainsKey(K key)
        {
            return mMap.ContainsKey(key);
        }

        public ICollection<K> Keys
        {
            get
            {
                return mMap.Keys;
            }
        }

        public bool Remove(K key)
        {
            if (mMap.ContainsKey(key))
            {
                mList.Remove(mMap[key].Node);
                return mMap.Remove(key);
            }
            return false;
        }

        public bool TryGetValue(K key, out V value)
        {
            value = this[key];
            return mMap.ContainsKey(key);
        }

        public ICollection<V> Values
        {
            get { throw new NotSupportedException("The method or operation is not supported."); }
        }

        public V this[K key]
        {
            get
            {
                if (mMap.ContainsKey(key))
                {
                    LinkedListNodeWrapper<K, V> wrapper = mMap[key];
                    LinkedListNode<K> element = wrapper.Node;
                    if (element.Previous != null)
                    {
                        //move to front
                        mList.Remove(element);
                        mList.AddFirst(element);
                    }
                    return wrapper.Item;
                }
                else
                {
                    return default(V);
                }
            }
            set
            {
                if (mMap.ContainsKey(key))
                {
                    LinkedListNodeWrapper<K, V> wrapper = mMap[key];
                    wrapper.Item = value;
                    LinkedListNode<K> element = wrapper.Node;
                    if (element.Previous != null)
                    {
                        //move to front
                        mList.Remove(element);
                        mList.AddFirst(element);
                    }
                }
                else
                {
                    if (mMap.Count == mCacheSize)
                    {
                        //remove last item
                        mMap.Remove(mList.Last.Value);
                        mList.RemoveLast();
                    }
                    LinkedListNodeWrapper<K, V> wrapper = new LinkedListNodeWrapper<K, V>(key, value);
                    mMap.Add(key, wrapper);
                    mList.AddFirst(wrapper.Node);
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<K,V>> Members

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            mMap.Clear();
            mList.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return mMap.ContainsKey(item.Key) && mMap[item.Key].Item.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public int Count
        {
            get
            {
                return mMap.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<K,V>> Members

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

