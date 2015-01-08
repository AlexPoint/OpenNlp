using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Trees.TRegex;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Flattens the following two structures:
    /// (NP (NP (NNP Month) (CD Day) )(, ,)(NP (CD Year) ))
    /// becomes
    /// (NP (NNP Month) (CD Day) (, ,) (CD Year) )
    /// (NP (NP (NNP Month) )(NP (CD Year) ))
    /// becomes
    /// (NP (NNP Month) (CD Year))
    /// 
    /// @author John Bauer
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class DateTreeTransformer : ITreeTransformer
    {
        private const string MonthRegex = 
            "January|February|March|April|May|June|July|August|September|October|November|December|Jan\\.|Feb\\.|Mar\\.|Apr\\.|Aug\\.|Sep\\.|Sept\\.|Oct\\.|Nov\\.|Dec\\.";

        private static readonly TregexPattern TregexMonthYear =
            TregexPatternCompiler.defaultCompiler.Compile("NP=root <1 (NP <: (NNP=month <: /" + MonthRegex +
                                                          "/)) <2 (NP=yearnp <: (CD=year <: __)) : =root <- =yearnp");

        private static readonly TregexPattern TregexMonthDayYear =
            TregexPatternCompiler.defaultCompiler.Compile("NP=root <1 (NP=monthdayroot <1 (NNP=month <: /" + MonthRegex +
                                                          "/) <2 (CD=day <: __)) <2 (/^,$/=comma <: /^,$/) <3 (NP=yearroot <: (CD=year <: __)) : (=root <- =yearroot) : (=monthdayroot <- =day)");

        public Tree TransformTree(Tree t)
        {
            TregexMatcher matcher = TregexMonthYear.Matcher(t);
            while (matcher.Find())
            {
                Tree root = matcher.GetNode("root");
                Tree month = matcher.GetNode("month");
                Tree year = matcher.GetNode("year");
                var children = new Tree[] {month, year};
                root.SetChildren(children);
                matcher = TregexMonthYear.Matcher(t);
            }
            matcher = TregexMonthDayYear.Matcher(t);
            while (matcher.Find())
            {
                Tree root = matcher.GetNode("root");
                Tree month = matcher.GetNode("month");
                Tree day = matcher.GetNode("day");
                Tree comma = matcher.GetNode("comma");
                Tree year = matcher.GetNode("year");
                var children = new Tree[] {month, day, comma, year};
                root.SetChildren(children);
                matcher = TregexMonthDayYear.Matcher(t);
            }
            return t;
        }
    }
}