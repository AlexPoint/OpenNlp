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
        private const string Delimiters = " \t\n\r";
            //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character	

        private readonly string[] _tokens;
        private int _position;

        /// <summary>
        /// Initializes a new class instance with a specified string to process
        /// </summary>
        /// <param name="input">
        /// string to tokenize
        /// </param>
        public StringTokenizer(string input) : this(input, Delimiters.ToCharArray())
        {
        }

        public StringTokenizer(string input, string separators) : this(input, separators.ToCharArray())
        {
        }

        public StringTokenizer(string input, params char[] separators)
        {
            _tokens = input.Split(separators);
            _position = 0;
        }

        public string NextToken()
        {
            while (_position < _tokens.Length)
            {
                if ((_tokens[_position].Length > 0))
                {
                    return _tokens[_position++];
                }
                _position++;
            }
            return null;
        }

    }
}