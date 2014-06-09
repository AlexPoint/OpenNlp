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

//This file is based on the ParserChunker.java source file found in the
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

namespace OpenNLP.Tools.Parser
{
	/// <summary>
	/// Interface that a chunker used with the parser should implement.
	/// </summary>
	public interface IParserChunker : Chunker.IChunker
	{
		/// <summary>
		/// Returns the top k chunk sequences for the specified sentence with the specified pos-tags
		/// </summary>
		/// <param name="sentence">
		/// The tokens of the sentence.
		/// </param>
		/// <param name="tags">
		/// The pos-tags for the specified sentence.
		/// </param>
		/// <returns>
		/// the top k chunk sequences for the specified sentence.
		/// </returns>
		Util.Sequence[] TopKSequences(ArrayList sentence, ArrayList tags);
			
		/// <summary>
		/// Returns the top k chunk sequences for the specified sentence with the specified pos-tags
		/// </summary>
		/// <param name="sentence">
		/// The tokens of the sentence.
		/// </param>
		/// <param name="tags">
		/// The pos-tags for the specified sentence.
		/// </param>
		/// <param name="minSequenceScore">
		/// </param>
		/// <returns>
		/// the top k chunk sequences for the specified sentence.
		/// </returns>
		Util.Sequence[] TopKSequences(string[] sentence, string[] tags, double minSequenceScore);
	}
}
