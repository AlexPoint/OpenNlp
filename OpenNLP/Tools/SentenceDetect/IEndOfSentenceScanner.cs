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

//This file is based on the EndOfSentenceScanner.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (c) 2001, Eric D. Friedman All Rights Reserved.
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
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OpenNLP.Tools.SentenceDetect
{
	/// <summary>
	/// Scans strings, StringBuilders, and char[] arrays for the offsets of
	/// sentence ending characters.
	/// 
	/// <p>Implementations of this interface can use regular expressions,
	/// hand-coded DFAs, and other scanning techniques to locate end of
	/// sentence offsets.</p>
	/// </summary>
	public interface IEndOfSentenceScanner
	{
		/// <summary>
		/// The receiver scans 'input' for sentence ending characters and
		/// returns their offsets.
		/// </summary>
		/// <param name="input">
		/// a <code>string</code> value
		/// </param>
		/// <returns>
		/// a <code>List</code> of integers.
		/// </returns>
        List<int> GetPositions(string input);

		/// <summary>
		/// The receiver scans 'buffer' for sentence ending characters and
		/// returns their offsets.
		/// </summary>
		/// <param name="buffer">
		/// a <code>StringBuilder</code> value
		/// </param>
		/// <returns>
		/// a <code>List</code> of integers.
		/// </returns>
        List<int> GetPositions(StringBuilder buffer);
			
		/// <summary>
		/// The receiver scans 'characterBuffer' for sentence ending characters and
		/// returns their offsets.
		/// </summary>
		/// <param name="characterBuffer">
		/// a <code>char[]</code> value
		/// </param>
		/// <returns>
		/// a <code>List</code> of integers.
		/// </returns>
        List<int> GetPositions(char[] characterBuffer);

        /// <summary>
        /// Gets the characters for which we are testing a potential 
        /// end of sentence for this scanner.
        /// </summary>
	    List<char> GetPotentialEndOfSentenceCharacters();
	} 
}
