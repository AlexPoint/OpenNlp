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
	    readonly MaximumEntropyParser _parser;
	    readonly Tokenize.EnglishMaximumEntropyTokenizer _tokenizer;

        // Constructors ---------------------

		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary, int beamSize, double advancePercentage)
		{
			var buildModelReader = new SharpEntropy.IO.BinaryGisModelReader(dataDirectory + "parser\\build.nbin");
			var buildModel = new SharpEntropy.GisModel(buildModelReader);

			var checkModelReader = new SharpEntropy.IO.BinaryGisModelReader(dataDirectory + "parser\\check.nbin");
			SharpEntropy.IMaximumEntropyModel checkModel = new SharpEntropy.GisModel(checkModelReader);

		    EnglishTreebankPosTagger posTagger = useTagDictionary ? 
                new EnglishTreebankPosTagger(dataDirectory + "parser\\tag.nbin", dataDirectory + "parser\\tagdict", useCaseSensitiveTagDictionary) 
		        : new EnglishTreebankPosTagger(dataDirectory + "parser\\tag.nbin");

			var chunker = new EnglishTreebankParserChunker(dataDirectory + "parser\\chunk.nbin");
			var headRules = new EnglishHeadRules(dataDirectory + "parser\\head_rules");

			_parser = new MaximumEntropyParser(buildModel, checkModel, posTagger, chunker, headRules, beamSize, advancePercentage);
		
			_tokenizer = new Tokenize.EnglishMaximumEntropyTokenizer(dataDirectory + "EnglishTok.nbin");

		}
		
		public EnglishTreebankParser(string dataDirectory):
            this(dataDirectory, true, false, MaximumEntropyParser.DefaultBeamSize, MaximumEntropyParser.DefaultAdvancePercentage){}
  
		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary):
            this(dataDirectory, useTagDictionary, useCaseSensitiveTagDictionary, MaximumEntropyParser.DefaultBeamSize, MaximumEntropyParser.DefaultAdvancePercentage){}

		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary, int beamSize):
            this(dataDirectory, useTagDictionary, useCaseSensitiveTagDictionary, beamSize, MaximumEntropyParser.DefaultAdvancePercentage){}

		public EnglishTreebankParser(string dataDirectory, bool useTagDictionary, bool useCaseSensitiveTagDictionary, double advancePercentage):
            this(dataDirectory, useTagDictionary, useCaseSensitiveTagDictionary, MaximumEntropyParser.DefaultBeamSize, advancePercentage){}


        // Methods ----------------------

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
		
        private Parse[] DoParse(IEnumerable<string> tokens, int requestedParses)
	    {
            var lineBuilder = new System.Text.StringBuilder();
            var convertedTokens = new List<string>();
            foreach (string rawToken in tokens)
            {
                string convertedToken = ConvertToken(rawToken);
                convertedTokens.Add(convertedToken);
                lineBuilder.Append(convertedToken).Append(" ");
            }
            if (lineBuilder.Length != 0)
            {
                string text = lineBuilder.ToString(0, lineBuilder.Length - 1);
                var currentParse = new Parse(text, new Util.Span(0, text.Length), "INC", 1, null);
                int start = 0;

                foreach (string token in convertedTokens)
                {
                    currentParse.Insert(new Parse(text, new Util.Span(start, start + token.Length), MaximumEntropyParser.TokenNode, 0));
                    start += token.Length + 1;
                }

                Parse[] parses = _parser.FullParse(currentParse, requestedParses);
                return parses;
            }
            else
            {
                return null;
            }
	    }

        /// <summary>
        /// Builds the syntax tree (or parse tree) of a sentence.
        /// </summary>
        /// <param name="sentence">The sentence</param>
        /// <returns>The syntax tree</returns>
		public Parse DoParse(string sentence)
		{
		    var tokens = _tokenizer.Tokenize(sentence);
		    return DoParse(tokens);
		}

        /// <summary>
        /// Builds the syntax Tree (or parse tree) of a tokenized sentence.
        /// </summary>
        /// <param name="tokens">The collection of tokens for a sentence</param>
        /// <returns>The sytax tree</returns>
	    public Parse DoParse(IEnumerable<string> tokens)
	    {
            Parse[] parses = DoParse(tokens, 1);
            if (parses != null)
            {
                return parses[0];
            }
            return null;
	    }


        // Inner classes ------------------------------
        private sealed class EnglishTreebankPosTagger : PosTagger.MaximumEntropyPosTagger, IParserTagger
        {
            private const int K = 10;
            private readonly int _beamSize;

            // Constructors ---------------------------

            public EnglishTreebankPosTagger(string modelFile, int beamSize = K, int cacheSize = K)
                : base(beamSize, new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)), new PosTagger.DefaultPosContextGenerator(cacheSize), null)
            {
                _beamSize = beamSize;
            }

            public EnglishTreebankPosTagger(string modelFile, string tagDictionary, bool useCase)
                : this(modelFile, K, tagDictionary, useCase, K)
            {
            }

            public EnglishTreebankPosTagger(string modelFile, int beamSize, string tagDictionary, bool useCase, int cacheSize)
                : base(beamSize, new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)), new PosTagger.DefaultPosContextGenerator(cacheSize), new PosTagger.PosLookupList(tagDictionary, useCase))
            {
                _beamSize = beamSize;
            }

            
            // Methods ---------------------------------

            public Util.Sequence[] TopKSequences(string[] sentence)
            {
                return Beam.BestSequences(_beamSize, sentence, null);
            }
        }

        private sealed class EnglishTreebankParserChunker : Chunker.MaximumEntropyChunker, IParserChunker
        {
            private const int K = 10;
            private readonly int _beamSize;
            private readonly Dictionary<string, string> _continueStartMap;


            // Constructors ------------------------

            public EnglishTreebankParserChunker(string modelFile, int beamSize = K, int cacheSize = K) :
                base(new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelFile)), new ChunkContextGenerator(cacheSize), beamSize)
            {
                _continueStartMap = new Dictionary<string, string>(Model.OutcomeCount);
                for (int currentOutcome = 0, outcomeCount = Model.OutcomeCount; currentOutcome < outcomeCount; currentOutcome++)
                {
                    string outcome = Model.GetOutcomeName(currentOutcome);
                    if (outcome.StartsWith(MaximumEntropyParser.ContinuePrefix))
                    {
                        _continueStartMap.Add(outcome, MaximumEntropyParser.StartPrefix + outcome.Substring(MaximumEntropyParser.ContinuePrefix.Length));
                    }
                }
                _beamSize = beamSize;
            }


            // Methods -----------------------------

            public Util.Sequence[] TopKSequences(string[] sentence, string[] tags, double minSequenceScore)
            {
                return Beam.BestSequences(_beamSize, sentence, new object[] { tags }, minSequenceScore);
            }

            protected internal override bool ValidOutcome(string outcome, string[] tagList)
            {
                if (_continueStartMap.ContainsKey(outcome))
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
                        if (lastTag == _continueStartMap[outcome])
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
                if (_continueStartMap.ContainsKey(outcome))
                {
                    string[] tags = sequence.Outcomes.ToArray();
                    int lastTagIndex = tags.Length - 1;
                    if (lastTagIndex == -1)
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
                        if (lastTag == _continueStartMap[outcome])
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
	}
}
