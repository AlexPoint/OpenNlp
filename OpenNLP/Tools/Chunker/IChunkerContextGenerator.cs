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

//This file is based on the ChunkerContextGenerator.java source file found in the
//original java implementation of OpenNLP.

using System;
using System.Collections;

namespace OpenNLP.Tools.Chunker
{
	/// <summary>
	/// Context generator interface for chunkers.
	/// </summary>
	public interface IChunkerContextGenerator : Util.IBeamSearchContextGenerator
	{
		/// <summary>
		/// Returns the contexts for chunking of the specified index.
		/// </summary>
		/// <param name="tokenIndex">
		/// The index of the token in the specified toks array for which the context should be constructed. 
		/// </param>
		/// <param name="tokens">
		/// The tokens of the sentence.  The <code>toString</code> methods of these objects should return the token text.
		/// </param>
		/// <param name="tags">
		/// The POS tags for the the specified tokens.
		/// </param>
		/// /// <param name="previousDecisions">
		/// The previous decisions made in the tagging of this sequence.  Only indices less than tokenIndex will be examined.
		/// </param>
		/// <returns>
		/// An array of predictive contexts on which a model basis its decisions.
		/// </returns>
		string[] GetContext(int tokenIndex, object[] tokens, string[] tags, string[] previousDecisions);
	}
}
