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

//This file is based on the Parse.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Mention
{
	/// <summary>
    /// Interface for syntactic and named-entity information to be used in coreference annotation.
    /// </summary>
	public interface IParse : IComparable
	{
		/// <summary>
        /// Returns the index of the sentence which contains this parse.
        /// </summary>
		int SentenceNumber
		{
			get;
		}

		/// <summary>
        /// Returns a list of the all noun phrases contained by this parse.
		/// </summary>
		List<IParse> NounPhrases
		{
			get;
		}

		/// <summary>
        /// Returns a list of all the named entities contained by this parse.
		/// </summary>
        List<IParse> NamedEntities
		{
			get;	
		}

		/// <summary>
        /// Returns a list of the children to this object.
		/// </summary>
        List<IParse> Children
		{
			get;
		}

		/// <summary>
        /// Returns a list of the children to this object which are constituents or tokens.
        /// This allows implementations which contain addition nodes for things such as semantic 
        /// categories to hide those nodes from the components which only care about syntactic nodes. 
		/// </summary>
		List<IParse> SyntacticChildren
		{
			get;
		}

		/// <summary>
        /// Returns a list of the tokens contained by this object.
		/// </summary>
        List<IParse> Tokens
		{
			get;
		}

		/// <summary>
        /// Returns the syntactic type of this node. Typically this is the part-of-speech or 
		/// constituent labeling.
		/// </summary>
		string SyntacticType
		{
			get;
		}

		/// <summary>
        /// Returns the named-entity type of this node.
        /// </summary>
		string EntityType
		{
			get;
		}

		/// <summary>
        /// Determines whether this has an ancestor of type NAC.
        /// </summary>
		bool ParentNac
		{
			get;
		}

		/// <summary>
        /// Returns the parent parse of this parse node.
        /// </summary>
		IParse Parent
		{
			get;
		}

		/// <summary>
        /// Specifies whether this parse is a named-entity.
        /// </summary>
		bool IsNamedEntity
		{
			get;
		}

		/// <summary>
        /// Specifies whether this parse is a noun phrase.
        /// </summary>
		bool IsNounPhrase
		{
			get;	
		}

		/// <summary>
        /// Specifies whether this parse is a sentence.
        /// </summary>
		bool IsSentence
		{
			get;
		}

		/// <summary>
        /// Specifies whether this parse is a coordinated noun phrase.
        /// </summary>
		bool IsCoordinatedNounPhrase
		{
			get;
		}

		/// <summary>
        /// Specifies whether this parse is a token.
        /// </summary>
		bool IsToken
		{
			get;
		}

		/// <summary>
        /// Returns an entity id associated with this parse and coreferent parses.  This is only used for training on
		/// already annotated coreference annotation.
		/// </summary>
		int EntityId
		{
			get;
		}

		/// <summary>
        /// Returns the character offsets of this parse node.
        /// </summary>
		Util.Span Span
		{
			get;	
		}

		/// <summary>
        /// Returns the first token which is not a child of this parse.  If the first token of a sentence is
		/// a child of this parse then null is returned.
		/// </summary>
		IParse PreviousToken
		{
			get;
		}

		/// <summary>
        /// Returns the next token which is not a child of this parse.  If the last token of a sentence is
		/// a child of this parse then null is returned.
		/// </summary>
		IParse NextToken
		{
			get;
		}
	}
}