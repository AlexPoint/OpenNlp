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

//This file is based on the Context.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Similarity
{
	/// <summary>
    /// Specifies the context of a mention for computing gender, number, and semantic compatibility.
    /// </summary>
    public class Context : Mention.Mention
	{
        private string mHeadTokenText;
        private string mHeadTokenTag;
        private Util.Set<string> mSynsets;
        private object[] mTokens;

        private int mHeadTokenIndex;

        public virtual object[] Tokens
		{
			get
			{
				return mTokens;
			}
            protected internal set
            {
                mTokens = value;
            }	
		}

		public virtual string HeadTokenText
		{
			get
			{
				return mHeadTokenText;
			}
            protected internal set
            {
                mHeadTokenText = value;
            }	
		}

		public virtual string HeadTokenTag
		{
			get
			{
				return mHeadTokenTag;
			}
            protected internal set
            {
                mHeadTokenTag = value;
            }	
		}

		public virtual Util.Set<string> Synsets
		{
			get
			{
				return mSynsets;
			}
            protected internal set
            {
                mSynsets = value;
            }
		}

        /// <summary>
        /// The token index in the head word of this mention.
        /// </summary>
		public virtual int HeadTokenIndex
		{
			get
			{
				return mHeadTokenIndex;
			}
            protected internal set
            {
                mHeadTokenIndex = value;
            }
		}

        public Context(Util.Span span, Util.Span headSpan, int entityId, Mention.IParse parse, string extentType, string nameType, Mention.IHeadFinder headFinder)
            : base(span, headSpan, entityId, parse, extentType, nameType)
		{
			Initialize(headFinder);
		}

        public Context(object[] tokens, string headToken, string headTag, string neType) : base(null, null, 1, null, null, neType)
		{
			mTokens = tokens;
			mHeadTokenIndex = tokens.Length - 1;
			mHeadTokenText = headToken;
			mHeadTokenTag = headTag;
			mSynsets = GetSynsetSet(this);
		}

        public Context(Mention.Mention mention, Mention.IHeadFinder headFinder) : base(mention)
		{
			Initialize(headFinder);
		}

        private void Initialize(Mention.IHeadFinder headFinder)
		{
            Mention.IParse head = headFinder.GetLastHead(Parse);
			List<Mention.IParse> tokenList = head.Tokens;
			mHeadTokenIndex = headFinder.GetHeadIndex(head);
            Mention.IParse headToken = headFinder.GetHeadToken(head);
            mTokens = tokenList.ToArray();
			mHeadTokenTag = headToken.SyntacticType;
			mHeadTokenText = headToken.ToString();
			if (mHeadTokenTag.StartsWith("NN") && !mHeadTokenTag.StartsWith("NNP"))
			{
				mSynsets = GetSynsetSet(this);
			}
			else
			{
				mSynsets = new Util.HashSet<string>();
			}
		}

        public static Context[] ConstructContexts(Mention.Mention[] mentions, Mention.IHeadFinder headFinder)
		{
			Context[] contexts = new Context[mentions.Length];
            for (int currentMention = 0; currentMention < mentions.Length; currentMention++)
			{
                contexts[currentMention] = new Context(mentions[currentMention], headFinder);
			}
			return contexts;
		}
		
		public override string ToString()
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
            for (int currentToken = 0; currentToken < mTokens.Length; currentToken++)
			{
                output.Append(mTokens[currentToken]).Append(" ");
			}
            return output.ToString();
		}
		
		public static Context ParseContext(string word)
		{
			string[] parts = word.Split('/');
			if (parts.Length == 2)
			{
				string[] tokens = parts[0].Split(' ');
				return new Context(tokens, tokens[tokens.Length - 1], parts[1], null);
			}
			else if (parts.Length == 3)
			{
				string[] tokens = parts[0].Split(' ');
				return new Context(tokens, tokens[tokens.Length - 1], parts[1], parts[2]);
			}
			return null;
		}
		
		private static Util.Set<string> GetSynsetSet(Context context)
		{
			Util.Set<string> synsetSet = new Util.HashSet<string>();
			string[] lemmas = GetLemmas(context);
            Mention.IDictionary dictionary = Mention.DictionaryFactory.GetDictionary();
			//System.err.println(lemmas.length+" lemmas for "+c.headToken);
			for (int currentLemma = 0; currentLemma < lemmas.Length; currentLemma++)
			{
				synsetSet.Add(dictionary.GetSenseKey(lemmas[currentLemma], "NN", 0));
				string[] synsets = dictionary.GetParentSenseKeys(lemmas[currentLemma], "NN", 0);
				for (int currentSynset = 0, sn = synsets.Length; currentSynset < sn; currentSynset++)
				{
					synsetSet.Add(synsets[currentSynset]);
				}
			}
			return synsetSet;
		}
		
		private static string[] GetLemmas(Context context)
		{
			string word = context.HeadTokenText.ToLower();
            return Mention.DictionaryFactory.GetDictionary().GetLemmas(word, "NN");
		}
	}
}