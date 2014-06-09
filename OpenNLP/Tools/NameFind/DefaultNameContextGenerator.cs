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

//This file is based on the DefaultNameContextGenerator.java source file found in the
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
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.NameFind
{
	/// <summary>
	/// Class for determining contextual features for a tag/chunk style named-entity recognizer.
	/// </summary>
	/// 
	public class DefaultNameContextGenerator : INameContextGenerator
	{
		
		// patterns
		private Regex mLowercasePattern;
		private Regex mTwoDigitsPattern;
		private Regex mFourDigitsPattern;
		private Regex mContainsNumberPattern;
		private Regex mContainsLetterPattern;
		private Regex mContainsHyphensPattern;
		private Regex mContainsBackslashPattern;
		private Regex mContainsCommaPattern;
		private Regex mContainsPeriodPattern;
		private Regex mAllCapsPattern;
		private Regex mCapPeriodPattern;
		private Regex mInitialCapPattern;
		
		private Util.Cache mContextsCache;
		private object mWordsKey;
		private int mPreviousIndex = -1;
		private List<string> mPreviousStaticFeatures;

		/// <summary>
		/// Creates a name context generator.
		/// </summary>
		public DefaultNameContextGenerator() : this(0)
		{
		}
		
		/// <summary>
		/// Creates a name context generator with the specified cache size.
		/// </summary>
		public DefaultNameContextGenerator(int cacheSize) : base()
		{
			InitializePatterns();
			if (cacheSize > 0)
			{
				mContextsCache = new Cache(cacheSize);
			}
		}

		private void InitializePatterns()
		{
			mLowercasePattern = new Regex("^[a-z]+$");
			mTwoDigitsPattern = new Regex("^[0-9][0-9]$");
			mFourDigitsPattern = new Regex("^[0-9][0-9][0-9][0-9]$");
			mContainsNumberPattern = new Regex("[0-9]");
			mContainsLetterPattern = new Regex("[a-zA-Z]");
			mContainsHyphensPattern = new Regex("-");
			mContainsBackslashPattern = new Regex("/");
			mContainsCommaPattern = new Regex(",");
			mContainsPeriodPattern = new Regex("\\.");
			mAllCapsPattern = new Regex("^[A-Z]+$");
			mCapPeriodPattern = new Regex("^[A-Z]\\.$");
			mInitialCapPattern = new Regex("^[A-Z]");
		}
		
		public virtual string[] GetContext(object context)
		{
			object[] contextData = (object[]) context;
            return (GetContext(((int)contextData[0]), (List<string>)contextData[1], (List<string>)contextData[2], (IDictionary<string, string>)contextData[3]));
		}
		
		public virtual string[] GetContext(int index, List<string> sequence, Sequence outcomesSequence, object[] additionalContext)
		{
			return GetContext(index, sequence, outcomesSequence.Outcomes, (IDictionary<string, string>) additionalContext[0]);
		}

        public virtual string[] GetContext(int index, List<string> tokens, List<string> predicates, IDictionary<string, string> previousTags)
		{
			return (GetContext(index, tokens.ToArray(), predicates.ToArray(), previousTags));
		}
		
		public virtual string[] GetContext(int index, object[] sequence, string[] priorDecisions, object[] additionalContext) 
		{
			return (GetContext(index, (string[])sequence, priorDecisions, (IDictionary<string, string>) additionalContext[0]));
		}

		/// <summary>
		/// Return the context for finding names at the specified index.
		/// </summary>
		/// <param name="index">
		/// The index of the token in the specified tokens array for which the context should be constructed. 
		/// </param>
		/// <param name="tokens">
		/// tokens of the sentence.
		/// </param>
		/// <param name="predicates">
		/// The previous decisions made in the tagging of this sequence.  Only indices less than {index} will be examined.
		/// </param>
		/// <param name="previousTags">
		/// A mapping between tokens and the previous outcome for these tokens. 
		/// </param>
		/// <returns>
		/// the context for finding names at the specified index.
		/// </returns>
		public virtual string[] GetContext(int index, string[] tokens, string[] predicates, IDictionary<string, string> previousTags)
		{
			string previous = MaximumEntropyNameFinder.Other;
			string previousPrevious = MaximumEntropyNameFinder.Other;
			if (index > 1)
			{
				previousPrevious = predicates[index - 2];
			}
			if (index > 0)
			{
				previous = predicates[index - 1];
			}

			string cacheKey = index.ToString(System.Globalization.CultureInfo.InvariantCulture) + previous + previousPrevious;
			if (mContextsCache != null)
			{
				if (mWordsKey == tokens)
				{
					string[] cachedContexts = (string[])mContextsCache[cacheKey];
					if (cachedContexts != null)
					{
						return cachedContexts;
					}
				}
				else
				{
					mContextsCache.Clear();
					mWordsKey = tokens;
				}
			}
			List<string> features;
			if (mWordsKey == tokens && index == mPreviousIndex)
			{
				features = mPreviousStaticFeatures;
			}
			else
			{
				features = GetStaticFeatures(tokens, index, previousTags);
				mPreviousIndex = index;
				mPreviousStaticFeatures = features;
			}
			
			int featureCount = features.Count;
			string[] contexts = new string[featureCount + 4];
			for (int currentFeature = 0; currentFeature < featureCount; currentFeature++)
			{
				contexts[currentFeature] = features[currentFeature];
			}
			contexts[featureCount] = "po=" + previous;
			contexts[featureCount + 1] = "pow=" + previous + tokens[index];
			contexts[featureCount + 2] = "powf=" + previous + WordFeature(tokens[index]);
			contexts[featureCount + 3] = "ppo=" + previousPrevious;
			if (mContextsCache != null)
			{
				mContextsCache[cacheKey] = contexts;
			}
			return contexts;
		}
		
		/// <summary>
		/// Returns a list of the features for <code>tokens[index]</code> that can
		/// be safely cached.  In other words, return a list of all
		/// features that do not depend on previous outcome or decision
		/// features.  This method is called by <code>search</code>.
		/// </summary>
		/// <param name="tokens">
		/// The list of tokens being processed.
		/// </param>
		/// <param name="index">
		/// The index of the token whose features should be
		/// returned.
		/// </param>
		/// <param name="previousTags">
		/// The list of previous tags.
		/// </param>
		/// <returns> a list of the features for <code>tokens[index]</code> that can
		/// be safely cached.
		/// </returns>
        private List<string> GetStaticFeatures(string[] tokens, int index, IDictionary<string, string> previousTags)
		{
            List<string> features = new List<string>();
			features.Add("def");
			
			//current word
			string currentWord = tokens[index].ToLower(System.Globalization.CultureInfo.InvariantCulture);
			features.Add("w=" + currentWord);
			string wordFeature = WordFeature(tokens[index]);
			features.Add("wf=" + wordFeature);
			features.Add("w&wf=" + currentWord + "," + wordFeature);

            if (previousTags.ContainsKey(tokens[index]))
            {
                features.Add("pd=" + previousTags[tokens[index]]);
            }
            else
            {
                features.Add("pd=");
            }			
			if (index == 0)
			{
				features.Add("df=it");
			}
			// previous previous word
			if (index - 2 >= 0)
			{
				string previousPreviousWord = tokens[index - 2].ToLower(System.Globalization.CultureInfo.InvariantCulture);
				features.Add("ppw=" + previousPreviousWord);
				string previousPreviousWordFeature = WordFeature(tokens[index - 2]);
				features.Add("ppwf=" + previousPreviousWordFeature);
				features.Add("ppw&f=" + previousPreviousWord + "," + previousPreviousWordFeature);
			}
			else
			{
				features.Add("ppw=BOS");
			}
			// previous word
			if (index == 0)
			{
				features.Add("pw=BOS");
				features.Add("pw=BOS,w=" + currentWord);
				features.Add("pwf=BOS,wf" + wordFeature);
			}
			else
			{
				string previousWord = tokens[index - 1].ToLower(System.Globalization.CultureInfo.InvariantCulture);
				features.Add("pw=" + previousWord);
				string previousWordFeature = WordFeature(tokens[index - 1]);
				features.Add("pwf=" + previousWordFeature);
				features.Add("pw&f=" + previousWord + "," + previousWordFeature);
				features.Add("pw=" + previousWord + ",w=" + currentWord);
				features.Add("pwf=" + previousWordFeature + ",wf=" + wordFeature);
			}
			//next word
			if (index + 1 >= tokens.Length)
			{
				features.Add("nw=EOS");
				features.Add("w=" + currentWord + ",nw=EOS");
				features.Add("wf=" + wordFeature + ",nw=EOS");
			}
			else
			{
				string nextWord = tokens[index + 1].ToLower(System.Globalization.CultureInfo.InvariantCulture);
				features.Add("nw=" + nextWord);
				string nextWordFeature = WordFeature(tokens[index + 1]);
				features.Add("nwf=" + nextWordFeature);
				features.Add("nw&f=" + nextWord + "," + nextWordFeature);
				features.Add("w=" + currentWord + ",nw=" + nextWord);
				features.Add("wf=" + wordFeature + ",nwf=" + nextWordFeature);
			}
			if (index + 2 >= tokens.Length)
			{
				features.Add("nnw=EOS");
			}
			else
			{
				string nextNextWord = tokens[index + 2].ToLower(System.Globalization.CultureInfo.InvariantCulture);
				features.Add("nnw=" + nextNextWord);
				string nextNextWordFeature = WordFeature(tokens[index + 2]);
				features.Add("nnwf=" + nextNextWordFeature);
				features.Add("nnw&f=" + nextNextWord + "," + nextNextWordFeature);
			}
			
			return features;
		}
		
		
		/// <summary>
		/// Return the most relevant feature for a given word.  This method
		/// is used to get the features for words
		/// within a window of the word being analyzed.  Typical features
		/// are "2d" (2 digits); "4d" (4 digits); and "ac" (all caps).
		/// Note that only a single feature is returned.  The default
		/// feature is "other".
		/// </summary>
		/// <param name="word">
		/// The word whose features should be returned.
		/// </param>
		/// <returns>
		/// A feature code.
		/// </returns>
		private string WordFeature(string word)
		{
			string feature;
			if (mLowercasePattern.IsMatch(word))
			{
				feature = "lc";
			}
			else if (mTwoDigitsPattern.IsMatch(word))
			{
				feature = "2d";
			}
			else if (mFourDigitsPattern.IsMatch(word))
			{
				feature = "4d";
			}
			else if (mContainsNumberPattern.IsMatch(word))
			{
				if (mContainsLetterPattern.IsMatch(word))
				{
					feature = "an";
				}
				else if (mContainsHyphensPattern.IsMatch(word))
				{
					feature = "dd";
				}
				else if (mContainsBackslashPattern.IsMatch(word))
				{
					feature = "ds";
				}
				else if (mContainsCommaPattern.IsMatch(word))
				{
					feature = "dc";
				}
				else if (mContainsPeriodPattern.IsMatch(word))
				{
					feature = "dp";
				}
				else
				{
					feature = "num";
				}
			}
			else if (mAllCapsPattern.IsMatch(word) && word.Length == 1)
			{
				feature = "sc";
			}
			else if (mAllCapsPattern.IsMatch(word))
			{
				feature = "ac";
			}
			else if (mCapPeriodPattern.IsMatch(word))
			{
				feature = "cp";
			}
			else if (mInitialCapPattern.IsMatch(word))
			{
				feature = "ic";
			}
			else
			{
				feature = "other";
			}
			
			return feature;
		}

	}
}
