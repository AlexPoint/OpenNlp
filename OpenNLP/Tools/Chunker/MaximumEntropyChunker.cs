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

//This file is based on the ChunkerME.java source file found in the
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

namespace OpenNLP.Tools.Chunker
{
	/// <summary>
	/// This class represents a maximum-entropy-based chunker.  Such a chunker can be used to
	/// find flat structures based on sequence inputs such as noun phrases or named entities.
	/// </summary>
	public class MaximumEntropyChunker : IChunker
	{		
		private Util.BeamSearch mBeam;
		private Util.Sequence mBestSequence;
		private SharpEntropy.IMaximumEntropyModel mModel; 

		/// <summary>
		/// The beam used to search for sequences of chunk tag assignments.
		/// </summary>
		protected internal Util.BeamSearch Beam
		{
			get
			{
				return mBeam;
			}
		}

		/// <summary>
		/// The model used to assign chunk tags to a sequence of tokens.
		/// </summary>
		protected internal SharpEntropy.IMaximumEntropyModel Model
		{
			get
			{
				return mModel;
			}
		}

		/// <summary>
		/// Creates a chunker using the specified model.
		/// </summary>
		/// <param name="model">
		/// The maximum entropy model for this chunker.
		/// </param>
		public MaximumEntropyChunker(SharpEntropy.IMaximumEntropyModel model):this(model, new DefaultChunkerContextGenerator(), 10)
		{
		}
		
		/// <summary>
		/// Creates a chunker using the specified model and context generator.
		/// </summary>
		/// <param name="model">
		/// The maximum entropy model for this chunker.
		/// </param>
		/// <param name="contextGenerator">
		/// The context generator to be used by the specified model.
		/// </param>
		public MaximumEntropyChunker(SharpEntropy.IMaximumEntropyModel model, IChunkerContextGenerator contextGenerator):this(model, contextGenerator, 10)
		{
		}
		
		/// <summary>
		/// Creates a chunker using the specified model and context generator and decodes the
		/// model using a beam search of the specified size.
		/// </summary>
		/// <param name="model">
		/// The maximum entropy model for this chunker.
		/// </param>
		/// <param name="contextGenerator">
		/// The context generator to be used by the specified model.
		/// </param>
		/// <param name="beamSize">
		/// The size of the beam that should be used when decoding sequences.
		/// </param>
		public MaximumEntropyChunker(SharpEntropy.IMaximumEntropyModel model, IChunkerContextGenerator contextGenerator, int beamSize)
		{
			mBeam = new ChunkBeamSearch(this, beamSize, contextGenerator, model);
			mModel = model;
		}
		
		/// <summary>
		/// Performs a chunking operation.
		/// </summary>
		/// <param name="tokens">
		/// ArrayList of tokens
		/// </param>
		/// <param name="tags">
		/// ArrayList of tags corresponding to the tokens
		/// </param>
		/// <returns>
		/// ArrayList of results, containing a value for each token, indicating the chunk that that token belongs to.
		/// </returns>
		public virtual ArrayList Chunk(ArrayList tokens, ArrayList tags)
		{
			mBestSequence = mBeam.BestSequence(tokens, new object[] { (string[]) tags.ToArray(typeof(string)) });
			return new ArrayList(mBestSequence.Outcomes);
		}
		
		/// <summary>
		/// Performs a chunking operation.
		/// </summary>
		/// <param name="tokens">
		/// Object array of tokens
		/// </param>
		/// <param name="tags">
		/// String array of POS tags corresponding to the tokens in the object array
		/// </param>
		/// <returns>
		/// String array containing a value for each token, indicating the chunk that that token belongs to.
		/// </returns>
		public virtual string[] Chunk(object[] tokens, string[] tags)
		{
			mBestSequence = mBeam.BestSequence(new ArrayList(tokens), new object[]{tags});
            return mBestSequence.Outcomes.ToArray();
		}
		
		/// <summary>
		/// Gets a list of all the possible chunking tags.
		/// </summary>
		/// <returns>
		/// String array, each entry containing a chunking tag.
		/// </returns>
		public virtual string[] AllTags()
		{
			string[] tags = new string[mModel.OutcomeCount];
			for (int currentTag = 0; currentTag < mModel.OutcomeCount; currentTag++)
			{
				tags[currentTag] = mModel.GetOutcomeName(currentTag);
			}
			return tags;
		}
		/// <summary>
		/// This method determines wheter the outcome is valid for the preceding sequence.  
		/// This can be used to implement constraints on what sequences are valid.  
		/// </summary>
		/// <param name="outcome">
		/// The outcome.
		/// </param>
		/// <param name="sequence">
		/// The preceding sequence of outcomes assignments. 
		/// </param>
		/// <returns>
		/// true if the outcome is valid for the sequence, false otherwise.
		/// </returns>
		protected internal virtual bool ValidOutcome(string outcome, Util.Sequence sequence)
		{
			return true;
		}
		
		/// <summary>
		/// This method determines wheter the outcome is valid for the preceeding sequence.  
		/// This can be used to implement constraints on what sequences are valid.  
		/// </summary>
		/// <param name="outcome">
		/// The outcome.
		/// </param>
		/// <param name="sequence">
		/// The preceding sequence of outcomes assignments. 
		/// </param>
		/// <returns>
		/// true if the outcome is valid for the sequence, false otherwise.
		/// </returns>
		protected internal virtual bool ValidOutcome(string outcome, string[] sequence) 
		{
			return true;
		}

		/// <summary>
		/// This class implements the abstract BeamSearch class to allow for the chunker to use
		/// the common beam search code. 
		/// </summary>
		private class ChunkBeamSearch : Util.BeamSearch
		{
			private MaximumEntropyChunker mMaxentChunker;
			
			public ChunkBeamSearch(MaximumEntropyChunker maxentChunker, int size, IChunkerContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model):base(size, contextGenerator, model)
			{
				mMaxentChunker = maxentChunker;
			}
			
			protected internal override bool ValidSequence(int index, ArrayList inputSequence, Util.Sequence outcomesSequence, string outcome)
			{
				return mMaxentChunker.ValidOutcome(outcome, outcomesSequence);
			}
    
			protected internal override bool ValidSequence(int index, object[] inputSequence, string[] outcomesSequence, string outcome) 
			{
				return mMaxentChunker.ValidOutcome(outcome, outcomesSequence);
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
		
		/// <summary>
		/// Trains the chunker.  Training file should be one word per line where each line consists of a
		/// space-delimited triple of "word pos outcome".  Sentence breaks are indicated by blank lines.
		/// </summary>
		/// <param name="eventReader">
		/// The chunker event reader.
		/// </param>
		/// <returns>
		/// Trained model.
		/// </returns>
		public static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventReader)
		{
			return Train(eventReader, 100, 5);
		}

		/// <summary>
		/// Trains the chunker.  Training file should be one word per line where each line consists of a
		/// space-delimited triple of "word pos outcome".  Sentence breaks are indicated by blank lines.
		/// </summary>
		/// <param name="eventReader">
		/// The chunker event reader.
		/// </param>
		/// <param name="iterations">
		/// The number of iterations to perform.
		/// </param>
		/// <param name="cutoff">
		/// The number of times a predicate must be seen in order
		/// to be relevant for training.
		/// </param>
		/// <returns>
		/// Trained model.
		/// </returns>
		public static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventReader, int iterations, int cutoff)
		{
			SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventReader, cutoff));
			return new SharpEntropy.GisModel(trainer);
		}
	}
}
