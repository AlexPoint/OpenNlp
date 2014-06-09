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

//This file is based on the NameFinderME.java source file found in the
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
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.NameFind
{
	/// <summary>
	/// Class for creating a maximum-entropy-based name finder.
	/// </summary>
	public class MaximumEntropyNameFinder : INameFinder
	{
		private SharpEntropy.IMaximumEntropyModel mModel;
		private INameContextGenerator mContextGenerator;
		private Sequence mBestSequence;
		private BeamSearch mBeam;
		
		public const string Start = "start";
		public const string Continue = "cont";
		public const string Other = "other";
		
		/// <summary>
		/// Creates a new name finder with the specified model.
		/// </summary>
		/// <param name="model">
		/// The model to be used to find names.
		/// </param>
		public MaximumEntropyNameFinder(SharpEntropy.IMaximumEntropyModel model) : this(model, new DefaultNameContextGenerator(10), 10)
		{
		}
		
		/// <summary>
		/// Creates a new name finder with the specified model and context generator.
		/// </summary>
		/// <param name="model">
		/// The model to be used to find names.
		/// </param>
		/// <param name="contextGenerator">
		/// The context generator to be used with this name finder.
		/// </param>
		public MaximumEntropyNameFinder(SharpEntropy.IMaximumEntropyModel model, INameContextGenerator contextGenerator) : this(model, contextGenerator, 10)
		{
		}
		
		/// <summary>
		/// Creates a new name finder with the specified model and context generator.
		/// </summary>
		/// <param name="model">
		/// The model to be used to find names.
		/// </param>
		/// <param name="contextGenerator">
		/// The context generator to be used with this name finder.
		/// </param>
		/// <param name="beamSize">
		/// The size of the beam to be used in decoding this model.
		/// </param>
		public MaximumEntropyNameFinder(SharpEntropy.IMaximumEntropyModel model, INameContextGenerator contextGenerator, int beamSize)
		{
			mModel = model;
			mContextGenerator = contextGenerator;
			mBeam = new NameBeamSearch(this, beamSize, contextGenerator, model, beamSize);
		}
		
		public virtual ArrayList Find(ArrayList tokens, IDictionary previousTags)
		{
			mBestSequence = mBeam.BestSequence(tokens, new object[]{previousTags});
			return new ArrayList(mBestSequence.Outcomes);
		}
		
		public virtual string[] Find(object[] tokens, IDictionary previousTags)
		{
			mBestSequence = mBeam.BestSequence(tokens, new object[]{previousTags});
			ArrayList outcomes = new ArrayList(mBestSequence.Outcomes);
			return (string[]) outcomes.ToArray(typeof(string));
		}
		
		/// <summary>
		/// This method determines wheter the outcome is valid for the preceding sequence.  
		/// This can be used to implement constraints on what sequences are valid.  
		/// </summary>
		/// <param name="outcome">
		/// The outcome.
		/// </param>
		/// <param name="sequence">
		/// The preceding sequence of outcome assignments. 
		/// </param>
		/// <returns>
		/// true is the outcome is valid for the sequence, false otherwise.
		/// </returns>
		protected internal virtual bool ValidOutcome(string outcome, Sequence sequence)
		{
			if (outcome == Continue)
			{
				string[] tags = sequence.Outcomes.ToArray();
				int lastTag = tags.Length - 1;
				if (lastTag == -1)
				{
					return false;
				}
				else if (tags[lastTag] == Other)
				{
					return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Implementation of the abstract beam search to allow the name finder to use the common beam search code. 
		/// </summary>
		private class NameBeamSearch : BeamSearch
		{
			private MaximumEntropyNameFinder mNameFinder;
						
			/// <summary>
			/// Creates a beam seach of the specified size using the specified model with the specified context generator.
			/// </summary>
			/// <param name="nameFinder">
			/// The associated MaximumEntropyNameFinder instance.
			/// </param>
			/// <param name="size">
			/// The size of the beam.
			/// </param>
			/// <param name="contextGenerator">
			/// The context generator used with the specified model.
			/// </param>
			/// <param name="model">
			/// The model used to determine names.
			/// </param>
			/// <param name="beamSize">
			/// The size of the beam to use in searching.
			/// </param>
			public NameBeamSearch(MaximumEntropyNameFinder nameFinder, int size, INameContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model, int beamSize) : base(size, contextGenerator, model, beamSize)
			{
				mNameFinder = nameFinder;
			}
			
			protected internal override bool ValidSequence(int index, ArrayList sequence, Sequence outcomeSequence, string outcome)
			{
				return mNameFinder.ValidOutcome(outcome, outcomeSequence);
			}
		}
		
		/// <summary>
		/// Populates the specified array with the probabilities of the last decoded sequence.  The
		/// sequence was determined based on the previous call to <code>chunk</code>.  The 
		/// specified array should be at least as large as the numbe of tokens in the previous call to <code>chunk</code>.
		/// </summary>
		/// <param name="probabilities">
		/// An array used to hold the probabilities of the last decoded sequence.
		/// </param>
		public virtual void GetProbabilities(double[] probabilities)
		{
			mBestSequence.GetProbabilities(probabilities);
		}
		
		/// <summary>
		/// Returns an array with the probabilities of the last decoded sequence.  The
		/// sequence was determined based on the previous call to <code>chunk</code>.
		/// </summary>
		/// <returns>
		/// An array with the same number of probabilities as tokens were sent to <code>chunk</code>
		/// when it was last called.   
		/// </returns>
		public virtual double[] GetProbabilities()
		{
			return mBestSequence.GetProbabilities();
		}
		
		private static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventReader, int iterations, int cutoff)
		{
			SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventReader, cutoff));
			return new SharpEntropy.GisModel(trainer);
		}
		
		public static SharpEntropy.GisModel TrainModel(string trainingFile)
		{
			return TrainModel(trainingFile, 100, 5);
		}

		public static SharpEntropy.GisModel TrainModel(string trainingFile, int iterations, int cutoff)
		{
			SharpEntropy.ITrainingEventReader eventReader = new NameFinderEventReader(new SharpEntropy.PlainTextByLineDataReader(new System.IO.StreamReader(trainingFile)));
			return Train(eventReader, iterations, cutoff);
		}
	}
}
