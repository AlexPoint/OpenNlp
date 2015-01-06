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
    /// This class manages a set of elements.
    /// </summary>
    public class Set<T> : List<T>
    {
        /// <summary>
        /// Creates a new set.
        /// </summary>
        public Set() : base()
        {
        }

        /// <summary>
        /// Creates a new set initialized with ICollection object
        /// </summary>
        /// <param name="collection">
        /// ICollection object to initialize the set object
        /// </param>
        public Set(ICollection<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Creates a new set initialized with a specific capacity.
        /// </summary>
        /// <param name="capacity">
        /// value to set the capacity of the set object
        /// </param>
        public Set(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Adds an element to the set.
        /// </summary>
        /// <param name="item">
        /// The object to be added.
        /// </param>
        /// <returns>
        /// True if the object was added, false otherwise.
        /// </returns>
        public new virtual bool Add(T item)
        {
            if (this.Contains(item))
            {
                return false;
            }
            else
            {
                base.Add(item);
                return true;
            }
        }

        /// <summary>
        /// Adds all the elements contained in the specified collection.
        /// </summary>
        /// <param name="collection">
        /// The collection used to extract the elements that will be added.
        /// </param>
        /// <returns>
        /// Returns true if all the elements were successfuly added. Otherwise returns false.
        /// </returns>
        public virtual bool AddAll(ICollection<T> collection)
        {
            bool result = false;
            if (collection != null)
            {
                foreach (T item in collection)
                {
                    result = this.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Verifies that all the elements of the specified collection are contained into the current collection. 
        /// </summary>
        /// <param name="collection">
        /// The collection used to extract the elements that will be verified.
        /// </param>
        /// <returns>
        /// True if the collection contains all the given elements.
        /// </returns>
        public virtual bool ContainsAll(ICollection<T> collection)
        {
            bool result = false;
            foreach (T item in collection)
            {
                if (!(result = this.Contains(item)))
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Verifies if the collection is empty.
        /// </summary>
        /// <returns>
        /// True if the collection is empty, false otherwise.
        /// </returns>
        public virtual bool IsEmpty()
        {
            return (this.Count == 0);
        }

        /// <summary>
        /// Removes an element from the set.
        /// </summary>
        /// <param name="elementToRemove">
        /// The element to be removed.
        /// </param>
        /// <returns>
        /// True if the element was removed.
        /// </returns>
        public new virtual bool Remove(T elementToRemove)
        {
            bool result = this.Contains(elementToRemove);
            base.Remove(elementToRemove);
            return result;
        }

        /// <summary>
        /// Removes all the elements contained in the specified collection.
        /// </summary>
        /// <param name="collection">
        /// The collection used to extract the elements that will be removed.
        /// </param>
        /// <returns>
        /// True if all the elements were successfuly removed, false otherwise.
        /// </returns>
        public virtual bool RemoveAll(ICollection<T> collection)
        {
            bool result = false;
            foreach (T item in collection)
            {
                if ((!result) && (this.Contains(item)))
                {
                    result = true;
                }
                this.Remove(item);
            }
            return result;
        }

        /// <summary>
        /// Removes all the elements that aren't contained in the specified collection.
        /// </summary>
        /// <param name="collection">
        /// The collection used to verify the elements that will be retained.
        /// </param>
        /// <returns>
        /// True if all the elements were successfully removed, false otherwise.
        /// </returns>
        public virtual bool RetainAll(ICollection<T> collection)
        {
            bool result = false;

            IEnumerator<T> enumerator = collection.GetEnumerator();
            var currentSet = (Set<T>) collection;
            while (enumerator.MoveNext())
                if (!currentSet.Contains(enumerator.Current))
                {
                    result = this.Remove(enumerator.Current);
                    enumerator = this.GetEnumerator();
                }
            return result;
        }

        /// <summary>
        /// Obtains an array containing all the elements in the collection.
        /// </summary>
        /// <param name="objects">
        /// The array into which the elements of the collection will be stored.
        /// </param>
        /// <returns>
        /// The array containing all the elements of the collection.
        /// </returns>
        public virtual T[] ToArray(T[] objects)
        {
            int index = 0;
            foreach (T item in this)
            {
                objects[index++] = item;
            }
            return objects;
        }
    }
}