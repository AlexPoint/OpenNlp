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

//This file is based on the JWNLDictionary.java source file found in the
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
using SharpWordNet;
using SharpWordNet.Morph;

namespace OpenNLP.Tools.Coreference.Mention
{
	/// <summary> 
    /// An implementation of the Dictionary interface using the SharpWordnet library. </summary>
	public class WordnetDictionary : IDictionary
	{
        private WordNetEngine mEngine;
        private IOperation[] mDefaultOperations;

        private static string[] empty = new string[0];
		
		public WordnetDictionary(string searchDirectory)
		{
            mEngine = new DataFileEngine(searchDirectory);
            Dictionary<string, string[][]> suffixMap = new Dictionary<string, string[][]>();
            suffixMap.Add("noun", new string[][] { new string[] { "s", "" }, new string[] { "ses", "s" }, new string[] { "xes", "x" }, new string[] { "zes", "z" }, new string[] { "ches", "ch" }, new string[] { "shes", "sh" }, new string[] { "men", "man" }, new string[] { "ies", "y" } });
            suffixMap.Add("verb", new string[][] { new string[] { "s", "" }, new string[] { "ies", "y" }, new string[] { "es", "e" }, new string[] { "es", "" }, new string[] { "ed", "e" }, new string[] { "ed", "" }, new string[] { "ing", "e" }, new string[] { "ing", "" } });
            suffixMap.Add("adjective", new string[][] { new string[] { "er", "" }, new string[] { "est", "" }, new string[] { "er", "e" }, new string[] { "est", "e" } });
            DetachSuffixesOperation tokDso = new DetachSuffixesOperation(suffixMap);
            tokDso.AddDelegate(DetachSuffixesOperation.Operations, new IOperation[] { new LookupIndexWordOperation(mEngine), new LookupExceptionsOperation(mEngine) });
            TokenizerOperation tokOp = new TokenizerOperation(mEngine, new string[] { " ", "-" });
            tokOp.AddDelegate(TokenizerOperation.TokenOperations, new IOperation[] { new LookupIndexWordOperation(mEngine), new LookupExceptionsOperation(mEngine), tokDso });
            DetachSuffixesOperation morphDso = new DetachSuffixesOperation(suffixMap);
            morphDso.AddDelegate(DetachSuffixesOperation.Operations, new IOperation[] { new LookupIndexWordOperation(mEngine), new LookupExceptionsOperation(mEngine) });
            mDefaultOperations = new IOperation[] { new LookupExceptionsOperation(mEngine), morphDso, tokOp };
		}
		
		public virtual string[] GetLemmas(string word, string tag)
		{
            string partOfSpeech;
            if (tag.StartsWith("N") || tag.StartsWith("n"))
            {
                partOfSpeech = "noun";
            }
            else if (tag.StartsWith("N") || tag.StartsWith("v"))
            {
                partOfSpeech = "verb";
            }
            else if (tag.StartsWith("J") || tag.StartsWith("a"))
            {
                partOfSpeech = "adjective";
            }
            else if (tag.StartsWith("R") || tag.StartsWith("r"))
            {
                partOfSpeech = "adverb";
            }
            else
            {
                partOfSpeech = "noun";
            }
            
            return mEngine.GetBaseForms(word, partOfSpeech, mDefaultOperations);
		}
		
		public virtual string GetSenseKey(string lemma, string partOfSpeech, int sense)
		{
            IndexWord indexWord = mEngine.GetIndexWord(lemma, "noun");
            if (indexWord == null)
            {
                return null;
            }
            return indexWord.SynsetOffsets[sense].ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public virtual int GetSenseCount(string lemma, string pos)
		{
            IndexWord indexWord = mEngine.GetIndexWord(lemma, "noun");
            if (indexWord == null)
            {
                return 0;
            }
            
            return indexWord.SenseCount;
		}
	
        private void GetParents(Synset currentSynset, List<string> parentOffsets)
        {
            for (int currentRelation = 0;currentRelation < currentSynset.RelationCount;currentRelation++)
            {
                Relation relation = currentSynset.GetRelation(currentRelation);
                if (relation.SynsetRelationType.Name == "Hypernym")
                {
                    Synset parentSynset = relation.TargetSynset;
                    parentOffsets.Add(parentSynset.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    GetParents(parentSynset, parentOffsets);
                }
            }
        }

        public virtual string[] GetParentSenseKeys(string lemma, string partOfSpeech, int sense)
        {
            Synset[] synsets = mEngine.GetSynsets(lemma, "noun");
            if (synsets.Length > sense)
            {
                List<string> parents = new List<string>();
                GetParents(synsets[sense], parents);
                return parents.ToArray();
            }
            else
            {
                return empty;
            }
        }
	}
}