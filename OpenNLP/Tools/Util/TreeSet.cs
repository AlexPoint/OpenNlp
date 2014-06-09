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

using System;
using System.Collections.Generic;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// This class manages a tree set collection of sorted elements.
	/// </summary>
	public class TreeSet<T> : SortedSet<T>
	{
		/// <summary>
		/// Creates a new TreeSet.
		/// </summary>
		public TreeSet()
		{
		}
			 	
		/// <summary>
		/// Create a new TreeSet with a specific collection.
		/// </summary>
		/// <param name="collection">
		/// The collection used to initialize the TreeSet
		/// </param>
		public TreeSet(ICollection<T> collection): base(collection)
		{
		}
	 
		/// <summary>
		/// Creates a copy of the TreeSet.
		/// </summary>
		/// <returns>A copy of the TreeSet.</returns>
		public virtual object TreeSetClone()
		{
			TreeSet<T> internalClone = new TreeSet<T>();
			internalClone.AddAll(this);
			return internalClone;
		}

		/// <summary>
		/// Retrieves the number of elements contained in the set.
		/// </summary>
		/// <returns>
		/// An integer value that represent the number of element in the set.
		/// </returns>
		public virtual int Size()
		{
			return this.Count;
		}
	}
}
