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

//This file is based on the SimilarityModel.java source file found in the
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
using System.Linq;

namespace OpenNLP.Tools.Coreference.Similarity
{
	/// <summary>
    /// Models semantic similarity between two mentions and returns a score based on 
	/// how semantically comparible the mentions are with one another.  
	/// </summary>
	public class SimilarityModel : ITestSimilarityModel, ITrainSimilarityModel
	{
        private readonly string ModelName;
	    private const string ModelExtension = ".nbin";
	    private readonly SharpEntropy.IMaximumEntropyModel _testModel;
        private readonly List<SharpEntropy.TrainingEvent> _events;
        private readonly int _sameIndex;
        private const string Same = "same";
        private const string Different = "diff";
	    private const bool DebugOn = false;

	    public static ITestSimilarityModel TestModel(string name)
        {
            return new SimilarityModel(name, false);
        }

        public static ITrainSimilarityModel TrainModel(string name)
        {
            return new SimilarityModel(name, true);
        }

        private SimilarityModel(string modelName, bool train)
        {
            ModelName = modelName;
            if (train)
            {
                _events = new List<SharpEntropy.TrainingEvent>();
            }
            else
            {
                _testModel = new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelName + ModelExtension));
                _sameIndex = _testModel.GetOutcomeIndex(Same);
            }
        }

		public virtual void SetExtents(Context[] extents)
		{
            var entities = new Util.HashList<int, Context>();
			// Extents which are not in a coreference chain.
            var singletons = new List<Context>();
            var allExtents = new List<Context>();
			//populate data structures
			foreach (Context currentExtent in extents)
			{
			    if (currentExtent.Id == -1)
			    {
			        singletons.Add(currentExtent);
			    }
			    else
			    {
			        entities.Put(currentExtent.Id, currentExtent);
			    }
			    allExtents.Add(currentExtent);
			}
			
			int allExtentsIndex = 0;
            Dictionary<int, Util.Set<string>> headSets = ConstructHeadSets(entities);
            Dictionary<int, Util.Set<string>> nameSets = ConstructNameSets(entities);
			
			foreach (int key in entities.Keys)
            {
                Util.Set<string> entityNameSet = nameSets[key];
				if (entityNameSet.Count == 0)
				{
					continue;
				}

				List<Context> entityContexts = entities[key];
                Util.Set<Context> exclusionSet = ConstructExclusionSet(key, entities, headSets, nameSets, singletons);

                //if (entityContexts.Count == 1)
                //{
                //}
				for (int firstEntityContextIndex = 0; firstEntityContextIndex < entityContexts.Count; firstEntityContextIndex++)
				{
					Context firstEntityContext = entityContexts[firstEntityContextIndex];
					//if (isPronoun(ec1)) {
					//  continue;
					//}
                    for (int secondEntityContextIndex = firstEntityContextIndex + 1; secondEntityContextIndex < entityContexts.Count; secondEntityContextIndex++)
					{
						Context secondEntityContext = entityContexts[secondEntityContextIndex];
						//if (isPronoun(ec2)) {
						//  continue;
						//}
						AddEvent(true, firstEntityContext, secondEntityContext);
						int startIndex = allExtentsIndex;
						do 
						{
							Context compareEntityContext = allExtents[allExtentsIndex];
							allExtentsIndex = (allExtentsIndex + 1) % allExtents.Count;
							if (!exclusionSet.Contains(compareEntityContext))
							{
								if (DebugOn)
								{
									System.Console.Error.WriteLine(firstEntityContext.ToString() + " " + string.Join(",", entityNameSet.ToArray()) + " " + compareEntityContext.ToString() + " " + nameSets[compareEntityContext.Id]);
								}
								AddEvent(false, firstEntityContext, compareEntityContext);
								break;
							}
						}
						while (allExtentsIndex != startIndex);
					}
				}
			}
		}

		private void AddEvent(bool same, Context firstNounPhrase, Context secondNounPhrase)
		{
			if (same)
			{
				List<string> features = GetFeatures(firstNounPhrase, secondNounPhrase);
                _events.Add(new SharpEntropy.TrainingEvent(Same, features.ToArray()));
			}
			else
			{
                List<string> features = GetFeatures(firstNounPhrase, secondNounPhrase);
                _events.Add(new SharpEntropy.TrainingEvent(Different, features.ToArray()));
			}
		}
		
		/// <summary> 
        /// Produces a set of head words for the specified list of mentions.
        /// </summary>
		/// <param name="mentions">
        /// The mentions to use to construct the 
		/// </param>
		/// <returns> 
        /// A set containing the head words of the sepecified mentions.
		/// </returns>
		private Util.Set<string> ConstructHeadSet(IEnumerable<Context> mentions)
		{
			Util.Set<string> headSet = new Util.HashSet<string>();
            foreach (Context currentContext in mentions)
			{
				headSet.Add(currentContext.HeadTokenText.ToLower());
			}
			return headSet;
		}

        private bool HasSameHead(IEnumerable<string> entityHeadSet, Util.Set<string> candidateHeadSet)
        {
            return entityHeadSet.Any(candidateHeadSet.Contains);
        }

	    private bool HasSameNameType(IEnumerable<string> entityNameSet, Util.Set<string> candidateNameSet)
	    {
	        return entityNameSet.Any(candidateNameSet.Contains);
	    }

	    private bool HasSuperClass(IEnumerable<Context> entityContexts, List<Context> candidateContexts)
		{
			foreach (Context currentEntityContext in entityContexts)
			{
                foreach (Context currentCandidateContext in candidateContexts)
				{
					if (InSuperClass(currentEntityContext, currentCandidateContext))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
        /// Constructs a set of entities which may be semantically compatible with the entity indicated by
        /// the specified entityKey.
        /// </summary>
		/// <param name="entityKey">
        /// The key of the entity for which the set is being constructed. 
		/// </param>
		/// <param name="entities">
        /// A mapping between entity keys and their mentions. 
		/// </param>
		/// <param name="headSets">
        /// A mapping between entity keys and their head sets.
		/// </param>
		/// <param name="nameSets">
        /// A mapping between entity keys and their name sets.
		/// </param>
		/// <param name="singletons">
        /// A list of all entities which consists of a single mention.
		/// </param>
		/// <returns>
        /// A set of mentions for all the entities which might be semantically compatible 
		/// with entity indicated by the specified key. 
		/// </returns>
		 private Util.Set<Context> ConstructExclusionSet(int entityKey, Util.HashList<int, Context> entities, 
             Dictionary<int, Util.Set<string>> headSets, Dictionary<int, Util.Set<string>> nameSets, IEnumerable<Context> singletons)
		{
			Util.Set<Context> exclusionSet = new Util.HashSet<Context>();
            Util.Set<string> entityHeadSet = headSets[entityKey];
            Util.Set<string> entityNameSet = nameSets[entityKey];
			List<Context> entityContexts = entities[entityKey];

			//entities
			foreach (int key in entities.Keys)
            {
				List<Context> candidateContexts = entities[key];

                if (key == entityKey)
				{
                    exclusionSet.AddAll(candidateContexts);
				}
				else if (nameSets[key].Count == 0)
				{
                    exclusionSet.AddAll(candidateContexts);
				}
                else if (HasSameHead(entityHeadSet, headSets[key]))
				{
                    exclusionSet.AddAll(candidateContexts);
				}
                else if (HasSameNameType(entityNameSet, nameSets[key]))
				{
                    exclusionSet.AddAll(candidateContexts);
				}
				else if (HasSuperClass(entityContexts, candidateContexts))
				{
                    exclusionSet.AddAll(candidateContexts);
				}
			}

			//singles
			var singles = new List<Context>(1);
			foreach (Context currentSingleton in singletons)
			{
				singles.Clear();
				singles.Add(currentSingleton);
				if (entityHeadSet.Contains(currentSingleton.HeadTokenText.ToLower()))
				{
					exclusionSet.Add(currentSingleton);
				}
                else if (currentSingleton.NameType == null)
				{
					exclusionSet.Add(currentSingleton);
				}
                else if (entityNameSet.Contains(currentSingleton.NameType))
				{
					exclusionSet.Add(currentSingleton);
				}
				else if (HasSuperClass(entityContexts, singles))
				{
					exclusionSet.Add(currentSingleton);
				}
			}
			return exclusionSet;
		}
		
		/// <summary>
        /// Constructs a mapping between the specified entities and their head set.
        /// </summary>
		/// <param name="entities">
        /// Mapping between a key and a list of mentions which compose an entity.
		/// </param>
		/// <returns> 
        /// a mapping between the keys of the secified entity mapping and the head set 
		/// generatated from the mentions associated with that key.
		/// </returns>
        private Dictionary<int, Util.Set<string>> ConstructHeadSets(Util.HashList<int, Context> entities)
		{
			var headSets = new Dictionary<int, Util.Set<string>>();
			foreach (int key in entities.Keys)
            {
				List<Context> entityContexts = entities[key];
				headSets[key] = ConstructHeadSet(entityContexts);
			}
			return headSets;
		}
		
		/// <summary> 
        /// Produces the set of name types associated with each of the specified mentions.
        /// </summary>
		/// <param name="mentions">
        /// A list of mentions.
		/// </param>
		/// <returns>
        /// A set of name types assigned to the specified mentions.
		/// </returns>
        private Util.Set<string> ConstructNameSet(IEnumerable<Context> mentions)
		{
			Util.Set<string> nameSet = new Util.HashSet<string>();
			foreach (Context currentContext in mentions)
            {
                if (currentContext.NameType != null)
				{
                    nameSet.Add(currentContext.NameType);
				}
			}
			return nameSet;
		}
		
		/// <summary> 
        /// Constructs a mappng between the specified entities and the names associated with these entities.
        /// </summary>
		/// <param name="entities">
        /// A mapping between a key and a list of mentions.
		/// </param>
		/// <returns>
        /// a mapping between each key in the specified entity map and the name types associated with the each mention of that entity.
		/// </returns>
        private Dictionary<int, Util.Set<string>> ConstructNameSets(Util.HashList<int, Context> entities)
		{
			var nameSets = new Dictionary<int, Util.Set<string>>();
			foreach (int key in entities.Keys)
            {
				List<Context> entityContexts = entities[key];
				nameSets[key] = ConstructNameSet(entityContexts);
			}
			return nameSets;
		}
		
		private bool InSuperClass(Context entityContext, Context candidateEntityContext)
		{
			if (entityContext.Synsets.Count == 0 || candidateEntityContext.Synsets.Count == 0)
			{
				return false;
			}
			else
			{
				int commonSynsetCount = 0;
				foreach (string synset in entityContext.Synsets)
                {
					if (candidateEntityContext.Synsets.Contains(synset))
					{
						commonSynsetCount++;
					}
				}
				if (commonSynsetCount == 0)
				{
					return false;
				}
				else if (commonSynsetCount == entityContext.Synsets.Count || commonSynsetCount == candidateEntityContext.Synsets.Count)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		/*
		private boolean isPronoun(MentionContext mention) {
		return mention.getHeadTokenTag().startsWith("PRP");
		}
		*/
		
		/// <summary> 
        /// Returns a number between 0 and 1 which represents the models belief that the specified mentions are
        /// compatible.
		/// Value closer to 1 are more compatible, while values closer to 0 are less compatible.
		/// </summary>
		/// <param name="firstMention">
        /// The first mention to be considered.
		/// </param>
		/// <param name="secondMention">
        /// The second mention to be considered.
		/// </param>
		/// <returns> 
        /// a number between 0 and 1 which represents the models belief that the specified mentions are compatible.
		/// </returns>
		public virtual double AreCompatible(Context firstMention, Context secondMention)
		{
			List<string> features = GetFeatures(firstMention, secondMention);
            if (DebugOn)
            {
                Console.Error.WriteLine("SimilarityModel.compatible: feats=" + string.Join(",", features.ToArray()));
            }
            return _testModel.Evaluate(features.ToArray())[_sameIndex];
		}
		
		/// <summary>Train a model based on the previously supplied evidence</summary>
		public virtual void TrainModel()
		{
			if (DebugOn)
			{
				var writer = new System.IO.StreamWriter(ModelName + ".events", false, System.Text.Encoding.Default);
				foreach (SharpEntropy.TrainingEvent trainingEvent in _events)
                {
					writer.Write(trainingEvent + "\n");
				}
				writer.Close();
			}

            var trainer = new SharpEntropy.GisTrainer();
            trainer.TrainModel(new Util.CollectionEventReader(_events), 100, 10);
            new SharpEntropy.IO.BinaryGisModelWriter().Persist(new SharpEntropy.GisModel(trainer), ModelName + ModelExtension);
		}
		
		private bool IsName(Context nounPhrase)
		{
			return PartsOfSpeech.IsProperNoun(nounPhrase.HeadTokenTag);
		}
		
		private bool IsCommonNoun(Context nounPhrase)
		{
			return !PartsOfSpeech.IsProperNoun(nounPhrase.HeadTokenTag) && PartsOfSpeech.IsNoun(nounPhrase.HeadTokenTag);
		}
		
		private bool IsPronoun(Context nounPhrase)
		{
			return PartsOfSpeech.IsPersOrPossPronoun(nounPhrase.HeadTokenTag);
		}
		
		private bool IsNumber(Context nounPhrase)
		{
            return nounPhrase.HeadTokenTag == PartsOfSpeech.CardinalNumber;
		}
		
		private IEnumerable<string> GetNameCommonFeatures(Context name, Context common)
		{
            Util.Set<string> synsets = common.Synsets;
            var features = new List<string>(2 + synsets.Count)
            {
                "nn=" + name.NameType + "," + common.NameType,
                "nw=" + name.NameType + "," + common.HeadTokenText.ToLower()
            };
		    foreach (string synset in synsets)
			{
                features.Add("ns=" + name.NameType + "," + synset);
			}
            if (name.NameType == null)
			{
				//features.addAll(GetCommonCommonFeatures(name,common));
			}
			return features;
		}
		
		private IEnumerable<string> GetNameNumberFeatures(Context name, Context number)
		{
            var features = new List<string>(2)
            {
                "nt=" + name.NameType + "," + number.HeadTokenTag,
                "nn=" + name.NameType + "," + number.NameType
            };
		    return features;
		}

        private IEnumerable<string> GetNamePronounFeatures(Context name, Context pronoun)
		{
            var features = new List<string>(2)
            {
                "nw=" + name.NameType + "," + pronoun.HeadTokenText.ToLower(),
                "ng=" + name.NameType + "," +
                Resolver.AbstractResolver.GetPronounGender(pronoun.HeadTokenText.ToLower())
            };
            return features;
		}

        private IEnumerable<string> GetCommonPronounFeatures(Context common, Context pronoun)
		{
            var features = new List<string>();
            Util.Set<string> synsets = common.Synsets;
			string pronounText = pronoun.HeadTokenText.ToLower();
            string genderText = Resolver.AbstractResolver.GetPronounGender(pronounText);
            features.Add("wn=" + pronounText + "," + common.NameType);
			foreach (string synset in synsets)
            {
				features.Add("ws=" + pronounText + "," + synset);
				features.Add("gs=" + genderText + "," + synset);
			}
			return features;
		}

        private IEnumerable<string> GetCommonNumberFeatures(Context common, Context number)
		{
            var features = new List<string>();
            Util.Set<string> synsets = common.Synsets;
			foreach (string synset in synsets)
            {
				features.Add("ts=" + number.HeadTokenTag + "," + synset);
                features.Add("ns=" + number.NameType + "," + synset);
			}
            features.Add("nn=" + number.NameType + "," + common.NameType);
			return features;
		}

        private IEnumerable<string> GetNumberPronounFeatures(Context number, Context pronoun)
		{
            var features = new List<string>();
			string pronounText = pronoun.HeadTokenText.ToLower();
            string genderText = Resolver.AbstractResolver.GetPronounGender(pronounText);
			features.Add("wt=" + pronounText + "," + number.HeadTokenTag);
            features.Add("wn=" + pronounText + "," + number.NameType);
			features.Add("wt=" + genderText + "," + number.HeadTokenTag);
            features.Add("wn=" + genderText + "," + number.NameType);
			return features;
		}

        private IEnumerable<string> GetNameNameFeatures(Context name1, Context name2)
		{
            var features = new List<string>(1);
            if (name1.NameType == null && name2.NameType == null)
			{
                features.Add("nn=" + name1.NameType + "," + name2.NameType);
				//features.addAll(getCommonCommonFeatures(name1,name2));
			}
            else if (name1.NameType == null)
			{
                features.Add("nn=" + name1.NameType + "," + name2.NameType);
				//features.addAll(getNameCommonFeatures(name2,name1));
			}
            else if (name2.NameType == null)
			{
                features.Add("nn=" + name2.NameType + "," + name1.NameType);
				//features.addAll(getNameCommonFeatures(name1,name2));
			}
			else
			{
                if (string.CompareOrdinal(name1.NameType, name2.NameType) < 0)
				{
                    features.Add("nn=" + name1.NameType + "," + name2.NameType);
				}
				else
				{
                    features.Add("nn=" + name2.NameType + "," + name1.NameType);
				}
                if (name1.NameType == name2.NameType)
				{
					features.Add("sameNameType");
				}
			}
			return features;
		}

        private IEnumerable<string> GetCommonCommonFeatures(Context common1, Context common2)
		{
            var features = new List<string>();
			Util.Set<string> synsets1 = common1.Synsets;
            Util.Set<string> synsets2 = common2.Synsets;
			
			if (synsets1.Count == 0)
			{
				//features.add("missing_"+common1.headToken);
				return features;
			}
			if (synsets2.Count == 0)
			{
				//features.add("missing_"+common2.headToken);
				return features;
			}
			int commonSynsetCount = 0;
            //RN commented out - this looks wrong in the java
            //bool same = false;
            
            //if (commonSynsetCount == 0)
            //{
            //    features.Add("ncss");
            //}
            //else if (commonSynsetCount == synsets1.Count && commonSynsetCount == synsets2.Count)
            //{
            //    same = true;
            //    features.Add("samess");
            //}
            //else if (commonSynsetCount == synsets1.Count)
            //{
            //    features.Add("2isa1");
            //    //features.add("2isa1-"+(synsets2.size() - numCommonSynsets));
            //}
            //else if (commonSynsetCount == synsets2.Count)
            //{
            //    features.Add("1isa2");
            //    //features.add("1isa2-"+(synsets1.size() - numCommonSynsets));
            //}
            

            //if (!same)
            //{
				foreach(string synset in synsets1)
				{
					if (synsets2.Contains(synset))
					{
						features.Add("ss=" + synset);
						commonSynsetCount++;
					}
				}
            //}
            //end RN commented out
			if (commonSynsetCount == 0)
			{
				features.Add("ncss");
			}
			else if (commonSynsetCount == synsets1.Count && commonSynsetCount == synsets2.Count)
			{
				features.Add("samess");
			}
			else if (commonSynsetCount == synsets1.Count)
			{
				features.Add("2isa1");
				//features.add("2isa1-"+(synsets2.size() - numCommonSynsets));
			}
			else if (commonSynsetCount == synsets2.Count)
			{
				features.Add("1isa2");
				//features.add("1isa2-"+(synsets1.size() - numCommonSynsets));
			}
			return features;
		}
		
		private IEnumerable<string> GetPronounPronounFeatures(Context pronoun1, Context pronoun2)
		{
            var features = new List<string>();
            string firstGender = Resolver.AbstractResolver.GetPronounGender(pronoun1.HeadTokenText);
            string secondGender = Resolver.AbstractResolver.GetPronounGender(pronoun2.HeadTokenText);
		    features.Add(firstGender == secondGender ? "sameGender" : "diffGender");
		    return features;
		}
		
		private List<string> GetFeatures(Context np1, Context np2)
		{
            var features = new List<string> {"default"};
		    //  semantic categories
			string w1 = np1.HeadTokenText.ToLower();
			string w2 = np2.HeadTokenText.ToLower();
			if (string.CompareOrdinal(w1, w2) < 0)
			{
				features.Add("ww=" + w1 + "," + w2);
			}
			else
			{
				features.Add("ww=" + w2 + "," + w1);
			}
			if (w1 == w2)
			{
				features.Add("sameHead");
			}
			//features.add("tt="+np1.headTag+","+np2.headTag);
			if (IsName(np1))
			{
				if (IsName(np2))
				{
					features.AddRange(GetNameNameFeatures(np1, np2));
				}
				else if (IsCommonNoun(np2))
				{
                    features.AddRange(GetNameCommonFeatures(np1, np2));
				}
				else if (IsPronoun(np2))
				{
                    features.AddRange(GetNamePronounFeatures(np1, np2));
				}
				else if (IsNumber(np2))
				{
                    features.AddRange(GetNameNumberFeatures(np1, np2));
				}
			}
			else if (IsCommonNoun(np1))
			{
				if (IsName(np2))
				{
                    features.AddRange(GetNameCommonFeatures(np2, np1));
				}
				else if (IsCommonNoun(np2))
				{
                    features.AddRange(GetCommonCommonFeatures(np1, np2));
				}
				else if (IsPronoun(np2))
				{
                    features.AddRange(GetCommonPronounFeatures(np1, np2));
				}
				else if (IsNumber(np2))
				{
                    features.AddRange(GetCommonNumberFeatures(np1, np2));
				}
			}
			else if (IsPronoun(np1))
			{
				if (IsName(np2))
				{
                    features.AddRange(GetNamePronounFeatures(np2, np1));
				}
				else if (IsCommonNoun(np2))
				{
                    features.AddRange(GetCommonPronounFeatures(np2, np1));
				}
				else if (IsPronoun(np2))
				{
                    features.AddRange(GetPronounPronounFeatures(np1, np2));
				}
				else if (IsNumber(np2))
				{
                    features.AddRange(GetNumberPronounFeatures(np2, np1));
				}
			}
			else if (IsNumber(np1))
			{
				if (IsName(np2))
				{
                    features.AddRange(GetNameNumberFeatures(np2, np1));
				}
				else if (IsCommonNoun(np2))
				{
                    features.AddRange(GetCommonNumberFeatures(np2, np1));
				}
				else if (IsPronoun(np2))
				{
                    features.AddRange(GetNumberPronounFeatures(np1, np2));
				}
				else if (IsNumber(np2))
				{
				}
			}
			return features;
		}
		
		//Usage: SimilarityModel modelName < tiger/NN bear/NN
        public static string SimilarityMain(string modelName, string line)
		{
			var model = new SimilarityModel(modelName, false);
			//Context.wn = new WordNet(System.getProperty("WNHOME"), true);
			//Context.morphy = new Morphy(Context.wn);
			
			string[] words = line.Split(' ');
			double p = model.AreCompatible(Context.ParseContext(words[0]), Context.ParseContext(words[1]));
			return p + " " + string.Join(",", model.GetFeatures(Context.ParseContext(words[0]), Context.ParseContext(words[1])).ToArray());
		}
	}
}