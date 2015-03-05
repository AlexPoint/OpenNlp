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

//This file is based on the EnglishNameFinder.java source file found in the
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
using OpenNLP.Tools.Trees;
using OpenNLP.Tools.Util;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.NameFind
{
	/// <summary> Class is used to create a name finder for English.</summary>
	public class EnglishNameFinder 
	{
		private readonly Dictionary<string, MaximumEntropyNameFinder> mFinders;
		private readonly string mModelPath;

        public static string[] NameTypes = { "person", "organization", "location", "date", "time", "percentage", "money" };
		
		public EnglishNameFinder(string modelPath)
		{
			mModelPath = modelPath;
            mFinders = new Dictionary<string, MaximumEntropyNameFinder>();
		}

		private Span[] TokenizeToSpans(string input)
		{
			var charType = CharacterEnum.Whitespace;
			CharacterEnum state = charType;

            var tokens = new List<Span>();
			int inputLength = input.Length;
			int start = - 1;
			var previousChar = (char) (0);
			for (int characterIndex = 0; characterIndex < inputLength; characterIndex++)
			{
				char c = input[characterIndex];
				if (char.IsWhiteSpace(c))
				{
					charType = CharacterEnum.Whitespace;
				}
				else if (char.IsLetter(c))
				{
					charType = CharacterEnum.Alphabetic;
				}
				else if (char.IsDigit(c))
				{
					charType = CharacterEnum.Numeric;
				}
				else
				{
					charType = CharacterEnum.Other;
				}
				if (state == CharacterEnum.Whitespace)
				{
					if (charType != CharacterEnum.Whitespace)
					{
						start = characterIndex;
					}
				}
				else
				{
					if (charType != state || (charType == CharacterEnum.Other && c != previousChar))
					{
						tokens.Add(new Span(start, characterIndex));
						start = characterIndex;
					}
				}
				state = charType;
				previousChar = c;
			}
			if (charType != CharacterEnum.Whitespace)
			{
				tokens.Add(new Span(start, inputLength));
			}
			return tokens.ToArray();
		}
		
		private string[] SpansToStrings(Span[] spans, string input)
		{
			var tokens = new string[spans.Length];
			for (int currentSpan = 0, spanCount = spans.Length; currentSpan < spanCount; currentSpan++)
			{
				tokens[currentSpan] = input.Substring(spans[currentSpan].Start, (spans[currentSpan].End) - (spans[currentSpan].Start));
			}
			return tokens;
		}
		
		private string[] Tokenize(string input)
		{
			return SpansToStrings(TokenizeToSpans(input), input);
		}

        private void AddNames(string tag, List<Span>names, Parse[] tokens, Parse lineParse)
		{
			for (int currentName = 0, nameCount = names.Count; currentName < nameCount; currentName++)
			{
				Span nameTokenSpan = names[currentName];
				Parse startToken = tokens[nameTokenSpan.Start];
				Parse endToken = tokens[nameTokenSpan.End];
				Parse commonParent = startToken.GetCommonParent(endToken);
				
				if (commonParent != null)
				{
					var nameSpan = new Span(startToken.Span.Start, endToken.Span.End);
					if (nameSpan.Equals(commonParent.Span))
					{
						
						commonParent.Insert(new Parse(commonParent.Text, nameSpan, tag, 1.0));
					}
					else
					{
						Parse[] kids = commonParent.GetChildren();
						bool crossingKids = false;
						for (int currentKid = 0, kidCount = kids.Length; currentKid < kidCount; currentKid++)
						{
							if (nameSpan.Crosses(kids[currentKid].Span))
							{
								crossingKids = true;
							}
						}
						if (!crossingKids)
						{
							commonParent.Insert(new Parse(commonParent.Text, nameSpan, tag, 1.0));
						}
						else
						{
                            if (commonParent.Type == CoordinationTransformer.Noun)
							{
								Parse[] grandKids = kids[0].GetChildren();
								if (grandKids.Length > 1 && nameSpan.Contains(grandKids[grandKids.Length - 1].Span))
								{
									commonParent.Insert(new Parse(commonParent.Text, commonParent.Span, tag, 1.0));
								}
							}
						}
					}

				}
			}
		}
		
		private Dictionary<string, string>[] CreatePreviousTokenMaps(string[] finders)
		{
            var previousTokenMaps = new Dictionary<string, string>[finders.Length];
			for (int currentFinder = 0, finderCount = finders.Length; currentFinder < finderCount; currentFinder++)
			{
                previousTokenMaps[currentFinder] = new Dictionary<string, string>();
			}
			return previousTokenMaps;
		}

        private void ClearPreviousTokenMaps(Dictionary<string, string>[] previousTokenMaps)
		{
			for (int currentMap = 0, mapCount = previousTokenMaps.Length; currentMap < mapCount; currentMap++)
			{
				previousTokenMaps[currentMap].Clear();
			}
		}

        private void UpdatePreviousTokenMaps(Dictionary<string, string>[] previousTokenMaps, string[] tokens, string[][] finderTags)
		{
			for (int currentMap = 0, mapCount = previousTokenMaps.Length; currentMap < mapCount; currentMap++)
			{
				for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
				{
					previousTokenMaps[currentMap][tokens[currentToken]] = finderTags[currentMap][currentToken];
				}
			}
		}
		
		private string ProcessParse(string[] models, Parse lineParse)
		{
			var output = new System.Text.StringBuilder();

			var finderTags = new string[models.Length][];
			Dictionary<string, string>[] previousTokenMaps = CreatePreviousTokenMaps(models);

			Parse[] tokenParses = lineParse.GetTagNodes();
            var tokens = new string[tokenParses.Length];
            for (int currentToken = 0; currentToken < tokens.Length; currentToken++)
            {
                tokens[currentToken] = tokenParses[currentToken].ToString();
            }

			for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
			{
				MaximumEntropyNameFinder finder = mFinders[models[currentFinder]];
                finderTags[currentFinder] = finder.Find(tokens, previousTokenMaps[currentFinder]);
			}
			UpdatePreviousTokenMaps(previousTokenMaps, tokens, finderTags);
			for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
			{
				int start = -1;

                var names = new List<Span>(5);
				for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
				{
					if ((finderTags[currentFinder][currentToken] == MaximumEntropyNameFinder.Start) || (finderTags[currentFinder][currentToken] == MaximumEntropyNameFinder.Other))
					{
						if (start != -1)
						{
							names.Add(new Span(start, currentToken - 1));
						}
						start = -1;
					}
					if (finderTags[currentFinder][currentToken] == MaximumEntropyNameFinder.Start)
					{
						start = currentToken;
					}
				}
				if (start != - 1)
				{
					names.Add(new Span(start, tokens.Length - 1));
				}
				AddNames(models[currentFinder], names, tokenParses, lineParse);
			}
			output.Append(lineParse.Show());
			//output.Append("\r\n");
			
			return output.ToString();
		}
		
		/// <summary>Adds sgml style name tags to the specified input string and outputs this information</summary>
		/// <param name="models">The model names for the name finders to be used</param>
		/// <param name="line">The input</param>
		private string ProcessText(string[] models, string line)
		{
			var output = new System.Text.StringBuilder();

			var finderTags = new string[models.Length][];
			Dictionary<string, string>[] previousTokenMaps = CreatePreviousTokenMaps(models);
			
			if (line.Length == 0)
			{
				ClearPreviousTokenMaps(previousTokenMaps);
				output.Append("\r\n");
			}
			else
			{
				Span[] spans = TokenizeToSpans(line);
				string[] tokens = SpansToStrings(spans, line);
				for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
				{
					MaximumEntropyNameFinder finder = mFinders[models[currentFinder]];
					finderTags[currentFinder] = finder.Find(tokens, previousTokenMaps[currentFinder]);
				}
				UpdatePreviousTokenMaps(previousTokenMaps, tokens, finderTags);
				for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
				{
					for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
					{
						//check for end tags
						if (currentToken != 0)
						{
							if ((finderTags[currentFinder][currentToken] == MaximumEntropyNameFinder.Start || finderTags[currentFinder][currentToken] == MaximumEntropyNameFinder.Other) && (finderTags[currentFinder][currentToken - 1] == MaximumEntropyNameFinder.Start || finderTags[currentFinder][currentToken - 1] == MaximumEntropyNameFinder.Continue))
							{
								output.Append("</" + models[currentFinder] + ">");
							}
						}
					}
					if (currentToken > 0 && spans[currentToken - 1].End < spans[currentToken].Start)
					{
						output.Append(line.Substring(spans[currentToken - 1].End, (spans[currentToken].Start) - (spans[currentToken - 1].End)));
					}
					//check for start tags
					for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
					{
						if (finderTags[currentFinder][currentToken] == MaximumEntropyNameFinder.Start)
						{
							output.Append("<" + models[currentFinder] + ">");
						}
					}
					output.Append(tokens[currentToken]);
				}
				//final end tags
				if (tokens.Length != 0)
				{
					for (int currentFinder = 0, finderCount = models.Length; currentFinder < finderCount; currentFinder++)
					{
						if (finderTags[currentFinder][tokens.Length - 1] == MaximumEntropyNameFinder.Start || finderTags[currentFinder][tokens.Length - 1] == MaximumEntropyNameFinder.Continue)
						{
							output.Append("</" + models[currentFinder] + ">");
						}
					}
				}
				if (tokens.Length != 0)
				{
					if (spans[tokens.Length - 1].End < line.Length)
					{
						output.Append(line.Substring(spans[tokens.Length - 1].End));
					}
				}
				output.Append("\r\n");
			}
			return output.ToString();
		}

		public string GetNames(string[] models, string data)
		{
			CreateModels(models);
			return ProcessText(models, data);
		}

		public string GetNames(string[] models, Parse data)
		{
			CreateModels(models);
			return ProcessParse(models, data);
		}

		private void CreateModels(IEnumerable<string> models)
		{
		    foreach (string mod in models)
		    {
		        if (!mFinders.ContainsKey(mod))
		        {
		            string modelName = mModelPath + mod + ".nbin";
		            SharpEntropy.IMaximumEntropyModel model = new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(modelName));
		            var finder = new MaximumEntropyNameFinder(model);
		            mFinders.Add(mod, finder);
		        }
		    }
		}
	}
	
	internal enum CharacterEnum
	{
		Whitespace,
		Alphabetic,
		Numeric,
		Other
	}
}
