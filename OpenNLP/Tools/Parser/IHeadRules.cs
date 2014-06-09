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

//This file is based on the HeadRules.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2004 Thomas Morton
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

namespace OpenNLP.Tools.Parser
{
	/// <summary>
	/// Interface for encoding the head rules associated with parsing.
	/// </summary>
	public interface IHeadRules
	{
		/// <summary>
		/// Returns the head constituent for the specified constituents of the specified type.
		/// </summary>
		/// <param name="constituents">
		/// The constituents which make up a constituent of the specified type.
		/// </param>
		/// <param name="type">
		/// The type of a constituent which is made up of the specifed constituents.
		/// </param>
		/// <returns>
		/// The constituent which is the head.
		/// </returns>
		Parse GetHead(Parse[] constituents, string type);
	}
}
