//Copyright (C) 2006 Richard J. Northedge
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

//This file is based on the CountedSet.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Thomas Morton
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OpenNLP.Tools.Util
{
	
	/// <summary>
    /// Set which counts the number of times a values are added to it.  
	/// This value can be accessed with the GetCount method.
	/// </summary>
	public class CountedSet<T> : ICollection<T>
    {
        private IDictionary<T, int> mCountedSet;

        /// <summary> Creates a new counted set.</summary>
        public CountedSet()
        {
            mCountedSet = new Dictionary<T, int>();
        }

        /// <summary>
        /// Creates a new counted set of the specified initial size.
        /// </summary>
        /// <param name="size">
        /// The initial size of this set.
        /// </param>
        public CountedSet(int size)
        {
            mCountedSet = new Dictionary<T, int>(size);
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            if (mCountedSet.ContainsKey(item))
            {
                mCountedSet[item]++;
            }
            else
            {
                mCountedSet.Add(item, 1);
            }

        }

        public void Clear()
        {
            mCountedSet.Clear();
        }

        public bool Contains(T item)
        {
            return mCountedSet.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException("The method or operation is not implemented.");
        }

        public int Count
        {
            get
            {
                return mCountedSet.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            if (mCountedSet.ContainsKey(item))
            {
                mCountedSet.Remove(item);
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Reduces the count associated with this object by 1.  If this causes the count
		/// to become 0, then the object is removed form the set.
		/// </summary>
		/// <param name="item">The item whose count is being reduced.
		/// </param>
        public virtual void Subtract(T item)
		{
            if (mCountedSet.ContainsKey(item))
            {
                if (mCountedSet[item] > 1)
                {
                    mCountedSet[item]--;
                }
                else
                {
                   mCountedSet.Remove(item);
                }
            }

		}
		
		/// <summary>
        /// Assigns the specified object the specified count in the set.
        /// </summary>
		/// <param name="item">
        /// The item to be added or updated in the set.
		/// </param>
		/// <param name="count">
        /// The count of the specified item.
		/// </param>
		public virtual void SetCount(T item, int count)
		{
            if (mCountedSet.ContainsKey(item))
            {
                mCountedSet[item] = count;
            }
            else
            {
                mCountedSet.Add(item, count);
            }
		}
		
		/// <summary> Return the count of the specified object.</summary>
		/// <param name="item">the object whose count needs to be determined.
		/// </param>
		/// <returns> the count of the specified object.
		/// </returns>
		public virtual int GetCount(T item)
		{
            if (!mCountedSet.ContainsKey(item))
			{
				return 0;
			}
			else
			{
                return mCountedSet[item];
			}
        }

        #region Write methods

        public virtual void Write(string fileName, int countCutoff)
		{
			Write(fileName, countCutoff, " ");
		}
		
		public virtual void Write(string fileName, int countCutoff, string delim)
		{
            using (StreamWriter streamWriter = new StreamWriter(fileName, false))
            {
                foreach (T key in mCountedSet.Keys)
                {
                    int count = this.GetCount(key);
                    if (count >= countCutoff)
                    {
                        streamWriter.WriteLine(count + delim + key);
                    }
                }
            }
		}


        public virtual void Write(string fileName, int countCutoff, string delim, System.Text.Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(fileName, false, encoding))
			{				
                foreach(T key in mCountedSet.Keys)
                {
                    int count = this.GetCount(key);
                    if (count >= countCutoff)
					{
						streamWriter.WriteLine(count + delim + key);
					}
                }
			}

        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new List<T>(mCountedSet.Keys).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ArrayList((ICollection)mCountedSet.Keys).GetEnumerator();
        }

        #endregion

        //public virtual System.Boolean AddAll(System.Collections.ICollection c)
        //{
        //    bool changed = false;
        //    //UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
        //    for (System.Collections.IEnumerator ci = c.GetEnumerator(); ci.MoveNext(); )
        //    {
        //        //UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
        //        changed = changed || Add(ci.Current);
        //    }
        //    return changed;
        //}

        ///// <summary>
        ///// Adds all the elements contained in the specified collection.
        ///// </summary>
        ///// <param name="collection">
        ///// The collection used to extract the elements that will be added.
        ///// </param>
        ///// <returns>
        ///// Returns true if all the elements were successfuly added. Otherwise returns false.
        ///// </returns>
        //public virtual bool AddAll(ICollection<T> collection)
        //{
        //    bool result = false;
        //    if (collection != null)
        //    {
        //        foreach (T item in collection)
        //        {
        //            result = this.Add(item);
        //        }
        //    }
        //    return result;
        //}
		
		
		
        ////UPGRADE_NOTE: The equivalent of method 'java.util.Set.containsAll' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //public virtual bool containsAll(System.Collections.ICollection c)
        //{
        //    //UPGRADE_TODO: Method 'java.util.Map.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapkeySet'"
        //    return SupportClass.ICollectionSupport.ContainsAll(new HashSet<T>(mCountedSet.Keys), c);
        //}
		
        ////UPGRADE_NOTE: The equivalent of method 'java.util.Set.isEmpty' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //public virtual bool isEmpty()
        //{
        //    return (mCountedSet.Count == 0);
        //}
		
        ////UPGRADE_ISSUE: The equivalent in .NET for method 'java.util.Set.iterator' returns a different type. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1224'"
        ////GetEnumerator is not virtual in the List<T>, which is the eventual base class
        ////public override List<T>.Enumerator GetEnumerator()
        ////{
        //    //UPGRADE_TODO: Method 'java.util.Map.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapkeySet'"
        ////    return new HashSet<T>(cset.Keys).GetEnumerator();
        ////}
		
        ////UPGRADE_ISSUE: The equivalent in .NET for method 'java.util.Set.remove' returns a different type. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1224'"
        //public override bool Remove(T o)
        //{
        //    if (mCountedSet.ContainsKey(o))
        //    {
        //        mCountedSet.Remove(o);
        //        return true;
        //    }
        //    return false;
        //}
		
        ////UPGRADE_NOTE: The equivalent of method 'java.util.Set.removeAll' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //public virtual bool removeAll(ICollection<T> c)
        //{
        //    bool changed = false;
        //    //UPGRADE_TODO: Method 'java.util.Map.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapkeySet'"
        //    //UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
        //    for (System.Collections.IEnumerator ki = new HashSet<T>(mCountedSet.Keys).GetEnumerator(); ki.MoveNext(); )
        //    {
        //        System.Object tempObject;
        //        //UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
        //        tempObject = mCountedSet[ki.Current];
        //        mCountedSet.Remove(ki.Current);
        //        changed = changed || (tempObject != null);
        //    }
        //    return changed;
        //}
		
        ////UPGRADE_NOTE: The equivalent of method 'java.util.Set.retainAll' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //public virtual bool retainAll(System.Collections.ICollection c)
        //{
        //    bool changed = false;
        //    //UPGRADE_TODO: Method 'java.util.Map.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapkeySet'"
        //    //UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
        //    for (System.Collections.IEnumerator ki = new SupportClass.HashSetSupport(mCountedSet.Keys).GetEnumerator(); ki.MoveNext(); )
        //    {
        //        //UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
        //        System.Object key = ki.Current;
        //        if (!SupportClass.ICollectionSupport.Contains(c, key))
        //        {
        //            mCountedSet.Remove(key);
        //            changed = true;
        //        }
        //    }
        //    return changed;
        //}
		
        ////UPGRADE_NOTE: The equivalent of method 'java.util.Set.toArray' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //public virtual System.Object[] toArray()
        //{
        //    //UPGRADE_TODO: Method 'java.util.Map.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapkeySet'"
        //    return SupportClass.ICollectionSupport.ToArray(new SupportClass.HashSetSupport(mCountedSet.Keys));
        //}
		
        ////UPGRADE_NOTE: The equivalent of method 'java.util.Set.toArray' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //public virtual System.Object[] toArray(System.Object[] arg0)
        //{
        //    //UPGRADE_TODO: Method 'java.util.Map.keySet' was converted to 'SupportClass.HashSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilMapkeySet'"
        //    return SupportClass.ICollectionSupport.ToArray(new SupportClass.HashSetSupport(mCountedSet.Keys), arg0);
        //}
	}
}