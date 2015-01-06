using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IdentityDictionary<T1, T2> : Dictionary<T1, T2>
        where T1 : class
    {
        public IdentityDictionary() : base(new IdentityEqualityComparer<T1>())
        {
        }
    }

    public class IdentityEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public int GetHashCode(T value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }

        public bool Equals(T left, T right)
        {
            return left == right; // Reference identity comparison
        }
    }
}