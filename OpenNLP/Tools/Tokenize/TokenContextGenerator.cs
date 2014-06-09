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

//This file is based on the TokContextGenerator.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2000 Jason Baldridge and Gann Bierner
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

namespace OpenNLP.Tools.Tokenize
{
	/// <summary>
	///  Generate events for maxent decisions for tokenization.
	/// </summary>
    public class TokenContextGenerator : SharpEntropy.IContextGenerator<Util.Pair<string, int>>
	{
		public const string SplitIndicator = "T";
		public const string NoSplitIndicator = "F";
		
		/// <summary>
		/// Builds up the list of features based on the information in the object,
		/// which is a pair containing a string and and integer which
		/// indicates the index of the position we are investigating.
		/// </summary>
        public virtual string[] GetContext(Util.Pair<string, int> pair)
		{
            string data = pair.FirstValue;
			int index = pair.SecondValue;

            List<string> predicates = new List<string>();
			predicates.Add("p=" + data.Substring(0, (index) - (0)));
			predicates.Add("s=" + data.Substring(index));
			if (index > 0)
			{
				AddCharPredicates("p1", data[index - 1], predicates);
				if (index > 1)
				{
					AddCharPredicates("p2", data[index - 2], predicates);
					predicates.Add("p21=" + data[index - 2] + data[index - 1]);
				}
				else
				{
					predicates.Add("p2=bok");
				}
				predicates.Add("p1f1=" + data[index - 1] + data[index]);
			}
			else
			{
				predicates.Add("p1=bok");
			}
			AddCharPredicates("f1", data[index], predicates);
			if (index + 1 < data.Length)
			{
				AddCharPredicates("f2", data[index + 1], predicates);
				predicates.Add("f12=" + data[index] + data[index + 1]);
			}
			else
			{
				predicates.Add("f2=bok");
			}
			if (data[0] == '&' && data[data.Length - 1] == ';')
			{
				predicates.Add("cc"); //character code
			}
			
			return predicates.ToArray();
		}
		
		/// <summary>
		/// Helper function for GetContext.
		/// </summary>
        private void AddCharPredicates(string key, char c, List<string> predicates)
		{
			predicates.Add(key + "=" + c);
			if (System.Char.IsLetter(c))
			{
				predicates.Add(key + "_alpha");
				if (System.Char.IsUpper(c))
				{
					predicates.Add(key + "_caps");
				}
			}
			else if (System.Char.IsDigit(c))
			{
				predicates.Add(key + "_num");
			}
			else if (System.Char.IsWhiteSpace(c))
			{
				predicates.Add(key + "_ws");
			}
			else
			{
				if (c == '.' || c == '?' || c == '!')
				{
					predicates.Add(key + "_eos");
				}
				else if (c == '`' || c == '"' || c == '\'')
				{
					predicates.Add(key + "_quote");
				}
				else if (c == '[' || c == '{' || c == '(')
				{
					predicates.Add(key + "_lp");
				}
				else if (c == ']' || c == '}' || c == ')')
				{
					predicates.Add(key + "_rp");
				}
			}
		}
	}
}
