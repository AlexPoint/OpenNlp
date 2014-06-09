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

//This file is based on the ParserEventStream.java source file found in the
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
	///  Enumerated type of event types for the parser. 
	/// </summary>
	public enum EventType
	{
		Build,
		Check,
		Chunk,
		Tag
	}

	/// <summary>
	/// Wrapper class for one of four parser event readers.  The particular event stream is specified 
	/// at construction.
	/// </summary>
	public class ParserEventReader : SharpEntropy.ITrainingEventReader
	{
		private BuildContextGenerator mBuildContextGenerator;
		private CheckContextGenerator mCheckContextGenerator;
		private ChunkContextGenerator mChunkContextGenerator;
		private OpenNLP.Tools.PosTagger.IPosContextGenerator mPosContextGenerator;
		private SharpEntropy.ITrainingDataReader<string> mDataReader;
		private SharpEntropy.TrainingEvent[] mEvents;
		private int mEventIndex;
		private IHeadRules mHeadRules;
		private EventType mEventType;
		
		/// <summary>
		/// Create an event reader based on the specified data reader of the specified type using the specified head rules.
		/// </summary>
		/// <param name="dataReader">
		/// A 1-parse-per-line Penn Treebank Style parse. 
		/// </param>
		/// <param name="rules">
		/// The head rules.
		/// </param>
		/// <param name="eventType">
		/// The type of events desired (tag, chunk, build, or check).
		/// </param>
        public ParserEventReader(SharpEntropy.ITrainingDataReader<string> dataReader, IHeadRules rules, EventType eventType)
		{
			if (eventType == EventType.Build)
			{
				mBuildContextGenerator = new BuildContextGenerator();
			}
			else if (eventType == EventType.Check)
			{
				mCheckContextGenerator = new CheckContextGenerator();
			}
			else if (eventType == EventType.Chunk)
			{
				mChunkContextGenerator = new ChunkContextGenerator();
			}
			else if (eventType == EventType.Tag)
			{
				mPosContextGenerator = new PosTagger.DefaultPosContextGenerator();
			}
			mHeadRules = rules;
			mEventType = eventType;
			mDataReader = dataReader;
			mEventIndex = 0;
			if (dataReader.HasNext())
			{
				AddNewEvents();
			}
			else
			{
				mEvents = new SharpEntropy.TrainingEvent[0];
			}
		}
		
		public virtual bool HasNext()
		{
			return (mEventIndex < mEvents.Length || mDataReader.HasNext());
		}
		
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			if (mEventIndex == mEvents.Length)
			{
				AddNewEvents();
				mEventIndex = 0;
			}
			return ((SharpEntropy.TrainingEvent) mEvents[mEventIndex++]);
		}

        private static void GetInitialChunks(Parse inputParse, List<Parse> initialChunks)
		{
			if (inputParse.IsPosTag)
			{
				initialChunks.Add(inputParse);
			}
			else
			{
				Parse[] kids = inputParse.GetChildren();
				bool AreAllKidsTags = true;
				for (int currentChild = 0, childCount = kids.Length; currentChild < childCount; currentChild++)
				{
					if (!(kids[currentChild]).IsPosTag)
					{
						AreAllKidsTags = false;
						break;
					}
				}
				if (AreAllKidsTags)
				{
					initialChunks.Add(inputParse);
				}
				else
				{
					for (int currentChild = 0, childCount = kids.Length; currentChild < childCount; currentChild++)
					{
						GetInitialChunks(kids[currentChild], initialChunks);
					}
				}
			}
		}
		
		private static Parse[] GetInitialChunks(Parse inputParse)
		{
            List<Parse> chunks = new List<Parse>();
			GetInitialChunks(inputParse, chunks);
			return chunks.ToArray();
		}
		
		private static bool IsFirstChild(Parse child, Parse parent)
		{
			Parse[] kids = parent.GetChildren();
			return kids[0] == child;
		}
		
		private static bool IsLastChild(Parse child, Parse parent)
		{
			Parse[] kids = parent.GetChildren();
			return kids[kids.Length - 1] == child;
		}
		
		private void AddNewEvents()
		{
			string parseString = mDataReader.NextToken();
			//System.Console.WriteLine("ParserEventStream.AddNewEvents: " + parseString);
            List<SharpEntropy.TrainingEvent> events = new List<SharpEntropy.TrainingEvent>();
			Parse rootParse = Parse.FromParseString(parseString);
			rootParse.UpdateHeads(mHeadRules);
			Parse[] chunks = GetInitialChunks(rootParse);
			if (mEventType == EventType.Tag)
			{
				AddTagEvents(events, chunks);
			}
			else if (mEventType == EventType.Chunk)
			{
				AddChunkEvents(events, chunks);
			}
			else
			{
				AddParseEvents(events, chunks);
			}
			mEvents = events.ToArray();
		}

        private void AddParseEvents(List<SharpEntropy.TrainingEvent> events, Parse[] chunks)
		{
			int currentChunk = 0;
			while (currentChunk < chunks.Length)
			{
				Parse chunkParse = chunks[currentChunk];
				Parse parentParse = chunkParse.Parent;
				if (parentParse != null)
				{
					string type = parentParse.Type;
					string outcome;
					if (IsFirstChild(chunkParse, parentParse))
					{
						outcome = MaximumEntropyParser.StartPrefix + type;
					}
					else
					{
						outcome = MaximumEntropyParser.ContinuePrefix + type;
					}
					chunkParse.Label = outcome;
					if (mEventType == EventType.Build)
					{
						events.Add(new SharpEntropy.TrainingEvent(outcome, mBuildContextGenerator.GetContext(chunks, currentChunk)));
					}
					int start = currentChunk - 1;
					while (start >= 0 && (chunks[start]).Parent == parentParse)
					{
						start--;
					}
					if (IsLastChild(chunkParse, parentParse))
					{
						if (mEventType == EventType.Check)
						{
							events.Add(new SharpEntropy.TrainingEvent(MaximumEntropyParser.CompleteOutcome, mCheckContextGenerator.GetContext(chunks, type, start + 1, currentChunk)));
						}
						//perform reduce
						int reduceStart = currentChunk;
						int reduceEnd = currentChunk;
						while (reduceStart >=0 && chunks[reduceStart].Parent == parentParse) 
						{
							reduceStart--;

						}
						reduceStart++;

						if (!(type == MaximumEntropyParser.TopNode))
						{
							Parse[] reducedChunks = new Parse[chunks.Length - (reduceEnd - reduceStart + 1) + 1]; //total - num_removed + 1 (for new node)
							//insert nodes before reduction
							for (int reductionIndex = 0, reductionCount = reduceStart; reductionIndex < reductionCount; reductionIndex++) 
							{
								reducedChunks[reductionIndex] = chunks[reductionIndex];
							}
							//insert reduced node
							reducedChunks[reduceStart] = parentParse;
							//insert nodes after reduction
							int currentReductionIndex = reduceStart + 1;
							for (int afterReductionIndex = reduceEnd + 1; afterReductionIndex < chunks.Length; afterReductionIndex++) 
							{
								reducedChunks[currentReductionIndex] = chunks[afterReductionIndex];
								currentReductionIndex++;
							}
							chunks = reducedChunks;
							currentChunk = reduceStart - 1; //currentChunk will be incremented at end of loop
						}
						else 
						{
							chunks = new Parse[0];
						}
					}
					else
					{
						if (mEventType == EventType.Check)
						{
							events.Add(new SharpEntropy.TrainingEvent(MaximumEntropyParser.IncompleteOutcome, mCheckContextGenerator.GetContext(chunks, type, start + 1, currentChunk)));
						}
					}
				}
				currentChunk++;
			}
		}

        private void AddChunkEvents(List<SharpEntropy.TrainingEvent> events, Parse[] chunks)
		{
            List<string> tokens = new List<string>();
            List<string> tags = new List<string>();
            List<string> predicates = new List<string>();
			for (int currentChunk = 0; currentChunk < chunks.Length; currentChunk++)
			{
				Parse chunkParse = chunks[currentChunk];
				if (chunkParse.IsPosTag)
				{
					tokens.Add(chunkParse.ToString());
					tags.Add(chunkParse.Type);
					predicates.Add(MaximumEntropyParser.OtherOutcome);
				}
				else
				{
					bool isStart = true;
					string chunkType = chunkParse.Type;
					Parse[] childParses = chunkParse.GetChildren();
					foreach (Parse tokenParse in childParses)
					{
						tokens.Add(tokenParse.ToString());
						tags.Add(tokenParse.Type);
						if (isStart)
						{
							predicates.Add(MaximumEntropyParser.StartPrefix + chunkType);
							isStart = false;
						}
						else
						{
							predicates.Add(MaximumEntropyParser.ContinuePrefix + chunkType);
						}
					}
				}
			}
			for (int currentToken = 0; currentToken < tokens.Count; currentToken++)
			{
				events.Add(new SharpEntropy.TrainingEvent(predicates[currentToken], mChunkContextGenerator.GetContext(currentToken, tokens.ToArray(), tags.ToArray(), predicates.ToArray())));
			}
		}

        private void AddTagEvents(List<SharpEntropy.TrainingEvent> events, Parse[] chunks)
		{
            List<string> tokens = new List<string>();
            List<string> predicates = new List<string>();
			for (int currentChunk = 0; currentChunk < chunks.Length; currentChunk++)
			{
				Parse chunkParse = chunks[currentChunk];
				if (chunkParse.IsPosTag)
				{
					tokens.Add(chunkParse.ToString());
					predicates.Add(chunkParse.Type);
				}
				else
				{
					Parse[] childParses = chunkParse.GetChildren();
					foreach (Parse tokenParse in childParses)
					{
						tokens.Add(tokenParse.ToString());
						predicates.Add(tokenParse.Type);
					}
				}
			}
			for (int currentToken = 0; currentToken < tokens.Count; currentToken++)
			{
				events.Add(new SharpEntropy.TrainingEvent(predicates[currentToken], mPosContextGenerator.GetContext(currentToken, tokens.ToArray(), predicates.ToArray(), null)));
			}
		}
		
//		[STAThread]
//		public static void  Main(System.String[] args)
//		{
//			if (args.Length == 0)
//			{
//				System.Console.Error.WriteLine("Usage ParserEventStream -[tag|chunk|build|check] head_rules < parses");
//				System.Environment.Exit(1);
//			}
//			EventTypeEnum etype = null;
//			int ai = 0;
//			if (args[ai].Equals("-build"))
//			{
//				etype = EventTypeEnum.Build;
//			}
//			else if (args[ai].Equals("-check"))
//			{
//				etype = EventTypeEnum.Check;
//			}
//			else if (args[ai].Equals("-chunk"))
//			{
//				etype = EventTypeEnum.Chunk;
//			}
//			else if (args[ai].Equals("-tag"))
//			{
//				etype = EventTypeEnum.Tag;
//			}
//			else
//			{
//				System.Console.Error.WriteLine("Invalid option " + args[ai]);
//				System.Environment.Exit(1);
//			}
//			ai++;
//			EnglishHeadRules rules = new EnglishHeadRules(args[ai++]);
//			MaxEnt.EventStream es = new ParserEventStream(new MaxEnt.PlainTextByLineDataStream(new System.IO.StreamReader(System.Console.In)), rules, etype);
//			while (es.hasNext())
//			{
//				System.Console.Out.WriteLine(es.nextEvent());
//			}
//		}
	}
}
