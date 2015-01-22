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
        private object[] _tokens;

        public object[] Tokens { get; protected internal set; }

		public string HeadTokenText { get; protected internal set; }

		public string HeadTokenTag { get; protected internal set; }

		public Util.Set<string> Synsets { get; protected internal set; }

        /// <summary>
        /// The token index in the head word of this mention.
        /// </summary>
		public int HeadTokenIndex { get; protected internal set; }


        // Constructors --------------------

        public Context(Util.Span span, Util.Span headSpan, int entityId, Mention.IParse parse, string extentType, string nameType, Mention.IHeadFinder headFinder)
            : base(span, headSpan, entityId, parse, extentType, nameType)
		{
			Initialize(headFinder);
		}

        public Context(object[] tokens, string headToken, string headTag, string neType) : base(null, null, 1, null, null, neType)
		{
			_tokens = tokens;
			this.HeadTokenIndex = tokens.Length - 1;
			this.HeadTokenText = headToken;
			this.HeadTokenTag = headTag;
			this.Synsets = GetSynsetSet(this);
		}

        public Context(Mention.Mention mention, Mention.IHeadFinder headFinder) : base(mention)
		{
			Initialize(headFinder);
		}

        private void Initialize(Mention.IHeadFinder headFinder)
		{
            Mention.IParse head = headFinder.GetLastHead(Parse);
			List<Mention.IParse> tokenList = head.Tokens;
			this.HeadTokenIndex = headFinder.GetHeadIndex(head);
            Mention.IParse headToken = headFinder.GetHeadToken(head);
            _tokens = tokenList.ToArray();
			this.HeadTokenTag = headToken.SyntacticType;
			this.HeadTokenText = headToken.ToString();
			if (PartsOfSpeech.IsNoun(this.HeadTokenTag) && !PartsOfSpeech.IsProperNoun(this.HeadTokenTag))
			{
				this.Synsets = GetSynsetSet(this);
			}
			else
			{
				this.Synsets = new Util.HashSet<string>();
			}
		}

        public static Context[] ConstructContexts(Mention.Mention[] mentions, Mention.IHeadFinder headFinder)
		{
			var contexts = new Context[mentions.Length];
            for (int currentMention = 0; currentMention < mentions.Length; currentMention++)
			{
                contexts[currentMention] = new Context(mentions[currentMention], headFinder);
			}
			return contexts;
		}
		
		public override string ToString()
		{
			var output = new System.Text.StringBuilder();
            foreach (object token in _tokens)
            {
                output.Append(token).Append(" ");
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
			foreach (string lemma in lemmas)
			{
			    synsetSet.Add(dictionary.GetSenseKey(lemma, PartsOfSpeech.NounSingularOrMass, 0));
                string[] synsets = dictionary.GetParentSenseKeys(lemma, PartsOfSpeech.NounSingularOrMass, 0);
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
            return Mention.DictionaryFactory.GetDictionary().GetLemmas(word, PartsOfSpeech.NounSingularOrMass);
		}
	}
}