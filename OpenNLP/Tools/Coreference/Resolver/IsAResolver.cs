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

//This file is based on the IsAResolver.java source file found in the
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
using System.Text.RegularExpressions;
using System.Collections.Generic;

using DiscourseEntity = OpenNLP.Tools.Coreference.DiscourseEntity;
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	/// <summary>  Resolves coreference between appositives. </summary>
	public class IsAResolver:MaximumEntropyResolver
	{
		
		internal Regex predicativePattern;
		
		public IsAResolver(string projectName, ResolverMode mode):base(projectName, "/imodel", mode, 20)
		{
			ShowExclusions = false;
			//predicativePattern = Pattern.compile("^(,|am|are|is|was|were|--)$");
			predicativePattern = new Regex("^(,|--)$", RegexOptions.Compiled);
		}

        public IsAResolver(string projectName, ResolverMode mode, INonReferentialResolver nonReferentialResolver) : base(projectName, "/imodel", mode, 20, nonReferentialResolver)
		{
			ShowExclusions = false;
			//predicativePattern = Pattern.compile("^(,|am|are|is|was|were|--)$");
            predicativePattern = new Regex("^(,|--)$", RegexOptions.Compiled);
		}
		
		public override bool CanResolve(MentionContext context)
		{
			if (PartsOfSpeech.IsNoun(context.HeadTokenTag))
			{
                return (context.PreviousToken != null && predicativePattern.IsMatch(context.PreviousToken.ToString()));
			}
			return false;
		}
		
		 protected internal override bool IsExcluded(MentionContext context, DiscourseEntity discourseEntity)
		{
			MentionContext currentContext = discourseEntity.LastExtent;
			if (context.SentenceNumber != currentContext.SentenceNumber)
			{
				return true;
			}
			//shallow parse appositives
			if (currentContext.IndexSpan.End == context.IndexSpan.Start - 2)
			{
				return false;
			}
			//full parse w/o trailing comma
			if (currentContext.IndexSpan.End == context.IndexSpan.End)
			{
				return false;
			}
			//full parse w/ trailing comma or period
			if (currentContext.IndexSpan.End <= context.IndexSpan.End + 2 && (context.NextToken != null 
                && (context.NextToken.ToString() == PartsOfSpeech.Comma || context.NextToken.ToString() == PartsOfSpeech.SentenceFinalPunctuation)))
			{
				return false;
			}
			return true;
		}
		
		protected internal override bool IsOutOfRange(MentionContext context, DiscourseEntity discourseEntity)
		{
			MentionContext currentContext = discourseEntity.LastExtent;
			return (currentContext.SentenceNumber != context.SentenceNumber);
		}
		
		protected internal override bool defaultReferent(DiscourseEntity discourseEntity)
		{
			return true;
		}

        protected internal override List<string> GetFeatures(MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
            
            if (entity != null)
			{
				MentionContext ant = entity.LastExtent;
                List<string> leftContexts = GetContextFeatures(ant);
				for (int ci = 0, cn = leftContexts.Count; ci < cn; ci++)
				{
					features.Add("l" + leftContexts[ci]);
				}
                List<string> rightContexts = GetContextFeatures(mention);
				for (int ci = 0, cn = rightContexts.Count; ci < cn; ci++)
				{
					features.Add("r" + rightContexts[ci]);
				}
				features.Add("hts" + ant.HeadTokenTag + "," + mention.HeadTokenTag);
			}
			/*
			if (entity != null) {
			//previous word and tag
			if (ant.prevToken != null) {
			features.add("pw=" + ant.prevToken);
			features.add("pt=" + ant.prevToken.getSyntacticType());
			}
			else {
			features.add("pw=<none>");
			features.add("pt=<none>");
			}
			
			//next word and tag
			if (mention.nextToken != null) {
			features.add("nw=" + mention.nextToken);
			features.add("nt=" + mention.nextToken.getSyntacticType());
			}
			else {
			features.add("nw=<none>");
			features.add("nt=<none>");
			}
			
			//modifier word and tag for c1
			int i = 0;
			List c1toks = ant.tokens;
			for (; i < ant.headTokenIndex; i++) {
			features.add("mw=" + c1toks.get(i));
			features.add("mt=" + ((Parse) c1toks.get(i)).getSyntacticType());
			}
			//head word and tag for c1
			features.add("mh=" + c1toks.get(i));
			features.add("mt=" + ((Parse) c1toks.get(i)).getSyntacticType());
			
			//modifier word and tag for c2
			i = 0;
			List c2toks = mention.tokens;
			for (; i < mention.headTokenIndex; i++) {
			features.add("mw=" + c2toks.get(i));
			features.add("mt=" + ((Parse) c2toks.get(i)).getSyntacticType());
			}
			//head word and tag for n2
			features.add("mh=" + c2toks.get(i));
			features.add("mt=" + ((Parse) c2toks.get(i)).getSyntacticType());
			
			//word/tag pairs
			for (i = 0; i < ant.headTokenIndex; i++) {
			for (int j = 0; j < mention.headTokenIndex; j++) {
			features.add("w=" + c1toks.get(i) + "|" + "w=" + c2toks.get(j));
			features.add("w=" + c1toks.get(i) + "|" + "t=" + ((Parse) c2toks.get(j)).getSyntacticType());
			features.add("t=" + ((Parse) c1toks.get(i)).getSyntacticType() + "|" + "w=" + c2toks.get(j));
			features.add("t=" + ((Parse) c1toks.get(i)).getSyntacticType() + "|" + "t=" + ((Parse) c2toks.get(j)).getSyntacticType());
			}
			}
			features.add("ht=" + ant.headTokenTag + "|" + "ht=" + mention.headTokenTag);
			features.add("ht1=" + ant.headTokenTag);
			features.add("ht2=" + mention.headTokenTag);
			*/
			//semantic categories
            /*
            if (ant.neType != null) {
            if (re.neType != null) {
            features.add("sc="+ant.neType+","+re.neType);
            }
            else if (!PartsOfSpeech.IsProperNoun(re.headTokenTag) && PartsOfSpeech.IsNoun(re.headTokenTag)) {
            Set synsets = re.synsets;
            for (Iterator si=synsets.iterator();si.hasNext();) {
            features.add("sc="+ant.neType+","+si.next());
            }
            }
            }
            else if (!PartsOfSpeech.IsProperNoun(ant.headTokenTag) && PartsOfSpeech.IsNoun(ant.headTokenTag)) {
            if (re.neType != null) {
            Set synsets = ant.synsets;
            for (Iterator si=synsets.iterator();si.hasNext();) {
            features.add("sc="+re.neType+","+si.next());
            }
            }
            else if (!PartsOfSpeech.IsProperNoun(re.headTokenTag) && PartsOfSpeech.IsNoun(re.headTokenTag)) {
            Set synsets1 = ant.synsets;
            Set synsets2 = re.synsets;
            for (Iterator si=synsets1.iterator();si.hasNext();) {
            Object synset = si.next();
            if (synsets2.contains(synset)) {
            features.add("sc="+synset);
            }
            }
            }
            }
            }
            */
            return features;
		}
	}
}