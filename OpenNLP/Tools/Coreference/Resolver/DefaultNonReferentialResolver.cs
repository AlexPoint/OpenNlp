//Copyright (C) 2006 Richard J. Northedge
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

//This file is based on the DefaultNonReferentialResolver.java source file found in the
//original java implementation of OpenNLP.  

using System;
using System.Collections.Generic;

namespace OpenNLP.Tools.Coreference.Resolver
{
    /// <summary> 
    /// Default implementation of the {@link INonReferentialResolver} interface.
    /// </summary>
	public class DefaultNonReferentialResolver : INonReferentialResolver
	{
        private SharpEntropy.IMaximumEntropyModel mModel;
		private List<SharpEntropy.TrainingEvent> mEvents;
		private bool mDebugOn = false;
		private ResolverMode mResolverMode;
		private string mModelName;
		private string mModelExtension = ".nbin";
		private int mNonReferentialIndex;
		
		public DefaultNonReferentialResolver(string projectName, string name, ResolverMode mode)
		{
			mResolverMode = mode;
            mModelName = projectName + "\\" + name + "_nr";
			if (mode == ResolverMode.Train)
			{
                mEvents = new List<SharpEntropy.TrainingEvent>();
			}
			else if (mode == ResolverMode.Test)
			{
				
                mModel = new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(mModelName + mModelExtension));
				mNonReferentialIndex = mModel.GetOutcomeIndex(MaximumEntropyResolver.Same);
			}
			else
			{
				throw new ArgumentException("unexpected mode " + mode);
			}
		}

        public virtual double GetNonReferentialProbability(Mention.MentionContext mention)
		{
			List<string> features = GetFeatures(mention);
			double probability = mModel.Evaluate(features.ToArray())[mNonReferentialIndex];
			if (mDebugOn)
			{
				System.Console.Error.WriteLine(this + " " + mention.ToText() + " ->  null " + probability + " " + string.Join(",", features.ToArray()));
			}
			return probability;
		}

        public virtual void AddEvent(Mention.MentionContext context)
		{
            List<string> features = GetFeatures(context);
			if (context.Id == -1)
			{
                mEvents.Add(new SharpEntropy.TrainingEvent(MaximumEntropyResolver.Same, features.ToArray()));
			}
			else
			{
                mEvents.Add(new SharpEntropy.TrainingEvent(MaximumEntropyResolver.Diff, features.ToArray()));
			}
		}

        protected internal virtual List<string> GetFeatures(Mention.MentionContext mention)
		{
            List<string> features = new List<string>();
			features.Add(MaximumEntropyResolver.Default);
			features.AddRange(GetNonReferentialFeatures(mention));
			return features;
		}
		
		/// <summary>
        /// Returns a list of features used to predict whether the specified mention is non-referential.
        /// </summary>
		/// <param name="mention">
        /// The mention under considereation.
		/// </param>
		/// <returns> 
        /// a list of featues used to predict whether the specified mention is non-referential.
		/// </returns>
        protected internal virtual List<string> GetNonReferentialFeatures(Mention.MentionContext mention)
		{
            List<string> features = new List<string>();
            Mention.IParse[] mentionTokens = mention.TokenParses;
			//System.err.println("getNonReferentialFeatures: mention has "+mtokens.length+" tokens");
			for (int tokenIndex = 0; tokenIndex <= mention.HeadTokenIndex; tokenIndex++)
			{
                Mention.IParse token = mentionTokens[tokenIndex];
                List<string> wordFeatureList = MaximumEntropyResolver.GetWordFeatures(token);
				for (int wordFeatureIndex = 0; wordFeatureIndex < wordFeatureList.Count; wordFeatureIndex++)
				{
					features.Add("nr" + (wordFeatureList[wordFeatureIndex]));
				}
			}
			features.AddRange(MaximumEntropyResolver.GetContextFeatures(mention));
			return features;
		}
		
		public virtual void Train()
		{
			if (ResolverMode.Train == mResolverMode)
			{
				System.Console.Error.WriteLine(this + " referential");

				if (mDebugOn)
				{
					System.IO.StreamWriter writer = new System.IO.StreamWriter(mModelName + ".events", false, System.Text.Encoding.Default);
					foreach (SharpEntropy.TrainingEvent trainingEvent in mEvents)
					{
						writer.Write(trainingEvent.ToString() + "\n");
					}
					writer.Close();
				}

                SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
                trainer.TrainModel(new Util.CollectionEventReader(mEvents), 100, 10);
                new SharpEntropy.IO.BinaryGisModelWriter().Persist(new SharpEntropy.GisModel(trainer), mModelName + mModelExtension);
			}
		}
	}
}