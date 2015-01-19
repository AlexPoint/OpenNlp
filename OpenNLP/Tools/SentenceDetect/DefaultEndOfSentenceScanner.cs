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

//This file is based on the DefaultEndOfSentenceScanner.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2002, Eric D. Friedman All Rights Reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace OpenNLP.Tools.SentenceDetect
{
	/// <summary>
	/// The default end of sentence scanner implements all of the
	/// EndOfSentenceScanner methods in terms of the GetPositions(char[])
	/// method.
	/// It scans for '.', '?', '!', '"'
	/// </summary>
	public class DefaultEndOfSentenceScanner : IEndOfSentenceScanner
	{
        private static readonly List<char> defaultEndOfSentenceCharacters = new List<char>(){'?','!','.'};
        
	    /// <summary> 
	    /// Creates a new <code>DefaultEndOfSentenceScanner</code> instance.
	    /// </summary>
	    public DefaultEndOfSentenceScanner(){}


        public List<char> GetPotentialEndOfSentenceCharacters()
        {
            return defaultEndOfSentenceCharacters;
        }

        public virtual List<int> GetPositions(string input)
		{
			return GetPositions(input.ToCharArray());
		}

        public virtual List<int> GetPositions(System.Text.StringBuilder buffer)
		{
			return GetPositions(buffer.ToString().ToCharArray());
		}
		
		public virtual List<int> GetPositions(char[] charBuffer)
		{
            var positionList = new List<int>();
			
			for (int currentChar = 0; currentChar < charBuffer.Length; currentChar++)
			{
			    if (this.GetPotentialEndOfSentenceCharacters().Contains(charBuffer[currentChar]))
			    {
			        positionList.Add(currentChar);
			    }
				/*switch (charBuffer[currentChar])
				{	
					case '.': 
					case '?': 
					case '!': 
						positionList.Add(currentChar);
						break;
					default: 
						break;
				}*/
			}
			return positionList;
		}


	    public static List<char> GetDefaultEndOfSentenceCharacters()
	    {
	        return defaultEndOfSentenceCharacters;
	    }
	}
}
