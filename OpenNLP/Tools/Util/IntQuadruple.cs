using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntQuadruple : IntTuple
    {
        public IntQuadruple() : base(4)
        {
        }

        public IntQuadruple(int src, int mid, int trgt, int trgt2) : this()
        {
            elements[0] = src;
            elements[1] = mid;
            elements[2] = trgt;
            elements[3] = trgt2;
        }


        public override IntTuple GetCopy()
        {
            var nT = new IntQuadruple(elements[0], elements[1], elements[2], elements[3]);
            return nT;
        }


        public int GetSource()
        {
            return Get(0);
        }


        public int GetMiddle()
        {
            return Get(1);
        }

        public int GetTarget()
        {
            return Get(2);
        }

        public int GetTarget2()
        {
            return Get(3);
        }
    }
}