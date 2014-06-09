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

//This file is based on the GenderModel.java source file found in the
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
using System.IO;
using System.Collections.Generic;

namespace OpenNLP.Tools.Coreference.Similarity
{
	/// <summary>
    /// Class which models the gender of a particular mentions and entities made up of mentions.
    /// </summary>
	/// <author> 
	/// Tom Morton
	/// </author>
	public class GenderModel : ITestGenderModel, ITrainSimilarityModel
	{
        private int mMaleIndex;
        private int mFemaleIndex;
        private int mNeuterIndex;

        private string mModelName;
        private string mModelExtension = ".nbin";
        private SharpEntropy.IMaximumEntropyModel mTestModel;
        private List<SharpEntropy.TrainingEvent> mEvents;
        private bool mDebugOn = true;

        private Util.Set<string> mMaleNames;
        private Util.Set<string> mFemaleNames;

        private GenderModel(string modelName, bool train)
        {
            mModelName = modelName;
            mMaleNames = ReadNames(modelName + ".mal");
            mFemaleNames = ReadNames(modelName + ".fem");
            if (train)
            {
                mEvents = new List<SharpEntropy.TrainingEvent>();
            }
            else
            {
                mTestModel = new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelName + mModelExtension));
                
                mMaleIndex = mTestModel.GetOutcomeIndex(GenderEnum.Male.ToString());
                mFemaleIndex = mTestModel.GetOutcomeIndex(GenderEnum.Female.ToString());
                mNeuterIndex = mTestModel.GetOutcomeIndex(GenderEnum.Neuter.ToString());
            }
        }

        #region ITestGenderModel Members

        public virtual int MaleIndex
        {
            get
            {
                return mMaleIndex;
            }
        }

        public virtual int FemaleIndex
        {
            get
            {
                return mFemaleIndex;
            }
        }

        public virtual int NeuterIndex
        {
            get
            {
                return mNeuterIndex;
            }
        }

        public virtual double[] GenderDistribution(Context nounPhrase)
        {
            List<string> features = GetFeatures(nounPhrase);
            if (mDebugOn)
            {
                //System.err.println("GenderModel.genderDistribution: "+features);
            }
            return mTestModel.Evaluate(features.ToArray());
        }

        #endregion

        #region ITrainSimilarityModel Members

        virtual public void SetExtents(Context[] extents)
        {
            Util.HashList<int, Context> entities = new Util.HashList<int, Context>();
            List<Context> singletons = new List<Context>();
            for (int currentExtent = 0; currentExtent < extents.Length; currentExtent++)
            {
                Context extent = extents[currentExtent];
                //System.err.println("GenderModel.setExtents: ec("+ec.getId()+") "+ec.toText());
                if (extent.Id != -1)
                {
                    entities.Put(extent.Id, extent);
                }
                else
                {
                    singletons.Add(extent);
                }
            }
            List<Context> males = new List<Context>();
            List<Context> females = new List<Context>();
            List<Context> eunuches = new List<Context>();
            //coref entities
            foreach (int key in entities.Keys)
           {
                List<Context> entityContexts = entities[key];
                GenderEnum gender = GetGender(entityContexts);
                if (gender != null)
                {
                    if (gender == GenderEnum.Male)
                    {
                        males.AddRange(entityContexts);
                    }
                    else if (gender == GenderEnum.Female)
                    {
                       females.AddRange(entityContexts);
                    }
                    else if (gender == GenderEnum.Neuter)
                    {
                        eunuches.AddRange(entityContexts);
                    }
                }
            }
            //non-coref entities
            foreach (Context entityContext in singletons)
            {
                GenderEnum gender = GetGender(entityContext);
                if (gender == GenderEnum.Male)
                {
                    males.Add(entityContext);
                }
                else if (gender == GenderEnum.Female)
                {
                    females.Add(entityContext);
                }
                else if (gender == GenderEnum.Neuter)
                {
                    eunuches.Add(entityContext);
                }
            }
            
            foreach (Context entityContext in males)
            {
                AddEvent(GenderEnum.Male.ToString(), entityContext);
            }
            
            foreach (Context entityContext in females)
            {
                AddEvent(GenderEnum.Female.ToString(), entityContext);
            }
            
            foreach (Context entityContext in eunuches)
            {
                AddEvent(GenderEnum.Neuter.ToString(), entityContext);
            }
        }

        public virtual void TrainModel()
        {
            if (mDebugOn)
            {
                StreamWriter writer = new StreamWriter(mModelName + ".events", false, System.Text.Encoding.Default);
               foreach (SharpEntropy.TrainingEvent currentEvent in mEvents)
                {
                    writer.Write(currentEvent.ToString() + "\n");
                }
                writer.Close();
            }

            SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
            trainer.Smoothing = true;
            trainer.TrainModel(new Util.CollectionEventReader(mEvents));
            new SharpEntropy.IO.BinaryGisModelWriter().Persist(new SharpEntropy.GisModel(trainer), mModelName + mModelExtension);
        }

        #endregion

		public static ITestGenderModel TestModel(string name)
		{
			GenderModel genderModel = new GenderModel(name, false);
			return genderModel;
		}
		
		public static ITrainSimilarityModel TrainModel(string name)
		{
            GenderModel genderModel = new GenderModel(name, true);
            return genderModel;
		}

        private Util.Set<string> ReadNames(string nameFile)
		{
			Util.Set<string> names = new Util.HashSet<string>();
			
            System.IO.StreamReader nameReader = new System.IO.StreamReader(nameFile, System.Text.Encoding.Default);
			for (string line = nameReader.ReadLine(); line != null; line = nameReader.ReadLine())
			{
				names.Add(line);
			}
			return names;
		}

		private List<string> GetFeatures(Context nounPhrase)
		{
            List<string> features = new List<string>();
			features.Add("default");
			for (int tokenIndex = 0; tokenIndex < nounPhrase.HeadTokenIndex; tokenIndex++)
			{
				features.Add("mw=" + nounPhrase.Tokens[tokenIndex].ToString());
			}
			features.Add("hw=" + nounPhrase.HeadTokenText);
            features.Add("n=" + nounPhrase.NameType);
            if (nounPhrase.NameType != null && nounPhrase.NameType == "person")
			{
				object[] tokens = nounPhrase.Tokens;
				//System.err.println("GenderModel.getFeatures: person name="+np1);
				for (int tokenIndex = 0; tokenIndex < nounPhrase.HeadTokenIndex || tokenIndex == 0; tokenIndex++)
				{
					string name = tokens[tokenIndex].ToString().ToLower();
					if (mFemaleNames.Contains(name))
					{
						features.Add("fem");
						//System.err.println("GenderModel.getFeatures: person (fem) "+np1);
					}
					if (mMaleNames.Contains(name))
					{
						features.Add("mas");
						//System.err.println("GenderModel.getFeatures: person (mas) "+np1);
					}
				}
			}
			
            foreach (string synset in nounPhrase.Synsets)
            {
                features.Add("ss=" + synset);
            }
            
			return features;
		}
		
		private void AddEvent(string outcome, Context nounPhrase)
		{
			List<string> features = GetFeatures(nounPhrase);
            mEvents.Add(new SharpEntropy.TrainingEvent(outcome, features.ToArray()));
		}
		
		/// <summary>
        /// Heuristic computation of gender for a mention context using pronouns and honorifics.
        /// </summary>
		/// <param name="mention">
        /// The mention whose gender is to be computed.
		/// </param>
		/// <returns>
        /// The heuristically determined gender or unknown.
		/// </returns>
		private GenderEnum GetGender(Context mention)
		{
			if (Linker.MalePronounPattern.IsMatch(mention.HeadTokenText))
			{
				return GenderEnum.Male;
			}
            else if (Linker.FemalePronounPattern.IsMatch(mention.HeadTokenText))
			{
				return GenderEnum.Female;
			}
            else if (Linker.NeuterPronounPattern.IsMatch(mention.HeadTokenText))
			{
				return GenderEnum.Neuter;
			}
			object[] mentionTokens = mention.Tokens;
			for (int tokenIndex = 0, tokenLength = mentionTokens.Length - 1; tokenIndex < tokenLength; tokenIndex++)
			{
				string token = mentionTokens[tokenIndex].ToString();
				if (token == "Mr." || token == "Mr")
				{
					return GenderEnum.Male;
				}
				else if (token == "Mrs." || token == "Mrs" || token == "Ms." || token == "Ms")
				{
					return GenderEnum.Female;
				}
			}
			return GenderEnum.Unknown;
		}

        private GenderEnum GetGender(List<Context> entity)
		{
			foreach (Context entityContext in entity)
           {
				GenderEnum gender = GetGender(entityContext);
				if (gender != GenderEnum.Unknown)
				{
					return gender;
				}
			}
			return GenderEnum.Unknown;
		}

        //Usage: GenderModel modelName < tiger/NN bear/NN
		public static string GenderMain(string modelName, string line)
		{
			GenderModel model = new GenderModel(modelName, false);
			
			string[] words = line.Split(' ');
			double[] dist = model.GenderDistribution(Context.ParseContext(words[0]));
			string output = "m=" + dist[model.MaleIndex] + " f=" + dist[model.FemaleIndex] + " n=" + dist[model.NeuterIndex] + " " + string.Join(",", (model.GetFeatures(Context.ParseContext(words[0])).ToArray()));
           			
            return output;
		}
		
		





        
    }
}