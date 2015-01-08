using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    /// <summary>
    /// A <code>TypedDependency</code> is a relation between two words in a
    /// <code>GrammaticalStructure</code>.  Each <code>TypedDependency</code>
    /// consists of a governor word, a dependent word, and a relation, which is
    /// normally an instance of {@link GrammaticalRelation
    /// <code>GrammaticalRelation</code>}.
    /// 
    /// @author Bill MacCartney
    /// 
    /// Code...
    /// </summary>
    public class TypedDependency : IComparable<TypedDependency>
    {
        private static readonly long serialVersionUID = -7690294213151279779L;

        // TODO FIXME: these should all be final.  That they are mutable is
        // awful design.  Awful.  It means that underlying data structures
        // can be mutated in ways you don't intend.  For example, there was
        // a time when you could call typedDependenciesCollapsed() and it
        // would change the GrammaticalStructure because of the way that
        // object mutated its TypedDependency objects.
        private GrammaticalRelation preln;
        private IndexedWord pgov;
        private IndexedWord pdep;
        private bool pextra; // = false; // to code whether the dependency preserves the tree structure or not
        // cdm: todo: remove this field and use typing on reln?  Expand implementation of SEMANTIC_DEPENDENT

        public TypedDependency(GrammaticalRelation reln, IndexedWord gov, IndexedWord dep)
        {
            this.preln = reln;
            this.pgov = gov;
            this.pdep = dep;
        }

        public TypedDependency(TypedDependency other)
        {
            this.preln = other.Reln();
            this.pgov = other.Gov();
            this.pdep = other.Dep();
            this.pextra = other.Extra();
        }

        public GrammaticalRelation Reln()
        {
            return preln;
        }

        public void SetGov(IndexedWord gov)
        {
            this.pgov = gov;
        }

        public void SetDep(IndexedWord dep)
        {
            this.pdep = dep;
        }


        public IndexedWord Gov()
        {
            return pgov;
        }

        public IndexedWord Dep()
        {
            return pdep;
        }

        public bool Extra()
        {
            return pextra;
        }

        public void SetReln(GrammaticalRelation reln)
        {
            this.preln = reln;
        }

        public void SetExtra()
        {
            this.pextra = true;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is TypedDependency))
            {
                return false;
            }

            var typedDep = (TypedDependency) o;
            if (preln != null ? !preln.Equals(typedDep.Reln()) : typedDep.Reln() != null)
            {
                return false;
            }
            if (pgov != null ? !pgov.Equals(typedDep.Gov()) : typedDep.Gov() != null)
            {
                return false;
            }
            if (pdep != null ? !pdep.Equals(typedDep.Dep()) : typedDep.Dep() != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = (preln != null ? preln.GetHashCode() : 17);
            result = 29*result + (pgov != null ? pgov.GetHashCode() : 0);
            result = 29*result + (pdep != null ? pdep.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return ToString(CoreLabel.OutputFormat.VALUE_INDEX);
        }

        public string ToString(CoreLabel.OutputFormat format)
        {
            return preln + "(" + pgov.ToString(format) + ", " + pdep.ToString(format) + ")";
        }

        public int CompareTo(TypedDependency tdArg)
        {
            IndexedWord depArg = tdArg.Dep();
            IndexedWord depThis = this.Dep();
            int indexArg = depArg.Index();
            int indexThis = depThis.Index();

            if (indexThis > indexArg)
            {
                return 1;
            }
            else if (indexThis < indexArg)
            {
                return -1;
            }

            // dependent indices are equal, check governor
            int govIndexArg = tdArg.Gov().Index();
            int govIndexThis = this.Gov().Index();
            if (govIndexThis > govIndexArg)
            {
                return 1;
            }
            else if (govIndexThis < govIndexArg)
            {
                return -1;
            }

            // dependent and governor indices equal, the relation decides
            return this.Reln().CompareTo(tdArg.Reln());
        }
    }
}