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

//This file is based on the AbstractMentionFinder.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Mention
{
	/// <summary>
    /// Provides default implementation of many of the methods in the IMentionFinder interface.
    /// </summary>
	public abstract class AbstractMentionFinder : IMentionFinder
	{
        private IHeadFinder mHeadFinder;
        private bool mPrenominalNamedEntitiesCollection;
        private bool mCoordinatedNounPhrasesCollection;

        protected internal IHeadFinder HeadFinder
        {
            get
            {
                return mHeadFinder;
            }
            set
            {
                mHeadFinder = value;
            }
        }

		public virtual bool PrenominalNamedEntitiesCollection
		{
			get
			{
				return mPrenominalNamedEntitiesCollection;
			}
			set
			{
				mPrenominalNamedEntitiesCollection = value;
			}
		}

		public virtual bool CoordinatedNounPhrasesCollection
		{
			get
			{
				return mCoordinatedNounPhrasesCollection;
			}
			set
			{
				mCoordinatedNounPhrasesCollection = value;
			}
		}

		private void GatherHeads(IParse parse, Dictionary<IParse, IParse> heads)
		{
			IParse head = mHeadFinder.GetHead(parse);
			if (head != null)
			{
				heads[head] = parse;
			}

			List<IParse> nounPhrases = parse.NounPhrases;
            foreach (IParse currentNounPhrase in nounPhrases)
            {
                GatherHeads(currentNounPhrase, heads);
            }
		}
		
		/// <summary>
        /// Assigns head relations between noun phrases and the child noun phrase
		/// which is their head.
		/// </summary>
		/// <param name="nounPhrases">
        /// List of valid noun phrases for this mention finder.
		/// </param>
		/// <returns>
        /// mapping from noun phrases and the child noun phrase which is their head
		/// </returns>
        protected internal virtual Dictionary<IParse, IParse> ConstructHeadMap(List<IParse> nounPhrases)
		{
            Dictionary<IParse, IParse> headMap = new Dictionary<IParse, IParse>();
            for (int currentNounPhrase = 0; currentNounPhrase < nounPhrases.Count; currentNounPhrase++)
			{
				GatherHeads(nounPhrases[currentNounPhrase], headMap);
			}
			return headMap;
		}
		
		protected internal virtual bool IsBasalNounPhrase(IParse nounPhrase)
		{
            return (nounPhrase.NounPhrases.Count == 0);
		}

        protected internal virtual bool IsPossessive(IParse nounPhrase)
		{
            List<IParse> parts = nounPhrase.SyntacticChildren;
			if (parts.Count > 1)
			{
				if (parts[0].IsNounPhrase)
				{
                    List<IParse> childTokens = parts[0].Tokens;
					IParse token = childTokens[childTokens.Count - 1];
					if (token.SyntacticType == "POS")
					{
						return true;
					}
				}
			}
			if (parts.Count > 2)
			{
                if (parts[1].IsToken && parts[1].SyntacticType == "POS" && parts[0].IsNounPhrase && parts[2].IsNounPhrase)
				{
					return true;
				}
			}
			return false;
		}
		
		protected internal virtual bool IsOfPrepPhrase(IParse nounPhrase)
		{
			List<IParse> parts = nounPhrase.SyntacticChildren;
			if (parts.Count == 2)
			{
                if (parts[0].IsNounPhrase)
				{
                    List<IParse> childParts = parts[1].SyntacticChildren;
                    if (childParts.Count == 2)
					{
                        if (childParts[0].IsToken && childParts[0].ToString() == "of")
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		
		protected internal virtual bool IsConjoinedBasal(IParse nounPhrase)
		{
			List<IParse> parts = nounPhrase.SyntacticChildren;
			bool allToken = true;
			bool hasConjunction = false;
            foreach (IParse currentPart in parts)
			{
                if (currentPart.IsToken)
				{
                    if (currentPart.SyntacticType == "CC")
					{
						hasConjunction = true;
					}
				}
				else
				{
					allToken = false;
					break;
				}
			}
			return (allToken && hasConjunction);
		}
		
		private void CollectCoordinatedNounPhraseMentions(IParse nounPhrase, List<Mention> entities)
		{
			//System.err.println("collectCoordNp: "+np);
			List<IParse> nounPhraseTokens = nounPhrase.Tokens;
			bool inCoordinatedNounPhrase = false;
			int lastNounPhraseTokenIndex = mHeadFinder.GetHeadIndex(nounPhrase);
			for (int tokenIndex = lastNounPhraseTokenIndex - 1; tokenIndex >= 0; tokenIndex--)
			{
				IParse token = nounPhraseTokens[tokenIndex];
				string tokenText = token.ToString();
				if (tokenText == "and" || tokenText == "or")
				{
					if (lastNounPhraseTokenIndex != tokenIndex)
					{
						if (tokenIndex - 1 >= 0 && (nounPhraseTokens[tokenIndex - 1]).SyntacticType.StartsWith("NN"))
						{
                            Util.Span nounPhraseSpan = new Util.Span((nounPhraseTokens[tokenIndex + 1]).Span.Start, (nounPhraseTokens[lastNounPhraseTokenIndex]).Span.End);
							Mention nounPhraseSpanExtent = new Mention(nounPhraseSpan, nounPhraseSpan, token.EntityId, null, "CNP");
							entities.Add(nounPhraseSpanExtent);
							//System.err.println("adding extent for conjunction in: "+np+" preeceeded by "+((Parse) npTokens.get(ti-1)).getSyntacticType());
							inCoordinatedNounPhrase = true;
						}
						else
						{
							break;
						}
					}
					lastNounPhraseTokenIndex = tokenIndex - 1;
				}
				else if (inCoordinatedNounPhrase && tokenText.Equals(","))
				{
					if (lastNounPhraseTokenIndex != tokenIndex)
					{
                        Util.Span nounPhraseSpan = new Util.Span((nounPhraseTokens[tokenIndex + 1]).Span.Start, (nounPhraseTokens[lastNounPhraseTokenIndex]).Span.End);
						Mention nounPhraseSpanExtent = new Mention(nounPhraseSpan, nounPhraseSpan, token.EntityId, null, "CNP");
						entities.Add(nounPhraseSpanExtent);
						//System.err.println("adding extent for comma in: "+np);
					}
					lastNounPhraseTokenIndex = tokenIndex - 1;
				}
				else if (inCoordinatedNounPhrase && tokenIndex == 0 && lastNounPhraseTokenIndex >= 0)
				{
                    Util.Span nounPhraseSpan = new Util.Span((nounPhraseTokens[tokenIndex]).Span.Start, (nounPhraseTokens[lastNounPhraseTokenIndex]).Span.End);
					Mention nounPhraseSpanExtent = new Mention(nounPhraseSpan, nounPhraseSpan, token.EntityId, null, "CNP");
					entities.Add(nounPhraseSpanExtent);
					//System.err.println("adding extent for start coord in: "+np);
				}
			}
		}
		
		private static bool IsHandledPronoun(string token)
		{
			return (Linker.SingularThirdPersonPronounPattern.IsMatch(token.ToString()) || 
                Linker.PluralThirdPersonPronounPattern.IsMatch(token.ToString()) || 
                Linker.SpeechPronounPattern.IsMatch(token.ToString()));
		}
		
		private void CollectPossessivePronouns(IParse nounPhrase, List<Mention> entities)
		{
			//TODO: Look at how training is done and examine whether this is needed or can be accomidated in a different way.
			/*
			List snps = np.getSubNounPhrases();
			if (snps.size() != 0) {
			//System.err.println("AbstractMentionFinder: Found existing snps");
			for (int si = 0, sl = snps.size(); si < sl; si++) {
			Parse snp = (Parse) snps.get(si);
			Extent ppExtent = new Extent(snp.getSpan(), snp.getSpan(), snp.getEntityId(), null,Linker.PRONOUN_MODIFIER);
			entities.add(ppExtent);
			}
			}
			else {
			*/
			//System.err.println("AbstractEntityFinder.collectPossesivePronouns: "+np);
			List<IParse> nounPhraseTokens = nounPhrase.Tokens;
			IParse headToken = mHeadFinder.GetHeadToken(nounPhrase);
			for (int tokenIndex = nounPhraseTokens.Count - 2; tokenIndex >= 0; tokenIndex--)
			{
				IParse token = nounPhraseTokens[tokenIndex];
				if (token == headToken)
				{
					continue;
				}
				if (token.SyntacticType.StartsWith("PRP") && IsHandledPronoun(token.ToString()))
				{
					Mention possessivePronounExtent = new Mention(token.Span, token.Span, token.EntityId, null, OpenNLP.Tools.Coreference.Linker.PronounModifier);
					//System.err.println("AbstractEntityFinder.collectPossesivePronouns: adding possesive pronoun: "+tok+" "+tok.getEntityId());
					entities.Add(possessivePronounExtent);
					//System.err.println("AbstractMentionFinder: adding pos-pro: "+ppExtent);
					break;
				}
			}
			//}
		}
		
		private static void RemoveDuplicates(List<Mention> extents)
		{
			Mention lastExtent = null;
			foreach (Mention extent in extents)
            {
                if (lastExtent != null && extent.Span.Equals(lastExtent.Span))
                {
                    extents.Remove(extent);
                }
                else
                {
                    lastExtent = extent;
                }
            }
  		}

        private static bool IsHeadOfExistingMention(IParse nounPhrase, Dictionary<IParse, IParse> headMap, Util.Set<IParse> mentions)
		{
            IParse head = nounPhrase;
            while (headMap.ContainsKey(head))
            {
                head = headMap[head];
                if (mentions.Contains(head))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ClearMentions(Util.Set<IParse> mentions, IParse nounPhrase)
		{
			Util.Span nounPhraseSpan = nounPhrase.Span;

            //loop backwards through the set so that we can remove from the end forwards
            for (int currentMention = mentions.Count - 1; currentMention > -1; currentMention--)
            {
                if (mentions[currentMention].Span.Contains(nounPhraseSpan))
                {
                    mentions.Remove(mentions[currentMention]);
                }
            }
		}

        private Mention[] CollectMentions(List<IParse> nounPhrases, Dictionary<IParse, IParse> headMap)
		{
            List<Mention> mentions = new List<Mention>(nounPhrases.Count);
			Util.Set<IParse> recentMentions = new Util.HashSet<IParse>();
			//System.err.println("AbtractMentionFinder.collectMentions: "+headMap);
			for (int nounPhraseIndex = 0; nounPhraseIndex < nounPhrases.Count; nounPhraseIndex++)
			{
				IParse nounPhrase = nounPhrases[nounPhraseIndex];
				//System.err.println("AbstractMentionFinder: collectMentions: np[" + npi + "]=" + np + " head=" + headMap.get(np));
				if (!IsHeadOfExistingMention(nounPhrase, headMap, recentMentions))
				{
					ClearMentions(recentMentions, nounPhrase);
					if (!IsPartOfName(nounPhrase))
					{
						IParse head = mHeadFinder.GetLastHead(nounPhrase);
						Mention extent = new Mention(nounPhrase.Span, head.Span, head.EntityId, nounPhrase, null);
						//System.err.println("adding "+np+" with head "+head);
						mentions.Add(extent);
						recentMentions.Add(nounPhrase);
						// determine name-entity type
						string entityType = GetEntityType(mHeadFinder.GetHeadToken(head));
						if (entityType != null)
						{
							extent.NameType = entityType;
						}
					}
					else
					{
						//System.err.println("AbstractMentionFinder.collectMentions excluding np as part of name. np=" + np);
					}
				}
				else
				{
					//System.err.println("AbstractMentionFinder.collectMentions excluding np as head of previous mention. np=" + np);
				}
				if (IsBasalNounPhrase(nounPhrase))
				{
					if (mPrenominalNamedEntitiesCollection)
					{
						CollectPrenominalNamedEntities(nounPhrase, mentions);
					}
					if (mCoordinatedNounPhrasesCollection)
					{
						CollectCoordinatedNounPhraseMentions(nounPhrase, mentions);
					}
					CollectPossessivePronouns(nounPhrase, mentions);
				}
				else
				{
					// Could use to get NP -> tokens CON structures for basal nps including NP -> NAC tokens
					//collectComplexNounPhrases(np,mentions);
				}
			}

            mentions.Sort(); 
			RemoveDuplicates(mentions);
			return mentions.ToArray();
		}
		
		/// <summary> 
        /// Adds a mention for the non-treebank-labeled possesive noun phrases.  
        /// </summary>
		/// <param name="possesiveNounPhrase">
        /// The possessive noun phase which may require an additional mention.
		/// </param>
		/// <param name="mentions">
        /// The list of mentions into which a new mention can be added. 
		/// </param>
        //private void AddPossessiveMentions(IParse possessiveNounPhrase, List<Mention> mentions)
        //{
        //    List<IParse> kids = possessiveNounPhrase.SyntacticChildren;
        //    if (kids.Count > 1)
        //    {
        //        IParse firstToken = kids[1];
        //        if (firstToken.IsToken && firstToken.SyntacticType != "POS")
        //        {
        //            IParse lastToken = kids[kids.Count - 1];
        //            if (lastToken.IsToken)
        //            {
        //                Util.Span extentSpan = new Util.Span(firstToken.Span.Start, lastToken.Span.End);
        //                Mention extent = new Mention(extentSpan, extentSpan, - 1, null, null);
        //                mentions.Add(extent);
        //            }
        //            else
        //            {
        //                System.Console.Error.WriteLine("AbstractMentionFinder.AddPossessiveMentions: odd parse structure: " + possessiveNounPhrase);
        //            }
        //        }
        //    }
        //}
		
		private void CollectPrenominalNamedEntities(IParse nounPhrase, List<Mention> extents)
		{
			IParse headToken = mHeadFinder.GetHeadToken(nounPhrase);
            List<IParse> namedEntities = nounPhrase.NamedEntities;
            Util.Span headTokenSpan = headToken.Span;
			for (int namedEntityIndex = 0; namedEntityIndex < namedEntities.Count; namedEntityIndex++)
			{
				IParse namedEntity = namedEntities[namedEntityIndex];
				if (!namedEntity.Span.Contains(headTokenSpan))
				{
					//System.err.println("adding extent for prenominal ne: "+ne);
					Mention extent = new Mention(namedEntity.Span, namedEntity.Span, namedEntity.EntityId, null, "NAME");
					extent.NameType = namedEntity.EntityType;
					extents.Add(extent);
				}
			}
		}
		
		private static string GetEntityType(IParse headToken)
		{
			string entityType;
			for (IParse parent = headToken.Parent; parent != null; parent = parent.Parent)
			{
				entityType = parent.EntityType;
				if (entityType != null)
				{
					return entityType;
				}
				if (parent.IsSentence)
				{
					break;
				}
			}
			List<IParse> tokenChildren = headToken.Children;
			int tokenChildCount = tokenChildren.Count;
			if (tokenChildCount > 0)
			{
				IParse tokenChild = tokenChildren[tokenChildCount - 1];
				entityType = tokenChild.EntityType;
				if (entityType != null)
				{
					return entityType;
				}
			}
			return null;
		}
		
		private static bool IsPartOfName(IParse nounPhrase)
		{
			string entityType;
			for (IParse parent = nounPhrase.Parent; parent != null; parent = parent.Parent)
			{
				entityType = parent.EntityType;
				//System.err.println("AbstractMentionFinder.isPartOfName: entityType="+entityType);
				if (entityType != null)
				{
					//System.err.println("npSpan = "+np.getSpan()+" parentSpan="+parent.getSpan());
					if (!nounPhrase.Span.Contains(parent.Span))
					{
						return true;
					}
				}
				if (parent.IsSentence)
				{
					break;
				}
			}
			return false;
		}
		
		/// <summary>
        /// Return all noun phrases which are contained by <code>parse</code>.
        /// </summary>
		/// <param name="parse">
        /// The parse in which to find the noun phrases. 
		/// </param>
		/// <returns>
        /// A list of <code>IParse</code> objects which are noun phrases contained by <code>parse</code>.
		/// </returns>
		//protected abstract List getNounPhrases(Parse p);

        public virtual List<IParse> GetNamedEntities(IParse parse)
		{
            return parse.NamedEntities;
		}
		
		public virtual Mention[] GetMentions(IParse parse)
		{
			List<IParse> nounPhrases = parse.NounPhrases;
			nounPhrases.Sort();
            Dictionary<IParse, IParse> headMap = ConstructHeadMap(nounPhrases);
			//System.err.println("AbstractMentionFinder.getMentions: got " + nps.size()); // + " nps, and " + nes.size() + " named entities");
			Mention[] mentions = CollectMentions(nounPhrases, headMap);
			return mentions;
		}
	}
}