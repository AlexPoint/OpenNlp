using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenNLP.Tools.Ling;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees
{
    /// <summary>
    /// An individual dependency between a head and a dependent.
    /// The head and dependent are represented as a Label.
    /// For example, these can be a Word or a WordTag. 
    /// If one wishes the dependencies to preserve positions 
    /// in a sentence, then each can be a LabeledConstituent. 
    /// 
    /// @author Christopher Manning
    /// @author Spence Green
    /// 
    /// Code ...
    /// </summary>
    public class UnnamedDependency : IDependency<ILabel, ILabel, Object>
    {
        // We store the text of the labels separately because it looks like
        // it is possible for an object to request a hash code using itself
        // in a partially reconstructed state when unserializing.  For
        // example, a TreeGraphNode might ask for the hash code of an
        // UnnamedDependency, which then uses an unfilled member of the same
        // TreeGraphNode to get the hash code.  Keeping the text of the
        // labels breaks that possible cycle.
        protected readonly string RegentText;
        protected readonly string DependentText;

        private readonly ILabel _regent;
        private readonly ILabel _dependent;

        public UnnamedDependency(string regent, string dependent)
        {
            if (regent == null || dependent == null)
            {
                throw new ArgumentException("governor or dependent cannot be null");
            }

            var headLabel = new CoreLabel();
            headLabel.SetValue(regent);
            headLabel.SetWord(regent);
            this._regent = headLabel;

            var depLabel = new CoreLabel();
            depLabel.SetValue(dependent);
            depLabel.SetWord(dependent);
            this._dependent = depLabel;

            RegentText = regent;
            DependentText = dependent;
        }

        public UnnamedDependency(ILabel regent, ILabel dependent)
        {
            if (regent == null || dependent == null)
            {
                throw new ArgumentException("governor or dependent cannot be null");
            }
            this._regent = regent;
            this._dependent = dependent;

            RegentText = GetText(regent);
            DependentText = GetText(dependent);
        }

        public ILabel Governor()
        {
            return _regent;
        }

        public ILabel Dependent()
        {
            return _dependent;
        }

        public virtual Object Name()
        {
            return null;
        }

        protected string GetText(ILabel label)
        {
            if (label is IHasWord)
            {
                string word = ((IHasWord) label).GetWord();
                if (word != null)
                {
                    return word;
                }
            }
            return label.Value();
        }

        public override int GetHashCode()
        {
            return RegentText.GetHashCode() ^ DependentText.GetHashCode();
        }

        public override bool Equals(Object o)
        {
            return EqualsIgnoreName(o);
        }

        public bool EqualsIgnoreName(Object o)
        {
            if (this == o)
            {
                return true;
            }
            else if (!(o is UnnamedDependency))
            {
                return false;
            }
            var d = (UnnamedDependency) o;

            string thisHeadWord = RegentText;
            string thisDepWord = DependentText;
            string headWord = d.RegentText;
            string depWord = d.DependentText;

            return thisHeadWord.Equals(headWord) && thisDepWord.Equals(depWord);
        }

        public override string ToString()
        {
            return string.Format("{0} --> {1}", RegentText, DependentText);
        }

        /**
           * Provide different printing options via a string keyword.
           * The recognized options are currently "xml", and "predicate".
           * Otherwise the default ToString() is used.
           */

        public virtual string ToString(string format)
        {
            switch (format)
            {
                case "xml":
                    return "  <dep>\n    <governor>" + XmlUtils.XmlEscape(Governor().Value()) +
                           "</governor>\n    <dependent>" + XmlUtils.XmlEscape(Dependent().Value()) +
                           "</dependent>\n  </dep>";
                case "predicate":
                    return "dep(" + Governor() + "," + Dependent() + ")";
                default:
                    return ToString();
            }
        }

        public virtual IDependencyFactory DependencyFactory()
        {
            return DependencyFactoryHolder.df;
        }

        public static IDependencyFactory Factory()
        {
            return DependencyFactoryHolder.df;
        }

        // extra class guarantees correct lazy loading (Bloch p.194)
        private static class DependencyFactoryHolder
        {
            public static readonly IDependencyFactory df = new UnnamedDependencyFactory();
        }

        /// <summary>
        /// A <code>DependencyFactory</code> acts as a factory for creating objects 
        /// of class <code>Dependency</code>
        /// </summary>
        private class UnnamedDependencyFactory : IDependencyFactory
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
                return new UnnamedDependency(regent, dependent);
            }
        }
    }
}