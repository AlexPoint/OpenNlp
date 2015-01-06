using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntPair : IntTuple
    {
        private static readonly long serialVersionUID = 1L;


        public IntPair() : base(2)
        {
        }

        public IntPair(int src, int trgt) : this()
        {
            elements[0] = src;
            elements[1] = trgt;
        }


        /**
   * Return the first element of the pair
   */

        public int getSource()
        {
            return get(0);
        }

        /**
   * Return the second element of the pair
   */

        public int getTarget()
        {
            return get(1);
        }


        //@Override
        public override IntTuple getCopy()
        {
            return new IntPair(elements[0], elements[1]);
        }

        //@Override
        public override bool Equals(Object iO)
        {
            if (!(iO is IntPair))
            {
                return false;
            }
            IntPair i = (IntPair) iO;
            return elements[0] == i.get(0) && elements[1] == i.get(1);
        }

        //@Override
        public override int GetHashCode()
        {
            return elements[0]*17 + elements[1];
        }
    }
}