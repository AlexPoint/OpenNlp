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

//This file is based on the ChunkContextGenerator.java source file found in the
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

namespace OpenNLP.Tools.Parser
{
	/// <summary>
	/// Creates predictive context for the pre-chunking phases of parsing.
	/// </summary>
	public class ChunkContextGenerator : Chunker.IChunkerContextGenerator
	{
		private const string mEndOfSentence = "eos";
		
		private Util.Cache mContextsCache;
		private object mWordsKey;

		public ChunkContextGenerator() : this(0)
		{
		}
  
		public ChunkContextGenerator(int cacheSize) : base()
		{
			if (cacheSize > 0) 
			{
				mContextsCache = new Util.Cache(cacheSize);
			}
		}

		public virtual string[] GetContext(object input)
		{
			object[] data = (object[]) input;
			return (GetContext(((int) data[0]), (string[]) data[1], (string[]) data[2], (string[]) data[3]));
		}
		
		public virtual string[] GetContext(int index, object[] words, string[] previousDecisions, object[] additionalContext) 
		{
			return(GetContext(index, words, (string[]) additionalContext[0], previousDecisions));
		}
		
		/// <summary>
		/// Returns the contexts for chunking of the specified index.
		/// </summary>
		/// <param name="index">
		/// The index of the token in the specified tokens array for which the context should be constructed. 
		/// </param>
		/// <param name="words">
		/// The tokens of the sentence.  The <code>ToString()</code> methods of these objects should return the token text.
		/// </param>
		/// <param name="predicates">
		/// The previous decisions made in the tagging of this sequence.  Only indices less than i will be examined.
		/// </param>
		/// <param name="tags">
		/// The POS tags for the the specified tokens.
		/// </param>
		/// <returns>
		/// An array of predictive contexts on which a model basis its decisions.
		/// </returns>
		public virtual string[] GetContext(int index, object[] words, string[] tags, string[] predicates)
		{
            List<string> features = new List<string>(19);
			int currentTokenIndex = index;
			int previousPreviousTokenIndex = currentTokenIndex - 2;
			int previousTokenIndex = currentTokenIndex - 1;
			int nextNextTokenIndex = currentTokenIndex + 2;
			int nextTokenIndex = currentTokenIndex + 1;

			string previousPreviousWord, previousWord, currentWord, nextWord, nextNextWord;
			string previousPreviousTag, previousTag, currentTag, nextTag, nextNextTag;
			string previousPreviousPriorDecision, previousPriorDecision;
			
			string[] contexts;

			// ChunkAndPosTag(-2)
			if (previousPreviousTokenIndex >= 0)
			{
				previousPreviousTag = tags[previousPreviousTokenIndex];
				previousPreviousPriorDecision = predicates[previousPreviousTokenIndex];
				previousPreviousWord = words[previousPreviousTokenIndex].ToString();
			}
			else
			{
				previousPreviousTag = mEndOfSentence;
				previousPreviousPriorDecision = mEndOfSentence;
				previousPreviousWord = mEndOfSentence;
			}
			
			// ChunkAndPosTag(-1)
			if (previousTokenIndex >= 0)
			{
				previousTag = tags[previousTokenIndex];
				previousPriorDecision = predicates[previousTokenIndex];
				previousWord = words[previousTokenIndex].ToString();
			}
			else
			{
				previousTag = mEndOfSentence;
				previousPriorDecision = mEndOfSentence;
				previousWord = mEndOfSentence;
			}
			
			// ChunkAndPosTag(0)
			currentTag = tags[currentTokenIndex];
			currentWord = words[currentTokenIndex].ToString();
			
			// ChunkAndPosTag(1)
			if (nextTokenIndex < tags.Length)
			{
				nextTag = tags[nextTokenIndex];
				nextWord = words[nextTokenIndex].ToString();
			}
			else
			{
				nextTag = mEndOfSentence;
				nextWord = mEndOfSentence;
			}
			
			// ChunkAndPosTag(2)
			if (nextNextTokenIndex < tags.Length)
			{
				nextNextTag = tags[nextNextTokenIndex];
				nextNextWord = words[nextNextTokenIndex].ToString();
			}
			else
			{
				nextNextTag = mEndOfSentence;
				nextNextWord = mEndOfSentence;
			}
			
			string cacheKey = currentTokenIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + previousPreviousTag + previousTag
				+ currentTag + nextTag + nextNextTag + previousPreviousPriorDecision
				+ previousPriorDecision;

			if (mContextsCache != null) 
			{
				if (mWordsKey == words) 
				{
					contexts = (string[]) mContextsCache[cacheKey];
					if (contexts != null) 
					{
						return contexts;
					}
				}
				else 
				{
					mContextsCache.Clear();
					mWordsKey = words;
				}
			}

			string previousPreviousChunkTag = ChunkAndPosTag(-2, previousPreviousWord, previousPreviousTag, previousPreviousPriorDecision);
			string previousPreviousChunkTagBackOff = ChunkAndPosTagBackOff(-2, previousPreviousTag, previousPreviousPriorDecision);
			string previousChunkTag = ChunkAndPosTag(-1, previousWord, previousTag, previousPriorDecision);
			string previousChunkTagBackOff = ChunkAndPosTagBackOff(-1, previousTag, previousPriorDecision);
			string currentChunkTag = ChunkAndPosTag(0, currentWord, currentTag, null);
			string currentChunkTagBackOff = ChunkAndPosTagBackOff(0, currentTag, null);
			string nextChunkTag = ChunkAndPosTag(1, nextWord, nextTag, null);
			string nextChunkTagBackOff = ChunkAndPosTagBackOff(1, nextTag, null);
			string nextNextChunkTag = ChunkAndPosTag(2, nextNextWord, nextNextTag, null);
			string nextNextChunkTagBackOff = ChunkAndPosTagBackOff(2, nextNextTag, null);

			features.Add("default");
			features.Add(previousPreviousChunkTag);
			features.Add(previousPreviousChunkTagBackOff);
			features.Add(previousChunkTag);
			features.Add(previousChunkTagBackOff);
			features.Add(currentChunkTag);
			features.Add(currentChunkTagBackOff);
			features.Add(nextChunkTag);
			features.Add(nextChunkTagBackOff);
			features.Add(nextNextChunkTag);
			features.Add(nextNextChunkTagBackOff);
			
			//ChunkAndPosTag(-1,0)
			features.Add(previousChunkTag + "," + currentChunkTag);
			features.Add(previousChunkTagBackOff + "," + currentChunkTag);
			features.Add(previousChunkTag + "," + currentChunkTagBackOff);
			features.Add(previousChunkTagBackOff + "," + currentChunkTagBackOff);
			
			//ChunkAndPosTag(0,1)
			features.Add(currentChunkTag + "," + nextChunkTag);
			features.Add(currentChunkTagBackOff + "," + nextChunkTag);
			features.Add(currentChunkTag + "," + nextChunkTagBackOff);
			features.Add(currentChunkTagBackOff + "," + nextChunkTagBackOff);
			
			contexts = features.ToArray();
			if (mContextsCache != null)
			{
				mContextsCache[cacheKey] = contexts;
			}
			return contexts;
		}
		
		private string ChunkAndPosTag(int index, string token, string tag, string chunk)
		{
			System.Text.StringBuilder feature = new System.Text.StringBuilder(20);
			feature.Append(index).Append("=").Append(token).Append("|").Append(tag);
			if (index < 0)
			{
				feature.Append("|").Append(chunk);
			}
			return (feature.ToString());
		}
		
		private string ChunkAndPosTagBackOff(int index, string tag, string chunk)
		{
			System.Text.StringBuilder feature = new System.Text.StringBuilder(20);
			feature.Append(index).Append("*=").Append(tag);
			if (index < 0)
			{
				feature.Append("|").Append(chunk);
			}
			return (feature.ToString());
		}
	}
}
