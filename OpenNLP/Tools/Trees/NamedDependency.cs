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
    /// An individual dependency between a head and a dependent.
    /// The head and dependent are represented as a Label.
    /// For example, these can be a Word or a WordTag.
    /// If one wishes the dependencies to preserve positions in a sentence, 
    /// then each can be a NamedConstituent. 
    /// 
    /// @author Christopher Manning
    /// @author Spence Green
    /// 
    /// Code ...
    /// </summary>
    public class NamedDependency : UnnamedDependency
    {
        private readonly Object _name;

        public NamedDependency(string regent, string dependent, Object name) :
            base(regent, dependent)
        {
            this._name = name;
        }

        public NamedDependency(ILabel regent, ILabel dependent, Object name) :
            base(regent, dependent)
        {
            this._name = name;
        }

        public override Object Name()
        {
            return _name;
        }

        public override int GetHashCode()
        {
            return RegentText.GetHashCode() ^ DependentText.GetHashCode() ^ _name.GetHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            else if (!(o is NamedDependency))
            {
                return false;
            }
            var d = (NamedDependency) o;
            return EqualsIgnoreName(o) && _name.Equals(d._name);
        }

        public override string ToString()
        {
            return string.Format("{0} --{1}--> {2}", RegentText, _name, DependentText);
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
                    return "  <dep>\n    <governor>" + XmlUtils.XmlEscape(Governor().Value()) +
                           "</governor>\n    <dependent>" + XmlUtils.XmlEscape(Dependent().Value()) +
                           "</dependent>\n  </dep>";
                case "predicate":
                    return "dep(" + Governor() + "," + Dependent() + "," + Name() + ")";
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
            public static readonly IDependencyFactory df = new NamedDependencyFactory();
        }

        /// <summary>
        /// A <code>DependencyFactory</code> acts as a factory for creating objects
        /// of class <code>Dependency</code>
        /// </summary>
        private /*static */ class NamedDependencyFactory : IDependencyFactory
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
                return new NamedDependency(regent, dependent, name);
            }
        }
    }
}