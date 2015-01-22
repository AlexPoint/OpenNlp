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

//This file is based on the SentenceDetector.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2002 Jason Baldridge and Gann Bierner
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
	/// The interface for sentence detectors, 
	/// which find the sentence boundaries in a text.
	/// </summary>
	public interface ISentenceDetector
	{
		/// <summary> 
		/// Sentence detect a string
		/// </summary>
		/// <param name="input">
		/// The string to be sentence detected.
		/// </param>
		/// <returns>
		/// The string[] with the individual sentences as the array
		/// elements.
		/// </returns>
		string[] SentenceDetect(string input);
			
		/// <summary> 
		/// Sentence detect a string.
		/// </summary>
		/// <param name="input">
		/// The string to be sentence detected.
		/// </param>
		/// <returns>
		/// An int[] with the starting offset positions of each
		/// detected sentence. 
		/// </returns>
		int[] SentencePositionDetect(string input);
	}
}
