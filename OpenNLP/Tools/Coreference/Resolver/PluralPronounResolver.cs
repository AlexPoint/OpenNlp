///////////////////////////////////////////////////////////////////////////////
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
//////////////////////////////////////////////////////////////////////////////
using System;
using DiscourseEntity = OpenNLP.Tools.Coreference.DiscourseEntity;
using Linker = OpenNLP.Tools.Coreference.ILinker;
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
using System.Collections.Generic;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	/// <summary> Resolves coreference between plural pronouns and their referents.</summary>
	public class PluralPronounResolver:MaximumEntropyResolver
	{
		
		internal int NUM_SENTS_BACK_PRONOUNS = 2;
		
		public PluralPronounResolver(string projectName, ResolverMode mode) : base(projectName, "tmodel", mode, 30)
		{
		}

        public PluralPronounResolver(string projectName, ResolverMode mode, INonReferentialResolver nonReferentialResolver) : base(projectName, "tmodel", mode, 30, nonReferentialResolver)
		{
		}
		
		protected internal override List<string> GetFeatures(MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
			
			//features.add("eid="+pc.id);
			if (entity != null)
			{
				//generate pronoun w/ referent features
                features.AddRange(GetPronounMatchFeatures(mention, entity));
				MentionContext cec = entity.LastExtent;
                features.AddRange(GetDistanceFeatures(mention, entity));
                features.AddRange(GetContextFeatures(cec));
				features.Add(GetMentionCountFeature(entity));
				/*
				//lexical features
				Set featureSet = new HashSet();
				for (Iterator ei = entity.getExtents(); ei.hasNext();) {
				MentionContext ec = (MentionContext) ei.next();
				int headIndex = PTBHeadFinder.getInstance().getHeadIndex(ec.tokens);
				Parse tok = (Parse) ec.tokens.get(headIndex);
				featureSet.add("hw=" + tok.ToString().toLowerCase());
				if (ec.parse.isCoordinatedNounPhrase()) {
				featureSet.add("ht=CC");
				}
				else {
				featureSet.add("ht=" + tok.getSyntacticType());
				}
				if (ec.neType != null){
				featureSet.add("ne="+ec.neType);
				}
				}
				Iterator fset = featureSet.iterator();
				while (fset.hasNext()) {
				string f = (string) fset.next();
				features.add(f);
				}
				*/
			}
			return features;
		}
		
		protected internal override bool IsOutOfRange(MentionContext mention, DiscourseEntity entity)
		{
			MentionContext cec = entity.LastExtent;
			return (mention.SentenceNumber - cec.SentenceNumber > NUM_SENTS_BACK_PRONOUNS);
		}
		
		public override bool CanResolve(MentionContext mention)
		{
			string tag = mention.HeadTokenTag;
            return (tag != null && PartsOfSpeech.IsPersOrPossPronoun(tag) && Linker.PluralThirdPersonPronounPattern.IsMatch(mention.HeadTokenText));
		}
	}
}