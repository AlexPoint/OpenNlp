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

//This file is based on the TreeHeap.java source file found in the
//original java implementation of OpenNLP. 

using System;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// An implemention of the heap interface based on SortedSet.
	/// This implementation will not allow multiple objects which are equal to be added to the heap.
	/// Only use this implementation when object in the heap can be totally ordered (no duplicates). 
	/// </summary>
	public class TreeHeap<T> : IHeap<T>
	{
		
		private SortedSet<T> mTree;
		
		/// <summary> Creates a new tree heap.</summary>
		public TreeHeap()
		{
			mTree = new TreeSet<T>();
		}
		
		/// <summary>
		/// Creates a new tree heap of the specified size.
		/// </summary>
		/// <param name="size">
		/// The size of the new tree heap.
		/// </param>
		public TreeHeap(int size)
		{
			mTree = new TreeSet<T>();
		}
		
		public virtual T Extract()
		{
			T extracted = mTree.First();
			mTree.Remove(extracted);
			return extracted;
		}
		
		public virtual T Top
		{
			get
			{
				return mTree.First();
			}
		}
		
		public virtual void Add(T input)
		{
			mTree.Add(input);
		}
		
		public void Sort()
		{
			mTree.Sort();
		}

		public virtual int Size
		{
			get
			{
				return mTree.Count;
			}
		}
		
		public virtual void Clear()
		{
			mTree.Clear();
		}

		public virtual bool IsEmpty
		{
			get
			{
				return mTree.IsEmpty();
			}
		}
	}
}
