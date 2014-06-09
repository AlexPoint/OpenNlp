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

//This file is based on the SingletonNonReferentialResolver.java source file found in the
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
    /// This class allows you to share a single instance of a non-referential resolver
	/// amoung several resolvers.   
	/// </summary>
	public class SingletonNonReferentialResolver:DefaultNonReferentialResolver
	{
		private static SingletonNonReferentialResolver mResolver;
		private static bool mTrained;
		
		private SingletonNonReferentialResolver(string projectName, ResolverMode mode):base(projectName, "nonref", mode)
		{
		}
		
		public static SingletonNonReferentialResolver GetInstance(string modelName, ResolverMode mode)
		{
			if (mResolver == null)
			{
				mResolver = new SingletonNonReferentialResolver(modelName, mode);
			}
			return mResolver;
		}
		
		public override void Train()
		{
			if (!mTrained)
			{
				base.Train();
				mTrained = true;
			}
		}

	}
}