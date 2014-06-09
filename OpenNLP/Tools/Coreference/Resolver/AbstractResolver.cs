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

//This file is based on the AbstractResolver.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Resolver
{
	/// <summary>
    /// Default implementation of some methods in the {@link IResolver} interface.
    /// </summary>
	public abstract class AbstractResolver : IResolver
	{
        /// <summary>
        /// The number of previous entities that resolver should consider.
        /// </summary>
        private int mNumberEntitiesBack;
        /// <summary>
        /// Debugging variable which specifies whether error output is generated if a class excludes as possibly coreferent mentions which are in-fact coreferent.
        /// </summary>
        private bool mShowExclusions;
        /// <summary>
        /// Debugging variable which holds statistics about mention distances durring training.
        /// </summary>
        private Util.CountedSet<int> mDistances;
        /// <summary>
        /// The number of sentences back this resolver should look for a referent.
        /// </summary>
        private int mNumberSentencesBack;
		
		/// <summary>
        /// The number of sentences back this resolver should look for a referent.
        /// </summary>
        protected internal virtual int NumberSentencesBack
        {
            get
            {
                return mNumberSentencesBack;
            }
            set
            {
                mNumberSentencesBack = value;
            }
        }

        /// <summary>
        /// Debugging variable which holds statistics about mention distances durring training.
        /// </summary>
        protected internal virtual bool ShowExclusions
        {
            get
            {
                return mShowExclusions;
            }
            set
            {
                mShowExclusions = value;
            }
        }

        /// <summary>
        /// Debugging variable which holds statistics about mention distances durring training.
        /// </summary>
        protected internal virtual Util.CountedSet<int> Distances
        {
            get
            {
                return mDistances;
            }
            set
            {
                mDistances = value;
            }
        }

		protected AbstractResolver(int numberEntitiesBack)
		{
            mNumberEntitiesBack = numberEntitiesBack;
			mShowExclusions = true;
			mDistances = new Util.CountedSet<int>();
		}
		
		/// <summary>
        /// Returns the number of previous entities that resolver should consider.
        /// </summary>
		/// <returns>
        /// the number of previous entities that resolver should consider.
		/// </returns>
		protected internal virtual int GetNumberEntitiesBack()
		{
			return mNumberEntitiesBack;
		}
		
		/// <summary>
        /// The number of entites that should be considered for resolution with the specified discourse model.
        /// </summary>
		/// <param name="discourseModel">
        /// The discourse model.
		/// </param>
		/// <returns>
        /// number of entites that should be considered for resolution.
		/// </returns>
        protected internal virtual int GetNumberEntitiesBack(DiscourseModel discourseModel)
		{
            return System.Math.Min(discourseModel.EntityCount, mNumberEntitiesBack);
		}
		
		/// <summary>
        /// Returns the head parse for the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <returns>
        /// the head parse for the specified mention.
		/// </returns>
        protected internal virtual Mention.IParse GetHead(Mention.MentionContext mention)
		{
			return mention.HeadTokenParse;
		}
		
		/// <summary>
        /// Returns the index for the head word for the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <returns>
        /// the index for the head word for the specified mention.
		/// </returns>
        protected internal virtual int GetHeadIndex(Mention.MentionContext mention)
		{
            Mention.IParse[] mentionTokens = mention.TokenParses;
			for (int currentToken = mentionTokens.Length - 1; currentToken >= 0; currentToken--)
			{
                Mention.IParse token = mentionTokens[currentToken];
				if (token.SyntacticType != "POS" && token.SyntacticType != "," && token.SyntacticType != ".")
				{
					return currentToken;
				}
			}
			return mentionTokens.Length - 1;
		}
		
		/// <summary>
        /// Returns the text of the head word for the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <returns>
        /// The text of the head word for the specified mention.
		/// </returns>
        protected internal virtual string GetHeadString(Mention.MentionContext mention)
		{
			return mention.HeadTokenText.ToLower();
		}
		
		/// <summary>
        /// Determines if the specified entity is too far from the specified mention to be resolved to it.  
		/// Once an entity has been determined to be out of range subsequent entities are not considered.
		/// </summary>
		/// <seealso cref="IsExcluded">
		/// </seealso>
		/// <param name="mention">
        /// The mention which is being considered.
		/// </param>
		/// <param name="entity">
        /// The entity to which the mention is to be resolved.
		/// </param>
		/// <returns>
        /// true is the entity is in range of the mention, false otherwise.
		/// </returns>
        protected internal virtual bool IsOutOfRange(Mention.MentionContext mention, DiscourseEntity entity)
		{
			return false;
		}
		
		/// <summary>
        /// Excludes entities which you are not compatible with the entity under consideration.  The default 
		/// implementation excludes entties whose last extent contains the extent under consideration.
		/// This prevents posessive pronouns from referring to the noun phrases they modify and other 
		/// undesirable things.
		/// </summary>
		/// <param name="mention">
        /// The mention which is being considered as referential.
		/// </param>
		/// <param name="entity">
        /// The entity to which the mention is to be resolved.
		/// </param>
		/// <returns>
        /// true if the entity should be excluded, false otherwise.
		/// </returns>
		protected internal virtual bool IsExcluded(Mention.MentionContext mention, DiscourseEntity entity)
		{
            Mention.MentionContext context = entity.LastExtent;
            return mention.SentenceNumber == context.SentenceNumber && mention.IndexSpan.End <= context.IndexSpan.End;
		}

        public virtual DiscourseEntity Retain(Mention.MentionContext mention, DiscourseModel discourseModel)
		{
			int entityIndex = 0;
			if (mention.Id == - 1)
			{
				return null;
			}
			for (; entityIndex < discourseModel.EntityCount; entityIndex++)
			{
				DiscourseEntity currentDiscourseEntity = discourseModel.GetEntity(entityIndex);
                Mention.MentionContext candidateExtentContext = currentDiscourseEntity.LastExtent;
				if (candidateExtentContext.Id == mention.Id)
				{
					Distances.Add(entityIndex);
					return currentDiscourseEntity;
				}
			}
			//System.err.println("AbstractResolver.Retain: non-referring entity with id: "+ec.toText()+" id="+ec.id);
			return null;
		}
		
		/// <summary>
        /// Returns the string of "_" delimited tokens for the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The mention.
		/// </param>
		/// <returns>
        /// the string of "_" delimited tokens for the specified mention.
		/// </returns>
        protected internal virtual string GetFeatureString(Mention.MentionContext mention)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			object[] mentionTokens = mention.Tokens;
			output.Append(mentionTokens[0].ToString());
            for (int currentToken = 1; currentToken < mentionTokens.Length; currentToken++)
			{
				output.Append("_").Append(mentionTokens[currentToken].ToString());
			}
			return output.ToString();
		}
		
		/// <summary>
        /// Returns a string for the specified mention with punctuation, honorifics, designators, and determiners removed.
        /// </summary>
		/// <param name="mention">
        /// The mention to be stripped.
		/// </param>
		/// <returns>
        /// a normalized string representation of the specified mention.
		/// </returns>
        protected internal virtual string StripNounPhrase(Mention.MentionContext mention)
		{
			int start = mention.NonDescriptorStart; //start after descriptors

            Mention.IParse[] mentionTokens = mention.TokenParses;
			int end = mention.HeadTokenIndex + 1;
			if (start == end)
			{
				//System.err.println("StripNounPhrase: return null 1");
                return null;
			}
			//strip determiners
			if (mentionTokens[start].SyntacticType == "DT")
			{
				start++;
			}
			if (start == end)
			{
                //System.err.println("StripNounPhrase: return null 2");
				return null;
			}
			//get to first NNP
			string type;
			for (int index = start; index < end; index++)
			{
				type = mentionTokens[start].SyntacticType;
				if (type.StartsWith("NNP"))
				{
					break;
				}
				start++;
			}
			if (start == end)
			{
                //System.err.println("StripNounPhrase: return null 3");
				return null;
			}
			if (start + 1 != end)
			{
				// don't do this on head words, to keep "U.S."
				//strip off honorifics in begining
                if (Linker.HonorificsPattern.IsMatch(mentionTokens[start].ToString()))
				{
					start++;
				}
				if (start == end)
				{
                    //System.err.println("StripNounPhrase: return null 4");
					return null;
				}
				//strip off and honorifics on the end
                if (Linker.DesignatorsPattern.IsMatch(mentionTokens[mentionTokens.Length - 1].ToString()))
				{
					end--;
				}
			}
			if (start == end)
			{
                //System.err.println("StripNounPhrase: return null 5");
				return null;
			}
            System.Text.StringBuilder strip = new System.Text.StringBuilder();
			for (int i = start; i < end; i++)
			{
				strip.Append(mentionTokens[i].ToString()).Append(" ");
			}
			return strip.ToString().Trim();
		}
		
		
		public virtual void Train()
		{
		}
		
		/// <summary>
        /// Returns a string representing the gender of the specifed pronoun.
        /// </summary>
		/// <param name="pronoun">
        /// An English pronoun. 
		/// </param>
		/// <returns>
        /// the gender of the specifed pronoun.
		/// </returns>
		public static string GetPronounGender(string pronoun)
		{
            //java uses "Matcher.matches" to check if the whole string matches the pattern
            if (Linker.MalePronounPattern.IsMatch(pronoun))
			{
				return "m";
			}
            else if (Linker.FemalePronounPattern.IsMatch(pronoun))
			{
				return "f";
			}
            else if (Linker.NeuterPronounPattern.IsMatch(pronoun))
			{
				return "n";
			}
			else
			{
				return "u";
			}
		}
		
		public abstract bool CanResolve(Mention.MentionContext mention);
		public abstract DiscourseEntity Resolve(Mention.MentionContext expression, DiscourseModel discourseModel);
	}
}