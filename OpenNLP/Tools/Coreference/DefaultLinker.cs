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

//This file is based on the DefaultLinker.java source file found in the
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
using OpenNLP.Tools.Coreference.Mention;
using OpenNLP.Tools.Coreference.Resolver;
using OpenNLP.Tools.Coreference.Similarity;

namespace OpenNLP.Tools.Coreference
{
	/// <summary>
    /// This class perform coreference for treebank style parses or for noun-phrase chunked data.    
	/// Non-constituent entites such as pre-nominal named-entities and sub entities in simple coordinated
	/// noun phases will be created.  This linker requires that named-entity information also be provided.   
	/// This information can be added to the parse using the -parse option with EnglishNameFinder. 
	/// </summary>
	public class DefaultLinker : AbstractLinker
	{
        private MaximumEntropyCompatibilityModel mCompatibilityModel;

        protected internal MaximumEntropyCompatibilityModel CompatibilityModel
        {
            get
            { 
                return mCompatibilityModel; 
            }
            set
            {
                mCompatibilityModel = value;
            }
        }
	
		/// <summary>
        /// Creates a new linker with the specified model directory, running in the specified mode.
        /// </summary>
		/// <param name="modelDirectory">
        /// The directory where the models for this linker are kept.
		/// </param>
		/// <param name="mode">
        /// The mode that this linker is running in.
		/// </param>
		public DefaultLinker(string modelDirectory, LinkerMode mode) : this(modelDirectory, mode, true, -1)
		{
		}
		
		/// <summary>
        /// Creates a new linker with the specified model directory, running in the specified mode which uses a discourse model
		/// based on the specified parameter.
		/// </summary>
		/// <param name="modelDirectory">
        /// The directory where the models for this linker are kept.
		/// </param>
		/// <param name="mode">
        /// The mode that this linker is running in.
		/// </param>
		/// <param name="useDiscourseModel">
        /// Whether the model should use a discourse model or not.
		/// </param>
		public DefaultLinker(string modelDirectory, LinkerMode mode, bool useDiscourseModel) : this(modelDirectory, mode, useDiscourseModel, -1)
		{
		}
		
		/// <summary>
        /// Creates a new linker with the specified model directory, running in the specified mode which uses a discourse model
		/// based on the specified parameter and uses the specified fixed non-referential probability.
		/// </summary>
		/// <param name="modelDirectory">
        /// The directory where the models for this linker are kept.
		/// </param>
		/// <param name="mode">
        /// The mode that this linker is running in.
		/// </param>
		/// <param name="useDiscourseModel">
        /// Whether the model should use a discourse model or not.
		/// </param>
		/// <param name="fixedNonReferentialProbability">
        /// The probability which resolvers are required to exceed to posit a coreference relationship.
		/// </param>
		public DefaultLinker(string modelDirectory, LinkerMode mode, bool useDiscourseModel, double fixedNonReferentialProbability) : base(modelDirectory, mode, useDiscourseModel)
		{
			if (mode != LinkerMode.Sim)
			{
				mCompatibilityModel = new MaximumEntropyCompatibilityModel(CoreferenceProjectName);
			}
			InitializeHeaderFinder();
			InitializeMentionFinder();
			if (mode != LinkerMode.Sim)
			{
				InitializeResolvers(mode, fixedNonReferentialProbability);
                Entities = new DiscourseEntity[Resolvers.Length];
			}
		}
		
		/// <summary>
        /// Initializes the resolvers used by this linker.
        /// </summary>
		/// <param name="mode">
        /// The mode in which this linker is being used.
		/// </param>
		/// <param name="fixedNonReferentialProbability">
		/// </param>
		protected internal virtual void InitializeResolvers(LinkerMode mode, double fixedNonReferentialProbability)
		{
			if (mode == LinkerMode.Train)
			{
				MentionFinder.PrenominalNamedEntitiesCollection = false;
				MentionFinder.CoordinatedNounPhrasesCollection = false;
			}
			SingularPronounIndex = 0;
			if (LinkerMode.Test == mode || LinkerMode.Eval == mode)
			{
				if (fixedNonReferentialProbability < 0)
				{
                    Resolvers = new MaximumEntropyResolver[] { new SingularPronounResolver(CoreferenceProjectName, ResolverMode.Test), new ProperNounResolver(CoreferenceProjectName, ResolverMode.Test), new DefiniteNounResolver(CoreferenceProjectName, ResolverMode.Test), new IsAResolver(CoreferenceProjectName, ResolverMode.Test), new PluralPronounResolver(CoreferenceProjectName, ResolverMode.Test), new PluralNounResolver(CoreferenceProjectName, ResolverMode.Test), new CommonNounResolver(CoreferenceProjectName, ResolverMode.Test), new SpeechPronounResolver(CoreferenceProjectName, ResolverMode.Test) };
				}
				else
				{
					INonReferentialResolver nrr = new FixedNonReferentialResolver(fixedNonReferentialProbability);
                    Resolvers = new MaximumEntropyResolver[] { new SingularPronounResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new ProperNounResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new DefiniteNounResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new IsAResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new PluralPronounResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new PluralNounResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new CommonNounResolver(CoreferenceProjectName, ResolverMode.Test, nrr), new SpeechPronounResolver(CoreferenceProjectName, ResolverMode.Test, nrr) };
				}
				if (LinkerMode.Eval == mode)
				{
					//String[] names = {"Pronoun", "Proper", "Def-NP", "Is-a", "Plural Pronoun"};
					//eval = new Evaluation(names);
				}
                MaximumEntropyResolver.SimilarityModel = SimilarityModel.TestModel(CoreferenceProjectName + "/sim");
			}
			else if (LinkerMode.Train == mode)
			{
				Resolvers = new AbstractResolver[9];
				Resolvers[0] = new SingularPronounResolver(CoreferenceProjectName, ResolverMode.Train);
				Resolvers[1] = new ProperNounResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[2] = new DefiniteNounResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[3] = new IsAResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[4] = new PluralPronounResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[5] = new PluralNounResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[6] = new CommonNounResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[7] = new SpeechPronounResolver(CoreferenceProjectName, ResolverMode.Train);
                Resolvers[8] = new PerfectResolver();
			}
			else
			{
				System.Console.Error.WriteLine("DefaultLinker: Invalid Mode");
			}
		}
		
		/// <summary> 
        /// Initializes the head finder for this linker.
        /// </summary>
		protected internal virtual void InitializeHeaderFinder()
		{
            HeadFinder = Mention.PennTreebankHeadFinder.Instance;
		}
		/// <summary> 
        /// Initializes the mention finder for this linker.  
		/// This can be overridden to change the space of mentions used for coreference. 
		/// </summary>
		protected internal virtual void InitializeMentionFinder()
		{
			MentionFinder = ShallowParseMentionFinder.GetInstance(HeadFinder);
		}

        protected internal override Gender ComputeGender(Mention.MentionContext mention)
		{
			return mCompatibilityModel.ComputeGender(mention);
		}

        protected internal override Number ComputeNumber(Mention.MentionContext mention)
		{
			return mCompatibilityModel.ComputeNumber(mention);
		}
	}
}