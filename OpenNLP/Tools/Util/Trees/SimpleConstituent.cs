using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Trees;

namespace OpenNLP.Tools.Util.Trees
{
    /**
 * A <code>SimpleConstituent</code> object defines a generic edge in a graph.
 * The <code>SimpleConstituent</code> records only the endpoints of the
 * <code>Constituent</code>, as two integers.
 * It doesn't label the edges.
 * (It doesn't implement equals() since this actually decreases
 * performance on a non-final class (requires dynamic resolution of which
 * to call).)
 *
 * @author Christopher Manning
 */

    public class SimpleConstituent : Constituent
    {
        /**
   * Left node of edge.
   */
        private int vStart;

        /**
   * End node of edge.
   */
        private int vEnd;


        /**
   * Create an empty <code>SimpleConstituent</code> object.
   */

        public SimpleConstituent()
        {
            // implicitly super();
        }


        /**
   * Create a <code>SimpleConstituent</code> object with given values.
   *
   * @param start start node of edge
   * @param end   end node of edge
   */

        public SimpleConstituent(int start, int end)
        {
            this.vStart = start;
            this.vEnd = end;
        }


        /**
   * access start node.
   */
        //@Override
        public override int start()
        {
            return vStart;
        }


        /**
   * set start node.
   */
        //@Override
        public override void setStart(int start)
        {
            this.vStart = start;
        }


        /**
   * access end node.
   */
        //@Override
        public override int end()
        {
            return vEnd;
        }


        /**
   * set end node.
   */
        //@Override
        public override void setEnd(int end)
        {
            this.vEnd = end;
        }


        /**
   * A <code>SimpleConstituentLabelFactory</code> object makes a
   * <code>StringLabel</code> <code>LabeledScoredConstituent</code>.
   */

        private /*static*/ class SimpleConstituentLabelFactory : LabelFactory
        {

            /**
     * Make a new <code>SimpleConstituent</code>.
     *
     * @param labelStr A string.
     * @return The created label
     */

            public Label newLabel( /*final*/ string labelStr)
            {
                return new SimpleConstituent(0, 0);
            }


            /**
     * Make a new <code>SimpleConstituent</code>.
     *
     * @param labelStr A string.
     * @param options  The options are ignored.
     * @return The created label
     */

            public Label newLabel( /*final*/ string labelStr, /*final */int options)
            {
                return newLabel(labelStr);
            }


            /**
     * Make a new <code>SimpleConstituent</code>.
     *
     * @param labelStr A string.
     * @return The created label
     */

            public Label newLabelFromString( /*final*/ string labelStr)
            {
                return newLabel(labelStr);
            }


            /**
     * Create a new <code>SimpleConstituent</code>.
     *
     * @param oldLabel A <code>Label</code>.
     * @return A new <code>SimpleConstituent</code>
     */

            public Label newLabel(Label oldLabel)
            {
                return new SimpleConstituent(0, 0);
            }

        }


        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class LabelFactoryHolder
        {
            public static readonly LabelFactory lf = new SimpleConstituentLabelFactory();
        }


        /**
   * Return a factory for this kind of label.
   * The factory returned is always the same one (a singleton)
   *
   * @return the label factory
   */

        public override LabelFactory labelFactory()
        {
            return LabelFactoryHolder.lf;
        }


        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class ConstituentFactoryHolder
        {

            /**
     * A <code>SimpleConstituentFactory</code> acts as a factory for
     * creating objects of class <code>SimpleConstituent</code>.
     */

            private /*static*/ class SimpleConstituentFactory : ConstituentFactory
            {

                public Constituent newConstituent(int start, int end)
                {
                    return new SimpleConstituent(start, end);
                }

                public Constituent newConstituent(int start, int end, Label label, double score)
                {
                    return new SimpleConstituent(start, end);
                }

            }

            public static readonly ConstituentFactory cf = new SimpleConstituentFactory();
        }


        /**
   * Return a factory for this kind of constituent.
   * The factory returned is always the same one (a singleton).
   *
   * @return The constituent factory
   */

        public ConstituentFactory constituentFactory()
        {
            return ConstituentFactoryHolder.cf;
        }


        /**
   * Return a factory for this kind of constituent.
   * The factory returned is always the same one (a singleton).
   *
   * @return The constituent factory
   */

        public static ConstituentFactory factory()
        {
            return ConstituentFactoryHolder.cf;
        }
    }
}