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

//This file is based on the AbstractParse.java source file found in the
//original java implementation of OpenNLP.

using System;
using System.Collections.Generic;

namespace OpenNLP.Tools.Coreference.Mention
{
	
	/// <summary>
    /// Provides default implemenation of many of the methods in the IParse interface.
    /// </summary>
	public abstract class AbstractParse : IParse
	{
		public virtual bool IsCoordinatedNounPhrase
		{
			get
			{
				List<IParse> parts = SyntacticChildren;
				if (parts.Count >= 2)
				{
					for (int currentPart = 1; currentPart < parts.Count; currentPart++)
					{
						IParse child = parts[currentPart];
						string childType = child.SyntacticType;
						if (childType != null && childType == "CC" && !(child.ToString() == "&"))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public virtual List<IParse> NounPhrases
		{
			get
			{
                List<IParse> parts = SyntacticChildren;
                List<IParse> nounPhrases = new List<IParse>();
				while (parts.Count > 0)
				{
                    List<IParse> newParts = new List<IParse>();
					for (int currentPart = 0; currentPart < parts.Count; currentPart++)
					{
						//System.err.println("AbstractParse.getNounPhrases "+parts.get(pi).getClass());
						IParse currentPartParse = parts[currentPart];
						if (currentPartParse.IsNounPhrase)
						{
							nounPhrases.Add(currentPartParse);
						}
						if (!currentPartParse.IsToken)
						{
                            newParts.AddRange(currentPartParse.SyntacticChildren);
						}
					}
					parts = newParts;
				}
				return nounPhrases;
			}	
		}

        public abstract List<IParse> NamedEntities { get;}
		public abstract Util.Span Span{get;}
        public abstract List<IParse> Tokens { get;}
        public abstract List<IParse> Children { get;}
		public abstract Coreference.Mention.IParse NextToken{get;}
		public abstract Coreference.Mention.IParse PreviousToken{get;}
		public abstract List<IParse> SyntacticChildren{get;}
		public abstract string SyntacticType{get;}
		public abstract bool ParentNac{get;}
		public abstract bool IsToken{get;}
		public abstract string EntityType{get;}
		public abstract bool IsNamedEntity{get;}
		public abstract bool IsNounPhrase{get;}
		public abstract bool IsSentence{get;}
		public abstract int EntityId{get;}
		public abstract Coreference.Mention.IParse Parent{get;}
		public abstract int SentenceNumber{get;}
		public abstract int CompareTo(object obj);
	}
}