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

//This file is based on the ShalowParseMentionFinder.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Mention
{
	/// <summary> 
    /// Finds mentions from shallow np-chunking based parses. 
    /// </summary>
	public sealed class ShallowParseMentionFinder : AbstractMentionFinder
	{
		private static ShallowParseMentionFinder mInstance;
		
		private ShallowParseMentionFinder(IHeadFinder headFinder)
		{
			HeadFinder = headFinder;
			PrenominalNamedEntitiesCollection = true;
			CoordinatedNounPhrasesCollection = true;
		}
		
		public static ShallowParseMentionFinder GetInstance(IHeadFinder headFinder)
		{
			if (mInstance == null)
			{
				mInstance = new ShallowParseMentionFinder(headFinder);
			}
			else if (mInstance.HeadFinder != headFinder)
			{
				mInstance = new ShallowParseMentionFinder(headFinder);
			}
			return mInstance;
		}
		
		/*
		protected final List getNounPhrases(Parse p) {
		List nps = p.getNounPhrases();
		List basals = new ArrayList();
		for (int ni=0,ns=nps.size();ni<ns;ni++) {
		Parse np = (Parse) nps.get(ni);
		if (isBasalNounPhrase(np)) {
		basals.add(np);
		}
		else if (isPossessive(np)) {
		basals.add(np);
		basals.addAll(getNounPhrases(np));
		}
		else if (isOfPrepPhrase(np)) {
		basals.add(np);
		basals.addAll(getNounPhrases(np));
		}
		else {
		basals.addAll(getNounPhrases(np));
		}
		}
		return(basals);
		}
		*/
	}
}