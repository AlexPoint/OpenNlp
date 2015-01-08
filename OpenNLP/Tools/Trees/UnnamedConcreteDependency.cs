using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// An individual dependency between a head and a dependent. The dependency 
    /// is associated with the token indices of the lexical items. 
    /// A key difference between this class and UnnamedDependency is the equals() method. 
    /// Equality of two UnnamedConcreteDependency objects is defined solely 
    /// with respect to the indices. The surface forms are not considered. 
    /// This permits a use case in which dependencies in two different parse trees have slightly different 
    /// pre-processing, possibly due to pre-processing.
    /// 
    /// @author Spence Green
    /// 
    /// Code..
    /// </summary>
    public class UnnamedConcreteDependency : UnnamedDependency
    {
        private readonly int headIndex;
        private readonly int depIndex;

        public UnnamedConcreteDependency(string regent, int regentIndex, string dependent, int dependentIndex) :
            base(regent, dependent)
        {

            headIndex = regentIndex;
            depIndex = dependentIndex;
        }

        public UnnamedConcreteDependency(ILabel regent, int regentIndex, ILabel dependent, int dependentIndex) :
            base(regent, dependent)
        {

            headIndex = regentIndex;
            depIndex = dependentIndex;
        }

        public UnnamedConcreteDependency(ILabel regent, ILabel dep) :
            base(regent, dep)
        {

            if (Governor() is IHasIndex)
            {
                headIndex = ((IHasIndex) Governor()).Index();
            }
            else
            {
                throw new ArgumentException("Label argument lacks IndexAnnotation.");
            }
            if (Dependent() is IHasIndex)
            {
                depIndex = ((IHasIndex) Dependent()).Index();
            }
            else
            {
                throw new ArgumentException("Label argument lacks IndexAnnotation.");
            }
        }

        public int GetGovernorIndex()
        {
            return headIndex;
        }

        public int GetDependentIndex()
        {
            return depIndex;
        }

        public override int GetHashCode()
        {
            return headIndex*(depIndex << 16);
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            else if (!(o is UnnamedConcreteDependency))
            {
                return false;
            }
            var d = (UnnamedConcreteDependency) o;
            return headIndex == d.headIndex && depIndex == d.depIndex;
        }

        public override string ToString()
        {
            string headWord = GetText(Governor());
            string depWord = GetText(Dependent());
            return string.Format("{0} [{1}] --> {2} [{3}]", headWord, headIndex, depWord, depIndex);
        }

        /// <summary>
        /// Provide different printing options via a string keyword.
        /// The recognized options are currently "xml", and "predicate".
        /// Otherwise the default ToString() is used.
        /// </summary>
        public override string ToString(string format)
        {
            switch (format)
            {
                case "xml":
                    string govIdxStr = " idx=\"" + headIndex + "\"";
                    string depIdxStr = " idx=\"" + depIndex + "\"";
                    return "  <dep>\n    <governor" + govIdxStr + ">" + XmlUtils.XmlEscape(Governor().Value()) +
                           "</governor>\n    <dependent" + depIdxStr + ">" + XmlUtils.XmlEscape(Dependent().Value()) +
                           "</dependent>\n  </dep>";
                case "predicate":
                    return "dep(" + Governor() + "," + Dependent() + ")";
                default:
                    return ToString();
            }
        }

        public override IDependencyFactory DependencyFactory()
        {
            return DependencyFactoryHolder.df;
        }

        public new static IDependencyFactory Factory()
        {
            return DependencyFactoryHolder.df;
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class DependencyFactoryHolder
        {
            public static readonly IDependencyFactory df = new UnnamedConcreteDependencyFactory();
        }

        /// <summary>
        /// A <code>DependencyFactory</code> acts as a factory for creating objects
        /// of class <code>Dependency</code>
        /// </summary>
        private class UnnamedConcreteDependencyFactory : IDependencyFactory
        {
            /// <summary>
            /// Create a new <code>Dependency</code>.
            /// </summary>
            public IDependency<ILabel, ILabel, Object> NewDependency(ILabel regent, ILabel dependent)
            {
                return NewDependency(regent, dependent, null);
            }

            /// <summary>
            /// Create a new <code>Dependency</code>.
            /// </summary>
            public IDependency<ILabel, ILabel, Object> NewDependency(ILabel regent, ILabel dependent, Object name)
            {
                return new UnnamedConcreteDependency(regent, dependent);
            }
        }
    }
}