using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /** Tsurgeon provides a way of editing trees based on a set of operations that
 *  are applied to tree locations matching a tregex pattern.
 *  A simple example from the command-line:
 *  <blockquote>
 * java edu.stanford.nlp.trees.tregex.tsurgeon.Tsurgeon -treeFile atree
 *        exciseNP renameVerb
 * </blockquote>
 * The file {@code atree} has Penn Treebank (S-expression) format trees.
 * The other (here, two) files have Tsurgeon operations.  These consist of
 * a list of pairs of a tregex expression on one or more
 * lines, a blank line, and then some number of lines of Tsurgeon operations and then
 * another blank line.
 * <p>
 * Tsurgeon uses the Tregex engine to match tree patterns on trees;
 * for more information on Tregex's tree-matching functionality,
 * syntax, and semantics, please see the documentation for the
 * {@link TregexPattern} class.
 * <p>

 * If you want to use Tsurgeon as an API, the relevant method is
 * {@link #processPattern}.  You will also need to look at the
 * {@link TsurgeonPattern} class and the {@link Tsurgeon#parseOperation} method.
 * <p>
 * Here's the simplest form of invocation on a single Tree:
 * <pre>
 * Tree t = Tree.valueOf("(ROOT (S (NP (NP (NNP Bank)) (PP (IN of) (NP (NNP America)))) (VP (VBD called)) (. .)))");
 * TregexPattern pat = TregexPattern.compile("NP &lt;1 (NP &lt;&lt; Bank) &lt;2 PP=remove");
 * TsurgeonPattern surgery = Tsurgeon.parseOperation("excise remove remove");
 * Tsurgeon.processPattern(pat, surgery, t).pennPrint();
 * </pre>
 * <p>
 * Here is another sample invocation:
 * <pre>
 * TregexPattern matchPattern = TregexPattern.compile("SQ=sq &lt; (/^WH/ $++ VP)");
 * List&lt;TsurgeonPattern&gt; ps = new ArrayList&lt;TsurgeonPattern&gt;();
 *
 * TsurgeonPattern p = Tsurgeon.parseOperation("relabel sq S");
 *
 * ps.add(p);
 *
 * Treebank lTrees;
 * List&lt;Tree&gt; result = Tsurgeon.processPatternOnTrees(matchPattern,Tsurgeon.collectOperations(ps),lTrees);
 * </pre>
 * <p>
 * <i>Note:</i> If you want to apply multiple surgery patterns, you
 * will not want to call processPatternOnTrees, for each individual
 * pattern.  Rather, you should either call processPatternsOnTree and
 * loop through the trees yourself, or, as above, use
 * <code>collectOperations</code> to collect all the surgery patterns
 * into one TsurgeonPattern, and then to call processPatternOnTrees.
 * Either of these latter methods is much faster.
 * </p><p>
 * The parser also has the ability to collect multiple
 * TsurgeonPatterns into one pattern by itself by enclosing each
 * pattern in <code>[ ... ]</code>.  For example,
 * <br>
 * <code>Tsurgeon.parseOperation("[relabel foo BAR] [prune bar]")</code>
 * </p><p>
 * For more information on using Tsurgeon from the command line,
 * see the {@link #main} method and the package Javadoc.
 *
 * @author Roger Levy
 */

    public class Tsurgeon
    {
        private static readonly bool DEBUG = false;
        private static bool verbose; // = false;

        private static readonly Regex emptyLinePattern = new Regex("^\\s*$", RegexOptions.Compiled);
        private static readonly string commentIntroducingCharacter = "%";
        private static readonly Regex commentPattern = new Regex("(?<!\\\\)%.*$", RegexOptions.Compiled);

        private static readonly Regex escapedCommentCharacterPattern = new Regex("\\\\" + commentIntroducingCharacter,
            RegexOptions.Compiled);

        private Tsurgeon()
        {
        } // not an instantiable class
        
        /**
   * Parses a tsurgeon script text input and compiles a tregex pattern and a list
   * of tsurgeon operations into a pair.
   *
   * @param reader Reader to read patterns from
   * @return A pair of a tregex and tsurgeon pattern read from a file, or <code>null</code>
   *    when the operations in the Reader have been exhausted
   * @throws IOException If any IO problem
   */
        /*public static Tuple<TregexPattern, TsurgeonPattern> getOperationFromReader(BufferedReader reader, TregexPatternCompiler compiler) /*throws IOException#1# {
    string patternString = getTregexPatternFromReader(reader);
    // System.err.println("Read tregex pattern: " + patternString);
    if ("".equals(patternString)) {
      return null;
    }
    TregexPattern matchPattern = compiler.compile(patternString);

    TsurgeonPattern collectedPattern = getTsurgeonOperationsFromReader(reader);
    return new Pair<TregexPattern,TsurgeonPattern>(matchPattern,collectedPattern);
  }*/

        /**
   * Assumes that we are at the beginning of a tsurgeon script file and gets the string for the
   * tregex pattern leading the file
   * @return tregex pattern string
   */
        /*public static string getTregexPatternFromReader(BufferedReader reader) throws IOException {
    StringBuilder matchString = new StringBuilder();
    for (string thisLine; (thisLine = reader.readLine()) != null; ) {
      if (matchString.length() > 0 && emptyLinePattern.matcher(thisLine).matches()) {
        // A blank line after getting some real content (not just comments or nothing)
        break;
      }
      Matcher m = commentPattern.matcher(thisLine);
      if (m.matches()) {
        // delete it
        thisLine = m.replaceFirst("");
      }
      if ( ! emptyLinePattern.matcher(thisLine).matches()) {
        matchString.append(thisLine);
      }
    }
    return matchString.ToString();
  }*/

        /**
   * Assumes the given reader has only tsurgeon operations (not a tregex pattern), and parses
   * these out, collecting them into one operation.  Stops on a whitespace line.
   *
   * @throws IOException
   */
        /*public static TsurgeonPattern getTsurgeonOperationsFromReader(BufferedReader reader) throws IOException {
    List<TsurgeonPattern> operations = new ArrayList<TsurgeonPattern>();
    for (string thisLine; (thisLine = reader.readLine()) != null; ) {
      if (emptyLinePattern.matcher(thisLine).matches()) {
        break;
      }
      thisLine = removeComments(thisLine);
      if (emptyLinePattern.matcher(thisLine).matches()) {
        continue;
      }
      // System.err.println("Read tsurgeon op: " + thisLine);
      operations.add(parseOperation(thisLine));
    }

    if (operations.size() == 0)
      throw new TsurgeonParseException("No Tsurgeon operation provided.");

    return collectOperations(operations);
  }*/


        /*private static string removeComments(string line) {
    Matcher m = commentPattern.matcher(line);
    line = m.replaceFirst("");
    Matcher m1 = escapedCommentCharacterPattern.matcher(line);
    line = m1.replaceAll(commentIntroducingCharacter);
    return line;
  }*/


        /**
   * Assumes the given reader has only tsurgeon operations (not a tregex pattern), and returns
   * them as a String, mirroring the way the strings appear in the file. This is helpful
   * for lazy evaluation of the operations, as in a GUI,
   * because you do not parse the operations on load.  Comments are still excised.
   * @throws IOException
   */
        /*public static string getTsurgeonTextFromReader(BufferedReader reader) throws IOException {
    StringBuilder sb = new StringBuilder();
    for (string thisLine; (thisLine = reader.readLine()) != null; ) {
      thisLine = removeComments(thisLine);
      if (emptyLinePattern.matcher(thisLine).matches()) {
        continue;
      }
      sb.append(thisLine);
      sb.append('\n');
    }
    return sb.ToString();
  }*/

        /**
   * Parses a tsurgeon script file and compiles all operations in the file into a list
   * of pairs of tregex and tsurgeon patterns.
   *
   * @param filename file containing the tsurgeon script
   * @return A pair of a tregex and tsurgeon pattern read from a file
   * @throws IOException If there is any I/O problem
   */
        /*public static List<Pair<TregexPattern, TsurgeonPattern>> getOperationsFromFile(string filename, string encoding, TregexPatternCompiler compiler) throws IOException {
    List<Pair<TregexPattern,TsurgeonPattern>> operations = new ArrayList<Pair<TregexPattern, TsurgeonPattern>>();
    BufferedReader reader = new BufferedReader(new InputStreamReader(new FileInputStream(filename), encoding));
    for ( ; ; ) {
      Pair<TregexPattern, TsurgeonPattern> operation = getOperationFromReader(reader, compiler);
      if (operation == null) {
        break;
      }
      operations.add(operation);
    }
    reader.close();
    return operations;
  }*/

        /**
   * Applies {#processPattern} to a collection of trees.
   * @param matchPattern A {@link TregexPattern} to be matched against a {@link Tree}.
   * @param p A {@link TsurgeonPattern} to apply.
   * @param inputTrees The input trees to be processed
   * @return A List of the transformed trees
   */

        public static List<Tree> processPatternOnTrees(TregexPattern matchPattern, TsurgeonPattern p,
            List<Tree> inputTrees)
        {
            List<Tree> result = new List<Tree>();
            foreach (Tree tree in inputTrees)
                result.Add(processPattern(matchPattern, p, tree));
            return result;
        }

        /**
   * Tries to match a pattern against a tree.  If it succeeds, apply the surgical operations contained in a {@link TsurgeonPattern}.
   * @param matchPattern A {@link TregexPattern} to be matched against a {@link Tree}.
   * @param p A {@link TsurgeonPattern} to apply.
   * @param t the {@link Tree} to match against and perform surgery on.
   * @return t, which has been surgically modified.
   */

        public static Tree processPattern(TregexPattern matchPattern, TsurgeonPattern p, Tree t)
        {
            TregexMatcher m = matchPattern.Matcher(t);
            TsurgeonMatcher tsm = p.matcher();
            while (m.Find())
            {
                t = tsm.evaluate(t, m);
                if (t == null)
                {
                    break;
                }
                m = matchPattern.Matcher(t);
            }
            return t;
        }

        private static bool matchedOnTree; // hack-in field for seeing whether there was a match.

        public static Tree processPatternsOnTree(List<Tuple<TregexPattern, TsurgeonPattern>> ops, Tree t)
        {
            matchedOnTree = false;
            foreach (Tuple<TregexPattern, TsurgeonPattern> op in ops)
            {
                try
                {
                    /*if (DEBUG) {
          System.err.println("Running pattern " + op.first());
        }*/
                    TregexMatcher m = op.Item1.Matcher(t);
                    TsurgeonMatcher tsm = op.Item2.matcher();
                    while (m.Find())
                    {
                        matchedOnTree = true;
                        t = tsm.evaluate(t, m);
                        if (t == null)
                        {
                            return null;
                        }
                        m = op.Item1.Matcher(t);
                    }
                }
                catch (NullReferenceException npe)
                {
                    throw new SystemException(
                        "Tsurgeon.processPatternsOnTree failed to match label for pattern: " + op.Item1 + ", " +
                        op.Item2, npe);
                }
            }
            return t;
        }



        /**
   * Parses an operation string into a {@link TsurgeonPattern}.  Throws an {@link TsurgeonParseException} if
   * the operation string is ill-formed.
   * <p>
   * Example of use:
   * <p>
   * <tt>
   * TsurgeonPattern p = Tsurgeon.parseOperation("prune ed");
   * </tt>
   * @param operationString The operation to perform, as a text string
   * @return the operation pattern.
   */

        public static TsurgeonPattern parseOperation(string operationString)
        {
            try
            {
                TsurgeonParser parser =
                    new TsurgeonParser(new StringReader(operationString + "\n"));
                return parser.Root();
            }
            catch (ParseException e)
            {
                throw new TsurgeonParseException("Error parsing Tsurgeon expression: " +
                                                 operationString, e);
            }
            catch (TokenMgrException e)
            {
                throw new TsurgeonParseException("Error parsing Tsurgeon expression: " +
                                                 operationString, e);
            }
        }

        /**
   * Collects a list of operation patterns into a sequence of operations to be applied.  Required to keep track of global properties
   * across a sequence of operations.  For example, if you want to insert a named node and then coindex it with another node,
   * you will need to collect the insertion and coindexation operations into a single TsurgeonPattern so that tsurgeon is aware
   * of the name of the new node and coindexation becomes possible.
   * @param patterns a list of {@link TsurgeonPattern} operations that you want to collect together into a single compound operation
   * @return a new {@link TsurgeonPattern} that performs all the operations in the sequence of the <code>patterns</code> argument
   */

        public static TsurgeonPattern collectOperations(List<TsurgeonPattern> patterns)
        {
            return new TsurgeonPatternRoot(patterns.ToArray());
            //return new TsurgeonPatternRoot(patterns.toArray(new TsurgeonPattern[patterns.size()]));
        }
    }
}