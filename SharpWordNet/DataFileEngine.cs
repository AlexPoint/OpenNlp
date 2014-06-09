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

using System;
using System.IO;
using System.Collections.Generic;

namespace SharpWordNet
{
	/// <summary>
	/// Summary description for DataFileEngine.
	/// </summary>
	public class DataFileEngine : WordNetEngine
	{
		private string mDataFolder;
		private Dictionary<string, PosDataFileSet> mDataFileDictionary;
		private string[] mLexicographerFiles;
		private Dictionary<string, RelationType> mRelationTypeDictionary;

#region Public Methods (class specific)
		public string DataFolder
		{
			get
			{
				return mDataFolder; 
			}
		}

		public DataFileEngine(string dataFolder)
		{
			mDataFolder = dataFolder;

            mDataFileDictionary = new Dictionary<string, PosDataFileSet>(4);

			mDataFileDictionary.Add("noun", new PosDataFileSet(dataFolder, "noun"));
			mDataFileDictionary.Add("verb", new PosDataFileSet(dataFolder, "verb"));
			mDataFileDictionary.Add("adjective", new PosDataFileSet(dataFolder, "adj"));
			mDataFileDictionary.Add("adverb", new PosDataFileSet(dataFolder, "adv"));

			InitializeLexicographerFiles();
			
			InitializeRelationTypes();
		}
#endregion

#region abstract methods implementation

		public override string[] GetPartsOfSpeech()
		{
			return new List<string>(mDataFileDictionary.Keys).ToArray();
		}

		public override string[] GetPartsOfSpeech(string lemma)
		{
            List<string> partsOfSpeech = new List<string>();
			foreach (string partOfSpeech in mDataFileDictionary.Keys)
			{
                if (BinarySearch(lemma, mDataFileDictionary[partOfSpeech].IndexFile) != null)
				{
					partsOfSpeech.Add(partOfSpeech);
				}
			}
			return partsOfSpeech.ToArray();
		}

        public override IndexWord[] GetAllIndexWords(string partOfSpeech)
        {
            StreamReader searchFile = mDataFileDictionary[partOfSpeech].IndexFile;
            string line;
            string space = " ";
            List<IndexWord> indexWords = new List<IndexWord>();
            searchFile.DiscardBufferedData();
            searchFile.BaseStream.Position = 0;
            while (!searchFile.EndOfStream)
            {
                line = searchFile.ReadLine();
                if (!line.StartsWith(space))
                {
                    indexWords.Add(CreateIndexWord(partOfSpeech, line));
                }
            }
            return indexWords.ToArray();
        }

        public override IndexWord GetIndexWord(string lemma, string partOfSpeech)
        {
            string line = BinarySearch(lemma, mDataFileDictionary[partOfSpeech].IndexFile);
            if (line != null)
            {
                return CreateIndexWord(partOfSpeech, line);
            }
            return null;
        }

        public override Synset[] GetSynsets(string lemma)
		{
            List<Synset> synsets = new List<Synset>();

			foreach (string partOfSpeech in mDataFileDictionary.Keys)
			{
                IndexWord indexWord = GetIndexWord(lemma, partOfSpeech);

                if (indexWord != null)
				{
                    foreach (int synsetOffset in indexWord.SynsetOffsets)
					{
						Synset synset = CreateSynset(partOfSpeech, synsetOffset);
						synsets.Add(synset);
					}
				}	
			}
			return synsets.ToArray();
		}

        public override Synset[] GetSynsets(string lemma, string partOfSpeech)
		{
            List<Synset> synsets = new List<Synset>();

            IndexWord indexWord = GetIndexWord(lemma, partOfSpeech);

            if (indexWord != null)
            {
				foreach (int synsetOffset in indexWord.SynsetOffsets)
				{
					Synset synset = CreateSynset(partOfSpeech, synsetOffset);
					synsets.Add(synset);
				}
			}

			return synsets.ToArray();
		}

		public override RelationType[] GetRelationTypes(string lemma, string partOfSpeech)
		{
            IndexWord indexWord = GetIndexWord(lemma, partOfSpeech);

            if (indexWord != null)
            {
                if (indexWord.RelationTypes != null)
				{
                    int relationTypeCount = indexWord.RelationTypes.Length;
					RelationType[] relationTypes = new RelationType[relationTypeCount];
					for (int currentRelationType = 0; currentRelationType < relationTypeCount; currentRelationType++)
					{
                        relationTypes[currentRelationType] = mRelationTypeDictionary[indexWord.RelationTypes[currentRelationType]];
					}
					return relationTypes;
				}
				return null;
			}
			return null;
		}

		public override Synset GetSynset(string lemma, string partOfSpeech, int senseNumber)
		{
			if (senseNumber < 1)
			{
				throw new ArgumentOutOfRangeException("senseNumber", senseNumber, "cannot be less than 1");
			}

            IndexWord indexWord = GetIndexWord(lemma, partOfSpeech);

            if (indexWord != null)
            {
                if (senseNumber > (indexWord.SynsetOffsets.Length + 1)) 
				{
					return (null);
				}
				Synset synset = CreateSynset(partOfSpeech, indexWord.SynsetOffsets[senseNumber - 1]);
				return (synset);
			}
			return null;
		}

#endregion

#region Private Methods

        private string BinarySearch(string searchKey, StreamReader searchFile)
        {
            if (searchKey.Length == 0)
            {
                return null;
            }

			int c,n;
			long top,bot,mid,diff;
			string line,key;
			diff = 666; 
			line = "";
            bot = searchFile.BaseStream.Seek(0, SeekOrigin.End);
			top = 0;
			mid = (bot-top)/2;

			do 
			{
                searchFile.DiscardBufferedData();
                searchFile.BaseStream.Position = mid - 1;
				if (mid!=1)
                    while ((c = searchFile.Read()) != '\n' && c != -1)
						;
                line = searchFile.ReadLine();
				if (line==null)
					return null;
				n = line.IndexOf(' ');
				key = line.Substring(0,n);
				key=key.Replace("-"," ").Replace("_"," ");
				if (string.CompareOrdinal(key, searchKey)<0) 
				{
					top = mid;
					diff = (bot - top)/2;
					mid = top + diff;
				}
                if (string.CompareOrdinal(key, searchKey) > 0)
				{
					bot = mid;
					diff = (bot - top)/2;
					mid = top + diff;
				}
			} while (key!=searchKey && diff!=0);
			
			if (key==searchKey)
				return line;
			return null;
		}

        private IndexWord CreateIndexWord(string partOfSpeech, string line)
        {
            Tokenizer tokenizer = new Tokenizer(line);
            string word = tokenizer.NextToken().Replace('_', ' ');
            string redundantPartOfSpeech = tokenizer.NextToken();
            int senseCount = int.Parse(tokenizer.NextToken());

            int relationTypeCount = int.Parse(tokenizer.NextToken());
            string[] relationTypes = null;
            if (relationTypeCount > 0)
            {
                relationTypes = new string[relationTypeCount];
                for (int currentRelationType = 0; currentRelationType < relationTypeCount; currentRelationType++)
                {
                    relationTypes[currentRelationType] = tokenizer.NextToken();
                }
            }
            int redundantSenseCount = int.Parse(tokenizer.NextToken());
            int tagSenseCount = int.Parse(tokenizer.NextToken());

            int[] synsetOffsets = null;
            if (senseCount > 0)
            {
                synsetOffsets = new int[senseCount];
                for (int currentOffset = 0; currentOffset < senseCount; currentOffset++)
                {
                    synsetOffsets[currentOffset] = int.Parse(tokenizer.NextToken());
                }
            }
            return new IndexWord(word, partOfSpeech, relationTypes, synsetOffsets, tagSenseCount);
        }

		protected internal override Synset CreateSynset(string partOfSpeech, int synsetOffset)
		{
			StreamReader dataFile = mDataFileDictionary[partOfSpeech].DataFile;
			dataFile.DiscardBufferedData();
			dataFile.BaseStream.Seek(synsetOffset, SeekOrigin.Begin);
			string record = dataFile.ReadLine();
			
			Tokenizer tokenizer = new Tokenizer(record);
			int offset = int.Parse(tokenizer.NextToken());
			string lexicographerFile = mLexicographerFiles[int.Parse(tokenizer.NextToken())];
			string synsetType = tokenizer.NextToken();
			int wordCount = int.Parse(tokenizer.NextToken(), System.Globalization.NumberStyles.HexNumber);
			
			string[] words = new string[wordCount];
			for (int iCurrentWord = 0; iCurrentWord < wordCount; iCurrentWord++) 
			{
				words[iCurrentWord] = tokenizer.NextToken().Replace("_", " ");
				int uniqueID = int.Parse(tokenizer.NextToken(), System.Globalization.NumberStyles.HexNumber);
			}

			int relationCount = int.Parse(tokenizer.NextToken());
			Relation[] relations = new Relation[relationCount];
			for (int currentRelation = 0; currentRelation < relationCount; currentRelation++)
			{
				string relationTypeKey = tokenizer.NextToken();
//				if (fpos.name=="adj" && sstype==AdjSynSetType.DontKnow) 
//				{
//					if (ptrs[j].ptp.mnemonic=="ANTPTR")
//						sstype = AdjSynSetType.DirectAnt;
//					else if (ptrs[j].ptp.mnemonic=="PERTPTR") 
//						sstype = AdjSynSetType.Pertainym;
//				}
				int targetSynsetOffset = int.Parse(tokenizer.NextToken());
				string targetPartOfSpeech = tokenizer.NextToken();
				switch (targetPartOfSpeech)
				{
					case "n":
						targetPartOfSpeech = "noun";
						break;
					case "v":
						targetPartOfSpeech = "verb";
						break;
					case "a":
					case "s":
						targetPartOfSpeech = "adjective";
						break;
					case "r":
						targetPartOfSpeech = "adverb";
						break;
				}

				int sourceTarget = int.Parse(tokenizer.NextToken(), System.Globalization.NumberStyles.HexNumber);
				if (sourceTarget == 0)
				{
					relations[currentRelation] = new Relation(this, (RelationType)mRelationTypeDictionary[relationTypeKey], targetSynsetOffset, targetPartOfSpeech);
				} 
				else
				{
					int sourceWord = sourceTarget >> 8;
					int targetWord = sourceTarget & 0xff;
					relations[currentRelation] = new Relation(this, (RelationType)mRelationTypeDictionary[relationTypeKey], targetSynsetOffset, targetPartOfSpeech, sourceWord, targetWord);
				}
			}
			string frameData = tokenizer.NextToken();
			if (frameData != "|") 
			{
				int frameCount = int.Parse(frameData);
				for (int currentFrame = 0; currentFrame < frameCount; currentFrame++) 
				{
					frameData = tokenizer.NextToken(); // +
					int frameNumber = int.Parse(tokenizer.NextToken());
					int wordID = int.Parse(tokenizer.NextToken(), System.Globalization.NumberStyles.HexNumber);
				}
				frameData = tokenizer.NextToken();
			}
			string gloss = record.Substring(record.IndexOf('|') + 1);

			Synset synset = new Synset(synsetOffset, gloss, words, lexicographerFile, relations);
			return synset;
		}

        protected internal override string[] GetExceptionForms(string lemma, string partOfSpeech)
        {
            string line = BinarySearch(lemma, mDataFileDictionary[partOfSpeech].ExceptionFile);
            if (line != null)
            {
                List<string> exceptionForms = new List<string>();
                Tokenizer tokenizer = new Tokenizer(line);
                string skipWord = tokenizer.NextToken();
                string word = tokenizer.NextToken();
                while (word != null)
                {
                    exceptionForms.Add(word);
                    word = tokenizer.NextToken();
                }
                return exceptionForms.ToArray();
            }
            return mEmpty;
        }

		private void InitializeLexicographerFiles()
		{
			mLexicographerFiles = new string[45];

			mLexicographerFiles[0] = "adj.all - all adjective clusters";  
			mLexicographerFiles[1] = "adj.pert - relational adjectives (pertainyms)";  
			mLexicographerFiles[2] = "adv.all - all adverbs";  
			mLexicographerFiles[3] = "noun.Tops - unique beginners for nouns";  
			mLexicographerFiles[4] = "noun.act - nouns denoting acts or actions";  
			mLexicographerFiles[5] = "noun.animal - nouns denoting animals";  
			mLexicographerFiles[6] = "noun.artifact - nouns denoting man-made objects";  
			mLexicographerFiles[7] = "noun.attribute - nouns denoting attributes of people and objects";  
			mLexicographerFiles[8] = "noun.body - nouns denoting body parts";  
			mLexicographerFiles[9] = "noun.cognition - nouns denoting cognitive processes and contents";  
			mLexicographerFiles[10] = "noun.communication - nouns denoting communicative processes and contents";  
			mLexicographerFiles[11] = "noun.event - nouns denoting natural events";  
			mLexicographerFiles[12] = "noun.feeling - nouns denoting feelings and emotions";  
			mLexicographerFiles[13] = "noun.food - nouns denoting foods and drinks";  
			mLexicographerFiles[14] = "noun.group - nouns denoting groupings of people or objects";  
			mLexicographerFiles[15] = "noun.location - nouns denoting spatial position";  
			mLexicographerFiles[16] = "noun.motive - nouns denoting goals";  
			mLexicographerFiles[17] = "noun.object - nouns denoting natural objects (not man-made)";  
			mLexicographerFiles[18] = "noun.person - nouns denoting people";  
			mLexicographerFiles[19] = "noun.phenomenon - nouns denoting natural phenomena";  
			mLexicographerFiles[20] = "noun.plant - nouns denoting plants";  
			mLexicographerFiles[21] = "noun.possession - nouns denoting possession and transfer of possession";  
			mLexicographerFiles[22] = "noun.process - nouns denoting natural processes";  
			mLexicographerFiles[23] = "noun.quantity - nouns denoting quantities and units of measure";  
			mLexicographerFiles[24] = "noun.relation - nouns denoting relations between people or things or ideas";  
			mLexicographerFiles[25] = "noun.shape - nouns denoting two and three dimensional shapes";  
			mLexicographerFiles[26] = "noun.state - nouns denoting stable states of affairs";  
			mLexicographerFiles[27] = "noun.substance - nouns denoting substances";  
			mLexicographerFiles[28] = "noun.time - nouns denoting time and temporal relations";  
			mLexicographerFiles[29] = "verb.body - verbs of grooming, dressing and bodily care";  
			mLexicographerFiles[30] = "verb.change - verbs of size, temperature change, intensifying, etc.";  
			mLexicographerFiles[31] = "verb.cognition - verbs of thinking, judging, analyzing, doubting";  
			mLexicographerFiles[32] = "verb.communication - verbs of telling, asking, ordering, singing";  
			mLexicographerFiles[33] = "verb.competition - verbs of fighting, athletic activities";  
			mLexicographerFiles[34] = "verb.consumption - verbs of eating and drinking";  
			mLexicographerFiles[35] = "verb.contact - verbs of touching, hitting, tying, digging";  
			mLexicographerFiles[36] = "verb.creation - verbs of sewing, baking, painting, performing";  
			mLexicographerFiles[37] = "verb.emotion - verbs of feeling";  
			mLexicographerFiles[38] = "verb.motion - verbs of walking, flying, swimming";  
			mLexicographerFiles[39] = "verb.perception - verbs of seeing, hearing, feeling";  
			mLexicographerFiles[40] = "verb.possession - verbs of buying, selling, owning";  
			mLexicographerFiles[41] = "verb.social - verbs of political and social activities and events";  
			mLexicographerFiles[42] = "verb.stative - verbs of being, having, spatial relations";  
			mLexicographerFiles[43] = "verb.weather - verbs of raining, snowing, thawing, thundering";  
			mLexicographerFiles[44] = "adj.ppl - participial adjectives";  

		}

		private void InitializeRelationTypes()
		{
            mRelationTypeDictionary = new Dictionary<string, RelationType>(30);
			
			mRelationTypeDictionary.Add("!", new RelationType("Antonym", new string[] {"noun", "verb", "adjective", "adverb"}));
			mRelationTypeDictionary.Add("@", new RelationType("Hypernym", new string[] {"noun", "verb"})); 
			mRelationTypeDictionary.Add("@i", new RelationType("Instance Hypernym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("~", new RelationType("Hyponym", new string[] {"noun", "verb"})); 
			mRelationTypeDictionary.Add("~i", new RelationType("Instance Hyponym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("#m", new RelationType("Member holonym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("#s", new RelationType("Substance holonym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("#p", new RelationType("Part holonym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("%m", new RelationType("Member meronym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("%s", new RelationType("Substance meronym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("%p", new RelationType("Part meronym", new string[] {"noun"})); 
			mRelationTypeDictionary.Add("=", new RelationType("Attribute", new string[] {"noun", "adjective"})); 
			mRelationTypeDictionary.Add("+", new RelationType("Derivationally related form", new string[] {"noun", "verb"}));         
			mRelationTypeDictionary.Add(";c", new RelationType("Domain of synset - TOPIC", new string[] {"noun", "verb", "adjective", "adverb"})); 
			mRelationTypeDictionary.Add("-c", new RelationType("Member of this domain - TOPIC", new string[] {"noun"})); 
			mRelationTypeDictionary.Add(";r", new RelationType("Domain of synset - REGION", new string[] {"noun", "verb", "adjective", "adverb"})); 
			mRelationTypeDictionary.Add("-r", new RelationType("Member of this domain - REGION", new string[] {"noun"})); 
			mRelationTypeDictionary.Add(";u", new RelationType("Domain of synset - USAGE", new string[] {"noun", "verb", "adjective", "adverb"})); 
			mRelationTypeDictionary.Add("-u", new RelationType("Member of this domain - USAGE", new string[] {"noun"})); 
			
			mRelationTypeDictionary.Add("*", new RelationType("Entailment", new string[] {"verb"}));  
			mRelationTypeDictionary.Add(">", new RelationType("Cause", new string[] {"verb"}));  
			mRelationTypeDictionary.Add("^", new RelationType("Also see", new string[] {"verb", "adjective"}));  
			mRelationTypeDictionary.Add("$", new RelationType("Verb Group", new string[] {"verb"}));
		
			mRelationTypeDictionary.Add("&", new RelationType("Similar to", new string[] {"adjective"})); 
			mRelationTypeDictionary.Add("<", new RelationType("Participle of verb", new string[] {"adjective"})); 
			mRelationTypeDictionary.Add(@"\", new RelationType("Pertainym", new string[] {"adjective", "adverb"})); 
			
			//moRelationTypeDictionary.Add(";", new RelationType("Domain of synset", new string[] {"noun", "verb", "adjective", "adverb"}));
			//moRelationTypeDictionary.Add("-", new RelationType("Member of this domain", new string[] {"noun"})); 

		}

        private class PosDataFileSet
        {
            private StreamReader mIndexFile;
            private StreamReader mDataFile;
            private StreamReader mExceptionFile;

            public StreamReader IndexFile
            {
                get
                {
                    return mIndexFile;
                }
            }

            public StreamReader DataFile
            {
                get
                {
                    return mDataFile;
                }
            }

            public StreamReader ExceptionFile
            {
                get
                {
                    return mExceptionFile;
                }
            }

            public PosDataFileSet(string dataFolder, string partOfSpeech)
            {
                mIndexFile = new StreamReader(dataFolder + "\\index." + partOfSpeech);
                mDataFile = new StreamReader(dataFolder + "\\data." + partOfSpeech);
                mExceptionFile = new StreamReader(dataFolder + "\\" + partOfSpeech + ".exc");
            }
        }

#endregion

	}
}
