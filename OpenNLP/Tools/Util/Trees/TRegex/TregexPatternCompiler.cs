using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Util.Trees.TRegex
{
    public class TregexPatternCompiler
    {
        public static readonly AbstractTreebankLanguagePack.BasicCategoryStringFunction DEFAULT_BASIC_CAT_FUNCTION =
            new PennTreebankLanguagePack().GetBasicCategoryFunction();

        public static readonly HeadFinder DEFAULT_HEAD_FINDER = new CollinsHeadFinder();

        private readonly AbstractTreebankLanguagePack.BasicCategoryStringFunction basicCatFunction;
        private readonly HeadFinder headFinder;

        private readonly List<Tuple<string, string>> macros =
            new List<Tuple<string, string>>();

        public static readonly TregexPatternCompiler defaultCompiler =
            new TregexPatternCompiler();

        public TregexPatternCompiler() :
            this(DEFAULT_HEAD_FINDER, DEFAULT_BASIC_CAT_FUNCTION)
        {
        }

        /**
   * A compiler that uses this basicCatFunction and the default HeadFinder.
   *
   * @param basicCatFunction the function mapping strings to Strings
   */

        public TregexPatternCompiler(AbstractTreebankLanguagePack.BasicCategoryStringFunction basicCatFunction) :
            this(DEFAULT_HEAD_FINDER, basicCatFunction)
        {
        }

        /**
   * A compiler that uses this HeadFinder and the default basicCategoryFunction
   *
   * @param headFinder the HeadFinder
   */

        public TregexPatternCompiler(HeadFinder headFinder) :
            this(headFinder, DEFAULT_BASIC_CAT_FUNCTION)
        {
        }

        /**
   * A compiler that uses this HeadFinder and this basicCategoryFunction
   *
   * @param headFinder       the HeadFinder
   * @param basicCatFunction The function mapping strings to Strings
   */

        public TregexPatternCompiler(HeadFinder headFinder,
            AbstractTreebankLanguagePack.BasicCategoryStringFunction basicCatFunction)
        {
            this.headFinder = headFinder;
            this.basicCatFunction = basicCatFunction;
        }

        // todo [cdm 2013]: Provide an easy way to do Matcher.quoteReplacement(): This would be quite useful, since the replacement will often contain $ or \

        /** Define a macro for rewriting a pattern in any tregex expression compiled
   *  by this compiler. The semantics of this is that all instances of the
   *  original in the pattern are replaced by the replacement, using exactly
   *  the semantics of string.replaceAll(original, replacement) and the
   *  result will then be compiled by the compiler. As such, note that a
   *  macro can replace any part of a tregex expression, in a syntax
   *  insensitive way.  Here's an example:
   *  {@code tpc.addMacro("FINITE_BE_AUX", "/^(?i:am|is|are|was|were)$/");}
   *
   *  @param original The string to match; becomes the first argument of a
   *                  string.replaceAll()
   *  @param replacement The replacement String; becomes the second argument
   *                  of a string.replaceAll()
   */

        public void AddMacro(string original, string replacement)
        {
            macros.Add(new Tuple<string, string>(original, replacement));
        }


        /**
   * Create a TregexPattern from this tregex string using the headFinder and
   * basicCat function this TregexPatternCompiler was created with.
   *
   * <i>Implementation note:</i> If there is an invalid token in the Tregex
   * parser, JavaCC will throw a TokenMgrError.  This is a class
   * that extends Error, not Exception (OMG! - bad!), and so rather than
   * requiring clients to catch it, we wrap it in a ParseException.
   * (The original Error's are thrown in TregexParserTokenManager.)
   *
   * @param tregex The pattern to parse
   * @return A new TregexPattern object based on this string
   * @throws TregexParseException If the expression is syntactically invalid
   */

        public TregexPattern Compile(string tregex)
        {
            foreach (Tuple<string, string> macro in macros)
            {
                //tregex = tregex.replaceAll(macro.first(), macro.second());
                tregex = Regex.Replace(tregex, macro.Item1, macro.Item2);
            }
            TregexPattern pattern;
            try
            {
                var parser = new TregexParser(new StringReader(tregex + '\n'),
                    basicCatFunction.Apply, headFinder);
                pattern = parser.Root();
            }
            catch (TokenMgrException tme)
            {
                throw new TregexParseException("Could not parse " + tregex /*, tme*/);
            }
            catch (ParseException e)
            {
                throw new TregexParseException("Could not parse " + tregex /*, e*/);
            }
            pattern.SetPatternString(tregex);
            return pattern;
        }
    }
}