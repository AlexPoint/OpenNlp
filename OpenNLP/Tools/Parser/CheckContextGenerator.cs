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

//This file is based on the CheckContextGenerator.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Thomas Morton
// 
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
// 
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
// 
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNLP.Tools.Parser
{
	/// <summary>
	/// Class for generating predictive context for deciding when a constituent is complete.
	/// </summary>
	public class CheckContextGenerator : SharpEntropy.IContextGenerator
	{
		private const string mEndOfSentence = "eos";
		
		/// <summary>
		/// Creates a new context generator for generating predictive context for deciding when a constituent is complete.
		/// </summary>
		public CheckContextGenerator()
		{
		}
		
		public virtual string[] GetContext(object input)
		{
			object[] parameters = (object[]) input;
			return GetContext((Parse[]) parameters[0], (string) parameters[1], ((int) parameters[2]), ((int) parameters[3]));
		}
		
		private void Surround(Parse inputParse, int index, string type, List<string> features)
		{
			StringBuilder feature = new StringBuilder(20);
			feature.Append("s").Append(index).Append("=");
			if (inputParse != null)
			{
				feature.Append(inputParse.Head.ToString()).Append("|").Append(type).Append("|").Append(inputParse.Head.Type);
			}
			else
			{
				feature.Append(mEndOfSentence).Append("|").Append(type).Append("|").Append(mEndOfSentence);
			}
			features.Add(feature.ToString());
			feature.Length = 0;
			feature.Append("s").Append(index).Append("*=");
			if (inputParse != null)
			{
				feature.Append(type).Append("|").Append(inputParse.Head.Type);
			}
			else
			{
				feature.Append(type).Append("|").Append(mEndOfSentence);
			}
			features.Add(feature.ToString());
		}
		
		private void CheckConstituent(Parse inputParse, string index, string type, List<string> features)
		{
			StringBuilder feature = new StringBuilder(20);
			feature.Append("c").Append(index).Append("=").Append(inputParse.Type).Append("|").Append(inputParse.Head.ToString()).Append("|").Append(type);
			features.Add(feature.ToString());
			feature.Length = 0;
			feature.Append("c").Append(index).Append("*=").Append(inputParse.Type).Append("|").Append(type);
			features.Add(feature.ToString());
		}
		
		private void CheckConstituent(Parse firstParse, Parse secondParse, string type, List<string> features)
		{
			StringBuilder feature = new StringBuilder(20);
			feature.Append("cil=").Append(type).Append(",").Append(firstParse.Type).Append("|").Append(firstParse.Head.ToString()).Append(",").Append(secondParse.Type).Append("|").Append(secondParse.Head.ToString());
			features.Add(feature.ToString());
			feature.Length = 0;
			feature.Append("ci*l=").Append(type).Append(",").Append(firstParse.Type).Append(",").Append(secondParse.Type).Append("|").Append(secondParse.Head.ToString());
			features.Add(feature.ToString());
			feature.Length = 0;
			feature.Append("cil*=").Append(type).Append(",").Append(firstParse.Type).Append("|").Append(firstParse.Head.ToString()).Append(",").Append(secondParse.Type);
			features.Add(feature.ToString());
			feature.Length = 0;
			feature.Append("ci*l*=").Append(type).Append(",").Append(firstParse.Type).Append(",").Append(secondParse.Type);
			features.Add(feature.ToString());
		}
		
		/// <summary>
		/// Returns predictive context for deciding whether the specified constituents between the specified start and end index 
		/// can be combined to form a new constituent of the specified type.  
		/// </summary>
		/// <param name="constituents">
		/// The constituents which have yet to be combined into new constituents.
		/// </param>
		/// <param name="type">
		/// The type of the new constituent proposed.
		/// </param>
		/// <param name="firstConstituent">
		/// The first constituent of the proposed constituent.
		/// </param>
		/// <param name="lastConstituent">
		/// The last constituent of the proposed constituent.
		/// </param>
		/// <returns>
		/// The predictive context for deciding whether a new constituent should be created.
		/// </returns>
		public virtual string[] GetContext(Parse[] constituents, string type, int firstConstituent, int lastConstituent)
		{
			int constituentCount = constituents.Length;
			List<string> features = new List<string>(100);
			
			//default 
			features.Add("default");
			
			Parse startParse = constituents[firstConstituent];
			Parse endParse = constituents[lastConstituent];
			CheckConstituent(startParse, "begin", type, features);
			CheckConstituent(endParse, "last", type, features);
			StringBuilder production = new StringBuilder(20);
			production.Append(type).Append("->");
			for (int parseIndex = firstConstituent; parseIndex < lastConstituent; parseIndex++)
			{
				Parse testParse = constituents[parseIndex];
				CheckConstituent(testParse, endParse, type, features);
				production.Append(testParse.Type).Append(",");
			}
			production.Append(endParse.Type);
			features.Add(production.ToString());
			Parse previousPreviousParse = null;
			Parse previousParse = null;
			Parse nextParse = null;
			Parse nextNextParse = null;
			if (firstConstituent - 2 >= 0)
			{
				previousPreviousParse = constituents[firstConstituent - 2];
			}
			if (firstConstituent - 1 >= 0)
			{
				previousParse = constituents[firstConstituent - 1];
			}
			if (lastConstituent + 1 < constituentCount)
			{
				nextParse = constituents[lastConstituent + 1];
			}
			if (lastConstituent + 2 < constituentCount)
			{
				nextNextParse = constituents[lastConstituent + 2];
			}
			Surround(previousParse, - 1, type, features);
			Surround(previousPreviousParse, - 2, type, features);
			Surround(nextParse, 1, type, features);
			Surround(nextNextParse, 2, type, features);
			return features.ToArray();
		}
	}
}
