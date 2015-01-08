using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public class IntTriple : IntTuple
    {
        public IntTriple() : base(3)
        {
        }

        public IntTriple(int src, int mid, int trgt) : this()
        {
            elements[0] = src;
            elements[1] = mid;
            elements[2] = trgt;
        }

        public override IntTuple GetCopy()
        {
            var nT = new IntTriple(elements[0], elements[1], elements[2]);
            return nT;
        }


        public int GetSource()
        {
            return elements[0];
        }

        public int GetTarget()
        {
            return elements[2];
        }

        public int GetMiddle()
        {
            return elements[1];
        }
    }
}