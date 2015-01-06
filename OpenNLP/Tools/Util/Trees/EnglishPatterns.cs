using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees
{
    public static class EnglishPatterns
    {
        public static readonly string timeWordRegex =
            "/^(?i:Mondays?|Tuesdays?|Wednesdays?|Thursdays?|Fridays?|Saturdays?|Sundays?|years?|months?|weeks?|days?|mornings?|evenings?|nights?|January|Jan\\.|February|Feb\\.|March|Mar\\.|April|Apr\\.|May|June|July|August|Aug\\.|September|Sept\\.|October|Oct\\.|November|Nov\\.|December|Dec\\.|today|yesterday|tomorrow|spring|summer|fall|autumn|winter)$/";

        public static readonly string timeWordLotRegex =
            "/^(?i:Mondays?|Tuesdays?|Wednesdays?|Thursdays?|Fridays?|Saturdays?|Sundays?|years?|months?|weeks?|days?|mornings?|evenings?|nights?|January|Jan\\.|February|Feb\\.|March|Mar\\.|April|Apr\\.|May|June|July|August|Aug\\.|September|Sept\\.|October|Oct\\.|November|Nov\\.|December|Dec\\.|today|yesterday|tomorrow|spring|summer|fall|autumn|winter|lot)$/";

        // TODO: remove everything but "to be".  Must do this carefully to
        // make sure we like all the dependency changes that happen
        public static readonly string copularWordRegex =
            "/^(?i:" + string.Join("|", SemanticHeadFinder.CopulaVerbs) + ")$/";

        public static readonly string clausalComplementRegex =
            "/^(?i:seem|seems|seemed|seeming|resemble|resembles|resembled|resembling|become|becomes|became|becoming|remain|remains|remained|remaining)$/";

        // r is for texting r = are
        public static readonly string passiveAuxWordRegex =
            "/^(?i:am|is|are|r|be|being|'s|'re|'m|was|were|been|s|ai|m|art|ar|wase|seem|seems|seemed|seeming|appear|appears|appeared|become|becomes|became|becoming|get|got|getting|gets|gotten|remains|remained|remain)$/";

        public static readonly string beAuxiliaryRegex =
            "/^(?i:am|is|are|r|be|being|'s|'re|'m|was|were|been|s|ai|m|art|ar|wase)$/";

        public static readonly string haveRegex =
            "/^(?i:have|had|has|having|'ve|ve|v|'d|d|hvae|hav|as)$/";

        // private static readonly string stopKeepRegex = "/^(?i:stop|stops|stopped|stopping|keep|keeps|kept|keeping)$/";

        public static readonly string selfRegex =
            "/^(?i:myself|yourself|himself|herself|itself|ourselves|yourselves|themselves)$/";

        public static readonly string xcompVerbRegex =
            "/^(?i:advise|advises|advised|advising|allow|allows|allowed|allowing|ask|asks|asked|asking|beg|begs|begged|begging|demand|demands|demanded|demanding|desire|desires|desired|desiring|expect|expects|expected|expecting|encourage|encourages|encouraged|encouraging|force|forces|forced|forcing|implore|implores|implored|imploring|lobby|lobbies|lobbied|lobbying|order|orders|ordered|ordering|persuade|persuades|persuaded|persuading|pressure|pressures|pressured|pressuring|prompt|prompts|prompted|prompting|require|requires|required|requiring|tell|tells|told|telling|urge|urges|urged|urging)$/";

        // A list of verbs where the answer to a question involving that
        // verb would be a ccomp.  For example, "I know when the train is
        // arriving."  What does the person know?
        public static readonly string ccompVerbRegex =
            "/^(?i:ask|asks|asked|asking|know|knows|knew|knowing|specify|specifies|specified|specifying|tell|tells|told|telling|understand|understands|understood|understanding|wonder|wonders|wondered|wondering)$/";

        // A subset of ccompVerbRegex where you could expect an object and
        // still have a ccomp.  For example, "They told me when ..." can
        // still have a ccomp.  "They know my order when ..." would not
        // expect a ccomp between "know" and the head of "when ..."
        public static readonly string ccompObjVerbRegex =
            "/^(?i:tell|tells|told|telling)$/";

        // TODO: is there some better pattern to look for? We do not have tag information at this point
        public static readonly string RELATIVIZING_WORD_REGEX = "(?i:that|what|which|who|whom|whose)";

        public static readonly Regex RELATIVIZING_WORD_PATTERN = new Regex(RELATIVIZING_WORD_REGEX,
            RegexOptions.Compiled);

        //private EnglishPatterns() {} // static constants
    }
}