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

//This file is based on the Chunker.java source file found in the
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

namespace OpenNLP.Tools.Chunker
{
	/// <summary>
	/// Features based on chunking model described in Fei Sha and Fernando Pereira. Shallow 
	/// parsing with conditional random fields. In Proceedings of HLT-NAACL 2003. Association 
	/// for Computational Linguistics, 2003.
	/// </summary>
	/// <author> 
	/// Tom Morton
	/// </author>
	public class DefaultChunkerContextGenerator : IChunkerContextGenerator
	{
		
		/// <summary>
		/// Creates the default context generator for a chunker.
		/// </summary>
		public DefaultChunkerContextGenerator() : base()
		{
		}

		/// <summary>
		/// Returns the contexts for chunking of the specified index.
		/// </summary>
		/// <param name="input">
		/// An object array containing:
		/// at index [0]: integer value, the index of the token in the tokens array for which the context should be constructed.
		/// at index [1]: object array, the ToString() methods of these objects make up the tokens of the sentence
		/// at index [2]: a Util.Sequence of previous decisions
		/// at index [3]: a string array, the POS tags for the specified tokens 
		/// </param>
		/// <returns>
		/// An array of predictive contexts on which a model bases its decisions.
		/// </returns>
		public virtual string[] GetContext(object input)
		{
			object[] data = (object[]) input;
			string[] outcomes = ((Util.Sequence) data[2]).Outcomes.ToArray();
			return (GetContext(((int)data[0]), (object[])data[1], (string[])data[3], outcomes));
		}
		
		/// <summary>
		/// Returns the contexts for chunking of the specified index.
		/// </summary>
		/// <param name="index">
		/// The index of the token in the specified tokens array for which the context should be constructed. 
		/// </param>
		/// <param name="sequence">
		/// The tokens of the sentence.  The <code>ToString</code> methods of these objects should return the token text.
		/// </param>
		/// <param name="priorDecisions">
		/// The previous decisions made in the tagging of this sequence.  Only indices less than index will be examined.
		/// </param>
		/// <param name="additionalContext">
		/// Object array of additional context information. The first object in the array is expected to be a string array
		/// containing the POS tags for the the specified tokens.
		/// </param>
		/// <returns>
		/// An array of predictive contexts on which a model bases its decisions.
		/// </returns>
		public virtual string[] GetContext(int index, object[] sequence, string[] priorDecisions, object[] additionalContext) 
		{
			return GetContext(index, sequence, (string[])additionalContext[0], priorDecisions); 
		}  

		/// <summary>
		/// Returns the contexts for chunking of the specified index.
		/// </summary>
		/// <param name="tokenIndex">
		/// The index of the token in the specified tokens array for which the context should be constructed. 
		/// </param>
		/// <param name="tokens">
		/// The tokens of the sentence.  The <code>ToString</code> methods of these objects should return the token text.
		/// </param>
		/// <param name="tags">
		/// The POS tags for the the specified tokens.
		/// </param>
		/// <param name="predicates">
		/// The previous decisions made in the tagging of this sequence.  Only indices less than tokenIndex will be examined.
		/// </param>
		/// <returns>
		/// An array of predictive contexts on which a model bases its decisions.
		/// </returns>
		public virtual string[] GetContext(int tokenIndex, object[] tokens, string[] tags, string[] predicates)
		{
            List<string> features = new List<string>(45);
			//words in a 5-word window
			string wordPreviousPrevious, wordPrevious, word, wordNext, wordNextNext;
			//tags in a 5-word window 
			string tagPreviousPrevious, tagPrevious, tag, tagNext, tagNextNext;
			//Previous predictions
			string predicatePreviousPrevious, predicatePrevious;
			if (tokenIndex < 2)
			{
				wordPreviousPrevious = "w_2=bos";
				tagPreviousPrevious = "t_2=bos";
				predicatePreviousPrevious = "p_2=bos";
			}
			else
			{
				wordPreviousPrevious = "w_2=" + tokens[tokenIndex - 2];
				tagPreviousPrevious = "t_2=" + tags[tokenIndex - 2];
				predicatePreviousPrevious = "p_2" + predicates[tokenIndex - 2];
			}
			if (tokenIndex < 1)
			{
				wordPrevious = "w_1=bos";
				tagPrevious = "t_1=bos";
				predicatePrevious = "p_1=bos";
			}
			else
			{
				wordPrevious = "w_1=" + tokens[tokenIndex - 1];
				tagPrevious = "t_1=" + tags[tokenIndex - 1];
				predicatePrevious = "p_1=" + predicates[tokenIndex - 1];
			}
			word = "w0=" + tokens[tokenIndex];
			tag = "t0=" + tags[tokenIndex];
			if (tokenIndex + 1 >= tokens.Length)
			{
				wordNext = "w1=eos";
				tagNext = "t1=eos";
			}
			else
			{
				wordNext = "w1=" + tokens[tokenIndex + 1];
				tagNext = "t1=" + tags[tokenIndex + 1];
			}
			if (tokenIndex + 2 >= tokens.Length)
			{
				wordNextNext = "w2=eos";
				tagNextNext = "t2=eos";
			}
			else
			{
				wordNextNext = "w2=" + tokens[tokenIndex + 2];
				tagNextNext = "t2=" + tags[tokenIndex + 2];
			}

			//add word features
			features.Add(wordPreviousPrevious);
			features.Add(wordPrevious);
			features.Add(word);
			features.Add(wordNext);
			features.Add(wordNextNext);
			features.Add(wordPrevious + word);
			features.Add(word + wordNext);

			//add tag features
			features.Add(tagPreviousPrevious);
			features.Add(tagPrevious);
			features.Add(tag);
			features.Add(tagNext);
			features.Add(tagNextNext);
			features.Add(tagPreviousPrevious + tagPrevious);
			features.Add(tagPrevious + tag);
			features.Add(tag + tagNext);
			features.Add(tagNext + tagNextNext);
			features.Add(tagPreviousPrevious + tagPrevious + tag);
			features.Add(tagPrevious + tag + tagNext);
			features.Add(tag + tagNext + tagNextNext);

			//add pred tags
			features.Add(predicatePreviousPrevious);
			features.Add(predicatePrevious);
			features.Add(predicatePreviousPrevious + predicatePrevious);

			//add pred and tag
			features.Add(predicatePrevious + tagPreviousPrevious);
			features.Add(predicatePrevious + tagPrevious);
			features.Add(predicatePrevious + tag);
			features.Add(predicatePrevious + tagNext);
			features.Add(predicatePrevious + tagNextNext);
			features.Add(predicatePrevious + tagPreviousPrevious + tagPrevious);
			features.Add(predicatePrevious + tagPrevious + tag);
			features.Add(predicatePrevious + tag + tagNext);
			features.Add(predicatePrevious + tagNext + tagNextNext);
			features.Add(predicatePrevious + tagPreviousPrevious + tagPrevious + tag);
			features.Add(predicatePrevious + tagPrevious + tag + tagNext);
			features.Add(predicatePrevious + tag + tagNext + tagNextNext);

			//add pred and word
			features.Add(predicatePrevious + wordPreviousPrevious);
			features.Add(predicatePrevious + wordPrevious);
			features.Add(predicatePrevious + word);
			features.Add(predicatePrevious + wordNext);
			features.Add(predicatePrevious + wordNextNext);
			features.Add(predicatePrevious + wordPrevious + word);
			features.Add(predicatePrevious + word + wordNext);
			return features.ToArray();
		}
	}
}
