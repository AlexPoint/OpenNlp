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

//This file is based on the EnglishTreebankParser.java source file found in the
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
using System.Collections.Generic;

namespace OpenNLP.Tools.Parser
{
	/// <summary>
	/// Class that wraps the MaximumEntropyParser to make it easy to perform full parses using the English Treebank
	/// based maximum entropy models.
	/// </summary>
	public sealed class EnglishTreebankParser
	{
		MaximumEntropyParser mParser;
		OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer mTokenizer;

		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary, int beamSize, double advancePercentage)
		{
			SharpEntropy.IO.BinaryGisModelReader buildModelReader = new SharpEntropy.IO.BinaryGisModelReader(dataDirectory + "parser\\build.nbin");
			SharpEntropy.GisModel buildModel = new SharpEntropy.GisModel(buildModelReader);

			SharpEntropy.IO.BinaryGisModelReader checkModelReader = new SharpEntropy.IO.BinaryGisModelReader(dataDirectory + "parser\\check.nbin");
			SharpEntropy.IMaximumEntropyModel checkModel = new SharpEntropy.GisModel(checkModelReader);

			EnglishTreebankPosTagger posTagger;

			if (useTagDictionary)
			{
				posTagger = new EnglishTreebankPosTagger(dataDirectory + "parser\\tag.nbin", dataDirectory + "parser\\tagdict", useCaseSensitiveTagDictionary);
			}
			else
			{
				posTagger = new EnglishTreebankPosTagger(dataDirectory + "parser\\tag.nbin");
			}

			EnglishTreebankParserChunker chunker = new EnglishTreebankParserChunker(dataDirectory + "parser\\chunk.nbin");
			EnglishHeadRules headRules = new EnglishHeadRules(dataDirectory + "parser\\head_rules");

			mParser = new MaximumEntropyParser(buildModel, checkModel, posTagger, chunker, headRules, beamSize, advancePercentage);
		
			mTokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(dataDirectory + "EnglishTok.nbin");

		}
		
		public EnglishTreebankParser(string dataDirectory) : this(dataDirectory, true, false, MaximumEntropyParser.DefaultBeamSize, MaximumEntropyParser.DefaultAdvancePercentage)
		{
		}
  
		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary) : this(dataDirectory, useTagDictionary, useCaseSensitiveTagDictionary, MaximumEntropyParser.DefaultBeamSize, MaximumEntropyParser.DefaultAdvancePercentage)
		{
		}

		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary, int beamSize) : this(dataDirectory, useTagDictionary, useCaseSensitiveTagDictionary, beamSize, MaximumEntropyParser.DefaultAdvancePercentage)
		{
		}

		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary, double advancePercentage) : this(dataDirectory, useTagDictionary, useCaseSensitiveTagDictionary, MaximumEntropyParser.DefaultBeamSize, advancePercentage)
		{
		}

		private class EnglishTreebankPosTagger : PosTagger.MaximumEntropyPosTagger, IParserTagger
		{
			private const int K = 10;
			private int mBeamSize;

			public EnglishTreebankPosTagger(string modelFile) : this(modelFile, K, K)
			{
			}
			
			public EnglishTreebankPosTagger(string modelFile, int beamSize, int cacheSize) : base(beamSize, new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)), new PosTagger.DefaultPosContextGenerator(cacheSize), null)
			{
				mBeamSize = beamSize;
			}

			public EnglishTreebankPosTagger(string modelFile, string tagDictionary, bool useCase): this(modelFile, K, tagDictionary, useCase, K)
			{
			}
			
			public EnglishTreebankPosTagger(string modelFile, int beamSize, string tagDictionary, bool useCase, int cacheSize) : base(beamSize, new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)), new PosTagger.DefaultPosContextGenerator(cacheSize), new PosTagger.PosLookupList(tagDictionary, useCase))
			{
				mBeamSize = beamSize;
			}

			public virtual Util.Sequence[] TopKSequences(ArrayList sentence)
			{
				return Beam.BestSequences(mBeamSize, sentence.ToArray(), null);
			}
			
			public virtual Util.Sequence[] TopKSequences(string[] sentence)
			{
				 return Beam.BestSequences(mBeamSize, sentence, null);
			}
		}
		
		private class EnglishTreebankParserChunker : Chunker.MaximumEntropyChunker, IParserChunker
		{
			private const int K = 10;
			private int mBeamSize;
			private Dictionary<string, string> mContinueStartMap;
    
			public EnglishTreebankParserChunker(string modelFile) : this(modelFile, K, K)
			{
			}
			
			public EnglishTreebankParserChunker(string modelFile, int beamSize, int cacheSize) : base(new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)), new ChunkContextGenerator(cacheSize), beamSize)
			{
                mContinueStartMap = new Dictionary<string, string>(Model.OutcomeCount);
				for (int currentOutcome = 0, outcomeCount = Model.OutcomeCount; currentOutcome < outcomeCount; currentOutcome++) 
				{
					string outcome = Model.GetOutcomeName(currentOutcome);
					if (outcome.StartsWith(MaximumEntropyParser.ContinuePrefix))
					{
						mContinueStartMap.Add(outcome, MaximumEntropyParser.StartPrefix + outcome.Substring(MaximumEntropyParser.ContinuePrefix.Length));
					}
				}
				mBeamSize = beamSize;
			}

			public virtual Util.Sequence[] TopKSequences(ArrayList sentence, ArrayList tags)
			{
				return Beam.BestSequences(mBeamSize, sentence.ToArray(), new object[]{tags});
			}
			
			public virtual Util.Sequence[] TopKSequences(string[] sentence, string[] tags, double minSequenceScore) 
			{
				return Beam.BestSequences(mBeamSize, sentence, new object[] {tags}, minSequenceScore);
			}
			
			protected internal override bool ValidOutcome(string outcome, string[] tagList) 
			{
				if (mContinueStartMap.ContainsKey(outcome)) 
				{
					int lastTagIndex = tagList.Length - 1;
					if (lastTagIndex == -1) 
					{
						return (false);
					}
					else 
					{
						string lastTag = tagList[lastTagIndex];
						if (lastTag == outcome) 
						{
							return true;
						}
						if (lastTag == mContinueStartMap[outcome]) 
						{
							return true;
						}
						if (lastTag == MaximumEntropyParser.OtherOutcome)
						{
							return false;
						}
						return false;
					}
				}
				return true;
			}

			protected internal override bool ValidOutcome(string outcome, Util.Sequence sequence)
			{
				if (mContinueStartMap.ContainsKey(outcome))
				{
					string[] tags = sequence.Outcomes.ToArray();
					int lastTagIndex = tags.Length - 1;
					if (lastTagIndex == - 1)
					{
						return false;
					}
					else
					{
						string lastTag = tags[lastTagIndex];
						if (lastTag == outcome) 
						{
							return true;
						}
						if (lastTag == mContinueStartMap[outcome]) 
						{
							return true;
						}
						if (lastTag == MaximumEntropyParser.OtherOutcome)
						{
							return false;
						}
						return false;
					}
				}
				return true;
			}
		}	
		
		
		private string ConvertToken(string token)
		{
			switch (token)
			{
				case "(":
					return "-LRB-";
				case ")":
					return "-RRB-";
				case "{":
					return "-LCB-";
				case "}":
					return "-RCB-";
				default:
					return token;
			}
		}
		
		public string DoParse(string[] lines, int requestedParses)
		{
						
			System.Text.StringBuilder parseStringBuilder = new System.Text.StringBuilder();

			foreach (string line in lines)
			{
				System.Text.StringBuilder lineBuilder = new System.Text.StringBuilder();				
			
				string[] rawTokens = mTokenizer.Tokenize(line);
				ArrayList tokens = new ArrayList();
				foreach (string rawToken in rawTokens)
				{
					string convertedToken = ConvertToken(rawToken);
					tokens.Add(convertedToken);
					lineBuilder.Append(convertedToken).Append(" ");
				}
				if (lineBuilder.Length != 0)
				{
					string text = lineBuilder.ToString(0, lineBuilder.Length - 1).ToString();
					Parse currentParse = new Parse(text, new Util.Span(0, text.Length), "INC", 1, null);
					int start = 0;
					
					foreach (string token in tokens)
					{
						currentParse.Insert(new Parse(text, new Util.Span(start, start + token.Length), MaximumEntropyParser.TokenNode, 0));
						start += token.Length + 1;
					}
					
					Parse[] parses = mParser.FullParse(currentParse, requestedParses);
					for (int currentParseIndex = 0, parseCount = parses.Length; currentParseIndex < parseCount; currentParseIndex++)
					{
						if (requestedParses > 1)
						{
						lineBuilder.Append(currentParse.ToString() + " " + parses[currentParseIndex].Probability.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ");
						}
						lineBuilder.Append(parses[currentParseIndex].Show());
						parseStringBuilder.Append(lineBuilder.ToString());
					}
				}
				else
				{
					parseStringBuilder.Append("\r\n");
				}
			}
			return parseStringBuilder.ToString();
		}

		public Parse[] DoParse(string line, int requestedParses)
		{			
			System.Text.StringBuilder lineBuilder = new System.Text.StringBuilder();				
			string[] rawTokens = mTokenizer.Tokenize(line);
			ArrayList tokens = new ArrayList();
			foreach (string rawToken in rawTokens)
			{
				string convertedToken = ConvertToken(rawToken);
				tokens.Add(convertedToken);
				lineBuilder.Append(convertedToken).Append(" ");
			}
			if (lineBuilder.Length != 0)
			{
				string text = lineBuilder.ToString(0, lineBuilder.Length - 1).ToString();
				Parse currentParse = new Parse(text, new Util.Span(0, text.Length), "INC", 1, null);
				int start = 0;
				
				foreach (string token in tokens)
				{
					currentParse.Insert(new Parse(text, new Util.Span(start, start + token.Length), MaximumEntropyParser.TokenNode, 0));
					start += token.Length + 1;
				}
				
				Parse[] parses = mParser.FullParse(currentParse, requestedParses);
				return parses;
			}
			else
			{
				return null;
			}
		}

		public Parse DoParse(string line)
		{
			Parse[] parses = DoParse(line, 1);
			if (parses != null)
			{
				return parses[0];
			}
			return null;
		}
	}
}
