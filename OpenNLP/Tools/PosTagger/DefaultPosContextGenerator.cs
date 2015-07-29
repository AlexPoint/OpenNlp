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

//This file is based on the DefaultPOSContextGenerator.java source file found in the
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace OpenNLP.Tools.PosTagger
{
	/// <summary> 
	/// A context generator for the POS Tagger.
	/// </summary>	
	public class DefaultPosContextGenerator : IPosContextGenerator
	{
		protected internal const string SentenceEnd = "*SE*";
		protected internal const string SentenceBeginning = "*SB*";

		private const int PrefixLength = 4;
		private const int SuffixLength = 4;
		
		private static readonly Regex HasCapitalRegex = new Regex("[A-Z]", RegexOptions.Compiled);
		private static readonly Regex HasNumericRegex = new Regex("[0-9]", RegexOptions.Compiled);
		
	    private readonly MemoryCache memoryCache;

        // Constructors ----------------------------------------

		public DefaultPosContextGenerator() : this(0){}
		
		public DefaultPosContextGenerator(int cacheSizeInMegaBytes) 
		{
			if (cacheSizeInMegaBytes > 0)
			{
			    var properties = new NameValueCollection
			    {
			        {"cacheMemoryLimitMegabytes", cacheSizeInMegaBytes.ToString()}
			    };
			    memoryCache = new MemoryCache("posContextCache", properties);
			}
		}


        // Methods ---------------------------------------------

		public virtual string[] GetContext(object input)
		{
			var data = (object[]) input;
			return GetContext(((int) data[0]), (string[]) data[1], (string[]) data[2], null);
		}
		
		public virtual string[] GetContext(int index, string[] sequence, string[] priorDecisions, object[] additionalContext) 
		{
			return GetContext(index, sequence, priorDecisions);
		}

		/// <summary>
		/// Returns the context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
		/// </summary>
		/// <param name="index">
		/// The index of the token for which the context is provided.
		/// </param>
		/// <param name="tokens">
		/// The tokens in the sentence.
		/// </param>
		/// <param name="tags">
		/// The tags assigned to the previous words in the sentence.
		/// </param>
		/// <returns>
		/// The context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
		/// </returns>
		public virtual string[] GetContext(int index, string[] tokens, string[] tags) 
		{
            string next, nextNext, lex, previous, previousPrevious;
            string tagPrevious, tagPreviousPrevious;
            tagPrevious = tagPreviousPrevious = null;
            next = nextNext = lex = previous = previousPrevious = null;
			
			lex = tokens[index];
			if (tokens.Length > index + 1) 
			{
				next = tokens[index + 1];
				if (tokens.Length > index + 2)
				{
					nextNext = tokens[index + 2];
				}
				else
				{
					nextNext = SentenceEnd; 
				}
			}
			else
			{
				next = SentenceEnd; 
			}
			
			if (index - 1 >= 0) 
			{
				previous = tokens[index - 1];
				tagPrevious = tags[index - 1];

				if (index - 2 >= 0) 
				{
					previousPrevious = tokens[index - 2];
					tagPreviousPrevious = tags[index - 2];
				}
				else
				{
					previousPrevious = SentenceBeginning; 
				}
			}
			else
			{
				previous = SentenceBeginning; 
			}
			
			string cacheKey = string.Format("{0}||{1}||{2}||{3}", index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                string.Join("|", tokens), tagPrevious, tagPreviousPrevious);
			if (memoryCache != null) 
			{
				var cachedContexts = (string[]) memoryCache[cacheKey];    
				if (cachedContexts != null) 
				{
					return cachedContexts;
				}
			}

		    var eventList = CreateEventList(lex, previous, previousPrevious, tagPrevious, tagPreviousPrevious, next, nextNext);

			string[] contexts = eventList.ToArray();
			if (memoryCache != null) 
			{
				memoryCache[cacheKey] = contexts;
			}
			return contexts;
		}

	    private List<string> CreateEventList(string lex, string previous, string previousPrevious, 
            string tagPrevious, string tagPreviousPrevious, string next, string nextNext)
	    {
            var eventList = new List<string>();

            // add the word itself
            eventList.Add("w=" + lex);

            // do some basic suffix analysis
            string[] suffixes = GetSuffixes(lex);
            for (int currentSuffix = 0; currentSuffix < suffixes.Length; currentSuffix++)
            {
                eventList.Add("suf=" + suffixes[currentSuffix]);
            }

            string[] prefixes = GetPrefixes(lex);
            for (int currentPrefix = 0; currentPrefix < prefixes.Length; currentPrefix++)
            {
                eventList.Add("pre=" + prefixes[currentPrefix]);
            }
            // see if the word has any special characters
            if (lex.IndexOf('-') != -1)
            {
                eventList.Add("h");
            }

            if (HasCapitalRegex.IsMatch(lex))
            {
                eventList.Add("c");
            }

            if (HasNumericRegex.IsMatch(lex))
            {
                eventList.Add("d");
            }

            // add the words and positions of the surrounding context
            if ((object)previous != null)
            {
                eventList.Add("p=" + previous);
                if ((object)tagPrevious != null)
                {
                    eventList.Add("t=" + tagPrevious);
                }
                if ((object)previousPrevious != null)
                {
                    eventList.Add("pp=" + previousPrevious);
                    if ((object)tagPreviousPrevious != null)
                    {
                        eventList.Add("tt=" + tagPreviousPrevious);
                    }
                }
            }

            if ((object)next != null)
            {
                eventList.Add("n=" + next);
                if ((object)nextNext != null)
                {
                    eventList.Add("nn=" + nextNext);
                }
            }

	        return eventList;
	    }
        
        protected internal static string[] GetPrefixes(string lex)
        {
            var prefixes = new string[PrefixLength];
            for (int currentPrefix = 0; currentPrefix < PrefixLength; currentPrefix++)
            {
                prefixes[currentPrefix] = lex.Substring(0, (Math.Min(currentPrefix + 1, lex.Length)) - (0));
            }
            return prefixes;
        }

        protected internal static string[] GetSuffixes(string lex)
        {
            var suffixes = new string[SuffixLength];
            for (int currentSuffix = 0; currentSuffix < SuffixLength; currentSuffix++)
            {
                suffixes[currentSuffix] = lex.Substring(Math.Max(lex.Length - currentSuffix - 1, 0));
            }
            return suffixes;
        }
		
	}
}
