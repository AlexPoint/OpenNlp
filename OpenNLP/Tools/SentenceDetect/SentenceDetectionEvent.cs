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

//This file is based on the SDEvent.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (c) 2001, Eric D. Friedman All Rights Reserved.
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

namespace OpenNLP.Tools.SentenceDetect
{
	/// <summary>
	/// An Event which can hold a pointer to another Event for use in a
	/// linked list.
	/// </summary>
	public class SentenceDetectionEvent : SharpEntropy.TrainingEvent
	{
		private SentenceDetectionEvent mNextEvent;
		
		internal SentenceDetectionEvent NextEvent
		{
			get
			{
				return mNextEvent;
			}
			set
			{
				mNextEvent = value;
			}
		}

		/// <summary> 
		/// package access only
		/// </summary>
		internal SentenceDetectionEvent(string outcome, string[] context) : base(outcome, context)
		{
		}
	}
}
