///////////////////////////////////////////////////////////////////////////////
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

//This file is based on the SpeechPronounResolver.java source file found in the
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
using DiscourseEntity = OpenNLP.Tools.Coreference.DiscourseEntity;
using Linker = OpenNLP.Tools.Coreference.ILinker;
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
using System.Collections.Generic;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	/// <summary> Resolves pronouns specific to quoted speech such as "you", "me", and "I".  </summary>
	public class SpeechPronounResolver:MaximumEntropyResolver
	{
		
		public SpeechPronounResolver(string projectName, ResolverMode mode):base(projectName, "fmodel", mode, 30)
		{
			NumberSentencesBack = 0;
			ShowExclusions = false;
			PreferFirstReferent = true;
		}

        public SpeechPronounResolver(string projectName, ResolverMode mode, INonReferentialResolver nonReferentialResolver)
            : base(projectName, "fmodel", mode, 30, nonReferentialResolver)
		{
			ShowExclusions = false;
			PreferFirstReferent = true;
		}


        protected internal override List<string> GetFeatures(MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
			
			if (entity != null)
			{
				features.AddRange(GetPronounMatchFeatures(mention, entity));
				List<string> contexts = GetContextFeatures(mention);
				MentionContext cec = entity.LastExtent;
				if (mention.HeadTokenTag.StartsWith("PRP") && cec.HeadTokenTag.StartsWith("PRP"))
				{
					features.Add(mention.HeadTokenText + "," + cec.HeadTokenText);
				}
				else if (mention.HeadTokenText.StartsWith("NNP"))
				{
					for (int ci = 0, cl = contexts.Count; ci < cl; ci++)
					{
						features.Add(contexts[ci]);
					}
					features.Add(mention.NameType + "," + cec.HeadTokenText);
				}
				else
				{
					List<string> ccontexts = GetContextFeatures(cec);
					for (int ci = 0, cl = ccontexts.Count; ci < cl; ci++)
					{
						features.Add(ccontexts[ci]);
					}
                    features.Add(cec.NameType + "," + mention.HeadTokenText);
				}
			}
			return (features);
		}
		
		protected internal override bool IsOutOfRange(MentionContext mention, DiscourseEntity entity)
		{
			MentionContext cec = entity.LastExtent;
			return (mention.SentenceNumber - cec.SentenceNumber > NumberSentencesBack);
		}
		
		public override bool CanResolve(MentionContext mention)
		{
			string tag = mention.HeadTokenTag;
			bool fpp = tag != null && tag.StartsWith("PRP") && Linker.SpeechPronounPattern.IsMatch(mention.HeadTokenText);
			bool pn = tag != null && tag.StartsWith("NNP");
			return (fpp || pn);
		}
		
		//UPGRADE_NOTE: Access modifiers of method 'excluded' were changed to 'public'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1204'"
        protected internal override bool IsExcluded(MentionContext mention, DiscourseEntity entity)
		{
			if (base.IsExcluded(mention, entity))
			{
				return true;
			}
			MentionContext cec = entity.LastExtent;
			if (!CanResolve(cec))
			{
				return true;
			}
			if (mention.HeadTokenTag.StartsWith("NNP"))
			{
				//mention is a propernoun
				if (cec.HeadTokenTag.StartsWith("NNP"))
				{
					return true; // both NNP
				}
				else
				{
					if (entity.MentionCount > 1)
					{
						return true;
					}
					return !CanResolve(cec);
				}
			}
			else if (mention.HeadTokenTag.StartsWith("PRP"))
			{
				// mention is a speech pronoun
				// cec can be either a speech pronoun or a propernoun
				if (cec.HeadTokenTag.StartsWith("NNP"))
				{
					//exclude antecedents not in the same sentence when they are not pronoun 
					return (mention.SentenceNumber - cec.SentenceNumber != 0);
				}
				else if (cec.HeadTokenTag.StartsWith("PRP"))
				{
					return false;
				}
				else
				{
					System.Console.Error.WriteLine("Unexpected candidate exluded: " + cec.ToText());
					return true;
				}
			}
			else
			{
				System.Console.Error.WriteLine("Unexpected mention exluded: " + mention.ToText());
				return true;
			}
		}
	}
}