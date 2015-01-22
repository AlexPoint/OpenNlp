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

//This file is based on the PluralNounResolver.java source file found in the
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
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
using System.Collections.Generic;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	
	/// <summary> Resolves coreference between plural nouns. </summary>
	public class PluralNounResolver:MaximumEntropyResolver
	{
        public PluralNounResolver(string projectName, ResolverMode mode) : base(projectName, "plmodel", mode, 80, true)
		{
			ShowExclusions = false;
		}

        public PluralNounResolver(string projectName, ResolverMode mode, INonReferentialResolver nonReferentialResolver) : base(projectName, "plmodel", mode, 80, true, nonReferentialResolver)
		{
			ShowExclusions = false;
		}
		
		protected internal override List<string> GetFeatures(MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
			
			if (entity != null)
			{
                features.AddRange(GetContextFeatures(mention));
                features.AddRange(GetStringMatchFeatures(mention, entity));
			}
			return features;
		}
		
		public override bool CanResolve(MentionContext mention)
		{
			string firstTok = mention.FirstTokenText.ToLower();
			string firstTokTag = mention.FirstToken.SyntacticType;
			bool rv = mention.HeadTokenTag == PartsOfSpeech.NounPlural && !IsDefiniteArticle(firstTok, firstTokTag);
			return rv;
		}
		
		protected internal override bool IsExcluded(MentionContext mention, DiscourseEntity entity)
		{
			if (base.IsExcluded(mention, entity))
			{
				return true;
			}
			else
			{
				MentionContext cec = entity.LastExtent;
                return (cec.HeadTokenTag != PartsOfSpeech.NounPlural || base.IsExcluded(mention, entity));
			}
		}
	}
}