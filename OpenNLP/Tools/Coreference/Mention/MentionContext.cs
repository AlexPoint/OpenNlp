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

//This file is based on the MentionContext.java source file found in the
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
    /// Data structure representation of a mention with additional contextual information.  The contextual
	/// information is used in performing coreference resolution.
	/// </summary>
    public class MentionContext : Similarity.Context
	{
		/// <summary>
        /// Returns the parse of the head token for this mention.
        /// </summary>
		/// <returns> 
        /// the parse of the head token for this mention.
		/// </returns>
		public virtual IParse HeadTokenParse
		{
			get
			{
				return mHeadToken;
			}
		}

		public virtual string HeadText
		{
			get
			{
				System.Text.StringBuilder headText = new System.Text.StringBuilder();
				for (int tokenIndex = 0; tokenIndex < Tokens.Length; tokenIndex++)
				{
					headText.Append(" ").Append(Tokens[tokenIndex].ToString());
				}
				return headText.ToString().Substring(1);
			}
		}

		public virtual IParse Head
		{
			get
			{
				return mHead;
			}
		}

		public virtual int NonDescriptorStart
		{
			get
			{
				return this.mNonDescriptorStart;
			}
		}

		/// <summary>
        /// Returns a sentence-based token span for this mention.  If this mention consist
		/// of the third, fourth, and fifth token, then this span will be 2..4.   
		/// </summary>
		/// <returns>
        /// a sentence-based token span for this mention.
		/// </returns>
        public virtual Util.Span IndexSpan
		{
			get
			{
				return mIndexSpan;
			}
		}

		/// <summary>
        /// Returns the index of the noun phrase for this mention in a sentence.
        /// </summary>
		/// <returns> 
        /// the index of the noun phrase for this mention in a sentence.
		/// </returns>
		public virtual int NounPhraseSentenceIndex
		{
			get
			{
				return mNounLocation;
			}
		}

		/// <summary> 
        /// Returns the index of the noun phrase for this mention in a document.
        /// </summary>
		/// <returns> 
        /// the index of the noun phrase for this mention in a document.
		/// </returns>
		public virtual int NounPhraseDocumentIndex
		{
			get
			{
				return mNounNumber;
			}
		}

		/// <summary> 
        /// Returns the index of the last noun phrase in the sentence containing this mention.
		/// This is one less than the number of noun phrases in the sentence which contains this mention. 
		/// </summary>
		/// <returns> 
        /// the index of the last noun phrase in the sentence containing this mention.
		/// </returns>
		public virtual int MaxNounPhraseSentenceIndex
		{
			get
			{
				return mMaxNounLocation;
			}
		}

		public virtual IParse NextTokenBasal
		{
			get
			{
				return mBasalNextToken;
			}
		}

		public virtual IParse PreviousToken
		{
			get
			{
				return mPreviousToken;
			}
		}

		public virtual IParse NextToken
		{
			get
			{
				return mNextToken;
			}
		}

		/// <summary>
        /// Returns the index of the sentence which contains this mention.
        /// </summary>
		/// <returns>
        /// the index of the sentence which contains this mention.
		/// </returns>
		public virtual int SentenceNumber
		{
			get
			{
				return mSentenceNumber;
			}
		}

		/// <summary>
        /// Returns the parse for the first token in this mention.
        /// </summary>
		/// <returns> 
        /// The parse for the first token in this mention.
		/// </returns>
		public virtual IParse FirstToken
		{
			get
			{
				return mFirstToken;
			}
		}

		/// <summary>
        /// Returns the text for the first token of the mention.
        /// </summary>
		/// <returns> 
        /// The text for the first token of the mention.
		/// </returns>
		public virtual string FirstTokenText
		{
			get
			{
				return mFirstTokenText;
			}
		}

		/// <summary> 
        /// Returns the pos-tag of the first token of this mention. 
        /// </summary>
		/// <returns>
        /// the pos-tag of the first token of this mention.
		/// </returns>
		public virtual string FirstTokenTag
		{
			get
			{
				return mFirstTokenTag;
			}
		}

		/// <summary>
        /// Returns the parses for the tokens which are contained in this mention.
        /// </summary>
		/// <returns> 
        /// An array of parses, in order, for each token contained in this mention.
		/// </returns>
		public virtual IParse[] TokenParses
		{
			get
			{
				return (IParse[]) Tokens;
			}
		}

		/// <summary>
        /// Returns the probability associated with the gender assignment.
        /// </summary>
		/// <returns> 
        /// The probability associated with the gender assignment.
		/// </returns>
		public virtual double GenderProbability
		{
			get
			{
				return mGenderProbability;
			}
		}

		/// <summary>
        /// Returns the probability associated with the number assignment.
        /// </summary>
		/// <returns>
        /// The probability associated with the number assignment.
		/// </returns>
		public virtual double NumberProbability
		{
			get
			{
				return mNumberProbability;
			}
		}

		/// <summary>
        /// The index of first token which is not part of a descriptor.  This is 0 if no descriptor is present. 
        /// </summary>
		private int mNonDescriptorStart;

		/// <summary>
        /// The Parse of the head constituent of this mention.
        /// </summary>
		private IParse mHead;

		/// <summary>
        /// Sentence-token-based span whose end is the last token of the mention.
        /// </summary>
        private Util.Span mIndexSpan;

		/// <summary>
        /// Position of the NP in the sentence. 
        /// </summary>
		private int mNounLocation;

		/// <summary>
        /// Position of the NP in the document. 
        /// </summary>
		private int mNounNumber;

		/// <summary>
        /// Number of noun phrases in the sentence which contains this mention.
        /// </summary>
		private int mMaxNounLocation;

		/// <summary>
        /// Index of the sentence in the document which contains this mention.
        /// </summary>
		private int mSentenceNumber;

		/// <summary>
        /// The token preceeding this mention's maximal noun phrase.
        /// </summary>
		private IParse mPreviousToken;

		/// <summary>
        /// The token following this mention's maximal noun phrase.
        /// </summary>
		private IParse mNextToken;

		/// <summary>
        /// The token following this mention's basal noun phrase.
        /// </summary>
		private IParse mBasalNextToken;
		
		/// <summary>
        /// The parse of the mention's head word. 
        /// </summary>
		private IParse mHeadToken;

		/// <summary>
        /// The parse of the first word in the mention. 
        /// </summary>
		private IParse mFirstToken;

		/// <summary>
        /// The text of the first word in the mention. 
        /// </summary>
		private string mFirstTokenText;

		/// <summary>
        /// The pos-tag of the first word in the mention. 
        /// </summary>
		private string mFirstTokenTag;

		/// <summary>
        /// The gender assigned to this mention. 
        /// </summary>
        private Similarity.GenderEnum mGender;

		/// <summary>
        /// The probability associated with the gender assignment.
        /// </summary>
		private double mGenderProbability;

		/// <summary>
        /// The number assigned to this mention. 
        /// </summary>
        private Similarity.NumberEnum mNumber;

		/// <summary>
        /// The probability associated with the number assignment. 
        /// </summary>
		private double mNumberProbability;


        public MentionContext(Util.Span span, Util.Span headSpan, int entityId, IParse parse, string extentType, string nameType, int mentionIndex, int mentionsInSentence, int mentionIndexInDocument, int sentenceIndex, IHeadFinder headFinder) : base(span, headSpan, entityId, parse, extentType, nameType, headFinder)
		{
			mNounLocation = mentionIndex;
			mMaxNounLocation = mentionsInSentence;
			mNounNumber = mentionIndexInDocument;
			mSentenceNumber = sentenceIndex;
			mIndexSpan = parse.Span;
			mPreviousToken = parse.PreviousToken;
			mNextToken = parse.NextToken;
			mHead = headFinder.GetLastHead(parse);
			List<IParse> headTokens = mHead.Tokens;
            Tokens = headTokens.ToArray();
			mBasalNextToken = mHead.NextToken;
			//System.err.println("MentionContext.init: "+ent+" "+ent.getEntityId()+" head="+head);
			//mNonDescriptorStart = 0;
			InitializeHeads(headFinder.GetHeadIndex(mHead));
            mGender = Similarity.GenderEnum.Unknown;
			mGenderProbability = 0d;
            mNumber = Similarity.NumberEnum.Unknown;
			mNumberProbability = 0d;
		}
		/// <summary> 
        /// Constructs context information for the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The mention object on which this object is based.
		/// </param>
		/// <param name="mentionIndexInSentence">
        /// The mention's position in the sentence.
		/// </param>
		/// <param name="mentionsInSentence">
        /// The number of mentions in the sentence.
		/// </param>
		/// <param name="mentionIndexInDocument">
        /// The index of this mention with respect to the document.
		/// </param>
		/// <param name="sentenceIndex">
        /// The index of the sentence which contains this mention.
		/// </param>
		/// <param name="headFinder">
        /// An object which provides head information.
		/// </param>
		public MentionContext(Mention mention, int mentionIndexInSentence, int mentionsInSentence, int mentionIndexInDocument, int sentenceIndex, IHeadFinder headFinder):this(mention.Span, mention.HeadSpan, mention.Id, mention.Parse, mention.Type, mention.NameType, mentionIndexInSentence, mentionsInSentence, mentionIndexInDocument, sentenceIndex, headFinder)
		{
		}
		
		/// <summary>
        /// Constructs context information for the specified mention.
        /// </summary>
		/// <param name="mentionParse">
        /// Mention parse structure for which context is to be constructed.
		/// </param>
		/// <param name="mentionIndex">
        /// mention position in sentence.
		/// </param>
		/// <param name="mentionsInSentence">
        /// Number of mentions in the sentence.
		/// </param>
		/// <param name="mentionsInDocument">
        /// Number of mentions in the document.
		/// </param>
		/// <param name="sentenceIndex">
        /// Sentence number for this mention.
		/// </param>
		/// <param name="nameType">
        /// The named-entity type for this mention.
		/// </param>
		/// <param name="headFinder">
        /// Object which provides head information.
		/// </param>
		/*
		public MentionContext(Parse mentionParse, int mentionIndex, int mentionsInSentence, int mentionsInDocument, int sentenceIndex, String nameType, HeadFinder headFinder) {
		nounLocation = mentionIndex;
		maxNounLocation = mentionsInDocument;
		sentenceNumber = sentenceIndex;
		parse = mentionParse;
		indexSpan = mentionParse.getSpan();
		prevToken = mentionParse.getPreviousToken();
		nextToken = mentionParse.getNextToken();
		head = headFinder.getLastHead(mentionParse);
		List headTokens = head.getTokens();
		tokens = (Parse[]) headTokens.toArray(new Parse[headTokens.size()]);
		basalNextToken = head.getNextToken();
		//System.err.println("MentionContext.init: "+ent+" "+ent.getEntityId()+" head="+head);
		indexHeadSpan = head.getSpan();
		nonDescriptorStart = 0;
		initHeads(headFinder.getHeadIndex(head));
		this.neType= nameType;
		if (getHeadTokenTag().startsWith("NN") && !getHeadTokenTag().startsWith("NNP")) {
		//if (headTokenTag.startsWith("NNP") && neType != null) {
		this.synsets = getSynsetSet(this);
		}
		else {
		this.synsets=Collections.EMPTY_SET;
		}
		gender = GenderEnum.UNKNOWN;
		this.genderProb = 0d;
		number = NumberEnum.UNKNOWN;
		this.numberProb = 0d;
		}
		*/
		
		private void  InitializeHeads(int headIndex)
		{
			HeadTokenIndex = headIndex;
			mHeadToken = (IParse) Tokens[HeadTokenIndex];
			HeadTokenText = mHeadToken.ToString();
			HeadTokenTag = mHeadToken.SyntacticType;
			mFirstToken = (IParse) Tokens[0];
			mFirstTokenTag = mFirstToken.SyntacticType;
			mFirstTokenText = mFirstToken.ToString();
		}
		
		/// <summary> 
        /// Returns the text of this mention. 
        /// </summary>
		/// <returns>
        /// A space-delimited string of the tokens of this mention.
		/// </returns>
		public virtual string ToText()
		{
			return Parse.ToString();
		}
		
		/*
		private static String[] getLemmas(MentionContext xec) {
		//TODO: Try multi-word lemmas first.
		String word = xec.getHeadTokenText();
		return DictionaryFactory.getDictionary().getLemmas(word,"NN");
		}
		
		private static Set getSynsetSet(MentionContext xec) {
		//System.err.println("getting synsets for mention:"+xec.toText());
		Set synsetSet = new HashSet();
		String[] lemmas = getLemmas(xec);
		for (int li = 0; li < lemmas.length; li++) {
		String[] synsets = DictionaryFactory.getDictionary().getParentSenseKeys(lemmas[li],"NN",0);
		for (int si=0,sn=synsets.length;si<sn;si++) {
		synsetSet.add(synsets[si]);
		}
		}
		return (synsetSet);
		}
		*/
		
		/// <summary>
        /// Assigns the specified gender with the specified probability to this mention.
        /// </summary>
		/// <param name="gender">
        /// The gender to be given to this mention.
		/// </param>
		/// <param name="probability">
        /// The probability assosicated with the gender assignment.
		/// </param>
        public virtual void SetGender(Similarity.GenderEnum gender, double probability)
		{
			mGender = gender;
			mGenderProbability = probability;
		}
		
		/// <summary>
        /// Returns the gender of this mention.</summary>
		/// <returns> 
        /// The gender of this mention.
		/// </returns>
        public virtual Similarity.GenderEnum GetGender()
		{
			return mGender;
		}
		
		/// <summary> 
        /// Assigns the specified number with the specified probability to this mention.
        /// </summary>
		/// <param name="number">
        /// The number to be given to this mention.
		/// </param>
		/// <param name="probability">
        /// The probability assosicated with the number assignment.
		/// </param>
        public virtual void SetNumber(Similarity.NumberEnum number, double probability)
		{
			mNumber = number;
			mNumberProbability = probability;
		}
		
		/// <summary> Returns the number of this mention.</summary>
		/// <returns> The number of this mention.
		/// </returns>
        public virtual Similarity.NumberEnum GetNumber()
		{
			return mNumber;
		}
	}
}