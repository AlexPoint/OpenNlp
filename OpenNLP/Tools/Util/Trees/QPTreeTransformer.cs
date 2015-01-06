using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Trees.TRegex;
using OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * Transforms an English structure parse tree in order to get the dependencies right:
 * Adds an extra structure in QP phrases:
 * <br>
 * (QP (RB well) (IN over) (CD 9)) becomes
 * <br>
 * (QP (XS (RB well) (IN over)) (CD 9))
 * <br>
 * (QP (...) (CC ...) (...)) becomes
 * <br>
 * (QP (NP ...) (CC ...) (NP ...))
 *
 *
 * @author mcdm
 */

    public class QPTreeTransformer : TreeTransformer
    {
        /**
   * Right now (Jan 2013) we only deal with the following QP structures:
   * <ul>
   * <li> NP (QP ...) (QP (CC and/or) ...)
   * <li> QP (RB IN CD|DT ...)   well over, more than
   * <li> QP (JJR IN CD|DT ...)  fewer than
   * <li> QP (IN JJS CD|DT ...)  at least
   * <li> QP (... CC ...)        between 5 and 10
   * </ul>
   *
   * @param t tree to be transformed
   * @return The tree t with an extra layer if there was a QP structure matching the ones mentioned above
   */
        //@Override
        public Tree TransformTree(Tree t)
        {
            return QpTransform(t);
        }


        private static readonly TregexPattern flattenNPoverQPTregex =
            TregexPattern.Compile("NP < (QP=left $+ (QP=right < CC))");

        private static readonly TsurgeonPattern flattenNPoverQPTsurgeon =
            Tsurgeon.parseOperation("[createSubtree QP left right] [excise left left] [excise right right]");

        private static readonly TregexPattern multiwordXSTregex =
            // TODO: should add NN and $ to the numeric expressions captured
            //   NN is for words such as "half" which are probably misparsed
            // TODO: <3 (IN < as|than) is to avoid one weird case in PTB,
            // "more than about".  Perhaps there is some way to generalize this
            // TODO: "all but X"
            // TODO: "all but about X"
            TregexPattern.Compile(
                "QP <1 /^RB|JJ|IN/=left [ ( <2 /^JJ|IN/=right <3 /^CD|DT/ ) | ( <2 /^JJ|IN/ <3 ( IN=right < /^(?i:as|than)$/ ) <4 /^CD|DT/ ) ] ");

        private static readonly TsurgeonPattern multiwordXSTsurgeon =
            Tsurgeon.parseOperation("createSubtree XS left right");

        // the old style split any flat QP with a CC in the middle
        // TOD: there should be some allowances for phrases such as "or more", "or so", etc
        private static readonly TregexPattern splitCCTregex =
            TregexPattern.Compile(
                "QP < (CC $- __=r1 $+ __=l2 ?$-- /^[$]|CC$/=lnum ?$++ /^[$]|CC$/=rnum) <1 __=l1 <- __=r2 !< (__ < (__ < __))");

        private static readonly TsurgeonPattern splitCCTsurgeon =
            Tsurgeon.parseOperation(
                "[if exists lnum createSubtree QP l1 r1] [if not exists lnum createSubtree NP l1 r1] " +
                "[if exists rnum createSubtree QP l2 r2] [if not exists rnum createSubtree NP l2 r2]");

        private static readonly TregexPattern splitMoneyTregex =
            TregexPattern.Compile("QP < (/^[$]$/ !$++ /^(?!([$]|CD)).*$/ !$++ (__ < (__ < __)) $+ __=left) <- __=right");

        private static readonly TsurgeonPattern splitMoneyTsurgeon =
            Tsurgeon.parseOperation("createSubtree QP left right");

        /**
   * Transforms t if it contains one of the following QP structure:
   * <ul>
   * <li> NP (QP ...) (QP (CC and/or) ...)
   * <li> QP (RB IN CD|DT ...)   well over, more than
   * <li> QP (JJR IN CD|DT ...)  fewer than
   * <li> QP (IN JJS CD|DT ...)  at least
   * <li> QP (... CC ...)        between 5 and 10
   * </ul>
   *
   * @param t a tree to be transformed
   * @return t transformed
   */

        public static Tree QpTransform(Tree t)
        {
            t = Tsurgeon.processPattern(flattenNPoverQPTregex, flattenNPoverQPTsurgeon, t);
            t = Tsurgeon.processPattern(multiwordXSTregex, multiwordXSTsurgeon, t);
            t = Tsurgeon.processPattern(splitCCTregex, splitCCTsurgeon, t);
            t = Tsurgeon.processPattern(splitMoneyTregex, splitMoneyTsurgeon, t);
            return t;
        }

    }
}