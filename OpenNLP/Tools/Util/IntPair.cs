using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntPair : IntTuple
    {
        public IntPair() : base(2)
        {
        }

        public IntPair(int src, int trgt) : this()
        {
            elements[0] = src;
            elements[1] = trgt;
        }
        
        /// <summary>
        /// Return the first element of the pair
        /// </summary>
        public int GetSource()
        {
            return Get(0);
        }

        /// <summary>
        /// Return the second element of the pair
        /// </summary>
        /// <returns></returns>
        public int GetTarget()
        {
            return Get(1);
        }

        public override IntTuple GetCopy()
        {
            return new IntPair(elements[0], elements[1]);
        }

        public override bool Equals(Object iO)
        {
            if (!(iO is IntPair))
            {
                return false;
            }
            var i = (IntPair) iO;
            return elements[0] == i.Get(0) && elements[1] == i.Get(1);
        }

        public override int GetHashCode()
        {
            return elements[0]*17 + elements[1];
        }
    }
}