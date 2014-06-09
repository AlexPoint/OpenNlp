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

//Copyright (C) 2003 Thomas Morton
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

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// Inteface for interacting with a Heap data structure.
	/// This implementation extract objects from smallest to largest based on either
	/// their natural ordering or the comparator provided to an implementation.
	/// While this is a typical of a heap it allows this objects natural ordering to
	/// match that of other sorted collections.
	/// </summary>
	public interface IHeap<T>
	{
			
		/// <summary>
		/// Removes the smallest element from the heap and returns it.
		/// </summary>
		/// <returns>
		/// The smallest element from the heap.
		/// </returns>
		T Extract();
			
		/// <summary>
		/// Returns the smallest element of the heap.
		/// </summary>
		/// <returns>
		/// The top element of the heap.
		/// </returns>
		T Top
		{
			get;
		}
			
		/// <summary>
		/// Adds the specified object to the heap.
		/// </summary>
		/// <param name="input">
		/// The object to add to the heap.
		/// </param>
		void Add(T input);
			
		/// <summary>
		/// Returns the size of the heap.
		/// </summary>
		/// <returns>
		/// The size of the heap.
		/// </returns>
		int Size
		{
			get;
		}
	
		/// <summary>
		/// Returns whether the heap is empty.
		/// </summary>
		/// <returns> 
		/// true if the heap is empty; false otherwise.
		///</returns>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// Clears the contents of the heap.
		/// </summary>
		void Clear();
	}
}
