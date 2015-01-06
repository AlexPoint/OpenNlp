using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Process;

namespace OpenNLP.Tools.Util.Trees
{
    public class PennTreebankLanguagePack : AbstractTreebankLanguagePack
    {
        /**
   * Gives a handle to the TreebankLanguagePack
   */

        public PennTreebankLanguagePack()
        {
        }


        private static readonly string[] pennPunctTags = {"''", "``", "-LRB-", "-RRB-", ".", ":", ","};

        private static readonly string[] pennSFPunctTags = {"."};

        private static readonly string[] collinsPunctTags = {"''", "``", ".", ":", ","};

        private static readonly string[] pennPunctWords =
        {
            "''", "'", "``", "`", "-LRB-", "-RRB-", "-LCB-", "-RCB-", ".",
            "?", "!", ",", ":", "-", "--", "...", ";"
        };

        private static readonly string[] pennSFPunctWords = {".", "!", "?"};


        /**
   * The first 3 are used by the Penn Treebank; # is used by the
   * BLLIP corpus, and ^ and ~ are used by Klein's lexparser.
   * Teg added _ (let me know if it hurts).
   * John Bauer added [ on account of category annotations added when
   * printing out lexicalized dependencies.  Note that ] ought to be
   * unnecessary, since it would end the annotation, not start it.
   */
        private static readonly char[] annotationIntroducingChars = {'-', '=', '|', '#', '^', '~', '_', '['};

        /**
   * This is valid for "BobChrisTreeNormalizer" conventions only.
   */
        private static readonly string[] pennStartSymbols = {"ROOT", "TOP"};


        /**
   * Returns a string array of punctuation tags for this treebank/language.
   *
   * @return The punctuation tags
   */
        //@Override
        public override string[] punctuationTags()
        {
            return pennPunctTags;
        }


        /**
   * Returns a string array of punctuation words for this treebank/language.
   *
   * @return The punctuation words
   */
        //@Override
        public override string[] punctuationWords()
        {
            return pennPunctWords;
        }


        /**
   * Returns a string array of sentence readonly punctuation tags for this
   * treebank/language.
   *
   * @return The sentence readonly punctuation tags
   */
        //@Override
        public override string[] sentenceFinalPunctuationTags()
        {
            return pennSFPunctTags;
        }

        /**
   * Returns a string array of sentence readonly punctuation words for this
   * treebank/language.
   *
   * @return The sentence readonly punctuation tags
   */
        //@Override
        public override string[] sentenceFinalPunctuationWords()
        {
            return pennSFPunctWords;
        }

        /**
   * Returns a string array of punctuation tags that EVALB-style evaluation
   * should ignore for this treebank/language.
   * Traditionally, EVALB has ignored a subset of the total set of
   * punctuation tags in the English Penn Treebank (quotes and
   * period, comma, colon, etc., but not brackets)
   *
   * @return Whether this is a EVALB-ignored punctuation tag
   */
        //@Override
        public override string[] evalBIgnoredPunctuationTags()
        {
            return collinsPunctTags;
        }


        /**
   * Return an array of characters at which a string should be
   * truncated to give the basic syntactic category of a label.
   * The idea here is that Penn treebank style labels follow a syntactic
   * category with various functional and crossreferencing information
   * introduced by special characters (such as "NP-SBJ=1").  This would
   * be truncated to "NP" by the array containing '-' and "=".
   *
   * @return An array of characters that set off label name suffixes
   */
        //@Override
        public override char[] labelAnnotationIntroducingCharacters()
        {
            return annotationIntroducingChars;
        }


        /**
   * Returns a string array of treebank start symbols.
   *
   * @return The start symbols
   */
        //@Override
        public override string[] startSymbols()
        {
            return pennStartSymbols;
        }

        /**
   * Returns a factory for {@link PTBTokenizer}.
   *
   * @return A tokenizer
   */
        //@Override
        /*public TokenizerFactory<CoreLabel> getTokenizerFactory() {
    return PTBTokenizer.coreLabelFactory();
  }*/

        public override TokenizerFactory<HasWord> getTokenizerFactory()
        {
            throw new NotImplementedException();
        }

        /**
   * Returns the extension of treebank files for this treebank.
   * This is "mrg".
   */
        //@Override
        public override string treebankFileExtension()
        {
            return "mrg";
        }

        /**
   * Return a GrammaticalStructure suitable for this language/treebank.
   *
   * @return A GrammaticalStructure suitable for this language/treebank.
   */
        //@Override
        public override GrammaticalStructureFactory grammaticalStructureFactory()
        {
            return new EnglishGrammaticalStructureFactory();
        }

        /**
   * Return a GrammaticalStructure suitable for this language/treebank.
   * <p>
   * <i>Note:</i> This is loaded by reflection so basic treebank use does not require all the Stanford Dependencies code.
   *
   * @return A GrammaticalStructure suitable for this language/treebank.
   */
        //@Override
        /*public GrammaticalStructureFactory grammaticalStructureFactory(Predicate<string> puncFilter) {
    return new EnglishGrammaticalStructureFactory(puncFilter);
  }*/

        //@Override
        /*public GrammaticalStructureFactory grammaticalStructureFactory(Predicate<string> puncFilter, HeadFinder hf) {
    return new EnglishGrammaticalStructureFactory(puncFilter, hf);
  }*/

        //@Override
        public override bool supportsGrammaticalStructures()
        {
            return true;
        }

        /** {@inheritDoc} */
        //@Override
        public override HeadFinder headFinder()
        {
            return new ModCollinsHeadFinder(this);
        }

        /** {@inheritDoc} */
        //@Override
        public override HeadFinder typedDependencyHeadFinder()
        {
            return new SemanticHeadFinder(this, true);
        }

        
        private static readonly long serialVersionUID = 9081305982861675328L;
    }
}