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

namespace OpenNLP.Tools.PosTagger
{
	/// <summary>
	/// A part-of-speech tagger that uses maximum entropy.  Trys to predict whether
	/// words are nouns, verbs, or any of 70 other POS tags depending on their
	/// surrounding context.
	/// </summary>
	public class MaximumEntropyPosTagger : IPosTagger
	{
		/// <summary>
		/// The maximum entropy model to use to evaluate contexts.
		/// </summary>
		private SharpEntropy.IMaximumEntropyModel mPosModel;

		/// <summary>
		/// The feature context generator.
		/// </summary>
		private IPosContextGenerator mContextGenerator;

		/// <summary>
		///Tag dictionary used for restricting words to a fixed set of tags.
		///</summary>
		private PosLookupList mDictionary;

		/// <summary>
		/// Says whether a filter should be used to check whether a tag assignment
		/// is to a word outside of a closed class.
		/// </summary>
		private bool mUseClosedClassTagsFilter = false;

		private const int mDefaultBeamSize = 3;
		
		/// <summary>
		/// The size of the beam to be used in determining the best sequence of pos tags.
		/// </summary>
		private int mBeamSize;

		private Util.Sequence mBestSequence;

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
				return mPosModel.OutcomeCount;
			}
		}
		
        /// <summary>
        /// Returns a list of all the possible POS tags predicted by this model.
        /// </summary>
        /// <returns>
        /// String array of the possible POS tags.
        /// </returns>
		public virtual string[] AllTags()
		{
			string[] tags = new string[mPosModel.OutcomeCount];
			for (int currentTag = 0; currentTag < mPosModel.OutcomeCount; currentTag++)
			{
				tags[currentTag] = mPosModel.GetOutcomeName(currentTag);
			}
			return tags;
		}

		protected internal SharpEntropy.IMaximumEntropyModel PosModel
		{
			get
			{
				return mPosModel;
			}
			set
			{
				mPosModel = value;
			}
		}

		protected internal IPosContextGenerator ContextGenerator
		{
			get
			{
				return mContextGenerator;
			}
			set
			{
				mContextGenerator = value;
			}
		}

		protected internal PosLookupList TagDictionary
		{
			get
			{
				return mDictionary;
			}
			set
			{
				mDictionary = value;
			}
		}

		/// <summary>
		/// Says whether a filter should be used to check whether a tag assignment
		/// is to a word outside of a closed class.
		/// </summary>
		protected internal bool UseClosedClassTagsFilter
		{
			get
			{
				return mUseClosedClassTagsFilter;
			}
			set
			{
				mUseClosedClassTagsFilter = value;
			}
		}

		protected internal int BeamSize
		{
			get
			{
				return mBeamSize;
			}
			set
			{
				mBeamSize = value;
			}
		}

		/// <summary>
		/// The search object used for search multiple sequences of tags.
		/// </summary>
		internal Util.BeamSearch Beam;
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model) : this(model, new DefaultPosContextGenerator())
		{
		}
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model, PosLookupList dictionary) : this(mDefaultBeamSize, model, new DefaultPosContextGenerator(), dictionary)
		{
		}
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model, IPosContextGenerator contextGenerator) : this(mDefaultBeamSize, model, contextGenerator, null)
		{
		}
		
		public MaximumEntropyPosTagger(SharpEntropy.IMaximumEntropyModel model, IPosContextGenerator contextGenerator, PosLookupList dictionary) : this(mDefaultBeamSize, model, contextGenerator, dictionary)
		{
		}
		
		public MaximumEntropyPosTagger(int beamSize, SharpEntropy.IMaximumEntropyModel model, IPosContextGenerator contextGenerator, PosLookupList dictionary)
		{
			mBeamSize = beamSize;
			mPosModel = model;
			mContextGenerator = contextGenerator;
			Beam = new PosBeamSearch(this, mBeamSize, contextGenerator, model);
			mDictionary = dictionary;
		}
		
		public virtual SharpEntropy.ITrainingEventReader GetEventReader(System.IO.TextReader reader)
		{
			return new PosEventReader(reader, mContextGenerator);
		}
		
		public virtual ArrayList Tag(ArrayList tokens)
		{
			mBestSequence = Beam.BestSequence(tokens, null);
			return new ArrayList(mBestSequence.Outcomes);
		}
		
		public virtual string[] Tag(string[] tokens)
		{
            mBestSequence = Beam.BestSequence(new ArrayList(tokens), null);
            return mBestSequence.Outcomes.ToArray();
		}
		
		public virtual void GetProbabilities(double[] probabilities)
		{
			mBestSequence.GetProbabilities(probabilities);
		}
		
		public virtual double[] GetProbabilities()
		{
			return mBestSequence.GetProbabilities();
		}
		
		public virtual string TagSentence(string sentence)
		{
			ArrayList tokens = new ArrayList(sentence.Split());
			ArrayList tags = Tag(tokens);
			System.Text.StringBuilder tagBuffer = new System.Text.StringBuilder();
			for (int currentTag = 0; currentTag < tags.Count; currentTag++)
			{
				tagBuffer.Append(tokens[currentTag] + "/" + tags[currentTag] + " ");
			}
			return tagBuffer.ToString().Trim();
		}
		
		public virtual void LocalEvaluate(SharpEntropy.IMaximumEntropyModel posModel, System.IO.StreamReader reader, out double accuracy, out double sentenceAccuracy)
		{
			mPosModel = posModel;
			float total = 0, correct = 0, sentences = 0, sentencesCorrect = 0;
			
			System.IO.StreamReader sentenceReader = new System.IO.StreamReader(reader.BaseStream, System.Text.Encoding.UTF7);
			string line;
			
			while ((object) (line = sentenceReader.ReadLine()) != null)
			{
				sentences++;
				Util.Pair<ArrayList, ArrayList> annotatedPair = PosEventReader.ConvertAnnotatedString(line);
				ArrayList words = annotatedPair.FirstValue;
				ArrayList outcomes = annotatedPair.SecondValue;
				ArrayList tags = new ArrayList(Beam.BestSequence(words, null).Outcomes);
				
				int count = 0;
				bool isSentenceOK = true;
				for (System.Collections.IEnumerator tagIndex = tags.GetEnumerator(); tagIndex.MoveNext(); count++)
				{
					total++;
					string tag = (string) tagIndex.Current;
					if (tag == (string)outcomes[count])
					{
						correct++;
					}
					else
					{
						isSentenceOK = false;
					}
				}
				if (isSentenceOK)
				{
					sentencesCorrect++;
				}
			}
			
			accuracy = correct / total;
			sentenceAccuracy = sentencesCorrect / sentences;
		}
		
		private class PosBeamSearch : Util.BeamSearch
		{
			private MaximumEntropyPosTagger mMaxentPosTagger;
			
			public PosBeamSearch(MaximumEntropyPosTagger posTagger, int size, IPosContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model) : base(size, contextGenerator, model)
			{
				mMaxentPosTagger = posTagger;
			}
			
			public PosBeamSearch(MaximumEntropyPosTagger posTagger, int size, IPosContextGenerator contextGenerator, SharpEntropy.IMaximumEntropyModel model, int cacheSize) : base(size, contextGenerator, model, cacheSize)
			{
				mMaxentPosTagger = posTagger;
			}

			protected internal override bool ValidSequence(int index, object[] inputSequence, string[] outcomesSequence, string outcome) 
			{
				if (mMaxentPosTagger.TagDictionary == null) 
				{
					return true;
				}
				else 
				{
					string[] tags = mMaxentPosTagger.TagDictionary.GetTags(inputSequence[index].ToString());
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
				if (mMaxentPosTagger.mDictionary == null)
				{
					return true;
				}
				else
				{
					string[] tags = mMaxentPosTagger.mDictionary.GetTags(inputSequence[index].ToString());
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
		
		public virtual string[] GetOrderedTags(ArrayList words, ArrayList tags, int index)
		{
			return GetOrderedTags(words, tags, index, null);
		}
		
		public virtual string[] GetOrderedTags(ArrayList words, ArrayList tags, int index, double[] tagProbabilities)
		{
			double[] probabilities = mPosModel.Evaluate(mContextGenerator.GetContext(index, words.ToArray(), (string[]) tags.ToArray(typeof(string)), null));
			string[] orderedTags = new string[probabilities.Length];
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
				orderedTags[currentProbability] = mPosModel.GetOutcomeName(max);
				if (tagProbabilities != null)
				{
					tagProbabilities[currentProbability] = probabilities[max];
				}
				probabilities[max] = 0;
			}
			return orderedTags;
		}
		
		/// <summary>
		/// Trains a POS tag maximum entropy model.
		/// </summary>
		/// <param name="eventStream">
		/// Stream of training events
		/// </param>
		/// <param name="iterations">
		/// number of training iterations to perform.
		/// </param>
		/// <param name="cut">
		/// cutoff value to use for the data indexer.
		/// </param>
		/// <returns>
		/// Trained GIS model.
		/// </returns>
		public static SharpEntropy.GisModel Train(SharpEntropy.ITrainingEventReader eventStream, int iterations, int cut)
		{
			SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
			trainer.TrainModel(iterations, new SharpEntropy.TwoPassDataIndexer(eventStream, cut));
			return new SharpEntropy.GisModel(trainer);
		}
		
		/// <summary>
		/// Trains a POS tag maximum entropy model.
		/// </summary>
		/// <param name="trainingFile">
		/// filepath to the training data.
		/// </param>
		/// <returns>
		/// Trained GIS model.
		/// </returns>
		public static SharpEntropy.GisModel TrainModel(string trainingFile)
		{
			return TrainModel(trainingFile, 100, 5);
		}

		/// <summary>
		/// Trains a POS tag maximum entropy model.
		/// </summary>
		/// <param name="trainingFile">
		/// filepath to the training data.
		/// </param>
		/// <param name="iterations">
		/// number of training iterations to perform.
		/// </param>
		/// <param name="cutoff">
		/// Cutoff value to use for the data indexer.
		/// </param>
		/// <returns>
		/// Trained GIS model.
		/// </returns>
		public static SharpEntropy.GisModel TrainModel(string trainingFile, int iterations, int cutoff)
		{
			SharpEntropy.ITrainingEventReader eventReader = new PosEventReader(new System.IO.StreamReader(trainingFile));
			return Train(eventReader, iterations, cutoff);
		}
	}
}
