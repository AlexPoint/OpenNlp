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

//This file is based on the AbstractLinker.java source file found in the
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
    /// Provides a default implementation of many of the methods in <code>ILinker</code> that
	/// most implementations of <code>ILinker</code> will want to extend.  
	/// </summary>
	public abstract class AbstractLinker : ILinker
	{
		public virtual IHeadFinder HeadFinder
		{
			get
			{
				return mHeadFinder;
			}
            protected internal set
            {
                mHeadFinder = value;
            }
		}

		public virtual IMentionFinder MentionFinder
		{
			get
			{
				return mMentionFinder;
			}
            protected internal set
            {
                mMentionFinder = value;
            }
		}

        protected internal AbstractResolver[] Resolvers
        {
            get
            {
                return mResolvers;
            }
            set
            {
                mResolvers = value;
            }
        }

        protected internal DiscourseEntity[] Entities
        {
            get
            {
                return mEntities;
            }
            set
            {
                mEntities = value;
            }
        }

        protected internal int SingularPronounIndex
        {
            get
            {
                return mSingularPronounIndex;
            }
            set
            {
                mSingularPronounIndex = value;
            }
        }

        protected internal string CoreferenceProjectName
        {
            get
            {
                return mCoreferenceProjectName;
            }
            set
            {
                mCoreferenceProjectName = value;
            }
        }

        protected internal bool UseDiscourseModel
        {
            get
            {
                return mUseDiscourseModel;
            }
            set
            {
                mUseDiscourseModel = value;
            }
        }

        protected internal bool RemoveUnresolvedMentions
        {
            get
            {
                return mRemoveUnresolvedMentions;
            }
            set
            {
                mRemoveUnresolvedMentions = value;
            }
        }

		/// <summary>
        /// The mention finder used to find mentions.
        /// </summary>
		private IMentionFinder mMentionFinder;
		
		/// <summary>The mode in which this linker is running. </summary>
		private LinkerMode mMode;
		
		/// <summary>The resolvers used by this Linker. </summary>
		private AbstractResolver[] mResolvers;
		
		/// <summary>Array used to store the results of each call made to the linker. </summary>
		private DiscourseEntity[] mEntities;
		
		/// <summary>The index of resolver which is used for singular pronouns. </summary>
		private int mSingularPronounIndex;
		
		/// <summary>The name of the project where the coreference models are stored. </summary>
		private string mCoreferenceProjectName;
		
		/// <summary>The head finder used in this linker. </summary>
		private IHeadFinder mHeadFinder;
		
		/// <summary>Specifies whether coreferent mentions should be combined into a single entity. 
		/// Set this to true to combine them, false otherwise.  
		/// </summary>
        private bool mUseDiscourseModel;
		
		/// <summary>Specifies whether mentions for which no resolver can be used should be added to the
		/// discourse model.
		/// </summary>
        private bool mRemoveUnresolvedMentions;

		/// <summary> 
        /// Creates a new linker using the models in the specified project directory and using the specified mode.
        /// </summary>
		/// <param name="project">
        /// The location of the models or other data needed by this linker.
		/// </param>
		/// <param name="mode">
        /// The mode the linker should be run in: testing, training, or evaluation.
		/// </param>
		protected AbstractLinker(string project, LinkerMode mode) : this(project, mode, true)
		{
		}
		
		/// <summary> 
        /// Creates a new linker using the models in the specified project directory, using the specified mode, 
		/// and combining coreferent entities based on the specified value.
		/// </summary>
		/// <param name="project">
        /// The location of the models or other data needed by this linker.
		/// </param>
		/// <param name="mode">
        /// The mode the linker should be run in: testing, training, or evaluation.
		/// </param>
		/// <param name="useDiscourseModel">
        /// Specifies whether coreferent mention should be combined or not.
		/// </param>
		protected AbstractLinker(string project, LinkerMode mode, bool useDiscourseModel)
		{
			mCoreferenceProjectName = project;
			mMode = mode;
			mSingularPronounIndex = -1;
			mUseDiscourseModel = useDiscourseModel;
			mRemoveUnresolvedMentions = true;
		}
		
		/// <summary>
        /// Removes the specified mention to an entity in the specified discourse model or creates a new entity for the mention.
        /// </summary>
		/// <param name="mention">
        /// The mention to resolve.
		/// </param>
		/// <param name="discourseModel">
        /// The discourse model of existing entities.
		/// </param>
		protected internal virtual void Resolve(MentionContext mention, DiscourseModel discourseModel)
		{
			bool validEntity = true; // true if we should add this entity to the dm
			bool canResolve = false;
			
			for (int currentResolver = 0; currentResolver < mResolvers.Length; currentResolver++)
			{
				if (mResolvers[currentResolver].CanResolve(mention))
				{
					if (mMode == LinkerMode.Test)
					{
						mEntities[currentResolver] = mResolvers[currentResolver].Resolve(mention, discourseModel);
						canResolve = true;
					}
					else if (mMode == LinkerMode.Train)
					{
						mEntities[currentResolver] = mResolvers[currentResolver].Retain(mention, discourseModel);
						if (currentResolver + 1 != mResolvers.Length)
						{
							canResolve = true;
						}
					}
					else if (mMode == LinkerMode.Eval)
					{
						mEntities[currentResolver] = mResolvers[currentResolver].Retain(mention, discourseModel);
						//DiscourseEntity rde = resolvers[ri].resolve(mention, discourseModel);
						//eval.update(rde == entities[ri], ri, entities[ri], rde);
					}
					else
					{
						System.Console.Error.WriteLine("AbstractLinker.Unknown mode: " + mMode);
					}
					if (currentResolver == mSingularPronounIndex && mEntities[currentResolver] == null)
					{
						validEntity = false;
					}
				}
				else
				{
					mEntities[currentResolver] = null;
				}
			}
			if (!canResolve && mRemoveUnresolvedMentions)
			{
				validEntity = false;
			}
			DiscourseEntity discourseEntity = CheckForMerges(discourseModel, mEntities);
			if (validEntity)
			{
				UpdateExtent(discourseModel, mention, discourseEntity, mUseDiscourseModel);
			}
		}
		
		/// <summary>
        /// Updates the specified discourse model with the specified mention as coreferent with the specified entity. 
        /// </summary>
		/// <param name="discourseModel">
        /// The discourse model
		/// </param>
		/// <param name="mention">
        /// The mention to be added to the specified entity.
		/// </param>
		/// <param name="entity">
        /// The entity which is mentioned by the specified mention.  
		/// </param>
		/// <param name="useDiscourseModel">
        /// Whether the mentions should be kept as an entiy or simply co-indexed.
		/// </param>
		protected internal virtual void UpdateExtent(DiscourseModel discourseModel, MentionContext mention, DiscourseEntity entity, bool useDiscourseModel)
		{
			if (useDiscourseModel)
			{
				if (entity != null)
				{
					if (entity.GenderProbability < mention.GenderProbability)
					{
						entity.Gender = mention.GetGender();
						entity.GenderProbability = mention.GenderProbability;
					}
					if (entity.NumberProbability < mention.NumberProbability)
					{
						entity.Number = mention.GetNumber();
						entity.NumberProbability = mention.NumberProbability;
					}
					entity.AddMention(mention);
					discourseModel.MentionEntity(entity);
				}
				else
				{
					entity = new DiscourseEntity(mention, mention.GetGender(), mention.GenderProbability, mention.GetNumber(), mention.NumberProbability);
					discourseModel.AddEntity(entity);
				}
			}
			else
			{
				if (entity != null)
				{
					var newEntity = new DiscourseEntity(mention, mention.GetGender(), mention.GenderProbability, mention.GetNumber(), mention.NumberProbability);
					discourseModel.AddEntity(newEntity);
					newEntity.Id = entity.Id;
				}
				else
				{
					var newEntity = new DiscourseEntity(mention, mention.GetGender(), mention.GenderProbability, mention.GetNumber(), mention.NumberProbability);
					discourseModel.AddEntity(newEntity);
				}
			}
		}
		
		protected internal virtual DiscourseEntity CheckForMerges(DiscourseModel discourseModel, DiscourseEntity[] discourseEntities)
		{
		    DiscourseEntity firstDiscourseEntity = discourseEntities[0];
			for (int discourseEntityIndex = 1; discourseEntityIndex < discourseEntities.Length; discourseEntityIndex++)
			{
			    DiscourseEntity secondDiscourseEntity = discourseEntities[discourseEntityIndex]; //temporary variable
			    if (secondDiscourseEntity != null)
				{
					if (firstDiscourseEntity != null && firstDiscourseEntity != secondDiscourseEntity)
					{
						discourseModel.MergeEntities(firstDiscourseEntity, secondDiscourseEntity, 1);
					}
					else
					{
						firstDiscourseEntity = secondDiscourseEntity;
					}
				}
			}
		    return firstDiscourseEntity;
		}

        public virtual DiscourseEntity[] GetEntitiesFromMentions(Mention.Mention[] mentions)
		{
			MentionContext[] extentContexts = ConstructMentionContexts(mentions);
			var discourseModel = new DiscourseModel();
			for (int extentIndex = 0; extentIndex < extentContexts.Length; extentIndex++)
			{
				Resolve(extentContexts[extentIndex], discourseModel);
			}
			return discourseModel.Entities;
		}

        public virtual void SetEntitiesFromMentions(Mention.Mention[] mentions)
		{
			GetEntitiesFromMentions(mentions);
		}
		
		public virtual void Train()
		{
			for (int resolverIndex = 0; resolverIndex < mResolvers.Length; resolverIndex++)
			{
				mResolvers[resolverIndex].Train();
			}
		}

        public virtual MentionContext[] ConstructMentionContexts(Mention.Mention[] mentions)
		{
            if (mentions == null)
            {
                throw new ArgumentNullException("mentions");
            }

			int mentionInSentenceIndex = -1;
			int mentionsInSentenceCount = -1;
			int previousSentenceIndex = -1;
			var contexts = new MentionContext[mentions.Length];
			for (int mentionIndex = 0, mentionCount = mentions.Length; mentionIndex < mentionCount; mentionIndex++)
			{
				IParse mentionParse = mentions[mentionIndex].Parse;
				if (mentionParse == null)
				{
					Console.Error.WriteLine("no parse for " + mentions[mentionIndex]);
				}
				int sentenceIndex = mentionParse.SentenceNumber;
				if (sentenceIndex != previousSentenceIndex)
				{
					mentionInSentenceIndex = 0;
					previousSentenceIndex = sentenceIndex;
					mentionsInSentenceCount = 0;
                    for (int currentMentionInSentence = mentionIndex; currentMentionInSentence < mentions.Length; currentMentionInSentence++)
					{
                        if (sentenceIndex != mentions[currentMentionInSentence].Parse.SentenceNumber)
						{
							break;
						}
						mentionsInSentenceCount++;
					}
				}
				contexts[mentionIndex] = new MentionContext(mentions[mentionIndex], mentionInSentenceIndex, mentionsInSentenceCount, mentionIndex, sentenceIndex, HeadFinder);
				contexts[mentionIndex].Id = mentions[mentionIndex].Id;
				mentionInSentenceIndex++;
				if (mMode != LinkerMode.Sim)
				{
					Gender gender = ComputeGender(contexts[mentionIndex]);
                    contexts[mentionIndex].SetGender(gender.Type, gender.Confidence);
					Number number = ComputeNumber(contexts[mentionIndex]);
                    contexts[mentionIndex].SetNumber(number.Type, number.Confidence);
				}
			}
			return contexts;
		}
		
		protected internal abstract Gender ComputeGender(MentionContext mention);
		protected internal abstract Number ComputeNumber(MentionContext mention);
	}
}