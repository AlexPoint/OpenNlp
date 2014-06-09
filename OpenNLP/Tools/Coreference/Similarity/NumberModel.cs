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

//This file is based on the NumberModel.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Similarity
{
	/// <summary> 
    /// Class which models the number of particular mentions and the entities made up of mentions. 
    /// </summary>
	public class NumberModel : ITestNumberModel, ITrainSimilarityModel
	{
		virtual public void SetExtents(Context[] extents)
		{
            Util.HashList<int, Context> entities = new Util.HashList<int, Context>();
            List<Context> singletons = new List<Context>();
			for (int extentIndex = 0; extentIndex < extents.Length; extentIndex++)
			{
				Context currentExtent = extents[extentIndex];
				//System.err.println("NumberModel.setExtents: ec("+ec.getId()+") "+ec.toText());
				if (currentExtent.Id != -1)
				{
					entities.Put(currentExtent.Id, currentExtent);
				}
				else
				{
					singletons.Add(currentExtent);
				}
			}
            List<Context> singles = new List<Context>();
            List<Context> plurals = new List<Context>();
			// coref entities
			foreach (int key in entities.Keys)
            {
				List<Context> entityContexts = entities[key];
				NumberEnum number = GetNumber(entityContexts);
				if (number == NumberEnum.Singular)
				{
					singles.AddRange(entityContexts);
				}
				else if (number == NumberEnum.Plural)
				{
					plurals.AddRange(entityContexts);
				}
			}
			// non-coref entities.
			foreach (Context currentContext in singletons)
            {
				NumberEnum number = GetNumber(currentContext);
				if (number == NumberEnum.Singular)
				{
					singles.Add(currentContext);
				}
				else if (number == NumberEnum.Plural)
				{
					plurals.Add(currentContext);
				}
			}
			
			foreach (Context currentContext in singles)
            {
				AddEvent(NumberEnum.Singular.ToString(), currentContext);
			}
			
            foreach (Context currentContext in plurals)
            {
				AddEvent(NumberEnum.Plural.ToString(), currentContext);
			}
			
		}

		public virtual int SingularIndex
		{
			get
			{
				return mSingularIndex;
			}
		}

		public virtual int PluralIndex
		{
			get
			{
				return mPluralIndex;
			}
		}
		
		private string mModelName;
		private string mModelExtension = ".nbin";
        private SharpEntropy.IMaximumEntropyModel mTestModel;
		private List<SharpEntropy.TrainingEvent> mEvents;
		
		private int mSingularIndex;
		private int mPluralIndex;
		
		public static ITestNumberModel TestModel(string name)
		{
			NumberModel numberModel = new NumberModel(name, false);
			return numberModel;
		}
		
		public static ITrainSimilarityModel TrainModel(string modelName)
		{
			NumberModel numberModel = new NumberModel(modelName, true);
			return numberModel;
		}
		
		private NumberModel(string modelName, bool train)
		{
			mModelName = modelName;
			if (train)
			{
				mEvents = new List<SharpEntropy.TrainingEvent>();
			}
			else
			{
				mTestModel = new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelName + mModelExtension));

				mSingularIndex = mTestModel.GetOutcomeIndex(NumberEnum.Singular.ToString());
                mPluralIndex = mTestModel.GetOutcomeIndex(NumberEnum.Plural.ToString());
			}
		}
		
		private List<string> GetFeatures(Context nounPhrase)
		{
            List<string> features = new List<string>();
			features.Add("default");
			object[] nounPhraseTokens = nounPhrase.Tokens;
			for (int tokenIndex = 0, tokenLength = nounPhraseTokens.Length - 1; tokenIndex < tokenLength; tokenIndex++)
			{
				features.Add("mw=" + nounPhraseTokens[tokenIndex].ToString());
			}
			features.Add("hw=" + nounPhrase.HeadTokenText.ToLower());
			features.Add("ht=" + nounPhrase.HeadTokenTag);
			return features;
		}
		
		private void AddEvent(string outcome, Context nounPhrase)
		{
			List<string> features = GetFeatures(nounPhrase);
            mEvents.Add(new SharpEntropy.TrainingEvent(outcome, features.ToArray()));
		}
		
		public virtual NumberEnum GetNumber(Context context)
		{
			if (Linker.SingularPronounPattern.IsMatch(context.HeadTokenText))
			{
				return NumberEnum.Singular;
			}
            else if (Linker.PluralPronounPattern.IsMatch(context.HeadTokenText))
			{
				return NumberEnum.Plural;
			}
			else
			{
				return NumberEnum.Unknown;
			}
		}
		
		private NumberEnum GetNumber(List<Context> entity)
		{
			foreach (Context currentContext in entity)
            {
				NumberEnum number = GetNumber(currentContext);
				if (number != NumberEnum.Unknown)
				{
					return number;
				}
			}
			return NumberEnum.Unknown;
		}
		
		public virtual double[] NumberDistribution(Context context)
		{
			List<string> features = GetFeatures(context);
			return mTestModel.Evaluate(features.ToArray());
		}
		
		public virtual void TrainModel()
		{
            SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
            trainer.TrainModel(new Util.CollectionEventReader(mEvents), 100, 10);
            new SharpEntropy.IO.BinaryGisModelWriter().Persist(new SharpEntropy.GisModel(trainer), mModelName + mModelExtension);
		}
	}
}