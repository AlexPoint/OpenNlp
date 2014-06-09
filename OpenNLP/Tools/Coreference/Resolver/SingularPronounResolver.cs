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

//This file is based on the SingularPronounResolver.java source file found in the
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
//UPGRADE_TODO: The type 'java.util.regex.Pattern' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using System.Text.RegularExpressions;
using DiscourseEntity = OpenNLP.Tools.Coreference.DiscourseEntity;
using Linker = OpenNLP.Tools.Coreference.ILinker;
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
using System.Collections.Generic;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	/// <summary> This class resolver singlular pronouns such as "he", "she", "it" and their various forms. </summary>
	public class SingularPronounResolver:MaximumEntropyResolver
	{
		
		internal int mode;
		
		internal Regex PronounPattern;
		
		public SingularPronounResolver(string projectName, ResolverMode mode):base(projectName, "pmodel", mode, 30)
		{
			NumberSentencesBack = 2;
		}
		
		public SingularPronounResolver(string projectName, ResolverMode mode, INonReferentialResolver nonReferentialResolver):base(projectName, "pmodel", mode, 30, nonReferentialResolver)
		{
			NumberSentencesBack = 2;
		}
		
		public override bool CanResolve(MentionContext mention)
		{
			//System.err.println("MaxentSingularPronounResolver.canResolve: ec= ("+mention.id+") "+ mention.toText());
			string tag = mention.HeadTokenTag;
			return (tag != null && tag.StartsWith("PRP") && Linker.SingularThirdPersonPronounPattern.IsMatch(mention.HeadTokenText));
		}
		
		protected internal override List<string> GetFeatures(MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
			
			if (entity != null)
			{
				//generate pronoun w/ referent features
				MentionContext cec = entity.LastExtent;
				//String gen = getPronounGender(pronoun);
                features.AddRange(GetPronounMatchFeatures(mention, entity));
                features.AddRange(GetContextFeatures(cec));
				features.AddRange(GetDistanceFeatures(mention, entity));
				features.Add(GetMentionCountFeature(entity));
				/*
				//lexical features
				Set featureSet = new HashSet();
				for (Iterator ei = entity.getExtents(); ei.hasNext();) {
				MentionContext ec = (MentionContext) ei.next();
				List toks = ec.tokens;
				Parse tok;
				int headIndex = PTBHeadFinder.getInstance().getHeadIndex(toks);
				for (int ti = 0; ti < headIndex; ti++) {
				tok = (Parse) toks.get(ti);
				featureSet.add(gen + "mw=" + tok.toString().toLowerCase());
				featureSet.add(gen + "mt=" + tok.getSyntacticType());
				}
				tok = (Parse) toks.get(headIndex);
				featureSet.add(gen + "hw=" + tok.toString().toLowerCase());
				featureSet.add(gen + "ht=" + tok.getSyntacticType());
				//semantic features
				if (ec.neType != null) {
				featureSet.add(gen + "," + ec.neType);
				}
				else {
				for (Iterator si = ec.synsets.iterator(); si.hasNext();) {
				Integer synset = (Integer) si.next();
				featureSet.add(gen + "," + synset);
				}
				}
				}
				Iterator fset = featureSet.iterator();
				while (fset.hasNext()) {
				String f = (String) fset.next();
				features.add(f);
				}
				*/
			}
			return (features);
		}

        protected internal override bool IsExcluded(MentionContext mention, DiscourseEntity entity)
		{
			if (base.IsExcluded(mention, entity))
			{
				return (true);
			}
			string mentionGender = null;
			
			foreach (MentionContext entityMention in entity.Mentions)
            {
				string tag = entityMention.HeadTokenTag;
				if (tag != null && tag.StartsWith("PRP") && Linker.SingularThirdPersonPronounPattern.IsMatch(mention.HeadTokenText))
				{
					if (mentionGender == null)
					{
						//lazy initilization
						mentionGender = GetPronounGender(mention.HeadTokenText);
					}
					string entityGender = GetPronounGender(entityMention.HeadTokenText);
					if (!entityGender.Equals("u") && !mentionGender.Equals(entityGender))
					{
						return (true);
					}
				}
			}
			return (false);
		}
		
		protected internal override bool IsOutOfRange(MentionContext mention, DiscourseEntity entity)
		{
			MentionContext cec = entity.LastExtent;
			//System.err.println("MaxentSingularPronounresolve.outOfRange: ["+entity.getLastExtent().toText()+" ("+entity.getId()+")] ["+mention.toText()+" ("+mention.getId()+")] entity.sentenceNumber=("+entity.getLastExtent().getSentenceNumber()+")-mention.sentenceNumber=("+mention.getSentenceNumber()+") > "+numSentencesBack);    
			return (mention.SentenceNumber - cec.SentenceNumber > NumberSentencesBack);
		}
		
		/*
		public boolean definiteArticle(String tok, String tag) {
		tok = tok.toLowerCase();
		if (tok.equals("the") || tok.equals("these")) {
		//|| tok.equals("these") || tag.equals("PRP$")) {
		return (true);
		}
		return (false);
		}
		*/
	}
}