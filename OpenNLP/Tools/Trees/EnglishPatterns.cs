using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// This class contains some English String or Tregex regular expression
    /// patterns. They originated in other classes like
    /// EnglishGrammaticalRelations, but were collected here so that they
    /// could be used without having to load large classes (which we might want
    /// to have parallel versions of.
    /// Some are just stored here as String objects, since they are often used as
    /// sub-patterns inside larger patterns.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public static class EnglishPatterns
    {
        public static readonly string TimeWordRegex =
            "/^(?i:Mondays?|Tuesdays?|Wednesdays?|Thursdays?|Fridays?|Saturdays?|Sundays?|years?|months?|weeks?|days?|mornings?|evenings?|nights?|January|Jan\\.|February|Feb\\.|March|Mar\\.|April|Apr\\.|May|June|July|August|Aug\\.|September|Sept\\.|October|Oct\\.|November|Nov\\.|December|Dec\\.|today|yesterday|tomorrow|spring|summer|fall|autumn|winter)$/";

        public static readonly string TimeWordLotRegex =
            "/^(?i:Mondays?|Tuesdays?|Wednesdays?|Thursdays?|Fridays?|Saturdays?|Sundays?|years?|months?|weeks?|days?|mornings?|evenings?|nights?|January|Jan\\.|February|Feb\\.|March|Mar\\.|April|Apr\\.|May|June|July|August|Aug\\.|September|Sept\\.|October|Oct\\.|November|Nov\\.|December|Dec\\.|today|yesterday|tomorrow|spring|summer|fall|autumn|winter|lot)$/";

        // TODO: remove everything but "to be".  Must do this carefully to
        // make sure we like all the dependency changes that happen
        public static readonly string CopularWordRegex =
            "/^(?i:" + string.Join("|", SemanticHeadFinder.CopulaVerbs) + ")$/";

        public static readonly string ClausalComplementRegex =
            "/^(?i:seem|seems|seemed|seeming|resemble|resembles|resembled|resembling|become|becomes|became|becoming|remain|remains|remained|remaining)$/";

        // r is for texting r = are
        public static readonly string PassiveAuxWordRegex =
            "/^(?i:am|is|are|r|be|being|'s|'re|'m|was|were|been|s|ai|m|art|ar|wase|seem|seems|seemed|seeming|appear|appears|appeared|become|becomes|became|becoming|get|got|getting|gets|gotten|remains|remained|remain)$/";

        public static readonly string BeAuxiliaryRegex =
            "/^(?i:am|is|are|r|be|being|'s|'re|'m|was|were|been|s|ai|m|art|ar|wase)$/";

        public static readonly string HaveRegex =
            "/^(?i:have|had|has|having|'ve|ve|v|'d|d|hvae|hav|as)$/";

        // private static readonly string stopKeepRegex = "/^(?i:stop|stops|stopped|stopping|keep|keeps|kept|keeping)$/";

        public static readonly string SelfRegex =
            "/^(?i:myself|yourself|himself|herself|itself|ourselves|yourselves|themselves)$/";

        public static readonly string XCompVerbRegex =
            "/^(?i:advise|advises|advised|advising|allow|allows|allowed|allowing|ask|asks|asked|asking|beg|begs|begged|begging|demand|demands|demanded|demanding|desire|desires|desired|desiring|expect|expects|expected|expecting|encourage|encourages|encouraged|encouraging|force|forces|forced|forcing|implore|implores|implored|imploring|lobby|lobbies|lobbied|lobbying|order|orders|ordered|ordering|persuade|persuades|persuaded|persuading|pressure|pressures|pressured|pressuring|prompt|prompts|prompted|prompting|require|requires|required|requiring|tell|tells|told|telling|urge|urges|urged|urging)$/";

        /// <summary>
        /// A list of verbs where the answer to a question involving that
        /// verb would be a ccomp.
        /// For example, "I know when the train is arriving."  What does the person know?
        /// </summary>
        public static readonly string CCompVerbRegex =
            "/^(?i:ask|asks|asked|asking|know|knows|knew|knowing|specify|specifies|specified|specifying|tell|tells|told|telling|understand|understands|understood|understanding|wonder|wonders|wondered|wondering)$/";

        /// <summary>
        /// A subset of ccompVerbRegex where you could expect an object and
        /// still have a ccomp.  For example, "They told me when ..." can
        /// still have a ccomp.  "They know my order when ..." would not
        /// expect a ccomp between "know" and the head of "when ..."
        /// </summary>
        public static readonly string CCompObjVerbRegex =
            "/^(?i:tell|tells|told|telling)$/";

        // TODO: is there some better pattern to look for? We do not have tag information at this point
        public static readonly string RelativizingWordRegex = "(?i:that|what|which|who|whom|whose)";

        public static readonly Regex RelativizingWordPattern = new Regex("^" + RelativizingWordRegex + "$", RegexOptions.Compiled);
    }
}