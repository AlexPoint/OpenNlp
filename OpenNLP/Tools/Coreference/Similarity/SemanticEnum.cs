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

//This file is based on the SemanticEnum.java source file found in the
//original java implementation of OpenNLP.  

using System;
namespace OpenNLP.Tools.Coreference.Similarity
{
	
	public class SemanticEnum
	{
		private string mCompatibility;

        private SemanticEnum(string compatibility)
        {
            mCompatibility = compatibility;
        }

        public override string ToString()
        {
            return mCompatibility;
        }

		/// <summary>
        /// Semantically compatible. 
        /// </summary>
		public static readonly SemanticEnum Compatible = new SemanticEnum("compatible");
		
        /// <summary>
        /// Semantically incompatible.
        /// </summary>
		public static readonly SemanticEnum Incompatible = new SemanticEnum("incompatible");
		
        /// <summary>
        /// Semantic compatibility Unknown. 
        /// </summary>
		public static readonly SemanticEnum Unknown = new SemanticEnum("unknown");
		
		
	}
}