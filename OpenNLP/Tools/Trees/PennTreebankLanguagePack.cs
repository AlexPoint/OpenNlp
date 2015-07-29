using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Specifies the treebank/language specific components needed for
    /// parsing the English Penn Treebank.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class PennTreebankLanguagePack : AbstractTreebankLanguagePack
    {

        private static readonly string[] PennPunctTags = { PartsOfSpeech.RightCloseDoubleQuote, PartsOfSpeech.LeftOpenDoubleQuote, "-LRB-", "-RRB-", PartsOfSpeech.SentenceFinalPunctuation, PartsOfSpeech.ColonSemiColon, PartsOfSpeech.Comma };

        private static readonly string[] PennSfPunctTags = { PartsOfSpeech.SentenceFinalPunctuation };

        private static readonly string[] CollinsPunctTags = { PartsOfSpeech.RightCloseDoubleQuote, PartsOfSpeech.LeftOpenDoubleQuote, PartsOfSpeech.SentenceFinalPunctuation, PartsOfSpeech.ColonSemiColon, PartsOfSpeech.Comma };

        private static readonly string[] PennPunctWords =
        {
            PartsOfSpeech.RightCloseDoubleQuote, "'", PartsOfSpeech.LeftOpenDoubleQuote, "`", "-LRB-", "-RRB-", "-LCB-", "-RCB-", PartsOfSpeech.SentenceFinalPunctuation,
            "?", "!", PartsOfSpeech.Comma, PartsOfSpeech.ColonSemiColon, "-", "--", "...", ";"
        };

        private static readonly string[] PennSfPunctWords = { PartsOfSpeech.SentenceFinalPunctuation, "!", "?" };


        /// <summary>
        /// The first 3 are used by the Penn Treebank; # is used by the
        /// BLLIP corpus, and ^ and ~ are used by Klein's lexparser.
        /// Teg added _ (let me know if it hurts).
        /// John Bauer added [ on account of category annotations added when
        /// printing out lexicalized dependencies.  Note that ] ought to be
        /// unnecessary, since it would end the annotation, not start it.
        /// </summary>
        private static readonly char[] AnnotationIntroducingChars = {'-', '=', '|', '#', '^', '~', '_', '['};

        /// <summary>
        /// This is valid for "BobChrisTreeNormalizer" conventions only.
        /// </summary>
        private static readonly string[] PennStartSymbols = { AbstractCollinsHeadFinder.ROOT, AbstractCollinsHeadFinder.TOP };

        /// <summary>
        /// Returns a string array of punctuation tags for this treebank/language.
        /// </summary>
        public override string[] PunctuationTags()
        {
            return PennPunctTags;
        }

        /// <summary>
        /// Returns a string array of punctuation words for this treebank/language.
        /// </summary>
        public override string[] PunctuationWords()
        {
            return PennPunctWords;
        }

        /// <summary>
        /// Returns a string array of sentence readonly punctuation tags for this treebank/language.
        /// </summary>
        public override string[] SentenceFinalPunctuationTags()
        {
            return PennSfPunctTags;
        }

        /// <summary>
        /// Returns a string array of sentence readonly punctuation words for this treebank/language.
        /// </summary>
        public override string[] SentenceFinalPunctuationWords()
        {
            return PennSfPunctWords;
        }

        /// <summary>
        /// Returns a string array of punctuation tags that EVALB-style evaluation
        /// should ignore for this treebank/language.
        /// Traditionally, EVALB has ignored a subset of the total set of
        /// punctuation tags in the English Penn Treebank (quotes and
        /// period, comma, colon, etc., but not brackets)
        /// </summary>
        /// <returns>Whether this is a EVALB-ignored punctuation tag</returns>
        public override string[] EvalBIgnoredPunctuationTags()
        {
            return CollinsPunctTags;
        }

        /// <summary>
        /// Return an array of characters at which a string should be
        /// truncated to give the basic syntactic category of a label.
        /// The idea here is that Penn treebank style labels follow a syntactic
        /// category with various functional and crossreferencing information
        /// introduced by special characters (such as "NP-SBJ=1").  This would
        /// be truncated to "NP" by the array containing '-' and "=".
        /// </summary>
        /// <returns>An array of characters that set off label name suffixes</returns>
        public override char[] LabelAnnotationIntroducingCharacters()
        {
            return AnnotationIntroducingChars;
        }

        /// <summary>
        /// Returns a string array of treebank start symbols.
        /// </summary>
        public override string[] StartSymbols()
        {
            return PennStartSymbols;
        }

        /**
       * Returns a factory for {@link PTBTokenizer}.
       *
       * @return A tokenizer
       */
        /*public TokenizerFactory<CoreLabel> getTokenizerFactory() {
            return PTBTokenizer.coreLabelFactory();
          }*/

        public override ITokenizerFactory<IHasWord> GetTokenizerFactory()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the extension of treebank files for this treebank.
        /// This is "mrg".
        /// </summary>
        public override string TreebankFileExtension()
        {
            return "mrg";
        }

        /// <summary>
        /// Return a GrammaticalStructure suitable for this language/treebank.
        /// </summary>
        public override IGrammaticalStructureFactory GrammaticalStructureFactory()
        {
            return new EnglishGrammaticalStructureFactory();
        }

        /**
        * Return a GrammaticalStructure suitable for this language/treebank.
        * <i>Note:</i> This is loaded by reflection so basic treebank use does not require all the Stanford Dependencies code.
        *
        * @return A GrammaticalStructure suitable for this language/treebank.
        */
        /*public GrammaticalStructureFactory grammaticalStructureFactory(Predicate<string> puncFilter) {
            return new EnglishGrammaticalStructureFactory(puncFilter);
          }*/

        /*public GrammaticalStructureFactory grammaticalStructureFactory(Predicate<string> puncFilter, HeadFinder hf) {
            return new EnglishGrammaticalStructureFactory(puncFilter, hf);
          }*/

        public override bool SupportsGrammaticalStructures()
        {
            return true;
        }

        public override IHeadFinder HeadFinder()
        {
            return new ModCollinsHeadFinder(this);
        }

        public override IHeadFinder TypedDependencyHeadFinder()
        {
            return new SemanticHeadFinder(this, true);
        }
        
    }
}