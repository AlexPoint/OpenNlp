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

//This file is based on the Linker.java source file found in the
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
//UPGRADE_TODO: The type 'java.util.regex.Pattern' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using Pattern = java.util.regex.Pattern;
using System.Text.RegularExpressions;
using HeadFinder = OpenNLP.Tools.Coreference.Mention.IHeadFinder;
using MentionContext = OpenNLP.Tools.Coreference.Mention.MentionContext;
using MentionFinder = OpenNLP.Tools.Coreference.Mention.IMentionFinder;
namespace OpenNLP.Tools.Coreference
{
	
	public class Linker
    {
		/// <summary>
        /// string constant used to label a mention which is a description.
        /// </summary>
		public const string Descriptor = "desc";
		/// <summary>
        /// string constant used to label an mention in an appositive relationship. 
        /// </summary>
        public const string IsA = "isa";
		/// <summary>
        /// string constant used to label a mention which consists of two or more noun phrases. 
        /// </summary>
        public const string CombinedNounPhrases = "cmbnd";
		/// <summary>
        /// string constant used to label a mention which consists of a single noun phrase. 
        /// </summary>
        public const string SingleNounPhrase = "np";
		/// <summary>
        /// string constant used to label a mention which is a proper noun modifing another noun. 
        /// </summary>
        public const string ProperNounModifier = "pnmod";
		/// <summary>
        /// string constatant used to label a mention which is a pronoun.
        /// </summary>
        public const string PronounModifier = "np";
		/// <summary>
        /// Regular expression for English singular third person pronouns.
        /// </summary>
		public readonly static Regex SingularThirdPersonPronounPattern = new Regex("^(he|she|it|him|her|his|hers|its|himself|herself|itself)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English plural third person pronouns. 
        /// </summary>
        public readonly static Regex PluralThirdPersonPronounPattern = new Regex("^(they|their|theirs|them|themselves)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English speech pronouns.
        /// </summary>
        public readonly static Regex SpeechPronounPattern = new Regex("^(I|me|my|you|your|you|we|us|our|ours)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English male pronouns. 
        /// </summary>
        public readonly static Regex MalePronounPattern = new Regex("^(he|him|his|himself)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English female pronouns. 
        /// </summary>
        public readonly static Regex FemalePronounPattern = new Regex("^(she|her|hers|herself)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English nueter pronouns. 
        /// </summary>
        public readonly static Regex NeuterPronounPattern = new Regex("^(it|its|itself)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English first person pronouns. 
        /// </summary>
        public readonly static Regex FirstPersonPronounPattern = new Regex("^(I|me|my|we|our|us|ours)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English singular second person pronouns.
        /// </summary>
        public readonly static Regex SecondPersonPronounPattern = new Regex("^(you|your|yours)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English third person pronouns.
        /// </summary>
        public readonly static Regex ThirdPersonPronounPattern = new Regex("^(he|she|it|him|her|his|hers|its|himself|herself|itself|they|their|theirs|them|themselves)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English singular pronouns. 
        /// </summary>
        public readonly static Regex SingularPronounPattern = new Regex("^(I|me|my|he|she|it|him|her|his|hers|its|himself|herself|itself)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English plural pronouns.
        /// </summary>
        public readonly static Regex PluralPronounPattern = new Regex("^(we|us|our|ours|they|their|theirs|them|themselves)$", RegexOptions.IgnoreCase);
		/// <summary>
        /// Regular expression for English honorifics. 
        /// </summary>
        public readonly static Regex HonorificsPattern = new Regex("[A-Z][a-z]+\\.$|^[A-Z][b-df-hj-np-tv-xz]+$");
		/// <summary>
        /// Regular expression for English corporate designators. 
        /// </summary>
        public readonly static Regex DesignatorsPattern = new Regex("[a-z]\\.$|^[A-Z][b-df-hj-np-tv-xz]+$|^Co(rp)?$");
	}

    /// <summary>
    /// A linker provides an interface for finding mentions, {@link #MentionFinder MentionFinder}, 
    /// and creating entities out of those mentions, {@link #GetEntitiesFromMentions getEntitiesFromMentions}.  This interface also allows
    /// for the training of a resolver with the method {@link #SetEntititesFromMentions setEntititesFromMentions} which is used to give the
    /// resolver mentions whose entityId fields indicate which mentions refer to the same entity and the 
    /// {@link #Train Train} method which compiles all the information provided via calls to 
    /// {@link #SetEntititesFromMentions SetEntititesFromMentions} into a model.
    /// </summary>
	public interface ILinker
	{
		/// <summary>
        /// The mention finder for this linker.  This can be used to get the mentions of a Parse.
        /// </summary>
		MentionFinder MentionFinder{ get; }

		/// <summary>
        /// The head finder associated with this linker.
        /// </summary>
		HeadFinder HeadFinder { get; }
		
		/// <summary>
        /// Indicated that the specified mentions can be used to train this linker.
		/// This requires that the coreference relationship between the mentions have been labeled
		/// in the mention's id field.
		/// </summary>
		/// <param name="mentions">
        /// The mentions to be used to train the linker.
		/// </param>
        void SetEntitiesFromMentions(Mention.Mention[] mentions);
		
		/// <summary>
        /// Returns a list of entities which group the mentions into entity classes.
        /// </summary>
		/// <param name="mentions">A array of mentions. 
		/// </param>
        /// <returns>
        /// An array of discourse entities.
        /// </returns>
        DiscourseEntity[] GetEntitiesFromMentions(Mention.Mention[] mentions);
		
		/// <summary>
        /// Creates mention contexts for the specified mention exents.  These are used to compute coreference features over.
        /// </summary>
		/// <param name="mentions">
        /// The mention of a document.
		/// </param>
		/// <returns>
        /// mention contexts for the specified mention exents.
		/// </returns>
        MentionContext[] ConstructMentionContexts(Mention.Mention[] mentions);
		
		/// <summary>
        /// Trains the linker based on the data specified via calls to {@link #SetEntities SetEntities}.
        /// </summary>
		void Train();
	}
}