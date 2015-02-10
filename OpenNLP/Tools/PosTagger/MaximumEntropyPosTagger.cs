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

//This file is based on the POSTaggerME.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2002 Jason Baldridge and Gann Bierner
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenNLP.Tools.PosTagger
{
	/// <summary>
	/// A part-of-speech tagger that uses maximum entropy.  Trys to predict whether
	/// words are nouns, verbs, or any of 70 other POS tags depending on their
	/// surrounding context.
	/// </summary>
	public class MaximumEntropyPosTagger : IPosTagger
	{
        private const int DefaultBeamSize = 3;

		public virtual string NegativeOutcome
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Returns the number of different tags predicted by this model.
		/// </summary>
		/// <returns>
		/// the number of different tags predicted by this model.
		/// </returns>
		public virtual int NumTags
		{
			get
			{
				return this.PosModel.OutcomeCount;
			}
		}
		
        /// <summary>
        /// The maximum entropy model to use to evaluate contexts.
        /// </summary>
		protected internal SharpEntropy.IMaximumEntropyModel PosModel { get; set; }

        /// <summary>
        /// The feature context generator.
        /// </summary>
		protected internal IPosContextGenerator ContextGenerator { get; set; }

        /// <summary>
        ///Tag dictionary used for restricting words to a fixed set of tags.
        ///</summary>
		protected internal PosLookupList TagDictionary { get; set; }

	    /// <summary>
	    /// Says whether a filter should be used to check whether a tag assignment
	    /// is to a word outside of a closed class.
	    /// </summary>
	    protected internal bool UseClosedClassTagsFilter { get; set; }

        /// <summary>
        /// The size of the beam to be used in determining the best sequence of pos tags.
        /// </summary>
	    protected internal int BeamSize { get; set; }

		/// <summary>
		/// The search object used for search multiple sequences of tags.
		/// </summary>
		internal Util.BeamSearch Beam;
		
        
        // Constructors ------------------------

		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model) : 
            this(model, new DefaultPosContextGenerator()){ }
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model, PosLookupList dictionary) : 
            this(DefaultBeamSize, model, new DefaultPosContextGenerator(), dictionary){ }
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model, IPosContextGenerator contextGenerator) : 
            this(DefaultBeamSize, model, contextGenerator, null){ }
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model, IPosContextGenerator contextGenerator, PosLookupList dictionary):
            this(DefaultBeamSize, model, contextGenerator, dictionary){ }
		
        public MaximumEntropyPosTagger(int beamSize, SharpEntropy.IMaximumEntropyModel model, IPosContextGenerator contextGenerator, PosLookupList dictionary)
		{
		    UseClosedClassTagsFilter = false;
		    this.BeamSize = beamSize;
			this.PosModel = model;
			this.ContextGenerator = contextGenerator;
			Beam = new PosBeamSearch(this, this.BeamSize, contextGenerator, model);
			this.TagDictionary = dictionary;
		}


        // Methods ----------------------------

        /// <summary>
        /// Returns a list of all the possible POS tags predicted by this model.
        /// </summary>
        /// <returns>string array of the possible POS tags</returns>
        public virtual string[] AllTags()
        {
            var tags = new string[this.PosModel.OutcomeCount];
            for (int currentTag = 0; currentTag < this.PosModel.OutcomeCount; currentTag++)
            {
                tags[currentTag] = this.PosModel.GetOutcomeName(currentTag);
            }
            return tags;
        }

		public virtual SharpEntropy.ITrainingEventReader GetEventReader(TextReader reader)
		{
			return new PosEventReader(reader, this.ContextGenerator);
		}
		
        /// <summary>
        /// Associates tags to a collection of tokens.
        /// This collection of tokens should represent a sentence.
        /// </summary>
        /// <param name="tokens">The collection of tokens as strings</param>
        /// <returns>The collection of tags as strings</returns>
		public virtual string[] Tag(string[] tokens)
		{
            var bestSequence = Beam.BestSequence(tokens, null);
            return bestSequence.Outcomes.ToArray();
		}
		
        /// <summary>
        /// Tags words in a given sentence
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
		public virtual List<TaggedWord> TagSentence(string sentence)
		{
			var tokens = sentence.Split();
			var tags = Tag(tokens);
		    var taggedWords = Enumerable.Range(0, tags.Length)
		        .Select(i => new TaggedWord(tokens[i], tags[i], i))
		        .ToList();

		    return taggedWords;
		}
		
		public virtual void LocalEvaluate(SharpEntropy.IMaximumEntropyModel posModel, StreamReader reader, out double accuracy, out double sentenceAccuracy)
		{
			this.PosModel = posModel;
			float total = 0, correct = 0, sentences = 0, sentencesCorrect = 0;
			
			var sentenceReader = new StreamReader(reader.BaseStream, System.Text.Encoding.UTF7);
			string line;
			
			while ((object) (line = sentenceReader.ReadLine()) != null)
			{
				sentences++;
				var annotatedPair = PosEventReader.ConvertAnnotatedString(line);
				var words = annotatedPair.Item1;
				var outcomes = annotatedPair.Item2;
				var tags = new ArrayList(Beam.BestSequence(words.ToArray(), null).Outcomes);
				
				int count = 0;
				bool isSentenceOk = true;
				for (IEnumerator tagIndex = tags.GetEnumerator(); tagIndex.MoveNext(); count++)
				{
					total++;
					var tag = (string) tagIndex.Current;
					if (tag == (string)outcomes[count])
					{
						correct++;
					}
					else
					{
						isSentenceOk = false;
					}
				}
				if (isSentenceOk)
				{
					sentencesCorrect++;
				}
			}
			
			accuracy = correct / total;
			sentenceAccuracy = sentencesCorrect / sentences;
		}
		
		public virtual string[] GetOrderedTags(List<string> words, List<string> tags, int index)
		{
			return GetOrderedTags(words, tags, index, null);
		}
		
		public virtual string[] GetOrderedTags(List<string> words, List<string> tags, int index, double[] tagProbabilities)
		{
			double[] probabilities = this.PosModel.Evaluate(this.ContextGenerator.GetContext(index, words.ToArray(), tags.ToArray(), null));
			var orderedTags = new string[probabilities.Length];
			for (int currentProbability = 0; currentProbability < probabilities.Length; currentProbability++)
			{
				int max = 0;
				for (int tagIndex = 1; tagIndex < probabilities.Length; tagIndex++)
				{
					if (probabilities[tagIndex] > probabilities[max])
					{
						max = tagIndex;
					}
				}
				orderedTags[currentProbability] = this.PosModel.GetOutcomeName(max);
				if (tagProbabilities != null)
				{
					tagProbabilities[currentProbability] = probabilities[max];
				}
				probabilities[max] = 0;
			}
			return orderedTags;
		}
		

        // Utilities ---------------------------------

		/// <summary>
		/// Trains a POS tag maximum entropy model.
		/// </summary>
		/// <param name="eventStream">Stream of training events</param>
		/// <param name="iterations">number of training iterations to perform</param>
		/// <param name="cut">cutoff value to use for the data indexer</param>
		/// <returns>Trained GIS model</returns>
		public static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventStream, int iterations, int cut)
		{
			var trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventStream, cut));
			return new SharpEntropy.GisModel(trainer);
		}

		/// <summary>
		/// Trains a POS tag maximum entropy model
		/// </summary>
		/// <param name="trainingFile">filepath to the training data</param>
		/// <param name="iterations">number of training iterations to perform</param>
		/// <param name="cutoff">Cutoff value to use for the data indexer</param>
		/// <returns>Trained GIS model</returns>
		public static SharpEntropy.GisModel TrainModel(string trainingFile, int iterations, int cutoff)
		{
			SharpEntropy.ITrainingEventReader eventReader = new PosEventReader(new StreamReader(trainingFile));
			return Train(eventReader, iterations, cutoff);
		}


        // Inner classes ---------------------------
        private class PosBeamSearch : Util.BeamSearch
        {
            private readonly MaximumEntropyPosTagger _maxentPosTagger;


            // Constructors ---------------------

            public PosBeamSearch(MaximumEntropyPosTagger posTagger, int size, IPosContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model) :
                base(size, contextGenerator, model)
            {
                _maxentPosTagger = posTagger;
            }

            public PosBeamSearch(MaximumEntropyPosTagger posTagger, int size, IPosContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model, int cacheSize) :
                base(size, contextGenerator, model, cacheSize)
            {
                _maxentPosTagger = posTagger;
            }


            // Methods ---------------------------

            protected internal override bool ValidSequence(int index, object[] inputSequence, string[] outcomesSequence, string outcome)
            {
                if (_maxentPosTagger.TagDictionary == null)
                {
                    return true;
                }
                else
                {
                    string[] tags = _maxentPosTagger.TagDictionary.GetTags(inputSequence[index].ToString());
                    if (tags == null)
                    {
                        return true;
                    }
                    else
                    {
                        return new ArrayList(tags).Contains(outcome);
                    }
                }
            }
            protected internal override bool ValidSequence(int index, ArrayList inputSequence, Util.Sequence outcomesSequence, string outcome)
            {
                if (_maxentPosTagger.TagDictionary == null)
                {
                    return true;
                }
                else
                {
                    string[] tags = _maxentPosTagger.TagDictionary.GetTags(inputSequence[index].ToString());
                    if (tags == null)
                    {
                        return true;
                    }
                    else
                    {
                        return new ArrayList(tags).Contains(outcome);
                    }
                }
            }
        }
	}
}
