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

//This file is based on the Pair.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2002 Jason Baldridge and Gann Bierner
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
	///  Dinky class to package pairs of things
	/// </summary>
	public sealed class Pair<A, B>
	{
		private A mFirst;
		private B mSecond;
		
		/// <summary>
		/// Value of the first object in the Pair.
		/// </summary>
		public A FirstValue
		{
			get
			{
				return mFirst;
			}
		}

		/// <summary>
		/// Value of the second object in the Pair.
		/// </summary>
		public B SecondValue
		{
			get
			{
				return mSecond;
			}
		}

		/// <summary>
		/// Constructor for the Pair object.
		/// </summary>
		/// <param name="first">
		/// First object to add to the Pair.
		/// </param>
		/// <param name="second">
		/// Second object to add to the Pair.
		/// </param>
		public Pair(A first, B second)
		{
			mFirst = first;
			mSecond = second;
		}
		
		/// <summary>
		/// Lists the values of the Pair object.
		/// </summary>
		/// <returns>
		/// String value.
		/// </returns>
		public override string ToString()
		{
			return "[" + mFirst.ToString() + "/" + mSecond.ToString() + "]";
		}
	}
}
