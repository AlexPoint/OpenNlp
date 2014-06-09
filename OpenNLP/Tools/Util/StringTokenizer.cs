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

using System;

namespace OpenNLP.Tools.Util
{
	/// <summary>
	/// Class providing simple tokenization of a string, for manipulation.  
	/// For NLP tokenizing, see the OpenNLP.Tools.Tokenize namespace.
	/// </summary>
	public class StringTokenizer
	{
		private const string mDelimiters = " \t\n\r";	//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character	
		private string[] mTokens;
		int mPosition;

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// </summary>
		/// <param name="input">
		/// String to tokenize
		/// </param>
		public StringTokenizer(string input) : this(input, mDelimiters.ToCharArray())
		{			
		}

		public StringTokenizer(string input, string separators) : this(input, separators.ToCharArray())
		{
		}

		public StringTokenizer(string input, params char[] separators) 
		{
			mTokens = input.Split(separators);
			mPosition = 0;
		}

		public string NextToken()
		{
			while (mPosition < mTokens.Length)
			{
				if ((mTokens[mPosition].Length > 0))
				{
					return mTokens[mPosition++];
				}
				mPosition++;
			}
			return null;
		}
		
	}
}
