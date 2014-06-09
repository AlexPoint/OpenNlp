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

//This file is based on the Chunker.java source file found in the
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

namespace OpenNLP.Tools.Chunker
{
	/// <summary>
	/// The interface for chunkers which provide chunk tags for a sequence of tokens.
	/// </summary>
	public interface IChunker
	{
		/// <summary>
		/// Generates chunk tags for the given sequence returning the result in a list.
		/// </summary>
		/// <param name="tokens">
		/// a list of the tokens or words of the sequence.
		/// </param>
		/// <param name="tags">
		/// a list of the pos tags of the sequence.
		/// </param>
		/// <returns>
		/// a list of chunk tags for each token in the sequence.
		/// </returns>
		ArrayList Chunk(ArrayList tokens, ArrayList tags);
			
		/// <summary>
		/// Generates chunk tags for the given sequence returning the result in an array.
		/// </summary>
		/// <param name="tokens">
		/// an array of the tokens or words of the sequence.
		/// </param>
		/// <param name="tags">
		/// an array of the pos tags of the sequence.
		/// </param>
		/// <returns>
		/// an array of chunk tags for each token in the sequence.
		/// </returns>
		string[] Chunk(object[] tokens, string[] tags);
	}
}
