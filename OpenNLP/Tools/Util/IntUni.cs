using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /// <summary>
    /// Just a single integer
    /// 
    /// @author Kristina Toutanova (kristina@cs.stanford.edu)
    /// Code ...
    /// </summary>
    public class IntUni : IntTuple
    {
        public IntUni() : base(1)
        {
        }
        
        public IntUni(int src) : this()
        {
            elements[0] = src;
        }
        
        public int GetSource()
        {
            return elements[0];
        }

        public void SetSource(int src)
        {
            elements[0] = src;
        }

        public override IntTuple GetCopy()
        {
            var nT = new IntUni(elements[0]);
            return nT;
        }

        public void Add(int val)
        {
            elements[0] += val;
        }
    }
}