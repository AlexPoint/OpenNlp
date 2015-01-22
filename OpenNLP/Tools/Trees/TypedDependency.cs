using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Ling;

namespace OpenNLP.Tools.Trees
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
    /// Code retrieved on the Stanford parser and ported to C# (see http://nlp.stanford.edu/software/lex-parser.shtml)
    /// </summary>
    public class TypedDependency : IComparable<TypedDependency>
    {
        // TODO FIXME: these should all be final.  That they are mutable is
        // awful design.  Awful.  It means that underlying data structures
        // can be mutated in ways you don't intend.  For example, there was
        // a time when you could call typedDependenciesCollapsed() and it
        // would change the GrammaticalStructure because of the way that
        // object mutated its TypedDependency objects.
        public GrammaticalRelation Reln { get; set; }
        public  IndexedWord Gov { get; set; }
        public IndexedWord Dep { get; set; }
        public bool Extra { get; set; } // = false; // to code whether the dependency preserves the tree structure or not
        // cdm: todo: remove this field and use typing on reln?  Expand implementation of SEMANTIC_DEPENDENT

        public TypedDependency(GrammaticalRelation reln, IndexedWord gov, IndexedWord dep)
        {
            this.Reln = reln;
            this.Gov = gov;
            this.Dep = dep;
        }

        public TypedDependency(TypedDependency other)
        {
            this.Reln = other.Reln;
            this.Gov = other.Gov;
            this.Dep = other.Dep;
            this.Extra = other.Extra;
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
            if (Reln != null ? !Reln.Equals(typedDep.Reln) : typedDep.Reln != null)
            {
                return false;
            }
            if (Gov != null ? !Gov.Equals(typedDep.Gov) : typedDep.Gov != null)
            {
                return false;
            }
            if (Dep != null ? !Dep.Equals(typedDep.Dep) : typedDep.Dep != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = (Reln != null ? Reln.GetHashCode() : 17);
            result = 29*result + (Gov != null ? Gov.GetHashCode() : 0);
            result = 29*result + (Dep != null ? Dep.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return ToString(CoreLabel.OutputFormat.VALUE_INDEX);
        }

        public string ToString(CoreLabel.OutputFormat format)
        {
            return Reln + "(" + Gov.ToString(format) + ", " + Dep.ToString(format) + ")";
        }

        public int CompareTo(TypedDependency tdArg)
        {
            IndexedWord depArg = tdArg.Dep;
            IndexedWord depThis = this.Dep;
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
            int govIndexArg = tdArg.Gov.Index();
            int govIndexThis = this.Gov.Index();
            if (govIndexThis > govIndexArg)
            {
                return 1;
            }
            else if (govIndexThis < govIndexArg)
            {
                return -1;
            }

            // dependent and governor indices equal, the relation decides
            return this.Reln.CompareTo(tdArg.Reln);
        }
    }
}