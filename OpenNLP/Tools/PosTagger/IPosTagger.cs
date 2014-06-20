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

//This file is based on the POSTagger.java source file found in the
//original java implementation of OpenNLP. 

using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenNLP.Tools.PosTagger
{
	/// <summary> 
	/// The interface for part of speech taggers.
	/// </summary>
	public interface IPosTagger
	{
			
		/// <summary>Assigns the sentence of tokens pos tags</summary>
		/// <param name="tokens">The sentence of tokens to be tagged</param>
		/// <returns>An array of pos tags for each token provided in sentence</returns>
		string[] Tag(string[] tokens);
			
		/// <summary> Assigns pos tags to the sentence of space-delimited tokens</summary>
		/// <param name="sentence">The sentence of space-delimited tokens to be tagged</param>
		/// <returns>A collection of tagged words (word + pos tag + index in sentence)</returns>
		List<TaggedWord> TagSentence(string sentence);
	}
}
