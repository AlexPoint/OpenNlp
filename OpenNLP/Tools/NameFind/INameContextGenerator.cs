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

//This file is based on the NameContextGenerator.java source file found in the
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
using System.Collections.Generic;

namespace OpenNLP.Tools.NameFind
{
	/// <summary>
	/// Context generator for the name find tool.
	/// </summary>
	public interface INameContextGenerator : Util.IBeamSearchContextGenerator
	{	
		/// <summary>
		/// Returns the contexts for chunking of the specified index.
		/// </summary>
		/// <param name="tokenIndex">
		/// The index of the token in the specified tokens array for which the context should be constructed. 
		/// </param>
		/// <param name="tokens">
		/// The tokens of the sentence.
		/// </param>
		/// <param name="previousDecisions">
        /// The previous decisions made in the tagging of this sequence.  Only indices less than tokenIndex will be examined.
		/// </param>
		/// <param name="previousTags">
		/// A mapping between tokens and the previous outcome for these tokens. 
		/// </param>
		/// <returns>
		/// An array of predictive contexts on which a model basis its decisions.
		/// </returns>
		string[] GetContext(int tokenIndex, List<string> tokens, List<string> previousDecisions, IDictionary<string, string> previousTags);
	}
}
