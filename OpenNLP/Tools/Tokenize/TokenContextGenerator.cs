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
    public class TokenContextGenerator : SharpEntropy.IContextGenerator<Tuple<string, int>>
	{
		/// <summary>
		/// Split the string
		/// </summary>
		public const string SplitIndicator = "T";
        /// <summary>
        /// Don't split the string
        /// </summary>
		public const string NoSplitIndicator = "F";
		
		/// <summary>
		/// Builds up the list of features based on the information in the object,
		/// which is a pair containing a string and and integer which
		/// indicates the index of the position we are investigating.
		/// </summary>
        public virtual string[] GetContext(Tuple<string, int> pair)
		{
            string token = pair.Item1;
			int index = pair.Item2;

            // add strings before and after the index in the token
            var predicates = new List<string>
            {
                "p=" + token.Substring(0, index), 
                "s=" + token.Substring(index)
            };
		    if (index > 0)
			{
                // add predicates for character just before the current index
                AddCharPredicates("p1", token[index - 1], predicates);
                predicates.Add("p1f1=" + token[index - 1] + token[index]);
				if (index > 1)
				{
                    // add predicates for the character 2 positions before the current index
					AddCharPredicates("p2", token[index - 2], predicates);
					predicates.Add("p21=" + token[index - 2] + token[index - 1]);
				}
				else
				{
					predicates.Add("p2=bok");
				}
			}
			else
			{
				predicates.Add("p1=bok");
			}

            // add predicates for char at the current index
			AddCharPredicates("f1", token[index], predicates);

            // add predicates for the char just after
			if (index + 1 < token.Length)
			{
				AddCharPredicates("f2", token[index + 1], predicates);
				predicates.Add("f12=" + token[index] + token[index + 1]);
			}
			else
			{
				predicates.Add("f2=bok");
			}

            // test if token starts by '&' or ends by ';'
			if (token[0] == '&' && token[token.Length - 1] == ';')
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
			if (char.IsLetter(c))
			{
                // whether it's a letter
				predicates.Add(key + "_alpha");
				if (char.IsUpper(c))
				{
                    // whether it's upper case
					predicates.Add(key + "_caps");
				}
			}
			else if (char.IsDigit(c))
			{
                // whether it's a digit
				predicates.Add(key + "_num");
			}
			else if (char.IsWhiteSpace(c))
			{
                // whether it's whitespace
				predicates.Add(key + "_ws");
			}
			else
			{
				if (c == '.' || c == '?' || c == '!')
				{
                    // whether it's an end of sentence
					predicates.Add(key + "_eos");
				}
				else if (c == '`' || c == '"' || c == '\'')
				{
                    // whether it's a quote
					predicates.Add(key + "_quote");
				}
				else if (c == '[' || c == '{' || c == '(')
				{
                    // whether it's a left parenthesis
					predicates.Add(key + "_lp");
				}
				else if (c == ']' || c == '}' || c == ')')
				{
                    // whether it's a right parenthesis
					predicates.Add(key + "_rp");
				}
			}
		}
	}
}
