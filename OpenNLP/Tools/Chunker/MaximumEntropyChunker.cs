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
		/// <summary>
		/// The beam used to search for sequences of chunk tag assignments.
		/// </summary>
		protected internal Util.BeamSearch Beam { get; private set; }

	    /// <summary>
	    /// The model used to assign chunk tags to a sequence of tokens.
	    /// </summary>
	    protected internal SharpEntropy.IMaximumEntropyModel Model  { get; private set; }

        
        // Constructors ---------------

		/// <summary>Creates a chunker using the specified model</summary>
		/// <param name="model">The maximum entropy model for this chunker</param>
		public MaximumEntropyChunker(SharpEntropy.IMaximumEntropyModel model):
            this(model, new DefaultChunkerContextGenerator(), 10){}
		
		/// <summary>
		/// Creates a chunker using the specified model and context generator.
		/// </summary>
		/// <param name="model">The maximum entropy model for this chunker</param>
		/// <param name="contextGenerator">The context generator to be used by the specified model</param>
		public MaximumEntropyChunker(SharpEntropy.IMaximumEntropyModel model, IChunkerContextGenerator contextGenerator):
            this(model, contextGenerator, 10){}
		
		/// <summary>
		/// Creates a chunker using the specified model and context generator and decodes the
		/// model using a beam search of the specified size.
		/// </summary>
		/// <param name="model">The maximum entropy model for this chunker</param>
		/// <param name="contextGenerator">The context generator to be used by the specified model</param>
		/// <param name="beamSize">The size of the beam that should be used when decoding sequences</param>
		public MaximumEntropyChunker(SharpEntropy.IMaximumEntropyModel model, IChunkerContextGenerator contextGenerator, int beamSize)
		{
			Beam = new ChunkBeamSearch(this, beamSize, contextGenerator, model);
			Model = model;
		}
		
        
        // Methods ----------------

		/// <summary>Performs a chunking operation</summary>
		/// <param name="tokens">Object array of tokens</param>
		/// <param name="tags">string array of POS tags corresponding to the tokens in the object array</param>
		/// <returns>string array containing a value for each token, indicating the chunk that that token belongs to</returns>
		public virtual string[] Chunk(string[] tokens, string[] tags)
		{
			var bestSequence = Beam.BestSequence(tokens, new object[]{tags});
            return bestSequence.Outcomes.ToArray();
		}
		
		/// <summary>Gets a list of all the possible chunking tags</summary>
		/// <returns>String array, each entry containing a chunking tag</returns>
		public virtual string[] AllTags()
		{
			var tags = new string[Model.OutcomeCount];
			for (int currentTag = 0; currentTag < Model.OutcomeCount; currentTag++)
			{
				tags[currentTag] = Model.GetOutcomeName(currentTag);
			}
			return tags;
		}
		
        /// <summary>
		/// This method determines wheter the outcome is valid for the preceding sequence.  
		/// This can be used to implement constraints on what sequences are valid.  
		/// </summary>
		/// <param name="outcome">The outcome</param>
		/// <param name="sequence">The preceding sequence of outcomes assignments</param>
		/// <returns>true if the outcome is valid for the sequence, false otherwise</returns>
		protected internal virtual bool ValidOutcome(string outcome, Util.Sequence sequence)
		{
			return true;
		}
		
		/// <summary>
		/// This method determines wheter the outcome is valid for the preceeding sequence.  
		/// This can be used to implement constraints on what sequences are valid.  
		/// </summary>
		/// <param name="outcome">The outcome</param>
		/// <param name="sequence">The preceding sequence of outcomes assignments</param>
		/// <returns>true if the outcome is valid for the sequence, false otherwise</returns>
		protected internal virtual bool ValidOutcome(string outcome, string[] sequence) 
		{
			return true;
		}
        

        // Utilities ---------------

		/// <summary>
		/// Trains the chunker.
		/// Training file should be one word per line where each line consists of a
		/// space-delimited triple of "word pos outcome".
		/// Sentence breaks are indicated by blank lines.
		/// </summary>
		/// <param name="eventReader">The chunker event reader</param>
		/// <returns>Trained model</returns>
		public static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventReader)
		{
			return Train(eventReader, 100, 5);
		}

		/// <summary>
		/// Trains the chunker.
		/// Training file should be one word per line where each line consists of a
		/// space-delimited triple of "word pos outcome".
		/// Sentence breaks are indicated by blank lines.
		/// </summary>
		/// <param name="eventReader">The chunker event reader</param>
		/// <param name="iterations">The number of iterations to perform</param>
		/// <param name="cutoff">
		/// The number of times a predicate must be seen in order
		/// to be relevant for training.
		/// </param>
		/// <returns>Trained model</returns>
		public static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventReader, int iterations, int cutoff)
		{
			var trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventReader, cutoff));
			return new SharpEntropy.GisModel(trainer);
		}


        // Inner classes -----------

        /// <summary>
        /// This class implements the abstract BeamSearch class to allow for the chunker to use
        /// the common beam search code. 
        /// </summary>
        private class ChunkBeamSearch : Util.BeamSearch
        {
            private readonly MaximumEntropyChunker _maxentChunker;

            public ChunkBeamSearch(MaximumEntropyChunker maxentChunker, int size, IChunkerContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model)
                : base(size, contextGenerator, model)
            {
                _maxentChunker = maxentChunker;
            }

            protected internal override bool ValidSequence(int index, ArrayList inputSequence, Util.Sequence outcomesSequence, string outcome)
            {
                return _maxentChunker.ValidOutcome(outcome, outcomesSequence);
            }

            protected internal override bool ValidSequence(int index, object[] inputSequence, string[] outcomesSequence, string outcome)
            {
                return _maxentChunker.ValidOutcome(outcome, outcomesSequence);
            }
        }
	}
}
