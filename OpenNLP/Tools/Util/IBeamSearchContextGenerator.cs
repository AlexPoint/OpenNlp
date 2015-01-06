//Copyright (C) 2005 Richard J. Northedge
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

//This file is based on the BeamSearchContextGenerator.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2004 Thomas Morton
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

using System;
using System.Collections;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// Interface for a context generator that uses a beam search. 
	/// </summary>
	public interface IBeamSearchContextGenerator : SharpEntropy.IContextGenerator
	{
		/// <summary>
		/// Returns the context for the specified position in the specified sequence (list).  </summary>
		/// <param name="index">
		/// The index of the sequence.
		/// </param>
		/// <param name="sequence">
		/// The sequence of items over which the beam search is performed.
		/// </param>
		/// <param name="priorDecisions">
		/// The sequence of decisions made prior to the context for which this decision is being made.
		/// </param>
		/// <param name="additionalContext">
		/// Any addition context specific to a class implementing this interface.
		/// </param>
		/// <returns>
		/// the context for the specified position in the specified sequence.
		/// </returns>
		string[] GetContext(int index, string[] sequence, string[] priorDecisions, object[] additionalContext);
	}
}