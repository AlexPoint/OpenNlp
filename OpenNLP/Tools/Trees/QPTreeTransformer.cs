using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Trees.TRegex;
using OpenNLP.Tools.Trees.TRegex.Tsurgeon;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// Transforms an English structure parse tree in order to get the dependencies right:
    /// Adds an extra structure in QP phrases:
    /// (QP (RB well) (IN over) (CD 9)) becomes
    /// (QP (XS (RB well) (IN over)) (CD 9))
    /// (QP (...) (CC ...) (...)) becomes
    /// (QP (NP ...) (CC ...) (NP ...))
    /// 
    /// @author mcdm
    /// 
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class QpTreeTransformer : ITreeTransformer
    {

        /// <summary>
        /// Right now (Jan 2013) we only deal with the following QP structures:
        /// <ul>
        /// <li> NP (QP ...) (QP (CC and/or) ...)</li>
        /// <li> QP (RB IN CD|DT ...)   well over, more than</li>
        /// <li> QP (JJR IN CD|DT ...)  fewer than</li>
        /// <li> QP (IN JJS CD|DT ...)  at least</li>
        /// <li> QP (... CC ...)        between 5 and 10</li>
        /// </ul>
        /// </summary>
        /// <param name="t">tree to be transformed</param>
        /// <returns>
        /// The tree t with an extra layer if there was a QP structure matching the ones mentioned above
        /// </returns>
        public Tree TransformTree(Tree t)
        {
            return QpTransform(t);
        }


        private static readonly TregexPattern FlattenNpOverQpTregex =
            TregexPattern.Compile("NP < (QP=left $+ (QP=right < CC))");

        private static readonly TsurgeonPattern FlattenNpOverQpTsurgeon =
            Tsurgeon.ParseOperation("[createSubtree QP left right] [excise left left] [excise right right]");

        private static readonly TregexPattern MultiwordXsTregex =
            // TODO: should add NN and $ to the numeric expressions captured
            //   NN is for words such as "half" which are probably misparsed
            // TODO: <3 (IN < as|than) is to avoid one weird case in PTB,
            // "more than about".  Perhaps there is some way to generalize this
            // TODO: "all but X"
            // TODO: "all but about X"
            TregexPattern.Compile(
                "QP <1 /^RB|JJ|IN/=left [ ( <2 /^JJ|IN/=right <3 /^CD|DT/ ) | ( <2 /^JJ|IN/ <3 ( IN=right < /^(?i:as|than)$/ ) <4 /^CD|DT/ ) ] ");

        private static readonly TsurgeonPattern MultiwordXsTsurgeon =
            Tsurgeon.ParseOperation("createSubtree XS left right");

        // the old style split any flat QP with a CC in the middle
        // TOD: there should be some allowances for phrases such as "or more", "or so", etc
        private static readonly TregexPattern SplitCcTregex =
            TregexPattern.Compile(
                "QP < (CC $- __=r1 $+ __=l2 ?$-- /^[$]|CC$/=lnum ?$++ /^[$]|CC$/=rnum) <1 __=l1 <- __=r2 !< (__ < (__ < __))");

        private static readonly TsurgeonPattern SplitCcTsurgeon =
            Tsurgeon.ParseOperation(
                "[if exists lnum createSubtree QP l1 r1] [if not exists lnum createSubtree NP l1 r1] " +
                "[if exists rnum createSubtree QP l2 r2] [if not exists rnum createSubtree NP l2 r2]");

        private static readonly TregexPattern SplitMoneyTregex =
            TregexPattern.Compile("QP < (/^[$]$/ !$++ /^(?!([$]|CD)).*$/ !$++ (__ < (__ < __)) $+ __=left) <- __=right");

        private static readonly TsurgeonPattern SplitMoneyTsurgeon =
            Tsurgeon.ParseOperation("createSubtree QP left right");

        /// <summary>
        /// Transforms t if it contains one of the following QP structure:
        /// <ul>
        /// <li> NP (QP ...) (QP (CC and/or) ...)</li>
        /// <li> QP (RB IN CD|DT ...)   well over, more than</li>
        /// <li> QP (JJR IN CD|DT ...)  fewer than</li>
        /// <li> QP (IN JJS CD|DT ...)  at least</li>
        /// <li> QP (... CC ...)        between 5 and 10</li>
        /// </ul>
        /// </summary>
        /// <param name="t">a tree to be transformed</param>
        /// <returns>t transformed</returns>
        public static Tree QpTransform(Tree t)
        {
            t = Tsurgeon.ProcessPattern(FlattenNpOverQpTregex, FlattenNpOverQpTsurgeon, t);
            t = Tsurgeon.ProcessPattern(MultiwordXsTregex, MultiwordXsTsurgeon, t);
            t = Tsurgeon.ProcessPattern(SplitCcTregex, SplitCcTsurgeon, t);
            t = Tsurgeon.ProcessPattern(SplitMoneyTregex, SplitMoneyTsurgeon, t);
            return t;
        }

    }
}