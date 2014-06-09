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
		private int M;

		///<summary>
		///The maximum number of parses to advance from a single preceding parse.
		///</summary>
		private int K;

		///<summary>
		///The minimum total probability mass of advanced outcomes.
		///</summary>
		private double Q;

		///<summary>
		///The default beam size used if no beam size is given.
		///</summary>
		public const int DefaultBeamSize = 20;

		///<summary>
		///The default amount of probability mass required of advanced outcomes.
		///</summary>
		public const double DefaultAdvancePercentage = 0.95;
		
		///<summary>
		///Completed parses.
		///</summary>
		private Util.SortedSet<Parse> mParses;
		
		///<summary>
		///Incomplete parses which will be advanced.
		///</summary>
        private Util.SortedSet<Parse> mOldDerivationsHeap;

		///<summary>
		///Incomplete parses which have been advanced.
		///</summary>
        private Util.SortedSet<Parse> mNewDerivationsHeap;

		private IParserTagger mPosTagger; 
		private IParserChunker mBasalChunker; 
		
		private SharpEntropy.IMaximumEntropyModel mBuildModel;
		private SharpEntropy.IMaximumEntropyModel mCheckModel;
		
		private BuildContextGenerator mBuildContextGenerator;
		private CheckContextGenerator mCheckContextGenerator;
		
		private IHeadRules mHeadRules;
		
		private double[] mBuildProbabilities;
		private double[] mCheckProbabilities;
		
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
		
		private const string mTopStart = StartPrefix + TopNode;

		private int mTopStartIndex;
		private Dictionary<string, string> mStartTypeMap;
        private Dictionary<string, string> mContinueTypeMap;
  
		private int mCompleteIndex;
		private int mIncompleteIndex;
  
		private bool mCreateDerivationString = false;

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
			mPosTagger = tagger;
			mBasalChunker = chunker;
			mBuildModel = buildModel;
			mCheckModel = checkModel;
			M = beamSize;
			K = beamSize;
			Q = advancePercentage;

			mBuildProbabilities = new double[mBuildModel.OutcomeCount];
			mCheckProbabilities = new double[mCheckModel.OutcomeCount];
			mBuildContextGenerator = new BuildContextGenerator();
			mCheckContextGenerator = new CheckContextGenerator();
			mHeadRules = headRules;
			mOldDerivationsHeap = new Util.TreeSet<Parse>();
			mNewDerivationsHeap = new Util.TreeSet<Parse>();
			mParses = new Util.TreeSet<Parse>();

			mStartTypeMap = new Dictionary<string, string>();
            mContinueTypeMap = new Dictionary<string, string>();
			for (int buildOutcomeIndex = 0, buildOutcomeCount = buildModel.OutcomeCount; buildOutcomeIndex < buildOutcomeCount; buildOutcomeIndex++) 
			{
				string outcome = buildModel.GetOutcomeName(buildOutcomeIndex);
				if (outcome.StartsWith(StartPrefix)) 
				{
					//System.Console.Error.WriteLine("startMap " + outcome + "->" + outcome.Substring(StartPrefix.Length));
					mStartTypeMap.Add(outcome, outcome.Substring(StartPrefix.Length));
				}
				else if (outcome.StartsWith(ContinuePrefix)) 
				{
					//System.Console.Error.WriteLine("contMap " + outcome + "->" + outcome.Substring(ContinuePrefix.Length));
					mContinueTypeMap.Add(outcome, outcome.Substring(ContinuePrefix.Length));
				}
			}
			mTopStartIndex = buildModel.GetOutcomeIndex(mTopStart);
			mCompleteIndex = checkModel.GetOutcomeIndex(CompleteOutcome);
			mIncompleteIndex = checkModel.GetOutcomeIndex(IncompleteOutcome);
		}
		
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
			if (mCreateDerivationString) 
			{
				flatParse.InitializeDerivationBuffer();
			}
			mOldDerivationsHeap.Clear();
			mNewDerivationsHeap.Clear();
			mParses.Clear();
			int derivationLength = 0; 
			int maxDerivationLength = 2 * flatParse.ChildCount + 3;
			mOldDerivationsHeap.Add(flatParse);
			Parse guessParse = null;
			double bestComplete = - 100000; //approximating -infinity/0 in ln domain
			while (mParses.Count < M && derivationLength < maxDerivationLength)
			{
				mNewDerivationsHeap = new Util.TreeSet<Parse>();
				if (mOldDerivationsHeap.Count > 0)
				{
					int derivationsProcessed = 0;

					foreach (Parse currentParse in mOldDerivationsHeap)
						//for (System.Collections.IEnumerator pi = mOldDerivationsHeap.GetEnumerator(); pi.MoveNext() && derivationsProcessed < K; derivationsProcessed++)
					{
						derivationsProcessed++;
						if (derivationsProcessed >= K) 
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

						//System.Console.Out.Write(derivationLength + " " + derivationsProcessed + " "+currentParse.Probability);
						//System.Console.Out.Write(currentParse.Show());
						//System.Console.Out.WriteLine();

						Parse[] newDerivations = null;
						if (0 == derivationLength) 
						{
							newDerivations = AdvanceTags(currentParse);
						}
						else if (1 == derivationLength) 
						{
							if (mNewDerivationsHeap.Count < K) 
							{
								//System.Console.Error.WriteLine("advancing ts " + derivationsProcessed + " " + mNewDerivationsHeap.Count + " < " + K);
								newDerivations = AdvanceChunks(currentParse, bestComplete);
							}
							else 
							{
								//System.Console.Error.WriteLine("advancing ts " + derivationsProcessed + " prob=" + ((Parse) mNewDerivationsHeap.Last()).Probability);
								newDerivations = AdvanceChunks(currentParse,((Parse) mNewDerivationsHeap.Last()).Probability);
							}
						}
						else 
						{ // derivationLength > 1
							newDerivations = AdvanceParses(currentParse, Q);
						}

						if (newDerivations != null)
						{
							for (int currentDerivation = 0, derivationCount = newDerivations.Length; currentDerivation < derivationCount; currentDerivation++)
							{
								//System.out.println("currentDerivation="+currentDerivation+" of "+newDerivations.length);
								if (newDerivations[currentDerivation].IsComplete)
								{
									AdvanceTop(newDerivations[currentDerivation]);
									if (newDerivations[currentDerivation].Probability > bestComplete)
									{
										bestComplete = newDerivations[currentDerivation].Probability;
									}
									mParses.Add(newDerivations[currentDerivation]);
									
								}
								else
								{
									mNewDerivationsHeap.Add(newDerivations[currentDerivation]);
								}
							}
							//RN added sort
							mNewDerivationsHeap.Sort();
						}
						else
						{
							System.Console.Error.WriteLine("Couldn't advance parse " + derivationLength + " stage " + derivationsProcessed + "!\n");
						}
					}
					derivationLength++;
					mOldDerivationsHeap = mNewDerivationsHeap;
				}
				else
				{
					break;
				}
			}
		
			//RN added sort
			mParses.Sort();
			
			if (mParses.Count == 0)
			{
				System.Console.Error.WriteLine("Couldn't find parse for: " + flatParse);
				//oFullParse = (Parse) mOldDerivationsHeap.First(); 
				return new Parse[] {guessParse};
			}
			else if (parseCount == 1)
			{
				//RN added parent adjustment
				Parse topParse = mParses.First();
				topParse.UpdateChildParents();
				return new Parse[] {topParse};
			}
			else
			{
                List<Parse> topParses = new List<Parse>(parseCount);
				while(!mParses.IsEmpty() && topParses.Count < parseCount) 
				{
					Parse topParse = mParses.First();
					//RN added parent adjustment
					topParse.UpdateChildParents();
					topParses.Add(topParse);
					mParses.Remove(topParse);
				}
				return topParses.ToArray();
			}
		}
		
		private void AdvanceTop(Parse inputParse)
		{
			mBuildModel.Evaluate(mBuildContextGenerator.GetContext(inputParse.GetChildren(), 0), mBuildProbabilities);
			inputParse.AddProbability(System.Math.Log(mBuildProbabilities[mTopStartIndex]));
			mCheckModel.Evaluate(mCheckContextGenerator.GetContext(inputParse.GetChildren(), TopNode, 0, 0), mCheckProbabilities);
			inputParse.AddProbability(System.Math.Log(mCheckProbabilities[mCompleteIndex]));
			inputParse.Type = TopNode;
		}
		
		
		///<summary>
		///Advances the specified parse and returns the an array advanced parses whose probability accounts for
		///more than the speicficed amount of probability mass, Q.
		///</summary>
		///<param name="inputParse">
		///The parse to advance.
		///</param>
		///<param name="Q">
		///The amount of probability mass that should be accounted for by the advanced parses.
		///</param> 
		private Parse[] AdvanceParses(Parse inputParse, double Q) 
		{
			double q = 1 - Q;
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
				else if (mStartTypeMap.ContainsKey(advanceNode.Label)) 
				{
					lastStartType = mStartTypeMap[advanceNode.Label];
					lastStartNode = advanceNode;
					lastStartIndex = advanceNodeIndex;
					//System.Console.Error.WriteLine("lastStart " + lastStartIndex + " " + lastStartNode.Label + " " + lastStartNode.Probability);
				}
			}
            List<Parse> newParsesList = new List<Parse>(mBuildModel.OutcomeCount);
			//call build
			mBuildModel.Evaluate(mBuildContextGenerator.GetContext(children, advanceNodeIndex), mBuildProbabilities);
			double buildProbabilitiesSum = 0;
			while (buildProbabilitiesSum < Q) 
			{
				//  The largest unadvanced labeling.
				int highestBuildProbabilityIndex = 0;
				for (int probabilityIndex = 1; probabilityIndex < mBuildProbabilities.Length; probabilityIndex++) 
				{ //for each build outcome
					if (mBuildProbabilities[probabilityIndex] > mBuildProbabilities[highestBuildProbabilityIndex]) 
					{
						highestBuildProbabilityIndex = probabilityIndex;
					}
				}
				if (mBuildProbabilities[highestBuildProbabilityIndex] == 0) 
				{
					break;
				}

				double highestBuildProbability = mBuildProbabilities[highestBuildProbabilityIndex];		

				mBuildProbabilities[highestBuildProbabilityIndex] = 0; //zero out so new max can be found
				buildProbabilitiesSum += highestBuildProbability;

				string tag = mBuildModel.GetOutcomeName(highestBuildProbabilityIndex);
				//System.Console.Out.WriteLine("trying " + tag + " " + buildProbabilitiesSum + " lst=" + lst);
				if (highestBuildProbabilityIndex == mTopStartIndex) 
				{ // can't have top until complete
					continue;
				}
				//System.Console.Error.WriteLine(probabilityIndex + " " + tag + " " + highestBuildProbability);
				if (mStartTypeMap.ContainsKey(tag)) 
				{ //update last start
					lastStartIndex = advanceNodeIndex;
					lastStartNode = advanceNode;
					lastStartType = mStartTypeMap[tag];
				}
				else if (mContinueTypeMap.ContainsKey(tag)) 
				{
					if (lastStartNode == null || lastStartType != mContinueTypeMap[tag]) 
					{
						continue; //Cont must match previous start or continue
					}
				}
				Parse newParse1 = (Parse) inputParse.Clone(); //clone parse
				if (mCreateDerivationString)
				{
					newParse1.AppendDerivationBuffer(highestBuildProbabilityIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
					newParse1.AppendDerivationBuffer("-");
				}
				newParse1.SetChild(advanceNodeIndex, tag); //replace constituent labeled

				newParse1.AddProbability(System.Math.Log(highestBuildProbability));
				//check
				mCheckModel.Evaluate(mCheckContextGenerator.GetContext(newParse1.GetChildren(), lastStartType, lastStartIndex, advanceNodeIndex), mCheckProbabilities);
				//System.Console.Out.WriteLine("check " + mCheckProbabilities[mCompleteIndex] + " " + mCheckProbabilities[mIncompleteIndex]);
				Parse newParse2 = newParse1;
				if (mCheckProbabilities[mCompleteIndex] > q) 
				{ //make sure a reduce is likely
					newParse2 = (Parse) newParse1.Clone();
					if (mCreateDerivationString)
					{
						newParse2.AppendDerivationBuffer("1");
						newParse2.AppendDerivationBuffer(".");
					}
					newParse2.AddProbability(System.Math.Log(mCheckProbabilities[1]));
					Parse[] constituent = new Parse[advanceNodeIndex - lastStartIndex + 1];
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
						newParse2.Insert(new Parse(inputParse.Text, new Util.Span(lastStartNode.Span.Start, advanceNode.Span.End), lastStartType, mCheckProbabilities[1], mHeadRules.GetHead(constituent, lastStartType)));
						newParsesList.Add(newParse2);
					}
				}
				if (mCheckProbabilities[mIncompleteIndex] > q) 
				{ //make sure a shift is likely
					if (mCreateDerivationString)
					{
						newParse1.AppendDerivationBuffer("0");
						newParse1.AppendDerivationBuffer(".");
					}
					if (advanceNodeIndex != nodeCount - 1) 
					{ //can't shift last element
						newParse1.AddProbability(System.Math.Log(mCheckProbabilities[0]));
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
			string[] words = new string[children.Length];
			string[] parseTags = new string[words.Length];
			double[] probabilities = new double[words.Length];
			Parse currentChildParse = null;
			for (int childParseIndex = 0, childParseCount = children.Length; childParseIndex < childParseCount; childParseIndex++) 
			{
				currentChildParse = children[childParseIndex];
				words[childParseIndex] = currentChildParse.Head.ToString();
				parseTags[childParseIndex] = currentChildParse.Type;
			}
			//System.Console.Error.WriteLine("adjusted min chunk score = " + (minChunkScore - inputParse.Probability));
			Util.Sequence[] chunkerSequences = mBasalChunker.TopKSequences(words, parseTags, minChunkScore - inputParse.Probability);
			Parse[] newParses = new Parse[chunkerSequences.Length];
			for (int sequenceIndex = 0, sequenceCount = chunkerSequences.Length; sequenceIndex < sequenceCount; sequenceIndex++) 
			{
				newParses[sequenceIndex] = (Parse) inputParse.Clone(); //copies top level
				if (mCreateDerivationString)
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
						newParses[sequenceIndex].AddProbability(System.Math.Log(probabilities[tagIndex]));
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
							Parse[] consitituents = new Parse[end - start + 1];
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
							newParses[sequenceIndex].Insert(new Parse(startParse.Text, new Util.Span(startParse.Span.Start, endParse.Span.End), type, 1, mHeadRules.GetHead(consitituents, type)));
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
			string[] words = new string[children.Length];
			double[] probabilities = new double[words.Length];
			for (int childParseIndex = 0; childParseIndex < children.Length; childParseIndex++) 
			{
				words[childParseIndex] = (children[childParseIndex]).ToString();
			}
			Util.Sequence[] tagSequences = mPosTagger.TopKSequences(words);
			if (tagSequences.Length == 0) 
			{
				System.Console.Error.WriteLine("no tag sequence");
			}
			Parse[] newParses = new Parse[tagSequences.Length];
			for (int tagSequenceIndex = 0; tagSequenceIndex < tagSequences.Length; tagSequenceIndex++) 
			{
				string[] tags = tagSequences[tagSequenceIndex].Outcomes.ToArray();
				tagSequences[tagSequenceIndex].GetProbabilities(probabilities);
				newParses[tagSequenceIndex] = (Parse) inputParse.Clone(); //copies top level
				if (mCreateDerivationString)
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
					newParses[tagSequenceIndex].AddProbability(System.Math.Log(wordProbability));
					//newParses[tagSequenceIndex].Show();
				}
			}
			return newParses;
		}

		private static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventStream, int iterations, int cut)
		{
			SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventStream, cut));
			return new SharpEntropy.GisModel(trainer);
		}
		
		public static SharpEntropy.GisModel TrainModel(string trainingFile, EventType modelType, string headRulesFile)
		{
			return TrainModel(trainingFile, modelType, headRulesFile, 100, 5);
		}

		public static SharpEntropy.GisModel TrainModel(string trainingFile, EventType modelType, string headRulesFile, int iterations, int cutoff)
		{
			EnglishHeadRules rules = new EnglishHeadRules(headRulesFile);
			SharpEntropy.ITrainingEventReader eventReader = new ParserEventReader(new SharpEntropy.PlainTextByLineDataReader(new System.IO.StreamReader(trainingFile)), rules, modelType);
			return Train(eventReader, iterations, cutoff);
		}
	}
}
