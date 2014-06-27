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

//This file is based on the Tokenizer.java source file found in the
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
using System.Collections.Generic;

namespace OpenNLP.Tools.Tokenize
{
	/// <summary> 
	/// The interface for tokenizers, which turn messy text into nicely segmented
	/// text tokens.
	/// </summary>
	public interface ITokenizer
	{
		/// <summary> 
		/// Tokenize a string.
		/// </summary>
		/// <param name="input">
		/// The string to be tokenized.
		/// </param>
		/// <returns>
		/// The string[] with the individual tokens as the array
		/// elements.
		/// </returns>
		string[] Tokenize(string input);
			
		/// <summary>Tokenize a string</summary>
		/// <param name="input">The string to be tokenized</param>
		/// <returns>
		/// The Span[] with the spans (offsets into input) for each
		/// token as the individuals array elements.
		/// </returns>
		Util.Span[] TokenizePositions(string input);

        /// <summary>
        /// Tests the current Tokenizer on a given set of test data.
        /// The test data is composed of sentences associated to the collection
        /// of spans delimitating the different tokens.
        /// </summary>
	    TokenizationTestResults RunAgainstTestData(List<TokenizerTestData> inputsAndAssociatedTokens);
	}
}
