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

//This file is based on the Event.java source file found in the
//original java implementation of MaxEnt.  That source file contains the following header:

// Copyright (C) 2001 Jason Baldridge and Gann Bierner
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;

namespace SharpEntropy
{
	/// <summary>
	/// The context of a decision point during training.  This includes
	/// contextual predicates and an outcome.
	/// </summary>
	/// <author>
	/// Jason Baldridge
	/// </author>
	/// <author>
	/// Richard J. Northedge
	/// </author>
	/// <version>
	/// based on Event.java, $Revision: 1.3 $, $Date: 2003/12/09 23:13:08 $
	/// </version>
	public class TrainingEvent
	{
		private string mOutcome;
		private string[] mContext;
		
		/// <summary>
		/// The outcome label for this training event.
		/// </summary>
		virtual public string Outcome
		{
			get
			{
				return mOutcome;
			}	
		}

		/// <summary>
		/// Gets the context for this training event.
		/// </summary>
		/// <returns>
		/// A string array of context values for this training event.
		/// </returns>
		virtual public string[] GetContext()
		{
			return mContext;
		}

		/// <summary>
		/// Constructor for a training event.
		/// </summary>
		/// <param name="outcome">
		/// the outcome label
		/// </param>
		/// <param name="context">
		/// array containing context values
		/// </param>
		public TrainingEvent(string outcome, string[] context)
		{
			mOutcome = outcome;
			mContext = context;
		}
		
		/// <summary>
		/// Override providing text summary of the training event.
		/// </summary>
		/// <returns>
		/// Summary of the training event.
		/// </returns>
		public override string ToString()
		{
			return mOutcome + " " + string.Join(", ", mContext);
		}
	}
}
