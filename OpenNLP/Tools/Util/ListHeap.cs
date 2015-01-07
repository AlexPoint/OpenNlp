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

//This file is based on the Heap.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2005 Thomas Morton
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
using System.Collections.Generic;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// This class implements the heap interface using a generic List as the underlying
	/// data structure.  This heap allows values which are equals to be inserted, however
	/// the order in which they are extracted is arbitrary.
	/// </summary>
	public class ListHeap<T> : IHeap<T>, IEnumerable<T>
	{
		private List<T> mList;
		private readonly IComparer<T> mComparer;
		private readonly int mSize;
		private T mMax;
		
		/// <summary>
		/// True if the heap is empty.
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				return (mList.Count == 0);
			}
		}
	
		/// <summary>
		/// Creates a new heap with the specified size using the sorted based on the
		/// specified comparator.
		/// </summary>
		/// <param name="size">
		/// The size of the heap.
		/// </param>
		/// <param name="comparer">
		/// The comparer to be used to sort heap elements.
		/// </param>
		public ListHeap(int size, IComparer<T> comparer)
		{
			mSize = size;
			mComparer = comparer;
			mList = new List<T>(size);
		}
		
		/// <summary>
		/// Createa a new heap of the specified size.
		/// </summary>
		/// <param name="size">
		/// The size of the new heap.
		/// </param>
		public ListHeap(int size) : this(size, null)
		{
		}
		
		private int ParentIndex(int index)
		{
			return (index - 1) / 2;
		}
		
		private int LeftIndex(int index)
		{
			return (index + 1) * 2 - 1;
		}
		
		private int RightIndex(int index)
		{
			return (index + 1) * 2;
		}
		
		/// <summary>
		/// The size of the heap.
		/// </summary>
		public virtual int Size
		{
			get
			{
				return mList.Count;
			}
			set
			{
				if (value > mList.Count)
				{
					return ;
				}
				else
				{
                    var newList = new List<T>(value);
					for (int currentItem = 0; currentItem < value; currentItem++)
					{
						newList.Add(this.Extract());
					}
					mList = newList;
				}
			}
		}
		
		private void Swap(int firstIndex, int secondIndex)
		{
			T firstObject = mList[firstIndex];
			T secondObject = mList[secondIndex];
			
			mList[secondIndex] = firstObject;
			mList[firstIndex] = secondObject;
		}
		
		private bool LessThan(T firstObject, T secondObject)
		{
			if (mComparer != null)
			{
				return (mComparer.Compare(firstObject, secondObject) < 0);
			}
			else
			{
				return (((IComparable) firstObject).CompareTo(secondObject) < 0);
			}
		}
		
		private bool GreaterThan(T firstObject, T secondObject)
		{
			if (mComparer != null)
			{
				return (mComparer.Compare(firstObject, secondObject) > 0);
			}
			else
			{
				return (((IComparable) firstObject).CompareTo(secondObject) > 0);
			}
		}
		
		private void Heapify(int index)
		{
			while (true)
			{
				int left = LeftIndex(index);
				int right = RightIndex(index);
				int smallest;
				
				if (left < mList.Count && LessThan(mList[left], mList[index]))
				{
					smallest = left;
				}
				else
				{
					smallest = index;
				}
				
				if (right < mList.Count && LessThan(mList[right], mList[smallest]))
				{
					smallest = right;
				}
				
				if (smallest != index)
				{
					Swap(smallest, index);
					index = smallest;
				}
				else
				{
					break;
				}
			}
		}
		
		public virtual T Extract()
		{
			if (mList.Count == 0)
			{
				throw new NotSupportedException("Heap Underflow");
			}
			T mMax = mList[0];
			int last = mList.Count - 1;
			if (last != 0)
			{
				mList[0] = mList[last];
				mList.RemoveAt(last);
				Heapify(0);
			}
			else
			{
				mList.RemoveAt(last);
			}
			
			return mMax;
		}
		
		/// <summary>
		/// Resets the heap size to its original value.
		/// </summary>
		public virtual void ResetSize()
		{
			this.Size = mSize;
		}
				
		/// <summary>
		/// Gets the object on top of the heap.
		/// </summary>
		public virtual T Top
		{
			get
			{
				if (mList.Count == 0)
				{
					throw new NotSupportedException("Heap Underflow");
				}
				return (mList[0]);
			}
		}
		
		public virtual void Add(T item)
		{
			/* keep track of min to prevent unnecessary insertion */
			if (mMax == null)
			{
				mMax = item;
			}
			else if (GreaterThan(item, mMax))
			{
				if (mList.Count < mSize)
				{
					mMax = item;
				}
				else
				{
					return;
				}
			}
			mList.Add(item);
			
			int index = mList.Count - 1;
			
			//percolate new node to correct position in heap.
			while (index > 0 && GreaterThan(mList[ParentIndex(index)], item))
			{
				mList[index] = mList[ParentIndex(index)];
				index = ParentIndex(index);
			}
			
			mList[index] = item;
		}
		
		public virtual void Clear()
		{
			mList.Clear();
		}
		
		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return (mList.GetEnumerator());
		}

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (mList.GetEnumerator());
        }

    }
}