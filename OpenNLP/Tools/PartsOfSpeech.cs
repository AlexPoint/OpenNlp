using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools
{
    /// <summary>
    /// The comprehensive list of all the parts of speech.
    /// </summary>
    public static class PartsOfSpeech
    {
        // List of all parts of speech ---------------------------------------

        // verbs
        public const string VerbBaseForm = "VB";
        public const string VerbNon3rdPersSingPresent = "VBP";
        public const string Verb3rdPersSingPresent = "VBZ";
        public const string VerbPastTense = "VBD";
        public const string VerbGerundOrPresentParticiple = "VBG";
        public const string VerbPastParticiple = "VBN";
        // adjectives
        public const string Adjective = "JJ";
        public const string AdjectiveComparative = "JJR";
        public const string AdjectiveSuperlative = "JJS";
        // nouns
        public const string NounSingularOrMass = "NN";
        public const string NounPlural = "NNS";
        public const string ProperNounSingular = "NNP";
        public const string ProperNounPlural = "NNPS";
        // adverbs
        public const string WhAdverb = "WRB";
        public const string Adverb = "RB";
        public const string AdverbComparative = "RBR";
        public const string AdverbSuperlative = "RBS";
        // conjunctions
        public const string CoordinatingConjunction = "CC";
        public const string PrepositionOrSubordinateConjunction = "IN";
        // pronouns
        public const string WhPronoun = "WP";
        public const string PossessiveWhPronoun = "WP$";
        public const string PersonalPronoun = "PRP";
        public const string PossessivePronoun = "PRP$";
        // misc
        public const string Particle = "RP";
        public const string CardinalNumber = "CD";
        public const string Determiner = "DT";
        public const string To = "TO";
        public const string ExistentialThere = "EX";
        public const string Interjection = "UH";
        public const string ForeignWord = "FW";
        public const string ListItemMarker = "LS";
        public const string Modal = "MD";
        public const string WhDeterminer = "WDT";
        public const string Predeterminer = "PDT";
        // punctuation
        public const string LeftOpenDoubleQuote = "``";
        public const string PossessiveEnding = "POS";
        public const string Comma = ",";
        public const string RightCloseDoubleQuote = "''";
        public const string SentenceFinalPunctuation = ".";
        public const string ColonSemiColon = ":";
        public const string LeftParenthesis = "-LRB";
        public const string RightParenthesis = "-RRB";
        // symbols
        public const string DollarSign = "$";
        public const string PoundSign = "#";
        public const string Symbol = "SYM";


        // Utilities -------------------------------------------------------

        /// <summary>
        /// Returns true if the pos corresponds to a verb
        /// (base form, present form non 3rd, present form 3rd, past form,
        /// present particle/gerundive or past participle).
        /// Return false otherwise.
        /// </summary>
        public static bool IsVerb(string pos)
        {
            return !string.IsNullOrEmpty(pos) && pos.StartsWith("VB");
        }

        /// <summary>
        /// Returns true if the pos corresponds to a noun (plural, singular or proper).
        /// Returns false otherwise.
        /// </summary>
        public static bool IsNoun(string pos)
        {
            return !string.IsNullOrEmpty(pos) && pos.StartsWith("NN");
        }

        /// <summary>
        /// Returns true if the pos corresponds to a proper noun (plural or singular).
        /// Returns false otherwise.
        /// </summary>
        public static bool IsProperNoun(string pos)
        {
            return !string.IsNullOrEmpty(pos) && pos.StartsWith("NNP");
        }

        /// <summary>
        /// Returns true if the pos corresponds to an adjective
        /// (regular, comparative or superlative).
        /// Returns false otherwise.
        /// </summary>
        public static bool IsAdjective(string pos)
        {
            return !string.IsNullOrEmpty(pos) && pos.StartsWith("JJ");
        }

        /// <summary>
        /// Returns true if the pos corresponds to a personnal or possessive pronoun.
        /// Returns false otherwise.
        /// </summary>
        public static bool IsPersOrPossPronoun(string tag)
        {
            return !string.IsNullOrEmpty(tag) && tag.StartsWith("PRP");
        }

        /// <summary>
        /// Writes the part of speech to be human understandable
        /// </summary>
        public static string Write(string pos)
        {
            var fields = typeof(PartsOfSpeech).GetFields();
            foreach (var fieldInfo in fields)
            {
                var value = (string)fieldInfo.GetValue(null);
                if (value == pos)
                {
                    return fieldInfo.Name;
                }
            }
            return string.Format("Unsupported function abbreviation ({0})", pos);
        }

        /// <summary>
        /// Checks if a string corresponds to a supported part of speech
        /// </summary>
        public static bool IsSupportedPartOfSpeech(string function)
        {
            var fields = typeof(PartsOfSpeech).GetFields();
            return fields
                .Select(fieldInfo => (string) fieldInfo.GetValue(null))
                .Any(value => value == function);
        }
    }
}
