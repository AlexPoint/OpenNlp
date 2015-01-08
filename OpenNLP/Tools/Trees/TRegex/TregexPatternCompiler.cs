using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Trees.TRegex
{
    /// <summary>
    /// A class for compiling TregexPatterns with specific HeadFinders and or basicCategoryFunctions.
    /// 
    /// @author Galen Andrew
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TregexPatternCompiler
    {
        public static readonly AbstractTreebankLanguagePack.BasicCategoryStringFunction DEFAULT_BASIC_CAT_FUNCTION =
            new PennTreebankLanguagePack().GetBasicCategoryFunction();

        public static readonly IHeadFinder DEFAULT_HEAD_FINDER = new CollinsHeadFinder();

        private readonly AbstractTreebankLanguagePack.BasicCategoryStringFunction basicCatFunction;
        private readonly IHeadFinder headFinder;

        private readonly List<Tuple<string, string>> macros =
            new List<Tuple<string, string>>();

        public static readonly TregexPatternCompiler defaultCompiler =
            new TregexPatternCompiler();

        public TregexPatternCompiler() :
            this(DEFAULT_HEAD_FINDER, DEFAULT_BASIC_CAT_FUNCTION)
        {
        }

        /// <summary>
        /// A compiler that uses this basicCatFunction and the default HeadFinder.
        /// </summary>
        /// <param name="basicCatFunction">the function mapping strings to Strings</param>
        public TregexPatternCompiler(AbstractTreebankLanguagePack.BasicCategoryStringFunction basicCatFunction) :
            this(DEFAULT_HEAD_FINDER, basicCatFunction)
        {
        }

        /// <summary>
        /// A compiler that uses this HeadFinder and the default basicCategoryFunction
        /// </summary>
        /// <param name="headFinder">the HeadFinder</param>
        public TregexPatternCompiler(IHeadFinder headFinder) :
            this(headFinder, DEFAULT_BASIC_CAT_FUNCTION)
        {
        }

        /// <summary>
        /// A compiler that uses this HeadFinder and this basicCategoryFunction
        /// </summary>
        /// <param name="headFinder">the HeadFinder</param>
        /// <param name="basicCatFunction">The function mapping strings to strings</param>
        public TregexPatternCompiler(IHeadFinder headFinder,
            AbstractTreebankLanguagePack.BasicCategoryStringFunction basicCatFunction)
        {
            this.headFinder = headFinder;
            this.basicCatFunction = basicCatFunction;
        }

        // TODO [cdm 2013]: Provide an easy way to do Matcher.quoteReplacement(): This would be quite useful, since the replacement will often contain $ or \
        /// <summary>
        /// Define a macro for rewriting a pattern in any tregex expression compiled
        /// by this compiler. The semantics of this is that all instances of the
        /// original in the pattern are replaced by the replacement, using exactly
        /// the semantics of string.replaceAll(original, replacement) and the
        /// result will then be compiled by the compiler. As such, note that a
        /// macro can replace any part of a tregex expression, in a syntax
        /// insensitive way.  Here's an example:
        /// {@code tpc.addMacro("FINITE_BE_AUX", "/^(?i:am|is|are|was|were)$/");}
        /// </summary>
        /// <param name="original">
        /// The string to match; becomes the first argument of a string.replaceAll()
        /// </param>
        /// <param name="replacement">
        /// The replacement String; becomes the second argument of a string.replaceAll()
        /// </param>
        public void AddMacro(string original, string replacement)
        {
            macros.Add(new Tuple<string, string>(original, replacement));
        }

        /// <summary>
        /// Create a TregexPattern from this tregex string using the headFinder and
        /// basicCat function this TregexPatternCompiler was created with.
        /// Implementation note: If there is an invalid token in the Tregex
        /// parser, JavaCC will throw a TokenMgrError.  This is a class
        /// that extends Error, not Exception (OMG! - bad!), and so rather than
        /// requiring clients to catch it, we wrap it in a ParseException.
        /// (The original Error's are thrown in TregexParserTokenManager.)
        /// </summary>
        /// <param name="tregex">The pattern to parse</param>
        /// <returns>A new TregexPattern object based on this string</returns>
        /// <exception cref="TregexParseException">If the expression is syntactically invalid</exception>
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