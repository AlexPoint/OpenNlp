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
		
		private readonly SortedSet<T> _tree;
		
		/// <summary> Creates a new tree heap.</summary>
		public TreeHeap()
		{
			_tree = new TreeSet<T>();
		}
		
		/// <summary>
		/// Creates a new tree heap of the specified size.
		/// </summary>
		/// <param name="size">
		/// The size of the new tree heap.
		/// </param>
		public TreeHeap(int size)
		{
			_tree = new TreeSet<T>();
		}
		
        /// <summary>
        /// Pops the first element of the tree
        /// </summary>
		public virtual T Extract()
		{
			T extracted = _tree.First();
			_tree.Remove(extracted);
			return extracted;
		}
		
        /// <summary>
        /// Returns the first element of the tree
        /// </summary>
		public virtual T Top
		{
			get
			{
				return _tree.First();
			}
		}
		
        /// <summary>
        /// Adds an element to the tree
        /// </summary>
		public virtual void Add(T input)
		{
			_tree.Add(input);
		}
		
        /// <summary>
        /// Sorts the tree
        /// </summary>
		public void Sort()
		{
			_tree.Sort();
		}

        /// <summary>
        /// Returns the size of the tree
        /// </summary>
		public virtual int Size
		{
			get
			{
				return _tree.Count;
			}
		}
		
        /// <summary>
        /// Clears the tree
        /// </summary>
		public virtual void Clear()
		{
			_tree.Clear();
		}

        /// <summary>
        /// Returns if the tree is empty
        /// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				return _tree.IsEmpty();
			}
		}
	}
}
