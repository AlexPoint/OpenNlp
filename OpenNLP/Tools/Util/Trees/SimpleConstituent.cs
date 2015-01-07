using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;
using OpenNLP.Tools.Util.Trees;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A <code>SimpleConstituent</code> object defines a generic edge in a graph.
    /// The <code>SimpleConstituent</code> records only the endpoints of the
    /// <code>Constituent</code>, as two integers.
    /// It doesn't label the edges.
    /// (It doesn't implement equals() since this actually decreases
    /// performance on a non-final class (requires dynamic resolution of which to call).)
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public class SimpleConstituent : Constituent
    {
        /// <summary>
        /// Left node of edge
        /// </summary>
        private int _start;

        /// <summary>
        /// End node of edge
        /// </summary>
        private int _end;

        /// <summary>
        /// Create an empty <code>SimpleConstituent</code> object
        /// </summary>
        public SimpleConstituent()
        {
            // implicitly super();
        }

        /// <summary>
        /// Create a <code>SimpleConstituent</code> object with given values
        /// </summary>
        /// <param name="start">start node of edge</param>
        /// <param name="end">end node of edge</param>
        public SimpleConstituent(int start, int end)
        {
            this._start = start;
            this._end = end;
        }

        /// <summary>
        /// access start node
        /// </summary>
        public override int Start()
        {
            return _start;
        }

        /// <summary>
        /// set start node
        /// </summary>
        public override void SetStart(int start)
        {
            this._start = start;
        }

        /// <summary>
        /// access end node
        /// </summary>
        public override int End()
        {
            return _end;
        }

        /// <summary>
        /// set end node
        /// </summary>
        public override void SetEnd(int end)
        {
            this._end = end;
        }

        /// <summary>
        /// A <code>SimpleConstituentLabelFactory</code> object makes a
        /// <code>StringLabel</code> <code>LabeledScoredConstituent</code>.
        /// </summary>
        private /*static*/ class SimpleConstituentLabelFactory : LabelFactory
        {
            public Label NewLabel( /*final*/ string labelStr)
            {
                return new SimpleConstituent(0, 0);
            }

            public Label NewLabel( /*final*/ string labelStr, /*final */int options)
            {
                return NewLabel(labelStr);
            }

            public Label NewLabelFromString( /*final*/ string labelStr)
            {
                return NewLabel(labelStr);
            }

            public Label NewLabel(Label oldLabel)
            {
                return new SimpleConstituent(0, 0);
            }

        }


        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class LabelFactoryHolder
        {
            public static readonly LabelFactory lf = new SimpleConstituentLabelFactory();
        }

        /// <summary>
        /// Return a factory for this kind of label.
        /// The factory returned is always the same one (a singleton)
        /// </summary>
        /// <returns>the label factory</returns>
        public override LabelFactory LabelFactory()
        {
            return LabelFactoryHolder.lf;
        }


        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class ConstituentFactoryHolder
        {

            /// <summary>
            /// A <code>SimpleConstituentFactory</code> acts as a factory for
            /// creating objects of class <code>SimpleConstituent</code>.
            /// </summary>
            private /*static*/ class SimpleConstituentFactory : ConstituentFactory
            {

                public Constituent NewConstituent(int start, int end)
                {
                    return new SimpleConstituent(start, end);
                }

                public Constituent NewConstituent(int start, int end, Label label, double score)
                {
                    return new SimpleConstituent(start, end);
                }

            }

            public static readonly ConstituentFactory cf = new SimpleConstituentFactory();
        }

        /// <summary>
        /// Return a factory for this kind of constituent.
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>The constituent factory</returns>
        public ConstituentFactory ConstituentFactory()
        {
            return ConstituentFactoryHolder.cf;
        }

        /// <summary>
        /// Return a factory for this kind of constituent.
        /// The factory returned is always the same one (a singleton).
        /// </summary>
        /// <returns>The constituent factory</returns>
        public static ConstituentFactory Factory()
        {
            return ConstituentFactoryHolder.cf;
        }
    }
}