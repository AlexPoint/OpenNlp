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

//This file is based on the Dictionary.java source file found in the
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
    /// Interface to provide dictionary information to the coreference module assuming a
	/// hierarchically structured dictionary (such as WordNet) is available. 
	/// </summary>
	public interface IDictionary
	{
		/// <summary>
        /// Returns the lemmas of the specified word with the specified part-of-speech.
        /// </summary>
		/// <param name="word">
        /// The word whose lemmas are desired.
		/// </param>
		/// <param name="partOfSpeech">The part-of-speech of the specified word.
		/// </param>
		/// <returns>
        /// The lemmas of the specified word given the specified part-of-speech.
		/// </returns>
		string[] GetLemmas(string word, string partOfSpeech);
		
		/// <summary> 
        /// Returns a key indicating the specified sense number of the specified 
		/// lemma with the specified part-of-speech.  
		/// </summary>
		/// <param name="lemma">
        /// The lemmas for which the key is desired.
		/// </param>
		/// <param name="partOfSpeech">
        /// The part of speech for which the key is desired.
		/// </param>
		/// <param name="senseNumber">
        /// The sense number for which the key is desired.
		/// </param>
		/// <returns> 
        /// a key indicating the specified sense number of the specified 
		/// lemma with the specified part-of-speech.
		/// </returns>
		string GetSenseKey(string lemma, string partOfSpeech, int senseNumber);
		
		/// <summary> 
        /// Returns the number of senses in the dictionry for the specified lemma.
        /// </summary>
		/// <param name="lemma">
        /// A lemmatized form of the word to look up.
		/// </param>
		/// <param name="partOfSpeech">
        /// The part-of-speech for the lemma.
		/// </param>
		/// <returns> the number of senses in the dictionary for the specified lemma.
		/// </returns>
        int GetSenseCount(string lemma, string partOfSpeech);
		
		/// <summary>
        /// Returns an array of keys for each parent of the specified sense number of the specified 
        /// lemma with the specified part-of-speech.
        /// </summary>
		/// <param name="lemma">
        /// A lemmatized form of the word to look up.
		/// </param>
        /// <param name="partOfSpeech">
        /// The part-of-sppech for the lemma.
		/// </param>
		/// <param name="senseNumber">
        /// The sense number for which the parent keys are desired.
		/// </param>
		/// <returns>
        /// an array of keys for each parent of the specified sense number of the specified lemma 
        /// with the specified part-of-speech.
		/// </returns>
        string[] GetParentSenseKeys(string lemma, string partOfSpeech, int senseNumber);
	}
}