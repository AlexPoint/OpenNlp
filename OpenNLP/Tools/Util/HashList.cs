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

//This file is based on the HashList.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Jeremy LaCivita
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
using System.Collections.Generic;

namespace OpenNLP.Tools.Util
{
	/// <summary>
    /// Class which creates mapping between keys and a list of values.
    /// </summary>
    [Serializable]
    public class HashList<TKey, TValue> : Dictionary<TKey, List<TValue>>
	{
		public virtual TValue GetValue(TKey key, int index)
		{
            if (!this.ContainsKey(key))
            {
                return default(TValue);
            }
            else
            {
                return this[key][index];
            }
		}
		
		public virtual List<TValue> PutAll(TKey key, List<TValue> values)
		{
			List<TValue> list;

            if (!this.ContainsKey(key))
            {
                list = new List<TValue>();
                base[key] = list;
            }
            else
            {
                list = this[key];
            }

			list.AddRange(values);

            if (list.Count == values.Count)
            {
                return null;
            }
            else
            {
                return list;
            }
		}
		
		public List<TValue> Put(TKey key, TValue value)
		{
			List<TValue> list;

            if (!this.ContainsKey(key))
            {
                list = new List<TValue>();
                base[key] = list;
            }
            else
            {
                list = this[key];
            }

			list.Add(value);

            if (list.Count == 1)
            {
                return null;
            }
            else
            {
                return list;
            }
		}
		
		public virtual bool Remove(TKey key, TValue value)
		{
			if (!this.ContainsKey(key))
			{
				return false;
			}
			else
			{
                List<TValue> list = this[key];
                bool removed = list.Remove(value);
                if (list.Count == 0)
				{
                    this.Remove(key);
				}
                return removed;
			}
		}
	}
}