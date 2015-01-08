using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /// <summary>
    /// A tuple of int. There are special classes for IntUni, IntPair, IntTriple
    /// and IntQuadruple. The motivation for that was the different hashCode implementations.
    /// By using the static IntTuple.getIntTuple(numElements) one can obtain an
    /// instance of the appropriate sub-class.
    /// 
    /// @author Kristina Toutanova (kristina@cs.stanford.edu)
    /// 
    /// Code ...
    /// </summary>
    [Serializable]
    public class IntTuple : IComparable<IntTuple>
    {
        protected readonly int[] elements;

        public IntTuple(int[] arr)
        {
            elements = arr;
        }

        public IntTuple(int num)
        {
            elements = new int[num];
        }

        /*@Override*/

        public int CompareTo(IntTuple o)
        {
            int commonLen = Math.Min(o.Length(), Length());
            for (int i = 0; i < commonLen; i++)
            {
                int a = Get(i);
                int b = o.Get(i);
                if (a < b) return -1;
                if (b < a) return 1;
            }
            if (o.Length() == Length())
            {
                return 0;
            }
            else
            {
                return (Length() < o.Length()) ? -1 : 1;
            }
        }

        public int Get(int num)
        {
            return elements[num];
        }


        public void Set(int num, int val)
        {
            elements[num] = val;
        }

        public void ShiftLeft()
        {
            var shiftedArray = elements.Skip(1).ToArray();
            for (var i = 0; i < elements.Length; i++)
            {
                if (i < elements.Length - 1)
                {
                    elements[i] = shiftedArray[i];
                }
                else
                {
                    elements[i] = 0;
                }
            }

            /*System.arraycopy(elements, 1, elements, 0, elements.Length - 1);  // the API does guarantee that this works when src and dest overlap, as here
    elements[elements.Length - 1] = 0;*/
        }


        public virtual IntTuple GetCopy()
        {

            IntTuple copy = IntTuple.GetIntTuple(elements.Length); //new IntTuple(numElements);
            for (var i = 0; i < elements.Length; i++)
            {
                copy.elements[i] = elements[i];
            }
            //System.arraycopy(elements, 0, copy.elements, 0, elements.Length);

            return copy;
        }


        public int[] Elems()
        {
            return elements;
        }

        public override bool Equals(Object iO)
        {
            if (!(iO is IntTuple))
            {
                return false;
            }
            var i = (IntTuple) iO;
            if (i.elements.Length != elements.Length)
            {
                return false;
            }
            for (int j = 0; j < elements.Length; j++)
            {
                if (elements[j] != i.Get(j))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;
            foreach (int element in elements)
            {
                sum = sum*17 + element;
            }
            return sum;
        }
        
        public int Length()
        {
            return elements.Length;
        }
        
        public static IntTuple GetIntTuple(int num)
        {
            if (num == 1)
            {
                return new IntUni();
            }
            if ((num == 2))
            {
                return new IntPair();
            }
            if (num == 3)
            {
                return new IntTriple();
            }
            if (num == 4)
            {
                return new IntQuadruple();
            }
            else
            {
                return new IntTuple(num);
            }
        }
        
        public static IntTuple GetIntTuple(List<int> integers)
        {
            IntTuple t = IntTuple.GetIntTuple(integers.Count);
            for (int i = 0; i < t.Length(); i++)
            {
                t.Set(i, integers[i]);
            }
            return t;
        }

        public override string ToString()
        {
            var name = new StringBuilder();
            for (int i = 0; i < elements.Length; i++)
            {
                name.Append(Get(i));
                if (i < elements.Length - 1)
                {
                    name.Append(' ');
                }
            }
            return name.ToString();
        }
        
        public static IntTuple Concat(IntTuple t1, IntTuple t2)
        {
            int n1 = t1.Length();
            int n2 = t2.Length();
            IntTuple res = IntTuple.GetIntTuple(n1 + n2);

            for (int j = 0; j < n1; j++)
            {
                res.Set(j, t1.Get(j));
            }
            for (int i = 0; i < n2; i++)
            {
                res.Set(n1 + i, t2.Get(i));
            }
            return res;
        }
    }
}