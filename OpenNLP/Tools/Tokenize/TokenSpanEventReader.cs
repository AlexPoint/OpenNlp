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

//This file is based on the TokSpanEventStream.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2003 Tom Morton
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
using System.Collections.Generic;

namespace OpenNLP.Tools.Tokenize
{
	/// <summary>
	/// An implementation of ITrainingEventReader which allows events to be added by 
	/// offset and returns events for these offset-based tokens.
	/// </summary>
	public class TokenSpanEventReader : SharpEntropy.ITrainingEventReader
	{
        private static readonly SharpEntropy.IContextGenerator<Util.Pair<string, int>> mContextGenerator = new TokenContextGenerator();
		private List<SharpEntropy.TrainingEvent> mEvents;
		private int mEventIndex;
		private bool mSkipAlphanumerics;
		
		public TokenSpanEventReader(bool skipAlphaNumerics)
		{
			mSkipAlphanumerics = skipAlphaNumerics;
			mEvents = new List<SharpEntropy.TrainingEvent>(50);
			mEventIndex = 0;
		}
		
		public virtual void AddEvents(Util.Span[] tokens, string input)
		{
			if (tokens.Length > 0)
			{
				int startPosition = tokens[0].Start;
				int endPosition = tokens[tokens.Length - 1].End;
				string sentence = input.Substring(startPosition, (endPosition) - (startPosition));
				Util.Span[] candidateTokens = MaximumEntropyTokenizer.Split(sentence);
				int firstTrainingToken = -1;
				int lastTrainingToken = -1;

				for (int currentCandidate = 0; currentCandidate < candidateTokens.Length; currentCandidate++)
				{
					Util.Span candidateSpan = candidateTokens[currentCandidate];
					string candidateToken = sentence.Substring(candidateSpan.Start, (candidateSpan.End) - (candidateSpan.Start));
					//adjust candidateSpan to text offsets
					candidateSpan = new Util.Span(candidateSpan.Start + startPosition, candidateSpan.End + startPosition);
					//should we skip this token
					if (candidateToken.Length > 1 && (!mSkipAlphanumerics || !MaximumEntropyTokenizer.AlphaNumeric.IsMatch(candidateToken))) 
					{
						//find offsets of annotated tokens inside candidate tokens
						bool foundTrainingTokens = false;
						for (int currentToken = lastTrainingToken + 1; currentToken < tokens.Length; currentToken++)
						{
							if (candidateSpan.Contains(tokens[currentToken]))
							{
								if (!foundTrainingTokens)
								{
									firstTrainingToken = currentToken;
									foundTrainingTokens = true;
								}
								lastTrainingToken = currentToken;
							}
							else if (candidateSpan.End < tokens[currentToken].End)
							{
								break;
							}
							else if (tokens[currentToken].End < candidateSpan.Start)
							{
								//keep looking
							}
							else
							{
								throw new ApplicationException("Bad training token: " + tokens[currentToken] + " cand: " + candidateSpan);
							}
						}
						// create training data
						if (foundTrainingTokens)
						{
							for (int currentToken = firstTrainingToken; currentToken <= lastTrainingToken; currentToken++)
							{
								Util.Span trainingTokenSpan = tokens[currentToken];
								
								int candidateStart = candidateSpan.Start;
								for (int currentPosition = trainingTokenSpan.Start + 1; currentPosition < trainingTokenSpan.End; currentPosition++)
								{
                                    string[] context = mContextGenerator.GetContext(new Util.Pair<string, int>(candidateToken, currentPosition - candidateStart));
									mEvents.Add(new SharpEntropy.TrainingEvent(TokenContextGenerator.NoSplitIndicator, context));
								}
								if (trainingTokenSpan.End != candidateSpan.End)
								{
                                    string[] context = mContextGenerator.GetContext(new Util.Pair<string, int>(candidateToken, trainingTokenSpan.End - candidateStart));
									mEvents.Add(new SharpEntropy.TrainingEvent(TokenContextGenerator.SplitIndicator, context));
								}
							}
						}
					}
				}
			}
		}
		
		public virtual bool HasNext()
		{
			return (mEventIndex < mEvents.Count);
		}
		
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			SharpEntropy.TrainingEvent nextEvent = mEvents[mEventIndex];
			mEventIndex++;
			if (mEventIndex == mEvents.Count)
			{
				mEvents.Clear();
				mEventIndex = 0;
			}
			return nextEvent;
		}
	}
}
