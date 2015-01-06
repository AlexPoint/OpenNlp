using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util.Ling;

namespace OpenNLP.Tools.Util.Trees
{
    [Serializable]
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
            this.preln = other.reln();
            this.pgov = other.gov();
            this.pdep = other.dep();
            this.pextra = other.extra();
        }

        public GrammaticalRelation reln()
        {
            return preln;
        }

        public void setGov(IndexedWord gov)
        {
            this.pgov = gov;
        }

        public void setDep(IndexedWord dep)
        {
            this.pdep = dep;
        }


        public IndexedWord gov()
        {
            return pgov;
        }

        public IndexedWord dep()
        {
            return pdep;
        }

        public bool extra()
        {
            return pextra;
        }

        public void setReln(GrammaticalRelation reln)
        {
            this.preln = reln;
        }

        public void setExtra()
        {
            this.pextra = true;
        }

        //@SuppressWarnings({"RedundantIfStatement"})
        //@Override
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
            /*final*/
            TypedDependency typedDep = (TypedDependency) o;

            if (preln != null ? !preln.Equals(typedDep.reln()) : typedDep.reln() != null)
            {
                return false;
            }
            if (pgov != null ? !pgov.Equals(typedDep.gov()) : typedDep.gov() != null)
            {
                return false;
            }
            if (pdep != null ? !pdep.Equals(typedDep.dep()) : typedDep.dep() != null)
            {
                return false;
            }

            return true;
        }

        //@Override
        public override int GetHashCode()
        {
            int result = (preln != null ? preln.GetHashCode() : 17);
            result = 29*result + (pgov != null ? pgov.GetHashCode() : 0);
            result = 29*result + (pdep != null ? pdep.GetHashCode() : 0);
            return result;
        }

        //@Override
        public override String ToString()
        {
            return toString(CoreLabel.OutputFormat.VALUE_INDEX);
        }

        public String toString(CoreLabel.OutputFormat format)
        {
            return preln + "(" + pgov.toString(format) + ", " + pdep.toString(format) + ")";
        }

        public int CompareTo(TypedDependency tdArg)
        {
            IndexedWord depArg = tdArg.dep();
            IndexedWord depThis = this.dep();
            int indexArg = depArg.index();
            int indexThis = depThis.index();

            if (indexThis > indexArg)
            {
                return 1;
            }
            else if (indexThis < indexArg)
            {
                return -1;
            }

            // dependent indices are equal, check governor
            int govIndexArg = tdArg.gov().index();
            int govIndexThis = this.gov().index();
            if (govIndexThis > govIndexArg)
            {
                return 1;
            }
            else if (govIndexThis < govIndexArg)
            {
                return -1;
            }

            // dependent and governor indices equal, the relation decides
            return this.reln().CompareTo(tdArg.reln());
        }
    }
}