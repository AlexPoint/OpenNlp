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

//This file is based on the MaxentResolver.java source file found in the
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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SharpEntropy.IO;

namespace OpenNLP.Tools.Coreference.Resolver
{
	/// <summary> 
    /// Provides common functionality used by classes which implement the {@link IResolver} interface
    /// and use maximum entropy models to make resolution decisions.
    /// </summary>
	public abstract class MaximumEntropyResolver : AbstractResolver
	{		
		/// <summary>
        /// Outcomes when two mentions are coreferent.
        /// </summary>
		public const string Same = "same";
		/// <summary>
        /// Outcome when two mentions are not coreferent.
        /// </summary>
		public const string Diff = "diff";
		/// <summary>
        /// Default feature value.
        /// </summary>
		public const string Default = "default";
		
		private static readonly Regex EndsWithPeriod = new Regex("\\.$", RegexOptions.Compiled);
	    private const double MinimumSimilarityProbability = 0.60;

	    private const string SimilarityCompatible = "sim.compatible";
		private const string SimilarityIncompatible = "sim.incompatible";
		private const string SimilarityUnknown = "sim.unknown";
		
		private const string NumberCompatible = "num.compatible";
	    private const string NumberIncompatible = "num.incompatible";
	    private const string NumberUnknown = "num.unknown";

	    private const string GenderCompatible = "gen.compatible";
	    private const string GenderIncompatible = "gen.incompatible";
	    private const string GenderUnknown = "gen.unknown";

	    private const bool DebugOn = false;

		private readonly string _modelName;
        private readonly SharpEntropy.IMaximumEntropyModel _model;
		private readonly double[] _candidateProbabilities;
		private readonly int _sameIndex;
		private readonly ResolverMode _resolverMode;
		private readonly List<SharpEntropy.TrainingEvent> _events;
        
        private const string ModelExtension = ".nbin";

        public static Similarity.ITestSimilarityModel SimilarityModel { get; set; }

		/// <summary>
        /// When true, this designates that the resolver should use the first referent encountered which it
		/// more preferable than non-reference.  When false, all non-excluded referents within this resolver's range
		/// are considered. 
		/// </summary>
        protected internal bool PreferFirstReferent { get; set; }

		/// <summary>
        /// When true, this designates that training should consist of a single positive and a single negative example
		/// (when possible) for each mention. 
		/// </summary>
        protected internal bool PairedSampleSelection { get; set; }
		
		/// <summary>
        /// When true, this designates that the same maximum entropy model should be used non-reference
		/// events (the pairing of a mention and the "null" reference) as is used for potentially 
		/// referential pairs.  When false a seperate model is created for these events.  
		/// </summary>
        protected internal bool UseSameModelForNonRef { get; set; }
        
		/// <summary>
        /// The model for computing non-referential probabilities.
        /// </summary>
        protected internal INonReferentialResolver NonReferentialResolver { get; set; }
		
		/// <summary>
        /// Creates a maximum-entropy-based resolver which will look the specified number of entities back
        /// for a referent.
		/// This constructor is only used for unit testing.
		/// </summary>
		/// <param name="numberOfEntitiesBack">
		/// </param>
		/// <param name="preferFirstReferent">
		/// </param>
		protected MaximumEntropyResolver(int numberOfEntitiesBack, bool preferFirstReferent) : base(numberOfEntitiesBack)
		{
			PreferFirstReferent = preferFirstReferent;
		}
		
		/// <summary>
        /// Creates a maximum-entropy-based resolver with the specified model name, using the 
		/// specified mode, which will look the specified number of entities back for a referent and
		/// prefer the first referent if specified.
		/// </summary>
		/// <param name="modelDirectory">
        /// The name of the directory where the resover models are stored.
		/// </param>
		/// <param name="name">
        /// The name of the file where this model will be read or written.
		/// </param>
		/// <param name="mode">
        /// The mode this resolver is being using in (training, testing).
		/// </param>
		/// <param name="numberOfEntitiesBack">
        /// The number of entities back in the text that this resolver will look
		/// for a referent.
		/// </param>
		/// <param name="preferFirstReferent">
        /// Set to true if the resolver should prefer the first referent which is more
		/// likly than non-reference.  This only affects testing.
		/// </param>
		/// <param name="nonReferentialResolver">
        /// Determines how likly it is that this entity is non-referential.
		/// </param>
		protected MaximumEntropyResolver(string modelDirectory, string name, ResolverMode mode, int numberOfEntitiesBack, bool preferFirstReferent, 
            INonReferentialResolver nonReferentialResolver): base(numberOfEntitiesBack)
		{
			PreferFirstReferent = preferFirstReferent;
			NonReferentialResolver = nonReferentialResolver;
			_resolverMode = mode;
			_modelName = modelDirectory + "/" + name;
			if (_resolverMode == ResolverMode.Test)
			{
			    _model = new SharpEntropy.GisModel(new BinaryGisModelReader(_modelName + ModelExtension));
				_sameIndex = _model.GetOutcomeIndex(Same);
			}
			else if (_resolverMode == ResolverMode.Train)
			{
                _events = new List<SharpEntropy.TrainingEvent>();
			}
			else
			{
				Console.Error.WriteLine("Unknown mode: " + _resolverMode);
			}
			//add one for non-referent possibility
			_candidateProbabilities = new double[GetNumberEntitiesBack() + 1];
		}
		
		/// <summary>
        /// Creates a maximum-entropy-based resolver with the specified model name, using the 
		/// specified mode, which will look the specified number of entities back for a referent.
		/// </summary>
		/// <param name="modelDirectory">
        /// The name of the directory where the resolver models are stored.
		/// </param>
		/// <param name="modelName">
        /// The name of the file where this model will be read or written.
		/// </param>
		/// <param name="mode">
        /// The mode this resolver is being using in (training, testing).
		/// </param>
		/// <param name="numberEntitiesBack">
        /// The number of entities back in the text that this resolver will look
		/// for a referent.
		/// </param>
		protected MaximumEntropyResolver(string modelDirectory, string modelName, ResolverMode mode, int numberEntitiesBack): 
            this(modelDirectory, modelName, mode, numberEntitiesBack, false){}

        protected MaximumEntropyResolver(string modelDirectory, string modelName, ResolverMode mode, int numberEntitiesBack, INonReferentialResolver nonReferentialResolver):
            this(modelDirectory, modelName, mode, numberEntitiesBack, false, nonReferentialResolver){}
		
		protected MaximumEntropyResolver(string modelDirectory, string modelName, ResolverMode mode, int numberEntitiesBack, bool preferFirstReferent):
            this(modelDirectory, modelName, mode, numberEntitiesBack, preferFirstReferent, new DefaultNonReferentialResolver(modelDirectory, modelName, mode)){}
		
		protected MaximumEntropyResolver(string modelDirectory, string modelName, ResolverMode mode, int numberEntitiesBack, bool preferFirstReferent, double nonReferentialProbability):
            this(modelDirectory, modelName, mode, numberEntitiesBack, preferFirstReferent, new FixedNonReferentialResolver(nonReferentialProbability)){}
		
        public override DiscourseEntity Resolve(Mention.MentionContext expression, DiscourseModel discourseModel)
		{
			DiscourseEntity discourseEntity;
			int entityIndex = 0;
			double nonReferentialProbability = NonReferentialResolver.GetNonReferentialProbability(expression);
			if (DebugOn)
			{
				System.Console.Error.WriteLine(this.ToString() + ".resolve: " + expression.ToText() + " -> " + "null " + nonReferentialProbability);
			}
			for (; entityIndex < GetNumberEntitiesBack(discourseModel); entityIndex++)
			{
				discourseEntity = discourseModel.GetEntity(entityIndex);
				if (IsOutOfRange(expression, discourseEntity))
				{
					break;
				}
				if (IsExcluded(expression, discourseEntity))
				{
					_candidateProbabilities[entityIndex] = 0;
					if (DebugOn)
					{
						Console.Error.WriteLine("excluded " + this.ToString() + ".resolve: " + expression.ToText() + " -> " + discourseEntity + " " + _candidateProbabilities[entityIndex]);
					}
				}
				else
				{
                    string[] features = GetFeatures(expression, discourseEntity).ToArray();
                    try
					{
						_candidateProbabilities[entityIndex] = _model.Evaluate(features)[_sameIndex];
					}
					catch (IndexOutOfRangeException)
					{
						_candidateProbabilities[entityIndex] = 0;
					}
					if (DebugOn)
					{
						Console.Error.WriteLine(this + ".resolve: " + expression.ToText() + " -> " + discourseEntity + " (" + expression.GetGender() + "," + discourseEntity.Gender + ") " + _candidateProbabilities[entityIndex] + " " + string.Join(",", features)); //SupportClass.CollectionToString(lfeatures));
					}
				}
				if (PreferFirstReferent && _candidateProbabilities[entityIndex] > nonReferentialProbability)
				{
					entityIndex++; //update for nonRef assignment
					break;
				}
			}
			_candidateProbabilities[entityIndex] = nonReferentialProbability;
			
			// find max
			int maximumCandidateIndex = 0;
			for (int currentCandidate = 1; currentCandidate <= entityIndex; currentCandidate++)
			{
				if (_candidateProbabilities[currentCandidate] > _candidateProbabilities[maximumCandidateIndex])
				{
					maximumCandidateIndex = currentCandidate;
				}
			}
			if (maximumCandidateIndex == entityIndex)
			{
				// no referent
				return null;
			}
			else
			{
				discourseEntity = discourseModel.GetEntity(maximumCandidateIndex);
				return discourseEntity;
			}
		}
		
		/*
		protected double getNonReferentialProbability(MentionContext ec) {
		if (useFixedNonReferentialProbability) {
		return fixedNonReferentialProbability;
		}
		List lfeatures = getFeatures(ec, null);
		string[] features = (string[]) lfeatures.toArray(new string[lfeatures.size()]);
		
		double[] dist = nrModel.eval(features);
		
		return (dist[nrSameIndex]);
		}
		*/
		
		/// <summary>
        /// Returns whether the specified entity satisfies the criteria for being a default referent.
		/// This criteria is used to perform sample selection on the training data and to select a single
		/// non-referent entity. Typically the criteria is a hueristic for a likely referent.
		/// </summary>
		/// <param name="discourseEntity">
        /// The discourse entity being considered for non-reference.
		/// </param>
		/// <returns>
        /// True if the entity should be used as a default referent, false otherwise. 
		/// </returns>
        protected internal virtual bool defaultReferent(DiscourseEntity discourseEntity)
		{
            Mention.MentionContext entityContext = discourseEntity.LastExtent;
            if (entityContext.NounPhraseSentenceIndex == 0)
			{
				return true;
			}
			return false;
		}

        public override DiscourseEntity Retain(Mention.MentionContext mention, DiscourseModel discourseModel)
		{
			if (_resolverMode == ResolverMode.Train)
			{
				DiscourseEntity discourseEntity = null;
				bool referentFound = false;
				bool hasReferentialCandidate = false;
				bool nonReferentFound = false;
				for (int entityIndex = 0; entityIndex < GetNumberEntitiesBack(discourseModel); entityIndex++)
				{
					DiscourseEntity currentDiscourseEntity = discourseModel.GetEntity(entityIndex);
                    Mention.MentionContext entityMention = currentDiscourseEntity.LastExtent;
					if (IsOutOfRange(mention, currentDiscourseEntity))
					{
						break;
					}
					if (IsExcluded(mention, currentDiscourseEntity))
					{
						if (ShowExclusions)
						{
							if (mention.Id != - 1 && entityMention.Id == mention.Id)
							{
								System.Console.Error.WriteLine(this + ".retain: Referent excluded: (" + mention.Id + ") " + mention.ToText() + " " + mention.IndexSpan + " -> (" + entityMention.Id + ") " + entityMention.ToText() + " " + entityMention.Span + " " + this);
							}
						}
					}
					else
					{
						hasReferentialCandidate = true;
						bool useAsDifferentExample = defaultReferent(currentDiscourseEntity);
						//if (!sampleSelection || (mention.getId() != -1 && entityMention.getId() == mention.getId()) || (!nonReferentFound && useAsDifferentExample)) {
						List<string> features = GetFeatures(mention, currentDiscourseEntity);
						
						//add Event to Model
						if (DebugOn)
						{
							Console.Error.WriteLine(this + ".retain: " + mention.Id + " " + mention.ToText() + " -> " + entityMention.Id + " " + currentDiscourseEntity);
						}
						if (mention.Id != - 1 && entityMention.Id == mention.Id)
						{
							referentFound = true;
                            _events.Add(new SharpEntropy.TrainingEvent(Same, features.ToArray()));
							discourseEntity = currentDiscourseEntity;
							Distances.Add(entityIndex);
						}
						else if (!PairedSampleSelection || (!nonReferentFound && useAsDifferentExample))
						{
							nonReferentFound = true;
                            _events.Add(new SharpEntropy.TrainingEvent(Diff, features.ToArray()));
						}
						//}
					}
					if (PairedSampleSelection && referentFound && nonReferentFound)
					{
						break;
					}
					if (PreferFirstReferent && referentFound)
					{
						break;
					}
				}
				// doesn't refer to anything
				if (hasReferentialCandidate)
				{
					NonReferentialResolver.AddEvent(mention);
				}
				return discourseEntity;
			}
			else
			{
				return base.Retain(mention, discourseModel);
			}
		}
		
		protected internal virtual string GetMentionCountFeature(DiscourseEntity discourseEntity)
		{
			if (discourseEntity.MentionCount >= 5)
			{
				return ("mc=5+");
			}
			else
			{
				return ("mc=" + discourseEntity.MentionCount);
			}
		}
		
		/// <summary>
        /// Returns a list of features for deciding whether the specified mention refers to the specified discourse entity.
        /// </summary>
		/// <param name="mention">
        /// the mention being considers as possibly referential. 
		/// </param>
		/// <param name="entity">
        /// The discourse entity with which the mention is being considered referential.  
		/// </param>
		/// <returns>
        /// a list of features used to predict reference between the specified mention and entity.
		/// </returns>
        protected internal virtual List<string> GetFeatures(Mention.MentionContext mention, DiscourseEntity entity)
		{
			List<string> features = new List<string>();
			features.Add(Default);
            features.AddRange(GetCompatibilityFeatures(mention, entity));
			return features;
		}
		
		public override void Train()
		{
			if (_resolverMode == ResolverMode.Train)
			{
				if (DebugOn)
				{
					System.Console.Error.WriteLine(this.ToString() + " referential");
                    using (var writer = new System.IO.StreamWriter(_modelName + ".events", false, System.Text.Encoding.Default))
                    {
                        foreach (SharpEntropy.TrainingEvent e in _events)
                        {
                            writer.Write(e.ToString() + "\n");
                        }
                        writer.Close();
                    }
				}

                var trainer = new SharpEntropy.GisTrainer();
                trainer.TrainModel(new Util.CollectionEventReader(_events), 100, 10);
                new BinaryGisModelWriter().Persist(new SharpEntropy.GisModel(trainer), _modelName + ModelExtension);
                
				NonReferentialResolver.Train();
			}
		}

        private string GetSemanticCompatibilityFeature(Mention.MentionContext entityContext, DiscourseEntity discourseEntity)
		{
			if (SimilarityModel != null)
			{
				double best = 0;
                foreach (Mention.MentionContext checkEntityContext in discourseEntity.Mentions)
				{
					double sim = SimilarityModel.AreCompatible(entityContext, checkEntityContext);
					if (DebugOn)
					{
						Console.Error.WriteLine("MaxentResolver.GetSemanticCompatibilityFeature: sem-compat " + sim + " " + entityContext.ToText() + " " + checkEntityContext.ToText());
					}
					if (sim > best)
					{
						best = sim;
					}
				}
				if (best > MinimumSimilarityProbability)
				{
					return SimilarityCompatible;
				}
				else if (best > (1 - MinimumSimilarityProbability))
				{
					return SimilarityUnknown;
				}
				else
				{
					return SimilarityIncompatible;
				}
			}
			else
			{
				Console.Error.WriteLine("MaxentResolver: Uninitialized Semantic Model");
				return SimilarityUnknown;
			}
		}

        private string GetGenderCompatibilityFeature(Mention.MentionContext entityContext, DiscourseEntity discourseEntity)
		{
            Similarity.GenderEnum entityGender = discourseEntity.Gender;
            if (entityGender == Similarity.GenderEnum.Unknown || entityContext.GetGender() == Similarity.GenderEnum.Unknown)
			{
				return GenderUnknown;
			}
			else if (entityContext.GetGender() == entityGender)
			{
				return GenderCompatible;
			}
			else
			{
				return GenderIncompatible;
			}
		}

        private string GetNumberCompatibilityFeature(Mention.MentionContext entityContext, DiscourseEntity discourseEntity)
		{
            Similarity.NumberEnum entityNumber = discourseEntity.Number;
            if (entityNumber == Similarity.NumberEnum.Unknown || entityContext.GetNumber() == Similarity.NumberEnum.Unknown)
			{
				return NumberUnknown;
			}
			else if (entityContext.GetNumber() == entityNumber)
			{
				return NumberCompatible;
			}
			else
			{
				return NumberIncompatible;
			}
		}
		
		/// <summary>
        /// Returns features indicating whether the specified mention and the specified entity are compatible.
        /// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <param name="entity">
        /// The entity.
		/// </param>
		/// <returns> 
        /// list of features indicating whether the specified mention and the specified entity are compatible.
		/// </returns>
        private IEnumerable<string> GetCompatibilityFeatures(Mention.MentionContext mention, DiscourseEntity entity)
		{
            var compatibilityFeatures = new List<string>();
			string semanticCompatibilityFeature = GetSemanticCompatibilityFeature(mention, entity);
			compatibilityFeatures.Add(semanticCompatibilityFeature);
			string genderCompatibilityFeature = GetGenderCompatibilityFeature(mention, entity);
			compatibilityFeatures.Add(genderCompatibilityFeature);
			string numberCompatibilityFeature = GetNumberCompatibilityFeature(mention, entity);
			compatibilityFeatures.Add(numberCompatibilityFeature);
			if (semanticCompatibilityFeature == SimilarityCompatible && genderCompatibilityFeature == GenderCompatible && numberCompatibilityFeature == NumberCompatible)
			{
				compatibilityFeatures.Add("all.compatible");
			}
			else if (semanticCompatibilityFeature == SimilarityIncompatible || genderCompatibilityFeature == GenderIncompatible || numberCompatibilityFeature == NumberIncompatible)
			{
				compatibilityFeatures.Add("some.incompatible");
			}
			return compatibilityFeatures;
		}
		
		/// <summary>
        /// Returns a list of features based on the surrounding context of the specified mention.
        /// </summary>
		/// <param name="mention">
        /// the mention whose surround context the features model. 
		/// </param>
		/// <returns>
        /// a list of features based on the surrounding context of the specified mention
		/// </returns>
        public static List<string> GetContextFeatures(Mention.MentionContext mention)
		{
            var features = new List<string>();
			if (mention.PreviousToken != null)
			{
				features.Add("pt=" + mention.PreviousToken.SyntacticType);
				features.Add("pw=" + mention.PreviousToken);
			}
			else
			{
				features.Add("pt=BOS");
				features.Add("pw=BOS");
			}
			if (mention.NextToken != null)
			{
				features.Add("nt=" + mention.NextToken.SyntacticType);
				features.Add("nw=" + mention.NextToken);
			}
			else
			{
				features.Add("nt=EOS");
				features.Add("nw=EOS");
			}
			if (mention.NextTokenBasal != null)
			{
				features.Add("bnt=" + mention.NextTokenBasal.SyntacticType);
				features.Add("bnw=" + mention.NextTokenBasal);
			}
			else
			{
				features.Add("bnt=EOS");
				features.Add("bnw=EOS");
			}
			return features;
		}

        private Util.Set<string> ConstructModifierSet(Mention.IParse[] tokens, int headIndex)
		{
			Util.Set<string> modifierSet = new Util.HashSet<string>();
			for (int tokenIndex = 0; tokenIndex < headIndex; tokenIndex++)
			{
                Mention.IParse token = tokens[tokenIndex];
				modifierSet.Add(token.ToString().ToLower());
			}
			return modifierSet;
		}
		
		/// <summary>
        /// Returns whether the specified token is a definite article.</summary>
		/// <param name="token">
        /// The token.
		/// </param>
		/// <param name="tag">
        /// The pos-tag for the specified token.
		/// </param>
		/// <returns> 
        /// whether the specified token is a definite article.
		/// </returns>
		protected internal virtual bool IsDefiniteArticle(string token, string tag)
		{
			token = token.ToLower();
			if (token == "the" || token == "these" || token == "these" || tag == PartsOfSpeech.PossessivePronoun)
			{
				return true;
			}
			return false;
		}
		
		private bool IsSubstring(string mentionStrip, string entityMentionStrip)
		{
            int index = entityMentionStrip.IndexOf(mentionStrip);
			if (index != - 1)
			{
				//check boundries
				if (index != 0 && entityMentionStrip[index - 1] != ' ')
				{
					return false;
				}
				int end = index + mentionStrip.Length;
				if (end != entityMentionStrip.Length && entityMentionStrip[end] != ' ')
				{
					return false;
				}
				return true;
			}
			return false;
		}
		
		protected internal override bool IsExcluded(Mention.MentionContext entityContext, DiscourseEntity discourseEntity)
		{
			if (base.IsExcluded(entityContext, discourseEntity))
			{
				return true;
			}
			return false;
			/*
			else {
			if (GEN_INCOMPATIBLE == getGenderCompatibilityFeature(ec,de)) {
			return true; 
			}
			else if (NUM_INCOMPATIBLE == getNumberCompatibilityFeature(ec,de)) {
			return true;
			}
			else if (SIM_INCOMPATIBLE == getSemanticCompatibilityFeature(ec,de)) {
			return true;
			}
			return false;
			}
			*/
		}
		
		/// <summary>
        /// Returns distance features for the specified mention and entity.
        /// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <param name="entity">
        /// The entity.
		/// </param>
		/// <returns>
        /// list of distance features for the specified mention and entity.
		/// </returns>
        protected internal virtual List<string> GetDistanceFeatures(Mention.MentionContext mention, DiscourseEntity entity)
		{
            var features = new List<string>();
            Mention.MentionContext currentEntityContext = entity.LastExtent;
			int entityDistance = mention.NounPhraseDocumentIndex - currentEntityContext.NounPhraseDocumentIndex;
			int sentenceDistance = mention.SentenceNumber - currentEntityContext.SentenceNumber;
			int hobbsEntityDistance;
			if (sentenceDistance == 0)
			{
				hobbsEntityDistance = currentEntityContext.NounPhraseSentenceIndex;
			}
			else
			{
				//hobbsEntityDistance = entityDistance - (entities within sentence from mention to end) + (entities within sentence form start to mention) 
				//hobbsEntityDistance = entityDistance - (cec.maxNounLocation - cec.getNounPhraseSentenceIndex) + cec.getNounPhraseSentenceIndex; 
				hobbsEntityDistance = entityDistance + (2 * currentEntityContext.NounPhraseSentenceIndex) - currentEntityContext.MaxNounPhraseSentenceIndex;
			}
			features.Add("hd=" + hobbsEntityDistance);
			features.Add("de=" + entityDistance);
			features.Add("ds=" + sentenceDistance);
			//features.add("ds=" + sdist + pronoun);
			//features.add("dn=" + cec.sentenceNumber);
			//features.add("ep=" + cec.nounLocation);
			return features;
		}
		
		private Dictionary<string, string> GetPronounFeatureMap(string pronoun)
		{
            var pronounMap = new Dictionary<string, string>();
			if (Linker.MalePronounPattern.IsMatch(pronoun))
			{
				pronounMap["gender"] = "male";
			}
            else if (Linker.FemalePronounPattern.IsMatch(pronoun))
			{
				pronounMap["gender"] = "female";
			}
            else if (Linker.NeuterPronounPattern.IsMatch(pronoun))
			{
				pronounMap["gender"] = "neuter";
			}
            if (Linker.SingularPronounPattern.IsMatch(pronoun))
			{
				pronounMap["number"] = "singular";
			}
            else if (Linker.PluralPronounPattern.IsMatch(pronoun))
			{
				pronounMap["number"] = "plural";
			}
			/*
			if (Linker.firstPersonPronounPattern.matcher(pronoun).matches()) {
			pronounMap.put("person","first");
			}
			else if (Linker.secondPersonPronounPattern.matcher(pronoun).matches()) {
			pronounMap.put("person","second");
			}
			else if (Linker.thirdPersonPronounPattern.matcher(pronoun).matches()) {
			pronounMap.put("person","third");
			}
			*/
			return pronounMap;
		}
		
		/// <summary>
        /// Returns features indicating whether the specified mention is compatible with the pronouns
		/// of the specified entity.
		/// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <param name="entity">
        /// The entity.
		/// </param>
		/// <returns> 
        /// list of features indicating whether the specified mention is compatible with the pronouns
		/// of the specified entity.
		/// </returns>
        protected internal virtual List<string> GetPronounMatchFeatures(Mention.MentionContext mention, DiscourseEntity entity)
		{
			bool foundCompatiblePronoun = false;
			bool foundIncompatiblePronoun = false;
			if (PartsOfSpeech.IsPersOrPossPronoun(mention.HeadTokenTag))
			{
                Dictionary<string, string> pronounMap = GetPronounFeatureMap(mention.HeadTokenText);
				foreach (Mention.MentionContext candidateMention in entity.Mentions)
                {
					if (PartsOfSpeech.IsPersOrPossPronoun(candidateMention.HeadTokenTag))
					{
						if (mention.HeadTokenText.ToUpper() == candidateMention.HeadTokenText.ToUpper())
						{
							foundCompatiblePronoun = true;
							break;
						}
						else
						{
                            Dictionary<string, string> candidatePronounMap = GetPronounFeatureMap(candidateMention.HeadTokenText);
							bool allKeysMatch = true;
							foreach (string key in pronounMap.Keys)
                           {
								if (candidatePronounMap.ContainsKey(key))
								{
									if (pronounMap[key] != candidatePronounMap[key])
                                    {
										foundIncompatiblePronoun = true;
										allKeysMatch = false;
									}
								}
								else
								{
									allKeysMatch = false;
								}
							}
							if (allKeysMatch)
							{
								foundCompatiblePronoun = true;
							}
						}
					}
				}
			}
            var pronounFeatures = new List<string>();
			if (foundCompatiblePronoun)
			{
				pronounFeatures.Add("compatiblePronoun");
			}
			if (foundIncompatiblePronoun)
			{
				pronounFeatures.Add("incompatiblePronoun");
			}
			return pronounFeatures;
		}
		
		/// <summary>
        /// Returns string-match features for the the specified mention and entity.</summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <param name="entity">
        /// The entity.
		/// </param>
		/// <returns>
        /// list of string-match features for the the specified mention and entity.
		/// </returns>
        protected internal virtual List<string> GetStringMatchFeatures(Mention.MentionContext mention, DiscourseEntity entity)
		{
			bool sameHead = false;
			bool modifersMatch = false;
			bool titleMatch = false;
			bool noTheModifiersMatch = false;
            var features = new List<string>();
            Mention.IParse[] mentionTokens = mention.TokenParses;
			var entityContextModifierSet = ConstructModifierSet(mentionTokens, mention.HeadTokenIndex);
			string mentionHeadString = mention.HeadTokenText.ToLower();
			Util.Set<string> featureSet = new Util.HashSet<string>();

            foreach (Mention.MentionContext entityMention in entity.Mentions)
            {
				string exactMatchFeature = GetExactMatchFeature(entityMention, mention);
				if (exactMatchFeature != null)
				{
					featureSet.Add(exactMatchFeature);
				}
				else if (entityMention.Parse.IsCoordinatedNounPhrase && !mention.Parse.IsCoordinatedNounPhrase)
				{
					featureSet.Add("cmix");
				}
				else
				{
					string mentionStrip = StripNounPhrase(mention);
					string entityMentionStrip = StripNounPhrase(entityMention);
					if (mentionStrip != null && entityMentionStrip != null)
					{
						if (IsSubstring(mentionStrip, entityMentionStrip))
						{
							featureSet.Add("substring");
						}
					}
				}
                Mention.IParse[] entityMentionTokens = entityMention.TokenParses;
				int headIndex = entityMention.HeadTokenIndex;
				//if (!mention.getHeadTokenTag().equals(entityMention.getHeadTokenTag())) {
				//  continue;
				//}  want to match NN NNP
				string entityMentionHeadString = entityMention.HeadTokenText.ToLower();
				// model lexical similarity
				if (mentionHeadString == entityMentionHeadString)
				{
					sameHead = true;
					featureSet.Add("hds=" + mentionHeadString);
					if (!modifersMatch || !noTheModifiersMatch)
					{
						//only check if we haven't already found one which is the same
						modifersMatch = true;
						noTheModifiersMatch = true;
                        Util.Set<string> entityMentionModifierSet = ConstructModifierSet(entityMentionTokens, headIndex);
						foreach (string modifierWord in entityContextModifierSet)
                        {
                            if (!entityMentionModifierSet.Contains(modifierWord))
							{
								modifersMatch = false;
                                if (modifierWord != "the")
								{
									noTheModifiersMatch = false;
                                    featureSet.Add("mmw=" + modifierWord);
								}
							}
						}
					}
				}
                Util.Set<string> descriptorModifierSet = ConstructModifierSet(entityMentionTokens, entityMention.NonDescriptorStart);
				if (descriptorModifierSet.Contains(mentionHeadString))
				{
					titleMatch = true;
				}
			}
			if (featureSet.Count != 0)
			{
                features.AddRange(featureSet);
			}
			if (sameHead)
			{
				features.Add("sameHead");
				if (modifersMatch)
				{
					features.Add("modsMatch");
				}
				else if (noTheModifiersMatch)
				{
					features.Add("nonTheModsMatch");
				}
				else
				{
					features.Add("modsMisMatch");
				}
			}
			if (titleMatch)
			{
				features.Add("titleMatch");
			}
			return features;
		}

        private string MentionString(Mention.MentionContext entityContext)
		{
			var output = new StringBuilder();
			object[] mentionTokens = entityContext.Tokens;
			output.Append(mentionTokens[0].ToString());
			for (int tokenIndex = 1; tokenIndex < mentionTokens.Length; tokenIndex++)
			{
				string token = mentionTokens[tokenIndex].ToString();
				output.Append(" ").Append(token);
			}
			return output.ToString();
		}

        private string ExcludedTheMentionString(Mention.MentionContext entityContext)
		{
			var output = new StringBuilder();
			bool first = true;
			object[] mentionTokens = entityContext.Tokens;
			foreach (object tokenObj in mentionTokens)
			{
			    string token = tokenObj.ToString();
			    if (token != "the" && token != "The" && token != "THE")
			    {
			        if (!first)
			        {
			            output.Append(" ");
			        }
			        output.Append(token);
			        first = false;
			    }
			}
			return output.ToString();
		}

        private string ExcludedHonorificMentionString(Mention.MentionContext entityContext)
		{
			var output = new System.Text.StringBuilder();
			bool first = true;
			object[] mentionTokens = entityContext.Tokens;
			for (int tokenIndex = 0; tokenIndex < mentionTokens.Length; tokenIndex++)
			{
				string token = mentionTokens[tokenIndex].ToString();
                if (Linker.HonorificsPattern.Match(token).Value != token)
				{
					if (!first)
					{
						output.Append(" ");
					}
					output.Append(token);
					first = false;
				}
			}
			return output.ToString();
		}

        private string ExcludedDeterminerMentionString(Mention.MentionContext entityContext)
		{
			var output = new StringBuilder();
			bool first = true;
            Mention.IParse[] mentionTokenParses = entityContext.TokenParses;
			for (int tokenIndex = 0; tokenIndex < mentionTokenParses.Length; tokenIndex++)
			{
                Mention.IParse token = mentionTokenParses[tokenIndex];
				string tag = token.SyntacticType;
				if (tag != PartsOfSpeech.Determiner)
				{
					if (!first)
					{
						output.Append(" ");
					}
					output.Append(token.ToString());
					first = false;
				}
			}
			return output.ToString();
		}

        private string GetExactMatchFeature(Mention.MentionContext entityContext, Mention.MentionContext compareContext)
		{
			if (MentionString(entityContext).Equals(MentionString(compareContext)))
			{
				return "exactMatch";
			}
			else if (ExcludedHonorificMentionString(entityContext).Equals(ExcludedHonorificMentionString(compareContext)))
			{
				return "exactMatchNoHonor";
			}
			else if (ExcludedTheMentionString(entityContext).Equals(ExcludedTheMentionString(compareContext)))
			{
				return "exactMatchNoThe";
			}
			else if (ExcludedDeterminerMentionString(entityContext).Equals(ExcludedDeterminerMentionString(compareContext)))
			{
				return "exactMatchNoDT";
			}
			return null;
		}
		
		/// <summary>
        /// Returns a list of word features for the specified tokens.
        /// </summary>
		/// <param name="token">
        /// The token for which features are to be computed.
		/// </param>
		/// <returns>
        /// a list of word features for the specified tokens.
		/// </returns>
        public static List<string> GetWordFeatures(Mention.IParse token)
		{
            var wordFeatures = new List<string>();
			string word = token.ToString().ToLower();
			string wordFeature = string.Empty;
			if (EndsWithPeriod.IsMatch(word))
			{
                wordFeature = @",endWithPeriod";
            }
			
			string tokenTag = token.SyntacticType;
			wordFeatures.Add("w=" + word + ",t=" + tokenTag + wordFeature);
			wordFeatures.Add("t=" + tokenTag + wordFeature);
			return wordFeatures;
		}
	}
}