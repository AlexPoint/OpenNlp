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

//This file is based on the DefiniteNounResolver.java source file found in the
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
using Parse = OpenNLP.Tools.Coreference.Mention.IParse;
using System.Collections.Generic;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	/// <summary> Resolves coreference between definite noun-phrases. </summary>
	public class DefiniteNounResolver:MaximumEntropyResolver
	{
		
		public DefiniteNounResolver(string projectName, ResolverMode mode):base(projectName, "defmodel", mode, 80)
		{
			//preferFirstReferent = true;
		}

        public DefiniteNounResolver(string projectName, ResolverMode mode, INonReferentialResolver nonReferentialResolver)
            : base(projectName, "defmodel", mode, 80, nonReferentialResolver)
		{
			//preferFirstReferent = true;
		}
		
		
		public override bool CanResolve(MentionContext mention)
		{
			System.Object[] mtokens = mention.Tokens;
			
			string firstTok = mention.FirstTokenText.ToLower();
			bool rv = mtokens.Length > 1 && !mention.HeadTokenTag.StartsWith("NNP") && IsDefiniteArticle(firstTok, mention.FirstTokenTag);
			//if (rv) {
			//  System.err.println("defNp "+ec);
			//}
			return (rv);
		}
		
		protected internal override List<string> GetFeatures(MentionContext mention, DiscourseEntity entity)
		{
            List<string> features = base.GetFeatures(mention, entity);
			
			if (entity != null)
			{
                features.AddRange(GetContextFeatures(mention));
                features.AddRange(GetStringMatchFeatures(mention, entity));
                features.AddRange(GetDistanceFeatures(mention, entity));
			}
			return features;
		}
	}
}