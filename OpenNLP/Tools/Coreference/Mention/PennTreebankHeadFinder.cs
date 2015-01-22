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

//This file is based on the PTBHeadFinder.java source file found in the
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
    /// Finds head information from Penn Treebank style parses.
    /// </summary>
	public sealed class PennTreebankHeadFinder : IHeadFinder
	{
        private static PennTreebankHeadFinder mInstance;
        private static Util.Set<string> mSkipSet = new Util.HashSet<string>();

		/// <summary>
        /// Returns an instance of this head finder.
        /// </summary>
		/// <returns>
        /// An instance of this head finder.
		/// </returns>
		public static IHeadFinder Instance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = new PennTreebankHeadFinder();
				}
				return mInstance;
			}
		}

		private PennTreebankHeadFinder()
		{
		}
		
		public IParse GetHead(IParse parse)
		{
			if (parse == null)
			{
				return null;
			}
			if (parse.IsNounPhrase)
			{
				List<IParse> parts = parse.SyntacticChildren;
				//shallow parse POS
				if (parts.Count > 2)
				{
                    if (parts[1].IsToken && parts[1].SyntacticType == PartsOfSpeech.PossessiveEnding
                        && parts[0].IsNounPhrase && parts[2].IsNounPhrase)
					{
                        return (parts[2]);
					}
				}
				//full parse POS
				if (parts.Count > 1)
				{
                    if (parts[0].IsNounPhrase)
					{
                        List<IParse> childTokens = parts[0].Tokens;
						if (childTokens.Count == 0)
						{
                            Console.Error.WriteLine("PTBHeadFinder: NP " + parts[0] + " with no tokens");
						}
						IParse tok = childTokens[childTokens.Count - 1];
						if (tok.SyntacticType == PartsOfSpeech.PossessiveEnding)
						{
							return null;
						}
					}
				}
				//coordinated nps are their own entities
				if (parts.Count > 1)
				{
					for (int currentPart = 1; currentPart < parts.Count - 1; currentPart++)
					{
                        if (parts[currentPart].IsToken && parts[currentPart].SyntacticType == PartsOfSpeech.CoordinatingConjunction)
						{
							return null;
						}
					}
				}
				//all other NPs
				for (int currentPart = 0; currentPart < parts.Count; currentPart++)
				{
                    if (parts[currentPart].IsNounPhrase)
					{
                        return parts[currentPart];
					}
				}
				return null;
			}
			else
			{
				return null;
			}
		}
		
		public int GetHeadIndex(IParse parse)
		{
			List<IParse> syntacticChildren = parse.SyntacticChildren;
			bool countTokens = false;
			int tokenCount = 0;
			//check for NP -> NN S type structures and return last token before S as head.
            for (int currentSyntacticChild = 0; currentSyntacticChild < syntacticChildren.Count; currentSyntacticChild++)
			{
				IParse syntacticChild = syntacticChildren[currentSyntacticChild];
				if (syntacticChild.SyntacticType.StartsWith("S"))
				{
					if (currentSyntacticChild != 0)
					{
						countTokens = true;
					}
				}
				if (countTokens)
				{
					tokenCount += syntacticChild.Tokens.Count;
				}
			}
			List<IParse> tokens = parse.Tokens;
			if (tokens.Count == 0)
			{
				Console.Error.WriteLine("PTBHeadFinder.getHeadIndex(): empty tok list for parse " + parse);
			}
			for (int currentToken = tokens.Count - tokenCount - 1; currentToken >= 0; currentToken--)
			{
				IParse token = tokens[currentToken];
				if (!mSkipSet.Contains(token.SyntacticType))
				{
					return currentToken;
				}
			}
			return (tokens.Count - tokenCount - 1);
		}
		
		/// <summary>
        /// Returns the bottom-most head of a <code>IParse</code>.  If no
		/// head is available which is a child of <code>parse</code> then
		/// <code>parse</code> is returned. 
		/// </summary>
		public IParse GetLastHead(IParse parse)
		{
			IParse head;

            while (null != (head = GetHead(parse)))
			{
                parse = head;
			}
            return parse;
		}
		
		public IParse GetHeadToken(IParse parse)
		{
			List<IParse> tokens = parse.Tokens;
			return tokens[GetHeadIndex(parse)];
		}

		static PennTreebankHeadFinder()
		{
			mSkipSet.Add(PartsOfSpeech.PossessiveEnding);
			mSkipSet.Add(PartsOfSpeech.Comma);
			mSkipSet.Add(PartsOfSpeech.ColonSemiColon);
			mSkipSet.Add(PartsOfSpeech.SentenceFinalPunctuation);
			mSkipSet.Add(PartsOfSpeech.RightCloseDoubleQuote);
			mSkipSet.Add("-RRB-");
			mSkipSet.Add("-RCB-");
		}
	}
}