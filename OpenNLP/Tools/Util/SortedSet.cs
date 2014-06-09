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
	/// This class contains methods to manage a sorted collection.
	/// RN note: this class (taken from the JLCA "support" code) doesn't actually
	/// sort anything.
	/// </summary>
	public class SortedSet<T> : Set<T>
	{
		/// <summary>
		/// Creates a new SortedSet.
		/// </summary>
		public SortedSet() : base()
		{
		}
				
		/// <summary>
		/// Create a new SortedSet with a specific collection.
		/// </summary>
		/// <param name="collection">
		/// The collection used to iniciatilize the SortedSetSupport
		/// </param>
		public SortedSet(ICollection<T> collection): base(collection)
		{
		}
	 
		/// <summary>
		/// Returns the first element from the set.
		/// </summary>
		/// <returns>
		/// Returns the first element from the set.
		/// </returns>
		public virtual T First()
		{
			IEnumerator<T> enumerator = this.GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}
	 
		/// <summary>
		/// Returns a view of elements until the specified element.
		/// </summary>
		/// <returns>
		/// Returns a sorted set of elements that are strictly less than the specified element.
		/// </returns>
        //public virtual SortedSet<T> HeadSet(T toElement)
        //{
        //    SortedSet<T> sortedSet = new SortedSet<T>();
        //    IEnumerator<T> enumerator = this.GetEnumerator();
        //    while((enumerator.MoveNext() && ((enumerator.Current.ToString().CompareTo(toElement.ToString())) < 0)))
        //    {
        //        sortedSet.Add(enumerator.Current);
        //    }
        //    return sortedSet;
        //}
	 
		/// <summary>
		/// Returns the last element of the set.
		/// </summary>
		/// <returns>Returns the last element from the set.</returns>
		public virtual T Last()
		{
			IEnumerator<T> enumerator = this.GetEnumerator();
			T element = default(T);
			while(enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					element = enumerator.Current;
				}
			}
			return element;
		}
	 
		/// <summary>
		/// Returns a view of elements from the specified element.
		/// </summary>
		/// <returns>
		/// Returns a sorted set of elements that are greater or equal to the specified element.
		/// </returns>
        //public virtual SortedSet<T> TailSet(T fromElement)
        //{
        //    SortedSet<T> sortedSet = new SortedSet<T>();
        //    IEnumerator<T> enumerator = this.GetEnumerator();
        //    while((enumerator.MoveNext() && (!(enumerator.Current.ToString().CompareTo(fromElement.ToString())) < 0)))
        //    {
        //        sortedSet.Add(enumerator.Current);
        //    }
        //    return sortedSet;
        //}
	 
		/// <summary>
		/// Returns a view of elements between the specified elements.
		/// </summary>
		/// <returns>
		/// Returns a sorted set of elements from the first specified element to the second specified element.
		/// </returns>
        //public virtual SortedSet<T> SubSet(T fromElement, T toElement)
        //{
        //    SortedSet<T> sortedSet = new SortedSet<T>();
        //    IEnumerator<T> enumerator = this.GetEnumerator();
        //    while((enumerator.MoveNext() && ((!(enumerator.Current.ToString().CompareTo(fromElement.ToString())) < 0))) && (!(enumerator.Current.ToString().CompareTo(toElement.ToString())) > 0))
        //    {
        //        sortedSet.Add(enumerator.Current);
        //    }
        //    return sortedSet;
        //}
	}
}
