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

//This file is based on the BeamSearch.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Gann Bierner and Thomas Morton
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
using System.Collections;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// Performs k-best search over sequence.  This is besed on the description in
	/// Ratnaparkhi (1998), PhD diss, Univ. of Pennsylvania. 
	/// </summary>
	public class BeamSearch
	{
		internal SharpEntropy.IMaximumEntropyModel Model;
		internal IBeamSearchContextGenerator ContextGenerator;
		internal int Size;
		private static object[] mEmptyAdditionalContext = new object[0];
		private double[] mProbabilities;
		private Cache mContextsCache;
		private const int mZeroLog = -100000;

		/// <summary>
		/// Creates new search object.
		/// </summary>
		/// <param name="size">
		/// The size of the beam (k).
		/// </param>
		/// <param name="contextGenerator">
		/// the context generator for the model. 
		/// </param>
		/// <param name="model">
		/// the model for assigning probabilities to the sequence outcomes.
		/// </param>
		public BeamSearch(int size, IBeamSearchContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model) : this(size, contextGenerator, model, 0)
		{
		}

		/// <summary>
		/// Creates new search object.
		/// </summary>
		/// <param name="size">
		/// The size of the beam (k).
		/// </param>
		/// <param name="contextGenerator">
		/// the context generator for the model. 
		/// </param>
		/// <param name="model">
		/// the model for assigning probabilities to the sequence outcomes.
		/// </param>
		/// <param name="cacheSize">
		/// size of the cache to use for performance.
		/// </param>
		public BeamSearch(int size, IBeamSearchContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model, int cacheSize)
		{
			Size = size;
			ContextGenerator = contextGenerator;
			Model = model;

			mProbabilities = new double[model.OutcomeCount];
			if (cacheSize > 0) 
			{
				mContextsCache = new Cache(cacheSize);
			}
		}
		
		/// <summary>
		/// Returns the best sequence of outcomes based on model for this object.</summary>
		/// <param name="numSequences">
		/// The maximum number of sequences to be returned.
		/// </param>
		/// <param name="sequence">
		/// The input sequence.
		/// </param>
		/// <param name="additionalContext">
		/// An object[] of additional context.  This is passed to the context generator blindly with the assumption that the context are appropiate.
		/// </param>
		/// <returns>
		/// An array of the top ranked sequences of outcomes.
		/// </returns>		
		public Sequence[] BestSequences(int numSequences, object[] sequence, object[] additionalContext) 
		{
			return BestSequences(numSequences, sequence, additionalContext, mZeroLog);
		}

		/// <summary>
		/// Returns the best sequence of outcomes based on model for this object.</summary>
		/// <param name="numSequences">
		/// The maximum number of sequences to be returned.
		/// </param>
		/// <param name="sequence">
		/// The input sequence.
		/// </param>
		/// <param name="additionalContext">
		/// An object[] of additional context.  This is passed to the context generator blindly with the assumption that the context are appropiate.
		/// </param>
		/// <param name="minSequenceScore">
		/// A lower bound on the score of a returned sequence.</param> 
		/// <returns>
		/// An array of the top ranked sequences of outcomes.
		/// </returns>		
		public virtual Sequence[] BestSequences(int numSequences, object[] sequence, object[] additionalContext, double minSequenceScore)
		{
			int sequenceCount = sequence.Length;
            ListHeap<Sequence> previousHeap = new ListHeap<Sequence>(Size);
            ListHeap<Sequence> nextHeap = new ListHeap<Sequence>(Size);
            ListHeap<Sequence> tempHeap;

			previousHeap.Add(new Sequence());
			if (additionalContext == null)
			{
				additionalContext = mEmptyAdditionalContext;
			}
			for (int currentSequence = 0; currentSequence < sequenceCount; currentSequence++)
			{
				int sz = System.Math.Min(Size, previousHeap.Size);
				int sc = 0;
				for (; previousHeap.Size > 0 && sc < sz; sc++) 
				{
					Sequence topSequence = previousHeap.Extract();
					String[] outcomes = topSequence.Outcomes.ToArray();
					String[] contexts = ContextGenerator.GetContext(currentSequence, sequence, outcomes, additionalContext);
					double[] scores;
					if (mContextsCache != null) 
					{
						scores = (double[]) mContextsCache[contexts];
						if (scores == null) 
						{
							scores = Model.Evaluate(contexts, mProbabilities);
							mContextsCache[contexts] = scores;
						}
					}
					else 
					{
						scores = Model.Evaluate(contexts, mProbabilities);
					}

					double[] tempScores = new double[scores.Length];
					Array.Copy(scores, tempScores, scores.Length);
					
					Array.Sort(tempScores);
					double minimum = tempScores[System.Math.Max(0, scores.Length - Size)];
					
					for (int currentScore = 0; currentScore < scores.Length; currentScore++)
					{
						if (scores[currentScore] < minimum)
						{
							continue; //only advance first "size" outcomes
						}

						string outcomeName = Model.GetOutcomeName(currentScore);
						if (ValidSequence(currentSequence, sequence, outcomes, outcomeName))
						{
							Sequence newSequence = new Sequence(topSequence, outcomeName, scores[currentScore]);
							if (newSequence.Score > minSequenceScore)
							{
								nextHeap.Add(newSequence);
							}
						}
					}
					if (nextHeap.Size == 0)
					{//if no advanced sequences, advance all valid
						for (int currentScore = 0; currentScore < scores.Length; currentScore++) 
						{
							string outcomeName = Model.GetOutcomeName(currentScore);
							if (ValidSequence(currentSequence, sequence, outcomes, outcomeName))
							{
								Sequence newSequence = new Sequence(topSequence, outcomeName, scores[currentScore]);
								if (newSequence.Score > minSequenceScore)
								{
									nextHeap.Add(newSequence);
								}
							}
						}
					}
					//nextHeap.Sort();
				}
				//    make prev = next; and re-init next (we reuse existing prev set once we clear it)
				previousHeap.Clear();
				tempHeap = previousHeap;
				previousHeap = nextHeap;
				nextHeap = tempHeap;
			}
			int topSequenceCount = System.Math.Min(numSequences, previousHeap.Size);
			Sequence[] topSequences = new Sequence[topSequenceCount];
			int sequenceIndex = 0;
			for (; sequenceIndex < topSequenceCount; sequenceIndex++) 
			{
				topSequences[sequenceIndex] = (Sequence) previousHeap.Extract();
			}
			return topSequences;
		}

		/// <summary>
		/// Returns the best sequence of outcomes based on model for this object.
		/// </summary>
		/// <param name="sequence">
		/// The input sequence.
		/// </param>
		/// <param name="additionalContext">
		/// An object[] of additional context.  This is passed to the context generator blindly with the assumption that the context are appropiate.
		/// </param>
		/// <returns>
		/// The top ranked sequence of outcomes.
		/// </returns>
		public virtual Sequence BestSequence(ArrayList sequence, object[] additionalContext)
		{
			return BestSequences(1, sequence.ToArray(), additionalContext)[0];
		}
  
		/// <summary>
		/// Returns the best sequence of outcomes based on model for this object.
		/// </summary>
		/// <param name="sequence">
		/// The input sequence.
		/// </param>
		/// <param name="additionalContext">
		/// An object[] of additional context.  This is passed to the context generator blindly with the assumption that the context are appropiate.
		/// </param>
		/// <returns>
		/// The top ranked sequence of outcomes.
		/// </returns>
		public Sequence BestSequence(object[] sequence, object[] additionalContext) 
		{
			return BestSequences(1, sequence, additionalContext, mZeroLog)[0];
		}
		
		/// <summary>
		/// Determines whether a particular continuation of a sequence is valid.  
		/// This is used to restrict invalid sequences such as thoses used in start/continue tag-based chunking 
		/// or could be used to implement tag dictionary restrictions.
		/// </summary>
		/// <param name="index">
		/// The index in the input sequence for which the new outcome is being proposed.
		/// </param>
		/// <param name="inputSequence">
		/// The input sequnce.
		/// </param>
		/// <param name="outcomesSequence">
		/// The outcomes so far in this sequence.
		/// </param>
		/// <param name="outcome">
		/// The next proposed outcome for the outcomes sequence.
		/// </param>
		/// <returns>
		/// true if the sequence would still be valid with the new outcome, false otherwise.
		/// </returns>
		protected internal virtual bool ValidSequence(int index, ArrayList inputSequence, Sequence outcomesSequence, string outcome)
		{
			return true;
		}

		/// <summary>
		/// Determines whether a particular continuation of a sequence is valid.  
		/// This is used to restrict invalid sequences such as thoses used in start/continure tag-based chunking 
		/// or could be used to implement tag dictionary restrictions.
		/// </summary>
		/// <param name="index">
		/// The index in the input sequence for which the new outcome is being proposed.
		/// </param>
		/// <param name="inputSequence">
		/// The input sequnce.
		/// </param>
		/// <param name="outcomesSequence">
		/// The outcomes so far in this sequence.
		/// </param>
		/// <param name="outcome">
		/// The next proposed outcome for the outcomes sequence.
		/// </param>
		/// <returns>
		/// true if the sequence would still be valid with the new outcome, false otherwise.
		/// </returns>
		protected internal virtual bool ValidSequence(int index, object[] inputSequence, string[] outcomesSequence, string outcome) 
		{
			return true;
		}
	}
}
