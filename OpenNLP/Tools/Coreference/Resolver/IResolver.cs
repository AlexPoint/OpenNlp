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

//This file is based on the Resolver.java source file found in the
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
    /// Interface for coreference resolvers.
    /// </summary>
	public interface IResolver
	{
		/// <summary>
        /// Returns true if this resolver is able to resolve the referening experession of the same type
		/// as the specified mention.
		/// </summary>
		/// <param name="mention">
        /// The mention being considered for resolution. 
		/// </param>
		/// <returns>
        /// true if the resolver handles this type of referring
		/// expression, false otherwise.
		/// </returns>
		bool CanResolve(Mention.MentionContext mention);
		
		/// <summary>
        /// Resolve this referirng expression to a discourse entity in the discourse model.
        /// </summary>
        /// <param name="expression">
        /// the referring expression. 
		/// </param>
        /// <param name="discourseModel">
        /// the discourse model.
		/// </param>
		/// <returns>
        /// the discourse entity which the resolver beleives this
		/// referring expression refers to or null if no discourse entity is
		/// coreferent with the referring expression. 
		/// </returns>
		DiscourseEntity Resolve(Mention.MentionContext expression, DiscourseModel discourseModel);
		
		/// <summary>
        /// Uses the specified mention and discourse model to train this resolver.
		/// All mentions sent to this method need to have their id fields set to indicate coreference
		/// relationships.    
		/// </summary>
		/// <param name="mention">
        /// The mention which is being used for training.
		/// </param>
		/// <param name="model">
        /// the discourse model.
		/// </param>
		/// <returns>
        /// the discourse entity which is referred to by the referring
		/// expression or null if no discourse entity is referenced.
		/// </returns>
		DiscourseEntity Retain(Mention.MentionContext mention, DiscourseModel model);
		
		/// <summary>
        /// Retrains model on examples for which retain was called.
        /// </summary>
		void Train();
	}
}