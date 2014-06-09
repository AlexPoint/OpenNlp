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

//This file is based on the DefaultPOSContextGenerator.java source file found in the
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
using System.Text.RegularExpressions;

namespace OpenNLP.Tools.PosTagger
{
	/// <summary> 
	/// A context generator for the POS Tagger.
	/// </summary>	
	public class DefaultPosContextGenerator : IPosContextGenerator
	{
		protected internal const string SentenceEnd = "*SE*";
		protected internal const string SentenceBeginning = "*SB*";

		private const int mPrefixLength = 4;
		private const int mSuffixLength = 4;
		
		private static Regex mHasCapitalRegex = new Regex("[A-Z]");
		private static Regex mHasNumericRegex = new Regex("[0-9]");
		
		private Util.Cache mContextsCache;
		private object mWordsKey;

		public DefaultPosContextGenerator() : this(0)
		{
		}
		
		public DefaultPosContextGenerator(int cacheSize) 
		{
			if (cacheSize > 0) 
			{
				mContextsCache = new Util.Cache(cacheSize);
			}
		}

		public virtual string[] GetContext(object input)
		{
			object[] data = (object[]) input;
			return GetContext(((int) data[0]), (object[]) data[1], (string[]) data[2], null);
		}
		
		protected internal static string[] GetPrefixes(string lex)
		{
			string[] prefixes = new string[mPrefixLength];
			for (int currentPrefix = 0; currentPrefix < mPrefixLength; currentPrefix++)
			{
				prefixes[currentPrefix] = lex.Substring(0, (System.Math.Min(currentPrefix + 1, lex.Length)) - (0));
			}
			return prefixes;
		}
		
		protected internal static string[] GetSuffixes(string lex)
		{
			string[] suffixes = new string[mSuffixLength];
			for (int currentSuffix = 0; currentSuffix < mSuffixLength; currentSuffix++)
			{
				suffixes[currentSuffix] = lex.Substring(System.Math.Max(lex.Length - currentSuffix - 1, 0));
			}
			return suffixes;
		}
		
		public virtual string[] GetContext(int index, object[] sequence, string[] priorDecisions, object[] additionalContext) 
		{
			return GetContext(index, sequence, priorDecisions);
		}

		/// <summary>
		/// Returns the context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
		/// </summary>
		/// <param name="index">
		/// The index of the token for which the context is provided.
		/// </param>
		/// <param name="tokens">
		/// The tokens in the sentence.
		/// </param>
		/// <param name="tags">
		/// The tags assigned to the previous words in the sentence.
		/// </param>
		/// <returns>
		/// The context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
		/// </returns>
		public virtual string[] GetContext(int index, object[] tokens, string[] tags) 
		{
			string next, nextNext, lex, previous, previousPrevious;
			string tagPrevious, tagPreviousPrevious;
			tagPrevious = tagPreviousPrevious = null;
			next = nextNext = lex = previous = previousPrevious = null;
			
			lex = tokens[index].ToString();
			if (tokens.Length > index + 1) 
			{
				next = tokens[index + 1].ToString();
				if (tokens.Length > index + 2)
				{
					nextNext = tokens[index + 2].ToString();
				}
				else
				{
					nextNext = SentenceEnd; 
				}
			}
			else
			{
				next = SentenceEnd; 
			}
			
			if (index - 1 >= 0) 
			{
				previous = tokens[index - 1].ToString();
				tagPrevious = tags[index - 1].ToString();

				if (index - 2 >= 0) 
				{
					previousPrevious = tokens[index - 2].ToString();
					tagPreviousPrevious = tags[index - 2].ToString();
				}
				else
				{
					previousPrevious = SentenceBeginning; 
				}
			}
			else
			{
				previous = SentenceBeginning; 
			}
			
			string cacheKey = index.ToString(System.Globalization.CultureInfo.InvariantCulture) + tagPrevious + tagPreviousPrevious;
			if (!(mContextsCache == null)) 
			{
				if (mWordsKey == tokens)
				{
					string[] cachedContexts = (string[]) mContextsCache[cacheKey];    
					if (cachedContexts != null) 
					{
						return cachedContexts;
					}
				}
				else 
				{
					mContextsCache.Clear();
					mWordsKey = tokens;
				}

			}

            List<string> eventList = new List<string>();
			
			// add the word itself
			eventList.Add("w=" + lex);
			
			// do some basic suffix analysis
			string[] suffixes = GetSuffixes(lex);
			for (int currentSuffix = 0; currentSuffix < suffixes.Length; currentSuffix++)
			{
				eventList.Add("suf=" + suffixes[currentSuffix]);
			}
			
			string[] prefixes = GetPrefixes(lex);
			for (int currentPrefix = 0; currentPrefix < prefixes.Length; currentPrefix++)
			{
				eventList.Add("pre=" + prefixes[currentPrefix]);
			}
			// see if the word has any special characters
			if (lex.IndexOf((char) '-') != - 1)
			{
				eventList.Add("h");
			}
			
			if (mHasCapitalRegex.IsMatch(lex)) 
			{
				eventList.Add("c");
			}
			
			if (mHasNumericRegex.IsMatch(lex))
			{
				eventList.Add("d");
			}
			
			// add the words and positions of the surrounding context
			if ((object) previous != null)
			{
				eventList.Add("p=" + previous);
				if ((object) tagPrevious != null)
				{
					eventList.Add("t=" + tagPrevious);
				}
				if ((object) previousPrevious != null)
				{
					eventList.Add("pp=" + previousPrevious);
					if ((object) tagPreviousPrevious != null)
					{
						eventList.Add("tt=" + tagPreviousPrevious);
					}
				}
			}
			
			if ((object) next != null)
			{
				eventList.Add("n=" + next);
				if ((object) nextNext != null)
				{
					eventList.Add("nn=" + nextNext);
				}
			}

			string[] contexts = eventList.ToArray();
			if (mContextsCache != null) 
			{
				mContextsCache[cacheKey] = contexts;
			}
			return contexts;
		}
		
	}
}
