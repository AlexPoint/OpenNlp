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

//This file is based on the FixedNonReferentialResolver.java source file found in the
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
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
namespace OpenNLP.Tools.Coreference.Resolver
{
	
	/// <summary> Implementation of non-referential classifier which uses a fixed-value threshold. </summary>
	public class FixedNonReferentialResolver : INonReferentialResolver
	{
		
		private double mNonReferentialProbability;
		
		public FixedNonReferentialResolver(double nonReferentialProbability)
		{
			mNonReferentialProbability = nonReferentialProbability;
		}
		
		public virtual double GetNonReferentialProbability(MentionContext mention)
		{
			return mNonReferentialProbability;
		}
		
		public virtual void AddEvent(MentionContext mention)
		{
		}
		
		public virtual void Train()
		{
		}
	}
}