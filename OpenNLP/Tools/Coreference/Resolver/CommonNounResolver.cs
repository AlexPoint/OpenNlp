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

//This file is based on the CommonNounResolver.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Resolver
{
	/// <summary>
    /// Resolves coreference between common nouns.
    /// </summary>
	public class CommonNounResolver : MaximumEntropyResolver
	{
		public CommonNounResolver(string projectName, ResolverMode resolverMode) : base(projectName, "cmodel", resolverMode, 80, true)
		{
			ShowExclusions = false;
			PreferFirstReferent = true;
		}
		
		public CommonNounResolver(string projectName, ResolverMode resolverMode, INonReferentialResolver nonReferentialResolver) : base(projectName, "cmodel", resolverMode, 80, true, nonReferentialResolver)
		{
			ShowExclusions = false;
			PreferFirstReferent = true;
		}

        protected internal override List<string> GetFeatures(Mention.MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
			
            if (entity != null)
			{
                features.AddRange(GetContextFeatures(mention));
                features.AddRange(GetStringMatchFeatures(mention, entity));
			}
			return features;
		}

        public override bool CanResolve(Mention.MentionContext mention)
		{
			string firstToken = mention.FirstTokenText.ToLower();
			string firstTokenTag = mention.FirstToken.SyntacticType;
			bool canResolve = mention.HeadTokenTag == PartsOfSpeech.NounSingularOrMass 
                && !IsDefiniteArticle(firstToken, firstTokenTag);
			return canResolve;
		}
		
		protected internal override bool IsExcluded(Mention.MentionContext entityContext, DiscourseEntity discourseEntity)
		{
			if (base.IsExcluded(entityContext, discourseEntity))
			{
				return true;
			}
			else
			{
                Mention.MentionContext currentEntityContext = discourseEntity.LastExtent;
				return (!CanResolve(currentEntityContext) || base.IsExcluded(entityContext, discourseEntity));
			}
		}
	}
}