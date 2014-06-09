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

//This file is based on the BuildContextGenerator.java source file found in the
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
	/// Class to generator predictive contexts for deciding how constituents should be combined together.
	/// </summary>
	public class BuildContextGenerator : SharpEntropy.IContextGenerator
	{
		
		private const string mEndOfSentence = "eos";
		
		/// <summary> 
		/// Creates a new context generator for making decisions about combining constituents together.
		/// </summary>
		public BuildContextGenerator()
		{
		}

        /// <summary>
        /// Returns the predictive context used to determine how the constituent at the specified index 
        /// should be combined with other constituents. 
        /// </summary>
        /// <param name="input">
        /// Object array containing an array of Parse objects (the uncombined constituents), and the index of the
        /// relevant constituent.
        /// </param>
		public virtual string[] GetContext(object input)
		{
			object[] parameters = (object[]) input;
			return GetContext((Parse[]) parameters[0], ((int) parameters[1]));
		}
		
		private string MakeConstituent(Parse inputParse, int index)
		{
			StringBuilder feature = new StringBuilder(20);
			feature.Append(index).Append("=");
			if (inputParse != null)
			{
				if (index < 0)
				{
					feature.Append(inputParse.Label).Append("|");
				}
				feature.Append(inputParse.Type).Append("|").Append(inputParse.Head.ToString());
			}
			else
			{
				feature.Append(mEndOfSentence).Append("|").Append(mEndOfSentence).Append("|").Append(mEndOfSentence);
			}
			return feature.ToString();
		}
		
		private string MakeConstituentBackOff(Parse inputParse, int index)
		{
			StringBuilder feature = new StringBuilder(20);
			feature.Append(index).Append("*=");
			if (inputParse != null)
			{
				if (index < 0)
				{
					feature.Append(inputParse.Label).Append("|");
				}
				feature.Append(inputParse.Type);
			}
			else
			{
				feature.Append(mEndOfSentence).Append("|").Append(mEndOfSentence);
			}
			return feature.ToString();
		}
		
		/// <summary>
		/// Returns the predictive context used to determine how the constituent at the specified index 
        /// should be combined with other constituents. 
		/// </summary>
		/// <param name="constituents">
		/// The constituents which have yet to be combined into new constituents.
		/// </param>
		/// <param name="index">
		/// The index of the constituent whcihi is being considered.
		/// </param>
		/// <returns>
		/// the context for building constituents at the specified index.
		/// </returns>
		public virtual string[] GetContext(Parse[] constituents, int index)
		{
			List<string> features = new List<string>(100);
			int constituentCount = constituents.Length;
			
			//default 
			features.Add("default");
			// cons(-2), cons(-1), cons(0), cons(1), cons(2)
			// cons(-2)
			Parse previousPreviousParse = null;
			Parse previousParse = null;
			Parse currentParse = null;
			Parse nextParse = null;
			Parse nextNextParse = null;
			
			if (index - 2 >= 0)
			{
				previousPreviousParse = constituents[index - 2];
			}
			if (index - 1 >= 0)
			{
				previousParse = constituents[index - 1];
			}
			currentParse = constituents[index];
			if (index + 1 < constituentCount)
			{
				nextParse = constituents[index + 1];
			}
			if (index + 2 < constituentCount)
			{
				nextNextParse = constituents[index + 2];
			}
			
			// cons(-2), cons(-1), cons(0), cons(1), cons(2)
			string previousPreviousConstituent = MakeConstituent(previousPreviousParse, - 2);
			string previousConstituent = MakeConstituent(previousParse, - 1);
			string currentConstituent = MakeConstituent(currentParse, 0);
			string nextConstituent = MakeConstituent(nextParse, 1);
			string nextNextConstituent = MakeConstituent(nextNextParse, 2);
			
			string previousPreviousConstituentBackOff = MakeConstituentBackOff(previousPreviousParse, - 2);
			string previousConstituentBackOff = MakeConstituentBackOff(previousParse, - 1);
			string currentConstituentBackOff = MakeConstituentBackOff(currentParse, 0);
			string nextConstituentBackOff = MakeConstituentBackOff(nextParse, 1);
			string nextNextConstituentBackOff = MakeConstituentBackOff(nextNextParse, 2);
			
			// cons(-2), cons(-1), cons(0), cons(1), cons(2)
			features.Add(previousPreviousConstituent);
			features.Add(previousPreviousConstituentBackOff);
			features.Add(previousConstituent);
			features.Add(previousConstituentBackOff);
			features.Add(currentConstituent);
			features.Add(currentConstituentBackOff);
			features.Add(nextConstituent);
			features.Add(nextConstituentBackOff);
			features.Add(nextNextConstituent);
			features.Add(nextNextConstituentBackOff);
			
			// cons(-1,0), cons(0,1)
			features.Add(previousConstituent + "," + currentConstituent);
			features.Add(previousConstituentBackOff + "," + currentConstituent);
			features.Add(previousConstituent + "," + currentConstituentBackOff);
			features.Add(previousConstituentBackOff + "," + currentConstituentBackOff);
			
			features.Add(currentConstituent + "," + nextConstituent);
			features.Add(currentConstituentBackOff + "," + nextConstituent);
			features.Add(currentConstituent + "," + nextConstituentBackOff);
			features.Add(currentConstituentBackOff + "," + nextConstituentBackOff);
			
			// cons3(-2,-1,0), cons3(-1,0,1), cons3(0,1,2)
			features.Add(previousPreviousConstituent + "," + previousConstituent + "," + currentConstituent);
			features.Add(previousPreviousConstituentBackOff + "," + previousConstituent + "," + currentConstituent);
			features.Add(previousPreviousConstituent + "," + previousConstituentBackOff + "," + currentConstituent);
			features.Add(previousPreviousConstituentBackOff + "," + previousConstituentBackOff + "," + currentConstituent);
			features.Add(previousPreviousConstituentBackOff + "," + previousConstituentBackOff + "," + currentConstituentBackOff);
			
			features.Add(previousConstituent + "," + currentConstituent + "," + nextConstituent);
			features.Add(previousConstituentBackOff + "," + currentConstituent + "," + nextConstituent);
			features.Add(previousConstituent + "," + currentConstituent + "," + nextConstituentBackOff);
			features.Add(previousConstituentBackOff + "," + currentConstituent + "," + nextConstituentBackOff);
			features.Add(previousConstituentBackOff + "," + currentConstituentBackOff + "," + nextConstituentBackOff);
			
			features.Add(currentConstituent + "," + nextConstituent + "," + nextNextConstituent);
			features.Add(currentConstituent + "," + nextConstituentBackOff + "," + nextNextConstituent);
			features.Add(currentConstituent + "," + nextConstituent + "," + nextNextConstituentBackOff);
			features.Add(currentConstituent + "," + nextConstituentBackOff + "," + nextNextConstituentBackOff);
			features.Add(currentConstituentBackOff + "," + nextConstituentBackOff + "," + nextNextConstituentBackOff);
			
			// punct
			string currentParseWord = currentParse.ToString();
			if (currentParseWord == "-RRB-")
			{
				for (int parseIndex = index - 1; parseIndex >= 0; parseIndex--)
				{
					Parse testParse = constituents[parseIndex];
					if (testParse.ToString() == "-LRB-")
					{
						features.Add("bracketsmatch");
						break;
					}
					if (testParse.Label.StartsWith(MaximumEntropyParser.StartPrefix))
					{
						break;
					}
				}
			}
			if (currentParseWord == "-RCB-")
			{
				for (int parseIndex = index - 1; parseIndex >= 0; parseIndex--)
				{
					Parse testParse = constituents[parseIndex];
					if (testParse.ToString() == "-LCB-")
					{
						features.Add("bracketsmatch");
						break;
					}
					if (testParse.Label.StartsWith(MaximumEntropyParser.StartPrefix))
					{
						break;
					}
				}
			}
			if (currentParseWord == "''")
			{
				for (int parseIndex = index - 1; parseIndex >= 0; parseIndex--)
				{
					Parse testParse = constituents[parseIndex];
					if (testParse.ToString() == "``")
					{
						features.Add("quotesmatch");
						break;
					}
					if (testParse.Label.StartsWith(MaximumEntropyParser.StartPrefix))
					{
						break;
					}
				}
			}
			if (currentParseWord == "'")
			{
				for (int parseIndex = index - 1; parseIndex >= 0; parseIndex--)
				{
					Parse testParse = constituents[parseIndex];
					if (testParse.ToString() == "`")
					{
						features.Add("quotesmatch");
						break;
					}
					if (testParse.Label.StartsWith(MaximumEntropyParser.StartPrefix))
					{
						break;
					}
				}
			}
			if (currentParseWord == ",")
			{
				for (int parseIndex = index - 1; parseIndex >= 0; parseIndex--)
				{
					Parse testParse = constituents[parseIndex];
					if (testParse.ToString() == ",")
					{
						features.Add("iscomma");
						break;
					}
					if (testParse.Label.StartsWith(MaximumEntropyParser.StartPrefix))
					{
						break;
					}
				}
			}
			if (currentParseWord == (".") && index == constituentCount - 1)
			{
				for (int parseIndex = index - 1; parseIndex >= 0; parseIndex--)
				{
					Parse testParse = constituents[parseIndex];
					if (testParse.Label.StartsWith(MaximumEntropyParser.StartPrefix))
					{
						if (parseIndex == 0)
						{
							features.Add("endofsentence");
						}
						break;
					}
				}
			}
			return features.ToArray();
		}
	}
}
