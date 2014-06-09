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

//This file is based on the EnglishTreebankChunker.java source file found in the
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
using System.Text;

namespace OpenNLP.Tools.Chunker
{
	/// <summary>
	/// This is a chunker based on the CONLL chunking task which uses Penn Treebank constituents as the basis for the chunks.
	/// See   http://cnts.uia.ac.be/conll2000/chunking/ for data and task definition.
	/// </summary>
	/// <author> 
	/// Tom Morton
	/// </author>
	public class EnglishTreebankChunker : MaximumEntropyChunker
	{
		/// <summary>
		/// Creates an English Treebank Chunker which uses the specified model file.
		/// </summary>
		/// <param name="modelFile">
		/// The name of the maxent model to be used.
		/// </param>
		public EnglishTreebankChunker(string modelFile) : base(new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)))
		{
		}
						
		/// <summary>
		/// This method determines whether the outcome is valid for the preceding sequence.  
		/// This can be used to implement constraints on what sequences are valid.  
		/// </summary>
		/// <param name="outcome">
		/// The outcome.
		/// </param>
		/// <param name="sequence">
		/// The preceding sequence of outcome assignments. 
		/// </param>
		/// <returns>
		/// true if the outcome is valid for the sequence, false otherwise.
		/// </returns>
		protected internal override bool ValidOutcome(string outcome, Util.Sequence sequence)
		{
			if (outcome.StartsWith("I-"))
			{
				string[] tags = sequence.Outcomes.ToArray();
				int lastTagIndex = tags.Length - 1;
				if (lastTagIndex == - 1)
				{
					return (false);
				}
				else
				{
					string lastTag = tags[lastTagIndex];
					if (lastTag == "O")
					{
						return false;
					}
					if (lastTag.Substring(2) != outcome.Substring(2))
					{
						return false;
					}
				}
			}
			return true;
		}
		
		/// <summary>
		/// Gets formatted chunk information for a specified sentence.
		/// </summary>
		/// <param name="tokens">
		/// string array of tokens in the sentence
		/// </param>
		/// <param name="tags">
		/// string array of POS tags for the tokens in the sentence
		/// </param>
		/// <returns>
		/// A string containing the formatted chunked sentence
		/// </returns>
		public string GetChunks(string[] tokens, string[] tags)
		{
			StringBuilder output = new StringBuilder();

			string[] chunks = Chunk(tokens, tags);
			for (int currentChunk = 0, chunkCount = chunks.Length; currentChunk < chunkCount; currentChunk++)
			{
				if (currentChunk > 0 && !chunks[currentChunk].StartsWith("I-") && chunks[currentChunk - 1] != "O")
				{
					output.Append(" ]");
				}
				if (chunks[currentChunk].StartsWith("B-"))
				{
					output.Append(" [" + chunks[currentChunk].Substring(2));
				}
				
				output.Append(" " + tokens[currentChunk] + "/" + tags[currentChunk]);
			}
			if (chunks[chunks.Length - 1] != "O")
			{
				output.Append(" ]");
			}
			output.Append("\r\n");

			return output.ToString();

		}

		/// <summary>
		/// Gets formatted chunk information for a specified sentence.
		/// </summary>
		/// <param name="data">
		/// a string containing a list of tokens and tags, separated by / characters. For example:
		/// Battle-tested/JJ Japanese/NNP industrial/JJ managers/NNS 
		/// </param>
		/// <returns>
		/// A string containing the formatted chunked sentence
		/// </returns>
		public string GetChunks(string data)
		{
			StringBuilder output = new StringBuilder();
			string[] tokenAndTags = data.Split(' ');
			string[] tokens = new string[tokenAndTags.Length];
			string[] tags = new string[tokenAndTags.Length];
			for (int currentTokenAndTag = 0, tokenAndTagCount = tokenAndTags.Length; currentTokenAndTag < tokenAndTagCount; currentTokenAndTag++)
			{
				string[] tokenAndTag = tokenAndTags[currentTokenAndTag].Split('/');
				tokens[currentTokenAndTag] = tokenAndTag[0];
				tags[currentTokenAndTag] = tokenAndTag[1];
			}

			return GetChunks(tokens, tags);
		}
	}
}
