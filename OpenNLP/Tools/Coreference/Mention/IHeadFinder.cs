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

//This file is based on the HeadFinder.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Mention
{
	/// <summary>
    /// Interface for finding head words in noun phrases and head noun-phrases in parses.
    /// </summary>
	public interface IHeadFinder
	{
		/// <summary>
        /// Returns the child parse which contains the lexical head of the specifie parse.
        /// </summary>
		/// <param name="parse">
        /// The parse in which to find the head.
		/// </param>
		/// <returns>
        /// The parse containing the lexical head of the specified parse.  If no head is
		/// available or the constituent has no sub-components that are eligible heads then null is returned.
		/// </returns>
		IParse GetHead(IParse parse);
		
		/// <summary>
        /// Returns which index the specified list of token is the head word.
        /// </summary>
		/// <param name="parse">
        /// The parse in which to find the head index.
		/// </param>
		/// <returns>
        /// The index of the head token.  
		/// </returns>
		int GetHeadIndex(IParse parse);
		
		/// <summary>
        /// Returns the parse bottom-most head of a <code>IParse</code>.  If no
		/// head is available which is a child of <code>parse</code> then
		/// <code>parse</code> is returned. 
		/// </summary>
        /// <param name="parse">
        /// Parse to find the head of.
		/// </param>
        /// <returns>
        /// bottom-most head of parse.
		/// </returns>
		IParse GetLastHead(IParse parse);
		
		/// <summary>
        /// Returns head token for the specified nounPhrase parse.
        /// </summary>
        /// <param name="nounPhrase">The noun parse to get head from.
		/// </param>
		/// <returns> head token parse.
		/// </returns>
        IParse GetHeadToken(IParse nounPhrase);
	}
}