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

//This file is based on the ParserME.java source file found in the
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
using System.IO;
using System.Linq;
using System.Text;

namespace OpenNLP.Tools.Parser
{
	/// <summary>
	/// Class for a shift reduce style parser based on Adwait Ratnaparki's 1998 thesis.
	/// </summary>
	public class MaximumEntropyParser
	{
		/// <summary>
		/// The maximum number of parses advanced from all preceding parses at each derivation step.
		/// </summary>
		private readonly int m;

		///<summary>
		///The maximum number of parses to advance from a single preceding parse.
		///</summary>
		private readonly int k;

		///<summary>
		///The minimum total probability mass of advanced outcomes.
		///</summary>
		private readonly double q;

		///<summary>
		///The default beam size used if no beam size is given.
		///</summary>
		public const int DefaultBeamSize = 20;

		///<summary>
		///The default amount of probability mass required of advanced outcomes.
		///</summary>
		public const double DefaultAdvancePercentage = 0.95;
		
		private readonly IParserTagger posTagger; 
		private readonly IParserChunker basalChunker; 
		
		private readonly SharpEntropy.IMaximumEntropyModel buildModel;
		private readonly SharpEntropy.IMaximumEntropyModel checkModel;
		
		private readonly BuildContextGenerator buildContextGenerator;
		private readonly CheckContextGenerator checkContextGenerator;
		
		private readonly IHeadRules headRules;
		
		public const string TopNode = "TOP";
		public const string TokenNode = "TK";
		
		public const int Zero = 0;
		
		/// <summary>
		/// Prefix for outcomes starting a constituent.
		/// </summary>
		public const string StartPrefix = "S-";

		/// <summary>
		/// Prefix for outcomes continuing a constituent.
		/// </summary>
		public const string ContinuePrefix = "C-";

		/// <summary>
		/// Outcome for token which is not contained in a basal constituent.
		/// </summary>
		public const string OtherOutcome = "O";
		
		/// <summary>
		/// Outcome used when a constituent is complete.
		/// </summary>
		public const string CompleteOutcome = "c";

		/// <summary>
		/// Outcome used when a constituent is incomplete.
		/// </summary>
		public const string IncompleteOutcome = "i";
		
		private const string MTopStart = StartPrefix + TopNode;

		private readonly int topStartIndex;
		private readonly Dictionary<string, string> startTypeMap;
        private readonly Dictionary<string, string> continueTypeMap;
        private readonly int completeIndex;
		private readonly int incompleteIndex;

	    private const bool CreateDerivationString = false;


        // Constructors -------------------------

	    ///<summary>
		///Creates a new parser using the specified models and head rules.
		///</summary>
		///<param name="buildModel">
		///The model to assign constituent labels.
		///</param>
		///<param name="checkModel">
		///The model to determine a constituent is complete.
		///</param>
		///<param name="tagger">
		///The model to assign pos-tags.
		///</param>
		///<param name="chunker">
		///The model to assign flat constituent labels.
		///</param>
		///<param name="headRules">
		///The head rules for head word perculation.
		///</param>
		public MaximumEntropyParser(SharpEntropy.IMaximumEntropyModel buildModel, SharpEntropy.IMaximumEntropyModel checkModel, IParserTagger tagger, IParserChunker chunker, IHeadRules headRules) : this(buildModel, checkModel, tagger, chunker, headRules, DefaultBeamSize, DefaultAdvancePercentage)
		{}

		///<summary>
		///Creates a new parser using the specified models and head rules using the specified beam size and advance percentage.
		///</summary>
		///<param name="buildModel">
		///The model to assign constituent labels.
		///</param>
		///<param name="checkModel">
		///The model to determine a constituent is complete.
		///</param>
		///<param name="tagger">
		///The model to assign pos-tags.
		///</param>
		///<param name="chunker">
		///The model to assign flat constituent labels.
		///</param>
		///<param name="headRules">
		///The head rules for head word perculation.
		///</param>
		///<param name="beamSize">
		///The number of different parses kept during parsing.
		///</param>
		///<param name="advancePercentage">
		///The minimal amount of probability mass which advanced outcomes must represent.
		///Only outcomes which contribute to the top "advancePercentage" will be explored.
		///</param>    
		public MaximumEntropyParser(SharpEntropy.IMaximumEntropyModel buildModel, SharpEntropy.IMaximumEntropyModel checkModel, IParserTagger tagger, IParserChunker chunker, IHeadRules headRules, int beamSize, double advancePercentage) 
		{
			posTagger = tagger;
			basalChunker = chunker;
			this.buildModel = buildModel;
			this.checkModel = checkModel;
			m = beamSize;
			k = beamSize;
			q = advancePercentage;

			buildContextGenerator = new BuildContextGenerator();
			checkContextGenerator = new CheckContextGenerator();
			this.headRules = headRules;
			
			startTypeMap = new Dictionary<string, string>();
            continueTypeMap = new Dictionary<string, string>();
			for (int buildOutcomeIndex = 0, buildOutcomeCount = buildModel.OutcomeCount; buildOutcomeIndex < buildOutcomeCount; buildOutcomeIndex++) 
			{
				string outcome = buildModel.GetOutcomeName(buildOutcomeIndex);
				if (outcome.StartsWith(StartPrefix)) 
				{
					//System.Console.Error.WriteLine("startMap " + outcome + "->" + outcome.Substring(StartPrefix.Length));
					startTypeMap.Add(outcome, outcome.Substring(StartPrefix.Length));
				}
				else if (outcome.StartsWith(ContinuePrefix)) 
				{
					//System.Console.Error.WriteLine("contMap " + outcome + "->" + outcome.Substring(ContinuePrefix.Length));
					continueTypeMap.Add(outcome, outcome.Substring(ContinuePrefix.Length));
				}
			}
			topStartIndex = buildModel.GetOutcomeIndex(MTopStart);
			completeIndex = checkModel.GetOutcomeIndex(CompleteOutcome);
			incompleteIndex = checkModel.GetOutcomeIndex(IncompleteOutcome);
		}
		

        // Methods ------------------------------

		/// <summary>
		/// Returns a parse for the specified parse of tokens.
		/// </summary>
		/// <param name="flatParse">
		/// A flat parse containing only tokens and a root node, p. 
		/// </param>
		/// <param name="parseCount">
		/// the number of parses required
		/// </param>
		/// <returns>
		/// A full parse of the specified tokens or the flat chunks of the tokens if a full parse could not be found.
		/// </returns>
		public virtual Parse[] FullParse(Parse flatParse, int parseCount)
		{
			if (CreateDerivationString) 
			{
				flatParse.InitializeDerivationBuffer();
			}

            var oldDerivationsHeap = new Util.SortedSet<Parse>();
            var parses = new Util.SortedSet<Parse>();

			int derivationLength = 0; 
			int maxDerivationLength = 2 * flatParse.ChildCount + 3;
			oldDerivationsHeap.Add(flatParse);
			Parse guessParse = null;
			double bestComplete = - 100000; //approximating -infinity/0 in ln domain
            
            var buildProbabilities = new double[this.buildModel.OutcomeCount];
            var checkProbabilities = new double[this.checkModel.OutcomeCount];

			while (parses.Count < m && derivationLength < maxDerivationLength)
			{
                var newDerivationsHeap = new Util.TreeSet<Parse>();
				if (oldDerivationsHeap.Count > 0)
				{
					int derivationsProcessed = 0;

					foreach (Parse currentParse in oldDerivationsHeap)
					{
						derivationsProcessed++;
						if (derivationsProcessed >= k) 
						{
							break;
						}

						// for each derivation
						//Parse currentParse = (Parse) pi.Current;
						if (currentParse.Probability < bestComplete)  //this parse and the ones which follow will never win, stop advancing.
						{
							break;
						}
						if (guessParse == null && derivationLength == 2)
						{
							guessParse = currentParse;
						}

						Parse[] newDerivations = null;
						if (0 == derivationLength) 
						{
							newDerivations = AdvanceTags(currentParse);
						}
						else if (derivationLength == 1)
						{
							if (newDerivationsHeap.Count < k) 
							{
								newDerivations = AdvanceChunks(currentParse, bestComplete);
							}
							else 
							{
								newDerivations = AdvanceChunks(currentParse, newDerivationsHeap.Last().Probability);
							}
						}
						else 
						{ // derivationLength > 1
							newDerivations = AdvanceParses(currentParse, q, buildProbabilities, checkProbabilities);
						}

						if (newDerivations != null)
						{
							for (int currentDerivation = 0, derivationCount = newDerivations.Length; currentDerivation < derivationCount; currentDerivation++)
							{
								if (newDerivations[currentDerivation].IsComplete)
								{
									AdvanceTop(newDerivations[currentDerivation], buildProbabilities, checkProbabilities);
									if (newDerivations[currentDerivation].Probability > bestComplete)
									{
										bestComplete = newDerivations[currentDerivation].Probability;
									}
									parses.Add(newDerivations[currentDerivation]);
									
								}
								else
								{
									newDerivationsHeap.Add(newDerivations[currentDerivation]);
								}
							}
							//RN added sort
							newDerivationsHeap.Sort();
						}
						else
						{
							//Console.Error.WriteLine("Couldn't advance parse " + derivationLength + " stage " + derivationsProcessed + "!\n");
						}
					}
					derivationLength++;
					oldDerivationsHeap = newDerivationsHeap;
				}
				else
				{
					break;
				}
			}
		
			//RN added sort
			parses.Sort();
			
			if (parses.Count == 0)
			{
				//Console.Error.WriteLine("Couldn't find parse for: " + flatParse);
				//oFullParse = (Parse) mOldDerivationsHeap.First(); 
				return new Parse[] {guessParse};
			}
			else if (parseCount == 1)
			{
				//RN added parent adjustment
				Parse topParse = parses.First();
				topParse.UpdateChildParents();
				return new Parse[] {topParse};
			}
			else
			{
                var topParses = new List<Parse>(parseCount);
				while(!parses.IsEmpty() && topParses.Count < parseCount) 
				{
					Parse topParse = parses.First();
					//RN added parent adjustment
					topParse.UpdateChildParents();
					topParses.Add(topParse);
					parses.Remove(topParse);
				}
				return topParses.ToArray();
			}
		}
		
		private void AdvanceTop(Parse inputParse, double[] buildProbabilities, double[] checkProbabilities)
		{
			buildModel.Evaluate(buildContextGenerator.GetContext(inputParse.GetChildren(), 0), buildProbabilities);
			inputParse.AddProbability(Math.Log(buildProbabilities[topStartIndex]));
			checkModel.Evaluate(checkContextGenerator.GetContext(inputParse.GetChildren(), TopNode, 0, 0), checkProbabilities);
			inputParse.AddProbability(Math.Log(checkProbabilities[completeIndex]));
			inputParse.Type = TopNode;
		}
		
		///<summary>
		///Advances the specified parse and returns the an array advanced parses whose probability accounts for
		///more than the speicficed amount of probability mass, Q.
		///</summary>
		///<param name="inputParse">
		///The parse to advance.
		///</param>
		///<param name="qParam">
		///The amount of probability mass that should be accounted for by the advanced parses.
		///</param> 
		private Parse[] AdvanceParses(Parse inputParse, double qParam, double[] buildProbabilities, double[] checkProbabilities) 
		{
			double qOpp = 1 - qParam;
			Parse lastStartNode = null;		// The closest previous node which has been labeled as a start node.
			int lastStartIndex = -1;			// The index of the closest previous node which has been labeled as a start node. 
			string lastStartType = null;	// The type of the closest previous node which has been labeled as a start node.
			int advanceNodeIndex;			// The index of the node which will be labeled in this iteration of advancing the parse.
			Parse advanceNode = null;		// The node which will be labeled in this iteration of advancing the parse.
            
			Parse[] children = inputParse.GetChildren();
			int nodeCount = children.Length;

			//determines which node needs to be labeled and prior labels.
			for (advanceNodeIndex = 0; advanceNodeIndex < nodeCount; advanceNodeIndex++) 
			{
				advanceNode = children[advanceNodeIndex];
				if (advanceNode.Label == null) 
				{
					break;
				}
				else if (startTypeMap.ContainsKey(advanceNode.Label)) 
				{
					lastStartType = startTypeMap[advanceNode.Label];
					lastStartNode = advanceNode;
					lastStartIndex = advanceNodeIndex;
				}
			}
            var newParsesList = new List<Parse>(buildModel.OutcomeCount);
			//call build
			buildModel.Evaluate(buildContextGenerator.GetContext(children, advanceNodeIndex), buildProbabilities);
			double buildProbabilitiesSum = 0;
			while (buildProbabilitiesSum < qParam) 
			{
				//  The largest unadvanced labeling.
				int highestBuildProbabilityIndex = 0;
				for (int probabilityIndex = 1; probabilityIndex < buildProbabilities.Length; probabilityIndex++) 
				{ //for each build outcome
					if (buildProbabilities[probabilityIndex] > buildProbabilities[highestBuildProbabilityIndex]) 
					{
						highestBuildProbabilityIndex = probabilityIndex;
					}
				}
				if (buildProbabilities[highestBuildProbabilityIndex] == 0) 
				{
					break;
				}

				double highestBuildProbability = buildProbabilities[highestBuildProbabilityIndex];		

				buildProbabilities[highestBuildProbabilityIndex] = 0; //zero out so new max can be found
				buildProbabilitiesSum += highestBuildProbability;

				string tag = buildModel.GetOutcomeName(highestBuildProbabilityIndex);
				//System.Console.Out.WriteLine("trying " + tag + " " + buildProbabilitiesSum + " lst=" + lst);
				if (highestBuildProbabilityIndex == topStartIndex) 
				{ // can't have top until complete
					continue;
				}
				//System.Console.Error.WriteLine(probabilityIndex + " " + tag + " " + highestBuildProbability);
				if (startTypeMap.ContainsKey(tag)) 
				{ //update last start
					lastStartIndex = advanceNodeIndex;
					lastStartNode = advanceNode;
					lastStartType = startTypeMap[tag];
				}
				else if (continueTypeMap.ContainsKey(tag)) 
				{
					if (lastStartNode == null || lastStartType != continueTypeMap[tag]) 
					{
						continue; //Cont must match previous start or continue
					}
				}
				var newParse1 = (Parse) inputParse.Clone(); //clone parse
				if (CreateDerivationString)
				{
					newParse1.AppendDerivationBuffer(highestBuildProbabilityIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
					newParse1.AppendDerivationBuffer("-");
				}
				newParse1.SetChild(advanceNodeIndex, tag); //replace constituent labeled

				newParse1.AddProbability(Math.Log(highestBuildProbability));
				//check
				checkModel.Evaluate(checkContextGenerator.GetContext(newParse1.GetChildren(), lastStartType, lastStartIndex, advanceNodeIndex), checkProbabilities);
				//System.Console.Out.WriteLine("check " + mCheckProbabilities[mCompleteIndex] + " " + mCheckProbabilities[mIncompleteIndex]);
				Parse newParse2 = newParse1;
				if (checkProbabilities[completeIndex] > qOpp) 
				{ //make sure a reduce is likely
					newParse2 = (Parse) newParse1.Clone();
					if (CreateDerivationString)
					{
						newParse2.AppendDerivationBuffer("1");
						newParse2.AppendDerivationBuffer(".");
					}
					newParse2.AddProbability(System.Math.Log(checkProbabilities[1]));
					var constituent = new Parse[advanceNodeIndex - lastStartIndex + 1];
					bool isFlat = true;
					//first
					constituent[0] = lastStartNode;
					if (constituent[0].Type != constituent[0].Head.Type)
					{
						isFlat = false;
					}
					//last
					constituent[advanceNodeIndex - lastStartIndex] = advanceNode;
					if (isFlat && constituent[advanceNodeIndex - lastStartIndex].Type != constituent[advanceNodeIndex - lastStartIndex].Head.Type) 
					{
						isFlat = false;
					}
					//middle
					for (int constituentIndex = 1; constituentIndex < advanceNodeIndex - lastStartIndex; constituentIndex++) 
					{
						constituent[constituentIndex] = children[constituentIndex + lastStartIndex];
						if (isFlat && constituent[constituentIndex].Type != constituent[constituentIndex].Head.Type) 
						{
							isFlat = false;
						}
					}
					if (!isFlat) 
					{ //flat chunks are done by chunker
						newParse2.Insert(new Parse(inputParse.Text, new Util.Span(lastStartNode.Span.Start, advanceNode.Span.End), lastStartType, checkProbabilities[1], headRules.GetHead(constituent, lastStartType)));
						newParsesList.Add(newParse2);
					}
				}
				if (checkProbabilities[incompleteIndex] > qOpp) 
				{ //make sure a shift is likely
					if (CreateDerivationString)
					{
						newParse1.AppendDerivationBuffer("0");
						newParse1.AppendDerivationBuffer(".");
					}
					if (advanceNodeIndex != nodeCount - 1) 
					{ //can't shift last element
						newParse1.AddProbability(Math.Log(checkProbabilities[0]));
						newParsesList.Add(newParse1);
					}
				}
			}
			Parse[] newParses = newParsesList.ToArray();
			return newParses;
		}

		///<summary>
		///Returns the top chunk sequences for the specified parse.
		///</summary>
		///<param name="inputParse">
		///A pos-tag assigned parse.
		///</param>
		/// <param name="minChunkScore">
		/// the minimum probability for an allowed chunk sequence.
		/// </param>
		///<returns>
		///The top chunk assignments to the specified parse.
		///</returns>
		private Parse[] AdvanceChunks(Parse inputParse, double minChunkScore) 
		{
			// chunk
			Parse[] children = inputParse.GetChildren();
			var words = new string[children.Length];
			var parseTags = new string[words.Length];
			var probabilities = new double[words.Length];
		    for (int childParseIndex = 0, childParseCount = children.Length; childParseIndex < childParseCount; childParseIndex++) 
			{
				Parse currentChildParse = children[childParseIndex];
				words[childParseIndex] = currentChildParse.Head.ToString();
				parseTags[childParseIndex] = currentChildParse.Type;
			}
			//System.Console.Error.WriteLine("adjusted min chunk score = " + (minChunkScore - inputParse.Probability));
			Util.Sequence[] chunkerSequences = basalChunker.TopKSequences(words, parseTags, minChunkScore - inputParse.Probability);
			var newParses = new Parse[chunkerSequences.Length];
			for (int sequenceIndex = 0, sequenceCount = chunkerSequences.Length; sequenceIndex < sequenceCount; sequenceIndex++) 
			{
				newParses[sequenceIndex] = (Parse) inputParse.Clone(); //copies top level
				if (CreateDerivationString)
				{
					newParses[sequenceIndex].AppendDerivationBuffer(sequenceIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
					newParses[sequenceIndex].AppendDerivationBuffer(".");
				}
				string[] tags = chunkerSequences[sequenceIndex].Outcomes.ToArray();
				chunkerSequences[sequenceIndex].GetProbabilities(probabilities);
				int start = -1;
				int end = 0;
				string type = null;
				//System.Console.Error.Write("sequence " + sequenceIndex + " ");
				for (int tagIndex = 0; tagIndex <= tags.Length; tagIndex++) 
				{
					//if (tagIndex != tags.Length)
					//{
					//	System.Console.Error.WriteLine(words[tagIndex] + " " + parseTags[tagIndex] + " " + tags[tagIndex] + " " + probabilities[tagIndex]);
					//}
					if (tagIndex != tags.Length) 
					{
						newParses[sequenceIndex].AddProbability(Math.Log(probabilities[tagIndex]));
					}
					if (tagIndex != tags.Length && tags[tagIndex].StartsWith(ContinuePrefix)) 
					{ // if continue just update end chunking tag don't use mContinueTypeMap
						end = tagIndex;
					}
					else 
					{ //make previous constituent if it exists
						if (type != null) 
						{
							//System.Console.Error.WriteLine("inserting tag " + tags[tagIndex]);
							Parse startParse = children[start];
							Parse endParse = children[end];
							//System.Console.Error.WriteLine("Putting " + type + " at " + start + "," + end + " " + newParses[sequenceIndex].Probability);
							var consitituents = new Parse[end - start + 1];
							consitituents[0] = startParse;
							//consitituents[0].Label = "Start-" + type;
							if (end - start != 0) 
							{
								consitituents[end - start] = endParse;
								//consitituents[end - start].Label = "Cont-" + type;
								for (int constituentIndex = 1; constituentIndex < end - start; constituentIndex++) 
								{
									consitituents[constituentIndex] = children[constituentIndex + start];
									//consitituents[constituentIndex].Label = "Cont-" + type;
								}
							}
							newParses[sequenceIndex].Insert(new Parse(startParse.Text, new Util.Span(startParse.Span.Start, endParse.Span.End), type, 1, headRules.GetHead(consitituents, type)));
						}
						if (tagIndex != tags.Length) 
						{ //update for new constituent
							if (tags[tagIndex].StartsWith(StartPrefix)) 
							{ // don't use mStartTypeMap these are chunk tags
								type = tags[tagIndex].Substring(StartPrefix.Length);
								start = tagIndex;
								end = tagIndex;
							}
							else 
							{ // other 
								type = null;
							}
						}
					}
				}
				//newParses[sequenceIndex].Show();
				//System.Console.Out.WriteLine();
			}
			return newParses;
		}
		
		///<summary>
		///Advances the parse by assigning it POS tags and returns multiple tag sequences.
		///</summary>
		///<param name="inputParse">
		///The parse to be tagged.
		///</param>
		///<returns>
		///Parses with different pos-tag sequence assignments.
		///</returns>
		private Parse[] AdvanceTags(Parse inputParse) 
		{
			Parse[] children = inputParse.GetChildren();
		    var words = children.Select(ch => ch.ToString()).ToArray();
            var probabilities = new double[words.Length];

			Util.Sequence[] tagSequences = posTagger.TopKSequences(words);
			if (tagSequences.Length == 0) 
			{
				Console.Error.WriteLine("no tag sequence");
			}
			var newParses = new Parse[tagSequences.Length];
			for (int tagSequenceIndex = 0; tagSequenceIndex < tagSequences.Length; tagSequenceIndex++) 
			{
				string[] tags = tagSequences[tagSequenceIndex].Outcomes.ToArray();
				tagSequences[tagSequenceIndex].GetProbabilities(probabilities);
				newParses[tagSequenceIndex] = (Parse) inputParse.Clone(); //copies top level
				if (CreateDerivationString)
				{
					newParses[tagSequenceIndex].AppendDerivationBuffer(tagSequenceIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
					newParses[tagSequenceIndex].AppendDerivationBuffer(".");
				}
				for (int wordIndex = 0; wordIndex < words.Length; wordIndex++) 
				{
					Parse wordParse = children[wordIndex];
					//System.Console.Error.WriteLine("inserting tag " + tags[wordIndex]);
					double wordProbability = probabilities[wordIndex];
					newParses[tagSequenceIndex].Insert(new Parse(wordParse.Text, wordParse.Span, tags[wordIndex], wordProbability));
					newParses[tagSequenceIndex].AddProbability(Math.Log(wordProbability));
					//newParses[tagSequenceIndex].Show();
				}
			}
			return newParses;
		}


        // Utilities -----------------------

		private static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventStream, int iterations, int cut)
		{
			var trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventStream, cut));
			return new SharpEntropy.GisModel(trainer);
		}
		
		public static SharpEntropy.GisModel TrainModel(string trainingFile, EventType modelType, string headRulesFile)
		{
			return TrainModel(trainingFile, modelType, headRulesFile, 100, 5);
		}

		public static SharpEntropy.GisModel TrainModel(string trainingFile, EventType modelType, string headRulesFile, int iterations, int cutoff)
		{
			var rules = new EnglishHeadRules(headRulesFile);
			SharpEntropy.ITrainingEventReader eventReader = new ParserEventReader(new SharpEntropy.PlainTextByLineDataReader(new StreamReader(trainingFile)), rules, modelType);
			return Train(eventReader, iterations, cutoff);
		}
	}
}
