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

//This file is based on the NonReferentialResolver.java source file found in the
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
    /// Provides the interface for a object to provide a resolver with a non-referential
	/// probability.  Non-referential resolvers compute the probability that a particular mention refers
	/// to no antecedent.  This probability can then compete with the probability that
	/// a mention refers with a specific antecedent. 
	/// </summary>
	public interface INonReferentialResolver
	{
		/// <summary>
        /// Returns the probability that the specified mention doesn't refer to any previous mention.
        /// </summary>
		/// <param name="mention">
        /// The mention under consideration.
		/// </param>
		/// <returns>
        /// A probability that the specified mention doesn't refer to any previous mention. 
		/// </returns>
        double GetNonReferentialProbability(Mention.MentionContext mention);
		
		/// <summary>
        /// Designates that the specified mention be used for training.
        /// </summary>
		/// <param name="mention">
        /// The mention to be used.  The mention id is used to determine
		/// whether this mention is referential or non-referential.
		/// </param>
        void AddEvent(Mention.MentionContext mention);
		
		/// <summary>
        /// Trains a model based on the events given to this resolver via AddEvent.
        /// </summary>
		void Train();
	}
}